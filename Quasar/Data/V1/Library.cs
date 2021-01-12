using Quasar.Controls.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Quasar.Data.V1
{
    [Serializable()]
    [XmlRoot("Library")]
    public class LibModList : ICollection
    {
        public string CollectionName;
        public ArrayList empArray = new ArrayList();

        public LibModList()
        {

        }

        public LibModList(List<LibraryMod> _inputList)
        {
            foreach (LibraryMod m in _inputList)
            {
                empArray.Add(m);
            }
        }

        public LibraryMod this[int index]
        {
            get { return (LibraryMod)empArray[index]; }
        }
        public void CopyTo(Array a, int index)
        {
            empArray.CopyTo(a, index);
        }
        public int Count
        {
            get { return empArray.Count; }
        }
        public object SyncRoot
        {
            get { return this; }
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public IEnumerator GetEnumerator()
        {
            return empArray.GetEnumerator();
        }

        public void Add(LibraryMod newMod)
        {
            empArray.Add(newMod);
        }

    }

    [Serializable()]
    public class LibraryMod : ObservableObject
    {
        #region Fields
        private int _ID { get; set; }
        private int _GameID { get; set; }
        private int _TypeID { get; set; }
        private string _TypeLabel { get; set; }
        private int _APICategoryID { get; set; }
        private string _APICategoryName { get; set; }
        private bool _FinishedProcessing { get; set; }
        private string _Name { get; set; }
        private string[][] _Authors { get; set; }
        private int _Updates { get; set; }
        private bool _Native { get; set; }
        private bool _Ready { get; set; }
        #endregion

        #region Properties
        [XmlAttribute("id")]
        public int ID
        {
            get => _ID;
            set
            {
                if (_ID == value)
                    return;

                _ID = value;
                OnPropertyChanged("ID");
            }
        }

        [XmlAttribute("GameID")]
        public int GameID
        {
            get => _GameID;
            set
            {
                if (_GameID == value)
                    return;

                _GameID = value;
                OnPropertyChanged("GameID");
            }
        }

        [XmlAttribute("type")]
        public int TypeID
        {
            get => _TypeID;
            set
            {
                if (_TypeID == value)
                    return;

                _TypeID = value;
                OnPropertyChanged("TypeID");
            }
        }

        [XmlAttribute("typeName")]
        public string TypeLabel
        {
            get => _TypeLabel;
            set
            {
                if (_TypeLabel == value)
                    return;

                _TypeLabel = value;
                OnPropertyChanged("TypeLabel");
            }
        }

        [XmlAttribute("category")]
        public int APICategoryID
        {
            get => _APICategoryID;
            set
            {
                if (_APICategoryID == value)
                    return;

                _APICategoryID = value;
                OnPropertyChanged("APICategoryID");
            }
        }

        [XmlAttribute("categoryName")]
        public string APICategoryName
        {
            get => _APICategoryName;
            set
            {
                if (_APICategoryName == value)
                    return;

                _APICategoryName = value;
                OnPropertyChanged("APICategoryName");
            }
        }

        [XmlAttribute("processed")]
        public bool FinishedProcessing
        {
            get => _FinishedProcessing;
            set
            {
                if (_FinishedProcessing == value)
                    return;

                _FinishedProcessing = value;
                OnPropertyChanged("FinishedProcessing");
            }
        }
        [XmlElement("Name")]
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name == value)
                    return;

                _Name = value;
                OnPropertyChanged("Name");
            }
        }

        [XmlElement(Type = typeof(string[][]), ElementName = "Authors")]
        public string[][] Authors
        {
            get => _Authors;
            set
            {
                if (_Authors == value)
                    return;

                _Authors = value;
                OnPropertyChanged("Authors");
            }
        }


        [XmlElement("Updates")]
        public int Updates
        {
            get => _Updates;
            set
            {
                if (_Updates == value)
                    return;

                _Updates = value;
                OnPropertyChanged("Updates");
            }
        }

        [XmlElement("Native")]
        public bool Native
        {
            get => _Native;
            set
            {
                if (_Native == value)
                    return;

                _Native = value;
                OnPropertyChanged("Native");
            }
        }

        public bool Ready { get; set; }
        #endregion


        public LibraryMod(int _id, string _Name, int _type, bool _processed, string[][] _authors, int _updates, bool _native, int _category)
        {
            ID = _id;
            Name = _Name;
            TypeID = _type;
            FinishedProcessing = _processed;
            Authors = _authors;
            Updates = _updates;
            Native = _native;
            APICategoryID = _category;
        }

        public LibraryMod(int _id, int _type, bool _ready)
        {
            ID = _id;
            TypeID = _type;
            Ready = _ready;
        }

        public LibraryMod()
        {

        }
    }
}
