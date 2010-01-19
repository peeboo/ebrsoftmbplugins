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

namespace SecureContent
{
    class Plugin : BasePlugin
    {

        static readonly Guid SecureContentGuid = new Guid("68e0a58f-9886-4a0a-b661-f9e69fb9c06d");
        MyConfig config;

        /// <summary>
        /// The Init method is called when the plug-in is loaded by MediaBrowser.  You should perform all your specific initializations
        /// here - including adding your theme to the list of available themes.
        /// </summary>
        /// <param name="kernel"></param>
        public override void Init(Kernel kernel)
        {
            bool isMC = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
            if (isMC)
            { //only do this inside of MediaCenter 
                try
                {
                    //The AddConfigPanel method will allow you to extend the config page of MediaBrowser with your own options panel.
                    //You must create this as an mcml UI that fits within the standard config panel area.  It must take Application and 
                    //FocusItem as parameters.  The project template should have generated an example ConfigPage.mcml that you can modify
                    //or, if you don't wish to extend the config, remove it and the following call to AddConfigPanel
                    //*** FOR THIS TO WORK *** you will need to define InstallGlobally to return "true"

                    //un-comment the following line to provide a config panel for your plugin
                    config = new MyConfig();
                    config.Locked = true;
                    kernel.AddConfigPanel("Secure", "resx://SecureContent/SecureContent.Resources/ConfigPanel#ConfigPanel", config);

                    //The AddStringData method will allow you to extend the localized strings used by MediaBrowser with your own.
                    //This is useful for adding descriptive text to go along with your theme options.  If you don't have any theme-
                    //specific options or other needs to extend the string data, remove the following call.

                    //un-comment the following line to provide string data extensions for your plugin
                    //kernel.StringData.AddStringData(MyStrings.FromFile(MyStrings.GetFileName("SecureContent-")));


                    //Tell the log we loaded.
                    Logger.ReportInfo("SecureContent (version " + Version + ") Plug-in Loaded.");
                }
                catch (Exception ex)
                {
                    Logger.ReportException("Error initializing SecureContent - probably incompatable MB version", ex);
                }
            }

        }

        public override string Name
        {
            //provide a short name for your plugin - this will display in the configurator list box
            get { return "SecureContent"; }
        }

        public override string Description
        {
            //provide a longer description of your plugin - this will display when the user selects the theme in the plug-in section
            get { return "Secure content plug-in for MediaBrowser.  Brought to you by ebrSoft www.ebrsoft.com"; }
        }

        public override bool InstallGlobally
        {
            get
            {
                //this must return true if you want to pass references to our resources to MB
                return true; //we need to be installed in a globally-accessible area (GAC, ehome)

                //return false if you don't need to pass resources
                //return false;
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
