using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace Quasar
{
    public static class Library
    {
        //Récupération de la liste de mods stockée
        public static List<Mod> GetMods()
        {
            List<Mod> Mods = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ModList));


            using (FileStream fileStream = new FileStream(@"Resources\Library.xml", FileMode.Open))
            {
                ModList result = (ModList)serializer.Deserialize(fileStream);
                Mods = result.Mods;
            }

            return Mods;
        }

        //Ecriture du fichier de librairie
        public static void SetMods(List<Mod> mods)
        {

            XmlSerializer xs = new XmlSerializer(typeof(ModList));
            using (StreamWriter wr = new StreamWriter(@"Resources\Library.xml"))
            {
                xs.Serialize(wr, mods);
            }
        }
        
        public static Mod getModfromAPIMod(APIMod mod)
        {
            return new Mod() { id = mod.id, Name = mod.name, Author = mod.authors };
        }

        [XmlRoot("Library")]
        [XmlInclude(typeof(Mod))]
        public class ModList
        {
            [XmlArray("Library")]
            [XmlArrayItem("Mod")]
            public List<Mod> Mods { get; set; }
        }

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
