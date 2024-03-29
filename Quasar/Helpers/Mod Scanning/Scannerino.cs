﻿using DataModels.User;
using DataModels.Resource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Quasar.MainUI.ViewModels;


namespace Quasar.Helpers.ModScanning
{
    public static class Scannerino
    {
        /// <summary>
        /// Does a full scan for every mod in the Library
        /// </summary>
        /// <param name="MUVM"></param>
        public static void ScanAllMods(MainUIViewModel MUVM)
        {
            foreach(LibraryItem li in MUVM.Library)
            {
                UpdateContents(MUVM, li);
            }
        }

        /// <summary>
        /// Updates all Content Items for a specific Library Mod
        /// </summary>
        /// <param name="MUVM"></param>
        /// <param name="li"></param>
        /// <param name="ScannedResults"></param>
        public static void UpdateContents(MainUIViewModel MUVM, LibraryItem li, ObservableCollection<ContentItem> ScannedResults = null)
        {
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", Properties.QuasarSettings.Default.DefaultDir, li.Guid);

            //Getting all scanned Content Items
            ObservableCollection<ContentItem> ScanResults;
            if (ScannedResults == null)
            {
                ScanResults = ScanMod(ModFolder, MUVM.QuasarModTypes, MUVM.Games[0], li);
            }
            else
            {
                ScanResults = ScannedResults;
            }
            
            //Getting all stored Content Items with the same Library Item
            List<ContentItem> ItemsToUpdate = MUVM.ContentItems.Where(c => c.LibraryItemGuid == li.Guid).ToList();
            List<ContentItem> UpdatedItems = new List<ContentItem>();
            foreach(ContentItem ScannedItem in ScanResults)
            {
                List<ContentItem> MatchingContentItems = MUVM.ContentItems.Where(ci => ci.LibraryItemGuid == ScannedItem.LibraryItemGuid && ci.GameElementID == ScannedItem.GameElementID && ci.SlotNumber == ScannedItem.SlotNumber && ci.QuasarModTypeID == ScannedItem.QuasarModTypeID).ToList();
                if (MatchingContentItems.Count < 1)
                {
                    //If there are no Content Items matching the Scanned Item
                    ScannedItem.Guid = Guid.NewGuid();
                    MUVM.ContentItems.Add(ScannedItem);
                }
                else
                {
                    bool Matched = false;
                    foreach(ContentItem ci in MatchingContentItems)
                    {
                        if (ci.ScanFiles[0].OriginPath.Replace('/','\\') == ScannedItem.ScanFiles[0].OriginPath.Replace('/', '\\'))
                        {
                            int i = MUVM.ContentItems.IndexOf(ci);
                            MUVM.ContentItems[i].Name = ScannedItem.Name;
                            MUVM.ContentItems[i].ScanFiles = ScannedItem.ScanFiles;
                            Matched = true;
                            UpdatedItems.Add(ci);
                        }
                    }
                    if (!Matched)
                    {
                        //If there are no Content Items matching the Scanned Item
                        ScannedItem.Guid = Guid.NewGuid();
                        MUVM.ContentItems.Add(ScannedItem);
                    }
                }
            }

            foreach(ContentItem ci in ItemsToUpdate)
            {
                if (!UpdatedItems.Contains(ci))
                {
                    MUVM.ContentItems.Remove(ci);
                }
            }

        }

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
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", Properties.QuasarSettings.Default.DefaultDir, LibraryItem.Guid);
            ScannedFiles = GetScanFiles(FolderPath, QuasarModTypes, game, ModFolder, FileList);

