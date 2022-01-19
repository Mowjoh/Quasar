using log4net;
using Quasar.Associations.Views;
using Quasar.Common.Models;
using DataModels.User;
using DataModels.Common;
using DataModels.Resource;
using Quasar.Helpers;
using Quasar.MainUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Workshop.FileManagement;
using Workshop.Scanners;
using Workshop.Associations;

namespace Quasar.Associations.ViewModels
{
    public class AssociationViewModel : ObservableObject
    {
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        #region View

        #region Private

        private ObservableCollection<ContentListItem> _ItemCollection { get; set; }
        #endregion

        #region Public
        public ObservableCollection<ContentListItem> ItemCollection
        {
            get => _ItemCollection;
            set
            {
                _ItemCollection = value;
                OnPropertyChanged("ItemCollection");
            }
        }
        #endregion

        #endregion

        #region Data

        #region Private
        private MainUIViewModel _MUVM { get; set; }

        private bool _GroupedTypes { get; set; }
        #endregion

        #region Public

        public MainUIViewModel MUVM
        {
            get => _MUVM;
            set
            {
                _MUVM = value;
                OnPropertyChanged("MUVM");
            }
        }

        public bool GroupedTypes
        {
            get => _GroupedTypes;
            set
            {
                if (_GroupedTypes == value) return;

                _GroupedTypes = value;
                OnPropertyChanged("GroupedTypes");
                
                Properties.QuasarSettings.Default.GroupAssignmentTypes = value;
                Properties.QuasarSettings.Default.Save();

                if(MUVM.LibraryViewModel.SelectedModListItem != null)
                {
                    ObservableCollection<AssignmentContent> AssignmentContents =
                    Grouper.GetAssignmentContents(
                        MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem, MUVM.ContentItems,
                        Properties.QuasarSettings.Default.GroupAssignmentTypes);

                    DisplayContentItems(AssignmentContents.ToList());
                }
                
            }
        }
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _ScanMods { get; set; }
        #endregion

        #region Public
        public ICommand ScanMods
        {
            get
            {
                if (_ScanMods == null)
                {
                    _ScanMods = new RelayCommand(_param => ScanFiles(true));
                }
                return _ScanMods;
            }
        }
        #endregion

        #endregion

        public ILog QuasarLogger { get; set; }

        /// <summary>
        /// Association View Model Constructor
        /// </summary>
        /// <param name="_muvm">MainUI View Model to link</param>
        public AssociationViewModel(MainUIViewModel _muvm, ILog _quasar_logger)
        {
            QuasarLogger = _quasar_logger;
            MUVM = _muvm;
            ItemCollection = new ObservableCollection<ContentListItem>();
            GroupedTypes = Properties.QuasarSettings.Default.GroupAssignmentTypes;

            EventSystem.Subscribe<ContentListItemViewModel>(ProcessIncomingModalEvent);
        }

        #region Actions
        public void DisplayContentItems(List<AssignmentContent> _assignment_contents)
        {
            //Clearing Collection
            ItemCollection.Clear();

            foreach (AssignmentContent AssignmentContent in _assignment_contents)
            {
                QuasarModType Qmt = MUVM.QuasarModTypes.Single(_t => _t.ID == AssignmentContent.AssignmentContentItems[0].QuasarModTypeID);
                GameElement Ge = MUVM.Games[0].GameElementFamilies.Single(_f => _f.ID == Qmt.GameElementFamilyID)
                    .GameElements.Single(_e => _e.ID == AssignmentContent.AssignmentContentItems[0].GameElementID);

                if (Ge != null) 
                {
                    if (Qmt.ID == 8)
                    {
                        List<Option> MusicOptions = new() { new Option() { Key = "none", Value = Properties.Resources.Assignment_Label_NoElement } };
                        foreach (GameElement GameElement in MUVM.Games[0].GameElementFamilies[2].GameElements)
                        {
                            MusicOptions.Add(new Option() { Key = GameElement.ID.ToString(), Value = GameElement.Name });
                        }
                        string Element = MUVM.Games[0].GameElementFamilies[2].GameElements.Single(e => e.ID == AssignmentContent.AssignmentContentItems[0].OriginalGameElementID).Name;
                        ItemCollection.Add(new ContentListItem(AssignmentContent, Qmt.Name, Ge.Name, Element, ContentTypes.ElementSelected, MusicOptions));
                    }
                    else
                    {
                        List<Option> SlotOptions = new() { new Option() { Key = "none", Value = Properties.Resources.Assignment_Label_NoSlot } };

                        for (int i = 0; i < 8; i++)
                        {
                            SlotOptions.Add(new Option() { Key = i.ToString(), Value = String.Format("Slot {0}", (i + 1)) });
                        }

                        string origin = (AssignmentContent.AssignmentContentItems[0].OriginalSlotNumber + 1).ToString();
                        if (!AssignmentContent.Single)
                        {
                            ItemCollection.Add(new ContentListItem(AssignmentContent, Qmt.GroupName, Ge.Name, origin, ContentTypes.Slotted, SlotOptions));
                        }
                        else
                        {
                            ItemCollection.Add(new ContentListItem(AssignmentContent, Qmt.Name, Ge.Name, origin, ContentTypes.Slotted, SlotOptions));
                        }

                    }
                }
                

                
            }

            OnPropertyChanged("ItemCollection");
        }
        #endregion

