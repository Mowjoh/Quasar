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

namespace Quasar
{
    class Downloader
    {
        ProgressBar progressBar;
        Label statusLabel;

        string downloadURL;
        public string contentType;
        public string contentID;
        string fileFormat;

        string dPath = Quasar.Properties.Settings.Default["DefaultDir"].ToString();



        public Downloader(ProgressBar _progressBar, Label _statusLabel)
        {
            progressBar = _progressBar;
            statusLabel = _statusLabel;
        }

        public async Task<bool> DownloadArchiveAsync(string _quasarURL)
        {
            await statusLabel.Dispatcher.BeginInvoke(new Action(() =>
            {
                progressBar.Value = 0;
                statusLabel.Visibility = Visibility.Hidden;
                progressBar.Visibility = Visibility.Visible;
            }), DispatcherPriority.Background);

            parseVariables(_quasarURL);
            var downloadLink = new Uri(downloadURL);
            var saveFilename = dPath+"\\Quasar\\Library\\Downloads\\"+contentID+"."+fileFormat;

            int i = 1;
            while (File.Exists(saveFilename))
            {
                saveFilename = dPath + "\\Quasar\\Library\\Downloads\\" + contentID + "_" + i + "." + fileFormat;
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

                statusLabel.Dispatcher.BeginInvoke((Action)(() =>
                {
                    statusLabel.Content = downloadProgress;
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
                statusLabel.Content = "Status : Up to date";
            }), DispatcherPriority.Background);

            return true;
        }

        public void parseVariables(string _quasarURL)
        {
            string parameters = _quasarURL.Substring(7);
            downloadURL = parameters.Split(',')[0];
            contentType = parameters.Split(',')[1];
            contentID   = parameters.Split(',')[2];
            fileFormat = parameters.Split(',')[3];
        }

    }
}
