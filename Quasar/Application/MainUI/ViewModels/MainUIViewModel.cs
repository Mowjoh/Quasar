using log4net;
using log4net.Appender;
using Quasar.Controls;
using Quasar.Common.Models;
using Quasar.Controls.ModManagement.ViewModels;
using Quasar.Controls.ModManagement.Views;
using Quasar.Settings.Models;
using Quasar.Helpers.ModScanning;
using Quasar.Helpers;
using Quasar.Internal.Tools;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Workshop.FileManagement;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quasar.Settings.Views;
using Quasar.Associations.Views;
using Quasar.Associations.ViewModels;
using System.Windows.Shell;
using Helpers.IPC;
using System.IO;
using System.Globalization;

namespace Quasar.MainUI.ViewModels
{
    public class MainUIViewModel : ObservableObject
    {
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        #region Instance Data

        #region Private
        private ModListItem _SelectedModListItem { get; set; }
        #endregion

        #region Public
        public ModListItem SelectedModListItem
        {
            get => _SelectedModListItem;
            set
            {
                if (_SelectedModListItem == value)
                    return;

                _SelectedModListItem = value;
                if (value != null)
                {
                    //CVM = new FileManagementViewModel(ContentMappings, _SelectedModListItem.ModViewModel.LibraryItem, InternalModTypes, GameDatas);
                    //ContentView.DataContext = CVM;
                }
                OnPropertyChanged("SelectedModListItem");
            }
        }
        /// <summary>
        /// Mutex that serves to know if a Quasar instance is already running
        /// </summary>
        public Mutex serverMutex;
        public ILog QuasarLogger { get; set; }
        #endregion

        #endregion

        #region Resource Data
        public ObservableCollection<Game> Games { get; set; }
        public Game CurrentGame { get; set; }
        public ObservableCollection<QuasarModType> QuasarModTypes { get; set; }
        public ObservableCollection<ModLoader> ModLoaders { get; set; }
        public GamebananaAPI API { get; set; }
        #endregion

        #region User Data
        public Workspace ActiveWorkspace { get; set; }
        public ObservableCollection<Workspace> Workspaces { get; set; }
        public ObservableCollection<LibraryItem> Library { get; set; }
        public ObservableCollection<ContentItem> ContentItems { get; set; }
        public bool UserDataLoaded { get; set; }
        #endregion

        #region Views

        #region Private
        private ObservableCollection<TabItem> _TabItems { get; set; }
        private TabItem _SelectedTabItem { get; set; }
        private int _SelectedTabIndex { get; set; }
        private LibraryView _LibraryView { get; set; }
        private LibraryViewModel _LibraryViewModel { get; set; }
        private AssignmentView _AssignmentView { get; set; }
        private AssociationViewModel _AVM { get; set; }
        private SettingsView _SettingsView { get; set; }
        private ModalPopupViewModel _ModalPopupViewModel { get; set; }

        private bool _BeginGameChoice { get; set; }
        private bool _StopGameChoice { get; set; }

        private bool _CreatorMode { get; set; }
        private bool _AdvancedMode { get; set; }
        private bool _Updating { get; set; } = false;
        private bool _UpdateFinished { get; set; } = false;

        private bool _TabLocked { get; set; }

        private bool _AssignmentTabActive { get; set; }
        private bool _FileManagerTabActive { get; set; }

        private TaskbarItemProgressState _TaskbarProgressState { get; set; }
        #endregion

