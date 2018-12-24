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
    public static class XML
    {
        //Récupération de la liste des personnages
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




}
