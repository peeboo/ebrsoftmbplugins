using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Net;
using MediaBrowser.Library.Plugins;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using MediaBrowser;
using MediaBrowser.Library.Configuration;
using MediaBrowser.Library;
using MediaBrowser.LibraryManagement;
using MediaBrowser.Library.Threading;

//******************************************************************************************************************
//  This class is the heart of your plug-in.  It is instantiated by the initial loading logic of MediaBrowser.
//
//  For your project to build you need to be sure the reference to the MediaBrowser assembly is current.  More than
//  likely, it is broken in this template as the template cannot copy binaries, plus it would probably be out of date
//  anyway.  Expand the References tree in the Solution Explorer to the right and make sure you have a good reference
//  to a current version of the mediabrowser.dll.
//
//  If you wish for your plug in needs to provide mcml or other resources to MediaBrowser (like a config panel)
//  You need to do the following:
//
//  For your project to load as a MediaBrowser Plug-in you need to build your release .dll and copy it to the ehome
//  directory (under your windows directory). AND ALSO create a .pgn file with the same name as your dll and put it
//  in c:\program data\mediabrowser\plugins.  The Configurator will do this automatically if provided a valid 
//  plugin_info.xml file and location for your .dll
//
//  If you don't need to provide any mcml or other resources you can load as a normal plugin and just place your release
//  .dll in c:\program data\mediabrowser\plugins.
//******************************************************************************************************************

namespace CoverArt
{
    public class Plugin : BasePlugin
    {

        static readonly Guid CoverArtGuid = new Guid("756c2e60-f6f1-4167-b287-3798ddf373c4");

        private string configPath;

        private MyConfigData configData;

        /// <summary>
        /// The Init method is called when the plug-in is loaded by MediaBrowser.  You should perform all your specific initializations
        /// here - including adding your theme to the list of available themes.
        /// </summary>
        /// <param name="kernel"></param>
        public override void Init(Kernel kernel)
        {
            try
            {
                //The AddConfigPanel method will allow you to extend the config page of MediaBrowser with your own options panel.
                //You must create this as an mcml UI that fits within the standard config panel area.  It must take Application and 
                //FocusItem as parameters.  The project template should have generated an example ConfigPage.mcml that you can modify
                //or, if you don't wish to extend the config, remove it and the following call to AddConfigPanel
                //*** FOR THIS TO WORK *** you will need to define InstallGlobally to return "true"

                //un-comment the following lines to provide a config panel for your plugin (you also MUST return true from InstallGlobally)
                //the conditional logic is important as this can only be done from inside MediaCenter
                //bool isMC = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
                //if (isMC) 
                //{                    
                //    config = new MyConfig();
                //    kernel.AddConfigPanel("CoverArt Options", "resx://CoverArt/CoverArt.Resources/ConfigPanel#ConfigPanel", config);
                //    //If you want to add any context menus they need to be inside this logic as well.
                //}
                //else Logger.ReportInfo("Not creating menus for CoverArt.  Appear to not be in MediaCenter.  AppDomain is: " + AppDomain.CurrentDomain.FriendlyName);

                //The AddStringData method will allow you to extend the localized strings used by MediaBrowser with your own.
                //This is useful for adding descriptive text to go along with your theme options.  If you don't have any theme-
                //specific options or other needs to extend the string data, remove the following call.

                //un-comment the following line to provide string data extensions for your plugin
                //kernel.StringData.AddStringData(MyStrings.FromFile(MyStrings.GetFileName("CoverArt-")));

                //Insert our image processor
                kernel.ImageProcessor = ProcessImage;

                //profiles.Add("default", defaultProfile);

                //Load custom profiles here...
                configPath = Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppPluginPath, "Configurations");
                if (!Directory.Exists(configPath)) Directory.CreateDirectory(configPath);
                configData = MyConfigData.FromFile(Path.Combine(configPath, "Coverart.xml"));

                //test
                //ignoreFolders.Add("\\\\mediaserver\\movies\\music");
                //profiles.Add("\\\\mediaserver\\movies\\music", new Profile("CoverArtClearCase", null, null, null, null, null, null));
                //profiles.Add("\\\\mediaserver\\movies\\hd", new Profile("c:\\programdata\\mediabrowser\\plugins\\coverart\\testprofiles\\diamond", null, null, null, null, "c:\\programdata\\mediabrowser\\plugins\\coverart\\testprofiles\\movie", null));
                //test

                Async.Queue("CAPing",()=> ping("http://www.ebrsoft.com/software/mb/plugins/CoverArtPing.php"));
                //Tell the log we loaded.
                Logger.ReportInfo("CoverArt (version " + Version + ") Plug-in Loaded.");
            }
            catch (Exception ex)
            {
                Logger.ReportException("Error initializing CoverArt - probably incompatable MB version", ex);
            }

        }

