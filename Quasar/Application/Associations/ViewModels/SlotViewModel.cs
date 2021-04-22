using Quasar.Common.Models;
using Quasar.Data.V2;
using Quasar.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Quasar.Associations.ViewModels
{
    public class SlotViewModel : ObservableObject
    {
        #region Data

        #region Private
        private Association _Association { get; set; }
        private List<ContentItem> _ContentItems { get; set; }
        #endregion

        #region Public
        public Association Association
        {
            get => _Association;
            set
            {
                if (_Association == value)
                    return;

                _Association = value;
                OnPropertyChanged("Association");
            }
        }
        public List<ContentItem> ContentItems
        {
            get => _ContentItems;
            set
            {
                if (_ContentItems == value)
                    return;

                _ContentItems = value;
                OnPropertyChanged("ContentItems");
                OnPropertyChanged("SlotText");
            }
        }
        public ObservableCollection<QuasarModType> QuasarModTypes { get; set; }
        #endregion

        #endregion

        #region View

        #region Private
        private string _ContentName { get; set; }
        private int _SlotNumber { get; set; }
        private int _Index { get; set; }
        private string _SlotNumberName { get; set; }
        private bool _EmptySlot { get; set; }
        private string _TypeName { get; set; }
        #endregion

        #region Public
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
        public int SlotNumber
        {
            get => _SlotNumber;
            set
            {
                if (_SlotNumber == value)
                    return;

                _SlotNumber = value;
                OnPropertyChanged("SlotNumber");
            }
        }
        public int Index
        {
            get => _Index;
            set
            {
                if (_Index == value)
                    return;

                _Index = value;
                OnPropertyChanged("Index");
            }
        }
        public string SlotNumberName
        {
            get => _SlotNumberName;
            set
            {
                if (_SlotNumberName == value)
                    return;

                _SlotNumberName = value;
                OnPropertyChanged("SlotNumberName");
            }
        }
        public bool EmptySlot
        {
            get => _EmptySlot;
            set
            {
                if (_EmptySlot == value)
                    return;

                _EmptySlot = value;
                OnPropertyChanged("EmptySlot");
            }
        }
        public string TypeName
        {
            get => _TypeName;
            set
            {
                if (_TypeName == value)
                    return;

                _TypeName = value;
                OnPropertyChanged("TypeName");
            }
        }
        public string SlotText
        {
            get => getSlotText();
        }
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _DeleteAssociationCommand { get; set; }
        #endregion

        #region Public
        public ICommand DeleteAssociationCommand
        {
            get
            {
                if (_DeleteAssociationCommand == null)
                {
                    _DeleteAssociationCommand = new RelayCommand(param => DeleteAssociations());
                }
                return _DeleteAssociationCommand;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Slot View Model Constructor
        /// </summary>
        public SlotViewModel()
        {

        }

        #region Actions
        /// <summary>
        /// Gets the displayed slot Text
        /// </summary>
        /// <returns>The formatted Slot Text</returns>
        public string getSlotText()
        {
            string SlotText = "";
            if(ContentItems != null)
            {
                foreach (ContentItem ci in ContentItems)
                {
                    QuasarModType qmt = QuasarModTypes.Single(q => q.ID == ci.QuasarModTypeID);
                    SlotText += String.Format("{0} - {1} - {2}\r", (ci.SlotNumber+1).ToString(), qmt.Name, ci.Name);
                }
            }
            
            return SlotText;
        }
        #endregion

        #region User Actions
        /// <summary>
        /// Triggers an event to delete all Associations related to this slot
        /// </summary>
        public void DeleteAssociations()
        {
            EventSystem.Publish<SlotViewModel>(this);
        }
        #endregion
    }
}