        #region Public
        public ObservableCollection<TabItem> TabItems
        {
            get => _TabItems;
            set
            {
                if (_TabItems == value)
                    return;

                _TabItems = value;
                OnPropertyChanged("TabItems");
            }
        }
        public TabItem SelectedTabItem
        {
            get => _SelectedTabItem;
            set
            {
                if (!TabLocked)
                {
                    if (_SelectedTabItem == value)
                        return;

                    if(value != null)
                    {
                        if ((string)value.Header == "Overview")
                        {
                            LibraryViewModel.ReloadAllStats();
                        }
                        if ((string)value.Header == "Management")
                        {
                        }
                        _SelectedTabItem = value;
                    }
                    
                    OnPropertyChanged("SelectedTabItem");
                }
            }
        }
        public int SelectedTabIndex
        {
            get => _SelectedTabIndex;
            set
            {
                if (!TabLocked)
                {
                    _SelectedTabIndex = value;
                    OnPropertyChanged("SelectedTabIndex");
                }
            }
        }
        public LibraryView LibraryView
        {
            get => _LibraryView;
            set
            {
                if (_LibraryView == value)
                    return;

                _LibraryView = value;
                OnPropertyChanged("LibraryView");
            }
        }
        public LibraryViewModel LibraryViewModel
        {
            get => _LibraryViewModel;
            set
            {
                if (_LibraryViewModel == value)
                    return;

                _LibraryViewModel = value;
                OnPropertyChanged("LibraryViewModel");
            }
        }

        public AssignmentView AssignmentView
        {
            get => _AssignmentView;
            set
            {
                if (_AssignmentView == value)
                    return;

                _AssignmentView = value;
                OnPropertyChanged("AssignmentView");
            }
        }
        public AssociationViewModel AVM
        {
            get => _AVM;
            set
            {
                if (_AVM == value)
                    return;

                _AVM = value;
                OnPropertyChanged("AVM");
            }
        }

        public SettingsView SettingsView
        {
            get => _SettingsView;
            set
            {
                if (_SettingsView == value)
                    return;

                _SettingsView = value;
                OnPropertyChanged("SettingsView");
            }
        }

        public ModalPopupViewModel ModalPopupViewModel
        {
            get => _ModalPopupViewModel;
            set
            {
                if (_ModalPopupViewModel == value)
                    return;

                _ModalPopupViewModel = value;
                OnPropertyChanged("ModalPopupViewModel");
            }
        }
        public bool BeginGameChoice
        {
            get => _BeginGameChoice;
            set
            {
                if (_BeginGameChoice == value)
                    return;

                _BeginGameChoice = value;
                OnPropertyChanged("BeginGameChoice");
            }
        }
        public bool StopGameChoice
        {
            get => _StopGameChoice;
            set
            {
                if (_StopGameChoice == value)
                    return;

                _StopGameChoice = value;
                OnPropertyChanged("StopGameChoice");
            }
        }

        public bool CreatorMode

        {
            get => _CreatorMode;
            set
            {
                if (_CreatorMode == value)
                    return;


                _CreatorMode = value;
                OnPropertyChanged("CreatorMode");
            }
        }
        public bool AdvancedMode

        {
            get => _AdvancedMode;
            set
            {
                _AdvancedMode = value;
                OnPropertyChanged("AdvancedMode");
            }
        }
        public bool FileManagerTabActive

        {
            get => _FileManagerTabActive;
            set
            {
                _FileManagerTabActive = value;
                OnPropertyChanged("FileManagerTabActive");
            }
        }
        public bool AssignmentTabActive

        {
            get => _AssignmentTabActive;
            set
            {
                _AssignmentTabActive = value;
                OnPropertyChanged("AssignmentTabActive");
            }
        }
        public bool Updating

        {
            get => _Updating;
            set
            {
                _Updating = value;
                OnPropertyChanged("Updating");
            }
        }
        public bool UpdateFinished

        {
            get => _UpdateFinished;
            set
            {
                _UpdateFinished = value;
                OnPropertyChanged("UpdateFinished");
            }
        }
        public bool TabLocked
        {
            get => _TabLocked;
            set
            {
                _TabLocked = value;
                OnPropertyChanged("TabLocked");
            }
        }

        public TaskbarItemProgressState TaskbarProgressState
        {
            get => _TaskbarProgressState;
            set
            {
                _TaskbarProgressState = value;
                OnPropertyChanged("TaskbarProgressState");
            }
        }
        #endregion

        #endregion

        //TO CLEANUP ELSEWHERE
        #region Commands

