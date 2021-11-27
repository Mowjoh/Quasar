using Quasar.Common.Models;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.FileSystem;
using Quasar.Helpers.ModScanning;
using Quasar.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Quasar.MainUI.ViewModels;
using Quasar.Content.Views;
using Ookii.Dialogs.Wpf;
using Workshop.FileManagement;

namespace Quasar.Content.ViewModels
{

    public class ContentViewModel : ObservableObject
    {
        public static string AppDataPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        #region Data

        #region Private
        //Working Data
        private ObservableCollection<ContentItem> _ContentItems { get; set; }
        private LibraryItem _LibraryItem { get; set; }
        private ObservableCollection<ContentListItem> _ContentListItems { get; set; }

        //References
        private ObservableCollection<QuasarModType> _QuasarModTypes { get; set; }
        private ObservableCollection<GameElementFamily> _GameElementFamilies { get; set; }

        private ContentListItem _SelectedContentListItem { get; set; }
        private MainUIViewModel _MUVM { get; set; }
        #endregion

        #region Public
        public ObservableCollection<ContentItem> ContentItems
        {
            get => _ContentItems;
            set
            {
                if (_ContentItems == value)
                    return;

                _ContentItems = value;
                GetContentListItems();
                OnPropertyChanged("ContentItems");
            }
        }
        public LibraryItem LibraryItem
        {
            get => _LibraryItem;
            set
            {
                if (_LibraryItem == value)
                    return;

                _LibraryItem = value;
                OnPropertyChanged("LibraryItem");
            }
        }
        public ObservableCollection<ContentListItem> ContentListItems
        {
            get => _ContentListItems;
            set
            {
                if (_ContentListItems == value)
                    return;

                _ContentListItems = value;

                OnPropertyChanged("ContentListItems");
            }
        }
        public ObservableCollection<QuasarModType> QuasarModTypes
        {
            get => _QuasarModTypes;
            set
            {
                if (_QuasarModTypes == value)
                    return;

                _QuasarModTypes = value;
                OnPropertyChanged("QuasarModTypes");
            }
        }
        public ObservableCollection<GameElementFamily> GameElementFamilies
        {
            get => _GameElementFamilies;
            set
            {
                if (_GameElementFamilies == value)
                    return;

                _GameElementFamilies = value;
                OnPropertyChanged("GameElementFamilies");
            }
        }
        public MainUIViewModel MUVM
        {
            get => _MUVM;
            set
            {
                if (_MUVM == value)
                    return;

                _MUVM = value;
                OnPropertyChanged("MUVM");
            }
        }
        public ContentListItem SelectedContentListItem
        {
            get => _SelectedContentListItem;
            set
            {
                if (_SelectedContentListItem == value)
                    return;

                if (_SelectedContentListItem != null)
                {
                    _SelectedContentListItem.CLIVM.Smol = true;
                }

                _SelectedContentListItem = value;
                if (_SelectedContentListItem != null)
                {
                    _SelectedContentListItem.CLIVM.Smol = false;
                }

                _SelectedContentListItem = value;
                OnPropertyChanged("SelectedContentListItem");
            }
        }
        #endregion

        #endregion

        #region View

        #region Private
        private string _ModName { get; set; }
        private bool _GroupRenaming { get; set; }
        #endregion

        #region Public
        public string ModName
        {
            get => _ModName;
            set
            {
                _ModName = value;
                OnPropertyChanged("ModName");
            }
        }

        public bool GroupRenaming
        {
            get => _GroupRenaming;
            set
            {
                _GroupRenaming = value;
                OnPropertyChanged("GroupRenaming");
            }
        }
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _SaveModNameCommand { get; set; }

        private ICommand _PickModFilesCommand { get; set; }
        #endregion

        #region Public
        public ICommand SaveModNameCommand
        {
            get
            {
                if (_SaveModNameCommand == null)
                {
                    _SaveModNameCommand = new RelayCommand(param => SaveModName());
                }
                return _SaveModNameCommand;
            }
        }

