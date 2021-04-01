using Quasar.Controls.Mod.ViewModels;
using Quasar.Controls.ModManagement.ViewModels;
using Quasar.Data.V2;
using Quasar.FileSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Quasar.Controls
{
    /// <summary>
    /// Interaction logic for ModListItem.xaml
    /// </summary>
    public partial class ModListItem : UserControl, INotifyPropertyChanged
    {
        #region Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(PropertyName);
                this.PropertyChanged(this, e);
            }
        }

        #endregion

        #region Fields
        private ModListItemViewModel _ModListItemViewModel { get; set; }
        #endregion

        #region Properties
        public ModListItemViewModel ModListItemViewModel
        {
            get => _ModListItemViewModel;
            set
            {
                if (_ModListItemViewModel == value)
                    return;

                _ModListItemViewModel = value;
                OnPropertyChanged("ModListItemViewModel");

            }
        }
        #endregion

        public ModListItem()
        {
            InitializeComponent();
        }

        public ModListItem(ModsViewModel model, LibraryItem _LibraryItem = null,  Game _Game = null, bool Downloading = false)
        {
            ModListItemViewModel = new ModListItemViewModel(_LibraryItem,_Game, model, Downloading);
            InitializeComponent();
            DataContext = ModListItemViewModel;
        }
        public ModListItem(string QuasarURL, ObservableCollection<Game> Games, ObservableCollection<LibraryItem> Mods, ModsViewModel model)
        {
            ModListItemViewModel = new ModListItemViewModel(QuasarURL, Games, Mods, model);
            InitializeComponent();
            DataContext = ModListItemViewModel;
        }

    }
}
