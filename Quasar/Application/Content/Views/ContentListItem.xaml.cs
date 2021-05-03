using Quasar.Content.ViewModels;
using Quasar.Data.V2;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Quasar.Content.Views
{
    /// <summary>
    /// Interaction logic for ModListItem.xaml
    /// </summary>
    public partial class ContentListItem : UserControl
    {
        public ContentListItemViewModel CLIVM { get; set; }

        public ContentListItem(ContentItem ContentItem, LibraryItem LibraryItem, QuasarModType qmt, List<GameElement> GameElements, int colorID)
        {
            
            CLIVM = new ContentListItemViewModel(ContentItem, LibraryItem, qmt, GameElements, colorID);
            InitializeComponent();
            DataContext = CLIVM;
        }

        private void ExpandRetract_Click(object sender, RoutedEventArgs e)
        {/*
            Smol = true;*/
        }

        private void SaveContentMapping(object sender, RoutedEventArgs e)
        {
                   
        }
    }
}
