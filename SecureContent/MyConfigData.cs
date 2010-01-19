using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaBrowser.Library.Persistance;
using MediaBrowser.Library.Configuration;

namespace SecureContent
{
    [Serializable]
    public class MyConfigData
    {
        public MyConfigData() {}

        public MyConfigData(string file)
        {
            this.file = file;
            this.settings = XmlSettings<MyConfigData>.Bind(this, file);
        }

        public bool Locked = true;
        public int UnlockPeriod = 3;
        public string Name = "Secure";
        public string UnlockCode = "1234";
        public string Path = "";

        [SkipField]
        string file;

        [SkipField]
        XmlSettings<MyConfigData> settings;


        public static MyConfigData FromFile(string file)
        {
            return new MyConfigData(file);
        }

        public void Save()
        {
            this.settings.Write();
        }

    }
}
