using Quasar.Common.Models;
using Quasar.Controls.Mod.Models;
using Quasar.Controls.Mod.ViewModels;
using Quasar.Helpers;
using Quasar.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using System.Windows.Data;
using Quasar.Settings.Models;
using System.Windows.Input;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.Helpers.Downloading;
using Quasar.MainUI.ViewModels;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Diagnostics;
using Ookii.Dialogs.Wpf;
using Quasar.Library.Models;
using Workshop.FileManagement;
using Workshop.Web;
using System.IO;
using Quasar.Common;

namespace Quasar.Controls.ModManagement.ViewModels
{
    public class LibraryViewModel : ObservableObject
    {

        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        #region Data

        #region Private
        private ObservableCollection<ModListItem> _ModListItems { get; set; }
        private ObservableCollection<ModManager> _ActiveModManagers { get; set; }
        private GamebananaRootCategory _SelectedGamebananaRootCategory { get; set; }
        private ModListItem _SelectedModListItem { get; set; }
        private ObservableCollection<string> _QuasarDownloads { get; set; }
        private CollectionViewSource _CollectionViewSource { get; set; }
        private MainUIViewModel _MUVM { get; set; }
        
        #endregion

        #region Public
        public ObservableCollection<ModListItem> ModListItems
        {
            get => _ModListItems;
            set
            {
                if (_ModListItems == value)
                    return;

                _ModListItems = value;
                OnPropertyChanged("ModListItems");
            }
        }
        public ObservableCollection<ModManager> ActiveModManagers
        {
            get => _ActiveModManagers;
            set
            {
                if (_ActiveModManagers == value)
                    return;

                _ActiveModManagers = value;
                OnPropertyChanged("ActiveModManagers");
            }
        }
        public GamebananaRootCategory SelectedGamebananaRootCategory
        {
            get => _SelectedGamebananaRootCategory;
            set
            {
                if (_SelectedGamebananaRootCategory == value)
                    return;

                _SelectedGamebananaRootCategory = value;
                CollectionViewSource.View.Refresh();
                OnPropertyChanged("SelectedGamebananaRootCategory");
            }
        }
        public ModListItem SelectedModListItem
        {
            get => _SelectedModListItem;
            set
            {
                if (_SelectedModListItem == value)
                    return;

                if(_SelectedModListItem != null)
                {
                    _SelectedModListItem.ModViewModel.Smol = true;
                }

                _SelectedModListItem = value;
                if (_SelectedModListItem != null)
                {
                    _SelectedModListItem.ModViewModel.Smol = false;
                    //SelectedModListItem.ModViewModel.ActionRequested = "ElementChanged";
                    //EventSystem.Publish<ModListItem>(SelectedModListItem);
                }
                   
                OnPropertyChanged("SelectedModListItem");
            }
        }
        public CollectionViewSource CollectionViewSource
        {
            get => _CollectionViewSource;
            set
            {
                if (_CollectionViewSource == value)
                    return;

                _CollectionViewSource = value;
                OnPropertyChanged("CollectionViewSource");
            }
        }
        public MainUIViewModel MUVM
        {
            get => _MUVM;
            set
            {
                if (_MUVM == value)
                    return;

                _MUVM = value;
                OnPropertyChanged("MUVM");
            }
        }
        ModListItem mliToDelete { get; set; }
        
        
        #endregion

        #endregion

        #region View

        #region Private

        private string _SearchText { get; set; } = "";
        private bool _BlackChecked { get; set; } = true;
        private bool _RedChecked { get; set; } = true;
        private bool _BlueChecked { get; set; } = true;
        private bool _GreenChecked { get; set; } = true;
        private bool _PurpleChecked { get; set; } = true;
        private bool _CreatorMode { get; set; }
        private bool _AdvanceMode { get; set; }
        private bool _TypeFilterSelected { get; set; }
        private bool _CategoryFilterSelected { get; set; }
        private bool _TimeFilterSelected { get; set; }
        private bool _TransferWindowVisible { get; set; }
        private bool _Building { get; set; } = false;
        private string _Logs { get; set; }
        private double _BuilderProgress { get; set; }
        private string _Steps { get; set; } = "Step :";
        private string _SubStep { get; set; } = "Sub-Step :";
        private string _Total { get; set; } = "0/0 Contents";
        private string _Speed { get; set; } = "0 MB";
        private string _Size { get; set; } = "0 MB/s";
        private bool _ProgressBarStyle { get; set; } = false;
        #endregion

        #region Public

