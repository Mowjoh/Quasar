using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels.User;
using DataModels.Common;
using DataModels.Resource;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace Workshop.Scanners
{
    public static class FileScanner
    {
        public static string[] IgnoreExtensions = { ".csv" };
        public static string[] IgnoreFiles = { "ModInformation.json" };
        /*
        /// <summary>
        /// Scans a Library Mod
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <param name="QuasarModTypes"></param>
        /// <param name="game"></param>
        /// <param name="LibraryItem"></param>
        /// <param name="FileList"></param>
        /// <returns>The mod's corresponding Content Items</returns>
        public static ObservableCollection<ContentItem> ScanMod(string FolderPath, ObservableCollection<QuasarModType> QuasarModTypes, Game game, LibraryItem LibraryItem, bool FileList = false)
        {
            ObservableCollection<ContentItem> SearchResults = new ObservableCollection<ContentItem>();

            ObservableCollection<ScanFile> ScannedFiles = new ObservableCollection<ScanFile>();
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", FolderPath, LibraryItem.Guid);
            ScannedFiles = GetScanFiles(FolderPath, QuasarModTypes, game, ModFolder);

            //Processing Search Results into ContentMappings
            foreach (ScanFile sf in ScannedFiles)
            {
                if (sf.Scanned == true)
                {
                    List<ContentItem> SearchList = SearchResults.Where(i => i.GameElementID == sf.GameElementID && i.SlotNumber == int.Parse(sf.Slot) && i.QuasarModTypeID == sf.QuasarModTypeID).ToList();
                    if (SearchList.Count == 0)
                    {
                        //If there is no content item related
                        ContentItem ci = new ContentItem()
                        {
                            GameElementID = sf.GameElementID,
                            QuasarModTypeID = sf.QuasarModTypeID,
                            SlotNumber = int.Parse(sf.Slot),
                            LibraryItemGuid = LibraryItem.Guid,
                            Name = LibraryItem.Name,
                            ScanFiles = new ObservableCollection<ScanFile>()
                        {
                            sf
                        }
                        };
                        SearchResults.Add(ci);
                    }
                    else
                    {
                        bool ItemAdded = false;
                        foreach (ContentItem ci in SearchList)
                        {
                            //If there is a related parent
                            if (ci.ScanFiles[0].OriginPath.Replace('/', '\\') == sf.OriginPath.Replace('/', '\\'))
                            {
                                ci.ScanFiles.Add(sf);
                                ItemAdded = true;
                            }
                        }
                        if (!ItemAdded)
                        {
                            //If there is no related parent
                            ContentItem ci = new ContentItem()
                            {
                                GameElementID = sf.GameElementID,
                                QuasarModTypeID = sf.QuasarModTypeID,
                                SlotNumber = int.Parse(sf.Slot),
                                LibraryItemGuid = LibraryItem.Guid,
                                Name = LibraryItem.Name + " #" + (SearchList.Count + 1).ToString(),
                                ScanFiles = new ObservableCollection<ScanFile>()
                                {
                                    sf
                                }
                            };
                            SearchResults.Add(ci);
                        }
                    }
                }
            }

            return SearchResults;
        }
        */

        /// <summary>
        /// Scans all files for a specific path
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <param name="QuasarModTypes"></param>
        /// <param name="game"></param>
        /// <param name="ModFolder"></param>
        /// <param name="FileList"></param>
        /// <returns>The list of all scanned files</returns>
        public static ObservableCollection<ScanFile> GetScanFiles(string FolderPath)
        {
            //Listing files to scan
            ObservableCollection<ScanFile> FilesToScan = new ObservableCollection<ScanFile>();

            foreach (string s in Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories))
            {
                FilesToScan.Add(new ScanFile()
                {
                    SourcePath = s
                });
            }

            return FilesToScan;
        }

        public static ObservableCollection<ScanFile> FilterIgnoredFiles(ObservableCollection<ScanFile> FilesToFilter)
        {
            foreach(ScanFile sf in FilesToFilter)
            {
                foreach (string FileName in IgnoreFiles)
                {
                    if (Path.GetFileName(sf.SourcePath) == FileName)
                    {
                        sf.Scanned = true;
                        sf.Ignored = true;
                    }
                }

                if (!sf.Ignored)
                {
                    foreach (string Extension in IgnoreExtensions)
                    {
                        if (Path.GetExtension(sf.SourcePath) == Extension)
                        {
                            sf.Scanned = true;
                            sf.Ignored = true;
                        }
                    }
                }
                
            }

            return FilesToFilter;
        }

        /// <summary>
        /// Launches the matching process for a ScanFile list
        /// </summary>
        /// <param name="FilesToMatch"></param>
        /// <param name="QuasarModTypes"></param>
        /// <param name="game"></param>
        /// <param name="ModFolder"></param>
        /// <returns></returns>
        public static ObservableCollection<ScanFile> MatchScanFiles(ObservableCollection<ScanFile> FilesToMatch, ObservableCollection<QuasarModType> QuasarModTypes, Game game, string ModFolder)
        {
            ObservableCollection<ScanFile> ScanResults = new ObservableCollection<ScanFile>();
            ObservableCollection<ScanFile> ScannedFiles = new ObservableCollection<ScanFile>();

            //Scanning Files
            foreach (QuasarModType qmt in QuasarModTypes.OrderBy(i => i.TypePriority))
            {
                foreach (QuasarModTypeFileDefinition FileDefinition in qmt.QuasarModTypeFileDefinitions.OrderBy(o => o.FilePriority).ToList())
                {
                    if (FilesToMatch.Count > 0)
                    {
                        FilesToMatch = MatchFilesToQuasarModType(FilesToMatch, FileDefinition, qmt, game, ModFolder);
                        foreach (ScanFile Scanned in FilesToMatch.Where(sf => sf.Scanned))
                        {
                            ScannedFiles.Add(Scanned);
                        }
                        foreach (ScanFile Scanned in ScannedFiles)
                        {
                            if (FilesToMatch.Contains(Scanned))
                                FilesToMatch.Remove(Scanned);
                        }
                    }
                }
            }
            foreach (ScanFile sf in FilesToMatch)
            {
                ScanResults.Add(sf);
            }
            foreach (ScanFile sf in ScannedFiles)
            {
                ScanResults.Add(sf);
            }

            return ScanResults;
        }

        /// <summary>
        /// Matches all scan files to a game entry
        /// </summary>
        /// <param name="FilesToMatch"></param>
        /// <param name="FileDefinition"></param>
        /// <param name="qmt"></param>
        /// <param name="game"></param>
        /// <param name="ModFolder"></param>
        /// <returns></returns>
        public static ObservableCollection<ScanFile> MatchFilesToQuasarModType(ObservableCollection<ScanFile> FilesToMatch, QuasarModTypeFileDefinition FileDefinition, QuasarModType qmt, Game game, string ModFolder)
        {
            Regex FileRegex = new Regex(PrepareRegex(FileDefinition.SearchFileName));
            Regex FolderRegex = new Regex(PrepareRegex(FileDefinition.SearchPath));

            foreach (ScanFile FileToMatch in FilesToMatch)
            {
                Match fileMatch = FileRegex.Match(FileToMatch.SourcePath.Replace('\\', '/'));
                Match folderMatch = FolderRegex.Match(FileToMatch.SourcePath.Replace('\\', '/'));

                if (folderMatch.Success || fileMatch.Success)
                {
                    //Match Data
                    string FolderSlot = "";
                    string FileSlot = "";
                    GameElement RecognisedFolderGameData = null;
                    GameElement RecognisedFileGameData = null;
                    GameElementFamily Family = game.GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID);

                    //Match Data Processing
                    if (folderMatch.Success)
                    {
                        Group GameData = folderMatch.Groups["GameData"];
                        string FolderGameData = GameData.Value;
                        RecognisedFolderGameData = Family.GameElements.SingleOrDefault(ge => (ge.GameFolderName == FolderGameData.ToLower()) || (ge.GameFolderName.Contains(";") ? ge.GameFolderName.Split(';').Contains(FolderGameData.ToLower()) : false));

                        Group Slot = folderMatch.Groups["Slot"];
                        FolderSlot = Slot.Value;
                    }
                    if (fileMatch.Success)
                    {
                        Group GameData = fileMatch.Groups["GameData"];
                        string FileGameData = GameData.Value;
                        RecognisedFileGameData = Family.GameElements.SingleOrDefault(ge => (ge.GameFolderName == FileGameData.ToLower()) || (ge.GameFolderName.Contains(";") ? ge.GameFolderName.Split(';').Contains(FileGameData.ToLower()) : false));

                        Group Slot = fileMatch.Groups["Slot"];
                        FileSlot = Slot.Value;
                    }

                    //Match Validation
                    if ((RecognisedFileGameData != null || RecognisedFolderGameData != null) && (FileDefinition.SearchPath == "" || folderMatch.Success) && fileMatch.Success)
                    {
                        FileToMatch.QuasarModTypeID = qmt.ID;
                        FileToMatch.QuasarModTypeFileDefinitionID = FileDefinition.ID;
                        FileToMatch.GameElementID = RecognisedFolderGameData != null ? RecognisedFolderGameData.ID : RecognisedFileGameData.ID;
                        FileToMatch.Slot = FolderSlot != "" ? FolderSlot : FileSlot != "" ? FileSlot : "00";

                        //Processing paths
                        FileToMatch.SourcePath = FileToMatch.SourcePath.Replace('/', '\\').Replace(ModFolder, "");
                        if (folderMatch.Value == "")
                        {
                            FileToMatch.OriginPath = FileToMatch.SourcePath;
                        }
                        else
                        {
                            FileToMatch.OriginPath = FileToMatch.SourcePath.Replace("\\" + folderMatch.Value.Replace('/', '\\'), "");
                        }
                        FileToMatch.OriginPath = FileToMatch.OriginPath.Replace(fileMatch.Value, "|");
                        FileToMatch.OriginPath = FileToMatch.OriginPath.Split('|')[0];
                        FileToMatch.Scanned = true;
                    }

                }
            }

            return FilesToMatch;
        }
        
        /// <summary>
        /// Turns a Type config value into a useable Regex
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The Regex String</returns>
        public static string PrepareRegex(string input)
        {
            string output = input;

            //Replacing underscores for regex interpretation
            //output = output.Replace(@"_", "\\_");

            //Replacing the tags
            output = output.Replace(@"{Empty}", @"");
            output = output.Replace(@"{Folder}", @"(?'Folder'[^\\\/]*)");
            output = output.Replace(@"{Part}", @"(?'Part'[a-zA-Z0-9]*)");
            output = output.Replace(@"{AnyFile}", @"(?'AnyFile'[a-zA-Z0-9\_]*)");
            output = output.Replace(@"{GameData}", @"(?'GameData'[a-zA-Z0-9\_]*)");

            //Replacing backslashes for regex interpretation
            output = output.Replace(@"\", @"\\");

            //Replacing points for regex interpretation
            output = output.Replace(@".", "\\.");
            output = output.Replace(@"+", "\\+");

            //Replacing Slot digits
            output = output.Replace(@"{S000}", @"(?'Slot'\d{3})");
            output = output.Replace(@"{S00}", @"(?'Slot'\d{2})");
            output = output.Replace(@"{S0}", @"(?'Slot'\d{1})");

            return output;
        }

        public static void OutputScanTests(ObservableCollection<ScanFile> Files, int GamebananaModID)
        {
            string Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ScanResults_" + GamebananaModID + ".csv";

            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
            
            var csv = new StringBuilder();

            var Header = string.Format("{0},{1},{2},{3},{4}", "Source", "Origin", "Slot", "Scanned", "Ignored");
            csv.AppendLine(Header);

            foreach (ScanFile sf in Files)
            {
                var newLine = string.Format("{0},{1},{2},{3},{4}", sf.SourcePath, sf.OriginPath, sf.Slot, sf.Scanned, sf.Ignored);
                csv.AppendLine(newLine);
            }

            //after your loop
            File.WriteAllText(Path, csv.ToString());
        }
    }
}
