using log4net;
using Quasar.Controls.Mod.ViewModels;
using Quasar.Controls.ModManagement.ViewModels;
using DataModels.User;
using DataModels.Resource;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

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
        private ModViewModel _ModViewModel { get; set; }
        #endregion

        #region Properties
        public ModViewModel ModViewModel
        {
            get => _ModViewModel;
            set
            {
                if (_ModViewModel == value)
                    return;

                _ModViewModel = value;
                OnPropertyChanged("ModViewModel");

            }
        }
        #endregion



        public ModListItem()
        {
            InitializeComponent();
        }

        public ModListItem(LibraryViewModel model,ILog _QuasarLogger, LibraryItem _LibraryItem = null,  Game _Game = null, bool Downloading = false)
        {
            ModViewModel = new ModViewModel(_LibraryItem,_Game, model, _QuasarLogger, Downloading);
            InitializeComponent();
            DataContext = ModViewModel;
        }
        public ModListItem(string QuasarURL, ObservableCollection<Game> Games, ObservableCollection<LibraryItem> Mods, LibraryViewModel model, ILog _QuasarLogger)
        {
            ModViewModel = new ModViewModel(QuasarURL, Games, Mods, model, _QuasarLogger);
            InitializeComponent();
            DataContext = ModViewModel;
        }

        private void ModNameKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
                ModViewModel.RenameMod();
        }
    }
}