        public string SearchText
        {
            get => _SearchText;
            set
            {
                if (_SearchText == value)
                    return;

                _SearchText = value;
                OnPropertyChanged("SearchText");
                CollectionViewSource.View.Refresh();
            }
        }
        public bool BlackChecked

        {
            get => _BlackChecked;
            set
            {
                if (value == false)
                    return;

                _BlackChecked = true;
                _RedChecked = false;
                _BlueChecked = false;
                _GreenChecked = false;
                OnPropertyChanged("BlackChecked");
                OnPropertyChanged("RedChecked");
                OnPropertyChanged("OrangeChecked");
                OnPropertyChanged("GreenChecked");
                CollectionViewSource.View.Refresh();
            }
        }
        public bool RedChecked

        {
            get => _RedChecked;
            set
            {
                if (_RedChecked == value)
                    return;

                _RedChecked = value;
                OnPropertyChanged("RedChecked");
                CollectionViewSource.View.Refresh();
            }
        }
        public bool BlueChecked
        {
            get => _BlueChecked;
            set
            {
                if (_BlueChecked == value)
                    return;

                _BlueChecked = value;
                OnPropertyChanged("BlueChecked");
                CollectionViewSource.View.Refresh();

            }
        }
        public bool GreenChecked
        {
            get => _GreenChecked;
            set
            {
                if (_GreenChecked == value)
                    return;

                _GreenChecked = value;
                OnPropertyChanged("GreenChecked");
                CollectionViewSource.View.Refresh();
            }
        }
        public bool PurpleChecked
        {
            get => _PurpleChecked;
            set
            {
                if (_PurpleChecked == value)
                    return;

                _PurpleChecked = value;
                OnPropertyChanged("PurpleChecked");
                CollectionViewSource.View.Refresh();
            }
        }

        public bool TypeFilterSelected
        {
            get => _TypeFilterSelected;
            set
            {
                if (_TypeFilterSelected == value)
                    return;

                _TypeFilterSelected = value;
                OnPropertyChanged("TypeFilterSelected");

                if (value)
                {
                    if (!CollectionViewSource.SortDescriptions.Any(sd => sd.PropertyName == "ModViewModel.LibraryItem.APICategoryName"))
                    {
                        CollectionViewSource.SortDescriptions.Insert((CollectionViewSource.SortDescriptions.Count - 1), new System.ComponentModel.SortDescription() { PropertyName = "ModViewModel.LibraryItem.APICategoryName", Direction = System.ComponentModel.ListSortDirection.Ascending });
                    }
                }
                else
                {
                    CollectionViewSource.SortDescriptions.Remove(CollectionViewSource.SortDescriptions.Single(sd => sd.PropertyName == "ModViewModel.LibraryItem.APICategoryName"));
                }

                CollectionViewSource.View.Refresh();
            }
        }
        public bool CategoryFilterSelected
        {
            get => _CategoryFilterSelected;
            set
            {
                if (_CategoryFilterSelected == value)
                    return;

                _CategoryFilterSelected = value;
                OnPropertyChanged("CategoryFilterSelected");

                if (value)
                {
                    if (!CollectionViewSource.SortDescriptions.Any(sd => sd.PropertyName == "ModViewModel.APISubCategoryName"))
                    {
                        CollectionViewSource.SortDescriptions.Insert((CollectionViewSource.SortDescriptions.Count - 1), new System.ComponentModel.SortDescription() { PropertyName = "ModViewModel.APISubCategoryName", Direction = System.ComponentModel.ListSortDirection.Ascending });
                    }
                }
                else
                {
                    CollectionViewSource.SortDescriptions.Remove(CollectionViewSource.SortDescriptions.Single(sd => sd.PropertyName == "ModViewModel.APISubCategoryName"));
                }

                CollectionViewSource.View.Refresh();
            }
        }

        public bool TimeFilterSelected
        {
            get => _TimeFilterSelected;
            set
            {
                if (_TimeFilterSelected == value)
                    return;

                _TimeFilterSelected = value;
                OnPropertyChanged("TimeFilterSelected");
                if (value)
                {
                    if(!CollectionViewSource.SortDescriptions.Any(sd => sd.PropertyName == "ModViewModel.LibraryItem.Time"))
                    {
                        CollectionViewSource.SortDescriptions.Insert((CollectionViewSource.SortDescriptions.Count - 1), new System.ComponentModel.SortDescription() { PropertyName = "ModViewModel.LibraryItem.Time", Direction = System.ComponentModel.ListSortDirection.Descending });
                    }
                }
                else
                {
                    CollectionViewSource.SortDescriptions.Remove(CollectionViewSource.SortDescriptions.Single(sd => sd.PropertyName == "ModViewModel.LibraryItem.Time"));
                }
                CollectionViewSource.View.Refresh();
            }
        }

