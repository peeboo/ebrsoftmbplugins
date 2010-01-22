using System;
using System.Collections.Generic;
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
            Bitmap newImage = new Bitmap(Resources.DVD);
            Graphics work;
            Image overlay = Resources.Overlay;

            if (!System.IO.File.Exists(item.Path))
            {
                //remote file - use generic case
                Logger.ReportInfo("Using generic case art for " + item.Name);
                newImage = Resources.Case;
            }
            else
            {

                if (Helper.IsBluRayFolder(item.Path, null))
                {
                    //blu-ray start with frame as background
                    Logger.ReportInfo("Using bluray case art for " + item.Name);
                    newImage = Resources.BD;
                }
                else
                {
                    if (Helper.IsDvDFolder(item.Path, null, null))
                    {
                        //dvd
                        Logger.ReportInfo("Using dvd case art for " + item.Name);
                        newImage = Resources.DVD;
                    }
                    else newImage = Resources.Case;
                }
            }

            //create graphics object
            work = Graphics.FromImage(newImage);
            //then put the root image in it's place
            work.DrawImage(rootImage, 15, 90, 595, 650);
            work.DrawImage(overlay, 0, 0);

            return newImage;
        }

        public override string Name
        {
            //provide a short name for your plugin - this will display in the configurator list box
            get { return "CoverArt"; }
        }

        public override string Description
        {
            //provide a longer description of your plugin - this will display when the user selects the theme in the plug-in section
            get { return "A new Plug-in for MediaBrowser"; }
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
