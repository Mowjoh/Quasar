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
using Quasar.Controls.InternalModTypes.ViewModels;
using Quasar.Controls.InternalModTypes.Views;
using Quasar.Controls.ModManagement.ViewModels;
using Quasar.Controls.ModManagement.Views;
using Quasar.Controls.Settings.Model;
using Quasar.Controls.Settings.View;
using Quasar.Controls.Settings.Workspaces.View;
using Quasar.Controls.Settings.Workspaces.ViewModels;
using Quasar.Internal;
using Quasar.Internal.Tools;
using Quasar.Quasar_Sys;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    class MainUIViewModel : ObservableObject
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
        private InternalModTypeView _InternalModTypeView { get; set; }
        private InternalModTypeViewModel _IMTV { get; set; }
        private SettingsView _SettingsView { get; set; }
        private WorkspaceView _WorkspaceView { get; set; }
        private WorkspaceViewModel _WVM { get; set; }

        private bool _BeginGameChoice { get; set; }
        private bool _StopGameChoice { get; set; }

        private bool _CreatorMode { get; set; }
        private bool _AdvancedMode { get; set; }
        #endregion

        #region Working Data
        private ObservableCollection<LibraryMod> _Mods { get; set; }
        private ObservableCollection<ContentMapping> _ContentMappings { get; set; }
        private ObservableCollection<Association> _Associations { get; set; }
        private ObservableCollection<Workspace> _Workspaces { get; set; }
        private Workspace _ActiveWorkspace { get; set; }
        private ModListItem _SelectedModListItem { get; set; }

        private Game _SelectedGame { get; set; }

        #endregion

        #region References
        private ObservableCollection<InternalModType> _InternalModTypes { get; set; }
        private ObservableCollection<GameData> _GameDatas { get; set; }
        private ObservableCollection<Game> _Games { get; set; }
        private ObservableCollection<ModLoader> _ModLoaders { get; set; }
        #endregion

        #region Commands
        private ICommand _StartGameSelectionCommand { get; set; }
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

        public InternalModTypeView InternalModTypeView
        {
            get => _InternalModTypeView;
            set
            {
                if (_InternalModTypeView == value)
                    return;

                _InternalModTypeView = value;
                OnPropertyChanged("InternalModTypeView");
            }
        }
        public InternalModTypeViewModel IMTV
        {
            get => _IMTV;
            set
            {
                if (_IMTV == value)
                    return;

                _IMTV = value;
                OnPropertyChanged("IMTV");
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
        #endregion

        #region Working Data
        /// <summary>
        /// List of all Library Mods
        /// </summary>
        public ObservableCollection<LibraryMod> Mods
        {
            get => _Mods;
            set
            {
                if (_Mods == value)
                    return;

                _Mods = value;
                OnPropertyChanged("Mods");
            }
        }
        /// <summary>
        /// List of all content mappings
        /// </summary>
        public ObservableCollection<ContentMapping> ContentMappings
        {
            get => _ContentMappings;
            set
            {
                if (_ContentMappings == value)
                    return;

                _ContentMappings = value;
                OnPropertyChanged("ContentMappings");
            }
        }
        /// <summary>
        /// List of the active workspace's current Associations
        /// </summary>
        public ObservableCollection<Association> Associations
        {
            get => _Associations;
            set
            {
                if (_Associations == value)
                    return;

                _Associations = value;
                OnPropertyChanged("Associations");
            }
        }
        /// <summary>
        /// List of workspaces
        /// </summary>
        public ObservableCollection<Workspace> Workspaces
        {
            get => _Workspaces;
            set
            {
                if (_Workspaces == value)
                    return;

                _Workspaces = value;
                OnPropertyChanged("Workspaces");
            }
        }
        /// <summary>
        /// Represents the Active Workspace
        /// </summary>
        public Workspace ActiveWorkspace
        {
            get => _ActiveWorkspace;
            set
            {
                if (_ActiveWorkspace == value)
                    return;

                _ActiveWorkspace = value;
                OnPropertyChanged("ActiveWorkspace");
            }
        }
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
                    CVM = new ContentViewModel(ContentMappings, _SelectedModListItem.ModListItemViewModel.LibraryMod, InternalModTypes, GameDatas);
                    ContentView.DataContext = CVM;
                }
                OnPropertyChanged("SelectedModListItem");
            }
        }
        /// <summary>
        /// Mutex that serves to know if a Quasar instance is already running
        /// </summary>
        public Mutex serverMutex;
        public ILog log { get; set; }

        public Game SelectedGame
        {
            get => _SelectedGame;
            set
            {
                if (_SelectedGame == value)
                    return;

                _SelectedGame = value;
                OnPropertyChanged("SelectedGame");
            }
        }
        #endregion

        #region References

        /// <summary>
        /// List of all Internal Mod Types
        /// </summary>
        public ObservableCollection<InternalModType> InternalModTypes
        {
            get => _InternalModTypes;
            set
            {
                if (_InternalModTypes == value)
                    return;

                _InternalModTypes = value;
                OnPropertyChanged("InternalModTypes");
            }
        }
        /// <summary>
        /// List of all Game Data
        /// </summary>
        public ObservableCollection<GameData> GameDatas
        {
            get => _GameDatas;
            set
            {
                if (_GameDatas == value)
                    return;

                _GameDatas = value;
                OnPropertyChanged("GameDatas");
            }
        }
        /// <summary>
        /// List of all Games and their API Categories
        /// </summary>
        public ObservableCollection<Game> Games
        {
            get => _Games;
            set
            {
                if (_Games == value)
                    return;

                _Games = value;
                OnPropertyChanged("Games");
            }
        }
        /// <summary>
        /// List of Mod Loaders
        /// </summary>
        public ObservableCollection<ModLoader> ModLoaders
        {
            get => _ModLoaders;
            set
            {
                if (_ModLoaders == value)
                    return;

                _ModLoaders = value;
                OnPropertyChanged("ModLoaders");
            }
        }

        #endregion

        #region Commands
        public ICommand StartGameSelectionCommand
        {
            get
            {
                if (_StartGameSelectionCommand == null)
                {
                    _StartGameSelectionCommand = new RelayCommand(param => StartGameSelection());
                }
                return _StartGameSelectionCommand;
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
                new Updater();

                log.Info("Loading References");
                LoadStuff();

                log.Info("Loading Views");
                SetupViews();

                AdvancedMode = false;
                CreatorMode = false;

                EventSystem.Subscribe<ModListItem>(SetModListItem);
            }
            catch(Exception e)
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

        public void LoadStuff()
        {
            //Working Data
            GetMods();
            GetContentMappings();
            GetWorkspaces();
            GetAssociations();

            //References
            GetInternalModTypes();
            GetGameData();
            GetGames();
            GetModLoaders();
        }
        public void SetupViews()
        {
            EventSystem.Subscribe<SettingItem>(SettingChanged);

            TabItems = new ObservableCollection<TabItem>();

            ModsView = new ModsView();
            MVM = new ModsViewModel(Mods, Games, ContentMappings, Workspaces,ActiveWorkspace, InternalModTypes, GameDatas, log);
            ModsView.DataContext = MVM;
            TabItems.Add(new TabItem() { Content = ModsView, Header = "Overview", Foreground = new SolidColorBrush() { Color = Colors.White } });

            ContentView = new ContentView();
            TabItems.Add(new TabItem() { Content = ContentView, Header = "Contents", Foreground = new SolidColorBrush() { Color = Colors.White } });

            
            AVM = new AssociationViewModel(GameDatas, InternalModTypes, Workspaces,ActiveWorkspace, ContentMappings);
            AVM.Log = log;
            AssociationView = new AssociationView() { AssociationViewModel = AVM };
            AssociationView.DataContext = AVM;
            TabItems.Add(new TabItem() { Content = AssociationView, Header = "Management", Foreground = new SolidColorBrush() { Color = Colors.White } });

            BuildView = new BuildView();
            BVM = new BuildViewModel(ModLoaders, Workspaces, ActiveWorkspace,Mods,ContentMappings,InternalModTypes,GameDatas,Games);
            BVM.Log = log;
            BuildView.DataContext = BVM;
            TabItems.Add(new TabItem() { Content = BuildView, Header = "File Transfer", Foreground = new SolidColorBrush() { Color = Colors.White } });

            InternalModTypeView = new InternalModTypeView();
            IMTV = new InternalModTypeViewModel(InternalModTypes, ModLoaders, GameDatas, Games);
            InternalModTypeView.DataContext = IMTV;
            TabItems.Add(new TabItem() { Content = InternalModTypeView, Header = "Types", Foreground = new SolidColorBrush() { Color = Colors.White }, Visibility = Properties.Settings.Default.EnableAdvanced? Visibility.Visible : Visibility.Collapsed });

            SettingsView = new SettingsView();
            TabItems.Add(new TabItem() { Content = SettingsView, Header = "Settings", Foreground = new SolidColorBrush() { Color = Colors.White } });

            WorkspaceView = new WorkspaceView();
            WVM = new WorkspaceViewModel(Workspaces, ActiveWorkspace);
            WorkspaceView.DataContext = WVM;
            TabItems.Add(new TabItem() { Content = WorkspaceView, Header = "Workspaces", Foreground = new SolidColorBrush() { Color = Colors.White }, Visibility = Properties.Settings.Default.EnableWorkspaces ? Visibility.Visible : Visibility.Collapsed });

            SettingsView.start();
        }
        public void SetupClientOrServer()
        {
            serverMutex = Checker.Instances(serverMutex, log);
        }

        //Data Parsing
        /// <summary>
        /// Retreives the list of all mods in the library
        /// </summary>
        public void GetMods()
        {
            Mods = new ObservableCollection<LibraryMod>();

            List<LibraryMod> ModList = Library.GetLibraryModList();
            foreach(LibraryMod lm in ModList)
            {
                Mods.Add(lm);
            }
        }
        public void GetContentMappings()
        {
            ContentMappings = new ObservableCollection<ContentMapping>();

            List<ContentMapping> ContentMappingList = ContentXML.GetContentMappings();
            foreach(ContentMapping cm in ContentMappingList)
            {
                ContentMappings.Add(cm);
            }
        }
        public void GetAssociations()
        {
            Associations = new ObservableCollection<Association>();

            foreach (Association ass in ActiveWorkspace.Associations)
            {
                Associations.Add(ass);
            }
        }
        public void GetWorkspaces()
        {
            Workspaces = new ObservableCollection<Workspace>();

            List<Workspace> List = WorkspaceXML.GetWorkspaces();
            foreach (Workspace w in List)
            {
                Workspaces.Add(w);
            }
            ActiveWorkspace = Workspaces[0];
        }

        //Reference Data Parsing
        public void GetInternalModTypes()
        {
            InternalModTypes = new ObservableCollection<InternalModType>();

            List<InternalModType> InternalModTypeList = XML.GetInternalModTypes();
            foreach (InternalModType imt in InternalModTypeList)
            {
                InternalModTypes.Add(imt);
            }
        }
        public void GetGameData()
        {
            GameDatas = new ObservableCollection<GameData>();

            List<GameData> GameDataList = XML.GetGameData();
            foreach (GameData gd in GameDataList)
            {
                GameDatas.Add(gd);
            }
        }
        public void GetGames()
        {
            Games = new ObservableCollection<Game>();
            List<Game> GameList = XML.GetGames();

            foreach(Game g in GameList)
            {
                Games.Add(g);
            }
        }
        /// <summary>
        /// Gets the workspaces from the XML File
        /// </summary>
        public void GetModLoaders()
        {
            ModLoaders = new ObservableCollection<ModLoader>();

            List<ModLoader> Loaders = XML.GetModLoaders();
            foreach (ModLoader ml in Loaders)
            {
                ModLoaders.Add(ml);
            }
        }

        //Events
        public void SetModListItem(ModListItem _SelectedModListItem)
        {
            if(_SelectedModListItem.ModListItemViewModel.ActionRequested == "ShowContents")
            {
                SelectedTabItem = TabItems[1];
                SelectedModListItem = _SelectedModListItem;
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
                TabItems[4].Visibility = Setting.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }
            if (Setting.SettingName == "EnableWorkspaces")
            {
                TabItems[6].Visibility = Setting.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void LogBuilderMessage()
        {

        }
        #endregion


    }
}
