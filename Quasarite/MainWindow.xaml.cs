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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string value = await GetDownloadURL();

            QuasarUrlLabel.Content = value;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(QuasarUrlLabel.Content.ToString());
        }

        private async void LaunchitClicked(object sender, RoutedEventArgs e)
        {
            string value = await GetDownloadURL();
            Process.Start(value);
            
        }

        private async Task<string> GetDownloadURL()
        {
            List<QueryStringItem> queryParameters;
            String PostUrl = UrlInput.Text;
            string quasarURL = "";
            if (PostUrl != "")
            {
                String PostID = PostUrl.Split('/')[PostUrl.Split('/').Length - 1];
                String PostType = PostUrl.Split('/')[PostUrl.Split('/').Length - 2];


                switch (PostType)
                {
                    case "skins":
                        PostType = "Skin";
                        break;
                    case "sounds":
                        PostType = "Sound";
                        break;
                    case "gamefiles":
                        PostType = "Gamefile";
                        break;
                    case "stages":
                        PostType = "Map";
                        break;
                    case "guis":
                        PostType = "Gui";
                        break;
                }

                string[] request = await APIRequest.GetDownloadFileName(PostType, PostID);

                string dlID = request[1].Split('/')[request[1].Split('/').Length - 1];
                string extension = request[0].Split('.')[request[0].Split('.').Length - 1];
                quasarURL = @"quasar:https://gamebanana.com/dl/" + dlID + "," + PostType + "," + PostID + "," + extension;
            }
            

            return quasarURL;
        }
    }
}
