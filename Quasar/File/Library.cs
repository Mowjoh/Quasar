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
    public static class Library
    {
        #region Basic Functions
        public static string GetLibraryPath()
        {
            return Quasar.Properties.Settings.Default["DefaultDir"].ToString() + "\\Library\\Library.xml";
        }

        #endregion

        #region XML Interactions
        //Parsing the full Mod List
        public static List<Mod> GetModListFile()
        {
            ModList Mods = new ModList();
            List<Mod> parsedMods = new List<Mod>();

            XmlSerializer serializer = new XmlSerializer(typeof(ModList));

            string libraryPath = GetLibraryPath();

            if (System.IO.File.Exists(libraryPath))
            {
                using (FileStream fileStream = new FileStream(libraryPath, FileMode.Open))
                {
                    ModList result = (ModList)serializer.Deserialize(fileStream);
                    parsedMods = result.Cast<Mod>().ToList();
                }
            }

            return parsedMods;
        }

        //Writing the Mod List to the Library file
        public static void WriteModListFile(List<Mod> _mods)
        {
            string libraryPath = GetLibraryPath();

            ModList list = new ModList(_mods);

            XmlSerializer xs = new XmlSerializer(typeof(ModList));
            using (StreamWriter wr = new StreamWriter(libraryPath))
            {
                xs.Serialize(wr, list);
            }
        }
        #endregion

        #region Data Operations
        public static Mod Parse(APIMod mod,List<ModType> modTypes)
        {
            int modType = -1;
            try
            {
                modType = modTypes.Find(mt => mt.APIName == mod.type).ID;
            }
            catch(Exception e)
            {

            }



            return new Mod() { id = mod.id, Name = mod.name, type = modType, association = -1, Authors = mod.authors , category = mod.Categoryname, Updates = mod.Updates};
        }

        public static string GetProperAuthors(string _input)
        {
            string output = "";



            return output;
        }

        #endregion

        #region XML Class Definitions
        //XML Serialization elements
        [Serializable()]
        [XmlRoot("Library")]
        public class ModList : ICollection
        {
            public string CollectionName;
            public ArrayList empArray = new ArrayList();

            public ModList()
            {

            }

            public ModList(List<Mod> _inputList)
            {
                foreach(Mod m in _inputList)
                {
                    empArray.Add(m);
                }
            }

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

            [XmlAttribute("type")]
            public int type { get; set; }

            [XmlAttribute("category")]
            public string category { get; set; }

            [XmlAttribute("association")]
            public int association { get; set; }

            [XmlElement("Name")]
            public string Name { get; set; }

            [XmlElement(Type = typeof(string[][]), ElementName ="Authors")]
            public string[][] Authors { get; set; }

            [XmlElement("Updates")]
            public int Updates { get; set; }

            [XmlElement("Native")]
            public bool Native { get; set; }

            public Mod(int _id, string _Name ,int _type ,int _association ,string[][] _authors, int _updates, bool _native, string _category)
            {
                id = _id;
                Name = _Name;
                type = _type;
                association = _association;
                Authors = _authors;
                Updates = _updates;
                Native = _native;
                category = _category;
            }

            public Mod()
            {

            }
        }
        #endregion

    }

}
