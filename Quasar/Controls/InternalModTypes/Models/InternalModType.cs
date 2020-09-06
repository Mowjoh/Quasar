using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Quasar
{
    [XmlRoot("InternalModType")]
    public class InternalModType
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Filename")]
        public string Filename { get; set; }

        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlAttribute("GameID")]
        public int GameID { get; set; }

        [XmlAttribute("Association")]
        public int Association { get; set; }

        [XmlAttribute("Slots")]
        public int Slots { get; set; }

        [XmlElement("InternalModTypeFile")]
        public List<InternalModTypeFile> Files { get; set; }
    }

    public class InternalModTypeFile
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlElement("SourcePath")]
        public string SourcePath { get; set; }

        [XmlElement("SourceFile")]
        public string SourceFile { get; set; }

        [XmlElement("BuilderFolder")]
        public List<BuilderFolder> Destinations { get; set; }

        [XmlElement("BuilderFile")]
        public List<BuilderFile> Files { get; set; }

        [XmlElement("Mandatory")]
        public bool Mandatory { get; set; }

        public InternalModTypeFile()
        {

        }
    }

    public class BuilderFile
    {
        [XmlAttribute("BuilderID")]
        public int BuilderID { get; set; }

        [XmlAttribute("Path")]
        public string Path { get; set; }
    }

    public class BuilderFolder
    {
        [XmlAttribute("BuilderID")]
        public int BuilderID { get; set; }

        [XmlAttribute("Path")]
        public string Path { get; set; }
    }
}
