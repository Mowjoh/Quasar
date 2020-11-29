using Quasar.Controls.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Quasar.Controls.Build.Models
{
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
