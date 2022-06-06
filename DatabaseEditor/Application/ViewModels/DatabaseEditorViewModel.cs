using DataModels.Common;
using DataModels.Resource;
using DataModels.User;
using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using Workshop.FileManagement;
using Workshop.Scanners;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace DatabaseEditor.ViewModels
{
    public class DatabaseEditorViewModel : ObservableObject
    {
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";
        public static string InstallDirectory = @"C:\Program Files (x86)\Quasar";

        #region Commands

        #region Private
        //Game Editor Tab
        private ICommand _AddFamilyCommand { get; set; }
        private ICommand _AddElementCommand { get; set; }
        private ICommand _SaveGameCommand { get; set; }

        //Scanner Tab
        private ICommand _PickPathCommand { get; set; }
        private ICommand _ScanModCommand { get; set; }
        private ICommand _ScanPathCommand { get; set; }

        //QMT Tab
        private ICommand _SaveQMTCommand { get; set; }

        private ICommand _EmptyQMTCommand { get; set; }
        private ICommand _AddQMTFCommand { get; set; }

        //Paths Tab
        private ICommand _SavePathCommand { get; set; }
        #endregion

        #region Public
        //Game Editor Tab
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

        //QMT Tab
        public ICommand SaveQMTCommand
        {
            get
            {
                if (_SaveQMTCommand == null)
                {
                    _SaveQMTCommand = new RelayCommand(param => SaveQuasarModTypes());
                }
                return _SaveQMTCommand;
            }
        }

        public ICommand EmptyQMTCommand
        {
            get
            {
                if (_EmptyQMTCommand == null)
                {
                    _EmptyQMTCommand = new RelayCommand(param => EmptyQuasarModTypes());
                }
                return _EmptyQMTCommand;
            }
        }
        public ICommand AddQMTFCommand
        {
            get
            {
                if (_AddQMTFCommand == null)
                {
                    _AddQMTFCommand = new RelayCommand(param => AddQuasarModTypeFileDefinition());
                }
                return _AddQMTFCommand;
            }
        }

        //Paths Tab
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

        #endregion

        #region Data

        #region Private
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


        //Quasar Mod Types
        private ObservableCollection<QuasarModType> _QuasarModTypes { get; set; }
        private QuasarModType _SelectedQuasarModType { get; set; }
        private QuasarModTypeFileDefinition _SelectedQuasarModTypeFileDefinition { get; set; }
        private LibraryItem _SelectedTestLibraryItem { get; set; }
        private ObservableCollection<ScanTestResult> _TestResults { get; set; }

        private double _ProgressValue { get; set; }
        private string _ProgressString { get; set; }

        //Paths
        private CollectionViewSource _CollectionViewSource { get; set; }
        private string _RepoPath { get; set; }
        

        
        #endregion

        #region Public
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
                try
                {
                    AssociatedRootCategory = API.Games[0].RootCategories.Single(c => c.Guid == value.GBItem.RootCategoryGuid);
                    AssociatedSubCategory = AssociatedRootCategory.SubCategories.Single(c => c.Guid == value.GBItem.SubCategoryGuid);
                }
                catch (NullReferenceException e)
                {

                }
                

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
                            SearchFileName = "{AnyFile}.{AnyExtension}",
                            OutputPath = @"{ModName}/",
                            FilePriority = 10,
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
                /*if (value != null)
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
                }*/


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
        public ObservableCollection<ScanTestResult> TestResults
        {
            get => _TestResults;
            set
            {
                _TestResults = value;
                OnPropertyChanged("TestResults");
            }
        }



        #endregion

        #endregion

        #region View

        #region Private
        private string _ScanPathText { get; set; } = @"O:\Super Smash Bros Ultimate\ARC Data\Scan Tests";
        private bool _UnrecognizedFilter { get; set; }
        private bool _Scanning { get; set; }
        private string _HumanTime { get; set; } = "00:00:00";
        private int _Seconds { get; set; }

        //Admin status
        private bool _IsAdmin { get; set; }

        #endregion

        #region Public

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

        public bool Scanning
        {
            get => _Scanning;
            set
            {
                _Scanning = value;
                OnPropertyChanged("Scanning");
            }
        }

        public string HumanTime
        {
            get => _HumanTime;
            set
            {
                _HumanTime = value;
                OnPropertyChanged("HumanTime");
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

        #endregion

        public DatabaseEditorViewModel() {

            //Loading referentials from the app and the user's Quasar folders
            Games = ResourceManager.GetGames(InstallDirectory);
            Library = UserDataManager.GetSeparatedLibrary(@"C:\Users\Mowjoh\Documents\Quasar");
            ContentItems = UserDataManager.GetSeparatedContentItems(@"C:\Users\Mowjoh\Documents\Quasar");
            //Workspaces = UserDataManager.GetWorkspaces(AppDataPath);
            QuasarModTypes = ResourceManager.GetQuasarModTypes(InstallDirectory);
            API = UserDataManager.GetSeparatedGamebananaApi(@"C:\Users\Mowjoh\Documents\Quasar");

            //Setting up the Test Result Source
            CollectionViewSource = new CollectionViewSource();
            CollectionViewSource.Source = TestResults;
            CollectionViewSource.Filter += CollectionViewSource_Filter;
            RepoPath = Properties.Settings.Default.RepoPath;

            //Showing Admin warning if necessary
            IsAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }

        #region Game Editor Actions

        /// <summary>
        /// Adds an empty Family to the Game File
        /// </summary>
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

        /// <summary>
        /// Adds a empty Element to the selected Family
        /// </summary>
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

        /// <summary>
        /// Saves the Game File
        /// </summary>
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

        #endregion

        #region Scanner Actions

        /// <summary>
        /// Prompts the user to pick a path to scan
        /// </summary>
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

        /// <summary>
        /// Scans the selected mod
        /// </summary>
        public async void ScanMod()
        {
            
            if (SelectedTestLibraryItem == null)
            {
                MessageBox.Show("Select a mod first maybe?", "Dumb Dumb", MessageBoxButton.OK,MessageBoxImage.Warning);
            }
            else
            {
                _Seconds = 0;
                HumanTime = "00:00:00";
                string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Quasar";
                string ModPath = String.Format(@"{0}\Library\Mods\{1}\", DocumentsPath, SelectedTestLibraryItem.Guid);
                Timer ZaWardo = new Timer();
                ZaWardo.Interval = 1000;
                ZaWardo.Tick += ZaWardo_Tick;

                ZaWardo.Start();
                Scanning = true;

                await Task.Run(() => Scan(ModPath));

                Scanning = false;
                ZaWardo.Stop();
            }
            
        }

        /// <summary>
        /// Scans the selectd path
        /// </summary>
        public async void ScanPath()
        {
            _Seconds = 0;
            HumanTime = "00:00:00";
            Timer ZaWardo = new Timer();
            ZaWardo.Interval = 1000;
            ZaWardo.Tick += ZaWardo_Tick;

            ZaWardo.Start();
            Scanning = true;

            await Task.Run(() => Scan(ScanPathText));

            Scanning = false;
            ZaWardo.Stop();
        }

        /// <summary>
        /// Scan Task
        /// </summary>
        /// <param name="Path">path to scan</param>
        /// <returns></returns>
        public async Task<int> Scan(string Path)
        {

            ObservableCollection<ScanFile> ScanResults = new ObservableCollection<ScanFile>();
            List<ScanFile> workingList = FileScanner.GetScanFiles(Path);

            workingList = FileScanner.FilterIgnoredFiles(workingList);
            workingList = FileScanner.MatchScanFiles(workingList, QuasarModTypes, Games[0], Path);

            foreach (ScanFile scan_file in workingList)
            {
                ScanResults.Add(scan_file);
            }

            Application.Current.Dispatcher.Invoke((Action)delegate {

                ProcessScan(ScanResults);
            });


            return 0;
        }

        /// <summary>
        /// Event to tick the clock when a second passes during scanning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZaWardo_Tick(object sender, EventArgs e)
        {
            _Seconds++;

            int Hours = _Seconds / 3600;
            int Minutes = (_Seconds - (Hours * 3600)) / 60;
            int Seconds = _Seconds % 60;

            HumanTime = String.Format("{0}:{1}:{2}", Hours.ToString("D2"), Minutes.ToString("D2"), Seconds.ToString("D2"));
        }

        /// <summary>
        /// Processes the scan results into useable data for the interface
        /// </summary>
        /// <param name="ScanResults"></param>
        public void ProcessScan(ObservableCollection<ScanFile> ScanResults)
        {
            TestResults = new ObservableCollection<ScanTestResult>();
            foreach (ScanFile Scan in ScanResults)
            {
                TestResults.Add(new ScanTestResult()
                {
                    ScanFile = Scan
                });
            }
            CollectionViewSource.Source = TestResults;
            CollectionViewSource.View.Refresh();
        }

        /// <summary>
        /// Filters the Test Results to ignore Scanned files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #endregion

        #region Quasar Mod Types Actions

        /// <summary>
        /// Saves the Quasar Mod Types File
        /// </summary>
        public void SaveQuasarModTypes()
        {
            ResourceManager.SaveQuasarModTypes(QuasarModTypes, InstallDirectory);
            ResourceManager.SaveQuasarModTypes(QuasarModTypes, RepoPath);
        }

        /// <summary>
        /// Empties the useless entries within the Quasar Mod Types File
        /// </summary>
        public void EmptyQuasarModTypes()
        {

            foreach (QuasarModType quasarModType in QuasarModTypes)
            {
                List<QuasarModTypeFileDefinition> ToDelete = new();

                foreach (QuasarModTypeFileDefinition quasarModTypeFileDefinition in quasarModType.QuasarModTypeFileDefinitions)
                {
                    if (string.IsNullOrEmpty(quasarModTypeFileDefinition.SearchPath) && string.IsNullOrEmpty(quasarModTypeFileDefinition.SearchFileName))
                    {
                        ToDelete.Add(quasarModTypeFileDefinition);

                    }
                }

                foreach (QuasarModTypeFileDefinition quasarModTypeFileDefinition in ToDelete)
                {
                    quasarModType.QuasarModTypeFileDefinitions.Remove(quasarModTypeFileDefinition);
                }
            }
        }

        public void AddQuasarModTypeFileDefinition()
        {
            int IDMax = -1;
            foreach (QuasarModTypeFileDefinition quasarModTypeFileDefinition in SelectedQuasarModType.QuasarModTypeFileDefinitions)
            {
                if (quasarModTypeFileDefinition.ID > IDMax)
                {
                    IDMax = quasarModTypeFileDefinition.ID;
                }
            }

            SelectedQuasarModType.QuasarModTypeFileDefinitions.Add(new()
            {
                ID = IDMax +1,
                SearchPath = "",
                SearchFileName = @"{AnyFile}.{AnyExtension}",
                FilePriority = ((SelectedQuasarModType.QuasarModTypeFileDefinitions.Count + 1 ) * 10),
                OutputPath = @"{ModName}/"
            });
        }

        #endregion

        #region Paths Actions

        /// <summary>
        /// Saves the user path to it's Quasar Data
        /// </summary>
        public void SavePath()
        {
            Properties.Settings.Default.RepoPath = RepoPath;
            Properties.Settings.Default.Save();
        }

        #endregion

    }


}
