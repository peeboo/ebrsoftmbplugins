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

namespace FilmsLikeThis
{
    class Plugin : BasePlugin
    {

        static readonly Guid FilmsLikeThisGuid = new Guid("4690e732-2cc9-4c30-b1ff-e9d2264ccc89");

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

                //un-comment the following line to provide a config panel for your plugin
                //kernel.AddConfigPanel("FilmsLikeThis Options", "resx://FilmsLikeThis/FilmsLikeThis.Resources/ConfigPanel#ConfigPanel");

                //The AddStringData method will allow you to extend the localized strings used by MediaBrowser with your own.
                //This is useful for adding descriptive text to go along with your theme options.  If you don't have any theme-
                //specific options or other needs to extend the string data, remove the following call.

                //un-comment the following line to provide string data extensions for your plugin
                //kernel.StringData.AddStringData(MyStrings.FromFile(MyStrings.GetFileName("FilmsLikeThis-")));
                bool isMC = AppDomain.CurrentDomain.FriendlyName.Contains("ehExtHost");
                if (isMC) //only do this inside of MediaCenter as menus can only be created inside MediaCenter
                {
                    kernel.AddMenuItem(new MenuItem("Like This...", "resx://MediaBrowser/MediaBrowser.Resources/IconFloral", getFilmsLikeThis, new List<Type>() { typeof(Movie) }));
                }
                else Logger.ReportInfo("Not creating menus for Films Like This.  Appear to not be in MediaCenter.  AppDomain is: " + AppDomain.CurrentDomain.FriendlyName);

                //Tell the log we loaded.
                Logger.ReportInfo("FilmsLikeThis Plug-in (Version "+Version.ToString()+") Loaded.");
            }
            catch (Exception ex)
            {
                Logger.ReportException("Error initializing FilmsLikeThis - probably incompatable MB version", ex);
            }

        }

        private void getFilmsLikeThis(Item item)
        {
            //look for films like the item passed in based on a scoring system

            //first create an index and child list - we will fill in asynch cuz it could take a while
            List<BaseItem> children = new List<BaseItem>();
            Index flt = new Index(Application.CurrentInstance.RootFolder,children);
            flt.Name = "Films Like "+item.Name;

            //now kick off populate routine
                Async.Queue("Films Like This Search",() => {
                foreach (BaseItem i in Application.CurrentInstance.RootFolder.RecursiveChildren)
                {
                    //check option for excluding remote content
                    if (Config.Instance.ExcludeRemoteContentInSearch && i.IsRemoteContent) continue;

                    //see if we're already in there - don't want dups
                    if (children.Find(us => us.Name == i.Name) == null)
                    {
                        try
                        {
                            float score = scoreAgainst(i, item);
                            if (score > 39)
                            {
                                //i.Name = score.ToString("F0") + "% - " + i.Name;
                                children.Add(i);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.ReportException("Uknown Error in find like this " + item.Name, ex);
                        }
                    }
                }
                });
            //}

            //and navigate to our new index
            Application.CurrentInstance.Navigate(ItemFactory.Instance.Create(flt));
        }

        private float scoreAgainst(BaseItem item, Item compareTo)
        {
            float score = 0;
            IShow show = item as IShow;
            if (show != null)
            {
                //primary score off genre
                if (compareTo.Genres != null && show.Genres != null)
                {
                    float factor = 100 / compareTo.Genres.Count;
                    foreach (var genre in compareTo.Genres)
                    {
                        if (show.Genres.Contains(genre)) score += factor;
                    }
                }

                //same primary director should get you in the list
                if (show.Directors != null)
                    if (show.Directors.Contains(compareTo.Directors[0])) { score += 40; Logger.ReportInfo("Found Director " + compareTo.Directors[0] + " in " + item.Name); }
            }
            return score;
        }


        public override string Name
        {
            //provide a short name for your plugin - this will display in the configurator list box
            get { return "Films Like This"; }
        }

        public override string Description
        {
            //provide a longer description of your plugin - this will display when the user selects the theme in the plug-in section
            get { return "Search for and present films that are most like the current selection.  Brought to you by ebrSoft. www.ebrsoft.com"; }
        }

        public override bool InstallGlobally
        {
            get
            {
                //this must return true if you want to pass references to our resources to MB
                //return true; //we need to be installed in a globally-accessible area (GAC, ehome)

                //return false if you don't need to pass resources
                return false;
            }
        }

        public override System.Version LatestVersion
        {
            get
            {
                return new System.Version(0, 2, 1, 0);
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
