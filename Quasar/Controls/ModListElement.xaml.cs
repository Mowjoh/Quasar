using Quasar.FileSystem;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        public Game game;

        public bool recto
        {
            get; set;
        }

        public bool smol = true;

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

            Treeview.Visibility = Visibility.Visible;
            Trash.Visibility = Visibility.Visible;

            int count = _mod.Authors.Length > 3 ? 3 : _mod.Authors.Length;
            for (int i = 0; i < count; i++)
            {
                if(_mod.Authors[i][2] == "0")
                {
                    AutorNameStackPanel.Children.Add(new Label() { Content = _mod.Authors[i][0], Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28 });
                }
                else
                {
                    Run run = new Run();
                    run.Text = _mod.Authors[i][0];

                    Hyperlink hl = new Hyperlink(run);
                    hl.NavigateUri = new Uri(@"https://gamebanana.com/members/" + _mod.Authors[i][2]);
                    hl.Click += new RoutedEventHandler(link_click);

                    TextBlock textBlock = new TextBlock() { Margin = new Thickness(5, 0, 0, 0) , Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28, TextAlignment = TextAlignment.Left, LineStackingStrategy = LineStackingStrategy.BlockLineHeight, LineHeight = 22  };
                    textBlock.Inlines.Add(hl);

                    AutorNameStackPanel.Children.Add(textBlock);
                }
                
                AutorRoleStackPanel.Children.Add(new Label() { Content = _mod.Authors[i][1], Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28 });
            }

        }

        public void link_click(Object sender, RoutedEventArgs routedEventArgs)
        {
            Hyperlink Link = (Hyperlink)sender;
            Process.Start(Link.NavigateUri.ToString());
        }

        public void setGame(Game _Game)
        {
            game = _Game;
            //ModGame.Content = _Game.Name;
            gameID = _Game.ID;
        }

        public void RefreshInterface()
        {
            Title.Content = LocalMod.Name;
            ModCategory.Content = LocalMod.APICategoryName;
            ModType.Content = LocalMod.TypeLabel;
            SetModTypeColor(modType);
            Status.Content = "";
            LoadImage(LocalMod);
            ModGame.Content = game.Name;

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
            BorderColor.Fill = brush;
        }

        private void ModImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            recto = false;
        }

        private void ExpandRetract_Click(object sender, RoutedEventArgs e)
        {
            RetractUI();
        }

        public void ExpandUI()
        {
            this.Height = 160;
            smol = false;

            ColoredRectangle.Rect = new Rect(0, 0, 450, 160);

            LoadImage(LocalMod);

            Minimize.Visibility = Visibility.Visible;
        }

        public void RetractUI()
        {
            this.Height = 30;
            smol = true;
            ColoredRectangle.Rect = new Rect(0, 0, 450, 30);
            Minimize.Visibility = Visibility.Hidden;
        }

        private void Treeview_Click(object sender, RoutedEventArgs e)
        {
            OpenTreeView(new ModFileManager(LocalMod, game).LibraryContentFolderPath, LocalMod.Name);


        }

        private void OpenTreeView(String path, String name)
        {
            new FileView(path,name).Show();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Trash_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
