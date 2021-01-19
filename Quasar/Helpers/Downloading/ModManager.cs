using Quasar.Compression;
using Quasar.Controls;
using Quasar.Controls.Common.Models;
using Quasar.Controls.Mod.Models;
using Quasar.Data.V2;
using Quasar.FileSystem;
using Quasar.Helpers.ModScanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Helpers.Downloading
{
    public class ModManager : ObservableObject
    {
        #region Private properties
        private bool _DownloadNeeded { get; set; }
        private bool _UpdateNeeded { get; set; }
        private bool _ScanNeeded { get; set; }
        #endregion

        #region Working references
        public bool DownloadNeeded
        {
            get => _DownloadNeeded;
            set
            {
                _DownloadNeeded = value;
                OnPropertyChanged("DownloadNeeded");
            }
        }
        public bool UpdateNeeded
        {
            get => _UpdateNeeded;
            set
            {
                _UpdateNeeded = value;
                OnPropertyChanged("UpdateNeeded");
            }
        }
        public bool ScanNeeded
        {
            get => _ScanNeeded;
            set
            {
                _ScanNeeded = value;
                OnPropertyChanged("ScanNeeded");
            }
        }

        public bool ActionNeeded
        {
            get
            {
                return (DownloadNeeded || UpdateNeeded || ScanNeeded);
            }
        }

        public string DownloadDestinationFilePath { get; set; }
        public string ArchiveContentFolderPath { get; set; }
        public string LibraryContentFolderPath { get; set; }

        public QuasarDownload QuasarURL { get; set; }
        public LibraryItem LibraryItem { get; set; }
        public APIMod APIMod { get; set; }
        public ModListItem ModListItem {get; set;}
        public ModFileManager ModFileManager { get; set; }
        public ObservableCollection<ContentItem> ScannedContents { get; set; }
        #endregion

        public ModManager(string _QuasarURL)
        {
            QuasarURL = new QuasarDownload() { QuasarURL = _QuasarURL };
        }

        #region Evaluation
        public async Task<bool> EvaluateActionNeeded(MainUIViewModel MUVM)
        {
            //If there already exists a mod with this ID
            if (MUVM.Library.Any(li => li.ID == int.Parse(QuasarURL.LibraryItemID)))
            {
                //Comparing Update Count with the API
                LibraryItem = MUVM.Library.Single(li => li.ID == int.Parse(QuasarURL.LibraryItemID));
                await GetAPIModInformation();
                UpdateNeeded = APIMod.UpdateCount > LibraryItem.UpdateCount;
            }

            //No mod with this ID is present in the Library
            else
            {
                await GetAPIModInformation();
                Game RelatedGame = MUVM.Games.Single(g => g.APIGameName == APIMod.GameName);
                GameAPISubCategory RelatedSubCategory = RelatedGame.GameAPICategories.Single(c => c.APICategoryName == APIMod.ModType).GameAPISubCategories.Single(scat => scat.APISubCategoryID == APIMod.CategoryID);
                //Generating new LibraryItem
                LibraryItem = new LibraryItem()
                {
                    ID = APIMod.ID,
                    APICategoryName = APIMod.ModType,
                    Name = APIMod.Name,
                    Description = APIMod.Description,
                    UpdateCount = APIMod.UpdateCount,
                    GameAPISubCategoryID = RelatedSubCategory.ID,
                    GameID = RelatedGame.ID,
                    Authors = new ObservableCollection<Author>()
                };

                foreach(string[] val in APIMod.Authors)
                {
                    Author au = new Author()
                    {
                        Name = val[0],
                        Role = val[1],
                        GamebananaAuthorID = int.Parse(val[2])
                    };
                    LibraryItem.Authors.Add(au);
                }
                DownloadNeeded = true;
            }

            //Checking if there is a need for a rescan
            if (DownloadNeeded || UpdateNeeded)
            {
                ScanNeeded = true;
            }

            return true;
        }
        public async Task<bool> GetAPIModInformation()
        {
            APIMod = await APIRequest.GetAPIMod(QuasarURL.APICategoryName, QuasarURL.LibraryItemID);
            return true;
        }
        #endregion

        #region Actions
        public async Task<bool> TakeAction()
        {
            bool ProcessAborted = true;

            if (DownloadNeeded || UpdateNeeded)
            {
                ModListItem.ModListItemViewModel.ModStatusValue = "Downloading";
                bool Downloaded = await Download();
                
                //Stopping the process if necessary
                if (Downloaded)
                {
                    ModListItem.ModListItemViewModel.ModStatusValue = "Extracting";
                    ModListItem.ModListItemViewModel.ModStatusTextValue = "Please wait";
                    
                    bool Extracted = await Extract();

                    if (Extracted)
                    {
                        ModListItem.ModListItemViewModel.ModStatusValue = "Processing Files";
                        
                        bool Processed = await ProcessExtractedFiles();

                        if (Processed)
                        {
                            GameAPICategory cat = ModListItem.ModListItemViewModel.Game.GameAPICategories.Single(c => c.APICategoryName == QuasarURL.APICategoryName);
                            await APIRequest.GetScreenshot(QuasarURL.APICategoryName, QuasarURL.LibraryItemID, LibraryItem.GameID.ToString(), cat.ID.ToString());
                            ProcessAborted = false;
                        }
                    }
                }
            }
            if (ProcessAborted)
            {
                ModListItem.ModListItemViewModel.DownloadFailed = true;
            }

            return false;
        }
        public async Task<bool> Download()
        {
            ModDownloader modDownloader = new ModDownloader(ModListItem.ModListItemViewModel);
            bool success = await modDownloader.DownloadArchiveAsync(QuasarURL);
            return success;
        }
        public async Task<bool> Update()
        {
            ModDownloader modDownloader = new ModDownloader(ModListItem.ModListItemViewModel);
            bool success = await modDownloader.DownloadArchiveAsync(QuasarURL);
            return success;
        }
        public async Task<bool> Extract()
        {
            ModFileManager = new ModFileManager(LibraryItem, ModListItem.ModListItemViewModel.Game, QuasarURL.ModArchiveFormat);
            Unarchiver un = new Unarchiver(ModListItem.ModListItemViewModel);
            bool success = await un.ExtractArchiveAsync(ModFileManager.DownloadDestinationFilePath, ModFileManager.ArchiveContentFolderPath, QuasarURL.ModArchiveFormat) == 0;
            return success;
        }
        public async Task<bool> ProcessExtractedFiles()
        {
            bool success = await ModFileManager.MoveDownload() == 0;
            ModFileManager.ClearDownloadContents();
            return success;
        }

        public async Task<bool> Scan(ObservableCollection<QuasarModType> QuasarModTypes, Game Game)
        {
            ModListItem.ModListItemViewModel.ModStatusValue = "Scanning mod files";
            ModListItem.ModListItemViewModel.ModStatusTextValue = "Please wait";

            await Task.Run(() =>
            {
                ScannedContents = Scannerino.ScanMod(ModFileManager.LibraryContentFolderPath, QuasarModTypes, Game, LibraryItem);
            });
            return true;
        }
        #endregion
    }
}
