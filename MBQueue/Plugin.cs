using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MediaBrowser.Library.Plugins;
using MediaBrowser.Library.Entities;
using MediaBrowser.Library.Logging;
using MediaBrowser;
using MediaBrowser.Library.Configuration;
using MediaBrowser.Library;

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

namespace MBQueue
{
    class Plugin : BasePlugin
    {

        static readonly Guid MBQueueGuid = new Guid("edf9dc76-e2b0-4a01-9aa8-24638df15403");

        private MyConfig config;
        private QueueFolder queue;

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
                queue = kernel.ItemRepository.RetrieveItem(MBQueueGuid) as QueueFolder ?? new QueueFolder();
                queue.Id = MBQueueGuid;
                queue.Path = "\\\\mediaserver\\movies\\Queue";
                //create directory if it doesn't exist
                if (!Directory.Exists(queue.Path)) Directory.CreateDirectory(queue.Path);
                kernel.RootFolder.AddVirtualChild(queue);
                kernel.ItemRepository.SaveItem(queue);
                bool isMC = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
                if (isMC) //only do this inside of MediaCenter as menus can only be created inside MediaCenter
                {
                    //kernel.AddMenuItem(new MenuItem(addRemoveText, "resx://MediaBrowser/MediaBrowser.Resources/Star_Full", this.addRemoveFavorite, new List<Type>() { typeof(Movie), typeof(Series), typeof(Folder) }));
                    //kernel.AddMenuItem(new MenuItem("Clear All", "resx://MediaBrowser/MediaBrowser.Resources/IconDelete", this.clearFavorites, new List<Type>() { typeof(FavoriteFolder) }));
                    //kernel.AddMenuItem(new MenuItem("Hide Favorites", "resx://MediaBrowser/MediaBrowser.Resources/IconDelete", this.hideFavorites, new List<Type>() { typeof(FavoriteFolder) }));
                }
                else Logger.ReportInfo("Not creating menus for MBQueue.  Appear to not be in MediaCenter.  AppDomain is: " + AppDomain.CurrentDomain.FriendlyName);
                //bool isMC = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
                //if (isMC) 
                //{                    
                //    config = new MyConfig();
                //    kernel.AddConfigPanel("MBQueue Options", "resx://MBQueue/MBQueue.Resources/ConfigPanel#ConfigPanel", config);
                //    //If you want to add any context menus they need to be inside this logic as well.
                //}
                //else Logger.ReportInfo("Not creating menus for MBQueue.  Appear to not be in MediaCenter.  AppDomain is: " + AppDomain.CurrentDomain.FriendlyName);

                //The AddStringData method will allow you to extend the localized strings used by MediaBrowser with your own.
                //This is useful for adding descriptive text to go along with your theme options.  If you don't have any theme-
                //specific options or other needs to extend the string data, remove the following call.

                //un-comment the following line to provide string data extensions for your plugin
                //kernel.StringData.AddStringData(MyStrings.FromFile(MyStrings.GetFileName("MBQueue-")));

                //Tell the log we loaded.
                Logger.ReportInfo("MBQueue (version " + Version + ") Plug-in Loaded.");
            }
            catch (Exception ex)
            {
                Logger.ReportException("Error initializing MBQueue - probably incompatable MB version", ex);
            }

        }

        public override string Name
        {
            //provide a short name for your plugin - this will display in the configurator list box
            get { return "MBQueue"; }
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
                return true; //we need to be installed in a globally-accessible area (GAC, ehome)

                //return false if you don't need to pass resources
                //return false;
            }
        }

        /// <summary>
        /// Return the lowest version of MediaBrowser with which this plug-in is compatable
        /// </summary>
        public override System.Version RequiredMBVersion
        {
            get
            {
                return new System.Version(2, 0, 0, 0);
            }
        }

        public override System.Version LatestVersion
        {
            get
            {
                return new System.Version(0, 1, 0, 0);
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
