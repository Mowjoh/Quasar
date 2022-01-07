using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DataModels.FileWriters;
using DataModels.Resource;
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
                                            OutputFilePath = String.Format(@"{0}\{1}\{2}", output_path, RemoveInvalidChars(LibraryItem.Name), ScanFile.FilePath),
                                            SourceFilePath = ScanFile.SourcePath,
                                            OutsideFile = false,
                                            FileHash = GetHash(ScanFile.SourcePath),
                                            Status = FileStatus.Normal
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

        public static async Task<List<FileReference>> CreateContentFileList(ObservableCollection<LibraryItem> Library,ObservableCollection<ContentItem> contentItems,ObservableCollection<QuasarModType> quasarModTypes,Game game, string LibraryPath, string output_path)
        {
            ObservableCollection<FileReference> Files = new();
            foreach (LibraryItem LibraryItem in Library)
            {
                if (LibraryItem.Included)
                {
                    try
                    {
                        //Foreach edited ContentItem
                        foreach(ContentItem ci in contentItems.Where(ci => ci.LibraryItemGuid == LibraryItem.Guid && (ci.OriginalGameElementID != ci.GameElementID || ci.SlotNumber != ci.OriginalSlotNumber)))
                        {
                            //If it is meant to be transferred
                            if(ci.SlotNumber != -1 || (ci.QuasarModTypeID == 8 && ci.GameElementID != -1))
                            {
                                foreach (ScanFile ScanFile in ci.ScanFiles)
                                {
                                    if (!ScanFile.Ignored)
                                    {
                                        if (ScanFile.RootPath != "" && ScanFile.FilePath != "")
                                        {
                                            QuasarModType qmt = quasarModTypes.Single(q => q.ID == ScanFile.QuasarModTypeID);
                                            GameElement ge = game.GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID).GameElements.Single(g => g.ID == ci.GameElementID);
                                            string FilePath = FileScanner.ProcessScanFileOutput(ScanFile, qmt, ci.SlotNumber, ge.GameFolderName,ge.isDLC);
                                            //Getting Files for that specific mod
                                            string LibraryContentFolderPath = LibraryPath + "\\Library\\Mods\\" + LibraryItem.Guid + "\\";
                                            LibraryContentFolderPath = LibraryContentFolderPath.Replace(@"\", @"/");

                                            Files.Add(new()
                                            {
                                                LibraryItem = LibraryItem,
                                                OutputFilePath = String.Format(@"{0}\{1}\{2}", output_path, RemoveInvalidChars(LibraryItem.Name), FilePath).Replace(@"\", @"/"),
                                                SourceFilePath = LibraryContentFolderPath + ScanFile.SourcePath,
                                                OutsideFile = false,
                                                FileHash = GetHash(LibraryContentFolderPath+ ScanFile.SourcePath),
                                                Status = FileStatus.Edited
                                            });
                                        }
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

        public static async Task<List<FileReference>> CreateIgnoreFileList(ObservableCollection<LibraryItem> Library, ObservableCollection<ContentItem> contentItems, string LibraryPath, string output_path)
        {
            ObservableCollection<FileReference> Files = new();
            foreach (LibraryItem LibraryItem in Library)
            {
                if (LibraryItem.Included)
                {
                    try
                    {
                        //Foreach edited ContentItem
                        foreach (ContentItem ci in contentItems.Where(ci => ci.LibraryItemGuid == LibraryItem.Guid && (ci.OriginalGameElementID != ci.GameElementID || ci.SlotNumber != ci.OriginalSlotNumber)))
                        {
                            //If it is meant to be ignored
                            if (ci.SlotNumber == -1 || (ci.QuasarModTypeID == 8 && ci.GameElementID == -1))
                            {
                                foreach (ScanFile ScanFile in ci.ScanFiles)
                                {
                                    if (!ScanFile.Ignored)
                                    {
                                        if (ScanFile.RootPath != "" && ScanFile.FilePath != "")
                                        {
                                            string LibraryContentFolderPath = LibraryPath + "\\Library\\Mods\\" + LibraryItem.Guid + "\\";
                                            LibraryContentFolderPath = LibraryContentFolderPath.Replace(@"\", @"/");

                                            Files.Add(new()
                                            {
                                                LibraryItem = LibraryItem,
                                                OutputFilePath = String.Format(@"{0}\{1}\{2}", output_path, RemoveInvalidChars(LibraryItem.Name), ScanFile.FilePath),
                                                SourceFilePath = LibraryContentFolderPath + ScanFile.SourcePath,
                                                OutsideFile = false,
                                                FileHash = GetHash(LibraryContentFolderPath + ScanFile.SourcePath),
                                                Status = FileStatus.Ignored
                                            });
                                        }
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

        public static string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }

        public static string ProcessOutputPath(string Path)
        {


            return "";
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
        public FileStatus Status {get; set;}
    }

    public enum FileStatus { Normal, Edited, Ignored}    
}
