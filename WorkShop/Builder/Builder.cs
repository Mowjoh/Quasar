using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DataModels.FileWriters;
using DataModels.User;
using log4net.Util;
using Workshop.Scanners;

namespace Workshop.Builder
{
    public static class Builder
    {
        /// <summary>
        /// Produces a transfer File List
        /// </summary>
        /// <param name="Library"></param>
        /// <param name="LibraryPath"></param>
        /// <returns></returns>
        public static async Task<List<FileReference>> CreateFileList(ObservableCollection<LibraryItem> Library, string LibraryPath, string output_path)
        {
            ObservableCollection<FileReference> Files = new();

            foreach (LibraryItem LibraryItem in Library)
            {
                if (LibraryItem.Included)
                {
                    try
                    {
                        //Getting Files for that specific mod
                        string LibraryContentFolderPath = LibraryPath + "\\Library\\Mods\\" + LibraryItem.Guid + "\\";

                        if (Directory.Exists(LibraryContentFolderPath))
                        {
                            ObservableCollection<ScanFile> ScanFiles = FileScanner.GetScanFiles(LibraryContentFolderPath);
                            ScanFiles = FileScanner.FilterIgnoredFiles(ScanFiles);

                            foreach (ScanFile ScanFile in ScanFiles)
                            {
                                if (!ScanFile.Ignored)
                                {
                                    if (ScanFile.RootPath != "" && ScanFile.FilePath != "")
                                    {
                                        Files.Add(new()
                                        {
                                            LibraryItem = LibraryItem,
                                            OutputFilePath = String.Format(@"{0}\{1}\{2}", output_path, LibraryItem.Name, ScanFile.FilePath),
                                            SourceFilePath = ScanFile.SourcePath,
                                            OutsideFile = false,
                                            FileHash = GetHash(ScanFile.SourcePath)
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    
                    
                }
            }

            return Files.ToList();
        }

        public static async Task<ObservableCollection<FileReference>> CompareFileList(ObservableCollection<FileReference> files_to_transfer, ObservableCollection<FileReference> distant_files)
        {

            return files_to_transfer;
        }

        public static void CompareProcessFileList()
        {

        }

        public static string GetHash(string SourceFilePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(SourceFilePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                }
            }
        }
    }

    public class FileReference
    {
        [JsonIgnore]
        public LibraryItem LibraryItem { get; set; }
        [JsonIgnore]
        public string SourceFilePath { get; set; }
        public string OutputFilePath { get; set; }

        public bool OutsideFile { get; set; }
        public string FileHash { get; set; }
    }

    
}
