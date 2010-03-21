using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using MediaBrowser.Library.Logging;

namespace CoverArt
{
    public class ImageSet
    {
        public ImageSet(string path)
        {
            directory = path;
            Load();
        }
        public ImageSet()
        {
            Load();
        }

        protected string directory = "";

        [SkipField]
        public string Name
        {
            get { return directory; }
        }

        public bool Is3D = false;
        public bool FrameOnTop = false;
        public Rectangle RootPosition = new Rectangle(15, 90, 585, 750);
        public bool RoundCorners = false;

        [SkipField]
        public bool JustRoundCorners = false; //we use this internally
        [SkipField]
        public Dictionary<string, Image> Frames = new Dictionary<string, Image>();
        [SkipField]
        public Image Overlay;

        [SkipField]
        protected static List<string> FrameTypes = new List<string>() {
            "default",
            "BD",
            "HDDVD",
            "DVD",
            "WMV",
            "AVI",
            "MKV",
            "MP4",
            "M4P",
            "MPG",
            "MOV",
            "MPEG",
            "DVRMS"
            
        };

        public void Save()
        {
            this.settings.Write();
        }

        [SkipField]
        string file = "CoverArt.xml";

        [SkipField]
        XmlSettings<ImageSet> settings;

        protected void Load()
        {
            if (String.IsNullOrEmpty(directory))
            {
                directory = "CoverArtCase";
            }
            switch (directory)
            {
                case "CoverArtCase":
                    //Internal Case
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = new Rectangle(15, 90, 585, 750);
                    Overlay = Resources.Overlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.Case},
                        {"BD",Resources.BD},
                        {"DVD",Resources.DVD},
                        {"MKV",Resources.Mkv},
                        {"WMV",Resources.Wmv},
                        {"AVI",Resources.Avi},
                        {"HDDVD",Resources.HDDVD}
                    };
                    break;
                case "CoverArtCaseBD":
                    //Internal Case
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = new Rectangle(15, 90, 585, 750);
                    Overlay = Resources.Overlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.BD},
                    };
                    break;
                case "CoverArtCaseDVD":
                    //Internal Case
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = new Rectangle(15, 90, 585, 750);
                    Overlay = Resources.Overlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.DVD},
                    };
                    break;
                case "CoverArtTV":
                    //Internal TV
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = new Rectangle(45, 35, 645, 345);
                    Overlay = Resources.TVOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.TV},
                    };
                    break;
                case "CoverArtTVMB":
                    //Internal TV with MB logo
                    Is3D = false;
                    FrameOnTop = true;
                    //RoundCorners = true;
                    RootPosition = new Rectangle(38,38,665,380);
                    Overlay = Resources.TVOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.TVMB},
                    };
                    break;
                case "CoverArtCD":
                    //Internal CD
                    Is3D = false;
                    FrameOnTop = true;
                    RootPosition = new Rectangle(70, 15, 470, 465);
                    Overlay = Resources.BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.CD},
                    };
                    break;
                case "CoverArtFilm":
                    //Internal Film
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = new Rectangle(95, 32, 553, 348);
                    Overlay = Resources.FilmOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.Film},
                    };
                    break;
                case "CoverArtFolder":
                    //Internal Folder
                    break;
                case "CoverArtClearCase":
                    //Internal ClearCase
                    Is3D = false;
                    FrameOnTop = true;
                    RoundCorners = true;
                    RootPosition = new Rectangle(75, 25, 470, 668);
                    Overlay = Resources.BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.ClearCase},
                        {"DVD",Resources.CC_DVD}
                    };
                    break;
                case "CoverArtRounded":
                    //Internal Rounded Corners - only the overlay will be used
                    JustRoundCorners = true;
                    RootPosition = new Rectangle(0,0,0,0);
                    Overlay = Resources.GlossOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.BlankOverlay}
                    };
                    break;
                case "CoverArtDiamond":
                    //Internal Rounded Corners - only the overlay will be used
                    RoundCorners = true;
                    FrameOnTop = true;
                    RootPosition = new Rectangle(26, 25, 470, 665);
                    Overlay = Resources.BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.Diamond}
                    };
                    break;
                case "Ignore":
                    //Ignore this cover
                    RootPosition = new Rectangle(0,0,0,0);
                    Overlay = Resources.BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Resources.Blank}
                    };
                    break;
                default:
                    //Load from directory
                    //first get our attributes from xml
                    string fullName = Path.Combine(directory,file);
                    //if (!File.Exists(fullName))
                    //{
                    //    //can't find xml file - either directory not valid or file not there
                    //    Logger.ReportError("CoverArt cannot find required xml definition file 'CoverArt.xml' in directory " + directory + ".  Using default.");
                    //    //Use default
                    //    Is3D = false;
                    //    FrameOnTop = false;
                    //    RootPosition = new Rectangle(15, 90, 585, 750);
                    //    Overlay = Resources.Overlay;
                    //    Frames = new Dictionary<string, Image>() {
                    //        {"default", Resources.Case},
                    //        {"BD",Resources.BD},
                    //        {"DVD",Resources.DVD},
                    //        {"MKV",Resources.Mkv},
                    //        {"WMV",Resources.Wmv},
                    //        {"AVI",Resources.Avi},
                    //        {"HDDVD",Resources.HDDVD}
                    //    };
                    //} else 
                    if (Directory.Exists(directory))
                    {
                        //read in settings
                        this.settings = XmlSettings<ImageSet>.Bind(this, fullName);
                        string filename;
                        //look for images and read those in
                        //first the frames
                        foreach (string frame in FrameTypes)
                        {
                            filename = Path.Combine(directory, frame + ".png");
                            if (File.Exists(filename))
                            {
                                Frames.Add(frame, Image.FromFile(filename));
                            }
                        }
                        //be sure we got at least a default
                        if (!Frames.ContainsKey("default"))
                        {
                            Logger.ReportError("CoverArt - No default frame found in " + directory);
                            Frames.Add("default", Resources.Case);
                        }

                        //now the overlay
                        filename = Path.Combine(directory, "overlay.png");
                        if (File.Exists(filename))
                        {
                            Overlay = Image.FromFile(filename);
                        }
                        else Overlay = Resources.BlankOverlay;
                    }
                    else Logger.ReportError("CoverArt ImageSet does not exist: " + directory);
                    break;
            }
        }
    }
}
