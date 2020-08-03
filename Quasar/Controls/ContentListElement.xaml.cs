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
    public partial class ContentListElement : UserControl
    {
        public bool smol = true;

        public InternalModType LocalModType;
        public ContentMapping LocalMapping;
        public LibraryMod LocalMod;
        public List<GameDataCategory> GameData;

        public ContentListElement()
        {
            InitializeComponent();
        }

        public ContentListElement(ContentMapping contentMapping, LibraryMod libraryMod, InternalModType imt, List<GameDataCategory> Categories)
        {
            InitializeComponent();

            SetContent(contentMapping);
            SetLibraryMod(libraryMod);
            SetInternalModType(imt);
            GameData = Categories;

            RefreshInterface();
        }

        public void SetContent(ContentMapping contentMapping)
        {
            LocalMapping = contentMapping;
        }

        public void SetLibraryMod(LibraryMod libraryMod)
        {
            LocalMod = libraryMod;
        }

        public void SetInternalModType(InternalModType internalModType)
        {
            LocalModType = internalModType;
        }

        public void setColor(int color)
        {
            SolidColorBrush brush;
            switch (color)
            {
                case 0:
                    brush = FindResource("ColorA") as SolidColorBrush;
                    break;
                case 1:
                    brush = FindResource("ColorB") as SolidColorBrush;
                    break;
                default:
                    brush = FindResource("ColorA") as SolidColorBrush;
                    break;
            }
            BorderColor.Fill = brush;
        }

        public void RefreshInterface()
        {
            ModName.Content = LocalMod.Name;
            ContentNameTextBox.Text = LocalMapping.Name;
            ContentName.Content = LocalMapping.Name;
            InternalModType.Content = LocalModType.Name;

            GameDataCategory cat = GameData.Find(gd => gd.ID == LocalModType.GameID);
            Association.Content = cat.Name;

            ContentAssociationComboBox.ItemsSource = cat.Items;
            ContentFilesTextBlock.Text = "";
            foreach(ContentMappingFile f in LocalMapping.Files)
            {
                ContentFilesTextBlock.Text += f.SourcePath + "\r\n";
            }
            Slot.Content = "Default Slot";
        }

        private void ExpandRetract_Click(object sender, RoutedEventArgs e)
        {
            RetractUI();
        }

        public void ExpandUI()
        {
            this.Height = 180;
            smol = false;

            ColoredRectangle.Rect = new Rect(0, 0, 450, 180);
            Minimize.Visibility = Visibility.Visible;
        }

        public void RetractUI()
        {
            this.Height = 30;
            smol = true;
            ColoredRectangle.Rect = new Rect(0, 0, 450, 30);
            Minimize.Visibility = Visibility.Hidden;
        }
    }
}
