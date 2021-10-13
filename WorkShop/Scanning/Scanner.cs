using DataModels.Resource;
using DataModels.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Workshop.Scanning
{
    
    public static class Scanner
    {
        static string[] ExtensionIgnores = { ".json" };
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

        /// <summary>
        /// Scans all files for a specific path
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <param name="QuasarModTypes"></param>
        /// <param name="game"></param>
        /// <param name="ModFolder"></param>
        /// <param name="FileList"></param>
        /// <returns>The list of all scanned files</returns>
        public static ObservableCollection<ScanFile> GetScanFiles(string FolderPath, ObservableCollection<QuasarModType> QuasarModTypes, Game game, string ModFolder, bool FileList = false)
        {
            ObservableCollection<ScanFile> ScanResults = new ObservableCollection<ScanFile>();

            //Listing files to scan
            ObservableCollection<ScanFile> FilesToScan = new ObservableCollection<ScanFile>();
            ObservableCollection<ScanFile> ScannedFiles = new ObservableCollection<ScanFile>();
            if (FileList)
            {
                System.IO.StreamReader file = new System.IO.StreamReader(FolderPath);
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

            FilesToScan = IgnoreFiles(FilesToScan);

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
            foreach (ScanFile sf in FilesToScan)
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
        public static ObservableCollection<ScanFile> MatchFiles(ObservableCollection<ScanFile> FilesToMatch, QuasarModTypeFileDefinition FileDefinition, QuasarModType qmt, Game game, string ModFolder)
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

        public static ObservableCollection<ScanFile> IgnoreFiles(ObservableCollection<ScanFile> FilesToVerify)
        {

            foreach(ScanFile sf in FilesToVerify)
            {
                foreach(string extension in ExtensionIgnores)
                {
                    if(Path.GetExtension(sf.SourcePath) == extension)
                    {
                        sf.Ignored = true;
                        sf.Scanned = true;
                    }
                }
            }

            return FilesToVerify;
        }
    }
}
