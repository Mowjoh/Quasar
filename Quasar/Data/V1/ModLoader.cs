using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;

namespace Quasar.Data.V1
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
        private int _ModLoaderID { get; set; }
        private int _GameID { get; set; }
        private string _Name { get; set; }
        private string _BasePath { get; set; }
        private string _InstallPath { get; set; }
        #endregion

        [XmlAttribute("ModLoaderID")]
        public int ModLoaderID
        {
            get => _ModLoaderID;
            set
            {
                if (_ModLoaderID == value)
                    return;

                _ModLoaderID = value;
                OnPropertyChanged("ModLoaderID");

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
