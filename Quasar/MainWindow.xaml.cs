using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Quasar.FileSystem;
using Quasar.Quasar_Sys;
using Quasar.Controls;
using Quasar.XMLResources;
using static Quasar.XMLResources.Library;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Media;
using Quasar.Views;
using System.ComponentModel;
using System.Windows.Data;

namespace Quasar
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region Triggers
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //Interface Triggers
        private bool _GameSelectOverlayDisplay;
        public bool GameSelectOverlayDisplay
        {
            get
            {
                return _GameSelectOverlayDisplay;
            }
            set
            {
                _GameSelectOverlayDisplay = value;
                GameSelectVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private Visibility _GameSelectVisibility;
        public Visibility GameSelectVisibility
        {
            get
            {
                return _GameSelectVisibility;
            }
            set
            {
                _GameSelectVisibility = value;
                OnPropertyChanged("GameSelectVisibility");
            }
        }
        #endregion
        
        #region Data
        //Mod Library References
        private List<LibraryMod> _Mods;
        public List<LibraryMod> Mods
        {
            get
            {
                return _Mods;
            }
            set
            {
                _Mods = value;
                ListMods = LoadLibraryMods();
            }
        }

        private ObservableCollection<ModListItem> _ListMods { get; set; }
        public ObservableCollection<ModListItem> ListMods
        {
            get
            {
                return _ListMods;
            }
            set
            {
                _ListMods = value;

                OnPropertyChanged("ListMods");
            }
        }

        public ObservableCollection<LibraryMod> WorkingModList;


        //API Library References
        private List<Game> _Games { get; set; }
        public List<Game> Games
        {
            get
            {
                return _Games;
            }
            set
            {
                _Games = value;
                OnPropertyChanged("Games");
            }
        }
        private Game _CurrentGame { get; set; }
        public Game CurrentGame
        {
            get
            {
                return _CurrentGame;
            }
            set
            {
                _CurrentGame = value;
                setGameIMT();
                setGameDataCategories();
                GameSelectOverlayDisplay = value.ID == -1 ? true : false;
                OnPropertyChanged("CurrentGame");
            }
        }

        private GameModType _CurrentGameApiModType;
        public GameModType CurrentGameApiModType
        {
            get
            {
                return _CurrentGameApiModType;
            }
            set
            {
                _CurrentGameApiModType = value;
                setGameAPICategories();
                OnPropertyChanged("CurrentGameApiModType");
            }
        }

        private ObservableCollection<Category> _GameAPISubCategories;
        public ObservableCollection<Category> GameAPISubCategories
        {
            get
            {
                return _GameAPISubCategories;
            }
            set
            {
                _GameAPISubCategories = value;
                OnPropertyChanged("GameAPISubCategories");
            }
        }
        private void setGameAPICategories()
        {
            GameAPISubCategories = new ObservableCollection<Category>();
            if (CurrentGameApiModType != null)
            {
                List<Category> categories = CurrentGame.GameModTypes.Find(gmt => gmt.ID == CurrentGameApiModType.ID).Categories;
                foreach (Category c in categories)
                {
                    GameAPISubCategories.Add(c);
                }
            }

        }


        //Content Library References

        private List<ContentMapping> _ContentMappings;
        public List<ContentMapping> ContentMappings
        {
            get
            {
                return _ContentMappings;
            }
            set
            {
                _ContentMappings = value;
                ListContents = LoadContentMappings();
                OnPropertyChanged("ContentMappings");
            }
        }

        private ObservableCollection<ContentListItem> _ListContents { get; set; }
        public ObservableCollection<ContentListItem> ListContents
        {
            get
            {
                return _ListContents;
            }
            set
            {
                _ListContents = value;
                OnPropertyChanged("ListContents");
            }
        }

        //GameData
        List<GameData> GameData { get; set; }

        private ObservableCollection<GameDataCategory> _GameDataCategories;
        public ObservableCollection<GameDataCategory> GameDataCategories
        {
            get
            {
                return _GameDataCategories;
            }
            set
            {
                _GameDataCategories = value;
                OnPropertyChanged("GameDataCategories");
            }
        }
        private void setGameDataCategories()
        {
            GameDataCategories = new ObservableCollection<GameDataCategory>();
            if (CurrentGame.ID != -1)
            {
                List<GameDataCategory> gdc = GameData.Find(g => g.GameID == CurrentGame.ID).Categories;
                foreach (GameDataCategory gd in gdc)
                {
                    GameDataCategories.Add(gd);
                }
            }
        }

        private GameDataCategory _CurrentGDC;
        public GameDataCategory CurrentGDC
        {
            get
            {
                return _CurrentGDC;
            }
            set
            {
                _CurrentGDC = value;
                OnPropertyChanged("CurrentGDC");
            }
        }

        //Internal Mod Types
        List<InternalModType> InternalModTypes { get; set; }

        private ObservableCollection<InternalModType> _GameIMT;
        public ObservableCollection<InternalModType> GameIMT
        {
            get
            {
                return _GameIMT;
            }
            set
            {
                _GameIMT = value;
                OnPropertyChanged("GameIMT");
            }
        }
        private void setGameIMT()
        {
            GameIMT = new ObservableCollection<InternalModType>();
            if (CurrentGame.ID != -1)
            {
                List<InternalModType> internalModTypes = InternalModTypes.FindAll(imt => imt.GameID == CurrentGame.ID);
                foreach (InternalModType imt in internalModTypes)
                {
                    GameIMT.Add(imt);
                }
            }
        }

        private InternalModType _CurrentIMT;
        public InternalModType CurrentIMT
        {
            get
            {
                return _CurrentIMT;
            }
            set
            {
                _CurrentIMT = value;
                OnPropertyChanged("CurrentIMT");
            }
        }
        #endregion

        #region Filters
        private void ShowOnlyNonFilteredMods(object sender, FilterEventArgs e)
        {
            ModListItem mle = e.Item as ModListItem;
            if (mle != null)
            {
                if (mle.Filter == false)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        private void ShowOnlyNonFilteredContents(object sender, FilterEventArgs e)
        {
            ContentListItem cli = e.Item as ContentListItem;
            if (cli != null)
            {
                if (cli.Filter == false)
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        #endregion

        //Quasar Downloads
        readonly Mutex serverMutex;
        public QuasarDownloads DLS;

        public bool readytoSelect { get; set; }


        public MainWindow()
        {
            //Setting up Server or Client
            DLS = new QuasarDownloads();
            DLS.List.CollectionChanged += QuasarDownloadCollectionChanged;
            serverMutex = Checker.Instances(serverMutex, DLS.List);

            //Pre-run checks
            bool Update = Checker.CheckQuasarUpdated();
            Folderino.CheckBaseFolders();
            Folderino.CompareReferences();
            bool Debug = false;

            #if DEBUG
            Debug = true;
            #endif

            if (Update || Debug)
            {
                Folderino.UpdateBaseFiles();
            }

            //Setting language
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);

            //Aww, here we go again
            InitializeComponent();

            //Loading things
            LoadBasicLists();
            LoadModLibrary();
            LoadContentLibrary();

            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = ListMods;
            cvs.Filter += ShowOnlyNonFilteredMods;


            SetInterfaceWithParams();

            readytoSelect = true;


        }


        #region XML LOAD
        //Load Mod Library into memory
        private void LoadModLibrary()
        {
            Mods = GetLibraryModList();
            WorkingModList = new ObservableCollection<LibraryMod>();
        }


        private void LoadContentLibrary()
        {
            ContentMappings = ContentXML.GetContentMappings();
        }

        private ObservableCollection<ModListItem> LoadLibraryMods()
        {
            ObservableCollection<ModListItem> newMods = new ObservableCollection<ModListItem>();

            foreach (LibraryMod lm in Mods)
            {
                Game gamu = Games.Find(g => g.ID == lm.GameID);

                ModListItem mli = new ModListItem(_OperationActive: false, _LibraryMod: lm, _Game: gamu);
                mli.Downloaded = true;
                mli.TrashRequested += Handler_TrashRequested;
                newMods.Add(mli);
            }

            return newMods;
        }

        private ObservableCollection<ContentListItem> LoadContentMappings()
        {
            ObservableCollection<ContentListItem> newContentList = new ObservableCollection<ContentListItem>();
            int modID = 0;
            int colorID = 0;

            foreach (ContentMapping cm in ContentMappings)
            {
                LibraryMod lm = Mods.Single(l => l.ID == cm.ModID);
                InternalModType imt = InternalModTypes.Single(i => i.ID == cm.InternalModType);

                colorID = modID != lm.ID ? colorID == 0 ? 1 : 0 : colorID;
                modID = modID != lm.ID ? lm.ID : modID;
                List<GameDataCategory> gdc = GameData.Find(gd => gd.GameID == lm.GameID).Categories;
                ContentListItem cli = new ContentListItem(cm, lm, imt, gdc, colorID);
                cli.SaveRequested += Handler_SaveRequested;
                newContentList.Add(cli);
            }
            return newContentList;
        }

        private void LoadBasicLists()
        {
            Games = XML.GetGames();
            InternalModTypes = XML.GetInternalModTypes();
            GameData = XML.GetGameData();
        }

        #endregion

        #region GameSelectOverlay
        private void ContentIMTSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GamesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (readytoSelect)
            {
                readytoSelect = false;
                // Create a DoubleAnimation to animate the width of the button.
                DoubleAnimation myDoubleAnimation = new DoubleAnimation() { From = 1, To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(200)) };

                // Configure the animation to target the button's Width property.
                Storyboard.SetTarget(myDoubleAnimation, OverlayGrid);
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath("RenderTransform.ScaleX"));

                // Create a storyboard to contain the animation.
                Storyboard ReturnAnimation = new Storyboard();
                ReturnAnimation.Children.Add(myDoubleAnimation);

                Game selectedGame = (Game)GamesListView.SelectedItem;

                SelectGame(selectedGame);

                //Properties.Settings.Default.LastSelectedGame = selectedGame.ID;
                Properties.Settings.Default.Save();

                ReturnAnimation.Begin();
            }

        }

        private void SelectGame(Game gamu)
        {
            CurrentGame = gamu;
            FilterModList(-1, -1, CurrentGame.ID);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentGame.ID != -1)
            {
                GamesListView.SelectedItem = GamesListView.Items[GamesListView.Items.IndexOf(CurrentGame)];
            }
            readytoSelect = true;
        }
        #endregion

        #region ModManagements

        //--------------------------------------
        //Mod Actions
        private void ModSelected(object sender, SelectionChangedEventArgs e)
        {

            ModListItem mli = (ModListItem)ManagementModListView.SelectedItem;
            if(mli != null)
            {
                foreach (ModListItem m in ManagementModListView.Items)
                {
                    m.Smol = true;
                }
                if (mli.Downloaded)
                {
                    mli.Smol = false;
                }
            }
        }

        private void ContentSelected(object sender, SelectionChangedEventArgs e)
        {
            ContentListItem cle = (ContentListItem)ContentListView.SelectedItem;
            foreach (ContentListItem cl in ContentListView.Items)
            {
                cl.Smol = true;
            }

            if (cle != null)
            {
                cle.Smol = false;
            }
        }

        //Version actions
        private async void CheckUpdates(object sender, RoutedEventArgs e)
        {
            ModListItem element = (ModListItem)ManagementModListView.SelectedItem;
            if (element != null)
            {
                //Getting local Mod
                LibraryMod mod = Mods.Find(mm => mm.ID == element.LocalMod.ID && mm.TypeID == element.LocalMod.TypeID);
                Game game = Games.Find(g => g.ID == mod.GameID);
                GameModType mt = game.GameModTypes.Find(g => g.ID == mod.TypeID);
                //Parsing mod info from API
                APIMod newAPIMod = await APIRequest.GetAPIMod(mt.APIName, element.LocalMod.ID.ToString());

                //Create Mod from API information
                LibraryMod newmod = GetLibraryMod(newAPIMod, game);

                if (mod.Updates < newmod.Updates)
                {
                    string[] newDL = await APIRequest.GetDownloadFileName(mt.APIName, element.LocalMod.ID.ToString());
                    string quasarURL = APIRequest.GetQuasarDownloadURL(newDL[0], newDL[1], mt.APIName, element.LocalMod.ID.ToString());
                    LaunchDownload(quasarURL);
                }
            }
        }

        public void Handler_TrashRequested(object sender, EventArgs e)
        {
            ModListItem item = (ModListItem)sender;

            //Removing from ContentMappings
            List<ContentMapping> relatedMappings = ContentMappings.FindAll(cm => cm.ModID == item.LocalMod.ID);
            foreach(ContentMapping cm in relatedMappings)
            {
                ContentMappings.Remove(cm);
            }

            //Refreshing Contents
            ListContents = LoadContentMappings();

            //Removing from Library
            Mods.Remove(item.LocalMod);

            //Refreshing Mods
            ListMods = LoadLibraryMods();

            //Writing changes
            Library.WriteModListFile(Mods);
            ContentXML.WriteContentMappingListFile(ContentMappings);

        }


        //--------------------------------------
        //Filtering Actions

        //Refreshes the content of the mod list based on mod type
        private void ModTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentGameApiModType = (GameModType)ManagementAPITypeSelect.SelectedItem;
            FilterModList(CurrentGameApiModType != null ? CurrentGameApiModType.ID : -1, -1, CurrentGame.ID);
        }

        //Refreshes the content of the mod list based on mod type and mod category
        private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Category selectedItem = (Category)ManagementAPICategorySelect.SelectedItem;

            FilterModList(CurrentGameApiModType != null ? CurrentGameApiModType.ID : -1, selectedItem != null ? selectedItem.ID : -1, CurrentGame.ID);

        }

        //Shows items according to filters
        private void FilterModList(int _modType, int modCategory, int _modGame)
        {
            
            foreach (ModListItem mle in ListMods)
            {
                bool AnyModType = _modType == -1;
                bool AnyCategory = modCategory == -1;
                bool AnyGame = _modGame == -1;

                if((AnyGame || mle.Game.ID == _modGame) && (AnyCategory || mle.LocalMod.APICategoryID == modCategory) && (AnyModType || mle.LocalMod.TypeID == _modType))
                {
                    mle.Filter = false;
                }
                else
                {
                    mle.Filter = true;
                }
            }
            CollectionViewSource cvs = (CollectionViewSource)this.Resources["CollectionOMods"];
            cvs.View.Refresh();
        }

        



        #endregion

        #region Mod Content
        private void ContentDataGridItemSelected(object sender, SelectionChangedEventArgs e)
        {
            ContentListItem cle = (ContentListItem)ContentListView.SelectedItem;
            foreach (ContentListItem cl in ContentListView.Items)
            {
                cl.Smol = true;
            }
            cle.Smol = false;
        }
        private void IMTTypeSelect(object sender, SelectionChangedEventArgs e)
        {
            CurrentIMT = (InternalModType)ContentIMTSelect.SelectedItem;
            if(CurrentIMT != null)
            {
                CurrentGDC = GameDataCategories.Single(gdc => gdc.ID == CurrentIMT.Association);
            }

            FilterContentList(CurrentIMT != null ? CurrentIMT.ID : -1, -1, CurrentGame.ID);

        }

        private void IMTGDCItemSelect(object sender, SelectionChangedEventArgs e)
        {
            GameDataItem CurrentGDI = (GameDataItem)ContentGDCSelect.SelectedItem;
            FilterContentList(CurrentIMT != null ? CurrentIMT.ID : -1, CurrentGDI != null ? CurrentGDI.ID : -1, CurrentGame.ID);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ContentGDCSelect.SelectedIndex = -1;
            ContentIMTSelect.SelectedIndex = -1;
        }

        private void FilterContentList(int _IMT, int _GDC, int _Game)
        {
            foreach (ContentListItem cli in ContentListView.Items)
            {
                bool AnyIMT = _IMT == -1;
                bool AnyGDC = _GDC == -1;
                bool AnyGame = _Game == -1;

                if ((AnyGame || cli.LocalMod.GameID == _Game) && (AnyGDC || cli.GameData.ID == _GDC) && (AnyIMT || cli.LocalModType.ID == _IMT))
                {
                    cli.Filter = false;
                }
                else
                {
                    cli.Filter = true;
                }
            }
        }

        public void Handler_SaveRequested(object sender, EventArgs e)
        {
            ContentListItem item = (ContentListItem)sender;
            ContentMapping cm = ContentMappings.ElementAt(ContentMappings.IndexOf(item.LocalMapping));
            GameDataItem gdi = (GameDataItem)item.ContentMappingAssociation.SelectedItem;

            cm.Name = item.ContentMappingName.Text;
            cm.GameDataItemID = gdi != null ? gdi.ID : -1;

            ContentXML.WriteContentMappingListFile(ContentMappings);
        }
        #endregion

        #region Mod Association
        private void AssociationGameDataList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            ListBox lb = (ListBox)sender;
            GameDataCategory gdc = (GameDataCategory)lb.SelectedItem;
            AssociationGameElementDataList.ItemsSource = gdc.Items;

            List<InternalModType> correspondingTypes = InternalModTypes.FindAll(imt => imt.Association == gdc.ID);
            AssociationTypeDataList.ItemsSource = correspondingTypes;
            */
        }

        #endregion

        #region InternalModTypes
        //State changes
        private void InternalModTypeSelected(object sender, SelectionChangedEventArgs e)
        {
            //Getting info
            GameData gameData = GameData.Find(g => g.GameID == CurrentGame.ID);
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            IMTDataGrid.ItemsSource = type.Files;
            GameDataCategory cat = gameData.Categories.Find(c =>c.ID == type.Association);

            //Resetting info
            IMTFileText.Text = "";
            IMTPathText.Text = "";
            IMTMandatory.IsChecked = false;
            IMTMandatory.IsEnabled = false;
            IMTFileText.IsEnabled = false;
            IMTPathText.IsEnabled = false;
            IMTSlotsText.IsEnabled = true;
            IMTAssotiationSelect.IsEnabled = true;

            //Displaying info
            IMTAssotiationSelect.SelectedItem = IMTAssotiationSelect.Items[IMTAssotiationSelect.Items.IndexOf(cat)];
            IMTSlotsText.Text = type.Slots.ToString();
        }

        private void DataGridRowSelected(object sender, SelectionChangedEventArgs e)
        {
            DataGrid selectedGrid = (DataGrid)sender;
            InternalModTypeFile file = (InternalModTypeFile)selectedGrid.SelectedItem;
            if (file != null)
            {
                IMTPathText.Text = file.Path;
                IMTFileText.Text = file.File;
                IMTMandatory.IsChecked = file.Mandatory;
                IMTFileText.IsEnabled = true;
                IMTPathText.IsEnabled = true;
                IMTMandatory.IsEnabled = true;
            }
        }
        private void IMTAssotiationSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            GameDataCategory category = (GameDataCategory)IMTAssotiationSelect.SelectedItem;
            if (category.ID == 0)
            {
                IMTSlotsText.IsEnabled = false;
                IMTCustomNameText.IsEnabled = true;
                IMTSlotsText.Text = "1";
            }
            else
            {
                IMTSlotsText.IsEnabled = true;
                IMTCustomNameText.IsEnabled = false;
                IMTCustomNameText.Text = "";
                InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
                IMTSlotsText.Text = type.Slots.ToString();

            }
        }

        //IMT Actions
        private void IMTAddFile(object sender, RoutedEventArgs e)
        {
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            if(type != null)
            {
                type.Files.Add(new InternalModTypeFile());
                IMTDataGrid.Items.Refresh();
            }
        }

        private void IMTDeleteFile(object sender, RoutedEventArgs e)
        {
            InternalModTypeFile file = (InternalModTypeFile)IMTDataGrid.SelectedItem;
            if (file != null)
            {
                InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
                if (type != null)
                {
                    type.Files.Remove(file);
                    IMTDataGrid.Items.Refresh();
                }
            }
        }

        //Saves
        private void IMTInfoSave(object sender, RoutedEventArgs e)
        {
            InternalModTypeFile file = (InternalModTypeFile)IMTDataGrid.SelectedItem;
            if (file != null)
            {
                file.Path = IMTPathText.Text;
                file.File = IMTFileText.Text;
                file.Mandatory = IMTMandatory.IsChecked ?? false;
            }
            IMTDataGrid.Items.Refresh();
        }
        private void IMTSaveXML(object sender, RoutedEventArgs e)
        {
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            if (type != null)
            {
                int i = 0;
                foreach(InternalModTypeFile file in type.Files)
                {
                    file.ID = i;
                    i++;
                }
                GameDataCategory cat = (GameDataCategory)IMTAssotiationSelect.SelectedItem;
                type.Association = cat.ID;
                type.Slots = Int32.Parse(IMTSlotsText.Text);
                XML.SaveInternalModType(type);
            }

            
        }
        #endregion

        #region Settings
        //Deletes Everything Quasar has stored cause that's the easy way out
        private void DeleteDocumentFolderContents(object sender, RoutedEventArgs e)
        {
            Folderino.DeleteDocumentsFolder();
        }

        private void ActivateCustomProtocol(object sender, RoutedEventArgs e)
        {

            if (Protoman.ActivateCustomProtocol())
            {
                Console.WriteLine("Fix Successful");
            }
            else
            {
                Console.WriteLine("You need admin rights to do that");
            }
        }

        private void SetInterfaceWithParams()
        {
            Game Selected = Games.Find(g => g.ID == Properties.Settings.Default.LastSelectedGame);
            SelectGame(Selected);
        }

        #endregion

        #region Detection
        //When the detector engine gets launched
        private void CMDetectStartButton_Click(object sender, RoutedEventArgs e)
        {
            List<ContentMapping> FullList = new List<ContentMapping>();

            if(CurrentGame != null)
            {
                if(CurrentGame.ID != -1)
                {
                    foreach (LibraryMod lm in Mods)
                    {
                        List<ContentMapping> MahList = Searchie.AutoDetectinator(lm, InternalModTypes, CurrentGame);

                        foreach (ContentMapping m in MahList)
                        {
                            FullList.Add(m);
                        }
                    }
                }
            }
            ContentMappings = FullList;
            
        }

        private bool FirstScanLibraryMod(LibraryMod libraryMod, Game game, List<InternalModType> types)
        {
            bool processed = false;
            List<ContentMapping> SearchList = Searchie.AutoDetectinator(libraryMod, types, game);

            List<ContentMapping> WorkingList = ContentMappings;
            foreach(ContentMapping cm in SearchList)
            {
                WorkingList.Add(cm);
                processed = true;
            }
            ContentMappings = WorkingList;

            ContentXML.WriteContentMappingListFile(ContentMappings);

            return processed;
        }

        private void DetectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            DetectionTreeView.Items.Clear();
            ContentMapping m = (ContentMapping)DetectionList.SelectedItem;
            if(DetectionList.SelectedIndex != -1)
            {
                foreach (ContentMappingFile cmf in m.Files)
                {
                    DetectionTreeView.Items.Add(new TreeViewItem() { Header = cmf.SourcePath});
                }
            }
            */
        }

        #endregion

        #region Downloads
        public class QuasarDownloads
        {
            public ObservableCollection<string> List { get; set; }

            public QuasarDownloads()
            {
                List = new ObservableCollection<string>();
            }
        }

        //Launches a Quasar Download from it's URL
        private async void LaunchDownload(string _URL)
        {
            bool newElement = false;
            string downloadText = "";
            ModListItem mli = new ModListItem(true);
            mli.TrashRequested += Handler_TrashRequested;
            
            //Setting base ModFileManager
            ModFileManager ModFileManager = new ModFileManager(_URL);

            //Parsing mod info from API
            APIMod newAPIMod = await APIRequest.GetAPIMod(ModFileManager.APIType, ModFileManager.ModID);

            //Finding related game
            Game game = Games.Find(g => g.GameName == newAPIMod.GameName);

            //Resetting ModFileManager based on new info
            ModFileManager = new ModFileManager(_URL, game);

            //Setting game UI
            mli.setGame(game);

            //Finding existing mod
            LibraryMod Mod = Mods.Find(mm => mm.ID == Int32.Parse(ModFileManager.ModID) && mm.TypeID == Int32.Parse(ModFileManager.ModTypeID));

            //Create Mod from API information
            LibraryMod newmod = GetLibraryMod(newAPIMod, game);
            
            bool needupdate = true;
            //Checking if Mod is already in library
            if (Mod != null)
            {
                if(Mod.Updates < newmod.Updates)
                {
                    var query = ListMods.Where(ml => ml.LocalMod == Mod);
                    mli = query.ElementAt(0);
                    downloadText = "Updating mod";
                }
                else
                {
                    needupdate = false;
                }
            }
            else
            {
                Mod = new LibraryMod(Int32.Parse(ModFileManager.ModID), Int32.Parse(ModFileManager.ModTypeID), false);
                newElement = true;
                downloadText = "Downloading new mod";
            }
            if (!WorkingModList.Contains(Mod) && needupdate)
            {
                WorkingModList.Add(Mod);

                //Setting up new ModList
                if (newElement)
                {
                    //Adding element to list
                    Mods.Add(newmod);
                    ListMods.Add(mli);
                }
                else
                {
                    //Updating List
                    Mods[Mods.IndexOf(Mod)] = newmod;
                }

                //Setting download UI
                mli.ModStatusValue = downloadText;

                Downloader modDownloader = new Downloader(mli);

                //Wait for download completion
                await modDownloader.DownloadArchiveAsync(ModFileManager);

                //Setting extract UI
                mli.ModStatusValue = "Extracting mod";

                //Preparing Extraction
                Unarchiver un = new Unarchiver(mli);

                //Wait for Archive extraction
                await un.ExtractArchiveAsync(ModFileManager.DownloadDestinationFilePath, ModFileManager.ArchiveContentFolderPath, ModFileManager.ModArchiveFormat);

                //Setting extract UI
                mli.ModStatusValue = "Moving files";

                //Moving files
                await ModFileManager.MoveDownload();

                //Cleanup
                ModFileManager.ClearDownloadContents();

                //Getting Screenshot from Gamebanana
                await APIRequest.GetScreenshot(ModFileManager.APIType, ModFileManager.ModID, game.ID.ToString(), Mod.TypeID.ToString(), Mod.ID.ToString());


                //Providing mod to ModListElement and showing info
                mli.SetMod(newmod);
                mli.Downloaded = true;

                CollectionViewSource cvs = (CollectionViewSource)this.Resources["CollectionOMods"];
                cvs.View.Refresh();

                //Scanning Files
                int modIndex = Mods.IndexOf(Mod);
                if(modIndex == -1)
                {
                    Mods[Mods.IndexOf(newmod)].FinishedProcessing = FirstScanLibraryMod(newmod, game, InternalModTypes);
                }
                else
                {
                    Mods[modIndex].FinishedProcessing = FirstScanLibraryMod(newmod, game, InternalModTypes);
                }

                //Refreshing  Interface
                mli.Operation = false;
                

                //Saving XML
                WriteModListFile(Mods);

                //Removing mod from Working List
                WorkingModList.Remove(Mod);
            }
        }

        public void QuasarDownloadCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (string quasari in DLS.List)
            {
                Dispatcher.BeginInvoke((Action)(() => { LaunchDownload(quasari); }));
            }
        }


        #endregion

        
    }

    
}
