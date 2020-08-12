using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using Quasar.XMLResources;

namespace Quasar.XMLResources
{
    

    public static class AssociationXML 
    {
        public static List<Workspace> GetWorkspaces()
        {
            List<Workspace> Workspaces = new List<Workspace>();
            string AssociationLibraryPath = Properties.Settings.Default.DefaultDir + @"\Library\Associations.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(Workspaces));

            if (File.Exists(AssociationLibraryPath))
            {
                using (FileStream fileStream = new FileStream(AssociationLibraryPath, FileMode.Open))
                {

                    Workspaces result = (Workspaces)serializer.Deserialize(fileStream);
                    foreach (Workspace workspace in result)
                    {
                        Workspaces.Add(workspace);
                    }
                }
            }
            return Workspaces;
        }

        public static void WriteAssociationFile(List<Workspace> Workspaces)
        {
            string AssociationLibraryPath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Associations.xml";

            Workspaces al = new Workspaces(Workspaces);

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(Workspaces));
            using (StreamWriter Writer = new StreamWriter(AssociationLibraryPath))
            {
                LibrarySerializer.Serialize(Writer, al);
            }
        }


        //XML Serialization elements
        [Serializable()]
        [XmlRoot("Workspaces")]
        public class Workspaces : ICollection
        {
            public string CollectionName;
            public ArrayList empArray = new ArrayList();

            public Workspaces()
            {

            }

            public Workspaces(List<Workspace> _inputList)
            {
                foreach (Workspace m in _inputList)
                {
                    empArray.Add(m);
                }
            }

            public Workspace this[int index]
            {
                get { return (Workspace)empArray[index]; }
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

            public void Add(Workspace newWorkspace)
            {
                empArray.Add(newWorkspace);
            }

        }

        [Serializable]
        public class Workspace
        {
            [XmlAttribute("ID")]
            public int ID { get; set; }

            [XmlAttribute("Name")]
            public String Name { get; set; }

            [XmlElement("Association")]
            public List<Association> Associations { get; set; }
        }

        [Serializable]
        public class Association
        {
            [XmlAttribute("Slot")]
            public int Slot { get; set; }

            [XmlAttribute("ContentMappingID")]
            public int ContentMappingID { get; set; }

            [XmlAttribute("GameDataItemID")]
            public int GameDataItemID { get; set; }

            [XmlAttribute("InternalModTypeID")]
            public int InternalModTypeID { get; set; }

        }

    }

}
