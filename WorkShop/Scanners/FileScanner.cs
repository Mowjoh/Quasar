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
        public static string[] IgnoreExtensions = { ".csv", ".jpg", ".txt", ".png" };
        public static string[] IgnoreFiles = { "APIData.json", "ContentData.json", "LibraryData.json", "Gamebanana.json", "ModInformation.json" };
        public static string RootFolders = "append|assist|boss|camera|campaign|common|effect|enemy|fighter|finalsmash|item|item|miihat|param|pokemon|prebuilt;|render|snapshot|stream;|sound|spirits|stage|standard|ui";

        #region Scanning

        /// <summary>
        /// Unused for now - Scans a Library Mod
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

            List<ScanFile> ModFiles = GetScanFiles(_folder_path);
            ModFiles = FilterIgnoredFiles(ModFiles);
            List<ScanFile> MatchedFiles = MatchScanFiles(ModFiles, _quasar_mod_types, _game, ModFolder);

            //Parsing Content Items from processed File Matches
            ObservableCollection<ContentItem> ParsedContentItems = ParseContentItems(MatchedFiles, _library_item);

            return ParsedContentItems;
        }

        #endregion

        #region List Processing

        /// <summary>
        /// Provides the list of files as a ScanFile Observable collection
        /// </summary>
        /// <param name="_FolderPath">The Mod's directory path</param>
        /// <returns>The list of all files as ScanFiles</returns>
        public static List<ScanFile> GetScanFiles(string _FolderPath)
        {
            //Listing files to scan
            List<ScanFile> FilesToScan = new List<ScanFile>();

            //Looping through the files in the specific path
            foreach (string s in Directory.GetFiles(_FolderPath, "*", SearchOption.AllDirectories))
            {
                //Getting game root folder if possible
                string[] paths = GetRootFolder(s);

                //Providing base information for the ScanFile
                FilesToScan.Add(new ScanFile()
                {
                    SourcePath = s,
                    RootPath = paths[0],
                    FilePath = paths[1]

                });

            }

            return FilesToScan;
        }

        /// <summary>
        /// Filters out Scanfiles with a specific extension or filename
        /// </summary>
        /// <param name="_FilesToFilter">List of ScanFiles to filter</param>
        /// <returns>The Filtered ScanFiles</returns>
        public static List<ScanFile> FilterIgnoredFiles(List<ScanFile> _FilesToFilter)
        {
            //Looping through files where the filename or extension matches their ignore list
            foreach (ScanFile sf in _FilesToFilter.Where(sf =>
                         IgnoreFiles.Any(i => i == Path.GetFileName(sf.SourcePath)) ||
                         IgnoreExtensions.Any(i => i == Path.GetExtension(sf.SourcePath))))
            {
                sf.Scanned = true;
                sf.Ignored = true;
}

            return _FilesToFilter;
        }

        #endregion

        #region File Matching
        /// <summary>
        /// Launches the matching process for a ScanFile list
        /// </summary>
        /// <param name="_files_to_match"></param>
        /// <param name="_quasar_mod_types"></param>
        /// <param name="_game"></param>
        /// <param name="_mod_folder"></param>
        /// <returns></returns>
        public static List<ScanFile> MatchScanFiles(List<ScanFile> _files_to_match, ObservableCollection<QuasarModType> _quasar_mod_types, Game _game, string _mod_folder)
        {
            List<ScanFile> Workload = _files_to_match;
            List<ScanFile> Output = new List<ScanFile>();

            //Scanning Files by type priority then file priority
            foreach (QuasarModType modType in _quasar_mod_types.OrderBy(i => i.TypePriority))
            {
                foreach (QuasarModTypeFileDefinition fileDefinition in modType.QuasarModTypeFileDefinitions.OrderBy(o => o.FilePriority).ToList())
                {
                    if (Workload.Count > 0)
                    {
                        //Trying to match files
                        Workload = MatchFilesWithDefinition(Workload, fileDefinition, modType, _game, _mod_folder);

                        //Adding matched files to the output list
                        Output.AddRange(Workload.Where(sf => sf.Scanned));

                        //Removing scanned files from the workload
                        List<ScanFile> newLoad = Workload.Where(sf => !sf.Scanned).ToList();
                        Workload.Clear();
                        Workload.AddRange(newLoad);
                    }
                }
            }

            //Adding remaining non scanned items
            Output.AddRange(Workload);

            return Output;
        }
        /// <summary>
        /// Matches all scan files to a file definition and game entries
        /// </summary>
        /// <param name="_files_to_match"></param>
        /// <param name="_file_definition"></param>
        /// <param name="_mod_type"></param>
        /// <param name="_game"></param>
        /// <param name="_mod_folder"></param>
        /// <returns></returns>
        public static List<ScanFile> MatchFilesWithDefinition(List<ScanFile> _files_to_match, QuasarModTypeFileDefinition _file_definition, QuasarModType _mod_type, Game _game, string _mod_folder)
        {
            List<ScanFile> Output = new List<ScanFile>();
            Regex FolderRegex = new Regex(PrepareRegex(_file_definition.SearchPath));

            //Looping through the workload
            if (_mod_type.BasicSearch)
            {
                foreach (ScanFile FileToMatch in _files_to_match)
                {
                    Output.Add(BasicMatch(FileToMatch, FolderRegex, _mod_type, _file_definition, _mod_folder));
                }
            }
            else
            {
                Regex FileRegex = new Regex(PrepareRegex(_file_definition.SearchFileName));
                foreach (ScanFile FileToMatch in _files_to_match)
                {
                    Output.Add(AdvancedMatch(FileToMatch, FolderRegex, FileRegex, _mod_type, _file_definition, _game.GameElementFamilies, _mod_folder));
                }
            }

            return Output;
        }
        /// <summary>
        /// Tries to match a Scan File to a specific folder
        /// </summary>
        /// <param name="_scan_file">Scan File to match</param>
        /// <param name="_folder_regex">Regex to use</param>
        /// <param name="_mod_type">The Quasar Mod Type containing the File Definition</param>
        /// <param name="_file_definition">The File Definition</param>
        /// <returns>The scan file with an updated status if the match was successful</returns>
        public static ScanFile BasicMatch(ScanFile _scan_file, Regex _folder_regex, QuasarModType _mod_type, QuasarModTypeFileDefinition _file_definition, string _mod_folder)
        {
            Match folderMatch = _folder_regex.Match(_scan_file.SourcePath.Replace('\\', '/').ToLower());

            //Exiting if there is no match
            if (!folderMatch.Success)
                return _scan_file;

            //Registering match data within the ScanFile
            _scan_file = RegisterMatchData(_scan_file, _mod_type, _file_definition,_mod_folder: _mod_folder);
            _scan_file = GetPreprocessedOutput(_scan_file, folderMatch, _file_definition);

            return _scan_file;
        }
        /// <summary>
        /// Tries to match a Scan File to a specific folder and file name/type
        /// Also parses additional data that can be parsed along if the match is successful
        /// </summary>
        /// <param name="scan_file"></param>
        /// <param name="folder_regex"></param>
        /// <param name="file_regex"></param>
        /// <param name="mod_type"></param>
        /// <param name="file_definition"></param>
        /// <param name="_families"></param>
        /// <returns></returns>
        public static ScanFile AdvancedMatch(ScanFile scan_file, Regex folder_regex, Regex file_regex, QuasarModType mod_type, QuasarModTypeFileDefinition file_definition, ObservableCollection<GameElementFamily> _families, string _mod_folder)
        {
            Match folderMatch = folder_regex.Match(scan_file.SourcePath.Replace('\\', '/').ToLower());
            Match fileMatch = file_regex.Match(scan_file.SourcePath.Replace('\\', '/'));

            if (fileMatch.Success)
            {
                //Match Data
                string folderSlot = "";
                string fileSlot = "";
                GameElement folderGameData = null;
                GameElement fileGameData = null;
                GameElementFamily family = _families.Single(f => f.ID == mod_type.GameElementFamilyID);

                //Match Data Processing
                if (folderMatch.Success)
                {
                    folderGameData = GetAssociatedGameData(folderMatch, family);
                    folderSlot = GetSlot(folderMatch);
                }
                fileGameData = GetAssociatedGameData(fileMatch, family);
                fileSlot = GetSlot(fileMatch);

                //Match Validation & Registration
                if (MatchIsValid(fileGameData != null, folderGameData != null, file_definition.SearchPath == ""
                        , folderMatch.Success, mod_type.NoGameElement))
                {
                    scan_file = RegisterMatchData(scan_file, mod_type, file_definition, fileGameData, folderGameData, fileSlot, folderSlot, _mod_folder);
                    scan_file = GetPreprocessedOutput(scan_file, folderMatch, fileMatch, file_definition);
                }
                    
            }

            return scan_file;
        }
       /// <summary>
       /// Function to define if an Advanced Match has all the necessary information
       /// </summary>
       /// <param name="_file_element"></param>
       /// <param name="_folder_element"></param>
       /// <param name="_file_definition"></param>
       /// <param name="_file_success"></param>
       /// <param name="_folder_success"></param>
       /// <param name="_no_game_element"></param>
       /// <returns></returns>
        public static bool MatchIsValid(bool _file_element_present, bool _folder_element_present, bool _empty_folder_defintion, bool _folder_success, bool _no_game_element)
        {
            bool GameElementRecognized = _file_element_present || _folder_element_present || _no_game_element;
            bool PathRecognized = _empty_folder_defintion || _folder_success;

            bool AllConditionsTrue = GameElementRecognized && PathRecognized;

            return AllConditionsTrue;
        }
        /// <summary>
        /// Registers match data within the provided Scan File
        /// </summary>
        /// <param name="scan_file"></param>
        /// <param name="mod_type"></param>
        /// <param name="file_definition"></param>
        /// <param name="_file_element"></param>
        /// <param name="_folder_element"></param>
        /// <param name="_file_slot"></param>
        /// <param name="_folder_slot"></param>
        /// <returns></returns>
        public static ScanFile RegisterMatchData(ScanFile scan_file, QuasarModType mod_type, QuasarModTypeFileDefinition file_definition, GameElement _file_element = null, GameElement _folder_element = null, string _file_slot = "", string _folder_slot = "", string _mod_folder = "")
        {
            //Registering match data within the ScanFile
            scan_file.QuasarModTypeID = mod_type.ID;
            scan_file.QuasarModTypeFileDefinitionID = file_definition.ID;
            scan_file.Slot = _folder_slot != "" ? _folder_slot : _file_slot != "" ? _file_slot : "00";
            if (_file_element != null || _folder_element != null)
                scan_file.GameElementID = _file_element != null ? _file_element.ID : _folder_element.ID;

            //Debug Values
            scan_file.QuasarModTypeName = mod_type.GroupName + " | " + mod_type.Name;
            scan_file.QuasarModTypeFileRule = file_definition.SearchPath + "/" + file_definition.SearchFileName;

            //Processing paths
            scan_file.OriginPath = scan_file.SourcePath.Replace('/', '\\').Replace(_mod_folder, "");
            if (file_definition.SearchPath == "")
            {
                scan_file.OriginPath = scan_file.SourcePath;
            }

            scan_file.Scanned = true;

            return scan_file;
        }

        #endregion

        #region Content Grouping

        /// <summary>
        /// Processes ScanFiles into a collection of ContentItems
        /// </summary>
        /// <param name="_ScanFiles">ScanFiles to process</param>
        /// <param name="_LibraryItem">Tied Library Item</param>
        /// <returns></returns>
        public static ObservableCollection<ContentItem> ParseContentItems(List<ScanFile> _ScanFiles, LibraryItem _LibraryItem)
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

        #endregion

        #region Output Generation
        public static ScanFile GetPreprocessedOutput(ScanFile _scan_file, Match _folder_match, QuasarModTypeFileDefinition _file_definition)
        {
            string destinationfolder = "{ModName}/" + _folder_match.Value;
            if (!destinationfolder.EndsWith(@"/"))
                destinationfolder += "/";

            string destinationfile = Path.GetFileName(_scan_file.SourcePath);

            _scan_file.PreProcessedOutputFilePath = $@"{destinationfolder}{destinationfile}";
            return _scan_file;
        }

        /// <summary>
        /// Processes the final path to transfer the file to
        /// </summary>
        /// <param name="scan_file"></param>
        /// <param name="qmt"></param>
        /// <param name="SlotNumber"></param>
        /// <param name="GameDataItem"></param>
        /// <param name="DLC"></param>
        /// <returns></returns>
        public static ScanFile GetPreprocessedOutput(ScanFile scan_file,  Match _folder_match , Match _file_match, QuasarModTypeFileDefinition _file_definition)
        {
            //Setting up variables
            Regex FolderReplacinator = new Regex("{Folder}");
            Regex PartReplacinator = new Regex("{Part}");
            Regex AnyReplacinator = new Regex("{AnyFile}");
            Regex AnyExtReplacinator = new Regex("{AnyExtension}");

            string OutputFile = _file_definition.SearchFileName;
            string OutputPath = _file_definition.OutputPath;

            //Replacing tags
            OutputFile = ReplaceTagWithValue(OutputFile,_file_match, "Part", PartReplacinator);
            OutputFile = ReplaceTagWithValue(OutputFile,_file_match, "Folder", FolderReplacinator);
            OutputFile = ReplaceTagWithValue(OutputFile,_file_match, "AnyFile", AnyReplacinator);
            OutputFile = ReplaceTagWithValue(OutputFile,_file_match, "AnyExtension", AnyExtReplacinator);

            OutputPath = ReplaceTagWithValue(OutputPath, _folder_match, "Folder", FolderReplacinator);
            OutputPath = ReplaceTagWithValue(OutputPath, _folder_match, "Part", PartReplacinator);

            scan_file.PreProcessedOutputFilePath = $@"{OutputPath}/{OutputFile}";
            return scan_file;
        }
        

        /// <summary>
        /// Processes the final path to transfer the file to
        /// </summary>
        /// <param name="scan_file"></param>
        /// <param name="qmt"></param>
        /// <param name="SlotNumber"></param>
        /// <param name="GameDataItem"></param>
        /// <param name="DLC"></param>
        /// <returns></returns>
        public static string ProcessFinalOutput(ScanFile scan_file, QuasarModType qmt, int SlotNumber, string GameDataItem, bool DLC)
        {
            //Setting up variables
            Regex GameDataReplacinator = new Regex("{GameData}");
            Regex SlotReplacinatorSingle = new Regex("{S0}");
            Regex SlotReplacinatorDouble = new Regex("{S00}");
            Regex SlotReplacinatorTriple = new Regex("{S000}");

            QuasarModTypeFileDefinition def = qmt.QuasarModTypeFileDefinitions.Single(d => d.ID == scan_file.QuasarModTypeFileDefinitionID);
            string OutputFile = def.SearchFileName;
            string OutputPath = def.OutputPath;

            //Replacing GameData tags
            OutputFile = GameDataReplacinator.Replace(OutputFile, GameDataItem, 1);
            OutputPath = GameDataReplacinator.Replace(OutputPath, GameDataItem, 1);

            //Replacing Slots tags
            OutputFile = SlotReplacinatorSingle.Replace(OutputFile, SlotNumber.ToString("0"), 1);
            OutputFile = SlotReplacinatorDouble.Replace(OutputFile, SlotNumber.ToString("00"), 1);
            OutputFile = SlotReplacinatorTriple.Replace(OutputFile, SlotNumber.ToString("000"), 1);
            OutputPath = SlotReplacinatorSingle.Replace(OutputPath, SlotNumber.ToString("0"), 1);
            OutputPath = SlotReplacinatorDouble.Replace(OutputPath, SlotNumber.ToString("00"), 1);
            OutputPath = SlotReplacinatorTriple.Replace(OutputPath, SlotNumber.ToString("000"), 1);

            OutputPath = OutputPath.Replace("{DLC}", DLC ? "_patch" : "");


            return OutputPath + @"/" + OutputFile;
        }

        public static string ReplaceTagWithValue(string _path, Match _match, string _name, Regex _regex)
        {
            for (int i = 0; i < _match.Groups[_name].Captures.Count; i++)
            {
                _path = _regex.Replace(_path, _match.Groups[_name].Value, 1);
            }

            return _path;
        }
        #endregion

        #region Tools

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
            output = output.Replace(@"{AnyExtension}", @"(?'AnyExtension'[a-zA-Z0-9\_]*)");
            output = output.Replace(@"{AnyPath}", @"(?'AnyPath'[a-zA-Z0-9\_\\\/]*[\\\/])");

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

        public static string FinalDestination(string _Input)
        {
            string output = _Input;
            Regex slotRegex = new(@"(?'Slot'\d{1})");

            return output;
        }

        public static string[] GetRootFolder(string _path)
        {
            Regex RootRegex = new Regex(String.Format(@"(?'RootFolder'{0})(?'FilePath'.*$)", RootFolders));
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
        public static GameElement GetAssociatedGameData(Match _match, GameElementFamily _family)
        {
            //Getting match data
            Group GameData = _match.Groups["GameData"];
            string FolderGameData = GameData.Value;

            //Getting associated Game Element
            GameElement recognizedGameData = _family.GameElements.SingleOrDefault(
                ge => (ge.GameFolderName == FolderGameData.ToLower())
                      || (ge.GameFolderName.Contains(";") ?
                          ge.GameFolderName.Split(';').Contains(FolderGameData.ToLower()) :
                          false));

            return recognizedGameData;
        }
        public static string GetSlot(Match _match)
        {
            //Getting Slot Value from Match
            Group Slot = _match.Groups["Slot"];
            string slotValue = Slot.Value;

            return slotValue;
        }
        #endregion
    }
}
