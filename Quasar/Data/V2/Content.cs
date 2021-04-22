using Quasar.Common.Models;
using Quasar.Data.V1;
using Quasar.Helpers.ModScanning;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Data.V2
{
    public class ContentItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int LibraryItemID { get; set; }
        public int GameElementID { get; set; }
        public int QuasarModTypeID { get; set; }
        public int SlotNumber { get; set; }
        public ObservableCollection<ScanFile> ScanFiles { get; set; }
    }

    public class ModFile
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
        public string Hash { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public int LibraryItemID { get; set; }
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
