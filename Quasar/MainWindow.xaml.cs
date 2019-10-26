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

namespace Quasar
{
    public partial class MainWindow : Window
    {
        ModList Mods;
        List<ModType> ModTypes { get; set; }
        List<Character> Characters { get; set; }
        List<Family> Families { get; set; }

        Mutex serverMutex;

        PipeClient Pc_principal { get; set; }

        public MainWindow()
        {
            //Things to run before app really starts
            serverMutex = Checker.Instances(serverMutex);
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

            foreach (Mod x in Mods)
            {
                ModListElement mle = new ModListElement();
                mle.Title.Content = x.Name;
                mle.Progress.Visibility = Visibility.Hidden;
                mle.Status.Content = "Awaiting Check";
                mle.modID = x.id;
                mle.modType = x.type;
                mle.modAssociation = x.association;
                SkinStackPanel.Children.Add(mle);
            }
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

        //Refreshes the content of the mod list based on mod type
        private void ModTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ModType selectedType = (ModType)comboBox.SelectedItem;
            switch (selectedType.ID)
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
                    break;

            }
        }

        //Refreshes the contents of the filter combobox
        public void PrintModInformation(APIMod _item)
        {
            SkinNameLabel.Content = "Name : " + _item.name;
            SkinAuthorLabel.Content = "Authors : " + _item.authors;
        }


        public void ShowFilteredList(ModList Filters)
        {
            foreach(ModListElement mle in SkinStackPanel.Children)
            {
                
                mle.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region OTHERS
        //Launches a Quasar Download from it's URL
        private async void TestDownloadButtonPress(object sender, RoutedEventArgs e)
        {
            ModListElement newItem = new ModListElement();
            SkinStackPanel.Children.Add(newItem);

            newItem.Title.Content = "Downloading new mod";
            Downloader modInstaller = new Downloader(newItem.Progress, newItem.Status);

            await modInstaller.DownloadArchiveAsync("quasar:https://gamebanana.com/mmdl/197052,Skin,153832,7z");

            APIMod newMod = await APIRequest.getMod(modInstaller.contentType, modInstaller.contentID); 

            Mod parsedMod = Parse(newMod, ModTypes);
            Mods.Add(parsedMod);
            WriteModListFile(Mods);

            PrintModInformation(newMod);

        }

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

    }

    
}
