using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quasarite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String PostUrl = UrlInput.Text;
            String DownloadUrl = DownloadInput.Text;
            String Extension = TypeInput.Text;
            String Type = APIInput.Text;

            String PostID = PostUrl.Split('/')[PostUrl.Split('/').Length - 1];
            String DownloadID = DownloadUrl.Split('/')[DownloadUrl.Split('/').Length - 1];

            String QuasarURL = @"quasar:https://gamebanana.com/dl/" + DownloadID + "," + Type + "," + PostID + "," + Extension;

            QuasarUrlLabel.Content = QuasarURL;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(QuasarUrlLabel.Content.ToString());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Process.Start(QuasarUrlLabel.Content.ToString());
        }
    }
}
