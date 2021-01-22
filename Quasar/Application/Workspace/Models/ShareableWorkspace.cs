using Quasar.Data.V2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar
{
    public class ShareableWorkspace
    {
        public Data.V2.Workspace Workspace { get; set; }
        public ObservableCollection<LibraryItem> LibraryItems { get; set; }
        public ObservableCollection<ContentItem> ContentItems { get; set; }

    }
}