        #region Private

        private ICommand _OnboardingCancel { get; set; }
        private ICommand _OnboardingNext { get; set; }
        private ICommand _OnboardingPrevious { get; set; }
        #endregion

        #region Public
        public ICommand OnboardingCancel
        {
            get
            {
                if (_OnboardingCancel == null)
                {
                    _OnboardingCancel = new RelayCommand(param => Onboarding(false));
                }
                return _OnboardingCancel;
            }
        }
        public ICommand OnboardingNext
        {
            get
            {
                if (_OnboardingNext == null)
                {
                    _OnboardingNext = new RelayCommand(param => nextStep());
                }
                return _OnboardingNext;
            }
        }
        public ICommand OnboardingPrevious
        {
            get
            {
                if (_OnboardingPrevious == null)
                {
                    _OnboardingPrevious = new RelayCommand(param => previousStep());
                }
                return _OnboardingPrevious;
            }
        }
        #endregion

        #endregion

        //TO CLEANUP ELSEWHERE
        #region Onboarding
        private bool _OnboardingVisible { get; set; } = false;
        public bool OnboardingVisible
        {
            get => _OnboardingVisible;
            set
            {
                _OnboardingVisible = value;
                OnPropertyChanged("OnboardingVisible");
            }
        }
        private int _OnboardingStep { get; set; } = 0;
        public int OnboardingStep
        {
            get => _OnboardingStep;
            set
            {
                _OnboardingStep = value;
                OnPropertyChanged("OnboardingStep");
            }
        }

        public void nextStep()
        {
            switch (OnboardingStep)
            {
                default:
                    break;
                case 2:
                    LibraryViewModel.SelectedModListItem = LibraryViewModel.ModListItems[1];
                    break;
            }
            if (OnboardingStep < 3)
                OnboardingStep++;
        }
        public void previousStep()
        {
            if(OnboardingStep > 1)
                OnboardingStep--;
        }
        public void Onboarding(bool visible)
        {
            OnboardingVisible = visible;
            OnboardingStep = visible ? 1 : 0;
            if (!visible)
            {
                Properties.Settings.Default.Onboarded = true;
                Properties.Settings.Default.Save();
            }
        }
        #endregion

