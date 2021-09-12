using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels.User;

namespace DataModels.Resource
{

    /// <summary>
    /// Represents all data needed for an API request
    /// </summary>
    public class GamebananaItem
    {
        public int GamebananaItemID { get; set; }
        public Guid RootCategoryGuid { get; set; }
        public Guid SubCategoryGuid { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string GameName { get; set; }
        public int UpdateCount { get; set; }
        public ObservableCollection<Author> Authors { get; set; }
        
    }

    /// <summary>
    /// Represents the item that holds all the API data
    /// </summary>
    public class GamebananaAPI
    {
        public ObservableCollection<GamebananaGame> Games { get; set; }

    }

    /// <summary>
    /// Representative of a Game's Data
    /// </summary>
    public class GamebananaGame
    {
        public Guid Guid { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public ObservableCollection<GamebananaRootCategory> RootCategories { get; set; }
    }

    /// <summary>
    /// Representative of a game's Root Category
    /// </summary>
    public class GamebananaRootCategory
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public ObservableCollection<GamebananaSubCategory> SubCategories { get; set; }

    }

    /// <summary>
    /// Representative of a Root Category's Sub Category
    /// </summary>
    public class GamebananaSubCategory
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
    }
}
