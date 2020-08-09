using Quasar.FileSystem;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    public partial class ContentListItem : UserControl, INotifyPropertyChanged
    {
        #region Data
        private InternalModType _LocalModType;
        public InternalModType LocalModType
        {
            get
            {
                return _LocalModType;
            }
            set
            {
                _LocalModType = value;
                OnPropertyChanged("LocalModType");
            }
        }

        private ContentMapping _LocalMapping;
        public ContentMapping LocalMapping
        {
            get
            {
                return _LocalMapping;
            }
            set
            {
                _LocalMapping = value;
                OnPropertyChanged("LocalMapping");
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
                OnPropertyChanged("LocalMod");
            }
        }

        private GameDataCategory _GameData;
        public GameDataCategory GameData
        {
            get
            {
                return _GameData;
            }
            set
            {
                _GameData = value;
                OnPropertyChanged("GameData");
            }
        }
        #endregion

        #region Triggers
        //Handlers
        public event EventHandler SaveRequested;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
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
                Rekt = new Rect(0, 0, 50, _Smol ? 30 : 160);
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

        #endregion

        #region Visuals
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

        private SolidColorBrush _ContentOddEven;

        public SolidColorBrush ContentOddEven
        {
            get
            {
                return _ContentOddEven;
            }
            set
            {
                _ContentOddEven = value;
                OnPropertyChanged("ContentOddEven");
            }
        }

        public int _ColorValue;

        public int ColorValue
        {
            get
            {
                return _ColorValue;
            }
            set
            {
                _ColorValue = value;
                setColor(value);
            }
        }
        #endregion

        public ContentListItem()
        {
            InitializeComponent();
            Smol = true;
        }

        public ContentListItem(ContentMapping contentMapping, LibraryMod libraryMod, InternalModType imt, List<GameDataCategory> Categories, int colorID)
        {
            InitializeComponent();

            LocalMapping = contentMapping;
            LocalMod = libraryMod;
            LocalModType = imt;

            GameData = Categories.Find(gd => gd.ID == LocalModType.GameID);
            foreach(ContentMappingFile cmf in LocalMapping.Files)
            {
                FilesTextBlock.Text += cmf.SourcePath + "\r\n";
            }
            ColorValue = colorID;

            Smol = true;
        }

        public void setColor(int color)
        {
            SolidColorBrush brush;
            switch (color)
            {
                case 0:
                    ContentOddEven = FindResource("ColorA") as SolidColorBrush;
                    break;
                case 1:
                    ContentOddEven = FindResource("ColorB") as SolidColorBrush;
                    break;
                default:
                    ContentOddEven = FindResource("ColorA") as SolidColorBrush;
                    break;
            }
        }

        private void ExpandRetract_Click(object sender, RoutedEventArgs e)
        {
            Smol = true;
        }

        private void SaveContentMapping(object sender, RoutedEventArgs e)
        {
            if (this.SaveRequested != null)
                this.SaveRequested(this, new EventArgs());
        }
    }
}
