using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaBrowser.Library.Logging;
using MediaBrowser.Library.Configuration;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using MediaBrowser.Library;
using MediaBrowser.Code;
using System.IO;


namespace SecureContent
{
    public class MyConfig : ModelItem
    {
        public MyConfig()
        {
            isValid = Load();
        }
        

        private MyConfigData data;
        private bool isValid;

        private void Save()
        {
            lock (this)
                this.data.Save();
        }

        private bool Load()
        {
            try
            {
                this.data = MyConfigData.FromFile(System.IO.Path.Combine(ApplicationPaths.AppConfigPath,"SecureContent.config"));
                return true;
            }
            catch (Exception ex)
            {
                MediaCenterEnvironment ev = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                DialogResult r = ev.Dialog(ex.Message + "\nReset to default?", "Error in configuration file", DialogButtons.Yes | DialogButtons.No, 600, true);
                if (r == DialogResult.Yes)
                {
                    this.data = new MyConfigData();
                    Save();
                    return true;
                }
                else
                    return false;
            }
        }


        public bool Locked { get; set; }
        public int UnlockPeriod { get { return data.UnlockPeriod; } set { if (data.UnlockPeriod != value) { data.UnlockPeriod = value; FirePropertyChanged("UnlockPeriod"); } } }
        public string Path { get { return data.Path; } set { if (data.Path != value) { data.Path = value; FirePropertyChanged("Path"); } } }
        public string UnlockCode { get { return data.UnlockCode; } set { if (data.UnlockCode != value) { data.UnlockCode = value; FirePropertyChanged("UnlockCode"); } } }

        public void Unlock()
        {
            Kernel.Instance.RootFolder.AddVirtualChild(SecureFolder.Instance);
            SecureFolder.Instance.Path = "\\\\mediaserver\\movies\\test";
            SecureFolder.Instance.CustomRating = "NC-17"; //assume high rating
            SecureFolder.Instance.RefreshMetadata();
            MediaBrowser.Application.CurrentInstance.RootFolderModel.RefreshUI();
            
        }

        public void Relock()
        {
            Kernel.Instance.RootFolder.RemoveVirtualChild(SecureFolder.Instance);
            MediaBrowser.Application.CurrentInstance.RootFolder.RefreshMetadata();
            MediaBrowser.Application.CurrentInstance.RootFolderModel.RefreshUI();
        }
    }
}
