using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DataModels.Common;

namespace Quasar.Data.V1
{
    [XmlRoot("InternalModType")]
    public class InternalModType : ObservableObject
    {

        #region Fields
        private string _Name { get; set; }
        private string _Filename { get; set; }
        private string _TypeGroup { get; set; }
        private int _ID { get; set; }
        private int _GameID { get; set; }
        private int _Association { get; set; }
        private int _Slots { get; set; }
        private bool _IgnoreableGameDataAssociation { get; set; }
        private bool _NoGameData { get; set; }
        private bool _OutsideFolder { get; set; }
        private string _OutsideFolderPath { get; set; }
        private List<InternalModTypeFile> _Files { get; set; }
        #endregion

        #region Properties
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

        [XmlAttribute("Filename")]
        public string Filename
        {
            get => _Filename;
            set
            {
                if (_Filename == value)
                    return;

                _Filename = value;
                OnPropertyChanged("Filename");
            }
        }

        [XmlAttribute("TypeGroup")]
        public string TypeGroup
        {
            get => _TypeGroup;
            set
            {
                if (_TypeGroup == value)
                    return;

                _TypeGroup = value;
                OnPropertyChanged("TypeGroup");
            }
        }

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
                if (_GameID == value)
                    return;

                _GameID = value;
                OnPropertyChanged("GameID");
            }
        }

        [XmlAttribute("Association")]
        public int Association
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

        [XmlAttribute("Slots")]
        public int Slots
        {
            get => _Slots;
            set
            {
                if (_Slots == value)
                    return;

                _Slots = value;
                OnPropertyChanged("Slots");
            }
        }
        [XmlAttribute("IgnoreableGameDataAssociation")]
        public bool IgnoreableGameDataAssociation
        {
            get => _IgnoreableGameDataAssociation;
            set
            {
                if (_IgnoreableGameDataAssociation == value)
                    return;

                _IgnoreableGameDataAssociation = value;
                OnPropertyChanged("IgnoreableGameDataAssociation");
            }
        }
        [XmlElement("NoGameData")]
        public bool NoGameData
        {
            get => _NoGameData;
            set
            {
                if (_NoGameData == value)
                    return;

                _NoGameData = value;
                OnPropertyChanged("NoGameData");
            }
        }
        [XmlElement("OutsideFolder")]
        public bool OutsideFolder
        {
            get => _OutsideFolder;
            set
            {
                if (_OutsideFolder == value)
                    return;

                _OutsideFolder = value;
                OnPropertyChanged("OutsideFolder");
            }
        }
        [XmlElement("OutsideFolderPath")]
        public string OutsideFolderPath
        {
            get => _OutsideFolderPath;
            set
            {
                if (_OutsideFolderPath == value)
                    return;

                _OutsideFolderPath = value;
                OnPropertyChanged("OutsideFolderPath");
            }
        }
        [XmlElement("InternalModTypeFile")]
        public List<InternalModTypeFile> Files
        {
            get => _Files;
            set
            {
                if (_Files == value)
                    return;

                _Files = value;
                OnPropertyChanged("Files");
            }
        }
        #endregion

    }

    public class InternalModTypeFile : ObservableObject
    {
        #region Fields
        private int _ID { get; set; }
        private string _SourcePath { get; set; }
        private string _SourceFile { get; set; }
        private List<BuilderFile> _Files { get; set; }
        #endregion

        #region Properties

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

        [XmlElement("SourcePath")]
        public string SourcePath
        {
            get => _SourcePath;
            set
            {
                if (_SourcePath == value)
                    return;

                _SourcePath = value;
                OnPropertyChanged("SourcePath");
            }
        }

        [XmlElement("SourceFile")]
        public string SourceFile
        {
            get => _SourceFile;
            set
            {
                if (_SourceFile == value)
                    return;

                _SourceFile = value;
                OnPropertyChanged("SourceFile");
            }
        }

        [XmlElement("BuilderFile")]
        public List<BuilderFile> Files
        {
            get => _Files;
            set
            {
                if (_Files == value)
                    return;

                _Files = value;
                OnPropertyChanged("Files");
            }
        }

        #endregion

    }

    public class BuilderFile : ObservableObject
    {

        #region Fields
        private int _BuilderID { get; set; }
        private string _Path { get; set; }
        private string _File { get; set; }
        #endregion

        #region Properties

        [XmlAttribute("BuilderID")]
        public int BuilderID
        {
            get => _BuilderID;
            set
            {
                if (_BuilderID == value)
                    return;

                _BuilderID = value;
                OnPropertyChanged("BuilderID");
            }
        }

        [XmlAttribute("Path")]
        public string Path
        {
            get => _Path;
            set
            {
                if (_Path == value)
                    return;

                _Path = value;
                OnPropertyChanged("Path");
            }
        }

        [XmlAttribute("File")]
        public string File
        {
            get => _File;
            set
            {
                if (_File == value)
                    return;

                _File = value;
                OnPropertyChanged("File");
            }
        }

        #endregion

    }
}
