﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using CoverArt;
using MediaBrowser;
using MediaBrowser.Library.Extensions;


namespace CoverArtConfig
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<PreviewItem>> PreviewItems = new Dictionary<string,List<PreviewItem>>();
        private ImageSet currentImageSet;
        private string currentImageSetName;
        private bool processChanges = true;
        private Dictionary<string, ImageSet> imageSets = new Dictionary<string, ImageSet>();
        private List<string> imageSetOptions = new List<string>();
        private List<string> builtinImageSets = new List<string>() {
            "CoverArtCase",
            //"CoverArtCaseMinimal",
            //"CoverArtCaseBD",
            //"CoverArtCaseDVD",
            "CoverArtCase3D",
            "CoverArtClearCase",
            //"CoverArtClearCasePlain",
            //"CoverArtClearCaseMinimal",
            //"CoverArtClearCaseDVD",
            //"CoverArtClearCaseBD",
            "CoverArtClearCase3D",
            "CoverArtOpenCase",
            "CoverArtCD",
            "CoverArtDiamond",
            "CoverArtDiamondThumb",
            "CoverArtPlaque",
            "CoverArtRounded",
            "CoverArtTV",
            "CoverArtTVMB",
            "CoverArtTVMB3D",
            "CoverArtFilm",
            "CoverArtFilm3D",
            "CoverArtBorder",
            "Ignore"
            
        };
        private MyConfigData config;
        private bool isReg = false;
        private DateTime expDate;
        private string regString
        {
            get
            {
                if (isReg)
                    return " (Registered)";
                else
                    if (expDate > DateTime.Now)
                        return " (Trial - Expires in " + ((expDate - DateTime.Now).Days + 1) + " Days)";
                    else
                        return " (Expired)";
            }
        }
        private string regBtnText
        {
            get
            {
                if (isReg)
                    return "Edit Key...";
                else
                    return "Register...";
            }
        }

        private ProfileDefinition currentProfileDef
        {
            get
            {
                return lbxProfiles.SelectedItem as ProfileDefinition ?? new ProfileDefinition(); // just return something valid so we don't have to check each time
            }
        }

        public MainWindow()
        {
            config = MyConfigData.FromFile(System.IO.Path.Combine(MediaBrowser.Library.Configuration.ApplicationPaths.AppPluginPath, "Configurations\\Coverart.xml"));
            isReg = CoverArt.Plugin.Validate(config.RegKey);
            expDate = CoverArt.Plugin.Ping("http://www.ebrsoft.com/software/mb/plugins/expdate.php?product=CoverArt2&mac="+CAHelper.GetMACAddress());

            InitializeComponent();
            refreshImageSetOptions();
            ProfileDefinition def = config.ProfileDefs.Find(p => p.Directory == "default");
            if (def == null)
            {
                config.ProfileDefs.Insert(0,new ProfileDefinition());
            }
            lbxProfiles.ItemsSource = config.ProfileDefs;
            setVersion();
        }

        private void setVersion()
        {
            version.Content = "Version " + CoverArt.Plugin.CurrentVersion + regString;
            btnRegister.Content = regBtnText;
        }


        private void refreshImageSetOptions()
        {
            imageSetOptions = new List<string>();
            imageSetOptions.AddRange(builtinImageSets);
            imageSetOptions.AddRange(config.CustomImageSets);
            ddlMovieLocation.ItemsSource = imageSetOptions;
            ddlSeriesLocation.ItemsSource = imageSetOptions;
            ddlSeasonLocation.ItemsSource = imageSetOptions;
            ddlEpisodeLocation.ItemsSource = imageSetOptions;
            ddlRemoteLocation.ItemsSource = imageSetOptions;
            ddlThumbLocation.ItemsSource = imageSetOptions;
            ddlAlbumLocation.ItemsSource = imageSetOptions;
            ddlFolderLocation.ItemsSource = imageSetOptions;
        }

        private List<string> getCustomImageSets()
        {
            List<string> customSets = new List<string>();
            foreach (Profile profile in config.Profiles.Values)
            {
                foreach (ImageSet imageSet in profile.ImageSetList)
                {
                    if (!builtinImageSets.Contains(imageSet.Name)) customSets.Add(imageSet.Name);
                }
            }
            return customSets;
        }
                
        public static string TempLocation
        {
            get
            {
                string tempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "CoverArt");
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
                return tempDir;
            }
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lbxProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxProfiles != null && lbxProfiles.SelectedItem != null)
            {
                ProfileDefinition lastProfile;
                if (e.RemovedItems.Count > 0)
                    lastProfile = e.RemovedItems[0] as ProfileDefinition;
                else
                    lastProfile = new ProfileDefinition();

                ProfileDefinition thisProfile = e.AddedItems[0] as ProfileDefinition;
                if (lastProfile.TypeMap.Count > 0 || thisProfile.TypeMap.Count > 0)
                {
                    //either the last selection or the new one has a type map so we can't re-use the previews
                    PreviewItems.Clear();
                }

                loadProfile(lbxProfiles.SelectedItem as ProfileDefinition);
            }
        }

        private bool loadProfile(ProfileDefinition profileDef)
        {
            processChanges = false; //don't want change events to fire
            ddlMovieLocation.SelectedItem = profileDef.MovieLocation;
            ddlSeasonLocation.SelectedItem = profileDef.SeasonLocation;
            ddlSeriesLocation.SelectedItem = profileDef.SeriesLocation;
            ddlEpisodeLocation.SelectedItem = profileDef.EpisodeLocation;
            ddlRemoteLocation.SelectedItem = profileDef.RemoteLocation;
            ddlThumbLocation.SelectedItem = profileDef.ThumbLocation;
            ddlAlbumLocation.SelectedItem = profileDef.AlbumLocation;
            ddlFolderLocation.SelectedItem = profileDef.FolderLocation;
            cbxByDef.IsChecked = profileDef.CoverByDefinition;
            processChanges = true;
            setCurrentImageSet();

            //create previews
            loadPreviews();
            return true;
        }
        private System.Drawing.Image exampleCover
        {
            get
            {
                switch (tabImageSet.SelectedIndex)
                {
                    case 0:
                    case 4:
                        return (System.Drawing.Image)CoverArtConfig.Resources.cover1.Clone();
                    case 1:
                    case 2:
                        return (System.Drawing.Image)CoverArtConfig.Resources.tvseries.Clone();
                    case 3:
                        return (System.Drawing.Image)CoverArtConfig.Resources.episode.Clone();
                    case 5:
                        return (System.Drawing.Image)CoverArtConfig.Resources.thumb.Clone();
                    case 6:
                        return (System.Drawing.Image)CoverArtConfig.Resources.album.Clone();
                    case 7:
                    default:
                        return (System.Drawing.Image)CoverArtConfig.Resources.folder.Clone();
                }
            }
        }
        
        private void loadPreviews()
        {
            string imageSetName = currentImageSetName;
            string imageSetType = "std";
            List<int> defOnlyTabs = new List<int>() {6};
            //determine the type of previews we should create
            if (tabImageSet.SelectedIndex == 0 && currentProfileDef.CoverByDefinition) imageSetType = "definition";
            else if (tabImageSet.SelectedIndex == 7) imageSetType = "folder";
            else if (tabImageSet.SelectedIndex == 1) imageSetType = "series";
            else if (tabImageSet.SelectedIndex == 2) imageSetType = "season";
            else if (tabImageSet.SelectedIndex == 4) imageSetType = "remote";
            else if (tabImageSet.SelectedIndex == 3) imageSetType = "episode";
            else if (defOnlyTabs.Contains(tabImageSet.SelectedIndex)) imageSetType = "defaultonly";
            imageSetName = imageSetName + imageSetType;

            if (!PreviewItems.ContainsKey(imageSetName))
            {
                this.Cursor = Cursors.Wait;
                PreviewItems.Add(imageSetName, CreatePreviews(currentProfileDef, currentImageSet, imageSetName, imageSetType,exampleCover));
                this.Cursor = Cursors.Arrow;
            }
            lblHasTypeMap.Visibility = (currentProfileDef.TypeMap.Count > 0) ? Visibility.Visible : Visibility.Hidden;
            coversView.ItemsSource = PreviewItems[imageSetName];
        }

        private static string Translate(ProfileDefinition profile, string type)
        {
            if (profile.TypeMap.ContainsKey(type))
                if (profile.TypeMap[type] == "default" && profile.TypeMap.ContainsKey("default"))
                    return profile.TypeMap["default"]; //if items are translated to default AND default is itself translated, then return the translated one
                else
                    return profile.TypeMap[type];
            else return type;
        }

        //public List<PreviewItem> CreatePreviews(ImageSet imageSet, string imageSetName)
        //{
        //    return CreatePreviews(currentProfileDef, imageSet, imageSetName);
        //}

        public static List<PreviewItem> CreatePreviews(ProfileDefinition profile, ImageSet imageSet, string imageSetName)
        {
            return CreatePreviews(profile, imageSet, imageSetName, "all", (System.Drawing.Image)CoverArtConfig.Resources.cover1.Clone());
        }

        public static List<PreviewItem> CreatePreviews(ProfileDefinition profile, ImageSet imageSet, string imageSetName, string previewType, System.Drawing.Image art) {
            List<PreviewItem> previews = new List<PreviewItem>();
            string filename;
            List<string> specialKeys = new List<string>() {"Folder","Series","Season","Specials","Album","Person","Remote","Episode"};
            List<string> keys;
            Dictionary<string, System.Drawing.Image> frames = new Dictionary<string, System.Drawing.Image>(imageSet.Frames);

            switch (previewType)
            {
                case "definition":
                    keys = new List<string>() { "default", "HD", "SD" };
                    break;
                case "defaultonly":
                    keys = new List<string>() { "default" };
                    break;
                case "folder":
                    keys = new List<string>() { "Folder" };
                    break;
                case "series":
                    keys = new List<string>() { "Series" };
                    break;
                case "season":
                    keys = new List<string>() { "Season", "Specials" };
                    break;
                case "remote":
                    keys = new List<string>() { "Remote" };
                    break;
                case "episode":
                    keys = new List<string>() { "Episode" };
                    break;
                case "std":
                default:
                    keys = new List<string>(ImageSet.FrameTypes.Where(x => !ImageSet.NonMediaFrameTypes.Contains(x)));
                    break;
            }

            //even if this imageset doesn't contain a specific frame for a certain type, we may have it mapped to one it does
            // or we may have a type mapped to a frame that doesn't exist so we need to re-map that to default
            foreach (KeyValuePair<string, string> entry in profile.TypeMap)
            {
                if (imageSet.Frames.ContainsKey(entry.Value) && !frames.ContainsKey(entry.Key))
                {
                    frames.Add(entry.Key, imageSet.Frames[entry.Value]);
                } else
                    if (!imageSet.Frames.ContainsKey(entry.Value) && !frames.ContainsKey(entry.Key))
                    {
                        frames.Add(entry.Key, imageSet.Frames["default"]);
                    }
            }
            //also it is possible an imageset doesn't contain frames for some of our special keys
            // if not, map those to default
            foreach (string spKey in specialKeys)
            {
                if (keys.Contains(spKey) && !frames.ContainsKey(spKey))
                    frames.Add(spKey, imageSet.Frames["default"]);
            }

            foreach (KeyValuePair<string, System.Drawing.Image> entry in frames)
            {
                if (keys.Contains(entry.Key)) //just create the ones we want
                {
                    //translate if need be
                    string effType = Translate(profile, entry.Key);
                    if (!imageSet.Frames.ContainsKey(effType)) effType = "default"; //its possible we translate to a type that doesn't exist in this image set
                    System.Drawing.Image frame = (System.Drawing.Image)imageSet.Frames[effType].Clone();
                    System.Drawing.Image img = CoverArt.Plugin.CreateImage(frame, art, imageSet.Overlay, imageSet.RootPosition, imageSet.FrameOnTop, imageSet.RoundCorners, imageSet.JustRoundCorners, imageSet.Is3D, imageSet.Skew);
                    filename = System.IO.Path.Combine(TempLocation, imageSetName.GetMD5() + System.DateTime.Now.Millisecond.ToString() + entry.Key + ".png");
                    img.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                    previews.Add(new PreviewItem(filename, entry.Key));
                    img.Dispose();
                    frame.Dispose();
                    frame = null;
                    img = null;
                }
            }
            return previews;
        }

        private void tabImageSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabImageSet.SelectedItem != null && ddlFolderLocation.SelectedItem != null && processChanges)
            {
                setCurrentImageSet();
                loadPreviews();
            }
        }

        private void ddlImageSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabImageSet.SelectedItem != null && ddlFolderLocation.SelectedItem != null && processChanges)
            {
                setCurrentImageSet();
                loadPreviews();
            }
        }

        private void setCurrentImageSet()
        {
            switch (tabImageSet.SelectedIndex)
            {
                case 0:
                    currentImageSetName = ddlMovieLocation.SelectedItem.ToString();
                    currentProfileDef.MovieLocation = currentImageSetName;
                    config.Save();
                    break;
                case 1:
                    currentImageSetName = ddlSeriesLocation.SelectedItem.ToString();
                    currentProfileDef.SeriesLocation = currentImageSetName;
                    config.Save();
                    break;
                case 2:
                    currentImageSetName = ddlSeasonLocation.SelectedItem.ToString();
                    currentProfileDef.SeasonLocation = currentImageSetName;
                    config.Save();
                    break;
                case 3:
                    currentImageSetName = ddlEpisodeLocation.SelectedItem.ToString();
                    currentProfileDef.EpisodeLocation = currentImageSetName;
                    config.Save();
                    break;
                case 4:
                    currentImageSetName = ddlRemoteLocation.SelectedItem.ToString();
                    currentProfileDef.RemoteLocation = currentImageSetName;
                    config.Save();
                    break;
                case 5:
                    currentImageSetName = ddlThumbLocation.SelectedItem.ToString();
                    currentProfileDef.ThumbLocation = currentImageSetName;
                    config.Save();
                    break;
                case 6:
                    currentImageSetName = ddlAlbumLocation.SelectedItem.ToString();
                    currentProfileDef.AlbumLocation = currentImageSetName;
                    config.Save();
                    break;
                case 7:
                    currentImageSetName = ddlFolderLocation.SelectedItem.ToString();
                    currentProfileDef.FolderLocation = currentImageSetName;
                    config.Save();
                    break;
            }
            if (!imageSets.ContainsKey(currentImageSetName))
            {
                currentImageSet = new ImageSet(currentImageSetName);
            }
            else
            {
                currentImageSet = imageSets[currentImageSetName];
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //clean up our temp directory
            try
            {
                Directory.Delete(TempLocation, true);
            }
            catch { } //no biggie if we can't delete it
        }

        private void btnCustomImageSet_Click(object sender, RoutedEventArgs e)
        {
            ImageSetsWindow dlg = new ImageSetsWindow(config.CustomImageSets, imageSets);
            dlg.Left = this.Left+200;
            dlg.Top = this.Top;
            dlg.ShowDialog();
            config.Save();
            refreshImageSetOptions();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            var dlg1 = new FolderBrowserDialogEx
             {
                 Description = "Select a folder for the new Profile to apply to...",
                 ShowNewFolderButton = true,
                 ShowEditBox = true,
                 //NewStyle = false,
                 ShowFullPathInEditBox = true,
             };
            dlg1.RootFolder = System.Environment.SpecialFolder.Desktop;

            var result = dlg1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ProfileDefinition newDef = new ProfileDefinition();
                newDef.Directory = dlg1.SelectedPath;
                config.ProfileDefs.Add(newDef);
                config.Save();
                lbxProfiles.Items.Refresh();
                lbxProfiles.SelectedItem = newDef;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbxProfiles.SelectedItem != null)
            {
                if (MessageBox.Show("Remove Profile for " + currentProfileDef.Directory + "? \n\nAre you sure?", "Remove Profile", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int x = lbxProfiles.SelectedIndex;
                    config.ProfileDefs.Remove((ProfileDefinition)lbxProfiles.SelectedItem);
                    config.Save();
                    lbxProfiles.Items.Refresh();
                    if (x > lbxProfiles.Items.Count-1) x = lbxProfiles.Items.Count-1;
                    lbxProfiles.SelectedIndex = x;
                }
            }

        }

        private void btnIgnores_Click(object sender, RoutedEventArgs e)
        {
            IgnoresWindow dlg = new IgnoresWindow(config);
            dlg.Left = this.Left + 40;
            dlg.Top = this.Top + 40;
            dlg.ShowDialog();
            config.Save(); //in case we changed anything
        }

        private void btnClearCache_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear entire image cache? This will delete all downloaded images (backdrops, everything) and force them to re-load.\n\nAre you sure?", "Clear Image Cache", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Directory.Delete(MediaBrowser.Library.Configuration.ApplicationPaths.AppImagePath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to delete image cache. Full Error: " + ex.Message, "Error");
                }
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ebrsoft.com/coverart-setup");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (lbxProfiles.Items != null && lbxProfiles.Items.Count > 0)
            {
                lbxProfiles.SelectedIndex = 0;
            }

        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegWindow dlg = new RegWindow(config);
            if (dlg.ShowDialog().Value)
            {
                this.Cursor = Cursors.Wait;
                isReg = CoverArt.Plugin.Validate(config.RegKey);
                this.Cursor = Cursors.Arrow;
                if (isReg)
                {
                    MessageBox.Show("Thank you for your support.", "Registered");
                }
                else
                {
                    MessageBox.Show("Invalid registration key.  Please paste from the email you received.", "NOT Registered");
                }
                setVersion();
            }
        }

        private void cbxByDef_Checked(object sender, RoutedEventArgs e)
        {
            currentProfileDef.CoverByDefinition = cbxByDef.IsChecked.Value;
            //btnTypeMap.IsEnabled = !cbxByDef.IsChecked.Value;
            config.Save();
            setCurrentImageSet();
            loadPreviews();
        }

        private void btnTypeMap_Click(object sender, RoutedEventArgs e)
        {
            //save the current type map in case we cancel changes
            Dictionary<string, string> sav = new Dictionary<string, string>(currentProfileDef.TypeMap);
            TypeMapWindow dlg = new TypeMapWindow(currentProfileDef);
            dlg.Left = this.Left + 40;
            dlg.Top = this.Top + 40;
            if (dlg.ShowDialog() == true)
            {
                config.Save(); //save changes
                //and we need to re-create all our previews because the covers changed
                PreviewItems.Clear();
                loadPreviews();
            }
            else
            {
                //user canceled changes
                currentProfileDef.TypeMap = sav; //replace with the old one because we could have modified it
            }
        }

    }
}
