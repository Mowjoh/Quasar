using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using Quasar.Resources;

namespace Quasar
{
    public static class ContentXML 
    { 
        
    }


    [XmlRoot("ContentMapping")]
    public class ContentMapping
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("InternalModType")]
        public int InternalModType { get; set; }

        [XmlAttribute("GameDataItemID")]
        public int GameDataItemID { get; set; }

        [XmlElement("File")]
        public List<ContentMappingFile> Files { get; set; }
    }

    public class ContentMappingFile
    {
        [XmlAttribute("InternalModTypeFileID")]
        public int InternalModTypeFileID { get; set; }

        [XmlElement("SourcePath")]
        public string SourcePath { get; set; }

        [XmlElement("Path")]
        public string Path { get; set; }

    }
}
