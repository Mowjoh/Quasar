using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Quasar.XMLResources.Library;

namespace Quasar.Controls
{
    /// <summary>
    /// Interaction logic for SkinItem.xaml
    /// </summary>
    public partial class ModListElement : UserControl
    {
        public int modID;
        public int modType;
        public int modCategory;
        public bool modProcessed;

        public int gameID;

        public bool recto
        {
            get; set;
        }

        public LibraryMod LocalMod;

        public bool Downloaded = false;

        public bool isActive { get; set; }
        public ModListElement()
        {
            InitializeComponent();
            isActive = true;
            recto = true;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        public void SetMod(LibraryMod _mod)
        {
            
            LocalMod = _mod;
            modID = _mod.ID;
            modType = _mod.TypeID;
            modCategory = _mod.APICategoryID;
            modProcessed = _mod.FinishedProcessing;
            Title.Content = _mod.Name;
            ModCategory.Content = _mod.APICategoryName;
            ModType.Content = _mod.TypeLabel;
            SetModTypeColor(modType);
            Status.Content = "Up to date";
            LoadImage(LocalMod);

        }
        public void setGame(Game _Game)
        {
            ModGame.Content = _Game.Name;
            gameID = _Game.ID;
        }

        public void RefreshInterface()
        {
            Title.Content = LocalMod.Name;
            ModCategory.Content = LocalMod.APICategoryName;
            ModType.Content = LocalMod.TypeLabel;
            SetModTypeColor(modType);
            Status.Content = "Up to date";
            LoadImage(LocalMod);

        }
        private void LoadImage(LibraryMod libraryMod)
        {
            string imageSource = Properties.Settings.Default.DefaultDir + @"\Library\Screenshots\";
            string imagename = libraryMod.GameID + "_" + libraryMod.TypeID + "_" + libraryMod.ID;
            string[] files = Directory.GetFiles(imageSource, imagename + ".*");

            if (files.Length > 0)
            {
                ModImage.Source = new BitmapImage(new Uri(files[0], UriKind.RelativeOrAbsolute));
            }
            else
            {
                ModImage.Source = null;
            }
        }

        public void SetModTypeColor(int type)
        {
            SolidColorBrush brush;
            switch (type)
            {
                case 0:
                    brush = FindResource("BlueText") as SolidColorBrush;
                    break;
                case 1:
                    brush = FindResource("RedText") as SolidColorBrush;
                    break;
                case 2:
                    brush = FindResource("GreenText") as SolidColorBrush;
                    break;
                case 3:
                    brush = FindResource("PurpleText") as SolidColorBrush;
                    break;
                case 4:
                    brush = FindResource("DarkBlueText") as SolidColorBrush;
                    break;
                default:
                    brush = FindResource("BlackText") as SolidColorBrush;
                    break;
            }
            /*
            Title.Foreground = brush;
            ModCategory.Foreground = brush;
            ModType.Foreground = brush;
            Status.Foreground = brush;*/
            Border.Stroke = brush;
        }

        private void ModImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            recto = false;
        }
    }
}
