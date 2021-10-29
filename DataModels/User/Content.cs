using DataModels.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.User
{
    public class ContentItem
    {
        [Newtonsoft.Json.JsonProperty]
        public Guid Guid { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public Guid LibraryItemGuid { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public int GameElementID { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public int QuasarModTypeID { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public int SlotNumber { get; set; }
        [Newtonsoft.Json.JsonProperty]
        public ObservableCollection<ScanFile> ScanFiles { get; set; }
    }

    public class SlotContent
    {
        public string SlotName { get; set; }

        public int SlotNumber { get; set; }
        public ObservableCollection<ContentItem> SlotContentItems { get; set; }
    }

    public class ModFile
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
        public string Hash { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Guid LibraryItemGuid { get; set; }
    }

    public class Hash : ObservableObject
    {
        #region Private Members
        [Newtonsoft.Json.JsonIgnore]
        private string _Category { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private string _HashString { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        private string _FilePath { get; set; }
        #endregion

        public string Category
        {
            get => _Category;
            set
            {
                _Category = value;
                OnPropertyChanged("Category");
            }
        }

        public string HashString
        {
            get => _HashString;
            set
            {
                _HashString = value;
                OnPropertyChanged("HashString");
            }
        }

        public string FilePath
        {
            get => _FilePath;
            set
            {
                _FilePath = value;
                OnPropertyChanged("FilePath");
            }
        }

    }
}
