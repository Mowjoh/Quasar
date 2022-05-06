using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataModels.Common;
using Quasar.Common.Models;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using Quasar.Music.ViewModels;
using Quasar.Music.Views;
using Quasar.Stages.ViewModels;
using Quasar.Stages.Views;

namespace Quasar.Music.ViewModels
{
    public class MusicManagerViewModel : ObservableObject
    {
        #region Data

        #region Private
        private SeriesItemView _SelectedSeriesItem { get; set; }
        #endregion

        #region Public
        public MainUIViewModel MUVM { get; set; }

        public SeriesItemView SelectedSeriesItem
        {
            get => _SelectedSeriesItem;
            set
            {
                _SelectedSeriesItem = value;
                OnPropertyChanged("SelectedSeriesItem");

                if (value != null)
                {
                    SelectionViewActive = false;
                }
                else
                {
                    SelectionViewActive = true;
                }
            }
        }
        #endregion

        #endregion

        #region View

        #region Private
        private ObservableCollection<SeriesItemView> _SeriesItems { get; set; }
        private bool _SelectionViewActive { get; set; }

        #endregion

        #region Public

        public ObservableCollection<SeriesItemView> SeriesItems
        {
            get => _SeriesItems;
            set
            {
                _SeriesItems = value;
                OnPropertyChanged("SeriesItems");
            }
        }

        public bool SelectionViewActive
        {
            get => _SelectionViewActive;
            set
            {
                _SelectionViewActive = value;
                OnPropertyChanged("SelectionViewActive");
                OnPropertyChanged("DisplayViewActive");
            }
        }
        public bool DisplayViewActive => !_SelectionViewActive;
        #endregion

        #endregion

        #region Commands



        #region Private
        private ICommand _GoBack { get; set; }
        #endregion

        #region Public
        public ICommand GoBack
        {
            get
            {
                if (_GoBack == null)
                {
                    _GoBack = new RelayCommand(param => ResetSelectedSeriesItem());
                }
                return _GoBack;
            }
        }
        #endregion

        #endregion

        public MusicManagerViewModel(MainUIViewModel _MUVM)
        {
            MUVM = _MUVM;
            SeriesItems = new()
            {
                new ("SmashBros"),
                new ("Mario"),
                new ("MarioKart"),
                new ("Zelda"),
                new ("Metroid"),
                new ("Yoshi"),
                new ("Kirby"),
                new ("StarFox"),
                new ("Pokemon"),
                new ("FZero"),
                new ("Earthbound"),
                new ("FireEmblem"),
                new ("Game&Watch"),
                new ("KidIcarus"),
                new ("Wario"),
                new ("Pikmin"),
                new ("WiiFit"),
                new ("PunchOut"),
                new ("Xenoblade"),
                new ("Splatoon"),
                new ("MetalGear"),
                new ("Sonic"),
                new ("MegaMan"),
                new ("PacMan"),
                new ("StreetFighter"),
                new ("FinalFantasy"),
                new ("Bayonetta"),
                new ("Castlevania"),
                new ("Persona"),
                new ("DragonQuest"),
                new ("BanjoKazooie"),
                new ("FatalFury"),
                new ("ARMS"),
                new ("Minecraft"),
                new ("Tekken"),
                new ("KingdomHearts"),
            };

            EventSystem.Subscribe<SeriesItemViewModel>(GetSelectedStageItem);
            SelectionViewActive = true;
        }

        public void GetSelectedStageItem(SeriesItemViewModel _series_item)
        {
            SeriesItemView Selection = SeriesItems.Single(c => c.SIVM == _series_item);
            SelectedSeriesItem = Selection;
            OnPropertyChanged("SelectedSeriesItem");
        }

        public void ResetSelectedSeriesItem()
        {
            SelectedSeriesItem = null;
        }
    }
}
