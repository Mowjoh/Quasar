using log4net;
using log4net.Appender;
using Quasar.Controls;
using Quasar.Common.Models;
using Quasar.Controls.ModManagement.ViewModels;
using Quasar.Controls.ModManagement.Views;
using Quasar.Settings.Models;
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
using System.Windows.Shell;
using Helpers.IPC;
using System.IO;
using System.Globalization;
using System.Windows.Threading;
using Quasar.Music.Views;
using Quasar.Other.Views;
using Quasar.Settings.ViewModels;
using Quasar.Skins.Views;
using Quasar.Stages.Views;

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
        private SkinManagementView _SkinManagementView { get; set; }
        private StageManagementView _StageManagementView { get; set; }
        private MusicManagerView _MusicManagementView { get; set; }
        private OtherManagementView _OtherManagementView { get; set; }
        private SettingsView _SettingsView { get; set; }
        private ModalPopupViewModel _ModalPopupViewModel { get; set; }

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
                        if ((string)value.Header == Properties.Resources.MainUI_LibraryTabHeader)
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

        public SkinManagementView SkinManagementView
        {
            get => _SkinManagementView;
            set
            {
                if (_SkinManagementView == value)
                    return;

                _SkinManagementView = value;
                OnPropertyChanged("SkinManagementView");
            }
        }
        public StageManagementView StageManagementView
        {
            get => _StageManagementView;
            set
            {
                if (_StageManagementView == value)
                    return;

                _StageManagementView = value;
                OnPropertyChanged("StageManagementView");
            }
        }

        public OtherManagementView OtherManagementView
        {
            get => _OtherManagementView;
            set
            {
                if (_OtherManagementView == value)
                    return;

                _OtherManagementView = value;
                OnPropertyChanged("OtherManagementView");
            }
        }


        public MusicManagerView MusicManagementView
        {
            get => _MusicManagementView;
            set
            {
                if (_MusicManagementView == value)
                    return;

                _MusicManagementView = value;
                OnPropertyChanged("MusicManagementView");
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

        /// <summary>
        /// Constructor for the MainUI ViewModel
        /// </summary>
        public MainUIViewModel()
        {
            TaskbarProgressState = TaskbarItemProgressState.None;

            SetupLogger();

            try
            {
                //Doing initial tasks
                UpdateStatus BootStatus = StartupActions();

                //Loading data referentials and setting up views
                DataSetup();

                //Bootup actions based on update status
                switch (BootStatus)
                {
                    case UpdateStatus.ToUpdate:
                        UpdateTask();
                        break;

                    case UpdateStatus.PreviouslyInstalled:
                        PreviousInstallTask();
                        break;

                    case UpdateStatus.Regular:
                        QuasarLogger.Debug("Update Status is Regular Boot");
                        break;
                }
            }
            catch (Exception e)
            {
                QuasarLogger.Error(e.Message);
                QuasarLogger.Error(e.StackTrace);
            }
        }

        #region Loads

        public void DataSetup()
        {
            QuasarLogger.Debug("Loading Data");
            LoadData();

            QuasarLogger.Info("Loading Views");
            SetupViews();

            EventSystem.Subscribe<ModListItem>(ModListItemEvent);
            EventSystem.Subscribe<ModalEvent>(ProcessModalEvent);
        }

        /// <summary>
        /// Loads Reference and User Data
        /// </summary>
        public void LoadData()
        {
            //Loading User Data
            try
            {
                Library = UserDataManager.GetSeparatedLibrary(Properties.QuasarSettings.Default.DefaultDir);
                ContentItems = UserDataManager.GetSeparatedContentItems(Properties.QuasarSettings.Default.DefaultDir);
                API = UserDataManager.GetSeparatedGamebananaApi(Properties.QuasarSettings.Default.DefaultDir);
                UserDataLoaded = true;
            }
            catch (Exception e)
            {
                UserDataLoaded = false;
            }

            //Loading Resource Data
            Games = ResourceManager.GetGames(Properties.QuasarSettings.Default.AppPath);
            CurrentGame = Games[0];
            QuasarModTypes = ResourceManager.GetQuasarModTypes(Properties.QuasarSettings.Default.AppPath);
            
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

            SkinManagementView = new(this,QuasarLogger);
            TabItems.Add(new TabItem(){Content = SkinManagementView , Header = "Skin", Foreground = new SolidColorBrush() { Color = Colors.White } });

            StageManagementView = new(this, QuasarLogger);
            TabItems.Add(new TabItem() { Content = StageManagementView, Header = "Stage", Foreground = new SolidColorBrush() { Color = Colors.White } });

            MusicManagementView = new(this, QuasarLogger);
            TabItems.Add(new TabItem() { Content = MusicManagementView, Header = "Music", Foreground = new SolidColorBrush() { Color = Colors.White } });

            OtherManagementView = new(this, QuasarLogger);
            TabItems.Add(new TabItem() { Content = OtherManagementView, Header = "Other", Foreground = new SolidColorBrush() { Color = Colors.White } });

            SettingsView = new SettingsView();
            TabItems.Add(new TabItem() { Content = SettingsView, Header = Properties.Resources.MainUI_SettingsTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });

            ModalPopupViewModel = new ModalPopupViewModel(QuasarLogger);

            SettingsView.Load(QuasarLogger);
        }

        #endregion

        #region Startup Tasks
        public UpdateStatus StartupActions()
        {
            QuasarLogger.Info("Tunnel Setup");
            SetupClientOrServer();

            QuasarLogger.Info("Update Process Started");
            UpdateStatus BootStatus = UpdateCommander.CheckUpdateStatus(QuasarLogger);

            CultureInfo.CurrentUICulture = new CultureInfo(Properties.QuasarSettings.Default.Language, false);

            if (BootStatus == UpdateStatus.FirstBoot)
            {
                QuasarLogger.Debug("First Boot behavior started");
                UpdateCommander.LaunchFirstBootSequence();
                if (File.Exists(Properties.QuasarSettings.Default.DefaultDir + @"\Library\Library.json"))
                {
                    BootStatus = UpdateStatus.ToUpdate;
                }
            }

            BootStatus = UpdateCommander.CheckPreviousInstall(QuasarLogger, BootStatus,
                Properties.QuasarSettings.Default.DefaultDir);

            return BootStatus;
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

        public void UpdateTask()
        {
            QuasarLogger.Debug("Update Status is To Update");
            ModalEvent Meuh = new ModalEvent()
            {
                Action = "Show",
                EventName = "Updating",
                Title = Properties.Resources.MainUI_Modal_UpdateProgressTitle,
                Content = Properties.Resources.MainUI_Modal_UpdateProgressContent,
                OkButtonText = "OK",
                Type = ModalType.Loader
            };

            EventSystem.Publish<ModalEvent>(Meuh);
            Task.Run(() =>
            {
                UpdateCommander.LaunchUpdateSequence(QuasarLogger, Library, API);
            });
        }

        public void PreviousInstallTask()
        {
            QuasarLogger.Debug("Update Status is Previously installed");
            ModalEvent Meuhdeux = new ModalEvent()
            {
                Action = "Show",
                EventName = "RecoveringInstallation",
                Title = Properties.Resources.MainUI_Modal_RecoverProgressTitle,
                Content = Properties.Resources.MainUI_Modal_RecoverProgressContent,
                OkButtonText = "OK",
                Type = ModalType.Loader
            };

            //EventSystem.Publish<ModalEvent>(Meuhdeux);
            //Task.Run(() =>
            //{
            //    Library = UpdateCommander.LaunchInstallRecoverySequence(Library, API, QuasarLogger);
            //    UserDataManager.SaveLibrary(Library, AppDataPath);
            //});
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
                case "Updating":
                    if(me.Action == "OK")
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate {
                            LibraryViewModel.ViewRefresh();
                        });
                        
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
                        Application.Current.Dispatcher.Invoke((Action)delegate {
                            LibraryViewModel.ViewRefresh();
                        });
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
            appender.File = Properties.QuasarSettings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;

            appender.ActivateOptions();

            QuasarLogger.Info("------------------------------");
            QuasarLogger.Warn("Quasar Start");
            QuasarLogger.Info("------------------------------");

        }

        #endregion
    }
}
