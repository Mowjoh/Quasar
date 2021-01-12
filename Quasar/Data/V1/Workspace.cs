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
    //XML Serialization elements
    [Serializable()]
    [XmlRoot("Workspaces")]
    public class Workspaces : ICollection
    {
        public string CollectionName;
        public ArrayList empArray = new ArrayList();

        public Workspaces()
        {

        }

        public Workspaces(List<Workspace> _inputList)
        {
            foreach (Workspace m in _inputList)
            {
                empArray.Add(m);
            }
        }

        public Workspace this[int index]
        {
            get { return (Workspace)empArray[index]; }
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

        public void Add(Workspace newWorkspace)
        {
            empArray.Add(newWorkspace);
        }

    }


    [Serializable]
    public class Workspace : ObservableObject
    {
        #region Fields
        private int _ID { get; set; }
        private string _Name { get; set; }
        private string _BuildDate { get; set; }
        private bool _Built { get; set; }
        private List<Association> _Associations { get; set; }
        #endregion


        [XmlAttribute("ID")]
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

        [XmlAttribute("Name")]
        public String Name
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

        [XmlAttribute("BuildDate")]
        public string BuildDate
        {
            get => _BuildDate;
            set
            {
                if (_BuildDate == value)
                    return;

                _BuildDate = value;
                OnPropertyChanged("BuildDate");
            }
        }

        [XmlAttribute("Built")]
        public bool Built
        {
            get => _Built;
            set
            {
                if (_Built == value)
                    return;

                _Built = value;
                OnPropertyChanged("Built");
            }
        }

        [XmlElement("Association")]
        public List<Association> Associations
        {
            get => _Associations;
            set
            {
                if (_Associations == value)
                    return;

                _Associations = value;
                OnPropertyChanged("Associations");
            }
        }
    }

    [Serializable]
    public class Association
    {
        [XmlAttribute("Slot")]
        public int Slot { get; set; }

        [XmlAttribute("ContentMappingID")]
        public int ContentMappingID { get; set; }

        [XmlAttribute("GameDataItemID")]
        public int GameDataItemID { get; set; }

        [XmlAttribute("InternalModTypeID")]
        public int InternalModTypeID { get; set; }

    }
}
