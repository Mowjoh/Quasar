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

namespace Quasar
{
    public partial class MainWindow : Window
    {
        //Loading references
        public List<LibraryMod> Mods;
        List<Game> Games { get; set; }
        Game CurrentGame { get; set; }
        List<InternalModType> InternalModTypes { get; set; }
        List<GameData> GameData { get; set; }
        List<ContentMapping> ContentMappings { get; set; }

        //Working references
        public List<ModListElement> ListMods { get; set; }
        public List<LibraryMod> WorkingModList;
        public LibraryMod SelectedMod { get; set; }

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

            if (Update)
            {
                Folderino.UpdateBaseFiles();
            }

            //Setting language
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);

            //Aww, here we go again
            InitializeComponent();

            

            //Loading things
            LoadBasicLists();
            LoadLibraryMods();

            readytoSelect = true;

        }

        #region XML LOAD
        //Load Mod Library into memory
        private void LoadLibraryMods()
        {
            Mods = GetLibraryModList();
            ListMods = new List<ModListElement>();
            WorkingModList = new List<LibraryMod>();

            foreach (LibraryMod x in Mods)
            {
                Game gamu = Games.Find(g => g.ID == x.GameID);

                ModListElement mle = new ModListElement();
                mle.Title.Content = x.Name;
                mle.ModType.Content = x.TypeLabel;
                mle.ModCategory.Content = x.APICategoryName;
                mle.Progress.Visibility = Visibility.Hidden;
                mle.SetMod(x);
                mle.setGame(gamu);
                mle.Downloaded = true;
                ListMods.Add(mle);
            }
            ModListView.ItemsSource = ListMods;

        }

        private void LoadBasicLists()
        {
            Games = XML.GetGames();
            GamesListView.ItemsSource = Games;
            InternalModTypes = XML.GetInternalModTypes();
            GameData = XML.GetGameData();
            ContentMappings = new List<ContentMapping>();
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
                CurrentGame = selectedGame;
                ModTypeSelect.ItemsSource = CurrentGame.GameModTypes;
                FilterList(-1, -1, CurrentGame.ID);
                ShowBasicFilters(CurrentGame);

                ModInfoStackPanelValues.Children.Clear();
                VersionStackPanel.Children.Clear();
                ModFileView.Items.Clear();
                ModImage.Source = null;


                List<InternalModType> internalModTypes = InternalModTypes.FindAll(imt => imt.GameID == selectedGame.ID);
                InternalModTypeSelect.ItemsSource = internalModTypes;

                List<GameDataCategory> gameDataCategories = GameData.Find(g => g.GameID == selectedGame.ID).Categories;
                IMTAssotiationSelect.ItemsSource = gameDataCategories;

                if (CurrentGame.ID == -1)
                {
                    IMTGameBlock.Visibility = Visibility.Visible;
                    AssignationGameBlock.Visibility = Visibility.Visible;
                    BuilderGameBlock.Visibility = Visibility.Visible;
                    CreationGameBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    IMTGameBlock.Visibility = Visibility.Hidden;
                    AssignationGameBlock.Visibility = Visibility.Hidden;
                    BuilderGameBlock.Visibility = Visibility.Hidden;
                    CreationGameBlock.Visibility = Visibility.Hidden;
                }

                ReturnAnimation.Begin();
            }

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentGame != null)
            {
                GamesListView.SelectedItem = GamesListView.Items[GamesListView.Items.IndexOf(CurrentGame)];
            }
            readytoSelect = true;
        }
        #endregion

        #region ModManagements
        //Version actions
        private async void CheckUpdates(object sender, RoutedEventArgs e)
        {
            ModListElement element = (ModListElement)ModListView.SelectedItem;
            if (element != null)
            {
                //Getting local Mod
                LibraryMod mod = Mods.Find(mm => mm.ID == element.modID && mm.TypeID == element.modType);
                Game game = Games.Find(g => g.ID == mod.GameID);
                GameModType mt = game.GameModTypes.Find(g => g.ID == mod.TypeID);
                //Parsing mod info from API
                APIMod newAPIMod = await APIRequest.GetAPIMod(mt.APIName, element.modID.ToString());

                //Create Mod from API information
                LibraryMod newmod = GetLibraryMod(newAPIMod, game);

                if (mod.Updates < newmod.Updates)
                {
                    string[] newDL = await APIRequest.GetDownloadFileName(mt.APIName, element.modID.ToString());
                    string quasarURL = APIRequest.GetQuasarDownloadURL(newDL[0], newDL[1], mt.APIName, element.modID.ToString());
                    LaunchDownload(quasarURL);
                }
            }
        }

        //Refreshes the contents of the filter combobox
        public void PrintModInformation(LibraryMod _item)
        {
            //Thrashing the place
            ModInfoStackPanelValues.Children.Clear();
            VersionStackPanel.Children.Clear();

            //Showing Name, Category and Authors
            ModInfoStackPanelValues.Children.Add(new Label() { Content = _item.Name });
            ModInfoStackPanelValues.Children.Add(new Label() { Content = _item.TypeLabel });
            ModInfoStackPanelValues.Children.Add(new Label() { Content = _item.APICategoryName });
            foreach (String[] author in _item.Authors)
            {
                ModInfoStackPanelValues.Children.Add(new Label() { Content = " - " + author[0] });
            }

            //Showing Version info
            VersionStackPanel.Children.Add(new Label() { Content = _item.Updates });
            VersionStackPanel.Children.Add(new Label() { Content = "Up to Date" });

            Game modGame = Games.Find(g => g.ID == _item.GameID);

            //Loading Tree View
            LoadTreeView(ModFileView, new ModFileManager(_item, modGame).LibraryContentFolderPath);
            LoadImage(_item);
        }
        //Refreshes the contents of the File Mod Tree View
        
        private void ModSelected(object sender, SelectionChangedEventArgs e)
        {
            ModListElement mle = (ModListElement)ModListView.SelectedItem;
            SelectedMod = mle.LocalMod;
            if (mle.Downloaded)
            {
                PrintModInformation(mle.LocalMod);
            }

        }
        //Refreshes the content of the mod list based on mod type
        private void ModTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            GameModType selectedType = (GameModType)comboBox.SelectedItem;
            if(selectedType != null)
            {
                FilterList(selectedType.ID, -1, CurrentGame.ID);
                ShowAdvancedFilters(selectedType);
            }
            else
            {
                FilterList(-1, -1, CurrentGame.ID);
                ModFilterSelect.ItemsSource = null;
            }
            
        }

        //Refreshes the content of the mod list based on mod type and mod category
        private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Category selectedItem = (Category)comboBox.SelectedItem;
            GameModType selectedModType = (GameModType)ModTypeSelect.SelectedItem;

            if(selectedModType != null)
            {
                if (selectedItem != null)
                {
                    FilterList(selectedModType.ID, selectedItem.ID, CurrentGame.ID);
                }
                else
                {
                    FilterList(selectedModType.ID, -1, -1);
                }
            }
            else
            {
                FilterList(-1, -1, -1);
            }
            


        }

        private void ShowBasicFilters(Game _Game)
        {
            ModTypeSelect.ItemsSource = null;
            if (_Game.GameModTypes.Count > 0)
            {
                ModTypeSelect.ItemsSource = _Game.GameModTypes;
            }
        }

        //Fill Category filter list
        private void ShowAdvancedFilters(GameModType modType)
        {
            ModFilterSelect.ItemsSource = null;
            if (modType.Categories.Count > 0)
            {
                ModFilterSelect.ItemsSource = modType.Categories;
            }
        }

        //Shows items according to filters
        private void FilterList(int _modType, int modCategory, int _modGame)
        {
            foreach (ModListElement mle in ModListView.Items)
            {
                bool AnyModType = _modType == -1;
                bool AnyCategory = modCategory == -1;
                bool AnyGame = _modGame == -1;

                if((AnyGame || mle.gameID == _modGame) && (AnyCategory || mle.modCategory == modCategory) && (AnyModType || mle.modType == _modType))
                {
                    mle.isActive = true;
                }
                else
                {
                    mle.isActive = false;
                }
            }
            ModListView.Items.Refresh();
        }

        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem { Header = directoryInfo.Name };
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Items.Add(CreateDirectoryNode(directory));

            foreach (var file in directoryInfo.GetFiles())
                directoryNode.Items.Add(new TreeViewItem { Header = file.Name });

            return directoryNode;
        }

        //Tree view actions
        private void ExpandTree(object sender, RoutedEventArgs e)
        {
            ExpandTree(ModFileView);

        }

        private void MinimizeTree(object sender, RoutedEventArgs e)
        {
            MinimizeTree(ModFileView);
        }

        private void LoadImage(LibraryMod libraryMod)
        {
            string imageSource = Properties.Settings.Default.DefaultDir + @"\Library\Screenshots\" + libraryMod.GameID + "_" + libraryMod.TypeID + "_" + libraryMod.ID + ".jpg";
            if (File.Exists(imageSource))
            {
                ModImage.Source = new BitmapImage(new Uri(imageSource, UriKind.RelativeOrAbsolute));
            }
            else
            {
                ModImage.Source = null;
            }
        }
        #endregion

        #region Mod Content
        private void ContentDataGridItemSelected(object sender, SelectionChangedEventArgs e)
        {
        }

        private void AutoDetectLaunch(object sender, RoutedEventArgs e)
        {
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

        #region Global
        private void ExpandTree(TreeView tv)
        {
            foreach (var item in tv.Items)
            {
                if (item is TreeViewItem tvi)
                {
                    tvi.ExpandSubtree();
                }
            }
        }

        private void MinimizeTree(TreeView tv)
        {
            foreach (var item in tv.Items)
            {
                if (item is TreeViewItem tvi)
                {
                    tvi.IsExpanded = false;
                }
            }
        }

        public void LoadTreeView(System.Windows.Controls.TreeView _tv, string _fp)
        {
            _tv.Items.Clear();

            foreach (string s in Directory.GetDirectories(_fp))
            {
                var rootDirectory = new DirectoryInfo(s);
                _tv.Items.Add(CreateDirectoryNode(rootDirectory));
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
            ModListElement mle = new ModListElement();
            
            //Setting base ModFileManager
            ModFileManager ModFileManager = new ModFileManager(_URL);

            //Parsing mod info from API
            APIMod newAPIMod = await APIRequest.GetAPIMod(ModFileManager.APIType, ModFileManager.ModID);

            //Finding related game
            Game game = Games.Find(g => g.GameName == newAPIMod.GameName);

            //Resetting ModFileManager based on new info
            ModFileManager = new ModFileManager(_URL, game);

            //Setting game UI
            mle.setGame(game);

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
                    mle = ListMods.Find(ml => ml.LocalMod == Mod);
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
                }
                else
                {
                    //Updating List
                    Mods[Mods.IndexOf(Mod)] = newmod;
                }

                
                if (newElement)
                {
                    //Creating interface element
                    ListMods.Add(mle);
                    ModListView.Items.Refresh();
                }

                //Setting download UI
                mle.Title.Content = downloadText;

                Downloader modDownloader = new Downloader(mle.Progress, mle.Status, mle.ModType);

                //Wait for download completion
                await modDownloader.DownloadArchiveAsync(ModFileManager);

                //Setting extract UI
                mle.Title.Content = "Extracting mod";

                //Preparing Extraction
                Unarchiver un = new Unarchiver(mle.Progress, mle.Status);

                //Wait for Archive extraction
                await un.ExtractArchiveAsync(ModFileManager.DownloadDestinationFilePath, ModFileManager.ArchiveContentFolderPath, ModFileManager.ModArchiveFormat);

                //Setting extract UI
                mle.Title.Content = "Moving files";

                //Moving files
                await ModFileManager.MoveDownload();

                //Cleanup
                ModFileManager.ClearDownloadContents();

                //Providing mod to ModListElement and showing info
                mle.SetMod(newmod);
                mle.Downloaded = true;

                //Refresh UI element
                mle.RefreshInterface();
                ModListView.SelectedItem = mle;

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
