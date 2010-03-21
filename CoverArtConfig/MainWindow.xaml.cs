using System;
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
            "CoverArtCaseBD",
            "CoverArtCaseDVD",
            "CoverArtClearCase",
            "CoverArtCD",
            "CoverArtDiamond",
            "CoverArtRounded",
            "CoverArtTV",
            "CoverArtTVMB",
            "CoverArtFilm",
            "Ignore"
            
        };
        private MyConfigData config;

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
            InitializeComponent();
            refreshImageSetOptions();
            ProfileDefinition def = config.ProfileDefs.Find(p => p.Directory == "default");
            if (def == null)
            {
                config.ProfileDefs.Insert(0,new ProfileDefinition());
            }
            lbxProfiles.ItemsSource = config.ProfileDefs;
            version.Content = "Version " + CoverArt.Plugin.CurrentVersion;
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
                foreach (ImageSet imageSet in profile.ImageSets)
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
            processChanges = true;
            setCurrentImageSet();

            //create previews
            loadPreviews();
            return true;
        }

        private void loadPreviews()
        {
            if (!PreviewItems.ContainsKey(currentImageSetName))
            {
                this.Cursor = Cursors.Wait;
                PreviewItems.Add(currentImageSetName, CreatePreviews(currentImageSet, currentImageSetName));
                this.Cursor = Cursors.Arrow;
            }
            coversView.ItemsSource = PreviewItems[currentImageSetName];
        }

        public static List<PreviewItem> CreatePreviews(ImageSet imageSet, string imageSetName) {
            List<PreviewItem> previews = new List<PreviewItem>();
            string filename;

            foreach (KeyValuePair<string, System.Drawing.Image> entry in imageSet.Frames)
            {
                System.Drawing.Image img = CoverArt.Plugin.CreateImage((System.Drawing.Image)entry.Value.Clone(), (System.Drawing.Image)CoverArtConfig.Resources.folder.Clone(), imageSet.Overlay, imageSet.RootPosition, imageSet.FrameOnTop, imageSet.RoundCorners, imageSet.JustRoundCorners);
                filename = System.IO.Path.Combine(TempLocation, imageSetName.GetMD5() + System.DateTime.Now.Millisecond.ToString() + entry.Key+".png");
                img.Save(filename,System.Drawing.Imaging.ImageFormat.Png);
                previews.Add(new PreviewItem(filename, entry.Key));
                img.Dispose();
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
            IgnoresWindow dlg = new IgnoresWindow(config.IgnoreFolders);
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

    }
}