        public MainUIViewModel()
        {
            TaskbarProgressState = TaskbarItemProgressState.None;

            SetupLogger();

            try
            {

                if (Properties.Settings.Default.Language == "[System Language]")
                {
                    Properties.Settings.Default.Language = "EN";
                    Properties.Settings.Default.Save();
                }

                CultureInfo.CurrentUICulture = new CultureInfo(Properties.Settings.Default.Language, false);


                QuasarLogger.Info("Tunnel Setup");
                SetupClientOrServer();

                QuasarLogger.Info("Update Process Started");
                UpdateStatus BootStatus = UpdateCommander.CheckUpdateStatus(QuasarLogger);

                if(BootStatus == UpdateStatus.FirstBoot)
                    UpdateCommander.LaunchFirstBootSequence();

                LoadData();

                QuasarLogger.Info("Loading Views");
                SetupViews();

                AdvancedMode = false;
                CreatorMode = false;

                EventSystem.Subscribe<ModListItem>(ModListItemEvent);
                EventSystem.Subscribe<ModalEvent>(ProcessModalEvent);

                //Bootup actions based on update status
                switch (BootStatus)
                {
                    case UpdateStatus.ToUpdate:
                        ModalEvent Meuh = new ModalEvent()
                        {
                            Action = "Show",
                            EventName = "Updating",
                            Title = "Update Process",
                            Content = "Please be patient, Quasar is updating",
                            OkButtonText = "OK",
                            Type = ModalType.Loader
                        };

                        EventSystem.Publish<ModalEvent>(Meuh);
                        Task.Run(() =>
                        {
                            UpdateCommander.LaunchUpdateSequence(QuasarLogger);
                        });
                        break;

                    case UpdateStatus.PreviouslyInstalled:
                        ModalEvent Meuhdeux = new ModalEvent()
                        {
                            Action = "Show",
                            EventName = "RecoveringInstallation",
                            Title = "Recovering Files and Data",
                            Content = "Please be patient, Quasar is looking for previously installed files",
                            OkButtonText = "OK",
                            Type = ModalType.Loader
                        };

                        EventSystem.Publish<ModalEvent>(Meuhdeux);
                        Task.Run(() =>
                        {
                            UpdateCommander.LaunchInstallRecoverySequence();
                        });
                        break;
                    case UpdateStatus.Regular:
                        InitialWarnings();
                        BackupRestoreUserData(UserDataLoaded);
                        break;
                }
                //If a previous install is detected
                if (BootStatus == UpdateStatus.PreviouslyInstalled)
                {
                    string ModsPath = Properties.Settings.Default.DefaultDir + @"\Library\Mods\";
                    string[] ModFolders = Directory.GetDirectories(ModsPath, "*", SearchOption.TopDirectoryOnly);

                    bool FoundRecoverableMods = false;

                    if (ModFolders.Length > 0)
                    {
                        
                        foreach(string ModFolder in ModFolders)
                        {
                            if(File.Exists(ModFolder + @"\ModInformation.json"))
                            {
                                FoundRecoverableMods = true;
                            }
                        }
                    }
                    if (FoundRecoverableMods)
                    {
                        ModalEvent meuh = new ModalEvent()
                        {
                            Action = "Show",
                            EventName = "RecoverMods",
                            Title = "Recovery possible",
                            Content = "Recoverable mods have been found, please wait while Quasar loads them up",
                            OkButtonText = "OK",
                            Type = ModalType.Loader
                        };
                        EventSystem.Publish<ModalEvent>(meuh);
                        Library = UserDataManager.RecoverMods(Properties.Settings.Default.DefaultDir,AppDataPath, Library, API);

                        meuh.Action = "LoadOK";
                        EventSystem.Publish<ModalEvent>(meuh);
                    }
                    else
                    {
                        InitialWarnings();
                        BackupRestoreUserData(UserDataLoaded);
                    }
                }
            }
            catch (Exception e)
            {
                QuasarLogger.Error(e.Message);
                QuasarLogger.Error(e.StackTrace);
            }
        }

        #region Actions

        //Startup
        /// <summary>
        /// Loads Reference and User Data
        /// </summary>
        public void LoadData()
        {
            //Loading User Data
            try
            {
                Workspaces = UserDataManager.GetWorkspaces(AppDataPath);
                Library = UserDataManager.GetLibrary(AppDataPath);
                ContentItems = UserDataManager.GetContentItems(AppDataPath);

                if (Workspaces.Count == 0)
                {
                    //Creating the base workspace if it's not there
                    Guid CreatedWorkspaceGuid = UserDataManager.CreateBaseWorkspace(AppDataPath);
                    Properties.Settings.Default.Save();

                    Workspaces = UserDataManager.GetWorkspaces(AppDataPath);
                }
                ActiveWorkspace = Workspaces[0];

                UserDataLoaded = true;
            }
            catch (Exception e)
            {
                UserDataLoaded = false;
            }

            //Loading Resource Data
            Games = ResourceManager.GetGames();
            CurrentGame = Games[0];
            QuasarModTypes = ResourceManager.GetQuasarModTypes();
            ModLoaders = ResourceManager.GetModLoaders();
            API = ResourceManager.GetGamebananaAPI();

            API = ResourceManager.CleanGamebananaAPIFile(API, Library);
            ResourceManager.SaveGamebananaAPI(API, AppDataPath);

        }

