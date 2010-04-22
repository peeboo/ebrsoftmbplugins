using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoverArt
{
    public class ProfileDefinition
    {
        public ProfileDefinition() { }

        public string Directory = "default";
        public string MovieLocation = "CoverArtCase";
        public string SeriesLocation = "CoverArtCase";
        public string SeasonLocation = "CoverArtCase";
        public string EpisodeLocation = "CoverArtTVMB";
        public string RemoteLocation = "CoverArtClearCase";
        public string ThumbLocation = "CoverArtFilm";
        public string AlbumLocation = "CoverArtCD";
        public string FolderLocation = "Ignore";
        public bool CoverByDefinition = false;

        public override string ToString()
        {
            return this.Directory;
        }
    }
}
