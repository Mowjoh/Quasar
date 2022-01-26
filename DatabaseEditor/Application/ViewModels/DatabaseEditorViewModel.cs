using DataModels.Common;
using DataModels.Resource;
using DataModels.User;
using Quasar.Common.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Workshop.FileManagement;
using Workshop.Scanners;

namespace DatabaseEditor.ViewModels
{
    public class DatabaseEditorViewModel : ObservableObject
    {
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";
        public static string InstallDirectory = @"C:\Program Files (x86)\Quasar";

        
        
        #region Commands
        //Game Editor Tab
        private ICommand _AddFamilyCommand { get; set; }
        public ICommand AddFamilyCommand
        {
            get
            {
                if (_AddFamilyCommand == null)
                {
                    _AddFamilyCommand = new RelayCommand(param => AddFamily());
                }
                return _AddFamilyCommand;
            }
        }
        private ICommand _AddElementCommand { get; set; }
        public ICommand AddElementCommand
        {
            get
            {
                if (_AddElementCommand == null)
                {
                    _AddElementCommand = new RelayCommand(param => AddElement());
                }
                return _AddElementCommand;
            }
        }
        private ICommand _SaveGameCommand { get; set; }
        public ICommand SaveGameCommand
        {
            get
            {
                if (_SaveGameCommand == null)
                {
                    _SaveGameCommand = new RelayCommand(param => SaveGameFile());
                }
                return _SaveGameCommand;
            }
        }

