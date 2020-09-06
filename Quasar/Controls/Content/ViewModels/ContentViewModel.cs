using Quasar.Controls.Common.Models;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Content.ViewModels
{

    class ContentViewModel : ObservableObject
    {
        #region Fields
        //Working Data
        private ObservableCollection<ContentMapping> _ContentMappings { get; set; }
        private ObservableCollection<LibraryMod> _Mods { get; set; }
        private ObservableCollection<ContentListItem> _ContentListItems { get; set; }

        //References
        private ObservableCollection<InternalModType> _InternalModTypes { get; set; }
        private ObservableCollection<GameData> _GameDatas { get; set; }
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
                OnPropertyChanged("ContentMappings");
            }
        }
        public ObservableCollection<LibraryMod> Mods
        {
            get => _Mods;
            set
            {
                if (_Mods == value)
                    return;

                _Mods = value;
                OnPropertyChanged("Mods");
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
        #endregion

        public ContentViewModel(ObservableCollection<ContentMapping> _ContentMappings, ObservableCollection<LibraryMod> _Mods, ObservableCollection<InternalModType> _InternalModTypes, ObservableCollection<GameData> _GameDatas)
        {
            ContentMappings = _ContentMappings;
            Mods = _Mods;
            InternalModTypes = _InternalModTypes;
            GameDatas = _GameDatas;

            GetContentListItems();
        }

        #region Actions
        public void GetContentListItems()
        {
            ContentListItems = new ObservableCollection<ContentListItem>();
            int modID = 0;
            int colorID = 0;

            foreach (ContentMapping cm in ContentMappings)
            {
                LibraryMod lm = Mods.Single(l => l.ID == cm.ModID);
                InternalModType imt = InternalModTypes.Single(i => i.ID == cm.InternalModType);

                colorID = modID != lm.ID ? colorID == 0 ? 1 : 0 : colorID;
                modID = modID != lm.ID ? lm.ID : modID;

                List<GameDataCategory> gdc = GameDatas.Single(gd => gd.GameID == lm.GameID).Categories;
                ContentListItem cli = new ContentListItem(cm, lm, imt, gdc, colorID);

                ContentListItems.Add(cli);
            }
        }
        #endregion
        
        

    }
}
