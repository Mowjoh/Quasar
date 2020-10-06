using Quasar.Controls.Common.Models;
using Quasar.Internal;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Content.ViewModels
{

    public class ContentViewModel : ObservableObject
    {
        #region Fields
        //Working Data
        private ObservableCollection<ContentMapping> _ContentMappings { get; set; }
        private LibraryMod _LibraryMod { get; set; }
        private ObservableCollection<ContentListItem> _ContentListItems { get; set; }

        //References
        private ObservableCollection<InternalModType> _InternalModTypes { get; set; }
        private ObservableCollection<GameData> _GameDatas { get; set; }

        private ContentListItem _SelectedContentListItem { get; set; }
        #endregion

        #region Properties
        public ObservableCollection<ContentMapping> ContentMappings
        {
            get => _ContentMappings;
            set
            {
                if (_ContentMappings == value)
                    return;

                _ContentMappings = value;
                GetContentListItems();
                OnPropertyChanged("ContentMappings");
            }
        }
        public LibraryMod LibraryMod
        {
            get => _LibraryMod;
            set
            {
                if (_LibraryMod == value)
                    return;

                _LibraryMod = value;
                OnPropertyChanged("LibraryMod");
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
        public ObservableCollection<InternalModType> InternalModTypes
        {
            get => _InternalModTypes;
            set
            {
                if (_InternalModTypes == value)
                    return;

                _InternalModTypes = value;
                OnPropertyChanged("InternalModTypes");
            }
        }
        public ObservableCollection<GameData> GameDatas
        {
            get => _GameDatas;
            set
            {
                if (_GameDatas == value)
                    return;

                _GameDatas = value;
                OnPropertyChanged("GameDatas");
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

        public ContentViewModel(ObservableCollection<ContentMapping> _ContentMappings, LibraryMod _LibraryMod, ObservableCollection<InternalModType> _InternalModTypes, ObservableCollection<GameData> _GameDatas)
        {
            LibraryMod = _LibraryMod;
            InternalModTypes = _InternalModTypes;
            GameDatas = _GameDatas;
            ContentMappings = _ContentMappings;

            EventSystem.Subscribe<string>(GetRefreshed);
            

        }

        #region Actions
        public void GetContentListItems()
        {
            ContentListItems = new ObservableCollection<ContentListItem>();
            int modID = 0;
            int colorID = 0;
            
            foreach (ContentMapping cm in ContentMappings)
            {
                if(cm.ModID == LibraryMod.ID)
                {
                    InternalModType imt = InternalModTypes.Single(i => i.ID == cm.InternalModType);

                    colorID = modID != LibraryMod.ID ? colorID == 0 ? 1 : 0 : colorID;
                    modID = modID != LibraryMod.ID ? LibraryMod.ID : modID;

                    List<GameDataCategory> gdc = GameDatas.Single(gd => gd.GameID == LibraryMod.GameID).Categories;
                    ContentListItem cli = new ContentListItem(cm, LibraryMod, imt, gdc, colorID);

                    ContentListItems.Add(cli);
                }
            }
        }

        public void GetRefreshed(string Action)
        {
            if(Action == "RefreshContents")
            {
                GetContentListItems();
            }
        }
        #endregion
        
        

    }
}
