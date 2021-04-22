using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Data.V2
{
    public class Workspace : ObservableObject
    {
        #region Private properties
        [Newtonsoft.Json.JsonIgnore]
        private int _ID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private string _Name { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private string _BuildDate { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private bool _Shared { get; set; } = false;
        [Newtonsoft.Json.JsonIgnore]
        private string _UniqueShareID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private ObservableCollection<Association> _Associations { get; set; }
        #endregion

        public int ID
        {
            get => _ID;
            set
            {
                _ID = value;
                OnPropertyChanged("ID");
            }
        }
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        public string BuildDate
        {
            get => _BuildDate;
            set
            {
                _BuildDate = value;
                OnPropertyChanged("BuildDate");
            }
        }
        public bool Shared
        {
            get => _Shared;
            set
            {
                _Shared = value;
                OnPropertyChanged("Shared");
            }
        }
        public string UniqueShareID
        {
            get => _UniqueShareID;
            set
            {
                _UniqueShareID = value;
                OnPropertyChanged("UniqueShareID");
            }
        }
        public ObservableCollection<Association> Associations
        {
            get => _Associations;
            set
            {
                _Associations = value;
                OnPropertyChanged("Associations");
            }
        }
    }

    public class Association : ObservableObject
    {
        #region Private properties
        [Newtonsoft.Json.JsonIgnore]
        private int _ContentItemID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private int _GameElementID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private int _QuasarModTypeID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private int _SlotNumber { get; set; } = 0;
        #endregion

        public int ContentItemID
        {
            get => _ContentItemID;
            set
            {
                _ContentItemID = value;
                OnPropertyChanged("ContentItemID");
            }
        }
        public int GameElementID
        {
            get => _GameElementID;
            set
            {
                _GameElementID = value;
                OnPropertyChanged("GameElementID");
            }
        }
        public int QuasarModTypeID
        {
            get => _QuasarModTypeID;
            set
            {
                _QuasarModTypeID = value;
                OnPropertyChanged("QuasarModTypeID");
            }
        }
        public int SlotNumber
        {
            get => _SlotNumber;
            set
            {
                _SlotNumber = value;
                OnPropertyChanged("SlotNumber");
                OnPropertyChanged("HumanReadableSlotNumber");
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public int HumanReadableSlotNumber => SlotNumber +1;
    }
}
