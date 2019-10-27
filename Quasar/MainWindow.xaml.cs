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

namespace Quasar
{
    public partial class MainWindow : Window
    {

        public List<Mod> Mods;
        public List<ModListElement> ListMods { get; set; }

        public Mod SelectedMod { get; set; }

        public QuasarDownloads DLS;

        List<ModType> ModTypes { get; set; }
        List<Character> Characters { get; set; }
        List<Family> Families { get; set; }

        Mutex serverMutex;

        public MainWindow()
        {
            //Things to run before app really starts
            DLS = new QuasarDownloads();
            DLS.List.CollectionChanged += QuasarDownloadCollectionChanged;
            serverMutex = Checker.Instances(serverMutex, DLS.List);
            Checker.FirstRun();
            Folderino.CheckBaseFolders();
            Folderino.CheckBaseFiles();

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
            Mods = GetModListFile();
            ListMods = new List<ModListElement>();

            foreach (Mod x in Mods)
            {
                ModListElement mle = new ModListElement();
                mle.Title.Content = x.Name;
                mle.Progress.Visibility = Visibility.Hidden;
                mle.setMod(x);
                mle.Downloaded = true;
                ListMods.Add(mle);
            }

            ModListView.ItemsSource = ListMods;
        }

        private void LoadBasicLists()
        {
            LoadModTypes();
            LoadCharacterList();
            LoadFamilies();

        }

        private void LoadModTypes()
        {
            ModTypes = XML.GetModTypes();
            ModTypeSelect.ItemsSource = ModTypes;
        }

        private void LoadCharacterList()
        {
            Characters = XML.GetCharacters();
        }

        private void LoadFamilies()
        {
            Families = XML.GetFamilies();
        }
        #endregion

        #region INTERFACE ACTIONS

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
            ShowAdvancedFilters(selectedType.ID);
        }

        private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ShowAdvancedFilters(int _modType)
        {
            switch (_modType)
            {
                case 0:
                    ModFilterSelect.ItemsSource = Characters;
                    break;
                case 1:
                    ModFilterSelect.ItemsSource = Families;
                    break;
                case 2:
                    ModFilterSelect.ItemsSource = Families;
                    break;
                default:
                    ModFilterSelect.ItemsSource = null;
                    break;
            }
        }
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

        //Refreshes the contents of the filter combobox
        public void PrintModInformation(Mod _item)
        {
            //Thrashing the place
            ModInfoStackPanelValues.Children.Clear();
            VersionStackPanel.Children.Clear();

            //Showing Name and Authors
            ModInfoStackPanelValues.Children.Add(new Label() { Content = _item.Name });
            foreach (String[] author in _item.Authors)
            {
                ModInfoStackPanelValues.Children.Add(new Label() { Content = "- " + author[0] });
            }

            //Showing Version info
            VersionStackPanel.Children.Add(new Label() { Content = _item.Updates });
            VersionStackPanel.Children.Add(new Label() { Content = "Up to Date" });
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
            ModListElement newItem;

            //Creating interface element
            newItem = new ModListElement();
            ListMods.Add(newItem);
            ModListView.Items.Refresh();

            //Setting download UI
            newItem.Title.Content = "Downloading new mod";

            Downloader modInstaller = new Downloader(newItem.Progress, newItem.Status);

            //Wait for download completion
            await modInstaller.DownloadArchiveAsync(_URL);

            //Parsing mod info from API
            APIMod newMod = await APIRequest.getMod(modInstaller.contentType, modInstaller.contentID);

            //Create Mod from API information
            Mod parsedMod = Parse(newMod, ModTypes);

            //Updating list and saving XML
            Mods.Add(parsedMod);
            WriteModListFile(Mods);

            //Providing mod to ModListElement and showing info
            newItem.setMod(parsedMod);
            newItem.Downloaded = true;

            ModListView.SelectedItem = newItem;

            //Setting extract UI
            newItem.Title.Content = "Extracting mod";

            string fileSource = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + modInstaller.contentID + "." + modInstaller.fileFormat;
            string fileDestination = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + modInstaller.contentID + "\\";
            Unarchiver un = new Unarchiver(newItem.Progress, newItem.Status);

            await un.ExtractArchiveAsync(fileSource, fileDestination,modInstaller.fileFormat);

            newItem.refreshUI();
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
