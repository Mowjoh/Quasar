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
        //Loads
        public static List<ModType> GetModTypes()
        {
            List<ModType> modTypes = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ModTypeList));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\Resources\ModTypes.xml", FileMode.Open))
            {
                ModTypeList result = (ModTypeList)serializer.Deserialize(fileStream);
                modTypes = result.ModTypes;
            }

            foreach (ModType m in modTypes)
            {
                if (m.ResourceFile != "")
                {
                    m.Categories = GetCategories(m.ResourceFile);
                }
                else
                {
                    m.Categories = new List<Category>();
                }

            }

            return modTypes;
        }

        public static List<InternalModType> GetInternalModTypes()
        {
            List<InternalModType> InteralModTypes = new List<InternalModType>();
            string typesFolderPath = Properties.Settings.Default.DefaultDir + @"\InternalModTypes\";
            XmlSerializer serializer = new XmlSerializer(typeof(InternalModType));

            foreach (string file in Directory.GetFiles(typesFolderPath))
            {
                using (FileStream fileStream = new FileStream(file, FileMode.Open))
                {

                    InternalModType result = (InternalModType)serializer.Deserialize(fileStream);
                    InteralModTypes.Add(result);
                }
            }

            return InteralModTypes;
        }
        
        public static List<Category> GetCategories(string _resource)
        {
            List<Category> Categories = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Categories));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\Resources\" + _resource, FileMode.Open))
            {
                Categories result = (Categories)serializer.Deserialize(fileStream);
                Categories = result.CategoryList;
            }

            return Categories;
        }

        public static List<GameDataCategory> getGameCategories()
        {
            List<GameDataCategory> categories = new List<GameDataCategory>();

            XmlSerializer serializer = new XmlSerializer(typeof(GameDataInformation));
            /*
            GameDataItemAttribute attr = new GameDataItemAttribute() { Key = "test", Value = "tada" };
            GameDataItem item = new GameDataItem() { ID = 1, Name = "name", Attributes = new List<GameDataItemAttribute>() { attr } };
            GameDataCategory cat = new GameDataCategory() { ID = 1, Items = new List<GameDataItem>() { item } };
            GameDataInformation info = new GameDataInformation() { Categories = new List<GameDataCategory>() { cat } };

            using (StreamWriter wr = new StreamWriter(Properties.Settings.Default.DefaultDir+ @"\Resources\output.xml"))
            {
                serializer.Serialize(wr, info);
            }
            */


            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\Resources\GameData.xml", FileMode.Open))
            {
                GameDataInformation result = (GameDataInformation)serializer.Deserialize(fileStream);
                categories = result.Categories;
            }

            return categories;
        }

        //Saves
        public static void SaveInternalModType(InternalModType type)
        {
            string destination = Properties.Settings.Default.DefaultDir + @"\InternalModTypes\" + type.Filename + ".xml";

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(InternalModType));

            using (StreamWriter wr = new StreamWriter(destination))
            {
                xmlSerializer.Serialize(wr, type);
            }
        }

    }

    [XmlRoot("ModTypes")]
    public class ModTypeList
    {
        [XmlElement("ModType")]
        public List<ModType> ModTypes { get; set; }
        public ModType SelectedModType { get; set; }
    }

    public class ModType
    {
        public List<Category> Categories { get; set; }

        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("APIName")]
        public string APIName { get; set; }

        [XmlElement("ResourceFile")]
        public string ResourceFile { get; set; }

        [XmlElement("Folder")]
        public string Folder { get; set; }

        ModType()
        {

        }

    }

    [XmlRoot("Categories")]
    public class Categories
    {
        [XmlElement("Category")]
        public List<Category> CategoryList { get; set; }
        public Category SelectedCategory { get; set; }
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

    [XmlRoot("InternalModType")]
    public class InternalModType
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Filename")]
        public string Filename { get; set; }

        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlAttribute("Association")]
        public int Association { get; set; }

        [XmlAttribute("Slots")]
        public int Slots { get; set; }

        [XmlElement("InternalModTypeFile")]
        public List<InternalModTypeFile> Files { get; set; }
    }

    public class InternalModTypeFile
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }

        [XmlElement("Path")]
        public string Path { get; set; }

        [XmlElement("File")]
        public string File { get; set; }

        [XmlElement("Mandatory")]
        public bool Mandatory { get; set; }

        public InternalModTypeFile()
        {

        }
    }


    [XmlRoot("GameDataInformation")]
    public class GameDataInformation
    {

        [XmlElement("Category")]
        public List<GameDataCategory> Categories { get; set; }

    }

    public class GameDataCategory
    {
        [XmlAttribute("ID")]
        public int ID;

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

}
