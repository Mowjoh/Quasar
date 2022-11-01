using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using DataModels.Common;
using DataModels.Resource;
using DataModels.User;
using GongSolutions.Wpf.DragDrop;
using Helpers.IPC;
using Microsoft.VisualBasic;
using Quasar.Common;
using Quasar.Common.Models;
using Quasar.Common.Views;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using Quasar.Skins.Views;
using Workshop.Associations;
using Workshop.FileManagement;

namespace Quasar.Skins.ViewModels
{
    public class SkinManagementViewModel : ObservableObject, IDropTarget
    {
        #region Data

        #region Private
        private CharacterItem _SelectedCharacterItem { get; set; }
        private SlotViewModel _SourceSlot { get; set; }
        private ObservableCollection<SlotViewModel> _Slots { get; set; }
        private int _StartingSlot { get; set; }
        private int _ModuloSlot => _StartingSlot < 8 ? _StartingSlot : (_StartingSlot % 8);
        private bool SlotChangeWorking = false;
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
        public SlotViewModel SourceSlot
        {
            get => _SourceSlot;
            set
            {
                _SourceSlot = value;
                OnPropertyChanged("SourceSlot");
            }
        }
        public ObservableCollection<SlotViewModel> Slots
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


        public bool SecondSlots => _ModuloSlot > 3;
        public string Slot1Text => $"Player {(_StartingSlot + 1)}";
        public string Slot2Text => $"Player {(_StartingSlot + 2)}";
        public string Slot3Text => $"Player {(_StartingSlot + 3)}";
        public string Slot4Text => $"Player {(_StartingSlot + 4)}";

        
        public string Slot1Image => SelectedCharacterItem != null ? $@"../../../Resources/images/Characters/{SelectedCharacterItem.CIVM.ScreenshotName}_0{_ModuloSlot}.png" : "";
        public string Slot2Image => SelectedCharacterItem != null ? $@"../../../Resources/images/Characters/{SelectedCharacterItem.CIVM.ScreenshotName}_0{_ModuloSlot +1}.png" : "";
        public string Slot3Image => SelectedCharacterItem != null ? $@"../../../Resources/images/Characters/{SelectedCharacterItem.CIVM.ScreenshotName}_0{_ModuloSlot +2}.png" : "";
        public string Slot4Image => SelectedCharacterItem != null ? $@"../../../Resources/images/Characters/{SelectedCharacterItem.CIVM.ScreenshotName}_0{_ModuloSlot +3}.png" : "";
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _GoBack { get; set; }
        private ICommand _PreviousSlots { get; set; }
        private ICommand _NextSlots { get; set; }
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

        public ICommand PreviousSlots
        {
            get
            {
                if (_PreviousSlots == null)
                {
                    _PreviousSlots = new RelayCommand(param => ChangeSlotPosition(false));
                }
                return _PreviousSlots;
            }
        }

        public ICommand NextSlots
        {
            get
            {
                if (_NextSlots == null)
                {
                    _NextSlots = new RelayCommand(param => ChangeSlotPosition(true));
                }
                return _NextSlots;
            }
        }
        #endregion

        #endregion

