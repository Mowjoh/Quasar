using Quasar.Controls;
using Quasar.Resources;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static Quasar.Library;
using Quasar.Singleton;
using System.Threading;
using System;
using System.Globalization;
using Quasar.File;
using Quasar.Quasar_Sys;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace Quasar
{
    public partial class MainWindow : Window
    {
        //Loading references
        public List<Mod> Mods;
        List<ModType> ModTypes { get; set; }
        List<InternalModType> InternalModTypes { get; set; }
        List<GameDataCategory> GameDataCategories { get; set; }
        List<ContentMapping> contentMappings { get; set; }

        //Working references
        public List<ModListElement> ListMods { get; set; }
        public List<Mod> WorkingModList;
        public Mod SelectedMod { get; set; }

        //Quasar Downloads
        Mutex serverMutex;
        public QuasarDownloads DLS;

        
        public MainWindow()
        {
            //Setting up Server or Client
            DLS = new QuasarDownloads();
            DLS.List.CollectionChanged += QuasarDownloadCollectionChanged;
            serverMutex = Checker.Instances(serverMutex, DLS.List);

            //Pre-run checks
            bool Update = Checker.checkUpdated();
            Folderino.CheckBaseFolders();
            Folderino.CompareResources();
            if (Update)
            {
                Folderino.UpdateBaseFiles();
            }

            //Setting language
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.Language);

            //Aww, here we go again
            InitializeComponent();

            //Loading things
            LoadBasicLists();
            LoadMods();

        }

        #region XML LOAD
        //Load Mod Library into memory
        private void LoadMods()
        {
            Mods = GetModListFile(ModTypes);
            ListMods = new List<ModListElement>();
            WorkingModList = new List<Mod>();

            foreach (Mod x in Mods)
            {
                ModListElement mle = new ModListElement();
                mle.Title.Content = x.Name;
                mle.TypeLabel.Content = x.typeName;
                mle.Category.Content = x.categoryName;
                mle.Progress.Visibility = Visibility.Hidden;
                mle.setMod(x);
                mle.Downloaded = true;
                ListMods.Add(mle);
            }
            ModListView.ItemsSource = ListMods;
            ContentDataGrid.ItemsSource = Mods;

            List<ContentMappingFile> Files = new List<ContentMappingFile>();
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\alp_gaogaen_001_col.nutexb", Path= "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\alp_gaogaen_001_nor.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\alp_gaogaen_001_prm.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\alp_gaogaen_002_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\alp_gaogaen_002_nor.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\alp_gaogaen_002_prm.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\deyes_eye_gaogaen_d_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\deyes_eye_gaogaen_wd_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\eye_gaogaen_b_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\eye_gaogaen_g_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\eye_gaogaen_w_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\eye_gaogaen_w_nor.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\eye_gaogaen_w_prm.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\leyes_eye_gaogaen_l_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\leyes_eye_gaogaen_wl_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\skin_gaogaen_001_col.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\skin_gaogaen_001_prm.nutexb", Path = "*.nutexb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\model.nutatb", Path = "*.nutatb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\model.numdlb", Path = "*.numdlb" });
            Files.Add(new ContentMappingFile() { InternalModTypeFileID = 1, SourcePath = @"Zangief Mod\fighter\gaogaen\model\body\c00\model.numshb", Path = "*.numshb" });
            ContentFileDataGrid.ItemsSource = Files;

        }

        private void LoadBasicLists()
        {
            ModTypes = XML.GetModTypes();
            ModTypeSelect.ItemsSource = ModTypes;

            InternalModTypes = XML.GetInternalModTypes();
            InternalModTypeSelect.ItemsSource = InternalModTypes;

            GameDataCategories = XML.getGameCategories();
            IMTAssotiationSelect.ItemsSource = GameDataCategories;
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
                Mod mod = Mods.Find(mm => mm.id == element.modID && mm.type == element.modType);
                ModType mt = ModTypes.Find(mmt => mmt.ID == mod.type);
                //Parsing mod info from API
                APIMod newAPIMod = await APIRequest.getMod(mt.APIName, element.modID.ToString());

                //Create Mod from API information
                Mod newmod = Parse(newAPIMod, ModTypes);

                if (mod.Updates < newmod.Updates)
                {
                    string[] newDL = await APIRequest.getDownloadFileName(mt.APIName, element.modID.ToString());
                    string quasarURL = APIRequest.getQuasarDownloadURL(newDL[0], newDL[1], mt.APIName, element.modID.ToString());
                    LaunchDownload(quasarURL);
                }
            }
        }

        //Refreshes the contents of the filter combobox
        public void PrintModInformation(Mod _item)
        {
            //Thrashing the place
            ModInfoStackPanelValues.Children.Clear();
            VersionStackPanel.Children.Clear();

            //Showing Name, Category and Authors
            ModInfoStackPanelValues.Children.Add(new Label() { Content = _item.Name });
            ModInfoStackPanelValues.Children.Add(new Label() { Content = _item.typeName });
            ModInfoStackPanelValues.Children.Add(new Label() { Content = _item.categoryName });
            foreach (String[] author in _item.Authors)
            {
                ModInfoStackPanelValues.Children.Add(new Label() { Content = "- " + author[0] });
            }

            //Showing Version info
            VersionStackPanel.Children.Add(new Label() { Content = _item.Updates });
            VersionStackPanel.Children.Add(new Label() { Content = "Up to Date" });

            //Loading Tree View
            LoadTreeView(ModFileView, new ModFileManager(_item, ModTypes).libraryContentPath);
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
            ModType selectedType = (ModType)comboBox.SelectedItem;
            FilterList(selectedType.ID);
            ShowAdvancedFilters(selectedType);
        }

        //Refreshes the content of the mod list based on mod type and mod category
        private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Category selectedItem = (Category)comboBox.SelectedItem;
            ModType selectedModType = (ModType)ModTypeSelect.SelectedItem;
            if (selectedItem != null)
            {
                if (selectedItem.ID == -1)
                {
                    FilterList(selectedModType.ID);
                }
                else
                {
                    FilterList(selectedModType.ID, selectedItem.ID);
                }
            }
            else
            {
                FilterList(selectedModType.ID);
            }


        }

        //Fill Category filter list
        private void ShowAdvancedFilters(ModType modType)
        {
            ModFilterSelect.ItemsSource = null;
            if (modType.Categories.Count > 0)
            {
                ModFilterSelect.ItemsSource = modType.Categories;
            }
        }

        //Shows items according to filters
        private void FilterList(int _modType)
        {
            foreach (ModListElement mle in ModListView.Items)
            {
                if (_modType == -1)
                {
                    mle.Visibility = Visibility.Visible;
                }
                else
                {
                    if (mle.modType != _modType)
                    {
                        mle.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        mle.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void FilterList(int _modType, int modCategory)
        {
            foreach (ModListElement mle in ModListView.Items)
            {
                if (_modType == -1)
                {
                    if (modCategory == -1)
                    {
                        mle.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (mle.modType == _modType && mle.modCategory == modCategory || mle.modType == _modType && mle.modCategory == -1)
                    {
                        mle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        mle.Visibility = Visibility.Collapsed;
                    }
                }
            }
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
        #endregion

        #region Mod Content
        private void ContentDataGridItemSelected(object sender, SelectionChangedEventArgs e)
        {
            Mod m = (Mod)ContentDataGrid.SelectedItem;
            if (m != null)
            {
                if (!m.processed)
                {
                    ContentDetectionProcessLabel.Content = "Awaiting Autodetect";
                    ContentAutoDetectLaunchButton.IsEnabled = true;
                }
                else
                {
                    ContentDetectionProcessLabel.Content = "Autodetect finished";
                    ContentAutoDetectLaunchButton.IsEnabled = false;
                }
            }
        }

        private void AutoDetectLaunch(object sender, RoutedEventArgs e)
        {
            Mod m = (Mod)ContentDataGrid.SelectedItem;
            List<ContentMapping> mappings = Searchie.AutoDetectinator(m, InternalModTypes, ModTypes);
            if (mappings.Count > 0)
            {
                ContentMapping contentMapping = mappings[0];
                ContentFileDataGrid.ItemsSource = contentMapping.Files;
            }
        }
        #endregion

        #region InternalModTypes
        //State changes
        private void InternalModTypeSelected(object sender, SelectionChangedEventArgs e)
        {
            //Getting info
            InternalModType type = (InternalModType)InternalModTypeSelect.SelectedItem;
            IMTDataGrid.ItemsSource = type.Files;
            GameDataCategory cat = GameDataCategories.Find(c => c.ID == type.Association);

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
                file.Mandatory = IMTMandatory.IsChecked.HasValue ? IMTMandatory.IsChecked.Value : false;
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
                var tvi = item as TreeViewItem;
                if (tvi != null)
                {
                    tvi.ExpandSubtree();
                }
            }
        }

        private void MinimizeTree(TreeView tv)
        {
            foreach (var item in tv.Items)
            {
                var tvi = item as TreeViewItem;
                if (tvi != null)
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
            ModFileManager FMan = new ModFileManager(_URL, ModTypes);

            Mod mod = Mods.Find(mm => mm.id == Int32.Parse(FMan.modID) && mm.type == Int32.Parse(FMan.modType));

            //Parsing mod info from API
            APIMod newAPIMod = await APIRequest.getMod(FMan.APIType, FMan.modID);

            //Create Mod from API information
            Mod newmod = Parse(newAPIMod, ModTypes);

            bool needupdate = true;
            //Checking if Mod is already in library
            if (mod != null)
            {
                if(mod.Updates < newmod.Updates)
                {
                    mle = ListMods.Find(ml => ml.LocalMod == mod);
                    downloadText = "Updating mod";
                }
                else
                {
                    needupdate = false;
                }
            }
            else
            {
                mod = new Mod(Int32.Parse(FMan.modID), Int32.Parse(FMan.modType), false);
                newElement = true;
                downloadText = "Downloading new mod";
            }
            if (!WorkingModList.Contains(mod) && needupdate)
            {
                WorkingModList.Add(mod);

                //Setting up new ModList
                if (newElement)
                {
                    //Adding element to list
                    Mods.Add(newmod);
                }
                else
                {
                    //Updating List
                    Mods[Mods.IndexOf(mod)] = newmod;
                }

                //Saving XML
                WriteModListFile(Mods);

                if (newElement)
                {
                    //Creating interface element
                    ListMods.Add(mle);
                    ModListView.Items.Refresh();
                }

                //Setting download UI
                mle.Title.Content = downloadText;

                Downloader modDownloader = new Downloader(mle.Progress, mle.Status, mle.TypeLabel, mle.Category);

                //Wait for download completion
                await modDownloader.DownloadArchiveAsync(FMan);

                //Setting extract UI
                mle.Title.Content = "Extracting mod";

                //Preparing Extraction
                Unarchiver un = new Unarchiver(mle.Progress, mle.Status);

                //Wait for Archive extraction
                await un.ExtractArchiveAsync(FMan.downloadDest, FMan.archiveContentDest, FMan.modArchiveFormat);

                //Setting extract UI
                mle.Title.Content = "Moving files";

                //Moving files
                await FMan.MoveDownload();

                //Cleanup
                FMan.ClearDownloadContents();

                //Providing mod to ModListElement and showing info
                mle.setMod(newmod);
                mle.Downloaded = true;

                //Refresh UI element
                mle.refreshUI();
                ModListView.SelectedItem = mle;

                //Removing mod from Working List
                WorkingModList.Remove(mod);
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