        public bool TransferWindowVisible

        {
            get => _TransferWindowVisible;
            set
            {
                if (_TransferWindowVisible == value)
                    return;


                _TransferWindowVisible = value;
                OnPropertyChanged("TransferWindowVisible");
            }
        }
        public bool Building
        {
            get => _Building;
            set
            {
                if (_Building == value)
                    return;

                _Building = value;
                OnPropertyChanged("Building");
            }
        }

        public string Logs
        {
            get => _Logs;
            set
            {
                _Logs = value;
                OnPropertyChanged("Logs");
            }
        }
        public double BuildProgress
        {
            get => _BuilderProgress;
            set
            {
                _BuilderProgress = value;
                OnPropertyChanged("BuildProgress");
            }
        }
        public string Steps
        {
            get => _Steps;
            set
            {
                _Steps = value;
                OnPropertyChanged("Steps");
            }
        }
        public string SubStep
        {
            get => _SubStep;
            set
            {
                _SubStep = value;
                OnPropertyChanged("SubStep");
            }
        }
        public string Total
        {
            get => _Total;
            set
            {
                _Total = value;
                OnPropertyChanged("Total");
            }
        }
        public string Speed
        {
            get => _Speed;
            set
            {
                _Speed = value;
                OnPropertyChanged("Speed");
            }
        }
        public string Size
        {
            get => _Size;
            set
            {
                _Size = value;
                OnPropertyChanged("Size");
            }
        }
        public bool ProgressBarStyle
        {
            get => _ProgressBarStyle;
            set
            {
                _ProgressBarStyle = value;
                OnPropertyChanged("ProgressBarStyle");
            }
        }
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _AddManual { get; set; }
        private ICommand _ResetFilters { get; set; }
        private ICommand _LaunchTransfer { get; set; }
        private ICommand _CloseBuildCommand { get; set; }
        #endregion

        #region Public
        public ICommand AddManual
        {
            get
            {
                if (_AddManual == null)
                {
                    _AddManual = new RelayCommand(param => AddManualMod());
                }
                return _AddManual;
            }
        }
        public ICommand ResetFilters
        {
            get
            {
                if (_ResetFilters == null)
                {
                    _ResetFilters = new RelayCommand(param => ResetFilter());
                }
                return _ResetFilters;
            }
        }
        public ICommand LauchTransferCommand
        {
            get
            {
                if (_LaunchTransfer == null)
                {
                    _LaunchTransfer = new RelayCommand(param => AskBuild());
                }
                return _LaunchTransfer;
            }
        }
        public ICommand CloseBuildCommand
        {
            get
            {
                if (_CloseBuildCommand == null)
                {
                    _CloseBuildCommand = new RelayCommand(param => CloseBuildWindow());
                }
                return _CloseBuildCommand;
            }
        }
        #endregion

        #endregion

        public ILog QuasarLogger { get; set; }

        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <param name="_MUVM"></param>
        /// <param name="_QuasarLogger"></param>
        public LibraryViewModel(MainUIViewModel _MUVM, ILog _QuasarLogger)
        {
            MUVM = _MUVM;
            QuasarLogger = _QuasarLogger;

            QuasarLogger.Debug("Parsing Mod List Items");
            ParseModListItems();

            CollectionViewSource = new CollectionViewSource();
            CollectionViewSource.Source = ModListItems;
            CollectionViewSource.SortDescriptions.Add(new System.ComponentModel.SortDescription() { PropertyName= "ModViewModel.LibraryItem.Name", Direction= System.ComponentModel.ListSortDirection.Ascending });
            CollectionViewSource.Filter += ModTypeFilter;

            EventSystem.Subscribe<ModViewModel>(GetModListElementTrigger);
            EventSystem.Subscribe<QuasarDownload>(Download);
            EventSystem.Subscribe<ModalEvent>(ProcessIncomingModalEvent);

            ActiveModManagers = new ObservableCollection<ModManager>();
        }

        #region Library View Management

        /// <summary>
        /// Creates and fills the mod list
        /// </summary>
        public void ParseModListItems()
        {
            ModListItems = new ObservableCollection<ModListItem>();


            foreach (LibraryItem li in MUVM.Library)
            {
                Game gamu = MUVM.Games.Single(g => g.ID == li.GameID);

                ModListItem mli = new ModListItem(this, QuasarLogger,_LibraryItem: li, _Game: gamu);
                mli.ModViewModel.LoadStats();
                ModListItems.Add(mli);
            }
            
        }
        
