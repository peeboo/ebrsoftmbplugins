using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library;


namespace CoverArt
{
    public class Profile
    {
        protected Dictionary<string, ImageSet> imageSets;
        protected Dictionary<string, string> typeMap = new Dictionary<string, string>();
        protected string movieLocation = "CoverArtCase";
        protected string seriesLocation = "CoverArtCase";
        protected string seasonLocation = "CoverArtCase";
        protected string episodeLocation = "CoverArtTVMB";
        protected string remoteLocation = "CoverArtClearCase";
        protected string thumbLocation = "CoverArtFilm";
        protected string albumLocation = "CoverArtCD";
        protected string folderLocation = "Ignore";


        public bool CoverByDefinition = false;


        public Profile()
        {
            init(null, null, null, null, null, null, null, null, false, null);
        }
        public Profile(string movieLocation, string seriesLocation, string seasonLocation, string episodeLocation, string remoteLocation, string thumbLocation, string albumLocation, string folderLocation, bool coverByDef, Dictionary<string,string> typeMap)
        {
            init(movieLocation, seriesLocation, seasonLocation, episodeLocation, remoteLocation, thumbLocation, albumLocation, folderLocation, coverByDef, typeMap);
        }

        private void init(string movieLocation, string seriesLocation, string seasonLocation, string episodeLocation, string remoteLocation, string thumbLocation, string albumLocation, string folderLocation, bool coverByDef, Dictionary<string,string> typeMap)
        {

            if (!String.IsNullOrEmpty(movieLocation))
            {
                this.movieLocation = movieLocation;
            }
            if (!String.IsNullOrEmpty(seriesLocation))
            {
                this.seriesLocation = seriesLocation;
            }

            if (!String.IsNullOrEmpty(seasonLocation))
            {
                this.seasonLocation = seasonLocation;
            }

            if (!String.IsNullOrEmpty(episodeLocation))
            {
                this.episodeLocation = episodeLocation;
            }

            if (!String.IsNullOrEmpty(remoteLocation))
            {
                this.remoteLocation = remoteLocation;
            }

            if (!String.IsNullOrEmpty(thumbLocation))
            {
                this.thumbLocation = thumbLocation;
            }

            if (!String.IsNullOrEmpty(albumLocation))
            {
                this.albumLocation = albumLocation;
            }

            if (!String.IsNullOrEmpty(folderLocation))
            {
                this.folderLocation = folderLocation;
            }

            //CreateImageSets();
            CoverByDefinition = coverByDef;

            this.typeMap = typeMap;

        }

        protected void CreateImageSets()
        {
            //Create all our imagesets
            imageSets = new Dictionary<string, ImageSet>();
            imageSets.Add("default", new ImageSet("CoverArtCase"));
            imageSets.Add("movie", new ImageSet(movieLocation));
            imageSets.Add("remote", new ImageSet(remoteLocation));
            imageSets.Add("series", new ImageSet(seriesLocation));
            imageSets.Add("season", new ImageSet(seasonLocation));
            imageSets.Add("episode", new ImageSet(episodeLocation));
            imageSets.Add("album", new ImageSet(albumLocation));
            imageSets.Add("thumb", new ImageSet(thumbLocation));
            imageSets.Add("folder", new ImageSet(folderLocation));
        }

        public void ClearImageSets()
        {
            //clear out our image sets to free up memory
            if (imageSets != null)
            {
                imageSets.Clear();
                imageSets = null;
            }
        }

        public string Translate(string type)
        {
            if (typeMap.ContainsKey(type))
                if (typeMap[type] == "default" && typeMap.ContainsKey("default"))
                    return typeMap["default"]; //if items are translated to default AND default is itself translated, then return the translated one
                else
                    return typeMap[type];
            else return type;
        }

        public ImageSet GetImageSet(string location)
        {

            if (imageSets.ContainsKey(location))
                return imageSets[location];
            else 
                return imageSets["default"];
        }

        public void SetImageSet(string type, string location)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            imageSets[type] = new ImageSet(location);
        }

        public List<ImageSet> ImageSetList
        {
            get
            {
                return imageSets.Values.ToList();
            }
        }

        protected Dictionary<string,ImageSet> ImageSets
        {
            get
            {
                if (imageSets == null) CreateImageSets(); //in case we unloaded these
                return imageSets;
            }
        }

        public Image Frame(string type)
        {
            return Frame(type, "default");
        }

        public Image Frame(string type, string subtype)
        {
            lock (ImageSets)
            {
                if (!ImageSets.ContainsKey(type)) type = "default";
                if (!ImageSets[type].Frames.ContainsKey(subtype)) subtype = "default";
                Logger.ReportInfo("Getting frame for " + type + "/" + subtype);
                return new Bitmap(ImageSets[type].Frames[subtype]);
            }
        }

        public Rectangle RootPosition(string type)
        {
            if (!ImageSets.ContainsKey(type)) type = "default";
            return ImageSets[type].RootPosition;
        }

        public Image Overlay(string type)
        {
            lock (ImageSets)
            {
                if (!ImageSets.ContainsKey(type)) type = "default";
                return new Bitmap(ImageSets[type].Overlay);
            }
        }

        public bool Is3D(string type) {
            if (!ImageSets.ContainsKey(type)) type = "default";
            return ImageSets[type].Is3D;
        }

        public SkewRatios Skew(string type)
        {
            if (!ImageSets.ContainsKey(type)) type = "default";
            return ImageSets[type].Skew;
        }

        public bool FrameOnTop(string type)
        {
            if (!ImageSets.ContainsKey(type)) type = "default";
            return ImageSets[type].FrameOnTop;
        }

        public bool JustRoundCorners(string type)
        {
            if (!ImageSets.ContainsKey(type)) type = "default";
            return ImageSets[type].JustRoundCorners;
        }

        public bool RoundCorners(string type)
        {
            if (!ImageSets.ContainsKey(type)) type = "default";
            return ImageSets[type].RoundCorners;
        }

        public Image MovieOverlay()
        {
            return Overlay("movie");
        }
        public Image SeriesOverlay()
        {
            return Overlay("series");
        }
        public Image SeasonOverlay()
        {
            return Overlay("season");
        }
        public Image EpisodeOverlay()
        {
            return Overlay("episode");
        }
        public Image RemoteOverlay()
        {
            return Overlay("remote");
        }
        public Image FolderOverlay()
        {
            return Overlay("folder");
        }
        public Image ThumbOverlay()
        {
            return Overlay("thumb");
        }
        public Image AlbumOverlay()
        {
            return Overlay("album");
        }

        public Image MovieFrame(string type)
        {
            return Frame("movie", Translate(type));
        }

        public Image SeasonFrame(string type)
        {
            return Frame("season", Translate(type));
        }

        public Image SeriesFrame(string type)
        {
            return Frame("series", Translate(type));
        }

        public Image EpisodeFrame(string type)
        {
            return Frame("episode", Translate(type));
        }

        public Image RemoteFrame(string type)
        {
            return Frame("remote", Translate(type));
        }

        public Image ThumbFrame(string type)
        {
            return Frame("thumb", Translate(type));
        }

        public Image AlbumFrame(string type)
        {
            return Frame("album", Translate(type));
        }

        public Image FolderFrame(string type)
        {
            return Frame("folder", Translate(type));
        }
    }
}
