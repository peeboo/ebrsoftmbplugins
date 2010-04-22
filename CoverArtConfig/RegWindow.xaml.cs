using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using CoverArt;

namespace CoverArtConfig
{
    /// <summary>
    /// Interaction logic for RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        private MyConfigData configData;

        public RegWindow(MyConfigData config)
        {
            InitializeComponent();
            configData = config;
            regKey.Text = config.RegKey;
            regKey.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            configData.RegKey = regKey.Text;
            configData.Save();
            this.DialogResult = true;
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {

            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;

        }

    }
}
