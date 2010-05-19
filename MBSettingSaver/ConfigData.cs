using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaBrowser.Library.Persistance;
using MediaBrowser.Library.Configuration;

namespace MBSettingSaver
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
        }
        #endregion
        public string LastRestoredName = "";
        public List<string> SavedSettingNames = new List<string>();
        public string SavePath = "";

        #region Load / Save Data
        public static MyConfigData FromFile(string file)
        {
            return new MyConfigData(file);
        }

        public void Load()
        {
            //re-bind
            this.settings = XmlSettings<MyConfigData>.Bind(this, file);
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
