using log4net;
using log4net.Appender;
using Quasar.Controls;
using Quasar.Controls.Assignation.ViewModels;
using Quasar.Controls.Assignation.Views;
using Quasar.Controls.Build.ViewModels;
using Quasar.Controls.Build.Views;
using Quasar.Controls.Common.Models;
using Quasar.Controls.Content.ViewModels;
using Quasar.Controls.Content.Views;
using Quasar.Controls.ModManagement.ViewModels;
using Quasar.Controls.ModManagement.Views;
using Quasar.Controls.Settings.Model;
using Quasar.Controls.Settings.View;
using Quasar.Controls.Settings.Workspaces.View;
using Quasar.Controls.Settings.Workspaces.ViewModels;
using Quasar.Data.Converter;
using Quasar.Data.V2;
using Quasar.FileSystem;
using Quasar.Helpers.Json;
using Quasar.Helpers.ModScanning;
using Quasar.Helpers.Quasar_Management;
using Quasar.Helpers.XML;
using Quasar.Internal;
using Quasar.Internal.Tools;
using Quasar.NamedPipes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Quasar
{
    public class MainUIViewModel : ObservableObject
    {
        #region Fields

        #region Views
        private ObservableCollection<TabItem> _TabItems { get; set; }
        private TabItem _SelectedTabItem { get; set; }
        private ModsView _ModsView { get; set; }
        private ModsViewModel _MVM { get; set; }
        private ContentView _ContentView { get; set; }
        private ContentViewModel _CVM { get; set; }
        private AssociationView _AssignationView { get; set; }
        private AssociationViewModel _AVM { get; set; }
        private BuildView _BuildView { get; set; }
        private BuildViewModel _BVM { get; set; }
        private SettingsView _SettingsView { get; set; }
        private WorkspaceView _WorkspaceView { get; set; }
        private WorkspaceViewModel _WVM { get; set; }

        private bool _BeginGameChoice { get; set; }
        private bool _StopGameChoice { get; set; }

        private bool _CreatorMode { get; set; }
        private bool _AdvancedMode { get; set; }
        private bool _Updating { get; set; } = false;
        private bool _UpdateFinished { get; set; } = false;
        #endregion

        #region Working Data
        private ModListItem _SelectedModListItem { get; set; }
        #endregion

        #region Commands
        private ICommand _UpdateOKCommand { get; set; }
        #endregion

        #endregion

        #region Properties

        #region Views
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
                if (_SelectedTabItem == value)
                    return;

                if ((string)value.Header == "Overview")
                {
                    MVM.ReloadAllStats();
                }
                if ((string)value.Header == "Management")
                {
                    AVM.ShowSlots();
                }
                _SelectedTabItem = value;
                OnPropertyChanged("SelectedTabItem");
            }
        }
        public ModsView ModsView
        {
            get => _ModsView;
            set
            {
                if (_ModsView == value)
                    return;

                _ModsView = value;
                OnPropertyChanged("ModsView");
            }
        }
        public ModsViewModel MVM
        {
            get => _MVM;
            set
            {
                if (_MVM == value)
                    return;

                _MVM = value;
                OnPropertyChanged("MVM");
            }
        }

        public ContentView ContentView
        {
            get => _ContentView;
            set
            {
                if (_ContentView == value)
                    return;

                _ContentView = value;
                OnPropertyChanged("ContentView");
            }
        }
        public ContentViewModel CVM
        {
            get => _CVM;
            set
            {
                if (_CVM == value)
                    return;

                _CVM = value;
                OnPropertyChanged("CVM");
            }
        }

        public AssociationView AssociationView
        {
            get => _AssignationView;
            set
            {
                if (_AssignationView == value)
                    return;

                _AssignationView = value;
                OnPropertyChanged("AssociationView");
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

        public BuildView BuildView
        {
            get => _BuildView;
            set
            {
                if (_BuildView == value)
                    return;

                _BuildView = value;
                OnPropertyChanged("BuildView");
            }
        }
        public BuildViewModel BVM
        {
            get => _BVM;
            set
            {
                if (_BVM == value)
                    return;

                _BVM = value;
                OnPropertyChanged("BVM");
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

        
        public WorkspaceView WorkspaceView
        {
            get => _WorkspaceView;
            set
            {
                if (_WorkspaceView == value)
                    return;

                _WorkspaceView = value;
                OnPropertyChanged("WorkspaceView");
            }
        }
        public WorkspaceViewModel WVM
        {
            get => _WVM;
            set
            {
                if (_WVM == value)
                    return;

                _WVM = value;
                OnPropertyChanged("WVM");
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
        #endregion

        #region Resource Data
        public ObservableCollection<Game> Games { get; set; }
        public Game CurrentGame { get; set; }
        public ObservableCollection<QuasarModType> QuasarModTypes { get; set; }
        public ObservableCollection<ModLoader> ModLoaders { get; set; }
        #endregion

        #region User Data
        public Workspace ActiveWorkspace { get; set; }
        public ObservableCollection<Workspace> Workspaces { get; set; }
        public ObservableCollection<LibraryItem> Library { get; set; }
        public ObservableCollection<ContentItem> ContentItems { get; set; }
        #endregion

        #region Working Data
        public ModListItem SelectedModListItem
        {
            get => _SelectedModListItem;
            set
            {
                if (_SelectedModListItem == value)
                    return;

                _SelectedModListItem = value;
                if(value != null)
                {
                    //CVM = new ContentViewModel(ContentMappings, _SelectedModListItem.ModListItemViewModel.LibraryItem, InternalModTypes, GameDatas);
                    //ContentView.DataContext = CVM;
                }
                OnPropertyChanged("SelectedModListItem");
            }
        }
        /// <summary>
        /// Mutex that serves to know if a Quasar instance is already running
        /// </summary>
        public Mutex serverMutex;
        public ILog log { get; set; }
        #endregion

        #region Commands
        public ICommand UpdateOKCommand
        {
            get
            {
                if (_UpdateOKCommand == null)
                {
                    _UpdateOKCommand = new RelayCommand(param => UpdateOK());
                }
                return _UpdateOKCommand;
            }
        }
        #endregion

        #endregion

        public MainUIViewModel()
        {
            
            SetupLogger();

            try
            {
                log.Info("Update Process Started");
                SetupClientOrServer();

                log.Info("Update Process Started");
                Updater.CheckExecuteUpdate();

                bool RemakeFiles = false;

                if (RemakeFiles)
                {
                    /*
                    //User Converts
                    V1toV2Converter.ProcessWorkspace(Workspaces);
                    V1toV2Converter.ProcessLibrary(Mods);
                    V1toV2Converter.ProcessContent(ContentMappings);

                    //Dev Converts
                    V1toV2Converter.ProcessGameFile(Games, GameDatas);
                    V1toV2Converter.ProcessQuasarModTypes(InternalModTypes);
                    V1toV2Converter.ProcessModLoaders(ModLoaders);*/
                }

                log.Info("Loading References");
                LoadNewStuff();

                log.Info("Loading Views");
                SetupViews();

                AdvancedMode = false;
                CreatorMode = false;

                EventSystem.Subscribe<ModListItem>(SetModListItem);

            }
            catch (Exception e)
            {
                log.Info(e.Message);
                log.Info(e.StackTrace);
            }
        }

        #region Actions
        //Startup Actions
        public void SetupLogger()
        {
            log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            if (Properties.Settings.Default.EnableAdvanced)
            {
                appender.Threshold = log4net.Core.Level.Debug;
            }
            else
            {
                appender.Threshold = log4net.Core.Level.Info;
            }
            
            appender.ActivateOptions();

            log.Info("------------------------------");
            log.Warn("Quasar Start");
            log.Info("------------------------------");
        }

        public void ChangeLogger(bool debug)
        {
            FileAppender appender = (FileAppender)log.Logger.Repository.GetAppenders()[0];
            appender.Threshold = log4net.Core.Level.Debug;
            if (debug)
            {
                appender.Threshold = log4net.Core.Level.Debug;
            }
            else
            {
                appender.Threshold = log4net.Core.Level.Info;
            }
            
        }

        public void LoadNewStuff()
        {
            //Loading User Data
            Workspaces = JSonHelper.GetWorkspaces();
            if (Workspaces.Count == 0)
            {
                InstallManager.CreateBaseWorkspace();
                Workspaces = JSonHelper.GetWorkspaces();
            }
            ActiveWorkspace = Workspaces[0];


            Library = JSonHelper.GetLibrary();
            ContentItems = JSonHelper.GetContentItems();

            //Loading Resource Data
            Games = JSonHelper.GetGames();
            CurrentGame = Games[0];
            QuasarModTypes = JSonHelper.GetQuasarModTypes();
            ModLoaders = JSonHelper.GetModLoaders();

        }
        public void SetupViews()
        {
            
            EventSystem.Subscribe<SettingItem>(SettingChanged);

            TabItems = new ObservableCollection<TabItem>();

            ModsView = new ModsView();
            MVM = new ModsViewModel(this, log);
            ModsView.DataContext = MVM;
            TabItems.Add(new TabItem() { Content = ModsView, Header = Properties.Resources.MainUI_OverviewTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });

            ContentView = new ContentView();
            CVM = new ContentViewModel(this);
            ContentView.DataContext = CVM;
            TabItems.Add(new TabItem() { Content = ContentView, Header = Properties.Resources.MainUI_ContentsTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });


            AVM = new AssociationViewModel(this);
            AVM.Log = log;
            AssociationView = new AssociationView() { AssociationViewModel = AVM };
            AssociationView.DataContext = AVM;
            TabItems.Add(new TabItem() { Content = AssociationView, Header = Properties.Resources.MainUI_ManagementTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });

            BuildView = new BuildView();
            BVM = new BuildViewModel(this);
            BVM.Log = log;
            BuildView.DataContext = BVM;
            TabItems.Add(new TabItem() { Content = BuildView, Header = Properties.Resources.MainUI_FileTransferTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });

            SettingsView = new SettingsView();
            TabItems.Add(new TabItem() { Content = SettingsView, Header = Properties.Resources.MainUI_SettingsTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White } });

            WorkspaceView = new WorkspaceView();
            WVM = new WorkspaceViewModel(this, log);
            WorkspaceView.DataContext = WVM;
            TabItems.Add(new TabItem() { Content = WorkspaceView, Header = Properties.Resources.MainUI_WorkspacesTabHeader, Foreground = new SolidColorBrush() { Color = Colors.White }, Visibility = Properties.Settings.Default.EnableWorkspaces ? Visibility.Visible : Visibility.Collapsed });

            

            SettingsView.start();
        }
        public void SetupClientOrServer()
        {
            serverMutex = PipeHelper.CheckExecuteInstance(serverMutex, log);
        }

        public async void ShowUpdateAndLaunch()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Updating = Updater.NeedsScanning();
            });

            if (Updater.NeedsScanning())
            {
                await Task.Run(() => {
                    Scannerino.ScanAllMods(this);
                });
            }

            JSonHelper.SaveWorkspaces(Workspaces);
            JSonHelper.SaveContentItems(ContentItems);

            Application.Current.Dispatcher.Invoke((Action)delegate {
                UpdateFinished = true;
            });
        }

        public void UpdateOK()
        { 

            MVM.CollectionViewSource.View.Refresh();
            MVM.ReloadAllStats();

            Updating = false;
        }

        //Events
        public void SetModListItem(ModListItem _SelectedModListItem)
        {
            if(_SelectedModListItem.ModListItemViewModel.ActionRequested == "ShowContents")
            {
                SelectedTabItem = TabItems[1];
                SelectedModListItem = _SelectedModListItem;
                CVM.GetRefreshed("RefreshContents");
            }
        }

        public void StartGameSelection()
        {
            BeginGameChoice = true;
            StopGameChoice = false;
        }

        public void SettingChanged(SettingItem Setting)
        {
            if(Setting.SettingName == "EnableCreator")
            {
                CreatorMode = Setting.IsChecked;
            }
            if (Setting.SettingName == "EnableAdvanced")
            {
                AdvancedMode = Setting.IsChecked;
                ChangeLogger(Setting.IsChecked);
            }
            if (Setting.SettingName == "EnableWorkspaces")
            {
                TabItems[5].Visibility = Setting.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion


    }
}