        /// <summary>
        /// Backs up data or restores it depending on the User Data Load success
        /// </summary>
        /// <param name="LoadSuccess">User Data Load success state</param>
        public void BackupRestoreUserData(bool LoadSuccess)
        {
            if (LoadSuccess)
            {
                UserDataManager.BackupUserDataFiles(AppDataPath);
                Properties.Settings.Default.BackupDate = DateTime.Now;
                Properties.Settings.Default.Save();
            }
            else
            {
                //Sending modal event to warn the user
                ModalEvent meuh = new ModalEvent()
                {
                    EventName = "LoadTrouble",
                    Action = "Show",
                    Type = ModalType.Warning,
                    Title = "Data Corruption",
                    Content = String.Format("Quasar could not load your data. \rA backup will be loaded instead \r(Made on : {0})",Properties.Settings.Default.BackupDate.ToLongDateString()),
                    OkButtonText = "I understand"
                };

                EventSystem.Publish<ModalEvent>(meuh);

                //Restoring data
                UserDataManager.RestoreUserDataFiles(AppDataPath);

                //Loading Data
                LoadData();
            }
        }

        /// <summary>
        /// Sets up all the Views with their ViewModels
        /// </summary>
        public void SetupViews()
        {
            
            EventSystem.Subscribe<SettingItem>(SettingChangedEvent);

            TabItems = new ObservableCollection<TabItem>();

            LibraryView = new LibraryView();
            LibraryViewModel = new LibraryViewModel(this, QuasarLogger);
            LibraryView.DataContext = LibraryViewModel;
            TabItems.Add(new TabItem() { Content = LibraryView, Header = Properties.Resources.MainUI_LibraryTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });

            AVM = new AssociationViewModel(this, QuasarLogger);
            AssignmentView = new AssignmentView() { AssignmentViewModel = AVM };
            AssignmentView.DataContext = AVM;
            TabItems.Add(new TabItem() { Content = AssignmentView, Header = Properties.Resources.MainUI_AssignmentTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });

            SettingsView = new SettingsView();
            TabItems.Add(new TabItem() { Content = SettingsView, Header = Properties.Resources.MainUI_SettingsTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });

            ModalPopupViewModel = new ModalPopupViewModel(QuasarLogger);

            SettingsView.Load();
        }
        /// <summary>
        /// Sets up Quasar as a client or server 
        /// Depends on if a Quasar instance is already running
        /// Takes action according to it's role
        /// </summary>
        public void SetupClientOrServer()
        {
            //serverMutex = PipeHelper.CheckExecuteInstance(serverMutex, QuasarLogger);
            serverMutex = IPCHandler.CheckExecuteInstance(serverMutex, QuasarLogger);
        }

        /// <summary>
        /// Shows the user warnings when he never saw them
        /// </summary>
        public void InitialWarnings()
        {
            if(!Properties.Settings.Default.CFWAcknowledged)
            {
                ModalEvent meuh = new ModalEvent()
                {
                    EventName = "Hacked",
                    Action = "Show",
                    Type = ModalType.Loader,
                    Content = "Quasar is an application that will help you manage mod \rfiles for you. Modding is only for Switches running \rCustom Firmware (CFW). If your Switch is not hacked, \rQuasar will be of no use to you.",
                    OkButtonText = "My switch is running CFW"
                };
                EventSystem.Publish<ModalEvent>(meuh);

                Task task = Task.Run(() => {
                    System.Threading.Thread.Sleep(5000);
                    meuh.Action = "LoadOK";
                    EventSystem.Publish<ModalEvent>(meuh);
                });
            }
            else
            {
                //if (!Properties.Settings.Default.Onboarded)
                if(false)
                {
                    ModalEvent meuh = new ModalEvent()
                    {
                        EventName = "Onboarding",
                        Action = "Show",
                        Type = ModalType.OkCancel,
                        Title = "DO YOU NEED HELP?",
                        Content = "If it's your first time using Quasar, or if you want\ra tour of what you can do, you can have a little demo.",
                        OkButtonText = "Yes, please show me around",
                        CancelButtonText = "No, I don't need help"

                    };
                    EventSystem.Publish<ModalEvent>(meuh);
                }
            }
        }