        private void ping(string path)
        {
            try
            {
                WebRequest request = WebRequest.Create(path);
                var response = request.GetResponse();
                Stream stream = response.GetResponseStream();
            }
            catch { } //just let it go
        }


        public Image ProcessImage(Image rootImage, BaseItem item)
        {
            Logger.ReportInfo("Image for " + item.Name + " being processed by CoverArt. Path is: "+item.Path);
            Image newImage = rootImage;

            bool frameOnTop = false;
            bool justRoundCorners = false;
            bool roundCorners = false;
            bool process = false;

            //be sure path is valid
            if (item.Path == null) {
                Logger.ReportInfo("Not processing Item " + item.Name +" (null path)");
                return rootImage;
            }

            //check for ignore
            if (Ignore(item.Path))
            {
                Logger.ReportInfo("Ignoring Item in " + item.Path);
                return rootImage;
            }

            Profile profile = getProfile(item.Path);
            Rectangle position = new Rectangle(0,0,0,0);
            Image overlay = new Bitmap(Resources.Overlay);

            if (item is Movie)
            {
                process = true;
                overlay = profile.MovieOverlay();
                position = profile.RootPosition("movie");

                if (item.Path.StartsWith("http://"))
                {
                    //remote file - use remote case
                    Logger.ReportInfo("Using remote case art for " + item.Name);
                    newImage = profile.RemoteFrame("default");
                    overlay = profile.RemoteOverlay();
                    position = profile.RootPosition("remote");
                    justRoundCorners = profile.JustRoundCorners("remote");
                    roundCorners = profile.RoundCorners("remote");
                    frameOnTop = profile.FrameOnTop("remote");
                }
                else
                {
                    //check the aspect ratio - if we are a landscape item (like a frame grab) use the thumb profile
                    if (rootImage.Width > rootImage.Height)
                    {
                        newImage = profile.ThumbFrame("default");
                        overlay = profile.ThumbOverlay();
                        position = profile.RootPosition("thumb");
                        frameOnTop = profile.FrameOnTop("thumb");
                        justRoundCorners = profile.JustRoundCorners("thumb");
                        roundCorners = profile.RoundCorners("thumb");
                    }
                    else
                    {

                        frameOnTop = profile.FrameOnTop("movie");
                        justRoundCorners = profile.JustRoundCorners("movie");
                        roundCorners = profile.RoundCorners("movie");
                        //Logger.ReportInfo("Process file " + item.Path);

                        if (Directory.Exists(item.Path))
                        {
                            //we are a directory - probably a file rip of dvd, bd, etc.
                            if (Helper.IsBluRayFolder(item.Path, null))
                            {
                                //blu-ray start with frame as background
                                Logger.ReportInfo("Using bluray case art for " + item.Name);
                                newImage = profile.MovieFrame("BD");
                            }
                            else
                            {
                                if (Helper.IsDvDFolder(item.Path, null, null))
                                {
                                    //dvd
                                    Logger.ReportInfo("Using dvd case art for " + item.Name);
                                    newImage = profile.MovieFrame("DVD");
                                }
                                else
                                    if (Helper.IsHDDVDFolder(item.Path, null))
                                    {
                                        //hd-dvd
                                        Logger.ReportInfo("Using hd-dvd case art for " + item.Name);
                                        newImage = profile.MovieFrame("HDDVD");
                                    }
                                    else newImage = movieFrameBasedOnFileType(item, profile);
                            }
                        }
                        else
                        {
                            //not a directory - maybe a file...?
                            if (File.Exists(item.Path))
                            {
                                newImage = movieFrameBasedOnFileType(item, profile);
                            }
                            else newImage = profile.MovieFrame("default");
                        }
                    }
                }

            }
            else
            {
                if (item is Episode)
                {
                    //apply episode treatment
                    process = true;
                    newImage = profile.EpisodeFrame("default");
                    overlay = profile.EpisodeOverlay();
                    frameOnTop = profile.FrameOnTop("episode");
                    justRoundCorners = profile.JustRoundCorners("episode");
                    position = profile.RootPosition("episode");
                    roundCorners = profile.RoundCorners("episode");
                }
                else
                {
                    if (item is Season)
                    {
                        //apply season treatment
                        process = true;
                        newImage = profile.SeasonFrame("default");
                        overlay = profile.SeasonOverlay();
                        frameOnTop = profile.FrameOnTop("season");
                        justRoundCorners = profile.JustRoundCorners("season");
                        position = profile.RootPosition("season");
                        roundCorners = profile.RoundCorners("season");
                    }
                    else
                    if (item is Series) 
                    {
                        //apply series treatment
                        process = true;
                        newImage = profile.SeriesFrame("default");
                        overlay = profile.SeriesOverlay();
                        frameOnTop = profile.FrameOnTop("series");
                        justRoundCorners = profile.JustRoundCorners("series");
                        position = profile.RootPosition("series");
                        roundCorners = profile.RoundCorners("series");
                    }
                    else
                    {
                        if (item is Folder && rootImage != null)
                        {
                            //see if we are a music folder of some sort
                            if (CAHelper.IsAlbumFolder(item.Path))
                            {
                                //apply album processing
                                process = true;
                                newImage = profile.AlbumFrame("default");
                                overlay = profile.AlbumOverlay();
                                frameOnTop = profile.FrameOnTop("album");
                                justRoundCorners = profile.JustRoundCorners("album");
                                position = profile.RootPosition("album");
                                roundCorners = profile.RoundCorners("album");

                            }
                            else
                            {
                                //just a plain old folder
                                process = true;
                                newImage = profile.FolderFrame("default");
                                overlay = profile.FolderOverlay();
                                frameOnTop = profile.FrameOnTop("folder");
                                justRoundCorners = profile.JustRoundCorners("folder");
                                roundCorners = profile.RoundCorners("folder");
                                position = profile.RootPosition("folder");
                            }
                        }
                    }
                }

            }

            if (process)
            {
                newImage = CreateImage(newImage, rootImage, overlay, position, frameOnTop, roundCorners, justRoundCorners);
            }
            else
            {
                Logger.ReportInfo("No processing applied to image for " + item.Name);
            }

            return newImage;
        }

