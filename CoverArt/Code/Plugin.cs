﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using MediaBrowser.Library.Plugins;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using MediaBrowser;
using MediaBrowser.Library.Configuration;
using MediaBrowser.Library;
using MediaBrowser.LibraryManagement;

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
    class Plugin : BasePlugin
    {

        static readonly Guid CoverArtGuid = new Guid("756c2e60-f6f1-4167-b287-3798ddf373c4");

        private MyConfig config;

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
                configData = MyConfigData.FromFile(Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppPluginPath, "Configurations\\Coverart.xml"));

                //test
                //ignoreFolders.Add("\\\\mediaserver\\movies\\music");
                //profiles.Add("\\\\mediaserver\\movies\\music", new Profile("CoverArtClearCase", null, null, null, null, null, null));
                //profiles.Add("\\\\mediaserver\\movies\\hd", new Profile("c:\\programdata\\mediabrowser\\plugins\\coverart\\testprofiles\\diamond", null, null, null, null, "c:\\programdata\\mediabrowser\\plugins\\coverart\\testprofiles\\movie", null));
                //test

                //Tell the log we loaded.
                Logger.ReportInfo("CoverArt (version " + Version + ") Plug-in Loaded.");
            }
            catch (Exception ex)
            {
                Logger.ReportException("Error initializing CoverArt - probably incompatable MB version", ex);
            }

        }

        public Image ProcessImage(Image rootImage, BaseItem item)
        {
            Logger.ReportInfo("Image for " + item.Name + " being processed by CoverArt");
            Image newImage = rootImage;

            Profile profile = getProfile(item.Path);
            bool frameOnTop = false;
            bool process = false;

            //check for ignore
            if (Ignore(item.Path))
            {
                Logger.ReportInfo("Ignoring Item in " + item.Path);
                return rootImage;
            }

            Graphics work;
            Rectangle position = new Rectangle(0,0,0,0);
            Image overlay = profile.MovieOverlay();

            if (item is Movie)
            {
                process = true;
                //overlay = profile.MovieOverlay;
                position = profile.RootPosition("movie");

                if (item.Path.StartsWith("http://"))
                {
                    //remote file - use remote case
                    Logger.ReportInfo("Using remote case art for " + item.Name);
                    newImage = profile.RemoteFrame("default");
                    overlay = profile.RemoteOverlay();
                    position = profile.RootPosition("remote");
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
                    }
                    else
                    {

                        frameOnTop = profile.FrameOnTop("movie");
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
                    position = profile.RootPosition("episode");
                }
                else
                {
                    if (item is Series)
                    {
                        //apply series treatment
                        process = true;
                        newImage = profile.SeriesFrame("default");
                        overlay = profile.SeriesOverlay();
                        frameOnTop = profile.FrameOnTop("series");
                        position = profile.RootPosition("series");
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
                                position = profile.RootPosition("album");

                            }
                        }
                    }
                }

            }

            if (process)
            {
                if (frameOnTop)
                {
                    //the frame goes on top so just create a base image of proper size
                    Image temp = newImage;
                    //no overlay so just create new bitmap of proper size
                    newImage = new Bitmap(temp.Width,temp.Height);
                    overlay = temp;
                }

                //create graphics object
                work = Graphics.FromImage(newImage);
                //then put the root image in it's place
                work.DrawImage(rootImage, position);
                //and the overlay
                work.DrawImage(overlay, 0, 0, newImage.Width, newImage.Height);
                work.Dispose();
            }
            else
            {
                Logger.ReportInfo("No processing applied to image for " + item.Name);
            }

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
                    default:
                        Logger.ReportInfo("Could not determine file type of " + video.VideoFiles.First().ToLower().Substring(video.VideoFiles.First().Length - 4));
                        return profile.MovieFrame("default");
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
                    if (location.StartsWith(folder))
                        return true;
                }
            }
            return false;
        }

        public override string Name
        {
            //provide a short name for your plugin - this will display in the configurator list box
            get { return "CoverArt"; }
        }

        public override string Description
        {
            //provide a longer description of your plugin - this will display when the user selects the theme in the plug-in section
            get { return "Built-in CoverArt for MediaBrowser.  Brought to you by ebrSoft (www.ebrsoft.com)"; }
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
                return new System.Version(2,2,2,0);
            }
        }

        public override System.Version LatestVersion
        {
            get
            {
                return new System.Version(0, 1, 0, 1);
            }
            set
            {
            }
        }

        public override System.Version Version
        {
            get
            {
                return LatestVersion;
            }
        }
    }


}
