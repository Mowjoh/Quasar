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
        //Parsing all Mod Type info
        public static List<ModType> GetModTypes()
        {
            List<ModType> modTypes = null;

            XmlSerializer serializer = new XmlSerializer(typeof(ModTypeList));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\Resources\ModTypes.xml", FileMode.Open))
            {
                ModTypeList result = (ModTypeList)serializer.Deserialize(fileStream);
                modTypes = result.ModTypes;
            }

            foreach(ModType m in modTypes)
            {
                if(m.ResourceFile != "")
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


        //Parsing all character info
        public static List<Category> GetCategories(string _resource)
        {
            List<Category> Categories = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Categories));

            using (FileStream fileStream = new FileStream(Properties.Settings.Default.DefaultDir + @"\Resources\"+_resource, FileMode.Open))
            {
                Categories result = (Categories)serializer.Deserialize(fileStream);
                Categories = result.CategoryList;
            }

            return Categories;
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




}
