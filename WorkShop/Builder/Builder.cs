using System.Collections.ObjectModel;
using System.IO;
using DataModels.User;

namespace Workshop.Builder
{
    public static class Builder
    {
        public static ObservableCollection<FileReference> GetFileList(ObservableCollection<LibraryItem> Library, string LibraryPath)
        {
            ObservableCollection<FileReference> Files = new();

            foreach (LibraryItem LibraryItem in Library)
            {
                if (LibraryItem.Included)
                {
                    string LibraryContentFolderPath = LibraryPath + "\\Library\\Mods\\" + LibraryItem.Guid + "\\";
                    string OutputContentFolderPath = @"ultimate\mods";
                    foreach (string File in Directory.GetFiles(LibraryContentFolderPath,"*", SearchOption.AllDirectories))
                    {
                        Files.Add(new()
                        {
                            SourceFilePath = File

                        });
                    }
                }
            }

            return Files;
        }

        public static void CompareProcessFileList()
        {

        }
    }

    public class FileReference
    {
        public LibraryItem LibraryItem { get; set; }
        public string SourceFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public bool OutsideFile { get; set; }
    }
}