        //Updates
        /// <summary>
        /// Shows a Modal to block UI while the app updates
        /// </summary>
        public async void ShowUpdateModal()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Updating = UpdateCommander.ScanRequested;
            });

            if (UpdateCommander.ScanRequested)
            {
                await Task.Run(() => {
                    Scannerino.ScanAllMods(this);
                });
            }

            UserDataManager.SaveWorkspaces(Workspaces, AppDataPath);
            UserDataManager.SaveContentItems(ContentItems, AppDataPath);

            Application.Current.Dispatcher.Invoke((Action)delegate {
                UpdateFinished = true;
            });
        }

        /// <summary>
        /// Sends the OK for the update Modal and refreshes UI
        /// </summary>
        public void UpdateOK()
        { 

            LibraryViewModel.CollectionViewSource.View.Refresh();
            LibraryViewModel.ReloadAllStats();

            Updating = false;
        }

        #endregion

        #region Events
        /// <summary>
        /// Processes the incoming Modal Event
        /// </summary>
        /// <param name="me">Modal Event to use</param>
        public void ProcessModalEvent(ModalEvent me)
        {
            switch (me.EventName)
            {
                case "Hacked":
                    if ((me.Action ?? "") == "OK")
                    {
                        Properties.Settings.Default.CFWAcknowledged = true;
                        Properties.Settings.Default.Save();
                        //if (!Properties.Settings.Default.Onboarded)
                        if(false)
                        {
                            ModalEvent meuh = new ModalEvent()
                            {
                                EventName = "Onboarding",
                                Action = "Show",
                                Type = ModalType.OkCancel,
                                Title = "DO YOU NEED HELP?",
                                Content = "If it's your first time using Quasar, or if you want\ra tour of what you can do, you can have a little demo.",
                                OkButtonText = "Yes, please show me around",
                                CancelButtonText = "No, I don't need help"

                            };
                            EventSystem.Publish<ModalEvent>(meuh);
                        }
                    }
                    break;
                case "Onboarding":
                    Onboarding(me.Action == "OK");
                    if (me.Action == "KO")
                    {
                        Properties.Settings.Default.Onboarded = true;
                        Properties.Settings.Default.Save();
                    }
                    break;
                case "Updating":
                    if(me.Action == "OK")
                    {
                        SetupViews();
                    }
                    break;
                case "RecoveringInstallation":
                    if (me.Action == "OK")
                    {
                        SetupViews();
                    }
                    break;
                case "RecoverMods":
                    if (me.Action == "OK")
                    {
                        SetupViews();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Opens the Content tab with this item
        /// </summary>
        /// <param name="_SelectedModListItem"></param>
        public void ModListItemEvent(ModListItem _SelectedModListItem)
        {
            if (_SelectedModListItem.ModViewModel.ActionRequested == "ShowAssignments")
            {
                AssignmentTabActive = true;
                FileManagerTabActive = false;
                SelectedTabItem = TabItems[1];
                SelectedModListItem = _SelectedModListItem;
                //AVM.GetRefreshed("RefreshContents");
            }
        }

        /// <summary>
        /// Updates the UI with the new settings
        /// </summary>
        /// <param name="Setting"></param>
        public void SettingChangedEvent(SettingItem Setting)
        {
            if (Setting.SettingName == "TabLock")
            {
                if(Setting.DisplayValue == "Mod")
                {
                    SelectedTabIndex = 0;
                    SelectedTabItem = TabItems[0];
                }
                TabLocked = Setting.IsChecked;
            }
            if (Setting.SettingName == "EnableCreator")
            {
                CreatorMode = Setting.IsChecked;
            }
            if (Setting.SettingName == "EnableAdvanced")
            {
                AdvancedMode = Setting.IsChecked;
            }
        }
        #endregion

        #region Logging
        /// <summary>
        /// Sets up the logger
        /// </summary>
        public void SetupLogger()
        {
            QuasarLogger = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)QuasarLogger.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;

            appender.ActivateOptions();

            QuasarLogger.Info("------------------------------");
            QuasarLogger.Warn("Quasar Start");
            QuasarLogger.Info("------------------------------");
        }

        #endregion
    }
}
