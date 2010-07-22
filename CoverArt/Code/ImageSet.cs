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
        [SkipField]
        public SkewRatios Skew = new SkewRatios(0,0,1,.055,0,1,1,.945);
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
            "WTV",
            "HD",
            "Trailer",
            "Boxset",
            "Folder",
            "Series",
            "Season",
            "Specials",
            "Episode",
            "Album",
            "Person",
            "Remote"
            
        };

        [SkipField]
        public static List<string> NonMediaFrameTypes = new List<string>() {
            "HD",
            "SD",
            "Trailer",
            "BoxSet",
            "Folder",
            "Series",
            "Season",
            "Specials",
            "Episode",
            "Album",
            "Person",
            "Remote"
            
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
        protected static Image OCOverlay = Resources.oc_overlay;

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
        protected static Image _BDCase3D;

        protected static Image BDCase3D
        {
            get
            {
                if (_BDCase3D == null) _BDCase3D = Resources.c3d_bd;
                return _BDCase3D;
            }
        }

        [SkipField]
        protected static Image _OpenCase;

        protected static Image OpenCase
        {
            get
            {
                if (_OpenCase == null) _OpenCase = Resources.OpenCase;
                return _OpenCase;
            }
        }

        [SkipField]
        protected static Image _OpenCaseBD;

        protected static Image OpenCaseBD
        {
            get
            {
                if (_OpenCaseBD == null) _OpenCaseBD = Resources.oc_bd;
                return _OpenCaseBD;
            }
        }

        [SkipField]
        protected static Image _OpenCaseDVD;

        protected static Image OpenCaseDVD
        {
            get
            {
                if (_OpenCaseDVD == null) _OpenCaseDVD = Resources.oc_dvd;
                return _OpenCaseDVD;
            }
        }

        [SkipField]
        protected static Image _OpenCaseHDDVD;

        protected static Image OpenCaseHDDVD
        {
            get
            {
                if (_OpenCaseHDDVD == null) _OpenCaseHDDVD = Resources.oc_hddvd;
                return _OpenCaseHDDVD;
            }
        }

        [SkipField]
        protected static Image _HDDVDCase;

        protected static Image HDDVDCase
        {
            get
            {
                if (_HDDVDCase == null) _HDDVDCase = Resources.HDDVD;
                return _HDDVDCase;
            }
        }

        [SkipField]
        protected static Image _HDDVDCase3D;

        protected static Image HDDVDCase3D
        {
            get
            {
                if (_HDDVDCase3D == null) _HDDVDCase3D = Resources.c3d_hddvd;
                return _HDDVDCase3D;
            }
        }

        [SkipField]
        protected static Image _DVDCase;

        protected static Image DVDCase
        {
            get
            {
                if (_DVDCase == null) _DVDCase = Resources.DVD;
                return _DVDCase;
            }
        }

        [SkipField]
        protected static Image _DVDCase3D;

        protected static Image DVDCase3D
        {
            get
            {
                if (_DVDCase3D == null) _DVDCase3D = Resources.c3d_dvd;
                return _DVDCase3D;
            }
        }

        [SkipField]
        protected static Image _StdCase;

        protected static Image StdCase
        {
            get
            {
                if (_StdCase == null) _StdCase = Resources.Case;
                return _StdCase;
            }
        }

        [SkipField]
        protected static Image _SeasonCase;

        protected static Image SeasonCase
        {
            get
            {
                if (_SeasonCase == null) _SeasonCase = Resources.Season;
                return _SeasonCase;
            }
        }

        [SkipField]
        protected static Image _SeriesCase;

        protected static Image SeriesCase
        {
            get
            {
                if (_SeriesCase == null) _SeriesCase = Resources.Series;
                return _SeriesCase;
            }
        }

        [SkipField]
        protected static Image _SpecialsCase;

        protected static Image SpecialsCase
        {
            get
            {
                if (_SpecialsCase == null) _SpecialsCase = Resources.Specials;
                return _SpecialsCase;
            }
        }

        [SkipField]
        protected static Image _TrailerCase;

        protected static Image TrailerCase
        {
            get
            {
                if (_TrailerCase == null) _TrailerCase = Resources.trailer;
                return _TrailerCase;
            }
        }

        [SkipField]
        protected static Image _BoxsetCase;

        protected static Image BoxsetCase
        {
            get
            {
                if (_BoxsetCase == null) _BoxsetCase = Resources.boxset;
                return _BoxsetCase;
            }
        }

        [SkipField]
        protected static Image _StdCase3D;

        protected static Image StdCase3D
        {
            get
            {
                if (_StdCase3D == null) _StdCase3D = Resources.c3d_case;
                return _StdCase3D;
            }
        }

        [SkipField]
        protected static Image BlankImage = Resources.Blank;

        [SkipField]
        protected static Image GlossOverlay = Resources.GlossOverlay;

        [SkipField]
        protected static Image _ClearCase;

        protected static Image ClearCase
        {
            get
            {
                if (_ClearCase == null) _ClearCase = Resources.ClearCase;
                return _ClearCase;
            }
        }

        [SkipField]
        protected static Image _ClearCaseDVD;

        protected static Image ClearCaseDVD
        {
            get
            {
                if (_ClearCaseDVD == null) _ClearCaseDVD = Resources.CC_DVD;
                return _ClearCaseDVD;
            }
        }

        [SkipField]
        protected static Image _ClearCaseBD;

        protected static Image ClearCaseBD
        {
            get
            {
                if (_ClearCaseBD == null) _ClearCaseBD = Resources.cc_bd;
                return _ClearCaseBD;
            }
        }

        [SkipField]
        protected static Image _ClearCaseHDDVD;

        protected static Image ClearCaseHDDVD
        {
            get
            {
                if (_ClearCaseHDDVD == null) _ClearCaseHDDVD = Resources.cc_hddvd;
                return _ClearCaseHDDVD;
            }
        }

        [SkipField]
        protected static Image _ClearCase3D;

        protected static Image ClearCase3D
        {
            get
            {
                if (_ClearCase3D == null) _ClearCase3D = Resources.ClearCase3d;
                return _ClearCase3D;
            }
        }

        [SkipField]
        protected static Image _ClearCaseDVD3D;

        protected static Image ClearCaseDVD3D
        {
            get
            {
                if (_ClearCaseDVD3D == null) _ClearCaseDVD3D = Resources.cc3d_dvd;
                return _ClearCaseDVD3D;
            }
        }

        [SkipField]
        protected static Image _ClearCaseBD3D;

        protected static Image ClearCaseBD3D
        {
            get
            {
                if (_ClearCaseBD3D == null) _ClearCaseBD3D = Resources.cc3d_bd;
                return _ClearCaseBD3D;
            }
        }

        [SkipField]
        protected static Image _ClearCaseHDDVD3D;

        protected static Image ClearCaseHDDVD3D
        {
            get
            {
                if (_ClearCaseHDDVD3D == null) _ClearCaseHDDVD3D = Resources.cc3d_hddvd;
                return _ClearCaseHDDVD3D;
            }
        }

        [SkipField]
        protected static Image _ClearCaseSeason;

        protected static Image ClearCaseSeason
        {
            get
            {
                if (_ClearCaseSeason == null) _ClearCaseSeason = Resources.cc_season;
                return _ClearCaseSeason;
            }
        }

        [SkipField]
        protected static Image _ClearCaseSeries;

        protected static Image ClearCaseSeries
        {
            get
            {
                if (_ClearCaseSeries == null) _ClearCaseSeries = Resources.cc_series;
                return _ClearCaseSeries;
            }
        }

        [SkipField]
        protected static Image _ClearCaseSpecials;

        protected static Image ClearCaseSpecials
        {
            get
            {
                if (_ClearCaseSpecials == null) _ClearCaseSpecials = Resources.cc_specials;
                return _ClearCaseSpecials;
            }
        }

        [SkipField]
        protected static Image _ClearCaseTrailer;

        protected static Image ClearCaseTrailer
        {
            get
            {
                if (_ClearCaseTrailer == null) _ClearCaseTrailer = Resources.cc_trailer;
                return _ClearCaseTrailer;
            }
        }

        [SkipField]
        protected static Image _ClearCaseBoxset;

        protected static Image ClearCaseBoxset
        {
            get
            {
                if (_ClearCaseBoxset == null) _ClearCaseBoxset = Resources.cc_boxset;
                return _ClearCaseBoxset;
            }
        }

        [SkipField]
        protected static Image _Diamond;

        protected static Image Diamond
        {
            get
            {
                if (_Diamond == null) _Diamond = Resources.DiamondGloss;
                return _Diamond;
            }
        }

        [SkipField]
        protected static Image _DiamondThumb;

        protected static Image DiamondThumb
        {
            get
            {
                if (_DiamondThumb == null) _DiamondThumb = Resources.DiamondGlossThumb;
                return _DiamondThumb;
            }
        }

        //[SkipField]
        //protected static Image _TV;

        //protected static Image TV
        //{
        //    get
        //    {
        //        if (_TV == null) _TV = Resources.TV;
        //        return _TV;
        //    }
        //}

        [SkipField]
        protected static Image _TVMB;

        protected static Image TVMB
        {
            get
            {
                if (_TVMB == null) _TVMB = Resources.TVMB;
                return _TVMB;
            }
        }

        [SkipField]
        protected static Image _TVMB3D;

        protected static Image TVMB3D
        {
            get
            {
                if (_TVMB3D == null) _TVMB3D = Resources.TVMB3D;
                return _TVMB3D;
            }
        }

        [SkipField]
        protected static Image _Film;

        protected static Image Film
        {
            get
            {
                if (_Film == null) _Film = Resources.Film;
                return _Film;
            }
        }

        [SkipField]
        protected static Image _Film3D;

        protected static Image Film3D
        {
            get
            {
                if (_Film3D == null) _Film3D = Resources.Film3D;
                return _Film3D;
            }
        }

        [SkipField]
        protected static Image _Border;

        protected static Image Border
        {
            get
            {
                if (_Border == null) _Border = Resources.SimpleBorder;
                return _Border;
            }
        }

        [SkipField]
        protected static Image _CD;

        protected static Image CD
        {
            get
            {
                if (_CD == null) _CD = Resources.CD;
                return _CD;
            }
        }

        [SkipField]
        protected static Image _GlassPlaque;

        protected static Image GlassPlaque
        {
            get
            {
                if (_GlassPlaque == null) _GlassPlaque = Resources.GlassPlaque;
                return _GlassPlaque;
            }
        }
        //[SkipField]
        //protected static Image TVOverlay = Resources.TVOverlay;

        [SkipField]
        protected static Image FilmOverlay = Resources.FilmOverlay;

        [SkipField]
        protected static Dictionary<string, Image> _InternalCase;
        protected static Dictionary<string, Image> InternalCase
        {
            get
            {
                if (_InternalCase == null)
                {
                    _InternalCase = new Dictionary<string, Image>() {
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
                        {"HD",Resources.HD},
                        {"Season",SeasonCase},
                        {"Series",SeriesCase},
                        {"Specials",SpecialsCase},
                        {"Trailer",TrailerCase},
                        {"Boxset",BoxsetCase}
                    };
                }
                return _InternalCase;
            }
        }

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
        protected static Dictionary<string, Image> _InternalClearCase3D;

        protected static Dictionary<string, Image> InternalClearCase3D
        {
            get
            {
                if (_InternalClearCase3D == null) _InternalClearCase3D = new Dictionary<string, Image>() {
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

                return _InternalClearCase3D;
            }
        }
    

        [SkipField]
        protected static Dictionary<string, Image> InternalOpenCase = new Dictionary<string, Image>() {
                        {"default", OpenCase},
                        {"BD",OpenCaseBD},
                        {"DVD",OpenCaseDVD},
                        //{"MKV",Resources.Mkv},
                        //{"WMV",Resources.Wmv},
                        //{"AVI",Resources.Avi},
                        {"HDDVD",OpenCaseHDDVD},
                        //{"DVRMS",Resources.dv},
                        //{"H264",Resources.h264},
                        //{"MPEG",Resources.Mpeg},
                        //{"MP4",Resources.Mpeg},
                        //{"DIVX",Resources.DivX},
                        //{"MOV",Resources.mov},
                        //{"XVID",Resources.Xvid},
                        {"HD",OpenCaseBD}
        };

        [SkipField]
        protected static Dictionary<string, Image> _InternalClearCase;
        protected static Dictionary<string, Image> InternalClearCase
        {
            get
            {
                if (_InternalClearCase == null)
                {
                    _InternalClearCase = new Dictionary<string, Image>() {
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
                        {"XVID",Resources.cc_xvid},
                        {"HD",ClearCaseBD},
                        {"Trailer",ClearCaseTrailer},
                        {"Boxset",ClearCaseBoxset},
                        {"Season",ClearCaseSeason},
                        {"Series",ClearCaseSeries},
                        {"Specials",ClearCaseSpecials},
                        {"Folder",ClearCase}
        };
                }
                return _InternalClearCase;
            }
        }

        public void Save()
        {
            this.settings.Write();
        }

        [SkipField]
        string file = "CoverArt.xml";

        [SkipField]
        XmlSettings<ImageSet> settings;

        public static void UnloadImages()
        {
            //cause all our images to unload from memory
            _ClearCase = null;
            _BDCase = null;
            _BDCase3D = null;
            _Border = null;
            _CD = null;
            _ClearCase3D = null;
            _ClearCaseBD = null;
            _ClearCaseBD3D = null;
            _ClearCaseDVD = null;
            _ClearCaseDVD3D = null;
            _ClearCaseHDDVD = null;
            _ClearCaseHDDVD3D = null;
            _ClearCaseBoxset = null;
            _ClearCaseSeason = null;
            _ClearCaseSeries = null;
            _ClearCaseSpecials = null;
            _Diamond = null;
            _DVDCase = null;
            _DVDCase3D = null;
            _Film = null;
            _Film3D = null;
            _HDDVDCase = null;
            _HDDVDCase3D = null;
            _StdCase = null;
            _StdCase3D = null;
            _SeriesCase = null;
            _SeasonCase = null;
            _TrailerCase = null;
            _SeasonCase = null;
            _SpecialsCase = null;
            _BoxsetCase = null;
            //_TV = null;
            _TVMB = null;
            _TVMB3D = null;
            _InternalCase = null;
            _InternalClearCase = null;
            _InternalClearCase3D = null;
            _GlassPlaque = null;
            _DiamondThumb = null;
            
            
        }

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
                case "CoverArtOpenCase":
                    //Internal OpenCase
                    Is3D = true;
                    Skew = new SkewRatios(0, .144, 1, 0, .2, 1, .95, .75);
                    FrameOnTop = true;
                    RootPosition = new Rectangle(70,72,510,767);
                    Overlay = BlankOverlay;
                    RoundCorners = true;
                    Frames = InternalOpenCase;
                    break;
                //case "CoverArtCaseBD":
                //    //Internal Case
                //    Is3D = false;
                //    FrameOnTop = false;
                //    RootPosition = caseRectangle;
                //    Overlay = StdOverlay;
                //    Frames = new Dictionary<string, Image>() {
                //        {"default", BDCase},
                //    };
                //    break;
                //case "CoverArtCaseDVD":
                //    //Internal Case
                //    Is3D = false;
                //    FrameOnTop = false;
                //    RootPosition = caseRectangle;
                //    Overlay = StdOverlay;
                //    Frames = new Dictionary<string, Image>() {
                //        {"default", DVDCase},
                //    };
                //    break;
                //case "CoverArtCaseMinimal":
                //    //Just the big 3
                //    Is3D = false;
                //    FrameOnTop = false;
                //    RootPosition = caseRectangle;
                //    Overlay = StdOverlay;
                //    Frames = new Dictionary<string, Image>() {
                //        {"default", StdCase},
                //        {"DVD",DVDCase},
                //        {"BD",BDCase},
                //        {"HDDVD",HDDVDCase},
                //        {"SD",DVDCase},
                //        {"HD",BDCase}
                //    };
                //    break;
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
                    //Is3D = false;
                    //FrameOnTop = false;
                    //RootPosition = new Rectangle(45, 35, 645, 345);
                    //Overlay = TVOverlay;
                    //Frames = new Dictionary<string, Image>() {
                    //    {"default", TV},
                    //};
                    //break;
                case "CoverArtTVMB":
                    //Internal TV with MB logo
                    Is3D = false;
                    FrameOnTop = true;
                    //RoundCorners = true;
                    RootPosition = new Rectangle(38,38,665,380);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", TVMB},
                    };
                    break;
                case "CoverArtTVMB3D":
                    //Internal TV with MB logo
                    Is3D = true;
                    FrameOnTop = true;
                    //RoundCorners = true;
                    RootPosition = new Rectangle(38, 42, 585, 378);
                    Skew = new SkewRatios(0, 0, 1, .130, 0, 1, 1, .910);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", TVMB3D},
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
                case "CoverArtFilm3D":
                    //Internal Film
                    Is3D = true;
                    FrameOnTop = true;
                    RootPosition = new Rectangle(95, 30, 485, 355);
                    Skew = new SkewRatios(0, 0, 1, .115, 0, 1, 1, .910);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", Film3D},
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
                //case "CoverArtClearCaseMinimal":
                //    //Internal ClearCase
                //    Is3D = false;
                //    FrameOnTop = true;
                //    RoundCorners = true;
                //    RootPosition = new Rectangle(75, 25, 470, 668);
                //    Overlay = BlankOverlay;
                //    Frames = new Dictionary<string, Image>() {
                //        {"default",ClearCase},
                //        {"DVD",ClearCaseDVD},
                //        {"BD",ClearCaseBD},
                //        {"HDDVD",ClearCaseHDDVD},
                //        {"HD",ClearCaseBD},
                //        {"SD",ClearCaseDVD}
                //    };
                //    break;
                //case "CoverArtClearCasePlain":
                //    //Internal ClearCase with just the plain one
                //    Is3D = false;
                //    FrameOnTop = true;
                //    RoundCorners = true;
                //    RootPosition = new Rectangle(75, 25, 470, 668);
                //    Overlay = BlankOverlay;
                //    Frames = new Dictionary<string, Image>() {
                //        {"default",ClearCase}
                //    };
                //    break;
                //case "CoverArtClearCaseBD":
                //    //Internal ClearCase with just the plain one
                //    Is3D = false;
                //    FrameOnTop = true;
                //    RoundCorners = true;
                //    RootPosition = new Rectangle(75, 25, 470, 668);
                //    Overlay = BlankOverlay;
                //    Frames = new Dictionary<string, Image>() {
                //        {"default",ClearCaseBD}
                //    };
                //    break;
                //case "CoverArtClearCaseDVD":
                //    //Internal ClearCase just DVD cover
                //    Is3D = false;
                //    FrameOnTop = true;
                //    RoundCorners = true;
                //    RootPosition = new Rectangle(75, 25, 470, 668);
                //    Overlay = BlankOverlay;
                //    Frames = new Dictionary<string, Image>() {
                //        {"default", ClearCaseDVD}
                //    };
                //    break;
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
                        {"default", Diamond},
                    };
                    break;
                case "CoverArtDiamondThumb":
                    //Landscape version
                    RoundCorners = true;
                    FrameOnTop = true;
                    RootPosition = new Rectangle(35, 15, 640, 370);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", DiamondThumb},
                    };
                    break;
                case "CoverArtPlaque":
                    //A glass plaque effect (good for people)
                    RoundCorners = false;
                    FrameOnTop = true;
                    RootPosition = new Rectangle(83, 83, 382, 585);
                    Overlay = BlankOverlay;
                    Frames = new Dictionary<string, Image>() {
                        {"default", GlassPlaque}
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
                    else
                    {
                        Logger.ReportError("CoverArt ImageSet does not exist: " + directory);
                        //Ignore 
                        RootPosition = new Rectangle(0, 0, 0, 0);
                        Overlay = BlankOverlay;
                        Frames = new Dictionary<string, Image>() {
                            {"default", BlankImage}
                        };
                    }

                    break;
            }
            //add in the special folder frame if not there
            if (!Frames.ContainsKey("Folder"))
            {
                Frames.Add("Folder", Frames["default"]);
            }
        }
    }
}
