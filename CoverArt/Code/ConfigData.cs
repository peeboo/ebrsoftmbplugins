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
            bool changed = false;
            //translate defintions to actual profiles
            foreach (ProfileDefinition def in ProfileDefs)
            {
                changed = changed | Migrate(def);
                Profiles.Add(def.Directory.ToLower(), new Profile(def.MovieLocation, def.SeriesLocation, def.SeasonLocation, def.EpisodeLocation, def.RemoteLocation, def.ThumbLocation, def.AlbumLocation, def.FolderLocation, def.CoverByDefinition, def.TypeMap));
            }
            //and add in default if not already there
            if (!Profiles.ContainsKey("default"))
                Profiles.Add("default", new Profile());
            if (changed) this.Save();
        }
        #endregion
        [SkipField]
        public Dictionary<string, Profile> Profiles = new Dictionary<string, Profile>();

        public List<ProfileDefinition> ProfileDefs = new List<ProfileDefinition>();
        public List<string> IgnoreFolders = new List<string>();
        public List<string> CustomImageSets = new List<string>();
        public string LastConfigVersion = "0.0.0.0";
        public string RegKey = "";
        public bool IgnoreTopFolders = true;
        public int MemoryReleaseInterval = 120; //in seconds

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

        private bool Migrate(ProfileDefinition def)
        {
            bool changed = false;
            switch (def.MovieLocation)
            {
                case "CoverArtCaseMinimal":
                    def.TypeMap.Clear();
                    foreach (string format in ImageSet.FrameTypes)
                    {
                        List<string> big3 = new List<string>() { "default", "DVD", "BD", "HDDVD", "HD" };
                        if (!big3.Contains(format))
                            def.TypeMap.Add(format, "default");
                    }
                    def.MovieLocation = "CoverArtCase";
                    changed = true;
                    break;

                case "CoverArtCaseBD":
                    def.TypeMap.Clear();
                    foreach (string format in CoverArt.ImageSet.FrameTypes)
                    {
                        def.TypeMap.Add(format, "BD");
                    }
                    def.MovieLocation = "CoverArtCase";
                    changed = true;
                    break;

                case "CoverArtCaseDVD":
                    def.TypeMap.Clear();
                    foreach (string format in CoverArt.ImageSet.FrameTypes)
                    {
                        def.TypeMap.Add(format, "DVD");
                    }
                    def.MovieLocation = "CoverArtCase";
                    changed = true;
                    break;

                case "CoverArtClearCasePlain":
                    def.TypeMap.Clear();
                    foreach (string format in ImageSet.FrameTypes)
                    {
                        if (format != "default")
                            def.TypeMap.Add(format, "default");
                    }
                    def.MovieLocation = "CoverArtClearCase";
                    changed = true;
                    break;

                case "CoverArtClearCaseMinimal":
                    def.TypeMap.Clear();
                    foreach (string format in ImageSet.FrameTypes)
                    {
                        List<string> big3 = new List<string>() { "default", "DVD", "BD", "HDDVD", "HD" };
                        if (!big3.Contains(format))
                            def.TypeMap.Add(format, "default");
                    }
                    def.MovieLocation = "CoverArtClearCase";
                    changed = true;
                    break;

                case "CoverClearArtCaseBD":
                    def.TypeMap.Clear();
                    foreach (string format in CoverArt.ImageSet.FrameTypes)
                    {
                        def.TypeMap.Add(format, "BD");
                    }
                    def.MovieLocation = "CoverArtClearCase";
                    changed = true;
                    break;

                case "CoverArtClearCaseDVD":
                    def.TypeMap.Clear();
                    foreach (string format in CoverArt.ImageSet.FrameTypes)
                    {
                        def.TypeMap.Add(format, "DVD");
                    }
                    def.MovieLocation = "CoverArtClearCase";
                    changed = true;
                    break;

            }

            //now get the rest of them
            changed = changed | MigrateLocation(ref def.SeriesLocation);
            changed = changed | MigrateLocation(ref def.SeasonLocation);
            changed = changed | MigrateLocation(ref def.EpisodeLocation);
            changed = changed | MigrateLocation(ref def.RemoteLocation);
            changed = changed | MigrateLocation(ref def.ThumbLocation);
            changed = changed | MigrateLocation(ref def.AlbumLocation);
            changed = changed | MigrateLocation(ref def.FolderLocation);

            return changed;
        }

        private bool MigrateLocation(ref string location) {
                    bool changed = false;
            switch (location)
            {
                case "CoverArtCaseMinimal":
                    location = "CoverArtCase";
                    changed = true;
                    break;

                case "CoverArtCaseBD":
                    location = "CoverArtCase";
                    changed = true;
                    break;

                case "CoverArtCaseDVD":
                    location = "CoverArtCase";
                    changed = true;
                    break;

                case "CoverArtClearCasePlain":
                    location = "CoverArtClearCase";
                    changed = true;
                    break;

                case "CoverArtClearCaseMinimal":
                    location = "CoverArtClearCase";
                    changed = true;
                    break;

                case "CoverClearArtCaseBD":
                    location = "CoverArtClearCase";
                    changed = true;
                    break;

                case "CoverArtClearCaseDVD":
                    location = "CoverArtClearCase";
                    changed = true;
                    break;

            }
            return changed;
        }


    }
}
