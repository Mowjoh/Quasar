using Quasar.Controls.Build.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Quasar.XMLResources
{
    public static class XML
    {
        //Loads
        public static List<Game> GetGames()
        {
            List<Game> Games = null;

            XmlSerializer serializer = new XmlSerializer(typeof(GameList));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\References\Sources\Games.xml", FileMode.Open))
            {
                GameList result = (GameList)serializer.Deserialize(fileStream);
                Games = result.Games;
            }
            foreach(Game game in Games)
            {
                game.ImagePath = Properties.Settings.Default.DefaultDir + @"\References\images\" + game.Image;
            }
            return Games;
        }

        public static List<InternalModType> GetInternalModTypes()
        {
            List<InternalModType> InteralModTypes = new List<InternalModType>();
            string typesFolderPath = Properties.Settings.Default.DefaultDir + @"\References\InternalModTypes\";
            XmlSerializer serializer = new XmlSerializer(typeof(InternalModType));

            foreach (string file in Directory.GetFiles(typesFolderPath,"*",SearchOption.AllDirectories))
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open))
                {

                    InternalModType result = (InternalModType)serializer.Deserialize(fileStream);
                    InteralModTypes.Add(result);
                }
            }

            return InteralModTypes;
        }
        
        public static List<GameData> GetGameData()
        {
            List<GameData> categories = new List<GameData>();

            XmlSerializer serializer = new XmlSerializer(typeof(GamesData));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\References\Sources\GameData.xml", FileMode.Open))
            {
                GamesData result = (GamesData)serializer.Deserialize(fileStream);
                categories = result.Games;
            }

            return categories;
        }

        public static List<ModLoader> GetModLoaders()
        {
            List<ModLoader> ModLoaders = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ModLoaders));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\References\Sources\ModLoaders.xml", FileMode.Open))
            {
                ModLoaders result = (ModLoaders)serializer.Deserialize(fileStream);
                ModLoaders = result.ModLoaderList;
            }

            return ModLoaders;
        }

        //Saves
        public static void SaveInternalModType(InternalModType type)
        {
            string destination = Properties.Settings.Default.DefaultDir + @"\References\InternalModTypes\" + type.Filename + ".xml";

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(InternalModType));

            using (StreamWriter wr = new StreamWriter(destination))
            {
                xmlSerializer.Serialize(wr, type);
            }
        }

        public static List<Hash> GetHashes(string FilePath)
        {
            List<Hash> Hashes = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Hashes));

            using (FileStream fileStream = new FileStream(FilePath, FileMode.Open))
            {
                Hashes result = (Hashes)serializer.Deserialize(fileStream);
                Hashes = result.HashList;
            }

            return Hashes;
        }

        public static void SaveHashes(string FilePath, List<Hash> Hashes)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Hashes));

            using (StreamWriter wr = new StreamWriter(FilePath))
            {
                xmlSerializer.Serialize(wr, new Hashes() { HashList = Hashes });
            }
        }

    }

    #region APIGames
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
    #endregion

    #region GameData
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

    }

    public class GameDataItemAttribute
    {
        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }
    }
    #endregion

    #region Builders
    

    #endregion
}
