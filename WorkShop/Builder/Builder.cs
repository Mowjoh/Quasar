using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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
        #region File List Generation
        /// <summary>
        /// Produces a transfer File List
        /// </summary>
        /// <param name="Library"></param>
        /// <param name="LibraryPath"></param>
        /// <returns></returns>
        public static async Task<List<FileReference>> ParseFileList(ObservableCollection<LibraryItem> Library, string LibraryPath, string output_path)
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
                                            Status = FileStatus.Copy
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

        public static async Task<List<FileReference>> ProcessContentFileList(ObservableCollection<LibraryItem> Library, ObservableCollection<ContentItem> contentItems, ObservableCollection<QuasarModType> quasarModTypes, Game game, string LibraryPath, string output_path)
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
                            //If it is meant to be transferred
                            if (ci.SlotNumber != -1 || (ci.QuasarModTypeID == 8 && ci.GameElementID != -1))
                            {
                                foreach (ScanFile ScanFile in ci.ScanFiles)
                                {
                                    if (!ScanFile.Ignored)
                                    {
                                        if (ScanFile.RootPath != "" && ScanFile.FilePath != "")
                                        {
                                            QuasarModType qmt = quasarModTypes.Single(q => q.ID == ScanFile.QuasarModTypeID);
                                            GameElement ge = game.GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID).GameElements.Single(g => g.ID == ci.GameElementID);
                                            string FilePath = FileScanner.ProcessScanFileOutput(ScanFile, qmt, ci.SlotNumber, ge.GameFolderName, ge.isDLC);
                                            //Getting Files for that specific mod
                                            string LibraryContentFolderPath = LibraryPath + "\\Library\\Mods\\" + LibraryItem.Guid + "\\";
                                            LibraryContentFolderPath = LibraryContentFolderPath.Replace(@"\", @"/");

                                            Files.Add(new()
                                            {
                                                LibraryItem = LibraryItem,
                                                OutputFilePath = String.Format(@"{0}\{1}\{2}", output_path, RemoveInvalidChars(LibraryItem.Name), FilePath).Replace(@"\", @"/"),
                                                SourceFilePath = LibraryContentFolderPath + ScanFile.SourcePath,
                                                OutsideFile = false,
                                                FileHash = GetHash(LibraryContentFolderPath + ScanFile.SourcePath),
                                                Status = FileStatus.CopyEdited
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

        public static async Task<List<FileReference>> ProcessIgnoreFileList(ObservableCollection<LibraryItem> Library, ObservableCollection<ContentItem> contentItems, string LibraryPath, string output_path)
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
        #endregion

        #region File List Comparisons
        public static List<FileReference> CompareAssignments(List<FileReference> transfer_index, List<FileReference> assignment_index)
        {
            foreach (FileReference file in assignment_index)
            {
                if (file.Status == FileStatus.CopyEdited)
                {
                    //If there is a file that should be edited, the output path will be edited
                    FileReference MatchedFile = transfer_index.SingleOrDefault(f => f.SourceFilePath.Replace(@"\", @"/") == file.SourceFilePath.Replace(@"\", @"/"));

                    if (MatchedFile == null)
                    {
                        transfer_index.Add(file);
                    }
                }
            }

            return transfer_index;
        }

        public static List<FileReference> CompareIgnored(List<FileReference> transfer_index, List<FileReference> assignment_index)
        {
            foreach (FileReference file in assignment_index)
            {

                if (file.Status == FileStatus.Ignored)
                {
                    //If there is a content item saying this file should be ignored, ignoring it
                    FileReference MatchedFile = transfer_index.SingleOrDefault(f => f.SourceFilePath == file.SourceFilePath);
                    if(MatchedFile != null)
                        MatchedFile.Status = FileStatus.Ignored;
                }

            }
            return transfer_index;
        }

        public static List<FileReference> CompareDistant(List<FileReference> transfer_index, List<FileReference> distant_index)
        {
            foreach (FileReference file in distant_index)
            {
                
                FileReference MatchedFile = transfer_index.SingleOrDefault(f => f.OutputFilePath == file.OutputFilePath);
                if (MatchedFile == null)
                {
                    //If there is no file with the same output path and hash, deleting it
                    file.Status = FileStatus.Delete;
                    transfer_index.Add(file);
                }
                else
                {
                    //If there is a file with the same output path and hash, ignoring it
                    MatchedFile.Status = FileStatus.Ignored;
                }
                
            }
            return transfer_index;
        }


        #endregion

        #region Utilities
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
        #endregion

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

    public enum FileStatus { Copy, CopyEdited, Ignored, Delete}    
}
