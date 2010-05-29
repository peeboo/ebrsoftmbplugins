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
    /// Interaction logic for AddTypeMapWindow.xaml
    /// </summary>
    public partial class AddTypeMapWindow : Window
    {
        public AddTypeMapWindow(List<string> possibleFormats)
        {
            //this.typeMap = typeMap;
            InitializeComponent();
            //only want formats we don't already have
            ddlFormat.ItemsSource = possibleFormats;
            ddlCover.ItemsSource = ImageSet.FrameTypes;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
