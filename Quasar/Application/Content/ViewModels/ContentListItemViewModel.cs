using Quasar.Common.Models;
using Quasar.Data.V2;
using Quasar.Internal;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Quasar.Content.ViewModels
{
    public class ContentListItemViewModel : ObservableObject
    {
        #region Data

        #region Private
        private ContentItem _ContentItem { get; set; }
        private LibraryItem _LibraryItem { get; set; }
        private QuasarModType _QuasarModType { get; set; }
        private Game _Game { get; set; }
        private List<GameElement> _GameElements { get; set; }
        private GameElement _GameElement { get; set; }
        #endregion

        #region Public
        public ContentItem ContentItem
        {
            get => _ContentItem;
            set
            {
                if (_ContentItem == value)
                    return;

                _ContentItem = value;
                OnPropertyChanged("ContentItem");

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
        public QuasarModType QuasarModType
        {
            get => _QuasarModType;
            set
            {
                if (_QuasarModType == value)
                    return;

                _QuasarModType = value;
                OnPropertyChanged("QuasarModType");
            }
        }
        public List<GameElement> GameElements
        {
            get => _GameElements;
            set
            {
                if (_GameElements == value)
                    return;

                _GameElements = value;
                OnPropertyChanged("GameElements");
            }
        }
        public GameElement GameElement
        {
            get => _GameElement;
            set
            {
                if (_GameElement == value)
                    return;

                _GameElement = value;
                OnPropertyChanged("GameElement");
            }
        }

        #endregion

        #endregion

        #region View

        #region Private
        private string _Text { get; set; }
        private string _ContentName { get; set; }
        private int _ColorValue { get; set; }
        private bool _Smol { get; set; }
        private Rect _Rekt { get; set; }
        #endregion

        #region Public
        public int HumanReadableSlotNumber => ContentItem.SlotNumber + 1;
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
        public string ContentName
        {
            get => _ContentName;
            set
            {
                if (_ContentName == value)
                    return;

                _ContentName = value;
                OnPropertyChanged("ContentName");
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

        #endregion

        #region Commands

        #region Private
        private ICommand _SaveContentNameCommand { get; set; }
        #endregion

        #region Public
        public ICommand SaveContentNameCommand
        {
            get
            {
                if (_SaveContentNameCommand == null)
                {
                    _SaveContentNameCommand = new RelayCommand(param => SaveContentName());
                }
                return _SaveContentNameCommand;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Content List Item View Model
        /// </summary>
        /// <param name="_ContentItem">Linked Content Item</param>
        /// <param name="_LibraryItem">Linked Library Item</param>
        /// <param name="_QuasarModType">Linked Quasar Mod Type</param>
        /// <param name="_GameElements">Game Elements corresponding to the type</param>
        /// <param name="colorID">Color ID</param>
        public ContentListItemViewModel(ContentItem _ContentItem, LibraryItem _LibraryItem, QuasarModType _QuasarModType, List<GameElement> _GameElements, int colorID)
        {
            ContentItem = _ContentItem;
            LibraryItem = _LibraryItem;
            QuasarModType = _QuasarModType;
            GameElements = _GameElements;
            ContentName = ContentItem.Name;

            GameElement = GameElements.Find(g => g.ID == ContentItem.GameElementID);

            ColorValue = colorID;

            Smol = true;
        }

        #region Actions
        /// <summary>
        /// Saves the Content's name
        /// </summary>
        public void SaveContentName()
        {
            ContentItem.Name = ContentName;
            OnPropertyChanged("ContentItem");
            OnPropertyChanged("ContentName");
            EventSystem.Publish<ContentItem>(ContentItem);
        }

        /// <summary>
        /// Triggers UI Refresh for the name
        /// </summary>
        public void RefreshName()
        {
            ContentName = ContentItem.Name;
            OnPropertyChanged("ContentItem");
            OnPropertyChanged("ContentName");
        }
        #endregion
    }
}
