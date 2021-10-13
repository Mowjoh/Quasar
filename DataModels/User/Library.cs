using DataModels.Resource;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.User
{
    public class LibraryItem
    {
        public Guid Guid { get; set; }
        public int GameID { get; set; }
        public string Name { get; set; }
        public GamebananaItem GBItem {get; set;}
        public DateTime Time { get; set; }
        public string HumanTime => GetFormattedTime();

        [JsonIgnore]
        public bool ManualMod => GBItem == null;

        public string GetFormattedTime()
        {
            TimeSpan t = DateTime.Now - Time;

            if (t.TotalDays > 10)
                return "A long time ago";
            if (t.TotalDays > 5)
                return "A couple days ago";
            if (t.TotalDays >= 2)
                return String.Format("{0} days ago",t.Days);
            if (t.TotalDays == 1)
                return String.Format("{0} day ago", t.Days);
            if (t.TotalHours > 5)
                return "A couple hours ago";
            if (t.TotalHours >= 2)
                return String.Format("{0} hour(s) ago", t.Hours);
            if (t.TotalHours == 1)
                return String.Format("{0} hour ago", t.Hours);

            return String.Format("Less than an hour ago");
        }
    }

    public class Author
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public int GamebananaAuthorID { get; set; }
    }

    public class ModInformation
    {
        public LibraryItem LibraryItem { get; set; }
        public GamebananaRootCategory GamebananaRootCategory { get; set; }
    }
    
}
