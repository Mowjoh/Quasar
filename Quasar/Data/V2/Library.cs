using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Data.V2
{
    public class LibraryItem
    {
        public int ID { get; set; }
        public int GameID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UpdateCount { get; set; }
        public int GamebananaItemID { get; set; }
        public int GameAPISubCategoryID { get; set; }
        public ObservableCollection<Author> Authors { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public int GamebananaAuthorID { get; set; }
    }
}
