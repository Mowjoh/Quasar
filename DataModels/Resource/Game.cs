using System.Collections.ObjectModel;

namespace DataModels.Resource
{
    //Base Game Class
    public class Game
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string APIGameName { get; set; }
        public ObservableCollection<GameAPICategory> GameAPICategories { get; set; }
        public ObservableCollection<GameElementFamily> GameElementFamilies { get; set; }

    }

    //API Data
    public class GameAPICategory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string APICategoryName { get; set; }
        public string LibraryFolderName { get; set; }
        public ObservableCollection<GameAPISubCategory> GameAPISubCategories { get; set; }
    }
    public class GameAPISubCategory
    {
        public int ID { get; set; }
        public string APISubCategoryName { get; set; }
        public int APISubCategoryID { get; set; }
    }

    //Game Data
    public class GameElementFamily
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string ViewType { get; set; }
        public string FilterName { get; set; }
        public ObservableCollection<GameElement> GameElements { get; set; }
    }
    public class GameElement
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GameFolderName { get; set; }
        public string FilterValue { get; set; }
        public bool isDLC { get; set; }
    }
}
