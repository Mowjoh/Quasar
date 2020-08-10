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
        public static List<Association> GetAssociations()
        {
            List<Association> Associations = new List<Association>();
            string AssociationLibraryPath = Properties.Settings.Default.DefaultDir + @"\Library\Associations.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(AssociationList));

            if (File.Exists(AssociationLibraryPath))
            {
                using (FileStream fileStream = new FileStream(AssociationLibraryPath, FileMode.Open))
                {

                    AssociationList result = (AssociationList)serializer.Deserialize(fileStream);
                    foreach (Association asso in result)
                    {
                        Associations.Add(asso);
                    }

                }
            }


            return Associations;
        }

        public static void WriteAssociationFile(List<Association> Associations)
        {
            string AssociationLibraryPath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Associations.xml";

            AssociationList al = new AssociationList(Associations);

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(AssociationList));
            using (StreamWriter Writer = new StreamWriter(AssociationLibraryPath))
            {
                LibrarySerializer.Serialize(Writer, al);
            }
        }


        //XML Serialization elements
        [Serializable()]
        [XmlRoot("AssociationList")]
        public class AssociationList : ICollection
        {
            public string CollectionName;
            public ArrayList empArray = new ArrayList();

            public AssociationList()
            {

            }

            public AssociationList(List<Association> _inputList)
            {
                foreach (Association m in _inputList)
                {
                    empArray.Add(m);
                }
            }

            public Association this[int index]
            {
                get { return (Association)empArray[index]; }
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

            public void Add(Association newAssociation)
            {
                empArray.Add(newAssociation);
            }

        }

        [Serializable]
        public class Association
        {
            [XmlAttribute("Slot")]
            public int Slot { get; set; }

            [XmlAttribute("ContentMappingID")]
            public int ContentMappingID { get; set; }

        }

    }

}
