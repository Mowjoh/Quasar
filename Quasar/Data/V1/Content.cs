using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using Quasar.Common.Models;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;

namespace Quasar.Data.V1
{
    //XML Serialization elements
    [Serializable()]
    [XmlRoot("ContentMappingList")]
    public class ContentMappingList : ICollection
    {
        public string CollectionName;
        public ArrayList empArray = new ArrayList();

        public ContentMappingList()
        {

        }

        public ContentMappingList(List<ContentMapping> _inputList)
        {
            foreach (ContentMapping m in _inputList)
            {
                empArray.Add(m);
            }
        }

        public ContentMapping this[int index]
        {
            get { return (ContentMapping)empArray[index]; }
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

        public void Add(ContentMapping newContentMapping)
        {
            empArray.Add(newContentMapping);
        }

    }

    [Serializable]
    public class ContentMapping : ObservableObject
    {
        #region Fields
        private int _ID { get; set; }
        private string _Name { get; set; }
        private string _SlotName { get; set; }
        private int _Slot { get; set; }
        private int _ModID { get; set; }
        private int _InternalModType { get; set; }
        private int _GameDataItemID { get; set; }
        private List<ContentMappingFile> _Files { get; set; }
        private string _Folder { get; set; }
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

        [XmlAttribute("SlotName")]
        public string SlotName
        {
            get => _SlotName;
            set
            {
                if (_SlotName == value)
                    return;

                _SlotName = value;
                OnPropertyChanged("SlotName");
            }
        }

        [XmlAttribute("Slot")]
        public int Slot
        {
            get => _Slot;
            set
            {
                if (_Slot == value)
                    return;

                _Slot = value;
                OnPropertyChanged("Slot");
            }
        }

        [XmlAttribute("ModID")]
        public int ModID
        {
            get => _ModID;
            set
            {
                if (_ModID == value)
                    return;

                _ModID = value;
                OnPropertyChanged("ModID");
            }
        }

        [XmlAttribute("InternalModType")]
        public int InternalModType
        {
            get => _InternalModType;
            set
            {
                if (_InternalModType == value)
                    return;

                _InternalModType = value;
                OnPropertyChanged("InternalModType");
            }
        }

        [XmlAttribute("GameDataItemID")]
        public int GameDataItemID
        {
            get => _GameDataItemID;
            set
            {
                if (_GameDataItemID == value)
                    return;

                _GameDataItemID = value;
                OnPropertyChanged("GameDataItemID");
            }
        }

        [XmlElement("File")]
        public List<ContentMappingFile> Files
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

        public string Folder
        {
            get => _Folder;
            set
            {
                if (_Folder == value)
                    return;

                _Folder = value;
                OnPropertyChanged("Folder");
            }
        }
        #endregion

    }

    public class ContentMappingFile : ObservableObject
    {
        #region Fields
        public int _InternalModTypeFileID { get; set; }
        public string _AnyFile { get; set; }
        public List<string> _FileParts { get; set; }
        public List<string> _FolderParts { get; set; }
        public List<string> _FileFolders { get; set; }
        public List<string> _FolderFolders { get; set; }
        public string _SourcePath { get; set; }
        public string _Path { get; set; }
        #endregion

        #region Properties
        [XmlAttribute("InternalModTypeFileID")]
        public int InternalModTypeFileID
        {
            get => _InternalModTypeFileID;
            set
            {
                if (_InternalModTypeFileID == value)
                    return;

                _InternalModTypeFileID = value;
                OnPropertyChanged("InternalModTypeFileID");
            }
        }

        [XmlElement("AnyFile")]
        public string AnyFile
        {
            get => _AnyFile;
            set
            {
                if (_AnyFile == value)
                    return;

                _AnyFile = value;
                OnPropertyChanged("AnyFile");
            }
        }

        [XmlElement("FilePart")]
        public List<string> FileParts
        {
            get => _FileParts;
            set
            {
                if (_FileParts == value)
                    return;

                _FileParts = value;
                OnPropertyChanged("FileParts");
            }
        }

        [XmlElement("FolderPart")]
        public List<string> FolderParts
        {
            get => _FolderParts;
            set
            {
                if (_FolderParts == value)
                    return;

                _FolderParts = value;
                OnPropertyChanged("FolderParts");
            }
        }

        [XmlElement("FileFolder")]
        public List<string> FileFolders
        {
            get => _FileFolders;
            set
            {
                if (_FileFolders == value)
                    return;

                _FileFolders = value;
                OnPropertyChanged("FileFolders");
            }
        }

        [XmlElement("FolderFolder")]
        public List<string> FolderFolders
        {
            get => _FolderFolders;
            set
            {
                if (_FolderFolders == value)
                    return;

                _FolderFolders = value;
                OnPropertyChanged("FolderFolders");
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

        [XmlElement("Path")]
        public string Path
        {
            get => _Path;
            set
            {
                if (_Path == value)
                    return;

                _Path = value;
                OnPropertyChanged("SourcePath");
            }
        }
        #endregion


    }

    [XmlRoot("Hashes")]
    public class Hashes
    {
        [XmlElement("Hash")]
        public List<Hash> HashList { get; set; }
    }


    public class Hash : ObservableObject
    {
        #region Fields
        private string _Category { get; set; }
        private string _HashString { get; set; }
        private string _FilePath { get; set; }
        #endregion

        [XmlAttribute("Category")]
        public string Category
        {
            get => _Category;
            set
            {
                if (_Category == value)
                    return;

                _Category = value;
                OnPropertyChanged("Category");

            }
        }

        [XmlAttribute("Hash")]
        public string HashString
        {
            get => _HashString;
            set
            {
                if (_HashString == value)
                    return;

                _HashString = value;
                OnPropertyChanged("HashString");

            }
        }

        [XmlAttribute("FilePath")]
        public string FilePath
        {
            get => _FilePath;
            set
            {
                if (_FilePath == value)
                    return;

                _FilePath = value;
                OnPropertyChanged("FilePath");
            }
        }

    }
}