        public SkinManagementViewModel(MainUIViewModel _MUVM)
        {
            MUVM = _MUVM;
            CharacterItems = GetCharacterItems();

            EventSystem.Subscribe<CharacterItemViewModel>(GetSelectedCharacterItem);
            EventSystem.Subscribe<AssignmentContent>(ElementRightClicked);
            SelectionViewActive = true;

            _StartingSlot = 0;

            SourceSlot = new()
            {
                Source = true,
                SlotModel = new()
                {
                    Contents = new(),
                    GameElement = null,
                    ModTypes = MUVM.QuasarModTypes.Where(t => t.GameElementFamilyID == 1).ToList(),
                    Slot = -1
                },
                Image = ""
            };

            Slots = new();
            for (int i = 0; i < 4; i++)
            {
                Slots.Add(new()
                {
                    Source = false,
                    SlotModel = new()
                    {
                        Contents = new(),
                        GameElement = null,
                        ModTypes = MUVM.QuasarModTypes.Where(t => t.GameElementFamilyID == 1).ToList(),
                        Slot = (_StartingSlot + i)
                    },
                    Image = ""
                });
            }

            EventSystem.Subscribe<(SlotModel, ContentItemView)>(ContentDropped);
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

        public void GetSelectedCharacterItem(CharacterItemViewModel ci)
        {
            CharacterItem cis = CharacterItems.Single(c => c.CIVM == ci);
            SelectedCharacterItem = cis;
            RefreshUI();
            RefreshSlots();
        }

        public void ResetSelectedCharacterItem()
        {
            SelectedCharacterItem = null;
        }

        public void ChangeSlotPosition(bool forward)
        {
            if (forward)
            {
                _StartingSlot += 4;
            }

            if (!forward == _StartingSlot > 0)
            {
                _StartingSlot -= 4;
            }

            for (int i = 0; i < 4; i++)
            {
                Slots[i].SlotModel.Slot = _StartingSlot + i;
            }
            RefreshUI();
            RefreshSlots();
        }

        public void RefreshUI()
        {
            OnPropertyChanged("SecondSlots");
            OnPropertyChanged("Slot1Text");
            OnPropertyChanged("Slot2Text");
            OnPropertyChanged("Slot3Text");
            OnPropertyChanged("Slot4Text");
            OnPropertyChanged("Slot1Image");
            OnPropertyChanged("Slot2Image");
            OnPropertyChanged("Slot3Image");
            OnPropertyChanged("Slot4Image");
        }

        public void RefreshSlots()
        {
            SourceSlot.SlotModel.GameElement = SelectedCharacterItem.CIVM.GameElement;
            SourceSlot.SlotModel.GetMatchingContents(MUVM.ContentItems, Grouped);

            foreach (SlotViewModel slotViewModel in Slots)
            {
                slotViewModel.SlotModel.GameElement = SelectedCharacterItem.CIVM.GameElement;
                slotViewModel.SlotModel.GetMatchingContents(MUVM.ContentItems, Grouped);
                slotViewModel.Refresh();
            }
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            {
                if (dropInfo.Data is ContentItemView && !SlotChangeWorking)
                {
                    SlotChangeWorking = true;
                    Task.Run(() =>
                    {
                        System.Threading.Thread.Sleep(1000);
                        SlotChangeWorking = false;
                    });

                    Button b = (Button)dropInfo.VisualTarget;
                    if (b.Name == "NextSlotButton")
                    {
                        ChangeSlotPosition(true);
                    }

                    if (b.Name == "PreviousSlotButton")
                    {
                        ChangeSlotPosition(false);
                    }
                    
                }
                

            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void ContentDropped((SlotModel, ContentItemView) _data)
        {
            SlotModel senderModel = _data.Item1;
            ContentItemView senderContentItemView = _data.Item2;
            
            foreach (ContentItem content in senderContentItemView.ViewModel.Assignment.AssignmentContentItems)
            {
                content.SlotNumber = senderModel.Slot;
            }
            UserDataManager.SaveSeparatedContentItems(senderContentItemView.ViewModel.Assignment.AssignmentContentItems, Properties.QuasarSettings.Default.DefaultDir);
            RefreshSlots();
        }

        public void ElementRightClicked(AssignmentContent contents)
        {
            foreach (ContentItem content in contents.AssignmentContentItems)
            {
                if (content.SlotNumber == -1)
                {
                    content.SlotNumber = content.OriginalSlotNumber;
                }
                else
                {
                    content.SlotNumber = -1;
                }
                
            }
            UserDataManager.SaveSeparatedContentItems(contents.AssignmentContentItems, Properties.QuasarSettings.Default.DefaultDir);
            RefreshSlots();
        }
    }
}
