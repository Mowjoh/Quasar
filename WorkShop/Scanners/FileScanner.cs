using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataModels.User;
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
        public static string[] IgnoreExtensions = { ".csv", ".jpg" };
        public static string[] IgnoreFiles = { "ModInformation.json" };
        public static string RootFolders = "append|assist|boss|camera|campaign|common|effect|enemy|fighter|finalsmash|item|item|miihat|param|pokemon|prebuilt;|render|snapshot|stream;|sound|spirits|stage|standard|ui";
        
        /// <summary>
        /// Scans a Library Mod
        /// </summary>
        /// <param name="_folder_path"></param>
        /// <param name="_quasar_mod_types"></param>
        /// <param name="_game"></param>
        /// <param name="_library_item"></param>
        /// <param name="FileList"></param>
        /// <returns>The mod's corresponding Content Items</returns>
        public static ObservableCollection<ContentItem> ScanMod(string _folder_path, ObservableCollection<QuasarModType> _quasar_mod_types, Game _game, LibraryItem _library_item)
        {
            //Getting Mod Folder
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", _folder_path, _library_item.Guid);

            //Listing Files and processing matches
            ObservableCollection<ScanFile> MatchedFiles = MatchScanFiles(GetScanFiles(_folder_path),_quasar_mod_types,_game, ModFolder);

            //Parsing Content Items from processed File Matches
            ObservableCollection<ContentItem> ParsedContentItems = ParseContentItems(MatchedFiles, _library_item);

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
                string[] paths = GetRootFolder(s);
                if (paths[0] != "")
                {
                    FilesToScan.Add(new ScanFile()
                    {
                        SourcePath = s,
                        RootPath = paths[0],
                        FilePath = paths[1]
                        
                    });
                }
                else
                {
                    FilesToScan.Add(new ScanFile()
                    {
                        SourcePath = s,
                        FilePath = Path.GetFileName(s)
                    });
                }
                
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
                            FileToMatch.OriginPath = FileToMatch.SourcePath.Replace("\\" + folderMatch.Value , "");
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
                                Guid = Guid.NewGuid(),
                                GameElementID = sf.GameElementID,
                                OriginalGameElementID = sf.GameElementID,
                                QuasarModTypeID = sf.QuasarModTypeID,
                                SlotNumber = int.Parse(sf.Slot),
                                OriginalSlotNumber = int.Parse(sf.Slot),
                                LibraryItemGuid = _LibraryItem.Guid,
                                Name = _LibraryItem.Name,
                                ScanFiles = new ObservableCollection<ScanFile>() { sf },
                                ParentName = sf.OriginPath.Replace('/', '\\')
                            };
                            SearchResults.Add(ci);
                        }
                        else
                        {
                            bool ItemAdded = false;
                            foreach (ContentItem ci in SearchList)
                            {
                                //If there is a related parent
                                if (ci.ScanFiles[0].RootPath == sf.RootPath)
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
                                    Guid = Guid.NewGuid(),
                                    GameElementID = sf.GameElementID,
                                    OriginalGameElementID = sf.GameElementID,
                                    QuasarModTypeID = sf.QuasarModTypeID,
                                    SlotNumber = int.Parse(sf.Slot),
                                    OriginalSlotNumber = int.Parse(sf.Slot),
                                    LibraryItemGuid = _LibraryItem.Guid,
                                    Name = _LibraryItem.Name + " #" + (SearchList.Count + 1).ToString(),
                                    ScanFiles = new ObservableCollection<ScanFile>() { sf }
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

        public static string ProcessScanFileOutput(ScanFile scan_file, QuasarModType qmt, int SlotNumber, string GameDataItem, bool DLC)
        {
            Regex FolderReplacinator = new Regex("{Folder}");
            Regex PartReplacinator = new Regex("{Part}");
            Regex GameDataReplacinator = new Regex("{GameData}");
            Regex SlotReplacinatorSingle = new Regex("{S0}");
            Regex SlotReplacinatorDouble = new Regex("{S00}");
            Regex SlotReplacinatorTriple = new Regex("{S000}");
            Regex AnyReplacinator = new Regex("{AnyFile}");

            QuasarModTypeFileDefinition def = qmt.QuasarModTypeFileDefinitions.Single(d => d.ID == scan_file.QuasarModTypeFileDefinitionID);
            string OutputFile = def.QuasarModTypeBuilderDefinitions[0].OutputFileName;
            string OutputPath = def.QuasarModTypeBuilderDefinitions[0].OutputPath;
            OutputPath = OutputPath.Replace("{DLC}", DLC ? "_patch" : "");

            string searchFile = def.SearchFileName;
            string searchPath = def.SearchPath;
            searchPath = def.SearchPath.Replace(@"/", @"\");
            Regex fileRegex = new Regex(PrepareRegex(searchFile));
            Regex folderRegex = new Regex(PrepareRegex(searchPath));

            Match m = fileRegex.Match(scan_file.FilePath);
            Match m2 = folderRegex.Match(scan_file.FilePath);

            //Processing File Output
            if(m.Groups["Part"].Captures.Count > 1)
            {
                foreach (string s in m.Groups["Part"].Captures)
                {
                    OutputFile = PartReplacinator.Replace(OutputFile, s, 1);
                }
            }
            else
            {
                OutputFile = PartReplacinator.Replace(OutputFile, m.Groups["Part"].Value, 1);
            }

            if (m.Groups["Folder"].Captures.Count > 1)
            {
                foreach (string s in m.Groups["Folder"].Captures)
                {
                    OutputFile = FolderReplacinator.Replace(OutputFile, s, 1);
                }
            }
            else
            {
                OutputFile = FolderReplacinator.Replace(OutputFile, m.Groups["Folder"].Value, 1);
            }

            OutputFile = GameDataReplacinator.Replace(OutputFile, GameDataItem, 1);

            OutputFile = SlotReplacinatorSingle.Replace(OutputFile, SlotNumber.ToString("0"), 1);
            OutputFile = SlotReplacinatorDouble.Replace(OutputFile, SlotNumber.ToString("00"), 1);
            OutputFile = SlotReplacinatorTriple.Replace(OutputFile, SlotNumber.ToString("000"), 1);

            if(m.Groups["AnyFile"].Success)
            {
                OutputFile = AnyReplacinator.Replace(OutputFile,Path.GetFileNameWithoutExtension(scan_file.SourcePath), 1);
            }

            //Processing Folder Output
            if (m2.Groups["Part"].Captures.Count > 1)
            {
                foreach (string s in m2.Groups["Part"].Captures)
                {
                    OutputFile = PartReplacinator.Replace(OutputFile, s, 1);
                }
            }
            else
            {
                OutputPath = PartReplacinator.Replace(OutputPath, m.Groups["Part"].Value, 1);
            }

            if (m2.Groups["Folder"].Captures.Count > 1)
            {
                foreach (string s in m2.Groups["Folder"].Captures)
                {
                    OutputPath = FolderReplacinator.Replace(OutputPath, s, 1);
                }
            }
            else
            {
                OutputPath = FolderReplacinator.Replace(OutputPath, m.Groups["Folder"].Value, 1);
            }

            OutputPath = GameDataReplacinator.Replace(OutputPath, GameDataItem, 1);

            OutputPath = SlotReplacinatorSingle.Replace(OutputPath, SlotNumber.ToString("0"), 1);
            OutputPath = SlotReplacinatorDouble.Replace(OutputPath, SlotNumber.ToString("00"), 1);
            OutputPath = SlotReplacinatorTriple.Replace(OutputPath, SlotNumber.ToString("000"), 1);

            OutputPath = OutputPath.Replace("{DLC}", DLC? "_patch" : "");


            return OutputPath + @"/" + OutputFile;
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

        public static string[] GetRootFolder(string _path)
        {
            Regex RootRegex = new Regex(String.Format(@"(?'RootFolder'{0})(?'FilePath'.*$)",RootFolders));
            Match m = RootRegex.Match(_path);

            if (m.Success)
            {
                string RootPath = _path.Replace(m.Groups["RootFolder"].Value, "");
                RootPath = RootPath.Replace(m.Groups["FilePath"].Value, "");
                return new[] { RootPath, m.Groups["RootFolder"].Value + m.Groups["FilePath"].Value };
            }
            else
            {
                return new[] { "", "" };
            }
            
        }
    }
}
