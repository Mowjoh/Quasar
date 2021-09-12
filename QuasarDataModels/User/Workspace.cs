using DataModels.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.User
{
    public class Workspace : ObservableObject
    {
        #region Private properties
        [Newtonsoft.Json.JsonIgnore]
        private Guid _Guid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private string _Name { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private string _BuildDate { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private bool _Shared { get; set; } = false;
        [Newtonsoft.Json.JsonIgnore]
        private Guid _UniqueShareID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private ObservableCollection<Association> _Associations { get; set; }
        #endregion

        public Guid Guid
        {
            get => _Guid;
            set
            {
                _Guid = value;
                OnPropertyChanged("Guid");
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
        public Guid UniqueShareID
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
        private Guid _ContentItemGuid { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private int _GameElementID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private int _QuasarModTypeID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private int _SlotNumber { get; set; } = 0;
        #endregion

        public Guid ContentItemGuid
        {
            get => _ContentItemGuid;
            set
            {
                _ContentItemGuid = value;
                OnPropertyChanged("ContentItemGuid");
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
