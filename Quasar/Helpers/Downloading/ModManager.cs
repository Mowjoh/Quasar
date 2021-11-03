using Quasar.Helpers.Compression;
using Quasar.Controls;
using Quasar.Controls.Mod.Models;
using DataModels.User;
using DataModels.Resource;
using DataModels.Common;
using Quasar.FileSystem;
using Quasar.Helpers.ModScanning;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Quasar.MainUI.ViewModels;
using log4net;
using Workshop.FileManagement;
using Workshop.Web;

namespace Quasar.Helpers.Downloading
{
    public class ModManager : ObservableObject
    {
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        #region Data
        public QuasarDownload QuasarURL { get; set; }
        public LibraryItem LibraryItem { get; set; }
        public APIMod APIMod { get; set; }
        public GamebananaItem RequestItem { get; set; }
        public ModListItem ModListItem { get; set; }
        public ModFileManager ModFileManager { get; set; }
        public ObservableCollection<ContentItem> ScannedContents { get; set; }
        #endregion

        #region Paths

        public string DownloadDestinationFilePath { get; set; }
        public string ArchiveContentFolderPath { get; set; }
        public string LibraryContentFolderPath { get; set; }

        #endregion

        #region Work Status

        #region Private
        private bool _DownloadNeeded { get; set; }
        private bool _UpdateNeeded { get; set; }
        private bool _ScanNeeded { get; set; }
        #endregion

        #region Public
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
        #endregion

        #endregion

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="_QuasarURL"></param>
        public ModManager(string _QuasarURL)
        {
            QuasarURL = new QuasarDownload() { QuasarURL = _QuasarURL };
        }

