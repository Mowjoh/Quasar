using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Resource
{
    public class ModLoader
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string WorkspacePath { get; set; }
        public int GameID { get; set; }
    }
}
