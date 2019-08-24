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
        //Parsing all character info
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

        //Parsing all Mod Type info
        public static List<ModType> GetModTypes()
        {
            List<ModType> modTypes = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ModTypeList));


            using (FileStream fileStream = new FileStream(@"Resources\ModTypes.xml", FileMode.Open))
            {
                ModTypeList result = (ModTypeList)serializer.Deserialize(fileStream);
                modTypes = result.ModTypes;
            }

            return modTypes;
        }

        //Parsing all Families info (for Music)
        public static List<Family> GetFamilies()
        {
            List<Family> families = null;

            XmlSerializer serializer = new XmlSerializer(typeof(FamilyList));


            using (FileStream fileStream = new FileStream(@"Resources\Families.xml", FileMode.Open))
            {
                FamilyList result = (FamilyList)serializer.Deserialize(fileStream);
                families = result.Families;
            }

            return families;
        }
    }

    #region Characters
    [XmlRoot("Characters")]
    public class CharacterList
    {
        [XmlElement("Character")]
        public List<Character> Characters { get; set; }
        public Character SelectedCharacter { get; set; }
        
    }

    public class Character
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlElement("GBName")]
        public string GBName { get; set; }

        public Character(string sname, string sgbname, int sID)
        {
            Name = sname;
            GBName = sgbname;
            ID = sID;
        }

        Character()
        {

        }
    }
    #endregion

    #region Families
    [XmlRoot("Families")]
    public class FamilyList
    {
        [XmlElement("Family")]
        public List<Family> Families { get; set; }
        public Family SelectedFamily { get; set; }
    }

    public class Family
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Folder")]
        public string Folder { get; set; }

    }
    #endregion

    #region ModTypes
    [XmlRoot("ModTypes")]
    public class ModTypeList
    {
        [XmlElement("ModType")]
        public List<ModType> ModTypes { get; set; }
        public ModType SelectedModType { get; set; }
    }

    public class ModType
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("APIName")]
        public string APIName { get; set; }

        [XmlElement("Folder")]
        public string Folder { get; set; }

    }
    #endregion




}
