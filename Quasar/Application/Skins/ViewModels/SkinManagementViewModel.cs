using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataModels.Common;
using DataModels.Resource;
using DataModels.User;
using Quasar.Common.Models;
using Quasar.Common.Views;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using Quasar.Skins.Views;
using Workshop.Associations;

namespace Quasar.Skins.ViewModels
{
    public class SkinManagementViewModel : ObservableObject
    {
        #region Data

        #region Private
        private CharacterItem _SelectedCharacterItem { get; set; }
        private SlotModel _SourceSlot { get; set; }
        private ObservableCollection<SlotModel> _Slots { get; set; }
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
        public SlotModel SourceSlot
        {
            get => _SourceSlot;
            set
            {
                _SourceSlot = value;
                OnPropertyChanged("SourceSlot");
            }
        }

        public ObservableCollection<SlotModel> Slots
        {
            get => _Slots;
            set
            {
                _Slots = value;
                OnPropertyChanged("Slots");
            }
        }
        #endregion

        #endregion

        #region View

        #region Private
        private ObservableCollection<CharacterItem> _CharacterItems { get; set; }
        private bool _SelectionViewActive { get; set; }
        private bool _Grouped { get; set; } = true;
        
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

        public bool Grouped
        {
            get => _Grouped;
            set
            {
                _Grouped = value;
                OnPropertyChanged("Grouped");
                RefreshSlots();
            }
        }
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
            CharacterItems = GetCharacterItems();

            EventSystem.Subscribe<CharacterItemViewModel>(GetSelectedCharacterItem);
            SelectionViewActive = true;

            SourceSlot = new()
            {
                Contents = new(),
                GameElement = null,
                ModTypes = MUVM.QuasarModTypes.Where(t => t.GameElementFamilyID == 1).ToList(),
                Slots = new List<int>() {-1}
            };

            Slots = new();
            for (int i = 0; i < 8; i++)
            {
                Slots.Add(new()
                {
                    Contents = new(),
                    GameElement = null,
                    ModTypes = MUVM.QuasarModTypes.Where(t => t.GameElementFamilyID == 1).ToList(),
                    Slots = new(){i}
                });
            }

        }

        public void GetSelectedCharacterItem(CharacterItemViewModel ci)
        {
            CharacterItem cis = CharacterItems.Single(c => c.CIVM == ci);
            SelectedCharacterItem = cis;
            RefreshSlots();
        }
        public void ResetSelectedCharacterItem()
        {
            SelectedCharacterItem = null;
        }

        public ObservableCollection<CharacterItem> GetCharacterItems()
        {
            return new()
            {
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "mario")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "donkey")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "link")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "samus")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "samusd")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "yoshi")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "kirby")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "fox")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "pikachu")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "luigi")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "ness")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "captain")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "purin")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "peach")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "daisy")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "koopa")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName.Contains("ice_climber"))),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "sheik")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "zelda")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "mariod")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "pichu")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "falco")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "marth")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "lucina")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "younglink")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "ganon")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "mewtwo")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "roy")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "chrom")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "gamewatch")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "metaknight")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "pit")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "pitb")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "szerosuit")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "wario")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "snake")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "ike")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName.Contains("ptrainer;pfushigisou;plizardon;pzenigame"))),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "diddy")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "lucas")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "sonic")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "dedede")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "pikmin")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "lucario")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "robot")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "toonlink")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "wolf")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "murabito")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "rockman")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "wiifit")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "rosetta")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "littlemac")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "gekkouga")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "palutena")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "pacman")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "reflet")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "shulk")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "koopajr")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "duckhunt")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "ryu")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "ken")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "cloud")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "kamui")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "bayonetta")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "inkling")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "ridley")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "simon")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "richter")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "krool")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "shizue")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "gaogaen")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "packun")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "jack")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "brave")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "buddy")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "dolly")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "master")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "tantan")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "pickel")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "edge")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName.Contains("elight"))),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "demon")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "trail")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "miifighter")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "miiswordsman")),
                new CharacterItem(MUVM.CurrentGame.GameElementFamilies[0].GameElements.SingleOrDefault(e => e.GameFolderName == "miigunner"))
            };
        }

        public void RefreshSlots()
        {
            SourceSlot.GameElement = SelectedCharacterItem.CIVM.GameElement;
            SourceSlot.GetMatchingContents(MUVM.ContentItems, Grouped);

            foreach (SlotModel slotModel in Slots)
            {
                slotModel.GameElement = SelectedCharacterItem.CIVM.GameElement;
                slotModel.GetMatchingContents(MUVM.ContentItems, Grouped);
            }
        }
    }
}
