using Quasar.Common.Models;
using Quasar.Data.V2;
using Quasar.Helpers.Json;
using Quasar.Helpers.ModScanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace QuasarDataEditor
{
    class QuasarDataEditorViewModel : ObservableObject
    {
        #region Static paths
        //private static string DocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string DocumentsFolder = @"M:\Super Smash Bros Ultimate\Software";
        private static string GameSource = DocumentsFolder + @"\Quasar Debug\Resources\Games.json";
        private static string LibrarySource = DocumentsFolder + @"\Quasar Debug\Library\Library.json";
        private static string ContentSource = DocumentsFolder + @"\Quasar Debug\Library\ContentItems.json";
        private static string WorkspaceSource = DocumentsFolder + @"\Quasar Debug\Library\Workspaces.json";
        private static string ModLoaderSource = DocumentsFolder + @"\Quasar Debug\Resources\ModLoaders.json";
        private static string QuasarModTypeSource = DocumentsFolder + @"\Quasar Debug\Resources\ModTypes.json";
        private static string APISource = DocumentsFolder + @"\Quasar Debug\Resources\Gamebanana.json";
        #endregion

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
        private ObservableCollection<TestResult> _TestResults {get; set;}

        private double _ProgressValue { get; set; }
        private string _ProgressString { get; set; }
        #endregion

        #region Commands
        #region Saves
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
        private ICommand _SaveLibraryCommand { get; set; }
        public ICommand SaveLibraryCommand
        {
            get
            {
                if (_SaveLibraryCommand == null)
                {
                    _SaveLibraryCommand = new RelayCommand(param => SaveLibraryFile());
                }
                return _SaveLibraryCommand;
            }
        }
        private ICommand _SaveContentCommand { get; set; }
        public ICommand SaveContentCommand
        {
            get
            {
                if (_SaveContentCommand == null)
                {
                    _SaveContentCommand = new RelayCommand(param => SaveContentItemFile());
                }
                return _SaveContentCommand;
            }
        }
        private ICommand _SaveWorkspaceCommand { get; set; }
        public ICommand SaveWorkspaceCommand
        {
            get
            {
                if (_SaveWorkspaceCommand == null)
                {
                    _SaveWorkspaceCommand = new RelayCommand(param => SaveWorkspaceFile());
                }
                return _SaveWorkspaceCommand;
            }
        }
        private ICommand _SaveModLoaderCommand { get; set; }
        public ICommand SaveModLoaderCommand
        {
            get
            {
                if (_SaveModLoaderCommand == null)
                {
                    _SaveModLoaderCommand = new RelayCommand(param => SaveModLoaderFile());
                }
                return _SaveModLoaderCommand;
            }
        }
        private ICommand _SaveQuasarModTypeCommand { get; set; }
        public ICommand SaveQuasarModTypeCommand
        {
            get
            {
                if (_SaveQuasarModTypeCommand == null)
                {
                    _SaveQuasarModTypeCommand = new RelayCommand(param => SaveQuasarModTypeFile());
                }
                return _SaveQuasarModTypeCommand;
            }
        }
        #endregion

        private ICommand _RefreshCommand { get; set; }
        public ICommand RefreshCommand
        {
            get
            {
                if (_RefreshCommand == null)
                {
                    _RefreshCommand = new RelayCommand(param => LoadStuff());
                }
                return _RefreshCommand;
            }
        }

        //Mod Testing
        public ICommand PickFolderCommand
        {
            get
            {
                if (_PickFolderCommand == null)
                {
                    _PickFolderCommand = new RelayCommand(param => PickPath());
                }
                return _PickFolderCommand;
            }
        }
        private ICommand _PickFolderCommand { get; set; }
        public ICommand TestModCommand
        {
            get
            {
                if (_TestModCommand == null)
                {
                    _TestModCommand = new RelayCommand(param => TestMod());
                }
                return _TestModCommand;
            }
        }
        private ICommand _TestModCommand { get; set; }
        public ICommand ProcessOutputCommand
        {
            get
            {
                if (_ProcessOutputCommand == null)
                {
                    _ProcessOutputCommand = new RelayCommand(param => ProcessModOutput());
                }
                return _ProcessOutputCommand;
            }
        }
        private ICommand _ProcessOutputCommand { get; set; }
        

        public ICommand ImportSongs
        {
            get
            {
                if (_ImportSongs == null)
                {
                    _ImportSongs = new RelayCommand(param => ProcessSongs());
                }
                return _ImportSongs;
            }
        }
        private ICommand _ImportSongs { get; set; }


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
                if(value != null)
                {
                    if(_SelectedQuasarModType.ID == 0)
                    {
                        _SelectedQuasarModType.ID = QuasarModTypes.Count;
                        OnPropertyChanged("QuasarModTypes");
                    }
                    if(_SelectedQuasarModType.QuasarModTypeFileDefinitions == null)
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
                if(value != null)
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
        public ObservableCollection<TestResult> TestResults
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
        #endregion

        public QuasarDataEditorViewModel()
        {
            LoadStuff();
        }

        public void LoadStuff()
        {
            Games = JSonHelper.GetGames(true, GameSource);
            Library = JSonHelper.GetLibrary(true, LibrarySource);
            ContentItems = JSonHelper.GetContentItems(true, ContentSource);
            Workspaces = JSonHelper.GetWorkspaces(true, WorkspaceSource);
            ModLoaders = JSonHelper.GetModLoaders(true, ModLoaderSource);
            QuasarModTypes = JSonHelper.GetQuasarModTypes(true, QuasarModTypeSource);
            API = JSonHelper.GetGamebananaAPI(true, APISource);
            
        }

        public void PickPath()
        {
            TestResults = new ObservableCollection<TestResult>();
            string pathselected = null;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"F:\FakeSwitch\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    pathselected = openFileDialog.FileName;

                }
            }
            if(pathselected != null)
            {
                ProgressString = "Scanning Files";
                ProgressValue = 0;
                ObservableCollection<ContentItem> Scan = Scannerino.ScanMod(pathselected, QuasarModTypes, Games[0], new LibraryItem() { Guid = Guid.NewGuid(), Name = "ScanName" }, true);
                int i = 1;
                foreach (ContentItem ci in Scan)
                {
                    ci.Guid = Guid.NewGuid();
                    i++;
                }
                string ModFolder = "";
                ObservableCollection<ScanFile> ScannedFiles = Scannerino.GetScanFiles(pathselected, QuasarModTypes, Games[0], ModFolder, true);

                ProgressString = "Processing ScanFiles";
                double count = 1;
                double total = ScannedFiles.Count;
                ProgressValue = (count / total) * 100;
                foreach (ScanFile sf in ScannedFiles)
                {
                    if (sf.Scanned)
                    {
                        ContentItem ci = Scan.SingleOrDefault(c => c.ScanFiles.Any(f => f.SourcePath == sf.SourcePath));
                        QuasarModType qmt = QuasarModTypes.Single(q => q.ID == sf.QuasarModTypeID);
                        GameElementFamily fam = Games[0].GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID);
                        GameElement ge = fam.GameElements.Single(e => e.ID == sf.GameElementID);
                        QuasarModTypeFileDefinition fd = qmt.QuasarModTypeFileDefinitions.Single(t => t.ID == sf.QuasarModTypeFileDefinitionID);
                        TestResults.Add(new TestResult()
                        {
                            ScanFile = sf,
                            ContentItem = ci,
                            QuasarModType = qmt,
                            QuasarModTypeFileDefinition = fd,
                            GameElement = ge
                        });
                    }
                    else
                    {
                        TestResults.Add(new TestResult()
                        {
                            ScanFile = sf
                        });
                    }
                    count++;
                    ProgressValue = (count / total) * 100;
                }
                ProgressString = "Done";
            }
            

        }

        public async void TestMod()
        {
            if(SelectedTestLibraryItem != null)
            {
                await TestModAction();
            }
        }

        public async Task<int> TestModAction()
        {
            TestResults = new ObservableCollection<TestResult>();
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", @"M:\Super Smash Bros Ultimate\Software\Quasar Debug", SelectedTestLibraryItem.Guid);

            ProgressString = "Scanning Files";
            ProgressValue = 0;
            ObservableCollection<ContentItem> Scan = Scannerino.ScanMod(ModFolder, QuasarModTypes, Games[0], SelectedTestLibraryItem);
            int i = 1;
            foreach (ContentItem ci in Scan)
            {
                ci.Guid =Guid.NewGuid();
                i++;
            }

            
            ObservableCollection<ScanFile> ScannedFiles = Scannerino.GetScanFiles(ModFolder, QuasarModTypes, Games[0], ModFolder);

            ProgressString = "Processing ScanFiles";
            double count = 1;
            double total = ScannedFiles.Count;
            ProgressValue = (count / total) *100;
            foreach (ScanFile sf in ScannedFiles)
            {
                if (sf.Scanned)
                {
                    ContentItem ci = Scan.SingleOrDefault(c => c.ScanFiles.Any(f => f.SourcePath == sf.SourcePath));
                    QuasarModType qmt = QuasarModTypes.Single(q => q.ID == sf.QuasarModTypeID);
                    GameElementFamily fam = Games[0].GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID);
                    GameElement ge = fam.GameElements.Single(e => e.ID == sf.GameElementID);
                    QuasarModTypeFileDefinition fd = qmt.QuasarModTypeFileDefinitions.Single(t => t.ID == sf.QuasarModTypeFileDefinitionID);
                    TestResults.Add(new TestResult()
                    {
                        ScanFile = sf,
                        ContentItem = ci,
                        QuasarModType = qmt,
                        QuasarModTypeFileDefinition = fd,
                        GameElement = ge
                    });
                }
                else
                {
                    TestResults.Add(new TestResult()
                    {
                        ScanFile = sf
                    });
                }
                count++;
                ProgressValue = (count / total) * 100;
            }
            ProgressString = "Done";
            return 0;
        }
        public void ProcessModOutput()
        {
            ProgressString = "Processing Outputs";
            double count = 1;
            double total = TestResults.Count;
            ProgressValue = (count / total) * 100;
            if (TestResults.Count != 0 && SelectedTestModLoader != null)
            {
                string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", @"F:\Quasar", SelectedTestLibraryItem.Guid);
                foreach (TestResult tr in TestResults)
                {
                    if (tr.ScanFile.Scanned)
                    {
                        tr.Output = Scannerino.ProcessOutput(tr.ScanFile.SourcePath, tr.QuasarModTypeFileDefinition, tr.GameElement, int.Parse(tr.ScanFile.Slot), SelectedTestModLoader.ID, SelectedTestLibraryItem, ModFolder);
                    }
                    count++;
                    ProgressValue = (count / total) * 100;
                }
            }
            ProgressString = "Done";
            OnPropertyChanged("TestResults");
        }

        public void ProcessSongs()
        {
            Games[0] = DBStructs.GetXML(Games[0]);
        }

        #region Saves
        public void SaveGameFile()
        {
            JSonHelper.SaveGamesFile(Games, GameSource);
        }
        public void SaveLibraryFile()
        {
            JSonHelper.SaveLibrary(Library, LibrarySource);
        }
        public void SaveContentItemFile()
        {
            JSonHelper.SaveContentItems(ContentItems, ContentSource);
        }
        public void SaveWorkspaceFile()
        {
            JSonHelper.SaveWorkspaces(Workspaces, WorkspaceSource);
        }
        public void SaveModLoaderFile()
        {
            JSonHelper.SaveModLoaders(ModLoaders, ModLoaderSource);
        }
        public void SaveQuasarModTypeFile()
        {
            JSonHelper.SaveQuasarModTypes(QuasarModTypes, QuasarModTypeSource);
        }
        #endregion

        
    }

    public class TestResult : ObservableObject
    {
        private ScanFile _ScanFile { get; set; }
        private ContentItem _ContentItem { get; set; }
        private QuasarModType _QuasarModType { get; set; }
        private QuasarModTypeFileDefinition _QuasarModTypeFileDefinition { get; set; }
        private GameElement _GameElement { get; set; }
        private string _Output { get; set; }


        public ScanFile ScanFile 
        {
            get => _ScanFile;
            set
            {
                _ScanFile = value;
                OnPropertyChanged("ScanFile");
            }
        }
        public ContentItem ContentItem
        {
            get => _ContentItem;
            set
            {
                _ContentItem = value;
                OnPropertyChanged("ContentItem");
            }
        }
        public QuasarModType QuasarModType
        {
            get => _QuasarModType;
            set
            {
                _QuasarModType = value;
                OnPropertyChanged("QuasarModType");
            }
        }
        public QuasarModTypeFileDefinition QuasarModTypeFileDefinition
        {
            get => _QuasarModTypeFileDefinition;
            set
            {
                _QuasarModTypeFileDefinition = value;
                OnPropertyChanged("QuasarModTypeFileDefinition");
            }
        }
        public GameElement GameElement
        {
            get => _GameElement;
            set
            {
                _GameElement = value;
                OnPropertyChanged("GameElement");
            }
        }
        public string Output
        {
            get => _Output;
            set
            {
                _Output = value;
                OnPropertyChanged("Output");
            }
        }
        public bool NoContent => ContentItem == null;
        public bool NoGameElement => GameElement == null;

    }
}
