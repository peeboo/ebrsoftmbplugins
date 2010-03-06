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

namespace CoverArtConfig
{
    /// <summary>
    /// Interaction logic for IgnoresWindow.xaml
    /// </summary>
    public partial class IgnoresWindow : Window
    {
        List<string> ignores;

        public IgnoresWindow()
        {
            InitializeComponent();
        }

        public IgnoresWindow(List<string> ignoreList)
        {
            InitializeComponent();
            ignores = ignoreList;
            lbxIgnores.ItemsSource = ignores;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lbxIgnores.SelectedItem != null)
            {
                if (MessageBox.Show("Remove Ignore Location '" + lbxIgnores.SelectedItem + "'? \n\nAre you sure?", "Remove Ignore", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int x = lbxIgnores.SelectedIndex;
                    ignores.Remove((string)lbxIgnores.SelectedItem);
                    lbxIgnores.Items.Refresh();
                    if (x > lbxIgnores.Items.Count - 1) x = lbxIgnores.Items.Count - 1;
                    lbxIgnores.SelectedIndex = x;
                }
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dlg1 = new FolderBrowserDialogEx
            {
                Description = "Select a folder to ignore (will include all sub-folders)...",
                ShowNewFolderButton = false,
                ShowEditBox = true,
                NewStyle = true,
                ShowFullPathInEditBox = true,
            };
            dlg1.RootFolder = System.Environment.SpecialFolder.Desktop;

            var result = dlg1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                ignores.Add(dlg1.SelectedPath);
                lbxIgnores.Items.Refresh();
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
