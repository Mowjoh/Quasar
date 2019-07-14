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
        public MainWindow()
        {
            //Récupération de la langue
            System.Threading.Thread.CurrentThread.CurrentUICulture =
            new System.Globalization.CultureInfo(Quasar.Properties.Settings.Default["language"].ToString());

            //Properties.Settings.Default.DefaultDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            InitializeComponent();

            //Load des éléments de base
            LoadCharacterList();

            LoadMods();

        }

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


        private void LoadMods()
        {
            Mods = GetModListFile();

            foreach(Mod x in Mods)
            {
                ModListElement mle = new ModListElement();
                mle.Title.Content = x.Name;
                mle.Progress.Visibility = Visibility.Hidden;
                mle.Status.Content = "Awaiting Check";
                SkinStackPanel.Children.Add(mle);

            }
        }

        private void LoadCharacterList()
        {
            List<Character> characters = XML.GetCharacters();

            foreach(Character character in characters)
            {
                ComboBoxItem cbi = new ComboBoxItem() { Content = character.name };
                ModFilterSelect.Items.Add(cbi);
            }
        }

        public void PrintModInformation(APIMod _item)
        {
            SkinNameLabel.Content = "Name :"+_item.name;
            SkinAuthorLabel.Content = "Authors :" + _item.authors;
            SkinDownloadsLabel.Content = "Downloads :" + _item.downloads;
        }

        private void DeleteDocumentFolderContents(object sender, RoutedEventArgs e)
        {
            string dPath = Quasar.Properties.Settings.Default["DefaultDir"].ToString();
            var saveFolder = dPath + "\\Library\\";

            Directory.Delete(saveFolder, true);
        }
    }

    
}
