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
        public static List<string> FrameTypes = new List<string>() {
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
            "DIVX",
            "H264",
            "XVID",
            "DVRMS",
            "HD",
            "SD"
            
        };

        [SkipField]
        protected static Rectangle caseRectangle = new Rectangle(15, 95, 580, 745);

        [SkipField]
        protected static Rectangle caseRectangle3D = new Rectangle(73, 96, 528, 768);

        [SkipField]
        protected static Image BlankOverlay = Resources.BlankOverlay;

        [SkipField]
        protected static Image StdOverlay = Resources.Overlay;

        [SkipField]
        protected static Image _BDCase;

        protected static Image BDCase
        {
            get
            {
                if (_BDCase == null) _BDCase = Resources.BD;
                return _BDCase;
            }
        }

        [SkipField]
        protected static Image BDCase3D = Resources.c3d_bd;

        [SkipField]
        protected static Image HDDVDCase = Resources.HDDVD;

        [SkipField]
        protected static Image HDDVDCase3D = Resources.c3d_hddvd;

        [SkipField]
        protected static Image DVDCase = Resources.DVD;

        [SkipField]
        protected static Image DVDCase3D = Resources.c3d_dvd;

        [SkipField]
        protected static Image StdCase = Resources.Case;

        [SkipField]
        protected static Image StdCase3D = Resources.c3d_case;

        [SkipField]
        protected static Image BlankImage = Resources.Blank;

        [SkipField]
        protected static Image GlossOverlay = Resources.GlossOverlay;

        [SkipField]
        protected static Image ClearCase = Resources.ClearCase;

        [SkipField]
        protected static Image ClearCaseDVD = Resources.CC_DVD;

        [SkipField]
        protected static Image ClearCaseBD = Resources.cc_bd;

        [SkipField]
        protected static Image ClearCaseHDDVD = Resources.cc_hddvd;

        [SkipField]
        protected static Image ClearCase3D = Resources.ClearCase3d;

        [SkipField]
        protected static Image ClearCaseDVD3D = Resources.cc3d_dvd;

        [SkipField]
        protected static Image ClearCaseBD3D = Resources.cc3d_bd;

        [SkipField]
        protected static Image ClearCaseHDDVD3D = Resources.cc3d_hddvd;

        [SkipField]
        protected static Image Diamond = Resources.Diamond;

        [SkipField]
        protected static Image TV = Resources.TV;

        [SkipField]
        protected static Image TVMB = Resources.TVMB;

        [SkipField]
        protected static Image Film = Resources.Film;

        [SkipField]
        protected static Image Border = Resources.SimpleBorder;

        [SkipField]
        protected static Image CD = Resources.CD;

        [SkipField]
        protected static Image TVOverlay = Resources.TVOverlay;

        [SkipField]
        protected static Image FilmOverlay = Resources.FilmOverlay;

        [SkipField]
        protected static Dictionary<string, Image> InternalCase = new Dictionary<string, Image>() {
                        {"default", StdCase},
                        {"BD",BDCase},
                        {"DVD",DVDCase},
                        {"MKV",Resources.Mkv},
                        {"WMV",Resources.Wmv},
                        {"AVI",Resources.Avi},
                        {"HDDVD",HDDVDCase},
                        {"DVRMS",Resources.dv},
                        {"H264",Resources.h264},
                        {"MPEG",Resources.Mpeg},
                        {"MP4",Resources.Mpeg},
                        {"DIVX",Resources.DivX},
                        {"MOV",Resources.mov},
                        {"XVID",Resources.Xvid},
                        {"HD",Resources.HD}
        };

        [SkipField]
        protected static Dictionary<string, Image> InternalCase3D = new Dictionary<string, Image>() {
                        {"default", StdCase3D},
                        {"BD",BDCase3D},
                        {"DVD",DVDCase3D},
                        //{"MKV",Resources.Mkv},
                        //{"WMV",Resources.Wmv},
                        //{"AVI",Resources.Avi},
                        {"HDDVD",HDDVDCase3D},
                        //{"DVRMS",Resources.dv},
                        //{"H264",Resources.h264},
                        //{"MPEG",Resources.Mpeg},
                        //{"MP4",Resources.Mpeg},
                        //{"DIVX",Resources.DivX},
                        //{"MOV",Resources.mov},
                        //{"XVID",Resources.Xvid},
                        {"HD",BDCase3D}
        };

        [SkipField]
        protected static Dictionary<string, Image> InternalClearCase3D = new Dictionary<string, Image>() {
                        {"default", ClearCase3D},
                        {"BD",ClearCaseBD3D},
                        {"DVD",ClearCaseDVD3D},
                        //{"MKV",Resources.Mkv},
                        //{"WMV",Resources.Wmv},
                        //{"AVI",Resources.Avi},
                        {"HDDVD",ClearCaseHDDVD3D},
                        //{"DVRMS",Resources.dv},
                        //{"H264",Resources.h264},
                        //{"MPEG",Resources.Mpeg},
                        //{"MP4",Resources.Mpeg},
                        //{"DIVX",Resources.DivX},
                        //{"MOV",Resources.mov},
                        //{"XVID",Resources.Xvid},
                        {"HD",ClearCaseBD3D}
        };

        [SkipField]
        protected static Dictionary<string, Image> InternalClearCase = new Dictionary<string, Image>() {
                        {"default", ClearCase},
                        {"BD",ClearCaseBD},
                        {"DVD",ClearCaseDVD},
                        {"MKV",Resources.cc_mkv},
                        {"WMV",Resources.cc_wmv},
                        {"AVI",Resources.cc_avi},
                        {"HDDVD",ClearCaseHDDVD},
                        {"DVRMS",Resources.cc_dv},
                        {"H264",Resources.cc_h264},
                        {"MPEG",Resources.cc_mpeg},
                        {"MP4",Resources.cc_mpeg},
                        {"DIVX",Resources.cc_divx},
                        {"MOV",Resources.cc_mov},
                        {"XVID",Resources.cc_xvid}
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
                    RootPosition = caseRectangle;
                    Overlay = StdOverlay;
                    Frames = InternalCase;
                    break;
                case "CoverArtCaseBD":
                    //Internal Case
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = caseRectangle;
                    Overlay = StdOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", BDCase},
                    };
                    break;
                case "CoverArtCaseDVD":
                    //Internal Case
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = caseRectangle;
                    Overlay = StdOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", DVDCase},
                    };
                    break;
                case "CoverArtCaseMinimal":
                    //Just the big 3
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = caseRectangle;
                    Overlay = StdOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", StdCase},
                        {"DVD",DVDCase},
                        {"BD",BDCase},
                        {"HDDVD",HDDVDCase},
                        {"SD",DVDCase},
                        {"HD",BDCase}
                    };
                    break;
                case "CoverArtCase3D":
                    //Internal 3D Case
                    Is3D = true;
                    FrameOnTop = true;
                    RootPosition = caseRectangle3D;
                    Overlay = BlankOverlay;
                    Frames = InternalCase3D;
                    break;
                case "CoverArtTV":
                    //Internal TV
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = new Rectangle(45, 35, 645, 345);
                    Overlay = TVOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", TV},
                    };
                    break;
                case "CoverArtTVMB":
                    //Internal TV with MB logo
                    Is3D = false;
                    FrameOnTop = true;
                    //RoundCorners = true;
                    RootPosition = new Rectangle(38,38,665,380);
                    Overlay = TVOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", TVMB},
                    };
                    break;
                case "CoverArtCD":
                    //Internal CD
                    Is3D = false;
                    FrameOnTop = true;
                    RootPosition = new Rectangle(70, 15, 470, 465);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", CD},
                    };
                    break;
                case "CoverArtFilm":
                    //Internal Film
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = new Rectangle(95, 32, 553, 348);
                    Overlay = FilmOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Film},
                    };
                    break;
                case "CoverArtBorder":
                    //Internal Film
                    Is3D = false;
                    FrameOnTop = false;
                    RootPosition = new Rectangle(13, 18, 554, 351);
                    Overlay = FilmOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Border},
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
                    Overlay = BlankOverlay;
                    Frames = InternalClearCase;
                    break;
                case "CoverArtClearCase3D":
                    //Internal 3D ClearCase
                    Is3D = true;
                    FrameOnTop = true;
                    RoundCorners = true;
                    RootPosition = new Rectangle(72, 19, 434, 667);
                    Overlay = BlankOverlay;
                    Frames = InternalClearCase3D;
                    break;
                case "CoverArtClearCaseMinimal":
                    //Internal ClearCase
                    Is3D = false;
                    FrameOnTop = true;
                    RoundCorners = true;
                    RootPosition = new Rectangle(75, 25, 470, 668);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default",ClearCase},
                        {"DVD",ClearCaseDVD},
                        {"BD",ClearCaseBD},
                        {"HDDVD",ClearCaseHDDVD},
                        {"HD",ClearCaseBD},
                        {"SD",ClearCaseDVD}
                    };
                    break;
                case "CoverArtClearCasePlain":
                    //Internal ClearCase with just the plain one
                    Is3D = false;
                    FrameOnTop = true;
                    RoundCorners = true;
                    RootPosition = new Rectangle(75, 25, 470, 668);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default",ClearCase}
                    };
                    break;
                case "CoverArtClearCaseBD":
                    //Internal ClearCase with just the plain one
                    Is3D = false;
                    FrameOnTop = true;
                    RoundCorners = true;
                    RootPosition = new Rectangle(75, 25, 470, 668);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default",ClearCaseBD}
                    };
                    break;
                case "CoverArtClearCaseDVD":
                    //Internal ClearCase just DVD cover
                    Is3D = false;
                    FrameOnTop = true;
                    RoundCorners = true;
                    RootPosition = new Rectangle(75, 25, 470, 668);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", ClearCaseDVD}
                    };
                    break;
                case "CoverArtRounded":
                    //Internal Rounded Corners - only the overlay will be used
                    JustRoundCorners = true;
                    RootPosition = new Rectangle(0,0,0,0);
                    Overlay = GlossOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", BlankOverlay}
                    };
                    break;
                case "CoverArtDiamond":
                    //Internal Rounded Corners - only the overlay will be used
                    RoundCorners = true;
                    FrameOnTop = true;
                    RootPosition = new Rectangle(26, 25, 470, 665);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Diamond}
                    };
                    break;
                case "Ignore":
                    //Ignore this cover
                    RootPosition = new Rectangle(0,0,0,0);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", BlankImage}
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
                            Frames.Add("default", StdCase);
                        }

                        //now the overlay
                        filename = Path.Combine(directory, "overlay.png");
                        if (File.Exists(filename))
                        {
                            Overlay = Image.FromFile(filename);
                        }
                        else Overlay = BlankOverlay;
                    }
                    else Logger.ReportError("CoverArt ImageSet does not exist: " + directory);
                    break;
            }
        }
    }
}
