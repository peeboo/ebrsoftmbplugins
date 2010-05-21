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
        protected Dictionary<string, ImageSet> imageSets = new Dictionary<string, ImageSet>();

        public bool CoverByDefinition = false;


        public Profile()
        {
            init(null, null, null, null, null, null, null, null, false);
        }
        public Profile(string movieLocation, string seriesLocation, string seasonLocation, string episodeLocation, string remoteLocation, string thumbLocation, string albumLocation, string folderLocation, bool coverByDef)
        {
            init(movieLocation, seriesLocation, seasonLocation, episodeLocation, remoteLocation, thumbLocation, albumLocation, folderLocation, coverByDef);
        }

        private void init(string movieLocation, string seriesLocation, string seasonLocation, string episodeLocation, string remoteLocation, string thumbLocation, string albumLocation, string folderLocation, bool coverByDef)
        {
            if (String.IsNullOrEmpty(movieLocation))
            {
                movieLocation = "CoverArtCase";
            }
            if (String.IsNullOrEmpty(seriesLocation))
            {
                seriesLocation = "CoverArtCase";
            }

            if (String.IsNullOrEmpty(seasonLocation))
            {
                seasonLocation = "CoverArtCase";
            }

            if (String.IsNullOrEmpty(episodeLocation))
            {
                episodeLocation = "CoverArtTVMB";
            }

            if (String.IsNullOrEmpty(remoteLocation))
            {
                remoteLocation = "CoverArtClearCase";
            }

            if (String.IsNullOrEmpty(thumbLocation))
            {
                thumbLocation = "CoverArtFilm";
            }

            if (String.IsNullOrEmpty(albumLocation))
            {
                albumLocation = "CoverArtCD";
            }

            if (String.IsNullOrEmpty(folderLocation))
            {
                albumLocation = "Ignore";
            }

            CoverByDefinition = coverByDef;

            //Create all our imagesets
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

        public List<ImageSet> ImageSets
        {
            get
            {
                return imageSets.Values.ToList();
            }
        }

        public Image Frame(string type)
        {
            return Frame(type, "default");
        }

        public Image Frame(string type, string subtype)
        {
            lock (imageSets)
            {
                if (!imageSets.ContainsKey(type)) type = "default";
                if (!imageSets[type].Frames.ContainsKey(subtype)) subtype = "default";
                Logger.ReportInfo("Getting frame for " + type + "/" + subtype);
                return new Bitmap(imageSets[type].Frames[subtype]);
            }
        }

        public Rectangle RootPosition(string type)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].RootPosition;
        }

        public Image Overlay(string type)
        {
            lock (imageSets)
            {
                if (!imageSets.ContainsKey(type)) type = "default";
                return new Bitmap(imageSets[type].Overlay);
            }
        }

        public bool Is3D(string type) {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].Is3D;
        }

        public SkewRatios Skew(string type)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].Skew;
        }

        public bool FrameOnTop(string type)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].FrameOnTop;
        }

        public bool JustRoundCorners(string type)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].JustRoundCorners;
        }

        public bool RoundCorners(string type)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].RoundCorners;
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
            return Frame("movie", type);
        }

        public Image SeasonFrame(string type)
        {
            return Frame("season", type);
        }

        public Image SeriesFrame(string type)
        {
            return Frame("series", type);
        }

        public Image EpisodeFrame(string type)
        {
            return Frame("episode", type);
        }

        public Image RemoteFrame(string type)
        {
            return Frame("remote", type);
        }

        public Image ThumbFrame(string type)
        {
            return Frame("thumb", type);
        }

        public Image AlbumFrame(string type)
        {
            return Frame("album", type);
        }

        public Image FolderFrame(string type)
        {
            return Frame("folder", type);
        }
    }
}
