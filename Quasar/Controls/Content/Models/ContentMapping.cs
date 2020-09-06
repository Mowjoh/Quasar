using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Quasar
{
    [Serializable]
    public class ContentMapping
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("SlotName")]
        public string SlotName { get; set; }

        [XmlAttribute("Slot")]
        public int Slot { get; set; }

        [XmlAttribute("ModID")]
        public int ModID { get; set; }

        [XmlAttribute("InternalModType")]
        public int InternalModType { get; set; }

        [XmlAttribute("GameDataItemID")]
        public int GameDataItemID { get; set; }

        [XmlElement("File")]
        public List<ContentMappingFile> Files { get; set; }

        public string Folder { get; set; }
    }

    public class ContentMappingFile
    {
        [XmlAttribute("InternalModTypeFileID")]
        public int InternalModTypeFileID { get; set; }

        [XmlElement("AnyFile")]
        public string AnyFile { get; set; }

        [XmlElement("FilePart")]
        public List<string> FileParts { get; set; }

        [XmlElement("FolderPart")]
        public List<string> FolderParts { get; set; }

        [XmlElement("FileFolder")]
        public List<string> FileFolders { get; set; }

        [XmlElement("FolderFolder")]
        public List<string> FolderFolders { get; set; }

        [XmlElement("SourcePath")]
        public string SourcePath { get; set; }

        [XmlElement("Path")]
        public string Path { get; set; }

    }
}
