using Quasar.Controls;
using Quasar.Resources;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static Quasar.Library;

namespace Quasar
{
    public partial class MainWindow : Window
    {
        List<Mod> Mods;
        public MainWindow()
        {
            //Récupération de la langue
            System.Threading.Thread.CurrentThread.CurrentUICulture =
            new System.Globalization.CultureInfo(Quasar.Properties.Settings.Default["language"].ToString());

            InitializeComponent();

            //Load des éléments de base
            LoadCharacterList();
            LoadMods();

        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            SkinItem newItem = new SkinItem();
            SkinStackPanel.Children.Add(newItem);

            newItem.Title.Content = "Downloading new mod";
            Downloader modInstaller = new Downloader(newItem.Progress, newItem.Status);

            await modInstaller.DownloadArchiveAsync("quasar:https://gamebanana.com/mmdl/403189,Skin,166918,7z");

            APIMod newMod = await APIRequest.getMod(modInstaller.contentType, modInstaller.contentID);
            showModInfo(newMod);
            Mods.Add(getModfromAPIMod(newMod));

        }


        private void LoadMods()
        {
            Mods = GetMods();
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

        public void showModInfo(APIMod _item)
        {
            SkinNameLabel.Content = "Name :"+_item.name;
            SkinAuthorLabel.Content = "Authors :" + _item.authors;
            SkinDownloadsLabel.Content = "Downloads :" + _item.downloads;
        }

        

    }

    
}
