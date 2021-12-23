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

    /// <summary>
    /// Contains all scanning methods
    /// </summary>
    public static class FileScanner
    {
        //List of files and extensions to filter with
        public static string[] IgnoreExtensions = { ".csv" };
        public static string[] IgnoreFiles = { "ModInformation.json" };
        
        /// <summary>
        /// Scans a Library Mod
        /// </summary>
        /// <param name="_FolderPath"></param>
        /// <param name="_QuasarModTypes"></param>
        /// <param name="_Game"></param>
        /// <param name="_LibraryItem"></param>
        /// <param name="FileList"></param>
        /// <returns>The mod's corresponding Content Items</returns>
        public static ObservableCollection<ContentItem> ScanMod(string _FolderPath, ObservableCollection<QuasarModType> _QuasarModTypes, Game _Game, LibraryItem _LibraryItem)
        {
            //Getting Mod Folder
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", _FolderPath, _LibraryItem.Guid);

            //Listing Files and processing matches
            ObservableCollection<ScanFile> MatchedFiles = MatchScanFiles(GetScanFiles(_FolderPath),_QuasarModTypes,_Game, ModFolder);

            //Parsing Content Items from processed File Matches
            ObservableCollection<ContentItem> ParsedContentItems = ParseContentItems(MatchedFiles, _LibraryItem);

            return ParsedContentItems;
        }

        /// <summary>
        /// Provides the list of files as a ScanFile Observable collection
        /// </summary>
        /// <param name="_FolderPath">The Mod's directory path</param>
        /// <returns>The list of all files as ScanFiles</returns>
        public static ObservableCollection<ScanFile> GetScanFiles(string _FolderPath)
        {
            //Listing files to scan
            ObservableCollection<ScanFile> FilesToScan = new ObservableCollection<ScanFile>();

            foreach (string s in Directory.GetFiles(_FolderPath, "*", SearchOption.AllDirectories))
            {
                FilesToScan.Add(new ScanFile()
                {
                    SourcePath = s
                });
            }

            return FilesToScan;
        }

        /// <summary>
        /// Filters out Scanfiles with a specific extension or filename
        /// </summary>
        /// <param name="_FilesToFilter">List of ScanFiles to filter</param>
        /// <returns>The Filtered ScanFiles</returns>
        public static ObservableCollection<ScanFile> FilterIgnoredFiles(ObservableCollection<ScanFile> _FilesToFilter)
        {
            foreach(ScanFile sf in _FilesToFilter)
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

            return _FilesToFilter;
        }

        /// <summary>
        /// Launches the matching process for a ScanFile list
        /// </summary>
        /// <param name="_FilesToMatch"></param>
        /// <param name="_QuasarModTypes"></param>
        /// <param name="_Game"></param>
        /// <param name="_ModFolder"></param>
        /// <returns></returns>
        public static ObservableCollection<ScanFile> MatchScanFiles(ObservableCollection<ScanFile> _FilesToMatch, ObservableCollection<QuasarModType> _QuasarModTypes, Game _Game, string _ModFolder)
        {
            ObservableCollection<ScanFile> ScanResults = new ObservableCollection<ScanFile>();
            ObservableCollection<ScanFile> ScannedFiles = new ObservableCollection<ScanFile>();

            //Scanning Files
            foreach (QuasarModType qmt in _QuasarModTypes.OrderBy(i => i.TypePriority))
            {
                foreach (QuasarModTypeFileDefinition FileDefinition in qmt.QuasarModTypeFileDefinitions.OrderBy(o => o.FilePriority).ToList())
                {
                    if (_FilesToMatch.Count > 0)
                    {
                        _FilesToMatch = MatchFilesToQuasarModType(_FilesToMatch, FileDefinition, qmt, _Game, _ModFolder);
                        foreach (ScanFile Scanned in _FilesToMatch.Where(sf => sf.Scanned))
                        {
                            ScannedFiles.Add(Scanned);
                        }
                        foreach (ScanFile Scanned in ScannedFiles)
                        {
                            if (_FilesToMatch.Contains(Scanned))
                                _FilesToMatch.Remove(Scanned);
                        }
                    }
                }
            }
            foreach (ScanFile sf in _FilesToMatch)
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
        /// <param name="_FilesToMatch"></param>
        /// <param name="_FileDefinition"></param>
        /// <param name="_QuasarModType"></param>
        /// <param name="_Game"></param>
        /// <param name="_ModFolder"></param>
        /// <returns></returns>
        public static ObservableCollection<ScanFile> MatchFilesToQuasarModType(ObservableCollection<ScanFile> _FilesToMatch, QuasarModTypeFileDefinition _FileDefinition, QuasarModType _QuasarModType, Game _Game, string _ModFolder)
        {
            Regex FileRegex = new Regex(PrepareRegex(_FileDefinition.SearchFileName));
            Regex FolderRegex = new Regex(PrepareRegex(_FileDefinition.SearchPath));

            foreach (ScanFile FileToMatch in _FilesToMatch)
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
                    GameElementFamily Family = _Game.GameElementFamilies.Single(f => f.ID == _QuasarModType.GameElementFamilyID);

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
                    if ((RecognisedFileGameData != null || RecognisedFolderGameData != null) && (_FileDefinition.SearchPath == "" || folderMatch.Success) && fileMatch.Success)
                    {
                        FileToMatch.QuasarModTypeID = _QuasarModType.ID;
                        FileToMatch.QuasarModTypeFileDefinitionID = _FileDefinition.ID;
                        FileToMatch.GameElementID = RecognisedFolderGameData != null ? RecognisedFolderGameData.ID : RecognisedFileGameData.ID;
                        FileToMatch.Slot = FolderSlot != "" ? FolderSlot : FileSlot != "" ? FileSlot : "00";

                        //Processing paths
                        FileToMatch.SourcePath = FileToMatch.SourcePath.Replace('/', '\\').Replace(_ModFolder, "");
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

            return _FilesToMatch;
        }

        public static ScanFile ProcessMatchData(ScanFile FileToProcess, string FileMatchValue, string FolderMatchValue, int QuasarModTypeID, int FileDefinitionID)
        {
            //FileToProcess.QuasarModTypeID = QuasarModTypeID;
            //FileToProcess.QuasarModTypeFileDefinitionID = FileDefinitionID;
            //FileToProcess.GameElementID = RecognisedFolderGameData != null ? RecognisedFolderGameData.ID : RecognisedFileGameData.ID;
            //FileToProcess.Slot = FolderSlot != "" ? FolderSlot : FileSlot != "" ? FileSlot : "00";

            ////Processing paths
            //FileToProcess.SourcePath = FileToProcess.SourcePath.Replace('/', '\\').Replace(_ModFolder, "");
            //if (FolderMatchValue == "")
            //{
            //    FileToProcess.OriginPath = FileToProcess.SourcePath;
            //}
            //else
            //{
            //    FileToProcess.OriginPath = FileToProcess.SourcePath.Replace("\\" + FolderMatchValue.Replace('/', '\\'), "");
            //}
            //FileToProcess.OriginPath = FileToProcess.OriginPath.Replace(FileMatchValue, "|");
            //FileToProcess.OriginPath = FileToProcess.OriginPath.Split('|')[0];
            //FileToProcess.Scanned = true;
            return FileToProcess;
        }

        /// <summary>
        /// Processes ScanFiles into a collection of ContentItems
        /// </summary>
        /// <param name="_ScanFiles">ScanFiles to process</param>
        /// <param name="_LibraryItem">Tied Library Item</param>
        /// <returns></returns>
        public static ObservableCollection<ContentItem> ParseContentItems(ObservableCollection<ScanFile> _ScanFiles, LibraryItem _LibraryItem)
        {
            ObservableCollection<ContentItem> SearchResults = new ObservableCollection<ContentItem>();

            //Processing Search Results into ContentItems
            foreach (ScanFile sf in _ScanFiles)
            {
                if (!sf.Ignored)
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
                                LibraryItemGuid = _LibraryItem.Guid,
                                Name = _LibraryItem.Name,
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
                                    LibraryItemGuid = _LibraryItem.Guid,
                                    Name = _LibraryItem.Name + " #" + (SearchList.Count + 1).ToString(),
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
                
            }

            return SearchResults;
        }
        
        /// <summary>
        /// Turns a Type config value into a useable Regex
        /// </summary>
        /// <param name="_Input"></param>
        /// <returns>The Regex String</returns>
        public static string PrepareRegex(string _Input)
        {
            string output = _Input;

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

        /// <summary>
        /// Output Scan results to a csv on the user's desktop
        /// </summary>
        /// <param name="_Files">ScanFiles to output</param>
        /// <param name="_GamebananaModID">The Gamebanana Mod's ID</param>
        public static void OutputScanTests(ObservableCollection<ScanFile> _Files, int _GamebananaModID)
        {
            string Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\ScanResults_" + _GamebananaModID + ".csv";

            if (File.Exists(Path))
            {
                File.Delete(Path);
            }
            
            var csv = new StringBuilder();

            var Header = string.Format("{0},{1},{2},{3},{4}", "Source", "Origin", "Slot", "Scanned", "Ignored");
            csv.AppendLine(Header);

            foreach (ScanFile sf in _Files)
            {
                var newLine = string.Format("{0},{1},{2},{3},{4}", sf.SourcePath, sf.OriginPath, sf.Slot, sf.Scanned, sf.Ignored);
                csv.AppendLine(newLine);
            }

            //after your loop
            File.WriteAllText(Path, csv.ToString());
        }
    }
}
