using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Quasar.Data.V1
{
    [XmlRoot("Games")]
    public class GameList
    {
        [XmlElement("Game")]
        public List<Game> Games { get; set; }
    }

    public class Game
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("GameName")]
        public string GameName { get; set; }

        [XmlAttribute("Image")]
        public string Image { get; set; }

        public string ImagePath { get; set; }

        [XmlElement("GameModType")]
        public List<GameModType> GameModTypes { get; set; }
    }

    public class GameModType
    {

        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("APIName")]
        public string APIName { get; set; }

        [XmlAttribute("LibraryFolder")]
        public string LibraryFolder { get; set; }

        [XmlElement("Category")]
        public List<Category> Categories { get; set; }

    }
    public class Category
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlAttribute("APICategory")]
        public int APICategory { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        public Category(string _Name, int _APICategeory, int _ID)
        {
            Name = _Name;
            APICategory = _APICategeory;
            ID = _ID;
        }

        Category()
        {

        }

        
    }
    [XmlRoot("GamesData")]
    public class GamesData
    {
        [XmlElement("GameData")]
        public List<GameData> Games { get; set; }
    }

    public class GameData
    {
        [XmlAttribute("GameID")]
        public int GameID { get; set; }

        [XmlElement("Category")]
        public List<GameDataCategory> Categories { get; set; }

    }

    public class GameDataCategory
    {
        [XmlAttribute("ID")]
        public int ID;

        [XmlAttribute("image")]
        public string image;

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Item")]
        public List<GameDataItem> Items { get; set; }
    }

    public class GameDataItem
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("ItemAttribute")]
        public List<GameDataItemAttribute> Attributes { get; set; }

        [XmlElement("ItemNameMappings")]
        public List<GameDataItemMapper> NameMappings { get; set; }

    }

    public class GameDataItemAttribute
    {
        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
    }

    public class GameDataItemMapper
    {
        [XmlAttribute("SlotNumber")]
        public int SlotNumber { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }
    }
}
