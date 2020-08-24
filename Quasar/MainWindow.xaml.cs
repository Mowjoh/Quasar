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
using static Quasar.XMLResources.AssociationXML;
using System.Windows.Input;
using Point = System.Windows.Point;
using Quasar.Internal.FileSystem;
using FluentFTP;
using System.Text.RegularExpressions;
using System.Net;

namespace Quasar
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        protected bool m_IsDraging = false;
        protected Point _dragStartPoint;

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

        #region ModLibrary

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
        #endregion

        #region API Library

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
        #endregion

        #region Content Library

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

        private ObservableCollection<ContentMapping> _AssociationContentMappings;
        public ObservableCollection<ContentMapping> AssociationContentMappings
        {
            get
            {
                return _AssociationContentMappings;
            }
            set
            {
                _AssociationContentMappings = value;
                OnPropertyChanged("AssociationContentMappings");
            }
        }

        private ObservableCollection<ContentMapping> _AssociationSlots;
        public ObservableCollection<ContentMapping> AssociationSlots
        {
            get
            {
                return _AssociationSlots;
            }
            set
            {
                _AssociationSlots = value;
                OnPropertyChanged("AssociationSlots");
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

        #endregion

        #region Associations
        private List<Workspace> _Workspaces;
        public List<Workspace> Workspaces
        {
            get
            {
                return _Workspaces;
            }
            set
            {
                _Workspaces = value;
                OnPropertyChanged("Workspaces");
            }
        }

        private Workspace _CurrentWorkspace;
        public Workspace CurrentWorkspace
        {
            get
            {
                return _CurrentWorkspace;
            }
            set
            {
                _CurrentWorkspace = value;
                OnPropertyChanged("CurrentWorkspace");
            }
        }
        #endregion

        #region Build
        private List<GameBuilder> _GameBuilders;
        public List<GameBuilder> GameBuilders
        {
            get => _GameBuilders;
            set
            {
                _GameBuilders = value;
                OnPropertyChanged("GameBuilders");
            }
        }

        private ObservableCollection<DriveInfo> _USBDrives;
        public ObservableCollection<DriveInfo> USBDrives
        {
            get
            {
                return _USBDrives;
            }
            set
            {
                _USBDrives = value;
                OnPropertyChanged("USBDrives");
            }
        }

        private ObservableCollection<string> _USBDriveLabels;
        public ObservableCollection<string> USBDriveLabels
        {
            get
            {
                return _USBDriveLabels;
            }
            set
            {
                _USBDriveLabels = value;
                OnPropertyChanged("USBDriveLabels");
            }
        }
        #endregion

        #region Game Data

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

        private GameDataCategory _IMTCurrentGDC;
        public GameDataCategory IMTCurrentGDC
        {
            get
            {
                return _IMTCurrentGDC;
            }
            set
            {
                _IMTCurrentGDC = value;
                OnPropertyChanged("IMTCurrentGDC");
            }
        }

        private GameDataCategory _AssociationCurrentGDC;
        public GameDataCategory AssociationCurrentGDC
        {
            get
            {
                return _AssociationCurrentGDC;
            }
            set
            {
                _AssociationCurrentGDC = value;
                OnPropertyChanged("AssociationCurrentGDC");
            }
        }
        #endregion

        #region Internal Mod Types
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

        private InternalModType _ContentCurrentIMT;
        public InternalModType ContentCurrentIMT
        {
            get
            {
                return _ContentCurrentIMT;
            }
            set
            {
                _ContentCurrentIMT = value;
                OnPropertyChanged("CurrentIMT");
            }
        }

        private ObservableCollection<InternalModType> _AssociationCorrespondingIMT;

        public ObservableCollection<InternalModType> AssociationCorrespondingIMT
        {
            get
            {
                return _AssociationCorrespondingIMT;
            }
            set
            {
                _AssociationCorrespondingIMT = value;
                OnPropertyChanged("AssociationCorrespondingIMT");
            }
        }
        #endregion

        #region Settings
        private ObservableCollection<QuasarSetting> _SettingsList;
        public ObservableCollection<QuasarSetting> SettingsList
        {
            get
            {
                return _SettingsList;
            }
            set
            {
                _SettingsList = value;
                OnPropertyChanged("SettingsList");
            }
        }
        #endregion

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

        private void ShowOnlyRelatedAssociation(object sender, FilterEventArgs e)
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

        #region Sorting

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
            bool Debug = true;

            if (Update || Debug)
            {
                Folderino.UpdateBaseFiles();
            }

            Checker.BaseWorkspace();

            //Setting language
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);

            //Aww, here we go again
            InitializeComponent();

            //Loading things
            LoadBasicLists();
            LoadModLibrary();
            LoadContentLibrary();
            LoadAssociationsLibrary();


            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = ListMods;
            cvs.Filter += ShowOnlyNonFilteredMods;


            SetInterfaceWithParams();

            readytoSelect = true;
            BuilderLogs.Text += "Please note that for both mod loaders I assume you have them installed.\r\n";
            BuilderLogs.Text += "For ARCropolis, that means you have copied the files from the release to your SD\r\n";
            BuilderLogs.Text += "For Ultimate Mod Manager, that means you have copied the homebrew and your dump is made.\r\n";
            BuilderLogs.Text += "Thanks for reading, I'll try to make it so you don't have to copy anything later on.\r\n \r\n";

            BuilderModLoaderCombo.SelectedItem = 1;

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

        private void LoadAssociationsLibrary()
        {
            Workspaces = AssociationXML.GetWorkspaces(); 
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

        private ObservableCollection<Association> LoadAssociations()
        {
            ObservableCollection<Association> newAssociationsList = new ObservableCollection<Association>();

            return newAssociationsList;
        }

        private void LoadBasicLists()
        {
            Games = XML.GetGames();
            InternalModTypes = XML.GetInternalModTypes();
            GameData = XML.GetGameData();
            GameBuilders = XML.GetBuilders();
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

                Properties.Settings.Default.LastSelectedGame = selectedGame.ID;
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
            if (CurrentGame.ID != -1)
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
            if (mli != null)
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
            Boolean proceed = false;
            if (!Properties.Settings.Default.SupressModDeletion)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this mod ?", "Mod Deletion", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        proceed = true;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            

            if (proceed || Properties.Settings.Default.SupressModDeletion)
            {
                ModListItem item = (ModListItem)sender;

                //Removing from ContentMappings
                List<ContentMapping> relatedMappings = ContentMappings.FindAll(cm => cm.ModID == item.LocalMod.ID);
                foreach (ContentMapping cm in relatedMappings)
                {
                    List<Association> associations = CurrentWorkspace.Associations.FindAll(ass => ass.ContentMappingID == cm.ID);
                    if (associations != null)
                    {
                        foreach (Association ass in associations)
                        {
                            CurrentWorkspace.Associations.Remove(ass);
                        }
                    }
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
                SaveWorkspaces();
            }
        }


        //--------------------------------------
        //Filtering Actions

        //Refreshes the content of the mod list based on mod type
        private void ModTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentGameApiModType = (GameModType)ManagementAPITypeSelect.SelectedItem;
            FilterModList(CurrentGameApiModType != null ? CurrentGameApiModType.ID : -1, -1, CurrentGame.ID);
            CollectionViewSource cvs = (CollectionViewSource)this.Resources["ModManagementFilterCollection"];
            cvs.View.Refresh();
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

                if ((AnyGame || mle.Game.ID == _modGame) && (AnyCategory || mle.LocalMod.APICategoryID == modCategory) && (AnyModType || mle.LocalMod.TypeID == _modType))
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
            ContentCurrentIMT = (InternalModType)ContentIMTSelect.SelectedItem;
            if (ContentCurrentIMT != null)
            {
                IMTCurrentGDC = GameDataCategories.Single(gdc => gdc.ID == ContentCurrentIMT.Association);
            }

            FilterContentList(ContentCurrentIMT != null ? ContentCurrentIMT.ID : -1, -1, CurrentGame.ID);

        }

        private void IMTGDCItemSelect(object sender, SelectionChangedEventArgs e)
        {
            GameDataItem CurrentGDI = (GameDataItem)ContentGDCSelect.SelectedItem;
            FilterContentList(ContentCurrentIMT != null ? ContentCurrentIMT.ID : -1, CurrentGDI != null ? CurrentGDI.ID : -1, CurrentGame.ID);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ContentGDCSelect.SelectedIndex = -1;
            ContentIMTSelect.SelectedIndex = -1;
        }

        private void FilterContentList(int _IMT, int _GDC, int _Game)
        {
            foreach (ContentListItem cli in ListContents)
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

                CollectionViewSource cvs = (CollectionViewSource)this.Resources["CollectionOContents"];
                cvs.View.Refresh();
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
            AssociationCorrespondingIMT = new ObservableCollection<InternalModType>();

            GameDataCategory gdc = (GameDataCategory)AssociationGameDataList.SelectedItem;
            if (gdc != null)
            {
                AssociationCurrentGDC = gdc;
                List<InternalModType> correspondingTypes = InternalModTypes.FindAll(imt => imt.Association == gdc.ID);
                foreach (InternalModType imt in correspondingTypes)
                {
                    AssociationCorrespondingIMT.Add(imt);
                }

            }
        }
        private void AssociationGameElementChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSlots();
        }
        private void AssociationIMTChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSlots();
        }

        private void FilterSlots()
        {
            
            AssociationContentMappings = new ObservableCollection<ContentMapping>();
            AssociationSlots = new ObservableCollection<ContentMapping>();
            GameDataItem SelectedCategory = (GameDataItem)AssociationGameElementDataList.SelectedItem;
            InternalModType SelectedIMT = (InternalModType)AssociationTypeDataList.SelectedItem;

            if (SelectedIMT != null)
            {
                int AnyCategory = SelectedCategory == null ? -1 : SelectedCategory.ID;

                List<ContentMapping> relatedMappings = ContentMappings.FindAll(cm => cm.GameDataItemID == AnyCategory && cm.InternalModType == SelectedIMT.ID);
                for (int i = 0; i < SelectedIMT.Slots; i++)
                {
                    AssociationSlots.Add(new ContentMapping() { Name = "Empty Slot n°" + (i + 1), SlotName = "FakeIMT" + (i + 1) });
                    
                }
                foreach (ContentMapping cm in relatedMappings)
                {
                    AssociationContentMappings.Add(cm);
                    List<Association> Slots = CurrentWorkspace.Associations.FindAll(asso => asso.ContentMappingID == cm.ID);
                    if(Slots != null)
                    {
                        foreach(Association ass in Slots)
                        {
                            setSlot(cm, ass.Slot);
                        }
                    }
                }
            }
        }

        private void setSlot(int indexSource, int indexDestination)
        {
            if(AssociationSlots.Count > 0)
            {
                ContentMapping DestinationMapping = AssociationContentMappings.ElementAt(indexSource);
                DestinationMapping.Slot = indexDestination;

                ContentMapping SourceMapping = (ContentMapping)ItemSlotListBox.Items.GetItemAt(indexDestination);
                if (!SourceMapping.SlotName.Substring(0, 7).Equals("FakeIMT"))
                {
                    List<Association> asso = CurrentWorkspace.Associations.FindAll(a=> a.ContentMappingID == SourceMapping.ID && a.Slot == SourceMapping.Slot);
                    foreach(Association a in asso)
                    {
                        CurrentWorkspace.Associations.Remove(a);
                    }
                }

                AssociationSlots.RemoveAt(indexDestination);
                AssociationSlots.Insert(indexDestination, DestinationMapping);

                saveSlots();
            }
        }

        private void setSlot(ContentMapping input, int indexDestination)
        {
            if (AssociationSlots.Count > 0)
            {
                if(indexDestination < AssociationSlots.Count)
                {
                    AssociationSlots.RemoveAt(indexDestination);
                    input.Slot = indexDestination;
                    AssociationSlots.Insert(indexDestination, input);
                }
                
            }
            
        }

        private void saveSlots()
        {
            foreach(ContentMapping cm in AssociationSlots)
            {
                if (!cm.SlotName.Substring(0, 7).Equals("FakeIMT"))
                {
                    Association aa = CurrentWorkspace.Associations.Find(a => a.ContentMappingID == cm.ID && a.Slot == cm.Slot);
                    if(aa == null)
                    {
                        Association asso = new Association() { ContentMappingID = cm.ID, Slot = cm.Slot, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType };
                        CurrentWorkspace.Associations.Add(asso);
                    }
                }
            }

            SaveWorkspaces();
        }

        private void ResetSlotsButton_Click(object sender, RoutedEventArgs e)
        {
            GameDataItem SelectedCategory = (GameDataItem)AssociationGameElementDataList.SelectedItem;
            InternalModType SelectedIMT = (InternalModType)AssociationTypeDataList.SelectedItem;

            if (SelectedIMT != null)
            {
                int AnyCategory = SelectedCategory == null ? -1 : SelectedCategory.ID;
                List<ContentMapping> relatedMappings = ContentMappings.FindAll(cm => cm.GameDataItemID == AnyCategory && cm.InternalModType == SelectedIMT.ID);
                for (int i = 0; i < SelectedIMT.Slots; i++)
                {
                    AssociationSlots.Add(new ContentMapping() { Name = "Empty Slot n°" + (i + 1), SlotName = "FakeIMT" + (i + 1) });

                }
                foreach (ContentMapping cm in relatedMappings)
                {
                    AssociationContentMappings.Add(cm);
                    List<Association> Slots = CurrentWorkspace.Associations.FindAll(asso => asso.ContentMappingID == cm.ID);
                    if (Slots != null)
                    {
                        foreach (Association ass in Slots)
                        {
                            CurrentWorkspace.Associations.Remove(ass);
                        }
                    }
                }
            }


            SaveWorkspaces();
            FilterSlots();
        }

        #region Drag Drop
        private void ItemSourceListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void ItemSourceListBox_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void ItemSourceListBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point point = e.GetPosition(null);
            Vector diff = _dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var lb = sender as ListBox;
                var lbi = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));
                if (lbi != null)
                {
                    DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
                }
            }
        }

        private void ItemSlotListBox_Drop(object sender, DragEventArgs e)
        {
            if (sender is Grid)
            {
                var source = e.Data.GetData(typeof(ContentMapping)) as ContentMapping;
                var target = (Grid)sender;
                Label l = (Label)target.Children[0];
                ContentMapping cm = AssociationSlots.Where(c => c.SlotName == l.Content.ToString()).ElementAt(0);

                int sourceIndex = ItemSourceListBox.Items.IndexOf(source);
                int targetIndex = ItemSlotListBox.Items.IndexOf(cm);

                setSlot(sourceIndex, targetIndex);
            }
        }

        private T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            return FindVisualParent<T>(parentObject);
        }
        #endregion


        #endregion

        #region InternalModTypes
        //State changes
        private void InternalModTypeSelected(object sender, SelectionChangedEventArgs e)
        {
            //Getting info
            GameData gameData = GameData.Find(g => g.GameID == CurrentGame.ID);
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            IMTDataGrid.ItemsSource = type.Files;
            GameDataCategory cat = gameData.Categories.Find(c => c.ID == type.Association);

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
                IMTPathText.Text = file.SourcePath;
                IMTFileText.Text = file.SourceFile;
                //IMTDestinationText.Text = file.Destination;
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

        private void IMTGameBuilderSelected(object sender, SelectionChangedEventArgs e)
        {
            if (IMTGameBuilderCombo.SelectedIndex == -1)
            {
                IMTBuilderOutputFilePath.IsEnabled = false;
                IMTBuilderOutputFilePath.Text = "";

                IMTBuilderOutputFolderPath.IsEnabled = false;
                IMTBuilderOutputFolderPath.Text = "";
            }
            else
            {
                IMTBuilderOutputFilePath.IsEnabled = true;
                IMTBuilderOutputFolderPath.IsEnabled = true;
                GameBuilder GB = (GameBuilder)IMTGameBuilderCombo.SelectedItem;
                InternalModTypeFile file = (InternalModTypeFile)IMTDataGrid.SelectedItem;
                
                BuilderFolder BFol = file.Destinations.Find(f => f.BuilderID == GB.ID);
                BuilderFile BFil = file.Files.Find(f => f.BuilderID == GB.ID);

                IMTBuilderOutputFilePath.Text = BFil.Path;
                IMTBuilderOutputFolderPath.Text = BFol.Path;

            }

        }

        //IMT Actions
        private void IMTAddFile(object sender, RoutedEventArgs e)
        {
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            if (type != null)
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
                file.SourcePath = IMTPathText.Text;
                file.SourceFile = IMTFileText.Text;
                //file.Destination = IMTDestinationText.Text;
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
                foreach (InternalModTypeFile file in type.Files)
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

        #region Build
        public void getSDCards()
        {
            DriveInfo[] CurrentDrives = DriveInfo.GetDrives();
            USBDrives = new ObservableCollection<DriveInfo>();

            foreach(DriveInfo di in CurrentDrives)
            {
                if(di.DriveType == DriveType.Removable && di.IsReady)
                {
                    USBDrives.Add(di);
                }
            }
            getSDLabels();
        }

        public void getSDLabels()
        {
            USBDriveLabels = new ObservableCollection<string>();
            foreach (DriveInfo di in USBDrives)
            {
                USBDriveLabels.Add(di.VolumeLabel + " {" + di.Name + "}");
            }
        }

        private async void Build_Button(object sender, RoutedEventArgs e)
        {
            bool willrun = true;
            string address = BuildFTPAddress.Text;
            string port = BuildFTPPort.Text;

            //Checking ModLoader
            if (BuilderModLoaderCombo.SelectedIndex == -1)
            {
                BuilderLogs.Text += "Please select a modloader first\r\n";
                willrun = false;
            }

            //Checking FTP
            if (BuilderFTPRadio.IsChecked == true)
            {
                if (!validateIP() || !validatePort())
                {
                    willrun = false;
                }
            }

            //Checking Local Transfer
            if (BuilderSDCombo.SelectedIndex == -1 && BuilderLocalRadio.IsChecked == true)
            {
                BuilderLogs.Text += "Please select a SD Drive first\r\n";
                willrun = false;
            }
            if (willrun)
            {
                BuilderBuild.IsEnabled = false;
                BuilderFTPTest.IsEnabled = false;
                Boolean proceed = false;
                if (!Properties.Settings.Default.SupressBuildDeletion)
                {
                    MessageBoxResult result = MessageBox.Show("You are about to build the workspace. This will wipe your mod folders on your Switch to avoid conflicts. Do you wish to proceed with the build process?", "File Deletion Warning", MessageBoxButton.YesNo);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            proceed = true;
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                }
                if (proceed || Properties.Settings.Default.SupressBuildDeletion)
                {
                    string pathname = BuilderSDCombo.SelectedIndex == -1 ? "" : USBDrives[BuilderSDCombo.SelectedIndex].Name;
                    string ftpPath = address + ":" + port;
                    NetworkCredential NC = null;
                    if (BuildPWRadio.IsChecked == true)
                    {
                        NC = new NetworkCredential(BuildFTPUN.Text, BuildFTPPW.Text);
                        BuilderProgress.IsIndeterminate = true;
                    }
                    if (BuilderLocalRadio.IsChecked == true)
                    {
                        ftpPath = "";
                    }

                    await Builder.SmashBuild(pathname, BuilderModLoaderCombo.SelectedIndex, ftpPath, NC, BuilderWipeCreateRadio.IsChecked == true ? 1 : -1, Mods, ContentMappings, CurrentWorkspace, InternalModTypes, CurrentGame, GameData, BuilderLogs, BuilderProgress);
                    BuilderProgress.Value = 100;
                    BuilderLogs.Text += "Done\r\n";
                    BuilderBuild.IsEnabled = true;
                    BuilderFTPTest.IsEnabled = true;
                }

            }
        }

        private void PrepareModLoaders(object sender, RoutedEventArgs e)
        {
            string source = Properties.Settings.Default.AppPath + @"\References\ModLoaders\";
            string destination = @"I:\atmosphere\contents\01006A800016E000\romfs\skyline\plugins\libarcropolis.nro";
            /*if (BuilderVerboseRadio.IsChecked == true)
            {
                Folderino.CheckCopyFile(source + "verbose_libarcropolis.nro", destination);
                BuilderLogs.Text += "ARCRopolis set to Verbose Mode \r\n";
            }
            else
            {
                Folderino.CheckCopyFile(source + "silent_libarcropolis.nro", destination);
                BuilderLogs.Text += "ARCRopolis set to Silent Mode \r\n";
            }*/

        }

        private void AutoEjectSD()
        {

        }

        private void BuilderDriveRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            getSDCards();
        }

        private async void FTP_Test(object sender, RoutedEventArgs e)
        {
            BuilderBuild.IsEnabled = false;
            BuilderFTPTest.IsEnabled = false;
            BuilderProgress.IsIndeterminate = true;
            string address = BuildFTPAddress.Text;
            string port = BuildFTPPort.Text;

            bool ShouldConnect = true;
            if (validateIP() && validatePort())
            {
                String errortext = "";
                BuilderLogs.Text += "Please Wait... \r\n";
                FtpClient client = new FtpClient(address);
                if (BuildPWRadio.IsChecked == true)
                {
                    client.Credentials = new NetworkCredential(BuildFTPUN.Text, BuildFTPPW.Text);
                }
                

                client.Port = Int32.Parse(port);
                try
                {
                    await client.ConnectAsync();
                }
                catch(Exception ex)
                {
                    errortext = ex.Message;
                }
                

                if (client.IsConnected)
                {
                    BuilderLogs.Text += "FTP Connection Successful \r\n";
                }
                else
                {
                    BuilderLogs.Text += "FTP Connection Unsuccessful "+errortext+" \r\n";
                }
            }
            BuilderProgress.IsIndeterminate = false;
            BuilderBuild.IsEnabled = true;
            BuilderFTPTest.IsEnabled = true;

        }

        private bool validateIP()
        {
            string address = BuildFTPAddress.Text;
            Regex IP = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");

            bool result = IP.IsMatch(address);
            if (!result)
            {
                BuilderLogs.Text += "Please enter a valid IP\r\n";
            }

            return result;
        }

        private bool validatePort()
        {
            string port = BuildFTPPort.Text;
            int val;

            bool result = Int32.TryParse(port, out val);

            if (result)
            {
                result = val > 0 && val < 70000;
            }

            if (!result)
            {
                BuilderLogs.Text += "Please enter a valid Port\r\n";
            }
            return result;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

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

            Workspace SelectedWorkspace = Workspaces.Find(w => w.ID == Properties.Settings.Default.LastSelectedWorkspace);
            CurrentWorkspace = SelectedWorkspace;

            SettingsList = new ObservableCollection<QuasarSetting>();

            List<QuasarSettingComboData> list = new List<QuasarSettingComboData>
            {
                new QuasarSettingComboData() { Name = "French", Value = "FR" },
                new QuasarSettingComboData() { Name = "English", Value = "EN" }
            };

            List<QuasarSettingComboData> WorkspaceList = new List<QuasarSettingComboData>();
            foreach (Workspace w in Workspaces)
            {
                WorkspaceList.Add(new QuasarSettingComboData() { Name = w.Name, Value = w.ID.ToString()});
            }

            SettingsList.Add(new QuasarSetting(new QuasarSettingData() { SettingName = "Quasar Version :",SettingValue= Properties.Settings.Default.AppVersion, NameOnly = true }));
            SettingsList.Add(new QuasarSetting(new QuasarSettingData() { SettingName = "Supress Mod ", SettingValue= "deletion warning", SettingCheck = Properties.Settings.Default.SupressModDeletion, Reference = "SupressModDeletion" }));
            SettingsList.Add(new QuasarSetting(new QuasarSettingData() { SettingName = "Supress Build", SettingValue = "deletion warning", SettingCheck = Properties.Settings.Default.SupressBuildDeletion, Reference = "SupressBuildDeletion" }));
            SettingsList.Add(new QuasarSetting(new QuasarSettingData() { SettingName = "Enable", SettingValue = "Internal Mod Types", SettingCheck = Properties.Settings.Default.EnableIMT, Reference = "EnableIMT" }));

            foreach(QuasarSetting QS in SettingsList)
            {
                QS.SettingsChanged += SettingsChanged;
            }

            BuilderModLoaderCombo.SelectedIndex = 0;
            BuilderWorkspaceCombo.SelectedIndex = 0;
            if (Properties.Settings.Default.EnableIMT)
            {
                IMTTab.Visibility = Visibility.Visible;
            }
            else
            {
                IMTTab.Visibility = Visibility.Collapsed;
                
            }
            getSDCards();

        }

        public void SettingsChanged(object sender, EventArgs e)
        {
            QuasarSetting sen = (QuasarSetting)sender;
            if(sen.LocalData.Reference != null)
            {
                if(sen.LocalData.Reference == "EnableIMT")
                {
                    BuilderWorkspaceCombo.SelectedIndex = 0;
                    if (Properties.Settings.Default.EnableIMT)
                    {
                        IMTTab.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        IMTTab.Visibility = Visibility.Collapsed;

                    }
                }
            }
        }

        private void SaveWorkspaces()
        {
            Workspace item = Workspaces.Find(w => w.ID == CurrentWorkspace.ID);
            Workspaces[Workspaces.IndexOf(item)] = CurrentWorkspace;
            AssociationXML.WriteAssociationFile(Workspaces);
        }

        #endregion

        #region Detection

        private bool FirstScanLibraryMod(LibraryMod libraryMod, Game game, List<InternalModType> types)
        {
            bool processed = false;
            List<ContentMapping> SearchList = Searchie.AutoDetectinator(libraryMod, types, game, GameData);

            List<ContentMapping> WorkingList = ContentMappings;
            foreach (ContentMapping cm in SearchList)
            {
                WorkingList.Add(cm);
                processed = true;
            }
            ContentMappings = WorkingList;
            AutoSlotDetectedItems(SearchList);
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


        private void AutoSlotDetectedItems(List<ContentMapping> elements)
        {
            foreach(ContentMapping cm in elements)
            {
                if(cm.GameDataItemID != -1)
                {
                    Association associations = CurrentWorkspace.Associations.Find(ass => ass.GameDataItemID == cm.GameDataItemID && ass.InternalModTypeID == cm.InternalModType && ass.Slot == cm.Slot);
                    if (associations != null)
                    {
                        CurrentWorkspace.Associations[CurrentWorkspace.Associations.IndexOf(associations)] = new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot };
                    }
                    else
                    {
                        CurrentWorkspace.Associations.Add(new Association() { ContentMappingID = cm.ID, GameDataItemID = cm.GameDataItemID, InternalModTypeID = cm.InternalModType, Slot = cm.Slot });
                    }
                }
            }
            SaveWorkspaces();
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
                if (Mod.Updates < newmod.Updates)
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
                if (modIndex == -1)
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
