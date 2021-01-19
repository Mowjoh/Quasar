using Quasar.Controls.Common.Models;
using Quasar.Data.V2;
using Quasar.Helpers.Json;
using Quasar.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quasar.Controls.Content.ViewModels
{

    public class ContentViewModel : ObservableObject
    {
        #region Fields
        //Working Data
        private ObservableCollection<ContentItem> _ContentItems { get; set; }
        private LibraryItem _LibraryItem { get; set; }
        private ObservableCollection<ContentListItem> _ContentListItems { get; set; }

        //References
        private ObservableCollection<QuasarModType> _QuasarModTypes { get; set; }
        private ObservableCollection<GameElementFamily> _GameElementFamilies { get; set; }

        private ContentListItem _SelectedContentListItem { get; set; }
        private MainUIViewModel _MUVM { get; set; }

        private string _ModName { get; set; }
        private ICommand _SaveModNameCommand { get; set; }
        #endregion

        #region Properties
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

        public string ModName
        {
            get => _ModName;
            set
            {
                _ModName = value;
                OnPropertyChanged("ModName");
            }
        }
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
        #endregion

        public ContentViewModel(MainUIViewModel _MUVM)
        {
            MUVM = _MUVM;
            

            EventSystem.Subscribe<string>(GetRefreshed);
            

        }

        #region Actions
        public void GetContentListItems()
        {
            ContentListItems = new ObservableCollection<ContentListItem>();
            int modID = 0;
            int colorID = 0;
            
            foreach (ContentItem ci in MUVM.ContentItems)
            {
                if(LibraryItem != null)
                {
                    if (ci.LibraryItemID == LibraryItem.ID)
                    {
                        QuasarModType qmt = MUVM.QuasarModTypes.Single(i => i.ID == ci.QuasarModTypeID);

                        colorID = modID != LibraryItem.ID ? colorID == 0 ? 1 : 0 : colorID;
                        modID = modID != LibraryItem.ID ? LibraryItem.ID : modID;

                        GameElementFamily Family = MUVM.Games[0].GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID);
                        ContentListItem cli = new ContentListItem(ci, LibraryItem, qmt, Family.GameElements.ToList(), colorID);

                        ContentListItems.Add(cli);
                    }
                }
            }
        }

        public void GetRefreshed(string Action)
        {
            if(Action == "RefreshContents")
            {
                if(MUVM.SelectedModListItem != null)
                {
                    LibraryItem = MUVM.SelectedModListItem.ModListItemViewModel.LibraryItem;
                    ModName = LibraryItem.Name;
                }
                
                GetContentListItems();
            }
        }

        public void SaveModName()
        {
            LibraryItem.Name = ModName;
            GetContentListItems();
            JSonHelper.SaveLibrary(MUVM.Library);
        }
        #endregion
        
        

    }
}
