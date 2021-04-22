using Quasar.Data.V2;
using System.Collections.ObjectModel;

namespace Quasar.Workspaces.Models
{
    public class ShareableWorkspace
    {
        public Workspace Workspace { get; set; }
        public ObservableCollection<LibraryItem> LibraryItems { get; set; }
        public ObservableCollection<ContentItem> ContentItems { get; set; }

    }
}
