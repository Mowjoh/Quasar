using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Quasar.Resources
{
    class XML
    {
        public static List<Character> GetCharacters()
        {
            List<Character> characters = null;

            XmlSerializer serializer = new XmlSerializer(typeof(CharacterList));


            using (FileStream fileStream = new FileStream(@"Resources\Characters.xml", FileMode.Open))
            {
                CharacterList result = (CharacterList)serializer.Deserialize(fileStream);
                characters = result.Characters;
            }

            return characters;
        }

        public static List<Mod> GetMods()
        {
            List<Mod> Mods = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ModList));


            using (FileStream fileStream = new FileStream(@"Resources\Library.xml", FileMode.Open))
            {
                ModList result = (ModList)serializer.Deserialize(fileStream);
                mods = result.Mods;
            }

            return mods;
        }

        public SetMods(List<Mod> mods)// pas sûr du type du param' pas certain de pouvoir taper dans les getters / setters de Mod en passant par ModList
        {
            // wip logic
        
        }


    }
    
    [XmlRoot("Characters")]
    public class CharacterList
    {
        [XmlElement("Character")]
        public List<Character> Characters { get; set; }
    }

    public class Character
    {
        [XmlAttribute("name")]
        public string name { get; set; }

        [XmlElement("GBName")]
        public string GBName { get; set; }

        public Character(string sname, string sgbname)
        {
            name = sname;
            GBName = sgbname;
        }

        Character()
        {

        }
    }

    [XmlRoot("Library")]
    public class ModList
    {
        [XmlElement("Mod")]
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
            Author=sauthor;
            Version=iversion;
            Native=bnative;
        }

        Mod()
        {

        }
    }


}