            //Processing Search Results into ContentMappings
            foreach (ScanFile sf in ScannedFiles)
            {
                if(sf.Scanned == true)
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
                        foreach(ContentItem ci in SearchList)
                        {
                            //If there is a related parent
                            if (ci.ScanFiles[0].OriginPath.Replace('/','\\') == sf.OriginPath.Replace('/', '\\'))
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
       
        /// <summary>
        /// Scans all files for a specific path
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <param name="QuasarModTypes"></param>
        /// <param name="game"></param>
        /// <param name="ModFolder"></param>
        /// <param name="FileList"></param>
        /// <returns>The list of all scanned files</returns>
        public static ObservableCollection<ScanFile> GetScanFiles(string FolderPath, ObservableCollection<QuasarModType> QuasarModTypes, Game game,string ModFolder, bool FileList = false)
        {
            ObservableCollection<ScanFile> ScanResults = new ObservableCollection<ScanFile>();

            //Listing files to scan
            ObservableCollection<ScanFile> FilesToScan = new ObservableCollection<ScanFile>();
            ObservableCollection<ScanFile> ScannedFiles = new ObservableCollection<ScanFile>();
            if (FileList){
                System.IO.StreamReader file =  new System.IO.StreamReader(FolderPath);
                string line = "";
                while ((line = file.ReadLine()) != null)
                {
                    FilesToScan.Add(new ScanFile()
                    {
                        SourcePath = line
                    });
                }
            }
            else
            {
                foreach (string s in Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories))
                {
                    FilesToScan.Add(new ScanFile()
                    {
                        SourcePath = s
                    });
                }
            }
            

            //Scanning Files
            foreach (QuasarModType qmt in QuasarModTypes.OrderBy(i => i.TypePriority))
            {
                foreach (QuasarModTypeFileDefinition FileDefinition in qmt.QuasarModTypeFileDefinitions.OrderBy(o => o.FilePriority).ToList())
                {
                    if (FilesToScan.Count > 0)
                    {
                        FilesToScan = MatchFiles(FilesToScan, FileDefinition, qmt, game, ModFolder);
                        foreach (ScanFile Scanned in FilesToScan.Where(sf => sf.Scanned))
                        {
                            ScannedFiles.Add(Scanned);
                        }
                        foreach (ScanFile Scanned in ScannedFiles)
                        {
                            if (FilesToScan.Contains(Scanned))
                                FilesToScan.Remove(Scanned);
                        }
                    }
                }
            }
            foreach(ScanFile sf in FilesToScan)
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
        public static ObservableCollection<ScanFile> MatchFiles(ObservableCollection<ScanFile> FilesToMatch, QuasarModTypeFileDefinition FileDefinition,QuasarModType qmt, Game game, string ModFolder)
        {
            Regex FileRegex = new Regex(PrepareRegex(FileDefinition.SearchFileName));
            Regex FolderRegex = new Regex(PrepareRegex(FileDefinition.SearchPath));

            foreach(ScanFile FileToMatch in FilesToMatch)
            {
                Match fileMatch = FileRegex.Match(FileToMatch.SourcePath.Replace('\\','/'));
                Match folderMatch = FolderRegex.Match(FileToMatch.SourcePath.Replace('\\', '/'));

                if(folderMatch.Success || fileMatch.Success)
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
        /// Outputs a list of Mod Files that can be built
        /// </summary>
        /// <param name="qmt"></param>
        /// <param name="Family"></param>
        /// <param name="ci"></param>
        /// <param name="Slot"></param>
        /// <param name="ModLoader"></param>
        /// <param name="li"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static ObservableCollection<ModFile> GetModFiles(QuasarModType qmt, GameElementFamily Family, ContentItem ci, int Slot, int ModLoader, LibraryItem li, Game game)
        {
            string DefaultDir = Properties.QuasarSettings.Default.DefaultDir;
            ObservableCollection<ModFile> PreparedModFiles = new ObservableCollection<ModFile>();
            
            foreach(ScanFile sf in ci.ScanFiles)
            {
                QuasarModTypeFileDefinition FileDefinition = qmt.QuasarModTypeFileDefinitions.Single(def => def.ID == sf.QuasarModTypeFileDefinitionID);
                GameElement ge = Family.GameElements.Single(e => e.ID == sf.GameElementID);
                string ModFolder = Properties.QuasarSettings.Default.DefaultDir + @"\Library\Mods\" + li.Guid + @"\";

                ModFile mf = new ModFile()
                {
                    SourceFilePath = ModFolder + sf.SourcePath,
                    DestinationFilePath = ProcessOutput(ModFolder + sf.SourcePath, FileDefinition, ge, Slot, ModLoader,li, ModFolder),
                    LibraryItemGuid = ci.LibraryItemGuid
                };

                PreparedModFiles.Add(mf);
                
            }
            return PreparedModFiles;
        }

        /// <summary>
        /// Processes a Mod File's output path for building
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="FileDefinition"></param>
        /// <param name="GameElement"></param>
        /// <param name="Slot"></param>
        /// <param name="ModLoader"></param>
        /// <param name="li"></param>
        /// <param name="ModFolder"></param>
        /// <returns></returns>
        public static string ProcessOutput(string SourcePath,QuasarModTypeFileDefinition FileDefinition, GameElement GameElement, int Slot, int ModLoader, LibraryItem li, string ModFolder)
        {
            //Setup
            QuasarModTypeBuilderDefinition Definition = FileDefinition.QuasarModTypeBuilderDefinitions.Single(d => d.ModLoaderID == ModLoader);
            Regex FileRegex = new Regex(PrepareRegex(FileDefinition.SearchFileName));
            Regex FolderRegex = new Regex(PrepareRegex(FileDefinition.SearchPath));

            //Getting File Match values
            Match fileMatch = FileRegex.Match(ModFolder + SourcePath.Replace('\\', '/'));
            Group FilePartGroup = fileMatch.Groups["Part"];
            Group FileFolderGroup = fileMatch.Groups["Folder"];
            Group FileAnyFileGroup = fileMatch.Groups["AnyFile"];

            Match folderMatch = FolderRegex.Match(ModFolder + SourcePath.Replace('\\', '/'));
            Group FolderPartGroup = folderMatch.Groups["Part"];
            Group FolderFolderGroup = folderMatch.Groups["Folder"];

            //Constructing output
            string OutputFile = Definition.OutputFileName;
            string OutputPath = Definition.OutputPath;
            Regex ModNameReplacinator = new Regex("{ModName}");
            Regex ModNameSlotReplacinator = new Regex("{ModNameSlot}");
            Regex illegalInFileName = new Regex(@"[\\/:*?""<>|.]");
            Regex FolderReplacinator = new Regex("{Folder}");
            Regex PartReplacinator = new Regex("{Part}");
            Regex GameDataReplacinator = new Regex("{GameData}");
            Regex SlotReplacinatorSingle = new Regex("{S0}");
            Regex SlotReplacinatorDouble = new Regex("{S00}");
            Regex SlotReplacinatorTriple = new Regex("{S000}");
            Regex AnyReplacinator = new Regex("{AnyFile}");
            
            //Processing File Output
            if (FilePartGroup.Captures.Count > 0)
            {
                CaptureCollection cc = FilePartGroup.Captures;
                foreach (Capture c in cc)
                {
                    OutputFile = PartReplacinator.Replace(OutputFile, c.Value, 1);
                }
            }
            if (FileFolderGroup.Captures.Count > 0)
            {
                CaptureCollection cc = FileFolderGroup.Captures;
                foreach (Capture c in cc)
                {
                    OutputFile = FolderReplacinator.Replace(OutputFile, c.Value, 1);
                }
            }

            if (GameElement.GameFolderName.Contains(';'))
            {
                string ProperGameFolder = "";

                //Match Data Processing
                if (fileMatch.Success)
                {
                    Group GameData = fileMatch.Groups["GameData"];
                    if (GameData.Value != "")
                        ProperGameFolder = GameData.Value;
                    OutputFile = GameDataReplacinator.Replace(OutputFile, ProperGameFolder, 1);
                }
                
            }
            else
            {
                OutputFile = GameDataReplacinator.Replace(OutputFile, GameElement.GameFolderName, 1);
            }

            

            OutputFile = SlotReplacinatorSingle.Replace(OutputFile, Slot.ToString("0"), 1);
            OutputFile = SlotReplacinatorDouble.Replace(OutputFile, Slot.ToString("00"), 1);
            OutputFile = SlotReplacinatorTriple.Replace(OutputFile, Slot.ToString("000"), 1);

            if (FileAnyFileGroup.Captures.Count > 0)
            {
                OutputFile = AnyReplacinator.Replace(OutputFile, FileAnyFileGroup.Value, 1);
            }

            //Processing Folder Output
            if (FolderPartGroup.Captures.Count > 0)
            {
                CaptureCollection cc = FolderPartGroup.Captures;
                foreach (Capture c in cc)
                {
                    OutputPath = PartReplacinator.Replace(OutputPath, c.Value, 1);
                }
            }

            if (FolderFolderGroup.Captures.Count > 0)
            {
                CaptureCollection cc = FolderFolderGroup.Captures;
                foreach (Capture c in cc)
                {
                    OutputPath = FolderReplacinator.Replace(OutputPath, c.Value, 1);
                }
            }

            if (GameElement.GameFolderName.Contains(';'))
            {
                string ProperGameFolder = "";

                //Match Data Processing
                if (folderMatch.Success)
                {
                    Group GameData = folderMatch.Groups["GameData"];
                    if (GameData.Value != "")
                        ProperGameFolder = GameData.Value;
                    OutputPath = GameDataReplacinator.Replace(OutputPath, ProperGameFolder, 1);
                }
            }
            else
            {
                OutputPath = GameDataReplacinator.Replace(OutputPath, GameElement.GameFolderName, 1);
            }
            

            OutputPath = SlotReplacinatorSingle.Replace(OutputPath, Slot.ToString("0"), 1);
            OutputPath = SlotReplacinatorDouble.Replace(OutputPath, Slot.ToString("00"), 1);
            OutputPath = SlotReplacinatorTriple.Replace(OutputPath, Slot.ToString("000"), 1);

            OutputPath = ModNameReplacinator.Replace(OutputPath, illegalInFileName.Replace(li.Name,""));
            OutputPath = ModNameSlotReplacinator.Replace(OutputPath, String.Format("{0} Slot {1}", illegalInFileName.Replace(li.Name, ""), (Slot+1).ToString()));


            OutputPath = OutputPath.Replace("{DLC}", GameElement.isDLC == true ? "_patch" : "");
            return OutputPath+"/"+OutputFile;
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

    }
}