        public static Image CreateImage(Image newImage, Image rootImage, Image overlay, Rectangle position, bool frameOnTop, bool roundCorners, bool justRoundCorners)
        {
            Graphics work;
            if (!justRoundCorners && (position.Width == 0 || position.Height == 0))
            {
                //use original (basically an ignore)
                return rootImage;
            }
            if (frameOnTop)
            {
                //the frame goes on top so just create a base image of proper size
                Image temp = newImage;
                //no overlay so just create new bitmap of proper size
                newImage = new Bitmap(temp.Width, temp.Height);
                overlay = temp;
            }

            //create graphics object

            if (justRoundCorners)
            {
                newImage = CAHelper.RoundCorners((Bitmap)rootImage, 1.0, 1);
                work = Graphics.FromImage(newImage);
            }
            else
            {
                //then put the root image in it's place
                work = Graphics.FromImage(newImage);
                if (roundCorners)
                {
                    rootImage = CAHelper.RoundCorners((Bitmap)rootImage, 0.41, 1);
                }
                work.DrawImage(rootImage, position);
            }
            //and the overlay
            work.DrawImage(overlay, 0, 0, newImage.Width, newImage.Height);
            work.Dispose();
            //work = null; 
            //overlay = null;
            return newImage;
        }

        private Image movieFrameBasedOnFileType(BaseItem item, Profile profile)
        {
            //Determine type
            Video video = item as Video;
            if (video != null)
            {
                switch (video.MediaType)
                {
                    case MediaType.BluRay:
                        return profile.MovieFrame("BD");
                    case MediaType.Mkv:
                        return profile.MovieFrame("MKV");
                    case MediaType.Wmv:
                        return profile.MovieFrame("WMV");
                    case MediaType.Avi:
                        return profile.MovieFrame("AVI");
                    case MediaType.Mp4:
                        return profile.MovieFrame("MP4");
                    case MediaType.DVRMS:
                        return profile.MovieFrame("DVRMS");
                    default:
                        //couldn't find type - try extension
                        //Logger.ReportInfo("Could not determine file type of " + video.VideoFiles.First().ToLower().Substring(video.VideoFiles.First().Length - 4));
                        return profile.MovieFrame(Path.GetExtension(video.VideoFiles.First()).ToUpper());
                }
            }
            else return profile.MovieFrame("default");
        }

