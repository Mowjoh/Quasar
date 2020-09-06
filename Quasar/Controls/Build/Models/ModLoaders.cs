using Quasar.Controls.Common.Models;
using System.Collections.Generic;
using System.Security;
using System.Xml.Serialization;

namespace Quasar
{

    [XmlRoot("ModLoaders")]
    public class ModLoaders
    {
        [XmlElement("ModLoader")]
        public List<ModLoader> ModLoaderList { get; set; }
    }


    public class ModLoader : ObservableObject
    {
        #region Fields
        private int _ID { get; set; }
        private int _GameID { get; set; }
        private string _Name { get; set; }
        private string _BasePath { get; set; }
        private string _InstallPath { get; set; }
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

        [XmlAttribute("GameID")]
        public int GameID
        {
            get => _GameID;
            set
            {
                if (_ID == value)
                    return;

                _ID = value;
                OnPropertyChanged("ID");
            }
        }

        [XmlAttribute("Name")]
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

        [XmlAttribute("BasePath")]
        public string BasePath
        {
            get => _BasePath;
            set
            {
                if (_BasePath == value)
                    return;

                _BasePath = value;
                OnPropertyChanged("BasePath");
            }
        }

        [XmlAttribute("InstallPath")]
        public string InstallPath
        {
            get => _InstallPath;
            set
            {
                if (_InstallPath == value)
                    return;

                _InstallPath = value;
                OnPropertyChanged("InstallPath");
            }
        }
    }
}
