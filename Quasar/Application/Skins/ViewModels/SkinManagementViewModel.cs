using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataModels.Common;
using DataModels.Resource;
using Quasar.Common.Models;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using Quasar.Skins.Views;

namespace Quasar.Skins.ViewModels
{
    public class SkinManagementViewModel : ObservableObject
    {
        #region Data

        #region Private
        private CharacterItem _SelectedCharacterItem { get; set; }
        #endregion

        #region Public
        public MainUIViewModel MUVM { get; set; }

        public CharacterItem SelectedCharacterItem
        {
            get => _SelectedCharacterItem;
            set
            {
                _SelectedCharacterItem = value;
                OnPropertyChanged("SelectedCharacterItem");

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
        private ObservableCollection<CharacterItem> _CharacterItems { get; set; }
        private bool _SelectionViewActive { get; set; }
        
        #endregion

        #region Public

        public ObservableCollection<CharacterItem> CharacterItems
        {
            get => _CharacterItems;
            set
            {
                _CharacterItems = value;
                OnPropertyChanged("CharacterItems");
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
                    _GoBack = new RelayCommand(param => ResetSelectedCharacterItem());
                }
                return _GoBack;
            }
        }
        #endregion

        #endregion

        public SkinManagementViewModel(MainUIViewModel _MUVM)
        {
            MUVM = _MUVM;
            CharacterItems = new()
            {
                new CharacterItem("mario"),
                new CharacterItem("donkey"),
                new CharacterItem("link"),
                new CharacterItem("samus"),
                new CharacterItem("samusd"),
                new CharacterItem("yoshi"),
                new CharacterItem("kirby"),
                new CharacterItem("fox"),
                new CharacterItem("pikachu"),
                new CharacterItem("luigi"),
                new CharacterItem("ness"),
                new CharacterItem("captain"),
                new CharacterItem("purin"),
                new CharacterItem("peach"),
                new CharacterItem("daisy"),
                new CharacterItem("koopa"),
                new CharacterItem("ice_climber"),
                new CharacterItem("sheik"),
                new CharacterItem("zelda"),
                new CharacterItem("mariod"),
                new CharacterItem("pichu"),
                new CharacterItem("falco"),
                new CharacterItem("marth"),
                new CharacterItem("lucina"),
                new CharacterItem("younglink"),
                new CharacterItem("ganon"),
                new CharacterItem("mewtwo"),
                new CharacterItem("roy"),
                new CharacterItem("chrom"),
                new CharacterItem("gamewatch"),
                new CharacterItem("metaknight"),
                new CharacterItem("pit"),
                new CharacterItem("pitb"),
                new CharacterItem("szerosuit"),
                new CharacterItem("wario"),
                new CharacterItem("snake"),
                new CharacterItem("ike"),
                new CharacterItem("ptrainer"),
                new CharacterItem("diddy"),
                new CharacterItem("lucas"),
                new CharacterItem("sonic"),
                new CharacterItem("dedede"),
                new CharacterItem("pikmin"),
                new CharacterItem("lucario"),
                new CharacterItem("robot"),
                new CharacterItem("toonlink"),
                new CharacterItem("wolf"),
                new CharacterItem("murabito"),
                new CharacterItem("rockman"),
                new CharacterItem("wiifit"),
                new CharacterItem("rosetta"),
                new CharacterItem("littlemac"),
                new CharacterItem("gekkouga"),
                new CharacterItem("palutena"),
                new CharacterItem("pacman"),
                new CharacterItem("reflet"),
                new CharacterItem("shulk"),
                new CharacterItem("koopajr"),
                new CharacterItem("duckhunt"),
                new CharacterItem("ryu"),
                new CharacterItem("ken"),
                new CharacterItem("cloud"),
                new CharacterItem("kamui"),
                new CharacterItem("bayonetta"),
                new CharacterItem("inkling"),
                new CharacterItem("ridley"),
                new CharacterItem("simon"),
                new CharacterItem("richter"),
                new CharacterItem("krool"),
                new CharacterItem("shizue"),
                new CharacterItem("gaogaen"),
                new CharacterItem("packun"),
                new CharacterItem("jack"),
                new CharacterItem("brave"),
                new CharacterItem("buddy"),
                new CharacterItem("dolly"),
                new CharacterItem("master"),
                new CharacterItem("tantan"),
                new CharacterItem("pickel"),
                new CharacterItem("edge"),
                new CharacterItem("elight"),
                new CharacterItem("demon"),
                new CharacterItem("trail"),
                new CharacterItem("miifighter"),
                new CharacterItem("miiswordsman"),
                new CharacterItem("miigunner")
            };

            EventSystem.Subscribe<CharacterItemViewModel>(GetSelectedCharacterItem);
            SelectionViewActive = true;

        }

        public void GetSelectedCharacterItem(CharacterItemViewModel ci)
        {
            CharacterItem cis = CharacterItems.Single(c => c.CIVM == ci);
            SelectedCharacterItem = cis;
        }

        public void ResetSelectedCharacterItem()
        {
            SelectedCharacterItem = null;
        }
    }
}
