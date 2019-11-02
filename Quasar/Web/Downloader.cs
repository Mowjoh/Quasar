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
using static Quasar.Library;
using Quasar.Resources;
using Quasar.File;

namespace Quasar
{
     class Downloader
    {
        //Setting Default Directory Path
        string DefaultDirectoryPath = Properties.Settings.Default["DefaultDir"].ToString();

        ProgressBar progressBar;
        Label statusLabel;
        Label typeLabel;
        Label categoryLabel;

        string downloadURL;
        public string contentType;
        public string contentID;
        public string fileFormat;

        public string saveFileName;

        public Downloader(ProgressBar _progressBar, Label _statusLabel, Label TypeLabel, Label _CategoryLabel)
        {
            progressBar = _progressBar;
            statusLabel = _statusLabel;
            categoryLabel = _CategoryLabel;
            typeLabel = TypeLabel;
        }

        public async Task<bool> DownloadArchiveAsync(ModFileManager _FMan)
        {
            await statusLabel.Dispatcher.BeginInvoke(new Action(() =>
            {
                progressBar.Value = 0;
                statusLabel.Visibility = Visibility.Hidden;
                progressBar.Visibility = Visibility.Visible;
            }), DispatcherPriority.Background);

            ParseQueryStringParameters(_FMan);
            var downloadLink = new Uri(downloadURL);
            var saveFolder = DefaultDirectoryPath + "\\Library\\Downloads\\";
            var saveFilename = DefaultDirectoryPath+"\\Library\\Downloads\\"+contentID+"."+fileFormat;

            if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

            int i = 1;
            while (System.IO.File.Exists(saveFilename))
            {
                saveFilename = DefaultDirectoryPath + "\\Library\\Downloads\\" + contentID + "_" + i + "." + fileFormat;
                i++;
            }

            DownloadProgressChangedEventHandler DownloadProgressChangedEvent = (s, e) =>
            {
                progressBar.Dispatcher.BeginInvoke((Action)(() =>
                {
                    progressBar.Value = e.ProgressPercentage;
                }));

                var downloadProgress = string.Format("{0} MB / {1} MB",
                        (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                        (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));

                typeLabel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    typeLabel.Content = downloadProgress;
                }));
            };

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += DownloadProgressChangedEvent;
                await webClient.DownloadFileTaskAsync(downloadLink, saveFilename);
            }

            await statusLabel.Dispatcher.BeginInvoke(new Action(() =>
            {
                statusLabel.Visibility = Visibility.Visible;
                progressBar.Visibility = Visibility.Hidden;
            }), DispatcherPriority.Background);

            return true;
        }

        public void ParseQueryStringParameters(ModFileManager _FMan)
        {
            downloadURL = _FMan.downloadURL;
            contentType = _FMan.modType;
            contentID   = _FMan.modID;
            fileFormat = _FMan.modArchiveFormat;
        }

    }
}
