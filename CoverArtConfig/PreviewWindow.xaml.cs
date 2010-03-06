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
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        public List<PreviewItem> Previews;

        public PreviewWindow()
        {
            InitializeComponent();
        }

        public PreviewWindow(List<PreviewItem> previews)
        {
            Previews = previews;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void UpdatePreviews()
        {
            coversView.ItemsSource = Previews;
        }

    }
}
