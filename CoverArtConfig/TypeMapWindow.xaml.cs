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
    /// Interaction logic for TypeMapWindow.xaml
    /// </summary>
    public partial class TypeMapWindow : Window
    {
        ProfileDefinition profile;

        public TypeMapWindow(ProfileDefinition profile)
        {
            this.profile = profile;
            InitializeComponent();
            lbxTypeMap.ItemsSource = profile.TypeMap;
            lblProfile.Content = profile.Directory;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            List<string> possibleFormats = new List<string>(ImageSet.FrameTypes);
            foreach (KeyValuePair<string, string> item in lbxTypeMap.Items)
            {
                //remove the ones we alredy have in there
                possibleFormats.Remove(item.Key);
            }
            if (possibleFormats.Count > 0)
            {
                AddTypeMapWindow dlg = new AddTypeMapWindow(possibleFormats);
                dlg.Left = this.Left + 40;
                dlg.Top = this.Top + 40;
                dlg.ShowDialog();
                if (dlg.DialogResult == true)
                {
                    //add the new type mapping
                    if (dlg.ddlCover.SelectedItem != null && dlg.ddlFormat.SelectedItem != null)
                    {
                        profile.TypeMap.Add(dlg.ddlFormat.SelectedItem.ToString(), dlg.ddlCover.SelectedItem.ToString());
                        lbxTypeMap.Items.Refresh();
                    }
                }
            }
            else
            {
                MessageBox.Show("All Media Formats have type mappings", "Cannot Add");
            }
        }

        private void lbxTypeMap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxTypeMap.SelectedIndex >= 0)
            {
                btnRemove.IsEnabled = true;
            }
            else
            {
                btnRemove.IsEnabled = false;
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbxTypeMap.SelectedItem != null)
            {
                int ndx = lbxTypeMap.SelectedIndex;
                KeyValuePair<string, string> entry = (KeyValuePair<string, string>)lbxTypeMap.SelectedItem;
                profile.TypeMap.Remove(entry.Key);
                lbxTypeMap.Items.Refresh();
                ndx = (ndx < lbxTypeMap.Items.Count) ? ndx : lbxTypeMap.Items.Count - 1;
                lbxTypeMap.SelectedIndex = ndx;
                
            }
        }

        private void pstDefaultOnly_Click(object sender, RoutedEventArgs e)
        {
            profile.TypeMap.Clear();
            foreach (string format in CoverArt.ImageSet.FrameTypes)
            {
                if (format != "default")
                    profile.TypeMap.Add(format, "default");
            }
            lbxTypeMap.Items.Refresh();
                
        }

        private void btnClearMap_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear all type mappings.  Are you sure?", "Clear Type Mappings", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                profile.TypeMap.Clear();
                lbxTypeMap.Items.Refresh();
            }
        }

        private void pstBDOnly_Click(object sender, RoutedEventArgs e)
        {
            profile.TypeMap.Clear();
            foreach (string format in CoverArt.ImageSet.FrameTypes)
            {
                profile.TypeMap.Add(format, "BD");
            }
            lbxTypeMap.Items.Refresh();

        }

        private void pstDVDOnly_Click(object sender, RoutedEventArgs e)
        {
            profile.TypeMap.Clear();
            foreach (string format in CoverArt.ImageSet.FrameTypes)
            {
                profile.TypeMap.Add(format, "DVD");
            }
            lbxTypeMap.Items.Refresh();
        }

        private void pstHDOnly_Click(object sender, RoutedEventArgs e)
        {
            profile.TypeMap.Clear();
            foreach (string format in CoverArt.ImageSet.FrameTypes)
            {
                profile.TypeMap.Add(format, "HD");
            }
            lbxTypeMap.Items.Refresh();
        }

        private void pstMinimal_Click(object sender, RoutedEventArgs e)
        {
            profile.TypeMap.Clear();
            foreach (string format in CoverArt.ImageSet.FrameTypes)
            {
                List<string> big3 = new List<string>() {"default","DVD","BD","HDDVD","HD"};
                if (!big3.Contains(format))
                    profile.TypeMap.Add(format, "default");
            }
            lbxTypeMap.Items.Refresh();

        }
    }
}
