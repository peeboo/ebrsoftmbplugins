using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using System.IO;
using MediaBrowser.Library.Logging;


namespace MBSettingSaver
{
    //******************************************************************************************************************
    //  This class is used to house your configuration parameters.  It is a decendant of ModelItem and will be passed 
    //  into your config panel at runtime (see configpanel setup in plugin.cs) so that you can bind to its properties.
    //******************************************************************************************************************

    public class MyConfig : ModelItem
    {
        public MyConfig() {
        configData = MyConfigData.FromFile(System.IO.Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppPluginPath,"Configurations\\MBSettingSaverExt.xml"));
        }

        private MyConfigData configData;

        public List<string> SavedSettingNames
        {
            get { return configData.SavedSettingNames; }
            set { if (configData.SavedSettingNames != value) { configData.SavedSettingNames = value; configData.Save(); FirePropertyChanged("SavedSettingNames"); } }
        }

        public string SavePath
        {
            get { return configData.SavePath; }
            set { if (configData.SavePath != value) { configData.SavePath = value; configData.Save(); FirePropertyChanged("SavePath"); } }
        }

        public string LastRestoredName
        {
            get { return configData.LastRestoredName; }
            set { if (configData.LastRestoredName != value) { configData.LastRestoredName = value; configData.Save(); FirePropertyChanged("LastRestoredName"); } }
        }

        public void ReLoad()
        {
            //this is necessary due to the fact that our mcml panel seems to create a new instance of the config object each time
            configData.Load();
            FirePropertyChanged("SavedSettingNames");
        }

        public void WaitOne()
        {
            System.Threading.Thread.Sleep(1000);
        }

        //This is easiest place to put this
        public void SaveSettings(string settingName)
        {
            if (!String.IsNullOrEmpty(settingName))
            {
                try
                {
                    //strip out bad chars
                    string safeName = MediaBrowser.LibraryManagement.Helper.RemoveInvalidFileChars(settingName);
                    string fullName = Path.Combine(SavePath, safeName);
                    string pluginPath = Path.Combine(fullName, "plugins");
                    string displayPath = Path.Combine(fullName, "display");
                    //create directory and save
                    if (Directory.Exists(fullName)) Directory.Delete(fullName, true);
                    Directory.CreateDirectory(fullName);
                    Directory.CreateDirectory(pluginPath);
                    Directory.CreateDirectory(displayPath);
                    foreach (string file in Directory.GetFiles(Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppPluginPath,"Configurations")))
                    {
                        File.Copy(file, Path.Combine(pluginPath, Path.GetFileName(file)),true);
                    }
                    foreach (string file in Directory.GetFiles(Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppCachePath, "display")))
                    {
                        File.Copy(file, Path.Combine(displayPath, Path.GetFileName(file)),true);
                    }
                    File.Copy(MediaBrowser.Library.Configuration.ApplicationPaths.ConfigFile, Path.Combine(fullName, Path.GetFileName(MediaBrowser.Library.Configuration.ApplicationPaths.ConfigFile)),true);
                    if (settingName != "MBSBackup")
                    {
                        MediaCenterEnvironment env = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                        env.Dialog(settingName + " Saved.", "Save Settings", DialogButtons.Ok, 5, false);
                    }

                }
                catch (Exception ex)
                {
                    Logger.ReportException("Error saving settings.", ex);
                }

                //and add to our list
                if (settingName != "MBSBackup" && !SavedSettingNames.Contains(settingName))
                {
                    SavedSettingNames.Add(settingName);
                    configData.Save();
                    FirePropertyChanged("SavedSettingNames");
                }
            }
        }
        public void RestoreSettings(string settingName)
        {
            if (!String.IsNullOrEmpty(settingName))
            {
                //save a backup
                SaveSettings("MBSBackup");
                try
                {
                    //strip out bad chars
                    string safeName = MediaBrowser.LibraryManagement.Helper.RemoveInvalidFileChars(settingName);
                    string fullName = Path.Combine(SavePath, safeName);
                    string pluginPath = Path.Combine(fullName, "plugins");
                    string displayPath = Path.Combine(fullName, "display");
                    //create directory and save
                    if (!Directory.Exists(fullName))
                    {
                        Logger.ReportError("Unable to find saved settings: " + settingName);
                        return;
                    }
                    MediaCenterEnvironment env = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                    DialogResult result = env.Dialog("This will overwrite your current settings and exit Media Browser. Proceed?", "Restore "+settingName, DialogButtons.Yes | DialogButtons.No, 20, true);
                    if (result == DialogResult.No) return;

                    foreach (string file in Directory.GetFiles(pluginPath))
                    {
                        if (!Path.GetFileName(file).ToLower().StartsWith("mbsettingsaver"))
                        { //don't copy over our config
                            File.Copy(file, Path.Combine(Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppPluginPath, "Configurations"), Path.GetFileName(file)), true);
                        }
                    }
                    foreach (string file in Directory.GetFiles(displayPath))
                    {
                        File.Copy(file, Path.Combine(Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppCachePath, "display"), Path.GetFileName(file)),true);
                    }
                    File.Copy(Path.Combine(fullName, Path.GetFileName(MediaBrowser.Library.Configuration.ApplicationPaths.ConfigFile)), MediaBrowser.Library.Configuration.ApplicationPaths.ConfigFile, true);
                    LastRestoredName = settingName;
                    MediaBrowser.Application.CurrentInstance.Close();
                }
                catch (Exception ex)
                {
                    Logger.ReportException("Error restoring settings.", ex);
                }

            }
        }
        public void DeleteSettings(string settingName)
        {
            if (!String.IsNullOrEmpty(settingName))
            {
                try
                {
                    //strip out bad chars
                    string safeName = MediaBrowser.LibraryManagement.Helper.RemoveInvalidFileChars(settingName);
                    string fullName = Path.Combine(SavePath, safeName);
                    if (!Directory.Exists(fullName))
                    {
                        Logger.ReportError("Unable to find saved settings: " + settingName);
                        return;
                    }
                    MediaCenterEnvironment env = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
                    DialogResult result = env.Dialog("Delete Saved Settings "+settingName+"?", "Delete ", DialogButtons.Yes | DialogButtons.No, 20, true);
                    if (result == DialogResult.No) return;

                    Directory.Delete(fullName, true);
                }
                catch (Exception ex)
                {
                    Logger.ReportException("Error deleting settings.", ex);
                }
                if (SavedSettingNames.Contains(settingName))
                {
                    SavedSettingNames.Remove(settingName);
                    configData.Save();
                    FirePropertyChanged("SavedSettingNames");
                }
            }
        }
    }
}
