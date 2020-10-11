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
using Quasar.Controls.Mod.ViewModels;

namespace Quasar
{
     class Downloader
    {
        //Setting Default Directory Path
        readonly string DefaultDirectoryPath = Properties.Settings.Default["DefaultDir"].ToString();

        ModListItemViewModel ModListItemView;

        string DownloadURL;
        public string ModTypeID;
        public string ModID;
        public string ArchiveExtension;

        public Downloader(ModListItemViewModel _MIVM)
        {
            ModListItemView = _MIVM;
        }

        //The big boi, the download Task
        public async Task<bool> DownloadArchiveAsync(ModFileManager _ModFileManager)
        {
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
                ModListItemView.ProgressBarValue = e.ProgressPercentage;
                //ModListItem.Progress.Dispatcher.BeginInvoke((Action)(() => { ModListItem.ProgressBarValue = e.ProgressPercentage; }));

                //Making a proper string to display
                var downloadProgress = string.Format("{0} MB / {1} MB", (e.BytesReceived / 1024d / 1024d).ToString("0.00"), (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));

                //Displaying value
                ModListItemView.ModStatusTextValue = downloadProgress;
                //ModListItem.ModStatusTextLabel.Dispatcher.BeginInvoke((Action)(() => { ModListItem.ModStatusTextValue = downloadProgress; }));
            }

            //File Download
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += DownloadProgressChangedEvent;
                await webClient.DownloadFileTaskAsync(DownloadURL, DestinationFilePath);
            }
            
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
