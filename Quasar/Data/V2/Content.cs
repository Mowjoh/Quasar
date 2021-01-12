using Quasar.Data.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Data.V2
{
    public class ContentItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int LibraryItemID { get; set; }
        public int QuasarModTypeID { get; set; }
        public int SlotNumber { get; set; }
    }

    public class ModFile
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
    }
}
