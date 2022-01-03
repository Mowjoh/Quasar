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
                
                Properties.Settings.Default.GroupAssignmentTypes = value;
                Properties.Settings.Default.Save();
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
                    _ScanMods = new RelayCommand(_param => ScanFiles());
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
            GroupedTypes = Properties.Settings.Default.GroupAssignmentTypes;
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

                if (Qmt.ID == 8)
                {
                    List<Option> MusicOptions = new();
                    foreach (GameElement GameElement in MUVM.Games[0].GameElementFamilies[2].GameElements)
                    {
                        MusicOptions.Add(new Option() { Key = GameElement.ID.ToString(), Value = GameElement.Name });
                    }
                    
                    ItemCollection.Add(new ContentListItem(AssignmentContent, Qmt.Name, Ge.Name, ContentTypes.ElementSelected, MusicOptions));
                }
                else
                {
                    List<Option> SlotOptions = new();
                    for (int i = 0; i < 8; i++)
                    {
                        SlotOptions.Add(new Option() { Key = i.ToString(), Value = String.Format("Slot {0}", (i + 1)) });
                    }

                    if (!AssignmentContent.Single)
                    {
                        ItemCollection.Add(new ContentListItem(AssignmentContent, Qmt.GroupName, Ge.Name, ContentTypes.Slotted, SlotOptions));
                    }
                    else
                    {
                        ItemCollection.Add(new ContentListItem(AssignmentContent, Qmt.Name, Ge.Name, ContentTypes.Slotted, SlotOptions));
                    }
                    
                }

                
            }

            OnPropertyChanged("ItemCollection");
        }
        #endregion

        #region User Actions
        public void ScanFiles()
        {
            //Notifying process start
            EventSystem.Publish<ModalEvent>(new()
            {
                EventName = "ScanningMod",
                Title = "Scanning",
                Content = "Please wait while Quasar scans the mod's contents",
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
            string LibraryContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\" + MUVM.LibraryViewModel.SelectedModListItem.ModViewModel.LibraryItem.Guid + "\\";
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

            //Saving
            UserDataManager.SaveContentItems(MUVM.ContentItems, AppDataPath);

            //Notifying process done
            EventSystem.Publish<ModalEvent>(new()
            {
                EventName = "ScanningMod",
                Title = "Scan Finished",
                Content = "Quasar has scanned the mod and it's \reditable contents will be displayed here",
                Action = "LoadOK",
                OkButtonText = Properties.Resources.Modal_Label_DefaultOK,
                Type = ModalType.Loader,
            });
        }
        #endregion

        #region Events

        private void ProcessIncomingModalEvent(ModalEvent _meuh)
        {

        }
        #endregion

    }
}
