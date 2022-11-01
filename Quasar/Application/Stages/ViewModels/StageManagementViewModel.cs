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
using Quasar.Skins.ViewModels;
using Quasar.Skins.Views;
using Quasar.Stages.Views;

namespace Quasar.Stages.ViewModels
{
    internal class StageManagementViewModel : ObservableObject
    {
        #region Data

        #region Private
        private StageItem _SelectedStageItem { get; set; }
        private SlotModel _SourceSlot { get; set; }
        private SlotModel _NormalSlot { get; set; }
        private SlotModel _OmegaSlot { get; set; }
        private SlotModel _BattlefieldSlot { get; set; }
        #endregion

        #region Public
        public MainUIViewModel MUVM { get; set; }

        public StageItem SelectedStageItem
        {
            get => _SelectedStageItem;
            set
            {
                _SelectedStageItem = value;
                OnPropertyChanged("SelectedStageItem");
                
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
        public SlotModel NormalSlot
        {
            get => _NormalSlot;
            set
            {
                _NormalSlot = value;
                OnPropertyChanged("NormalSlot");
            }
        }
        public SlotModel OmegaSlot
        {
            get => _OmegaSlot;
            set
            {
                _OmegaSlot = value;
                OnPropertyChanged("OmegaSlot");
            }
        }
        public SlotModel BattlefieldSlot
        {
            get => _BattlefieldSlot;
            set
            {
                _BattlefieldSlot = value;
                OnPropertyChanged("BattlefieldSlot");
            }
        }
        #endregion

        #endregion

        #region View

        #region Private
        private ObservableCollection<StageItem> _StageItems { get; set; }
        private bool _SelectionViewActive { get; set; }
        private bool _Grouped { get; set; } = true;
        #endregion

        #region Public

        public ObservableCollection<StageItem> StageItems
        {
            get => _StageItems;
            set
            {
                _StageItems = value;
                OnPropertyChanged("StageItems");
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
                    _GoBack = new RelayCommand(param => ResetSelectedStageItem());
                }
                return _GoBack;
            }
        }
        #endregion

        #endregion

        public StageManagementViewModel(MainUIViewModel _MUVM)
        {
            MUVM = _MUVM;
            StageItems = new()
            {
                new("blank"),
                new("blank"),
                new("blank"),
                new("BattlefieldL"),
                new("Battlefield"),
                new("BattlefieldS"),
                new("End"),
                new("Mario_Castle64"),
                new("DK_Jungle"),
                new("Zelda_Hyrule"),
                new("Yoshi_Story"),

                new("Kirby_Pupupu64"),
                new("Poke_Yamabuki"),
                new("Mario_Past64"),
                new("Mario_CastleDx"),
                new("Mario_Rainbow"),
                new("DK_Waterfall"),
                new("DK_Lodge"),
                new("Zelda_Greatbay"),
                new("Zelda_Temple"),
                new("Metroid_Zebestandardx"),
                new("Yoshi_Yoster"),

                new("Yoshi_Cartboard"),
                new("Kirby_Fountain"),
                new("Kirby_Greens"),
                new("Fox_Corneria"),
                new("Fox_Venom"),
                new("Poke_Stadium"),
                new("Mother_Onett"),
                new("Mario_PastUSA"),
                new("Metroid_Kraid"),
                new("FZero_Bigblue"),
                new("Mother_Fourside"),

                new("Mario_Dolpic"),
                new("Mario_PastX"),
                new("Kart_CircuitX"),
                new("Wario_Madein"),
                new("Zelda_Oldin"),
                new("Metroid_Norfair"),
                new("Metroid_Orpheon"),
                new("Yoshi_Island"),
                new("Kirby_Halberd"),
                new("Fox_LylatCruise"),
                new("Poke_Stadium2"),

                new("Fzero_Porttown"),
                new("FE_Siege"),
                new("Pikmin_Planet"),
                new("Animal_Village"),
                new("Mother_Newpork"),
                new("Ice_Top"),
                new("Icarus_SkyWorld"),
                new("MG_Shadowmoses"),
                new("LuigiMansion"),
                new("Zelda_Pirates"),
                new("Poke_Tengam"),

                new("75m"),
                new("MarioBros"),
                new("Plankton"),
                new("Sonic_Greenhill"),
                new("Mario_3DLand"),
                new("Mario_NewBros2"),
                new("Mario_Paper"),
                new("Zelda_Gerudo"),
                new("Zelda_Train"),
                new("Kirby_Gameboy"),
                new("Poke_Unova"),

                new("Poke_Tower"),
                new("Fzero_Mutecity3DS"),
                new("Mother_Magicant"),
                new("FE_Arena"),
                new("Icarus_Uprising"),
                new("Animal_Island"),
                new("BalloonFight"),
                new("NintenDogs"),
                new("StreetPass"),
                new("Tomodachi"),
                new("Pictochat2"),

                new("Mario_Uworld"),
                new("Mario_Galaxy"),
                new("Kart_CircuitFor"),
                new("Zelda_Skyward"),
                new("Kirby_Cave"),
                new("Poke_Kalos"),
                new("FE_Colloseum"),
                new("FlatZoneX"),
                new("Icarus_Angeland"),
                new("Wario_Gamer"),
                new("Pikmin_Garden"),

                new("Animal_City"),
                new("WiiFit"),
                new("PunchOutSB"),
                new("Xeno_Gaur"),
                new("DuckHunt"),
                new("WreckingCrew"),
                new("Pilotwings"),
                new("WufuIsland"),
                new("Sonic_Windyhill"),
                new("Rock_Wily"),
                new("Pac_Land"),

                new("Mario_Maker"),
                new("SF_Suzaku"),
                new("FF_Midgar"),
                new("Bayo_Clock"),
                new("Mario_Odyssey"),
                new("Zelda_Tower"),
                new("Spla_Parking"),
                new("Dracula_Castle"),
                new("Jack_Mementoes"),
                new("Brave_Altar"),
                new("Buddy_Spiral"),

                new("Dolly_Stadium"),
                new("FE_Shrine"),
                new("Tantan_Spring"),
                new("Pickel_World"),
                new("FF_Cave"),
                new("Xeno_Alst"),
                new("Demon_Dojo"),
                new("Trail_Castle"),

            };

            EventSystem.Subscribe<StageItemViewModel>(GetSelectedStageItem);
            SelectionViewActive = true;

            SourceSlot = new()
            {
                Contents = new(),
                GameElement = null,
                ModTypes = MUVM.QuasarModTypes.Where(t => t.GameElementFamilyID == 2).ToList(),
                Slot = -1
            };

            NormalSlot = new()
            {
                Contents = new(),
                GameElement = null,
                ModTypes = MUVM.QuasarModTypes.Where(t => t.GameElementFamilyID == 2).ToList(),
                Slot = 1
            };

            OmegaSlot = new()
            {
                Contents = new(),
                GameElement = null,
                ModTypes = MUVM.QuasarModTypes.Where(t => t.GameElementFamilyID == 2).ToList(),
                Slot = 1
            };

            BattlefieldSlot = new()
            {
                Contents = new(),
                GameElement = null,
                ModTypes = MUVM.QuasarModTypes.Where(t => t.GameElementFamilyID == 2).ToList(),
                Slot = 1
            };
        }

        public void GetSelectedStageItem(StageItemViewModel _stage_item)
        {
            StageItem selectedStageItem = StageItems.Single(c => c.SIVM == _stage_item);
            SelectedStageItem = selectedStageItem;
            OnPropertyChanged("SelectedStageItem");
        }

        public void ResetSelectedStageItem()
        {
            SelectedStageItem = null;
        }

        public void RefreshSlots()
        {
            //SourceSlot.GameElement = SelectedStageItem.SIVM.GameElement;
            //NormalSlot.GameElement = SelectedStageItem.SIVM.GameElement;
            //OmegaSlot.GameElement = SelectedStageItem.SIVM.GameElement;
            //BattlefieldSlot.GameElement = SelectedStageItem.SIVM.GameElement;
            SourceSlot.GetMatchingContents(MUVM.ContentItems, Grouped);
            NormalSlot.GetMatchingContents(MUVM.ContentItems, Grouped);
            OmegaSlot.GetMatchingContents(MUVM.ContentItems, Grouped);
            BattlefieldSlot.GetMatchingContents(MUVM.ContentItems, Grouped);
        }
    }
}
