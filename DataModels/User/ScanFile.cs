using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.User
{
    public class ScanFile
    {
        public string SourcePath { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string DestinationPath { get; set; }
        public string OriginPath { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Hash { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string Slot { get; set; }
        public int GameElementID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public int QuasarModTypeID { get; set; }
        public int QuasarModTypeFileDefinitionID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool Scanned { get; set; }
        public bool Ignored { get; set; }
    }
}