        #region Evaluation
        /// <summary>
        /// Evaluates if the mod needs to be downloaded or updated
        /// </summary>
        /// <param name="MUVM"></param>
        /// <returns>Success State</returns>
        public async Task<bool> EvaluateActionNeeded(MainUIViewModel MUVM)
        {
            try
            {
                //If there already exists a mod with this ID
                if (MUVM.Library.Any(li => li.GBItem?.GamebananaItemID == int.Parse(QuasarURL.GamebananaItemID)))
                {
                    //Comparing Update Count with the API
                    LibraryItem = MUVM.Library.Single(li => li.GBItem.GamebananaItemID == int.Parse(QuasarURL.GamebananaItemID));
                    await GetAPIModInformation();
                    UpdateNeeded = APIMod.UpdateCount > LibraryItem.GBItem.UpdateCount;
                }

                //No mod with this ID is present in the Library
                else
                {
                    await GetAPIModInformation();
                    GamebananaItem GBItem = GetGamebananaItem(APIMod, MUVM);

                    Game RelatedGame = MUVM.Games.Single(g => g.APIGameName == APIMod.Game.Name);
                    GamebananaGame APIGame = MUVM.API.Games.SingleOrDefault(g => g.Name == APIMod.Game.Name);


                    //Generating new LibraryItem
                    LibraryItem = new LibraryItem()
                    {
                        Guid = Guid.NewGuid(),
                        Name = APIMod.Name,
                        GameID = RelatedGame.ID,
                        GBItem = GBItem,
                        Time = DateTime.Now
                    };

                    DownloadNeeded = true;
                }

                //Checking if there is a need for a rescan
                if (DownloadNeeded || UpdateNeeded)
                {
                    ScanNeeded = true;
                }

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Parses information from Gamebanana's API about this mod
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetAPIModInformation()
        {
            APIMod = await APIRequest.GetModInformation(QuasarURL.GamebananaItemID, QuasarURL.APICategoryName);
            return true;
        }

        public GamebananaItem GetGamebananaItem(APIMod Request, MainUIViewModel MUVM)
        {
            //Processing base info
            GamebananaItem Item = new GamebananaItem()
            {
                Name = Request.Name,
                GamebananaItemID = Request.ID,
                Authors = new ObservableCollection<Author>(),
                GameName = Request.Game.Name,
                UpdateCount = Request.UpdateCount
            };

            //Processing Authors
            if (Request.Authors.KeyAuthors != null)
            {
                foreach (string[] val in Request.Authors.KeyAuthors)
                {
                    Author au = new Author()
                    {
                        Name = val[0],
                        Role = val[1],
                        GamebananaAuthorID = int.Parse(val[2])
                    };
                    Item.Authors.Add(au);
                }
            }

            if (Request.Authors.Authors != null)
            {
                foreach (string[] val in Request.Authors.Authors)
                {
                    Author au = new Author()
                    {
                        Name = val[0],
                        Role = val[1],
                        GamebananaAuthorID = int.Parse(val[2])
                    };
                    Item.Authors.Add(au);
                }
            }

            GamebananaRootCategory RCat = null;


            //If Mod is unmoved
            if (Request.GamebananaRootCategoryName != "Mod")
            {
                RCat = MUVM.API.Games[0].RootCategories.SingleOrDefault(cat => cat.Name == Request.GamebananaRootCategoryName);
            }
            else
            {
                //If there is a Root Category
                if(Request.SuperCategory == null)
                {
                    Request.SuperCategory = new APISuperCategory()
                    {
                        ID = Request.SubCategory.ID,
                        Name = Request.SubCategory.Name
                    };
                }

                RCat = MUVM.API.Games[0].RootCategories.SingleOrDefault(cat => cat.Name == Request.SuperCategory.Name);
            }


            //If no Root Category is found in the database
            if(RCat == null)
            {
                string catName = "";
                if(Request.SuperCategory == null)
                {
                    catName = Request.GamebananaRootCategoryName;
                }
                else
                {
                    catName = Request.SuperCategory.Name;
                }

                RCat = new GamebananaRootCategory()
                {
                    Guid = Guid.NewGuid(),
                    Name = catName,
                    SubCategories = new ObservableCollection<GamebananaSubCategory>()
                };

                GamebananaSubCategory SC = new GamebananaSubCategory()
                {
                    Guid = Guid.NewGuid(),
                    Name = Request.SubCategory.Name,
                    ID = Request.SubCategory.ID
                };

                RCat.SubCategories.Add(SC);
                MUVM.API.Games[0].RootCategories.Add(RCat);

                //Saving changes
                ResourceManager.SaveGamebananaAPI(MUVM.API, AppDataPath);

                //Setting API GUID
                Item.RootCategoryGuid = RCat.Guid;
                Item.SubCategoryGuid = SC.Guid;
            }
            else
            {
                //Setting Root Cat GUID
                Item.RootCategoryGuid = RCat.Guid;

                //Finding Subcategory
                GamebananaSubCategory SC = RCat.SubCategories.SingleOrDefault(cat => cat.ID == Request.SubCategory.ID);

                if(SC == null)
                {
                    SC = new GamebananaSubCategory()
                    {
                        Guid = Guid.NewGuid(),
                        Name = Request.SubCategory.Name,
                        ID = Request.SubCategory.ID
                    };

                    RCat.SubCategories.Add(SC);

                    //Saving changes
                    ResourceManager.SaveGamebananaAPI(MUVM.API, AppDataPath);
                }

                //Setting SCat Guid
                Item.SubCategoryGuid = SC.Guid;
            }

            //Saving changes
            ResourceManager.SaveGamebananaAPI(MUVM.API, AppDataPath);

            return Item;
        }

        #endregion

        #region Actions
        /// <summary>
        /// Takes action based on what's needed
        /// </summary>
        /// <returns>Success state</returns>
        public async Task<bool> TakeAction(ILog QuasarLogger)
        {
            bool ProcessAborted = true;

            if (DownloadNeeded || UpdateNeeded)
            {
                QuasarLogger.Debug("Launching Download");
                ModListItem.ModListItemViewModel.ModStatusValue = "Downloading";
                bool Downloaded = await Download();
                
                //Stopping the process if necessary
                if (Downloaded)
                {
                    QuasarLogger.Debug("Launching Extraction");
                    ModListItem.ModListItemViewModel.ModStatusValue = "Extracting";
                    ModListItem.ModListItemViewModel.ModStatusTextValue = "Please wait";
                    
                    bool Extracted = await Extract();

                    if (Extracted)
                    {
                        QuasarLogger.Debug("Launching File location change");
                        ModListItem.ModListItemViewModel.ModStatusValue = "Processing Files";
                        
                        bool Processed = await ProcessExtractedFiles();

                        if (Processed)
                        {
                            QuasarLogger.Debug("Getting Screenshot");
                            APIScreenshot ScreenshotInformation = await APIRequest.GetScreenshotInformation(QuasarURL.GamebananaItemID);
                            await Downloader.DownloadScreenshot(@"https://images.gamebanana.com/img/ss/mods/"+ ScreenshotInformation.Media.Images[0].File, LibraryItem.Guid.ToString(), Properties.Settings.Default.DefaultDir);
                            ProcessAborted = false;
                        }
                    }
                    else
                    {
                        ProcessAborted = true;
                    }
                }
            }
            if (ProcessAborted)
            {
                ModListItem.ModListItemViewModel.DownloadFailed = true;
            }

            return false;
        }

        /// <summary>
        /// Launches the download sequence
        /// </summary>
        /// <returns>Success state</returns>
        public async Task<bool> Download()
        {
            ModDownloader modDownloader = new ModDownloader(ModListItem.ModListItemViewModel);
            bool success = await modDownloader.DownloadArchiveAsync(QuasarURL);
            return success;
        }

        /// <summary>
        /// Launches the update sequence
        /// </summary>
        /// <returns>Success state</returns>
        public async Task<bool> Update()
        {
            ModDownloader modDownloader = new ModDownloader(ModListItem.ModListItemViewModel);
            bool success = await modDownloader.DownloadArchiveAsync(QuasarURL);
            return success;
        }

        /// <summary>
        /// Launches the extraction sequence
        /// </summary>
        /// <returns>Success Status</returns>
        public async Task<bool> Extract()
        {
            ModFileManager = new ModFileManager(LibraryItem, QuasarURL.ModArchiveFormat);
            Unarchiver un = new Unarchiver(ModListItem.ModListItemViewModel);
            bool success = await un.ExtractArchiveAsync(ModFileManager.DownloadDestinationFilePath, ModFileManager.ArchiveContentFolderPath, QuasarURL.ModArchiveFormat) == 0;
            return success;
        }

        /// <summary>
        /// Moves the download to the library folder
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ProcessExtractedFiles()
        {
            bool success = await ModFileManager.MoveDownload() == 0;
            ModFileManager.ClearDownloadContents();
            return success;
        }

        /// <summary>
        /// Scans the mod files
        /// </summary>
        /// <param name="QuasarModTypes"></param>
        /// <param name="Game"></param>
        /// <returns>Success Status</returns>
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
