using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoverArt
{
    public class MyConfigData
    {
        #region constructor
        public MyConfigData()
        {
        }
        public MyConfigData(string file)
        {
            this.file = file;
            this.settings = XmlSettings<MyConfigData>.Bind(this, file);
            //translate defintions to actual profiles
            foreach (ProfileDefinition def in ProfileDefs)
            {
                Profiles.Add(def.Directory.ToLower(), new Profile(def.MovieLocation, def.SeriesLocation, def.SeasonLocation, def.EpisodeLocation, def.RemoteLocation, def.ThumbLocation, def.AlbumLocation, def.FolderLocation, def.CoverByDefinition));
            }
            //and add in default if not already there
            if (!Profiles.ContainsKey("default"))
                Profiles.Add("default", new Profile());
        }
        #endregion
        [SkipField]
        public Dictionary<string, Profile> Profiles = new Dictionary<string, Profile>();

        public List<ProfileDefinition> ProfileDefs = new List<ProfileDefinition>();
        public List<string> IgnoreFolders = new List<string>();
        public List<string> CustomImageSets = new List<string>();
        public string LastConfigVersion = "0.0.0.0";
        public string RegKey = "";

        #region Load / Save Data
        public static MyConfigData FromFile(string file)
        {
            return new MyConfigData(file);
        }

        public void Save()
        {
            this.settings.Write();
        }

        [SkipField]
        string file;

        [SkipField]
        XmlSettings<MyConfigData> settings;
        #endregion

    }
}