        private Profile getProfile(string path) {
            string location = path.ToLower();
            foreach (string key in configData.Profiles.Keys) {
                if (location.StartsWith(key))
                {
                    Logger.ReportInfo("Using custom profile for " + path);
                    return configData.Profiles[key];
                }
            }
            return configData.Profiles["default"];
        }

        public bool Ignore(string path)
        {
            if (configData.IgnoreFolders.Count > 0)
            {
                string location = path.ToLower();
                foreach (string folder in configData.IgnoreFolders)
                {
                    if (!String.IsNullOrEmpty(folder) && location.StartsWith(folder.ToLower()))
                        return true;
                }
            }
            return false;
        }

        public void EnsureConfigIsExtracted()
        {
            string fileName = Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppPluginPath, "CoverArtConfig.exe");
            if (!File.Exists(fileName) || configData.LastConfigVersion != this.Version.ToString())
            {
                try
                {
                    Logger.ReportInfo("CoverArt Extracting New Config Tool (version "+this.Version.ToString()+")");
                    if (File.Exists(fileName)) File.Delete(fileName);
                    FileInfo fileInfoOutputFile = new FileInfo(fileName);
                    FileStream streamToOutputFile = fileInfoOutputFile.OpenWrite();
                    //GET THE STREAM TO THE RESOURCES
                    Stream streamToResourceFile = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("CoverArt.CoverArtConfig.exe");
                    const int size = 4096;
                    byte[] bytes = new byte[4096];
                    int numBytes;
                    while ((numBytes = streamToResourceFile.Read(bytes, 0, size)) > 0)
                    {
                        streamToOutputFile.Write(bytes, 0, numBytes);
                    }
                    
                    streamToOutputFile.Close();
                    streamToResourceFile.Close();
                    configData.LastConfigVersion = this.Version.ToString();
                    configData.Save();
                }
                catch (Exception ex)
                {
                    Logger.ReportException("Error extracting CoverArt Config utility", ex);
                }
            }
        }

        public override bool IsConfigurable
        {
            get
            {
                return true;
            }
        }

        public override void Configure()
        {
            try
            {
                EnsureConfigIsExtracted();
            }
            catch (Exception e)
            {
                Logger.ReportException("Could not extract CoverArtConfig.", e);
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppPluginPath, "CoverArtConfig.exe"));
            }
            catch (Exception e)
            {
                Logger.ReportException("Could not execute CoverArtConfig.", e);
            }
        }

        public override string Name
        {
            //provide a short name for your plugin - this will display in the configurator list box
            get { return "CoverArt"; }
        }

        public override string Description
        {
            //provide a longer description of your plugin - this will display when the user selects the theme in the plug-in section
            get { return "Built-in CoverArt for MediaBrowser. (REQUIRES Pegasus). Version 1.1.3 SIGNIFICANTLY reduces the memory usage of CoverArt.  Brought to you by ebrSoft (www.ebrsoft.com)"; }
        }

        public override bool InstallGlobally
        {
            get
            {
                //this must return true if you want to pass references to our resources to MB (including a config page)
                //return true; //we need to be installed in a globally-accessible area (GAC, ehome)

                //return false if you don't need to pass resources
                return false;
            }
        }

        public override System.Version RequiredMBVersion
        {
            get
            {
                return new System.Version(2,2,3,0);
            }
        }

        public override System.Version TestedMBVersion
        {
            get
            {
                return new System.Version(2, 2, 3, 0);
            }
        }

        public override System.Version LatestVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
            set
            {
            }
        }

        public override string RichDescURL
        {
            get
            {
                return "http://www.ebrsoft.com/software/mb/plugins/CoverArtdesc.htm";
            }
        }

        public override System.Version Version
        {
            get
            {
                return LatestVersion;
            }
        }

        public static System.Version CurrentVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
    }


}
