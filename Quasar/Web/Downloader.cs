using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Quasar.Controls;
using Quasar.XMLResources;
using Quasar.FileSystem;

namespace Quasar
{
     class Downloader
    {
        //Setting Default Directory Path
        readonly string DefaultDirectoryPath = Properties.Settings.Default["DefaultDir"].ToString();
        readonly ProgressBar DownloadProgressBar;
        readonly Label StatusLabel;
        readonly Label TypeLabel;

        string DownloadURL;
        public string ModTypeID;
        public string ModID;
        public string ArchiveExtension;

        public Downloader(ProgressBar _ProgressBar, Label _StatusLabel, Label _TypeLabel)
        {
            DownloadProgressBar = _ProgressBar;
            StatusLabel = _StatusLabel;
            TypeLabel = _TypeLabel;
        }

        //The big boi, the download Task
        public async Task<bool> DownloadArchiveAsync(ModFileManager _ModFileManager)
        {
            //Setting Status and ProgressBar
            await StatusLabel.Dispatcher.BeginInvoke(new Action(() =>
            {
                DownloadProgressBar.Value = 0;
                StatusLabel.Visibility = Visibility.Hidden;
                DownloadProgressBar.Visibility = Visibility.Visible;
            }), DispatcherPriority.Background);

            //Getting info from Quasar URL
            ParseQueryStringParameters(_ModFileManager);
            var DownloadURL = new Uri(this.DownloadURL);
            var DestinationFolderPath = DefaultDirectoryPath + "\\Library\\Downloads\\";
            var DestinationFilePath = DestinationFolderPath + ModID + "." + ArchiveExtension;
            Folderino.CheckCreate(DestinationFolderPath);

            //Setting up Progress actions
            void DownloadProgressChangedEvent(object s, DownloadProgressChangedEventArgs e)
            {
                //Changing ProgressBar value
                DownloadProgressBar.Dispatcher.BeginInvoke((Action)(() =>
                {
                    DownloadProgressBar.Value = e.ProgressPercentage;
                }));

                //Making a proper string to display
                var downloadProgress = string.Format("{0} MB / {1} MB",
                        (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                        (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));

                //Displaying value
                TypeLabel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    TypeLabel.Content = downloadProgress;
                }));
            }

            //File Download
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += DownloadProgressChangedEvent;
                await webClient.DownloadFileTaskAsync(DownloadURL, DestinationFilePath);
            }
            
            //Setting UI for finished state
            await StatusLabel.Dispatcher.BeginInvoke(new Action(() =>
            {
                StatusLabel.Visibility = Visibility.Visible;
                DownloadProgressBar.Visibility = Visibility.Hidden;
            }), DispatcherPriority.Background);

            return true;
        }


        public static async Task<bool> DownloadFile(string _URL, string _Destination)
        {
            //File Download
            using (WebClient webClient = new WebClient())
            {
                await webClient.DownloadFileTaskAsync(_URL, _Destination);
            }

            return true;
        }
        //Parsing information from a ModFileManager instance
        public void ParseQueryStringParameters(ModFileManager _ModFileManager)
        {
            DownloadURL = _ModFileManager.DownloadURL;
            ModTypeID = _ModFileManager.ModTypeID;
            ModID   = _ModFileManager.ModID;
            ArchiveExtension = _ModFileManager.ModArchiveFormat;
        }

    }
}