        public ICommand PickModFilesCommand
        {
            get
            {
                if (_PickModFilesCommand == null)
                {
                    _PickModFilesCommand = new RelayCommand(param => PickModFiles());
                }
                return _PickModFilesCommand;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Basic Content View Model constructor
        /// </summary>
        /// <param name="_MUVM">MainUI View Model to link</param>
        public ContentViewModel(MainUIViewModel _MUVM)
        {
            MUVM = _MUVM;

            EventSystem.Subscribe<string>(GetRefreshed);
            EventSystem.Subscribe<ContentItem>(GroupRename);

        }

        #region Actions

        /// <summary>
        /// Parses all the corresponding Content List Items
        /// </summary>
        public void GetContentListItems()
        {
            ContentListItems = new ObservableCollection<ContentListItem>();
            int modID = 0;
            int colorID = 0;
            
            foreach (ContentItem ci in MUVM.ContentItems)
            {
                if(LibraryItem != null)
                {
                    if (ci.LibraryItemGuid == LibraryItem.Guid)
                    {
                        QuasarModType qmt = MUVM.QuasarModTypes.Single(i => i.ID == ci.QuasarModTypeID);

                        //colorID = modID != LibraryItem.Guid ? colorID == 0 ? 1 : 0 : colorID;
                        //modID = modID != LibraryItem.Guid ? LibraryItem.ID : modID;

                        GameElementFamily Family = MUVM.Games[0].GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID);
                        ContentListItem cli = new ContentListItem(ci, LibraryItem, qmt, Family.GameElements.ToList(), colorID);

                        ContentListItems.Add(cli);
                    }
                }
            }
        }

        #endregion

        #region User Action

        /// <summary>
        /// Opens a folder browser to pick a new source for this mod's files
        /// </summary>
        public void PickModFiles()
        {
            ModFileManager FileManager = new ModFileManager(LibraryItem);

            VistaFolderBrowserDialog newDialog = new VistaFolderBrowserDialog();
            newDialog.ShowDialog();

            if (newDialog.SelectedPath == "")
                return;

            //Importing files
            string NewInstallPath = newDialog.SelectedPath;
            FileManager.ImportFolder(NewInstallPath);

            //Launching scan
            Scannerino.UpdateContents(MUVM, LibraryItem, Scannerino.ScanMod(FileManager.LibraryContentFolderPath, MUVM.QuasarModTypes, MUVM.Games[0], LibraryItem));

            //Saving Contents
            UserDataManager.SaveContentItems(MUVM.ContentItems, AppDataPath);

            GetContentListItems();
        }

        /// <summary>
        /// Saves a mod's name
        /// </summary>
        public void SaveModName()
        {
            LibraryItem.Name = ModName;
            EventSystem.Publish<LibraryItem>(LibraryItem);
            GetContentListItems();
            UserDataManager.SaveLibrary(MUVM.Library, AppDataPath);
        }

        #endregion

        #region Events
        /// <summary>
        /// Refreshes the UI
        /// </summary>
        /// <param name="Action">Action to specify</param>
        public void GetRefreshed(string Action)
        {
            if (Action == "RefreshContents")
            {
                if (MUVM.SelectedModListItem != null)
                {
                    LibraryItem = MUVM.SelectedModListItem.ModViewModel.LibraryItem;
                    ModName = LibraryItem.Name;
                }

                GetContentListItems();
            }
        }

        /// <summary>
        /// Function that renames all matching Content Items
        /// </summary>
        /// <param name="ci">Content Item to base the rename on</param>
        public void GroupRename(ContentItem ci)
        {
            QuasarModType qmtci = MUVM.QuasarModTypes.Single(q => q.ID == ci.QuasarModTypeID);

            if (GroupRenaming)
            {

                foreach (ContentListItem c in ContentListItems)
                {

                    QuasarModType qmtc = MUVM.QuasarModTypes.Single(q => q.ID == c.CLIVM.ContentItem.QuasarModTypeID);
                    if (c.CLIVM.ContentItem.SlotNumber == ci.SlotNumber && c.CLIVM.ContentItem.GameElementID == ci.GameElementID && qmtc.GroupName == qmtci.GroupName)
                    {
                        c.CLIVM.ContentItem.Name = ci.Name;
                        c.CLIVM.RefreshName();
                    }
                }
            }
        }
        #endregion

    }
}
