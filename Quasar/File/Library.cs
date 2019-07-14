using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Collections;

namespace Quasar
{
    public static class Library
    {
        public static string GetLibraryPath()
        {
            string libraryPath = Quasar.Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Library.xml";
            return libraryPath;
        }

        //Parsing the full Mod List
        public static ModList GetModListFile()
        {
            ModList Mods = new ModList();

            XmlSerializer serializer = new XmlSerializer(typeof(ModList));

            string libraryPath = GetLibraryPath();

            if (File.Exists(libraryPath))
            {
                using (FileStream fileStream = new FileStream(libraryPath, FileMode.Open))
                {
                    ModList result = (ModList)serializer.Deserialize(fileStream);
                    Mods = result;
                }
            }

            return Mods;
        }

        //Writing the Mod List to the Library file
        public static void WriteModListFile(ModList mods)
        {
            string libraryPath = GetLibraryPath();

            XmlSerializer xs = new XmlSerializer(typeof(ModList));
            using (StreamWriter wr = new StreamWriter(libraryPath))
            {
                xs.Serialize(wr, mods);
            }
        }
        
        public static Mod Parse(APIMod mod)
        {
            return new Mod() { id = mod.id, Name = mod.name, Author = mod.authors };
        }

        


        //XML Serialization elements
        [Serializable()]
        [XmlRoot("Library")]
        public class ModList:ICollection
        {
            public string CollectionName;
            private ArrayList empArray = new ArrayList();

            public Mod this[int index]
            {
                get { return (Mod)empArray[index]; }
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

            public void Add(Mod newMod)
            {
                empArray.Add(newMod);
            }
        }

        [Serializable()]
        public class Mod
        {
            [XmlAttribute("id")]
            public int id { get; set; }

            [XmlElement("Name")]
            public string Name { get; set; }

            [XmlElement("Author")]
            public string Author { get; set; }

            [XmlElement("Version")]
            public int Version { get; set; }

            [XmlElement("Native")]
            public bool Native { get; set; }

            public Mod(int iid, string sname, string sauthor, int iversion, bool bnative)
            {
                id = iid;
                Name = sname;
                Author = sauthor;
                Version = iversion;
                Native = bnative;
            }

            public Mod()
            {

            }
        }




    }

}