        #region User Actions
        public void ScanFiles(bool overrideScan = false)
        {
            if (overrideScan || !MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem.Scanned)
            {
                //Notifying process start
                EventSystem.Publish<ModalEvent>(new()
                {
                    EventName = "ScanningMod",
                    Title = Properties.Resources.Assignment_Modal_ScanTitle,
                    Content = Properties.Resources.Assignment_Modal_ScanContent,
                    Action = "Show",
                    OkButtonText = Properties.Resources.Modal_Label_DefaultOK,
                    Type = ModalType.Loader,
                });

                //Removing Old Content Items
                foreach (ContentItem ContentItem in MUVM.ContentItems.Where(_ci =>
                    _ci.LibraryItemGuid == MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem.Guid).ToList())
                {
                    MUVM.ContentItems.Remove(ContentItem);
                }

                //Getting and filtering files
                string LibraryContentFolderPath = Properties.QuasarSettings.Default.DefaultDir + "\\Library\\Mods\\" + MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem.Guid + "\\";
                ObservableCollection<ScanFile> Files = FileScanner.GetScanFiles(LibraryContentFolderPath);
                Files = FileScanner.FilterIgnoredFiles(Files);

                //Scanning and matching files to ContentItems
                Files = FileScanner.MatchScanFiles(Files, MUVM.QuasarModTypes, MUVM.Games[0], LibraryContentFolderPath);
                ObservableCollection<ContentItem> Contents = FileScanner.ParseContentItems(Files,
                    MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem);

                //Adding them to the library
                foreach (ContentItem ContentItem in Contents)
                {
                    MUVM.ContentItems.Add((ContentItem));
                }

               

                MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem.Scanned = true;

                //Saving
                UserDataManager.SaveContentItems(MUVM.ContentItems, AppDataPath);
                UserDataManager.SaveLibrary(MUVM.Library, AppDataPath);

                DisplayContentItems(Grouper.GetAssignmentContents(MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem,MUVM.ContentItems, Properties.QuasarSettings.Default.GroupAssignmentTypes).ToList());

                //Notifying process done
                EventSystem.Publish<ModalEvent>(new()
                {
                    EventName = "ScanningMod",
                    Title = Properties.Resources.Assignment_Modal_ScanFinishedTitle,
                    Content = Properties.Resources.Assignment_Modal_ScanFinishedContent,
                    Action = "LoadOK",
                    OkButtonText = Properties.Resources.Modal_Label_DefaultOK,
                    Type = ModalType.Loader,
                });

               
            }
            
        }
        #endregion

        #region Events

        private void ProcessIncomingModalEvent(ContentListItemViewModel _meuh)
        {
            if (_meuh.RequestedAction == "SlotChange")
            {
                string Slot = _meuh.SelectedOption.Key;

                //Getting content items with the same LibraryItem
                List<ContentItem> Matches = MUVM.ContentItems.Where(ci =>
                    ci.LibraryItemGuid == _meuh.AssignmentContent.AssignmentContentItems[0].LibraryItemGuid).ToList();

                //Assigning slots according to user actions
                foreach (ContentItem ContentItem in _meuh.AssignmentContent.AssignmentContentItems)
                {
                    ContentItem.SlotNumber = Slot == "none" ? -1 : int.Parse(Slot);
                    if (ContentItem.SlotNumber != -1)
                    {
                        List<ContentItem> SubMatches = Matches.Where(ci => ci.SlotNumber == ContentItem.SlotNumber && ci.Guid != ContentItem.Guid && ci.QuasarModTypeID == ContentItem.QuasarModTypeID).ToList();
                        foreach (ContentItem SubMatch in SubMatches)
                        {
                            SubMatch.SlotNumber = -1;
                        }
                    }
                }
            }

            if (_meuh.RequestedAction == "ElementChange")
            {
                string Element = _meuh.SelectedOption.Key;

                //Assigning elements according to user actions
                foreach (ContentItem ContentItem in _meuh.AssignmentContent.AssignmentContentItems)
                {
                    ContentItem.GameElementID = Element == "none" ? -1 : int.Parse(Element);
                }
            }

            ObservableCollection<AssignmentContent> AssignmentContents =
                Grouper.GetAssignmentContents(
                    MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem, MUVM.ContentItems,
                    Properties.QuasarSettings.Default.GroupAssignmentTypes);

            DisplayContentItems(AssignmentContents.ToList());

            UserDataManager.SaveContentItems(MUVM.ContentItems, AppDataPath);
        }
        #endregion

    }
}
