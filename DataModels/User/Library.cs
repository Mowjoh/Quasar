﻿using DataModels.Resource;
using Newtonsoft.Json;
using System;

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

        [JsonIgnore] public bool ManualMod => GBItem == null;

        [JsonIgnore] public bool Editing { get; set; } = false;
        public bool Included { get; set; }
        public bool Modified { get; set; }

        public bool Scanned { get; set; }

        public string GetFormattedTime()
        {
            TimeSpan t = DateTime.Now - Time;

            if (t.TotalDays > 10)
                return "A long time ago";
            if (t.TotalDays >= 2)
                return String.Format("{0} days ago",t.Days);
            if (t.TotalDays == 1)
                return String.Format("{0} day ago", t.Days);
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
