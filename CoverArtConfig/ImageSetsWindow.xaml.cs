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
using System.Windows.Shapes;
using CoverArt;

namespace CoverArtConfig
{
    /// <summary>
    /// Interaction logic for ImageSetsWindow.xaml
    /// </summary>
    public partial class ImageSetsWindow : Window
    {
        List<string> ImageSetLocations;
        Dictionary<string, ImageSet> ImageSets;
        PreviewWindow PreviewWin;

        public ImageSetsWindow()
        {
            InitializeComponent();
        }

        public ImageSetsWindow(List<string> imageSetLocations, Dictionary<string,ImageSet> imageSets )
        {
            InitializeComponent();
            ImageSets = imageSets;
            ImageSetLocations = imageSetLocations;
            lbxImageSets.ItemsSource = ImageSetLocations;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            if (lbxImageSets.SelectedItem != null) updateImageSet(lbxImageSets.SelectedItem.ToString());
            this.Close();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbxImageSets.SelectedItem != null)
            {
                if (MessageBox.Show("Remove Image Set '" + lbxImageSets.SelectedItem + "'? \n\nAre you sure?", "Remove Ignore", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int x = lbxImageSets.SelectedIndex;
                    ImageSets.Remove((string)lbxImageSets.SelectedItem);
                    ImageSetLocations.Remove((string)lbxImageSets.SelectedItem);
                    lbxImageSets.Items.Refresh();
                    if (x > lbxImageSets.Items.Count - 1) x = lbxImageSets.Items.Count - 1;
                    lbxImageSets.SelectedIndex = x;
                    posHeight.Text = "";
                    posWidth.Text = "";
                    posX.Text = "";
                    posY.Text = "";
                    cbxFrameOnTop.IsChecked = false;
                    cbxRoundCorners.IsChecked = false;

                }
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg1 = new FolderBrowserDialogEx
            {
                Description = "Select the folder that contains your custom images",
                ShowNewFolderButton = false,
                ShowEditBox = true,
                NewStyle = true,
                ShowFullPathInEditBox = true,
            };
            dlg1.RootFolder = System.Environment.SpecialFolder.Desktop;

            var result = dlg1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (isValidImageSetLocation(dlg1.SelectedPath))
                    {
                        if (ImageSetLocations.Contains(dlg1.SelectedPath))
                        {
                            MessageBox.Show(dlg1.SelectedPath + " already exists.", "Error");
                        }
                        else
                        {
                            ImageSetLocations.Add(dlg1.SelectedPath);
                            ImageSets.Add(dlg1.SelectedPath, new ImageSet(dlg1.SelectedPath));
                            lbxImageSets.Items.Refresh();
                            lbxImageSets.SelectedItem = dlg1.SelectedPath;
                        }
                    }
                    else
                    {
                        MessageBox.Show(dlg1.SelectedPath + " doesn't appear to be a valid Image Set location.  At a minimum, it must contain a 'default.png' file.", "Invalid Location");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error attempting to add Image Set. " + ex.Message, "Error");
                }

            }

        }

        private bool isValidImageSetLocation(string path) {
            return System.IO.File.Exists(System.IO.Path.Combine(path,"default.png"));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (lbxImageSets.Items != null && lbxImageSets.Items.Count > 0)
            {
                lbxImageSets.SelectedIndex = 0;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PreviewWin != null) PreviewWin.Close();
        }

        private void validateNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.Text[0]);
            base.OnPreviewTextInput(e); 

        }

        private void lbxImageSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxImageSets != null && lbxImageSets.SelectedItem != null)
            {
                string imageSetName = lbxImageSets.SelectedItem.ToString();
                if (!ImageSets.ContainsKey(imageSetName))
                {
                    ImageSets.Add(imageSetName, new ImageSet(imageSetName));
                }
                //first update last values
                if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                {
                    updateImageSet(e.RemovedItems[0].ToString());
                }
                //and now fill in current ones
                posX.Text = ImageSets[imageSetName].RootPosition.X.ToString();
                posY.Text = ImageSets[imageSetName].RootPosition.Y.ToString();
                posWidth.Text = ImageSets[imageSetName].RootPosition.Width.ToString();
                posHeight.Text = ImageSets[imageSetName].RootPosition.Height.ToString();
                cbxRoundCorners.IsChecked = ImageSets[imageSetName].RoundCorners;
                cbxFrameOnTop.IsChecked = ImageSets[imageSetName].FrameOnTop;
            }
        }

        private void updateImageSet(string location)
        {
            int x, y, width, height;
            Int32.TryParse(posX.Text, out x);
            ImageSets[location].RootPosition.X = x;
            Int32.TryParse(posY.Text, out y);
            ImageSets[location].RootPosition.Y = y;
            Int32.TryParse(posWidth.Text, out width);
            ImageSets[location].RootPosition.Width = width;
            Int32.TryParse(posHeight.Text, out height);
            ImageSets[location].RootPosition.Height = height;
            ImageSets[location].RoundCorners = cbxRoundCorners.IsChecked.Value;
            ImageSets[location].FrameOnTop = cbxFrameOnTop.IsChecked.Value;
            ImageSets[location].Save();
        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (lbxImageSets.SelectedItem != null)
            {
                string imageSetName = lbxImageSets.SelectedItem.ToString();
                updateImageSet(imageSetName);
                ImageSet imageSet = ImageSets[imageSetName];
                if (PreviewWin == null)
                {
                    PreviewWin = new PreviewWindow();
                    PreviewWin.Closed += previewWindowClosed;
                }
                this.Cursor = Cursors.Wait;
                PreviewWin.Previews = MainWindow.CreatePreviews(imageSet, imageSetName);
                this.Cursor = Cursors.Arrow;
                PreviewWin.UpdatePreviews();
                PreviewWin.Show();
            }
        }

        private void previewWindowClosed(object sender, EventArgs e)
        {
            PreviewWin = null;
        }

        private void txtBoxAutoSelect(object sender, RoutedEventArgs e)
        {
            TextBox us = sender as TextBox;
            if (us != null) us.SelectAll();
        }

    }
}