        /// <summary>
        /// Function called to filter the mod List
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ModTypeFilter(object sender, FilterEventArgs e)
        {
            //Getting Item
            ModListItem mli = e.Item as ModListItem;

            //Getting Color Match Status
            int ColorValue = mli.ModViewModel.ContentStatValue;
            bool MatchingCheckBox = (RedChecked && !mli.ModViewModel.LibraryItem.Included)
                 || (BlueChecked && mli.ModViewModel.Edited && mli.ModViewModel.LibraryItem.Included)
                 || (GreenChecked && !mli.ModViewModel.Edited && mli.ModViewModel.LibraryItem.Included);

            //Getting Type Select Match
            bool NoSelectedType = SelectedGamebananaRootCategory == null;
            bool MatchingSelectedType = false;
            if(!NoSelectedType)
                MatchingSelectedType = mli.ModViewModel.LibraryItem.GBItem?.RootCategoryGuid == SelectedGamebananaRootCategory.Guid;

            //Getting Filter Text Match
            bool EmptyText = SearchText.Length == 0;
            bool MatchingName = mli.ModViewModel.LibraryItem.Name.Contains(SearchText.ToLower()) && !EmptyText;
            bool MatchingCategory = mli.ModViewModel.APISubCategoryName.ToLower().Contains(SearchText.ToLower()) && !EmptyText;

            //Match status
            if (MatchingCheckBox && (MatchingSelectedType || NoSelectedType) && (MatchingName || MatchingCategory || EmptyText))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }
        
        /// <summary>
        /// Reloads the workspace presence status for all list items
        /// </summary>
        public void ReloadAllStats()
        {
            foreach (ModListItem i in ModListItems)
            {
                i.ModViewModel.LoadStats();
            }
        }

        /// <summary>
        /// Refreshes the view
        /// </summary>
        public void ViewRefresh()
        {
            CollectionViewSource.Source = null;
            ParseModListItems();
            CollectionViewSource.Source = ModListItems;
            CollectionViewSource.View.Refresh();
        }
        
        /// <summary>
        /// Flashes the task bar to notify the user that it's working
        /// </summary>
        public void Flash()
        {
            TaskbarManager.Instance.SetProgressValue(100, 100, Process.GetCurrentProcess().MainWindowHandle);
            System.Threading.Thread.Sleep(500);
            TaskbarManager.Instance.SetProgressValue(0, 100, Process.GetCurrentProcess().MainWindowHandle);
        }

        #endregion

        #region Downloads

