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
            SkinItem.Title.Content = "Downloading new mod";
            Installer modInstaller = new Installer(SkinItem.Progress, SkinItem.Status);
            var t = Task.Run(() => modInstaller.DownloadArchiveAsync("quasar:https://gamebanana.com/mmdl/403189,Skin,166918"));

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    
}