        //Scanner Tab
        private ICommand _PickPathCommand { get; set; }
        public ICommand PickPathCommand
        {
            get
            {
                if (_PickPathCommand == null)
                {
                    _PickPathCommand = new RelayCommand(param => PickPath());
                }
                return _PickPathCommand;
            }
        }
        private ICommand _ScanModCommand { get; set; }
        public ICommand ScanModCommand
        {
            get
            {
                if (_ScanModCommand == null)
                {
                    _ScanModCommand = new RelayCommand(param => ScanMod());
                }
                return _ScanModCommand;
            }
        }
        private ICommand _ScanPathCommand { get; set; }
        public ICommand ScanPathCommand
        {
            get
            {
                if (_ScanPathCommand == null)
                {
                    _ScanPathCommand = new RelayCommand(param => ScanPath());
                }
                return _ScanPathCommand;
            }
        }
        //Paths Tab
        private ICommand _SavePathCommand { get; set; }
        public ICommand SavePathCommand
        {
            get
            {
                if (_SavePathCommand == null)
                {
                    _SavePathCommand = new RelayCommand(param => SavePath());
                }
                return _SavePathCommand;
            }
        }
        #endregion
        public DatabaseEditorViewModel() {

            //Games = ResourceManager.GetGames(Properties.);
            //Library = UserDataManager.GetLibrary(AppDataPath);
            //ContentItems = UserDataManager.GetContentItems(AppDataPath);
            //Workspaces = UserDataManager.GetWorkspaces(AppDataPath);
            //ModLoaders = ResourceManager.GetModLoaders();
            //QuasarModTypes = ResourceManager.GetQuasarModTypes();
            //API = ResourceManager.GetGamebananaAPI();

            CollectionViewSource = new CollectionViewSource();

            CollectionViewSource.Source = TestResults;
            CollectionViewSource.Filter += CollectionViewSource_Filter;
            RepoPath = Properties.Settings.Default.RepoPath;

            IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            ScanTestResult result = e.Item as ScanTestResult;
            
            if (UnrecognizedFilter && result.ScanFile.Scanned == false || !UnrecognizedFilter)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        #region Private Members
        //Games
        private ObservableCollection<Game> _Games { get; set; }
        private Game _SelectedGame { get; set; }
        private GameElementFamily _SelectedGameElementFamily { get; set; }
        private GamebananaAPI _API { get; set; }
        private GamebananaRootCategory _SelectedGamebananaRootCategory { get; set; }

        //Library
        private ObservableCollection<LibraryItem> _Library { get; set; }
        private LibraryItem _SelectedLibraryItem { get; set; }

        //Content
        private ObservableCollection<ContentItem> _ContentItems { get; set; }
        private ContentItem _SelectedContentItem { get; set; }

        //Workspaces
        private ObservableCollection<Workspace> _Workspaces { get; set; }
        private Workspace _SelectedWorkspace { get; set; }

        //Mod Loaders
        private ObservableCollection<ModLoader> _ModLoaders { get; set; }

        //Quasar Mod Types
        private ObservableCollection<QuasarModType> _QuasarModTypes { get; set; }
        private QuasarModType _SelectedQuasarModType { get; set; }
        private QuasarModTypeFileDefinition _SelectedQuasarModTypeFileDefinition { get; set; }
        private LibraryItem _SelectedTestLibraryItem { get; set; }
        private ModLoader _SelectedTestModLoader { get; set; }
        private ObservableCollection<ScanTestResult> _TestResults { get; set; }

        private double _ProgressValue { get; set; }
        private string _ProgressString { get; set; }

        //Paths
        private CollectionViewSource _CollectionViewSource { get; set; }
        private string _RepoPath { get; set; }
        private string _ScanPathText { get; set; }
        private bool _UnrecognizedFilter { get; set; }

        //Admin status
        private bool _IsAdmin { get; set; }
        #endregion

        #region View
        //Games
        public ObservableCollection<Game> Games
        {
            get => _Games;
            set
            {
                _Games = value;
                OnPropertyChanged("Games");
            }
        }
        public Game SelectedGame
        {
            get => _SelectedGame;
            set
            {
                _SelectedGame = value;
                OnPropertyChanged("SelectedGame");
            }
        }
        public GameElementFamily SelectedGameElementFamily
        {
            get => _SelectedGameElementFamily;
            set
            {
                _SelectedGameElementFamily = value;
                OnPropertyChanged("SelectedGameElementFamily");
            }
        }
        public GamebananaAPI API
        {
            get => _API;
            set
            {
                _API = value;
                OnPropertyChanged("API");
            }
        }
        public GamebananaRootCategory SelectedGamebananaRootCategory
        {
            get => _SelectedGamebananaRootCategory;
            set
            {
                _SelectedGamebananaRootCategory = value;
                OnPropertyChanged("SelectedGamebananaRootCategory");
            }
        }



        //Library
        public ObservableCollection<LibraryItem> Library
        {
            get => _Library;
            set
            {
                _Library = value;
                OnPropertyChanged("Library");
            }
        }
        public LibraryItem SelectedLibraryItem
        {
            get => _SelectedLibraryItem;
            set
            {
                _SelectedLibraryItem = value;
                AssociatedRootCategory = API.Games[0].RootCategories.Single(c => c.Guid == value.GBItem.RootCategoryGuid);
                AssociatedSubCategory = AssociatedRootCategory.SubCategories.Single(c => c.Guid == value.GBItem.SubCategoryGuid);

                OnPropertyChanged("SelectedLibraryItem");
                OnPropertyChanged("AssociatedRootCategory");
                OnPropertyChanged("AssociatedSubCategory");
            }
        }
        public GamebananaRootCategory AssociatedRootCategory { get; set; }
        public GamebananaSubCategory AssociatedSubCategory { get; set; }

        //Content
        public ObservableCollection<ContentItem> ContentItems
        {
            get => _ContentItems;
            set
            {
                _ContentItems = value;
                OnPropertyChanged("ContentItems");
            }
        }
        public ContentItem SelectedContentItem
        {
            get => _SelectedContentItem;
            set
            {
                _SelectedContentItem = value;
                OnPropertyChanged("SelectedContentItem");
            }
        }

        //Workspaces
        public ObservableCollection<Workspace> Workspaces
        {
            get => _Workspaces;
            set
            {
                _Workspaces = value;
                OnPropertyChanged("Workspaces");
            }
        }
        public Workspace SelectedWorkspace
        {
            get => _SelectedWorkspace;
            set
            {
                _SelectedWorkspace = value;
                OnPropertyChanged("SelectedWorkspace");
            }
        }
        //Mod Loaders
        public ObservableCollection<ModLoader> ModLoaders
        {
            get => _ModLoaders;
            set
            {
                _ModLoaders = value;
                OnPropertyChanged("ModLoaders");
            }
        }

        //Quasar Mod Types
        public ObservableCollection<QuasarModType> QuasarModTypes
        {
            get => _QuasarModTypes;
            set
            {
                _QuasarModTypes = value;
                OnPropertyChanged("QuasarModTypes");
            }
        }
        public QuasarModType SelectedQuasarModType
        {
            get => _SelectedQuasarModType;
            set
            {
                _SelectedQuasarModType = value;
                if (value != null)
                {
                    if (_SelectedQuasarModType.ID == 0)
                    {
                        _SelectedQuasarModType.ID = QuasarModTypes.Count;
                        OnPropertyChanged("QuasarModTypes");
                    }
                    if (_SelectedQuasarModType.QuasarModTypeFileDefinitions == null)
                    {
                        _SelectedQuasarModType.QuasarModTypeFileDefinitions = new ObservableCollection<QuasarModTypeFileDefinition>();
                        _SelectedQuasarModType.QuasarModTypeFileDefinitions.Add(new QuasarModTypeFileDefinition()
                        {
                            ID = 0,
                            SearchFileName = "Filename",
                            SearchPath = "Path",
                            QuasarModTypeBuilderDefinitions = new ObservableCollection<QuasarModTypeBuilderDefinition>(),
                        });
                    }


                }


                OnPropertyChanged("SelectedQuasarModType");
            }
        }
        public QuasarModTypeFileDefinition SelectedQuasarModTypeFileDefinition
        {
            get => _SelectedQuasarModTypeFileDefinition;
            set
            {
                _SelectedQuasarModTypeFileDefinition = value;
                if (value != null)
                {
                    bool processed = false;
                    if (_SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions == null)
                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions = new ObservableCollection<QuasarModTypeBuilderDefinition>();

                    while (_SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions.Count < ModLoaders.Count)
                    {
                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions.Add(new QuasarModTypeBuilderDefinition()
                        {
                            ModLoaderID = _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions.Count + 1,
                            OutputFileName = "TBD",
                            OutputPath = "TBD"
                        });
                        processed = true;
                    }

                    if (processed)
                    {
                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[0].OutputPath = _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[1].OutputPath;

                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[2].OutputFileName = _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[0].OutputFileName;
                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[2].OutputPath = "{ModNameSlot}/" + _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[1].OutputPath;
                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[3].OutputFileName = _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[0].OutputFileName;
                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[3].OutputPath = "{ModName}/" + _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[1].OutputPath;

                        _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[1].OutputPath = _SelectedQuasarModTypeFileDefinition.QuasarModTypeBuilderDefinitions[3].OutputPath;

                    }
                }


                OnPropertyChanged("SelectedQuasarModTypeFileDefinition");
            }
        }
        public LibraryItem SelectedTestLibraryItem
        {
            get => _SelectedTestLibraryItem;
            set
            {
                _SelectedTestLibraryItem = value;
                OnPropertyChanged("SelectedTestLibraryItem");
            }
        }
        public ModLoader SelectedTestModLoader
        {
            get => _SelectedTestModLoader;
            set
            {
                _SelectedTestModLoader = value;
                OnPropertyChanged("SelectedTestModLoader");
            }
        }
        public ObservableCollection<ScanTestResult> TestResults
        {
            get => _TestResults;
            set
            {
                _TestResults = value;
                OnPropertyChanged("TestResults");
            }
        }

        public CollectionViewSource CVS { get; set; }
        public double ProgressValue
        {
            get => _ProgressValue;
            set
            {
                _ProgressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }
        public string ProgressString
        {
            get => _ProgressString;
            set
            {
                _ProgressString = value;
                OnPropertyChanged("ProgressString");
            }
        }

        //Scanning
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
        public string RepoPath
        {
            get => _RepoPath;
            set
            {
                _RepoPath = value;
                OnPropertyChanged("RepoPath");
            }
        }
        public string ScanPathText
        {
            get => _ScanPathText;
            set
            {
                _ScanPathText = value;
                OnPropertyChanged("ScanPathText");
            }
        }
        public bool UnrecognizedFilter
        {
            get => _UnrecognizedFilter;
            set
            {
                _UnrecognizedFilter = value;
                CollectionViewSource.View.Refresh();
                OnPropertyChanged("UnrecognizedFilter");
            }
        }

        public bool IsAdmin
        {
            get => _IsAdmin;
            set
            {
                _IsAdmin = value;
                OnPropertyChanged("IsAdmin");
            }
        }
        #endregion

        #region Actions
        //Game Editor Tab
        public void AddFamily()
        {
            if (SelectedGame != null)
            {
                int provisoryID = Games[0].GameElementFamilies.Count;
                while (Games[0].GameElementFamilies.Any(f => f.ID == provisoryID))
                {
                    provisoryID++;
                }

                GameElementFamily Family = new GameElementFamily()
                {
                    ID = provisoryID
                };
                Games[0].GameElementFamilies.Add(Family);
            }

        }
        public void AddElement()
        {
            if (SelectedGameElementFamily != null)
            {
                int provisoryID = SelectedGameElementFamily.GameElements.Count;
                while (SelectedGameElementFamily.GameElements.Any(f => f.ID == provisoryID))
                {
                    provisoryID++;
                }

                GameElement Element = new GameElement()
                {
                    ID = provisoryID
                };

                SelectedGameElementFamily.GameElements.Add(Element);
            }

        }
        public void SaveGameFile()
        {
            if (RepoPath != "")
            {
                ResourceManager.SaveGamesFile(Games, RepoPath + @"\Resources\");
                if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    ResourceManager.SaveGamesFile(Games, InstallDirectory + @"\Resources\");
                }

            }
        }

        //Scanner Tab
        public void PickPath()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult results = fbd.ShowDialog();

                if (results == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    ScanPathText = fbd.SelectedPath;
                }
            }
        }
        public void ScanMod()
        {
            string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Quasar\";
            string ModPath = String.Format(@"{0}\Library\Mods\{1}\", DocumentsPath, SelectedTestLibraryItem.Guid);
            ObservableCollection<ScanFile> ScanResults = FileScanner.GetScanFiles(ModPath);
            ScanResults = FileScanner.FilterIgnoredFiles(ScanResults);
            ScanResults = FileScanner.MatchScanFiles(ScanResults, QuasarModTypes, Games[0], ModPath);
            ProcessScan(ScanResults);
        }
        public void ScanPath()
        {
            ObservableCollection<ScanFile> ScanResults = FileScanner.GetScanFiles(ScanPathText);
            ScanResults = FileScanner.FilterIgnoredFiles(ScanResults);
            ScanResults = FileScanner.MatchScanFiles(ScanResults, QuasarModTypes, Games[0], ScanPathText);
            ProcessScan(ScanResults);
            
        }

        public void ProcessScan(ObservableCollection<ScanFile> ScanResults)
        {
            TestResults = new ObservableCollection<ScanTestResult>();
            foreach(ScanFile Scan in ScanResults)
            {
                TestResults.Add(new ScanTestResult()
                {
                    ScanFile = Scan
                });
            }
            CollectionViewSource.Source = TestResults;
            CollectionViewSource.View.Refresh();
        }

        //Paths Tab
        public void SavePath()
        {
            Properties.Settings.Default.RepoPath = RepoPath;
            Properties.Settings.Default.Save();
        }

        
        #endregion

    }

    
}