        /// <summary>
        /// Trigger for downloads
        /// </summary>
        /// <param name="download"></param>
        public void Download(QuasarDownload download)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {

                Task.Run(() => Flash());
                Task.Run(() => DownloadMod(download.QuasarURL));
            });
        }

        /// <summary>
        /// Download process
        /// </summary>
        /// <param name="_quasar_url"></param>
        /// <returns></returns>
        public async Task<bool> DownloadMod(string _quasar_url)
        {
            ModManager manager = new ModManager(_quasar_url);

            if (!TryLaunchDownload(manager))
                return false;

            //Evaluating if something needs to be done
            bool result = await manager.EvaluateActionNeeded(MUVM);

            if (result && manager.ActionNeeded)
            {
                try
                {
                    manager.ModListItem = GetModListItem(manager);

                    //Executing tasks for this mod
                    await manager.TakeAction(QuasarLogger, MUVM.QuasarModTypes, MUVM.CurrentGame);

                    foreach (ContentItem managerScannedContent in manager.ScannedContents)
                    {
                        MUVM.ContentItems.Add(managerScannedContent);
                    }
                    UserDataManager.SaveSeparatedContentItems(MUVM.ContentItems, Properties.QuasarSettings.Default.DefaultDir);
                    //Updating Library
                    if (manager.DownloadNeeded)
                    {
                        AddToLibrary(manager);
                    }
                    else
                    {
                        EditLibrary(manager);
                    }

                    ReloadAllStats();
                }
                catch (Exception e)
                {
                    manager.ModListItem.ModViewModel.DownloadFailed = true;

                    QuasarLogger.Error("Could not download mod");
                    QuasarLogger.Error(e.Message);
                    QuasarLogger.Error(e.StackTrace);
                }


                manager.ModListItem.ModViewModel.Downloading = false;
            }

            ActiveModManagers.Remove(manager);

            //Unlocking tabs if necessary
            if (ActiveModManagers.Count == 0)
                SendTabLock(false);

            return true;
        }

        public ModListItem GetModListItem(ModManager _manager)
        {
            ModListItem modListItem = null;

            if (_manager.DownloadNeeded)
            {
                QuasarLogger.Debug("New Item");
                //Creating new Mod List Item
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    modListItem = new ModListItem(this, QuasarLogger, _manager.LibraryItem, MUVM.Games[0], true);
                    ModListItems.Add(modListItem);
                });

            }
            else
            {
                QuasarLogger.Debug("Existing Item");
                //Parsing Existing Mod List Item
                modListItem = ModListItems.Single(i => i.ModViewModel.LibraryItem.GBItem.GamebananaItemID.ToString() == _manager.QuasarURL.GamebananaItemID);
            }

            return modListItem;
        }

        public void AddToLibrary(ModManager _manager)
        {
            QuasarLogger.Debug("Adding to Library");
            //If the mod is new and downloaded
            _manager.LibraryItem.Included = true;
            MUVM.Library.Add(_manager.LibraryItem);
            UserDataManager.SaveLibrary(MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);
            //Saving changes
            UserDataManager.SaveSeparatedGamebananaApi(MUVM.API, MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);

            GamebananaRootCategory RC = MUVM.API.Games[0].RootCategories.Single(c => c.Guid == _manager.LibraryItem.GBItem.RootCategoryGuid);
            GamebananaSubCategory SC = RC.SubCategories.Single(c => c.Guid == _manager.LibraryItem.GBItem.SubCategoryGuid);
            APIData MI = new APIData()
            {
                GamebananaItemID = _manager.QuasarURL.GamebananaItemID,
                GamebananaModType = _manager.QuasarURL.APICategoryName
            };
            UserDataManager.SaveAPIData(MI, Properties.QuasarSettings.Default.DefaultDir, _manager.LibraryItem.Guid.ToString());
        }

        public void EditLibrary(ModManager _manager)
        {
            QuasarLogger.Debug("Editing Library");
            //If the mod is updated
            LibraryItem li = MUVM.Library.Single(i => i.Guid == _manager.LibraryItem.Guid);
            li = _manager.LibraryItem;
            UserDataManager.SaveLibrary(MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);
            //Saving changes
            UserDataManager.SaveSeparatedGamebananaApi(MUVM.API, MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);

            GamebananaRootCategory RC = MUVM.API.Games[0].RootCategories.Single(c => c.Guid == _manager.LibraryItem.GBItem.RootCategoryGuid);
            GamebananaSubCategory SC = RC.SubCategories.Single(c => c.Guid == _manager.LibraryItem.GBItem.SubCategoryGuid);
            APIData MI = new APIData()
            {
                GamebananaItemID = _manager.QuasarURL.GamebananaItemID,
                GamebananaModType = _manager.QuasarURL.APICategoryName
            };
            UserDataManager.SaveAPIData(MI, Properties.QuasarSettings.Default.DefaultDir, _manager.LibraryItem.Guid.ToString());
        }

        public bool TryLaunchDownload(ModManager _manager)
        {
            bool ManagerAvailable = !ActiveModManagers.Any(m => m.QuasarURL.GamebananaItemID == _manager.QuasarURL.GamebananaItemID);
            
            if (!ManagerAvailable) 
                return false;

            ActiveModManagers.Add(_manager);
            SendTabLock(true);

            return true;
        }

        public void SendTabLock(bool Active)
        {
            EventSystem.Publish<SettingItem>(new SettingItem
            {
                IsChecked = Active,
                SettingName = "TabLock",
                DisplayValue = "Mod"
            });
        }
        #endregion

        #region Transfers

        /// <summary>
        /// Asks the user if he really wants to clear the mods folder before sending
        /// </summary>
        public void AskBuild()
        {
            if (Building)
                return;

            if (Properties.QuasarSettings.Default.TransferQuasarFoldersOnly && Properties.QuasarSettings.Default.SuppressManageWarning)
            {
                Popup.CallModal(Modal.TransferWarning);
            }
            else
            {
                Build();
            }
        }

        /// <summary>
        /// Starts the transfer process
        /// </summary>
        public async void Build()
        {
            if (Building)
                return;

            StartBuildProcess();

            //Starting the transfer for the selected FileWriter
            FileWriter FW = await GetFileWriter();

            if (FW == null)
            {
                BuildLog(Properties.Resources.Transfer_Log_Error, Properties.Resources.Transfer_Log_NoLaunch);
                EndBuildProcess();
                return;
            }

            await BuildProcess(FW);

            EndBuildProcess();
        }

        public void StartBuildProcess()
        {
            QuasarLogger.Info("Transfer Started");
            TransferWindowVisible = true;

            Building = true;
            //Setting Tab Lock ON
            EventSystem.Publish<SettingItem>(new SettingItem
            {
                IsChecked = true,
                SettingName = "TabLock"
            });

            ResetLogs();

            //Everything is in prepared to start the transfer
            BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_ProcessStart);
        }

        public void EndBuildProcess()
        {
            QuasarLogger.Info(Properties.Resources.Transfer_Log_TransferFinished);
            SetStep(Properties.Resources.Transfer_Step_FinishedStepText);
            SetSubStep("");
            SetProgression(100);
            SetProgressionStyle(false);
            SetSize("");
            SetSpeed("");
            SetTotal("0", "0");
            TaskbarManager.Instance.SetProgressValue(100, 100, Process.GetCurrentProcess().MainWindowHandle);
            Building = false;
            BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_TransferFinished);

            EventSystem.Publish<SettingItem>(new SettingItem
            {
                IsChecked = false,
                SettingName = "TabLock"
            });

        }

        public async Task<bool> BuildProcess(FileWriter _writer)
        {
            SmashBuilder SmashBuilder = new(_writer, this);
            bool BuildSuccess = false;

            await Task.Run(() =>
            {
                BuildSuccess = SmashBuilder.StartBuild().Result;
            });

            if (!BuildSuccess)
                BuildLog("Error", "Something went wrong");

            return BuildSuccess;
        }

        /// <summary>
        /// Gets the FileWriter used for the transfer
        /// </summary>
        /// <returns>The initialized FileWriter </returns>
        public async Task<FileWriter> GetFileWriter()
        {
            FileWriter FW = null;

            switch (Properties.QuasarSettings.Default.PreferredTransferMethod)
            {
                case "FTP":
                    QuasarLogger.Info("FTP Transfer wanted");
                    //FTP FileWriter
                    BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_FTPConnectionTest);
                    FW = new FTPWriter(this) { Log = QuasarLogger };
                    bool ok = await FW.VerifyOK();
                    if (!ok)
                    {
                        BuildLog(Properties.Resources.Transfer_Log_Error, Properties.Resources.Transfer_Log_FTPConnectionFail);
                    }
                    else
                    {
                        QuasarLogger.Info($"FTP Settings are : {Properties.QuasarSettings.Default.FtpIP}:{Properties.QuasarSettings.Default.FtpPort}");
                        BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_FTPConnectionSuccess);
                    }
                    break;
                case "SD":
                    QuasarLogger.Info("SD Transfer wanted");

                    //SD Card FileWriter

                    if (Properties.QuasarSettings.Default.SelectedSD == null)
                    {
                        QuasarLogger.Info($"SelectedSD not present");
                        BuildLog(Properties.Resources.Transfer_Log_Error, Properties.Resources.Transfer_Log_NoSDSelected);
                        FW = null;
                    }
                    else
                    {
                        QuasarLogger.Info($"SD is {Properties.QuasarSettings.Default.SelectedSD}");
                        if (DriveInfo.GetDrives().Any(d => d.Name == Properties.QuasarSettings.Default.SelectedSD))
                        {
                            FW = new SDWriter(this) { LetterPath = Properties.QuasarSettings.Default.SelectedSD, Log = QuasarLogger };
                        }
                        else
                        {
                            BuildLog(Properties.Resources.Transfer_Log_Error, Properties.Resources.Transfer_Log_NoSDSelected);
                            FW = null;
                        }

                    }
                    break;
                case "Disk":
                    QuasarLogger.Info("Disk Transfer wanted");
                    QuasarLogger.Info($"Path is {Properties.QuasarSettings.Default.DiskPath}");
                    if (Directory.Exists(Properties.QuasarSettings.Default.DiskPath))
                    {
                        FW = new DiskWriter(this) { DiskPath = Properties.QuasarSettings.Default.DiskPath, Log = QuasarLogger };
                    }
                    else
                    {
                        BuildLog(Properties.Resources.Transfer_Log_Error, Properties.Resources.Transfer_Log_DiskPathUnavailable);
                        FW = null;
                    }

                    break;
            }

            return FW;
        }

        /// <summary>
        /// Sets UI for the end build process
        /// </summary>
        

        #endregion

        #region User Actions

        /// <summary>
        /// Adds a new Manual mod entry to the library
        /// </summary>
        public void AddManualMod()
        {
            LibraryItem li = new LibraryItem()
            {
                Name = "Manually added mod",
                GameID = 1,
                Guid = Guid.NewGuid(),
                Time = DateTime.Now
            };

            ModListItem mli = new ModListItem(this, QuasarLogger, li, MUVM.Games[0]);

            MUVM.Library.Add(li);
            ModListItems.Add(mli);
            CollectionViewSource.View.Refresh();
            UserDataManager.SaveLibrary(MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);
            SelectedModListItem = mli;
            mli.ModViewModel.RenameMod();


        }

        /// <summary>
        /// Resets the selected Filters
        /// </summary>
        public void ResetFilter()
        {
            SelectedGamebananaRootCategory = null;
            SearchText = "";
            OnPropertyChanged("SelectedGamebananaRootCategory");
            OnPropertyChanged("SearchText");
        }

        /// <summary>
        /// Closes the Build Window UI
        /// </summary>
        public void CloseBuildWindow()
        {
            TransferWindowVisible = false;
        }
        #endregion

        #region Event System

        //Mod List Item Triggers
        /// <summary>
        /// Trigger for an event incoming from a Mod List Item View Model
        /// </summary>
        /// <param name="modViewModeliewModel"></param>
        public void GetModListElementTrigger(ModViewModel modViewModel)
        {
            ModListItem MLI = ModListItems.Single(m => m.ModViewModel == modViewModel);
            switch (modViewModel.ActionRequested)
            {
                case "Delete":
                    AskDeleteMod(MLI);
                    break;
                case "Add":
                    AddToTransferList(MLI);
                    break;
                case "Remove":
                    RemoveMod(MLI);
                    break;
                case "ShowContents":
                    EditModFiles(MLI);
                    break;
                case "ShowAssignments":
                    ShowModAssignments(MLI);
                    break;
                case "SaveLibrary":
                    SaveLibrary(MLI);
                    break;
                case "Update":
                    UpdateMod(MLI);
                    break;
                case "Editing":
                    EditingName(MLI);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Asks if the user wants to delete the mod
        /// </summary>
        /// <param name="item"></param>
        public void AskDeleteMod(ModListItem item)
        {
            mliToDelete = item;

            if (Properties.QuasarSettings.Default.SupressModDeletion)
            {
                Popup.CallModal(Modal.DeleteMod);
            }
            else
            {
                DeleteMod(mliToDelete);
            }
        }

        /// <summary>
        /// Adds this mod's contents to the workspace
        /// </summary>
        /// <param name="MLI"></param>
        public void AddToTransferList(ModListItem MLI)
        {
            QuasarLogger.Info($"Adding Mod to transfer list : '{MLI.ModViewModel.LibraryItem.Name}'");
            MUVM.Library.Single(li => li.Guid == MLI.ModViewModel.LibraryItem.Guid).Included = true;
            UserDataManager.SaveLibrary(MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);
            ReloadAllStats();
            CollectionViewSource.View.Refresh();
        }

        /// <summary>
        /// Removes this mod's contents from the workspace
        /// </summary>
        /// <param name="MLI"></param>
        public void RemoveMod(ModListItem MLI)
        {
            QuasarLogger.Info($"Removing Mod from transfer list : '{MLI.ModViewModel.LibraryItem.Name}'");
            MUVM.Library.Single(li => li.Guid == MLI.ModViewModel.LibraryItem.Guid).Included = false;
            UserDataManager.SaveLibrary(MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);
            ReloadAllStats();
            CollectionViewSource.View.Refresh();

        }

        /// <summary>
        /// Triggers this mod's update
        /// </summary>
        /// <param name="MLI"></param>
        public void UpdateMod(ModListItem MLI)
        {
            GamebananaRootCategory RCat = MUVM.API.Games[0].RootCategories.Single(c => c.Guid == MLI.ModViewModel.LibraryItem.GBItem.RootCategoryGuid);

            Application.Current.Dispatcher.Invoke((Action)delegate {
                Task.Run(() =>
                DownloadMod(APIRequest.GetQuasarDownloadURL("",RCat.Name, MLI.ModViewModel.LibraryItem.GBItem.GamebananaItemID.ToString())));;
            });
        }

        /// <summary>
        /// Show this mod's file view
        /// </summary>
        /// <param name="MLI"></param>
        public void EditModFiles(ModListItem MLI)
        {
            QuasarLogger.Info($"Editing Mod files for : '{MLI.ModViewModel.LibraryItem.Name}'");
            if (SelectedModListItem == null)
                return;

            Popup.CallModal(Modal.ImportFiles);

            ModFileManager FileManager = new ModFileManager(MLI.ModViewModel.LibraryItem);

            VistaFolderBrowserDialog newDialog = new VistaFolderBrowserDialog();
            newDialog.ShowDialog();

            if (newDialog.SelectedPath == "")
                return;

            QuasarLogger.Info($"Selected path is : '{newDialog.SelectedPath}'");

            //Importing files
            string NewInstallPath = newDialog.SelectedPath;
            FileManager.ImportFolder(NewInstallPath);

            Popup.UpdateModalStatus(Modal.ImportFiles,true);
            QuasarLogger.Info($"Finished importing files");
        }

        /// <summary>
        /// Triggers the UI to show the assignments for a certain mod
        /// </summary>
        /// <param name="MLI">The corresponding ModListItem</param>
        public void ShowModAssignments(ModListItem MLI)
        {
            SelectedModListItem = MLI;
            EventSystem.Publish<ModalEvent>(new(){ EventName = "ShowAssignments"});
        }

        /// <summary>
        /// Saves the Library
        /// </summary>
        /// <param name="MLI"></param>
        public void SaveLibrary(ModListItem MLI)
        {
            UserDataManager.SaveLibrary(MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);
        }

        /// <summary>
        /// Saves the Library after the mod's been renamed
        /// </summary>
        /// <param name="MLI"></param>
        public void EditingName(ModListItem MLI)
        {
            foreach (ModListItem ModListItem in ModListItems)
            {
                if (ModListItem != MLI && ModListItem.ModViewModel.LibraryItem.Editing)
                {
                    ModListItem.ModViewModel.RenameMod();
                }
            }
        }
        #endregion

        #region Modal Events

        /// <summary>
        /// Trigger when an incoming ModalEvent is received
        /// </summary>
        /// <param name="_modal_event"></param>
        public void ProcessIncomingModalEvent(ModalEvent _modal_event)
        {
            if (_modal_event.EventName == "DeleteMod")
            {
                switch (_modal_event.Action)
                {
                    case "OK":
                        DeleteMod(mliToDelete);
                        break;
                    default:
                        break;
                }
            }

            if (_modal_event.EventName == "TransferWarning")
            {
                switch (_modal_event.Action)
                {
                    case "OK":
                        Build();
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Deletes the mod from the Library
        /// </summary>
        /// <param name="_mod_list_item"></param>
        public void DeleteMod(ModListItem _mod_list_item)
        {
            QuasarLogger.Info($"Deleting Mod : '{_mod_list_item.ModViewModel.LibraryItem.Name}'");

            //Removing Files
            ModFileManager modFileManager = new ModFileManager(_mod_list_item.ModViewModel.LibraryItem);
            modFileManager.DeleteFiles();

            //Removing from ContentMappings
            List<ContentItem> relatedMappings = MUVM.ContentItems.Where(cm => cm.LibraryItemGuid == _mod_list_item.ModViewModel.LibraryItem.Guid).ToList();
            foreach (ContentItem ci in relatedMappings)
            {
                MUVM.ContentItems.Remove(ci);
            }

            ModListItems.Remove(_mod_list_item);
            MUVM.Library.Remove(_mod_list_item.ModViewModel.LibraryItem);

            //Writing changes
            UserDataManager.SaveLibrary(MUVM.Library, Properties.QuasarSettings.Default.DefaultDir);
            UserDataManager.SaveSeparatedContentItems(MUVM.ContentItems, Properties.QuasarSettings.Default.DefaultDir);

            Application.Current.Dispatcher.Invoke((Action)delegate {
                EventSystem.Publish<string>("RefreshContents");
            });
        }

        #endregion

        #region Async UI Modifiers
        public void SetStep(string s)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Steps = String.Format(Properties.Resources.Transfer_Step_StepText, s);
            }));
        }
        public void SetSubStep(string s)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                SubStep = String.Format(Properties.Resources.Transfer_Step_SubStepText, s);
            }));
        }

        public void SetTotal(string current, string total)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Total = String.Format(Properties.Resources.Transfer_Step_Files, current, total);

                int cur = int.Parse(current);
                int tot = int.Parse(total);
                if (cur >= 0 && tot >= 0)
                {

                    TaskbarManager.Instance.SetProgressValue(cur, tot, Process.GetCurrentProcess().MainWindowHandle);

                }
            }));
        }
        public void SetProgression(double value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                BuildProgress = value;
            }));
        }

        public void SetSpeed(string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Speed = value;
            }));
        }
        public void SetSize(string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Size = value;
            }));
        }
        public void SetProgressionStyle(bool IsIndeterminate)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                ProgressBarStyle = IsIndeterminate;
            }));
        }

        public void BuildLog(string type, string log)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Logs += String.Format("{0} - {1} \r\n", type, log);
            }));
        }

        public void ResetLogs()
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Logs = "";
            }));
        }
        #endregion
    }

    //Sort Type Enum
    public enum SortType { AtoZ = 1, ZtoA = 2, Disabled = 3}
}
