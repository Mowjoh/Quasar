using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using Quasar.XMLResources;
using static Quasar.XMLResources.Library;

namespace Quasar.XMLResources
{
    public static class ContentXML 
    {
        public static List<ContentMapping> GetContentMappings()
        {
            List<ContentMapping> ContentMappings = new List<ContentMapping>();
            string ContentMappingLibraryPath = Properties.Settings.Default.DefaultDir + @"\Library\ContentMapping.xml";
            XmlSerializer serializer = new XmlSerializer(typeof(ContentMappingList));

            if (File.Exists(ContentMappingLibraryPath))
            {
                using (FileStream fileStream = new FileStream(ContentMappingLibraryPath, FileMode.Open))
                {

                    ContentMappingList result = (ContentMappingList)serializer.Deserialize(fileStream);
                    foreach (ContentMapping cm in result)
                    {
                        ContentMappings.Add(cm);
                    }

                }
            }
            

            return ContentMappings;
        }

        //Writing the Mod List to the Library file
        public static void WriteContentMappingListFile(List<ContentMapping> contentMappings)
        {
            string ContentMappingFilePath = Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\ContentMapping.xml";

            contentMappings = contentMappings.OrderBy(a => a.GameDataItemID).ThenBy(a => a.InternalModType).ThenBy(a => a.Slot).ToList();

            ContentMappingList cml = new ContentMappingList(contentMappings);

            XmlSerializer LibrarySerializer = new XmlSerializer(typeof(ContentMappingList));
            using (StreamWriter Writer = new StreamWriter(ContentMappingFilePath))
            {
                LibrarySerializer.Serialize(Writer, cml);
            }
        }

        public static List<ContentMapping> RemoveUpdatedModMappings(List<ContentMapping> contentMappings, LibraryMod libraryMod)
        {
            List<ContentMapping> CorrespondingMappings = contentMappings.FindAll(cm => cm.ModID == libraryMod.ID);
            foreach(ContentMapping foundContent in CorrespondingMappings)
            {
                contentMappings.Remove(foundContent);
            }

            return contentMappings;
        }

        public static List<ContentMapping> RemoveUpdatedInternalModTypeMappings(List<ContentMapping> contentMappings, InternalModType imt)
        {
            List<ContentMapping> CorrespondingMappings = contentMappings.FindAll(cm => cm.InternalModType == imt.ID);
            foreach (ContentMapping foundContent in CorrespondingMappings)
            {
                contentMappings.Remove(foundContent);
            }

            return contentMappings;
        }
    }

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

    
}
