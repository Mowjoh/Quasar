using Quasar.Controls.Common.Models;
using Quasar.Data.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Associations.ViewModels
{
    public class SlotItemViewModel : ObservableObject
    {
        #region fields
        private string _ContentName { get; set; }
        private int _SlotNumber { get; set; }
        private int _Index { get; set; }
        private string _SlotNumberName { get; set; }
        private bool _EmptySlot { get; set; }
        private string _TypeName { get; set; }
        private Association _Association { get; set; }
        private List<ContentItem> _ContentItems { get; set; }
        #endregion

        #region Properties
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
            }
        }
        #endregion

        public SlotItemViewModel()
        {

        }

        #region Actions

        #endregion
    }
}
