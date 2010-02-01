using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library;


namespace CoverArt
{
    public class Profile
    {
        protected Dictionary<string, ImageSet> imageSets = new Dictionary<string, ImageSet>();


        public Profile()
        {
            init(null, null, null, null, null, null, null);
        }
        public Profile(string movieLocation, string seriesLocation, string seasonLocation, string episodeLocation, string remoteLocation, string thumbLocation, string albumLocation)
        {
            init(movieLocation, seriesLocation, seasonLocation, episodeLocation, remoteLocation, thumbLocation, albumLocation);
        }

        private void init(string movieLocation, string seriesLocation, string seasonLocation, string episodeLocation, string remoteLocation, string thumbLocation, string albumLocation)
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
                episodeLocation = "CoverArtTV";
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

            //Create all our imagesets
            imageSets.Add("default", new ImageSet("CoverArtCase"));
            imageSets.Add("movie", new ImageSet(movieLocation));
            imageSets.Add("remote", new ImageSet(remoteLocation));
            imageSets.Add("series", new ImageSet(seriesLocation));
            imageSets.Add("season", new ImageSet(seasonLocation));
            imageSets.Add("episode", new ImageSet(episodeLocation));
            imageSets.Add("album", new ImageSet(albumLocation));
            imageSets.Add("thumb", new ImageSet(thumbLocation));
        }

        public Image Frame(string type)
        {
            return Frame(type, "default");
        }

        public Image Frame(string type, string subtype)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            if (!imageSets[type].Frames.ContainsKey(subtype)) subtype = "default";
            Logger.ReportInfo("Getting frame for " + type + "/" + subtype);
            return imageSets[type].Frames[subtype];
        }

        public Rectangle RootPosition(string type)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].RootPosition;
        }

        public Image Overlay(string type)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].Overlay;
        }

        public bool Is3D(string type) {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].Is3D;
        }

        public bool FrameOnTop(string type)
        {
            if (!imageSets.ContainsKey(type)) type = "default";
            return imageSets[type].FrameOnTop;
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

    }
}
