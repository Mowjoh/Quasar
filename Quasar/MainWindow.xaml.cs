using Quasar.Controls;
using Quasar.Resources;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static Quasar.Library;

namespace Quasar
{
    public partial class MainWindow : Window
    {
        ModList Mods;
        List<ModType> ModTypes;
        List<Character> Characters;
        List<Family> Families;
        
        public MainWindow()
        {
            InitializeComponent();

            //Load des éléments de base
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
            ModTypeSelect.Items.Clear();

            ComboBoxItem allItem = new ComboBoxItem() { Content = "All Types" };
            ModTypeSelect.Items.Add(allItem);

            foreach (ModType modType in ModTypes)
            {
                ComboBoxItem cbi = new ComboBoxItem() { Content = modType.Name };
                ModTypeSelect.Items.Add(cbi);
            }
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

        #region SINGLETON

        #endregion

        #region INTERFACE ACTIONS

        private void ModTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem SelectedItem = (ComboBoxItem)comboBox.SelectedItem;
            if ((string)SelectedItem.Content != "All Types")
            {
                ModType SelectedModType = ModTypes.Find(mt => mt.Name.Equals(SelectedItem.Content));
                FillFilterBox(SelectedModType.ID);
            }

        }

        //Refreshes the contents of the filter combobox
        private void FillFilterBox(int mode)
        {
            ModFilterSelect.Items.Clear();
            ModFilterSelect.Text = "Filter things here";

            ComboBoxItem allItem = new ComboBoxItem() { Content = "All Items" };
            ModTypeSelect.Items.Add(allItem);

            switch (mode)
            {
                //Skins
                case 0:
                    foreach (Character character in Characters)
                    {
                        ComboBoxItem cbi = new ComboBoxItem() { Content = character.name };
                        ModFilterSelect.Items.Add(cbi);
                    }
                    ModFilterSelect.IsEnabled = true;
                    break;
                case 1:
                    foreach (Family fam in Families)
                    {
                        ComboBoxItem cbi = new ComboBoxItem() { Content = fam.Name };
                        ModFilterSelect.Items.Add(cbi);
                    }
                    ModFilterSelect.IsEnabled = true;
                    break;
                default:
                    ModFilterSelect.IsEnabled = false;
                    break;
            }


        }

        public void PrintModInformation(APIMod _item)
        {
            SkinNameLabel.Content = "Name :" + _item.name;
            SkinAuthorLabel.Content = "Authors :" + _item.authors;
            SkinDownloadsLabel.Content = "Downloads :" + _item.downloads;
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

            await modInstaller.DownloadArchiveAsync("quasar:https://gamebanana.com/mmdl/403189,Skin,166918,7z");

            APIMod newMod = await APIRequest.getMod(modInstaller.contentType, modInstaller.contentID);
            PrintModInformation(newMod);
            Mods.Add(Parse(newMod));
            WriteModListFile(Mods);

        }

        //Deletes Everything Quasar has stored cause that's the easy way out
        private void DeleteDocumentFolderContents(object sender, RoutedEventArgs e)
        {
            var saveFolder = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\";

            Directory.Delete(saveFolder, true);
        }

        #endregion



    }

    
}
