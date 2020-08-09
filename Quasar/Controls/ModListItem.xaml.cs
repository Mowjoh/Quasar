using Quasar.FileSystem;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for ModListItem.xaml
    /// </summary>
    public partial class ModListItem : UserControl, INotifyPropertyChanged
    {
        //Referentials
        private Game _Game;
        public Game Game
        {
            get
            {
                return _Game;
            }
            set
            {
                _Game = value;
                OnPropertyChanged("Game");
            }
        }

        private LibraryMod _LocalMod;
        public LibraryMod LocalMod
        {
            get
            {
                return _LocalMod;
            }
            set
            {
                _LocalMod = value;
                SetModTypeColor(value != null? value.TypeID : 0);
                OnPropertyChanged("LocalMod");
            }
        }

        //Action values
        public bool isActive { get; set; }
        public bool Downloaded = false;

        //Handlers
        public event EventHandler TrashRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //Operations Status
        private String _ModStatusValue;
        public String ModStatusValue
        {
            get
            {
                return _ModStatusValue;
            }
            set
            {
                _ModStatusValue = value;
                OnPropertyChanged("ModStatusValue");
            }
        }

        private String _ModStatusTextValue;
        public String ModStatusTextValue
        {
            get
            {
                return _ModStatusTextValue;
            }
            set
            {
                _ModStatusTextValue = value;
                OnPropertyChanged("ModStatusTextValue");
            }
        }

        private int _ProgressBarValue;
        public int ProgressBarValue
        {
            get
            {
                return _ProgressBarValue;
            }
            set
            {
                _ProgressBarValue = value;
                OnPropertyChanged("ProgressBarValue");
            }
        }


        //Visibility Controllers
        private Boolean _Operation;
        public Boolean Operation
        {
            get
            {
                return _Operation;
            }

            set
            {
                _Operation = value;
                ElementActive = !value;
                OnPropertyChanged("Operation");
               
            }
        }

        private Boolean _ElementActive;
        public Boolean ElementActive
        {
            get
            {
                return _ElementActive;
            }

            set
            {
                _ElementActive = value;
                OnPropertyChanged("ElementActive");
            }
        }

        private Visibility _Filtered;

        public Visibility Filtered
        {
            get
            {
                return _Filtered;
            }

            set
            {
                _Filtered = value;
                OnPropertyChanged("Filtered");
            }
        }

        private bool _Filter;

        public bool Filter
        {
            get
            {
                return _Filter;
            }
            set
            {
                _Filter = value;
                if (value)
                {
                    Filtered = Visibility.Collapsed;
                }
                else
                {
                    Filtered = Visibility.Visible;
                }
            }
        }

        //Look Status
        private bool _Smol;
        public bool Smol
        {
            get
            {
                return _Smol;
            }

            set
            {
                _Smol = value;
                Rekt = new Rect(0, 0, 50, _Smol? 30 : 160);
                ElementHeight = _Smol ? 30 : 160;
                BigIcons = !value;
                OnPropertyChanged("Smol");
            }
        }

        private bool _BigIcons;
        public bool BigIcons
        {
            get
            {
                return _BigIcons;
            }

            set
            {
                _BigIcons = value;
                OnPropertyChanged("BigIcons");
            }
        }

        private int _ElementHeight;
        public int ElementHeight
        {
            get
            {
                return _ElementHeight;
            }
            set
            {
                _ElementHeight = value;
                OnPropertyChanged("ElementHeight");
            }
        }

        private Uri _ImageSource;
        public Uri ImageSource
        {
            get
            {
                return _ImageSource;
            }

            set
            {
                _ImageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        private Rect _Rekt;
        public Rect Rekt
        {
            get
            {
                return _Rekt;
            }
            set
            {
                _Rekt = value;
                OnPropertyChanged("Rekt");
            }
        }

        private SolidColorBrush _ModTypeFillColor;

        public SolidColorBrush ModTypeFillColor
        {
            get
            {
                return _ModTypeFillColor;
            }
            set
            {
                _ModTypeFillColor = value;
                OnPropertyChanged("ModTypeFillColor");
            }
        }




        public ModListItem()
        {
            InitializeComponent();
            Operation = false;
            Smol = true;
            SetModTypeColor(0);
            Filter = false;
        }

        public ModListItem(Boolean _OperationActive = false, LibraryMod _LibraryMod = null, Game _Game = null)
        {
            InitializeComponent();
            Operation = _OperationActive;
            LocalMod = _LibraryMod;
            Game = _Game;
            Smol = true;
            if(_LibraryMod != null)
            {
                LoadImage(LocalMod);
            }
            Filter = false;
        }

        //Internal sets
        public void SetMod(LibraryMod _mod)
        {

            LocalMod = _mod;

            LoadImage(LocalMod);

            //Dynamic info
            int count = _mod.Authors.Length > 3 ? 3 : _mod.Authors.Length;
            for (int i = 0; i < count; i++)
            {
                if (_mod.Authors[i][2] == "0")
                {
                    //AutorNameStackPanel.Children.Add(new Label() { Content = _mod.Authors[i][0], Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28 });
                }
                else
                {
                    Run run = new Run();
                    run.Text = _mod.Authors[i][0];

                    Hyperlink hl = new Hyperlink(run);
                    hl.NavigateUri = new Uri(@"https://gamebanana.com/members/" + _mod.Authors[i][2]);
                    hl.Click += new RoutedEventHandler(link_click);

                    TextBlock textBlock = new TextBlock() { Margin = new Thickness(5, 0, 0, 0), Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28, TextAlignment = TextAlignment.Left, LineStackingStrategy = LineStackingStrategy.BlockLineHeight, LineHeight = 22 };
                    textBlock.Inlines.Add(hl);

                    //AutorNameStackPanel.Children.Add(textBlock);
                }

                //AutorRoleStackPanel.Children.Add(new Label() { Content = _mod.Authors[i][1], Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"], Height = 28 });
            }

        }

        public void setGame(Game _Game)
        {
            Game = _Game;
        }

        //Interface
        public void LoadImage(LibraryMod libraryMod)
        {
            string imageSource = Properties.Settings.Default.DefaultDir + @"\Library\Screenshots\";
            string imagename = libraryMod.GameID + "_" + libraryMod.TypeID + "_" + libraryMod.ID;
            string[] files = Directory.GetFiles(imageSource, imagename + ".*");

            if (files.Length > 0)
            {
                ImageSource = new Uri(files[0], UriKind.RelativeOrAbsolute);
            }
            else
            {
                ImageSource = null;
            }
        }

        public void SetModTypeColor(int type)
        {
            switch (type)
            {
                case 1:
                    ModTypeFillColor = FindResource("SkinColor") as SolidColorBrush;
                    break;
                case 2:
                    ModTypeFillColor = FindResource("MapColor") as SolidColorBrush;
                    break;
                case 3:
                    ModTypeFillColor = FindResource("SoundColor") as SolidColorBrush;
                    break;
                case 4:
                    ModTypeFillColor = FindResource("GamefileColor") as SolidColorBrush;
                    break;
                case 5:
                    ModTypeFillColor = FindResource("GUIColor") as SolidColorBrush;
                    break;
                default:
                    ModTypeFillColor = FindResource("DefaultColor") as SolidColorBrush;
                    break;
            }
        }

        public void RetractUI()
        {
            Smol = true;
        }

        //Actions
        public void link_click(Object sender, RoutedEventArgs routedEventArgs)
        {
            Hyperlink Link = (Hyperlink)sender;
            Process.Start(Link.NavigateUri.ToString());
        }

        private void ExpandRetract_Click(object sender, RoutedEventArgs e)
        {
            RetractUI();
        }

        private void Treeview_Click(object sender, RoutedEventArgs e)
        {
            new FileView(new ModFileManager(LocalMod, Game).LibraryContentFolderPath, LocalMod.Name).Show();
        }
        
        private void Update_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void Trash_Click(object sender, RoutedEventArgs e)
        {
            if (this.TrashRequested != null)
                this.TrashRequested(this, new EventArgs());
        }
    }
}
