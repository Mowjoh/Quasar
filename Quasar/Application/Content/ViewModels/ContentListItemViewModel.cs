using Quasar.Controls.Common.Models;
using Quasar.Data.V1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quasar.Controls.Content.ViewModels
{
    public class ContentListItemViewModel : ObservableObject
    {
        #region Fields
        private ContentMapping _ContentMapping { get; set; }
        private LibraryMod _LibraryMod { get; set; }
        private InternalModType _InternalModType { get; set; }
        private GameData _GameData { get; set; }
        private GameDataCategory _GameDataCategory { get; set; }
        private GameDataItem _GameDataItem { get; set; }


        private string _Text { get; set; }
        private int _ColorValue { get; set; }
        private bool _Smol { get; set; }
        private Rect _Rekt { get; set; }
        #endregion

        #region Properties
        public ContentMapping ContentMapping
        {
            get => _ContentMapping;
            set
            {
                if (_ContentMapping == value)
                    return;

                _ContentMapping = value;
                OnPropertyChanged("ContentMapping");
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
        public InternalModType InternalModType
        {
            get => _InternalModType;
            set
            {
                if (_InternalModType == value)
                    return;

                _InternalModType = value;
                OnPropertyChanged("InternalModType");
            }
        }
        public GameData GameData
        {
            get => _GameData;
            set
            {
                if (_GameData == value)
                    return;

                _GameData = value;
                OnPropertyChanged("GameData");
            }
        }
        public GameDataCategory GameDataCategory
        {
            get => _GameDataCategory;
            set
            {
                if (_GameDataCategory == value)
                    return;

                _GameDataCategory = value;
                OnPropertyChanged("GameDataCategory");
            }
        }
        public GameDataItem GameDataItem
        {
            get => _GameDataItem;
            set
            {
                if (_GameDataItem == value)
                    return;

                _GameDataItem = value;
                OnPropertyChanged("GameDataItem");
            }
        }

        public string Text
        {
            get => _Text;
            set
            {
                if (_Text == value)
                    return;

                _Text = value;
                OnPropertyChanged("Text");
            }
        }
        public int ColorValue
        {
            get => _ColorValue;
            set
            {
                if (_ColorValue == value)
                    return;

                _ColorValue = value;
                OnPropertyChanged("ColorValue");
            }
        }
        public bool Smol
        {
            get
            {
                return _Smol;
            }
            set
            {
                if (_Smol == value)
                    return;

                _Smol = value;

                Rekt = value ? new Rect(0, 0, 50, 44) : new Rect(0, 0, 50, 190);
                OnPropertyChanged("Smol");
            }
        }
        public Rect Rekt
        {
            get => _Rekt;
            set
            {
                if (_Rekt == value)
                    return;

                _Rekt = value;
                OnPropertyChanged("Rekt");
            }
        }
        #endregion

        public ContentListItemViewModel(ContentMapping _ContentMapping, LibraryMod _LibraryMod, InternalModType _InternalModType, List<GameDataCategory> Categories, int colorID)
        {
            ContentMapping = _ContentMapping;
            LibraryMod = _LibraryMod;
            InternalModType = _InternalModType;

            GameDataCategory = Categories.Single(gd => gd.ID == InternalModType.GameID);
            GameDataItem = GameDataCategory.Items.Find(g => g.ID == ContentMapping.GameDataItemID);

            //Displaying files
            foreach (ContentMappingFile cmf in ContentMapping.Files)
            {
                Text += cmf.SourcePath + "\r\n";
            }

            ColorValue = colorID;

            Smol = true;
        }

        #region Actions

        #endregion
    }
}
