using Quasar.Data.V2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Helpers.ModScanning
{
    static class Scannerino
    {
        public static void ScanAllMods(ObservableCollection<LibraryItem> Library)
        {

        }

        public static ObservableCollection<ContentItem> ScanMod(LibraryItem _LibraryItem)
        {
            ObservableCollection<ContentItem> SearchResults = new ObservableCollection<ContentItem>();

            return SearchResults;
        }

        public static ObservableCollection<ModFile> GetModFiles()
        {
            ObservableCollection<ModFile> PreparedModFiles = new ObservableCollection<ModFile>();

            return PreparedModFiles;
        }
    }
}
