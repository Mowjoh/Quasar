using System;
using System.Net;
using System.Threading.Tasks;
using Quasar.Controls.Mod.ViewModels;
using Quasar.Helpers.FileOperations;
using Quasar.Controls.Mod.Models;

namespace Quasar.Helpers.Downloading
{
    class ModDownloader
    {
        //Parsing Default Directory Path
        readonly string DefaultDirectoryPath = Properties.Settings.Default["DefaultDir"].ToString();

        ModListItemViewModel ModListItemView;

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="_MIVM">Mod List Item View Model to update</param>
        public ModDownloader(ModListItemViewModel _MIVM)
        {
            ModListItemView = _MIVM;
        }

        /// <summary>
        /// The big boi, the download process
        /// </summary>
        /// <param name="_QuasarDownload">Quasar Download item to process</param>
        /// <returns></returns>
        public async Task<bool> DownloadArchiveAsync(QuasarDownload _QuasarDownload)
        {
            //Getting info from Quasar URL
            var DownloadURL = new Uri(_QuasarDownload.DownloadURL);
            var DestinationFolderPath = DefaultDirectoryPath + "\\Library\\Downloads\\";
            var DestinationFilePath = DestinationFolderPath + ModListItemView.LibraryItem.Guid + "." + _QuasarDownload.ModArchiveFormat;
            FileOperation.CheckCreate(DestinationFolderPath);

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

        /// <summary>
        /// Async Download Task
        /// </summary>
        /// <param name="_URL">File URL</param>
        /// <param name="_Destination">Destination file path</param>
        /// <returns></returns>
        public static async Task<bool> DownloadFile(string _URL, string _Destination)
        {
            //File Download
            using (WebClient webClient = new WebClient())
            {
                await webClient.DownloadFileTaskAsync(_URL, _Destination);
            }

            return true;
        }

    }
}
