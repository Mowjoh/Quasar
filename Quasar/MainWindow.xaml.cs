using Quasar.Controls;
using Quasar.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Quasar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture =
            new System.Globalization.CultureInfo(Quasar.Properties.Settings.Default["language"].ToString());

            InitializeComponent();
            LoadCharacterList();

        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            SkinItem newItem = new SkinItem();
            SkinStackPanel.Children.Add(newItem);

            newItem.Title.Content = "Downloading new mod";
            Installer modInstaller = new Installer(newItem.Progress, newItem.Status);

            await modInstaller.DownloadArchiveAsync("quasar:https://gamebanana.com/mmdl/403189,Skin,166918,rar");

            APIMod newMod = await APIRequest.getMod(modInstaller.contentType, modInstaller.contentID);
            showModInfo(newMod);

        }

        private void LoadCharacterList()
        {
            List<Character> characters = XML.GetCharacters();

            foreach(Character character in characters)
            {
                ComboBoxItem cbi = new ComboBoxItem() { Content = character.name };
                CharacterSelect.Items.Add(cbi);
                
            }
        }

        private void Skins_Click(object sender, RoutedEventArgs e)
        {
            ManagerContentTab.SelectedItem = (SkinsTab);
        }

        private void Stages_Click(object sender, RoutedEventArgs e)
        {
            ManagerContentTab.SelectedItem = (StagesTab);
        }

        public void showModInfo(APIMod _item)
        {
            SkinNameLabel.Content = "Name :"+_item.name;
            SkinAuthorLabel.Content = "Authors :" + _item.authors;
            SkinDownloadsLabel.Content = "Downloads :" + _item.downloads;
        }

    }

    
}
