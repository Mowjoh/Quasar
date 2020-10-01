using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Quasar.FileSystem;
using Quasar.Internal.Tools;
using Quasar.XMLResources;
using static Quasar.XMLResources.Library;

namespace Quasar.Quasar_Sys
{
    public class Searchie
    {
        public enum MatchTypes : int { FullMatch = 0, FileMatch = 1, ParentMatch = 2 } 

        //Launches Detection process for a specific Mod
        public static ObservableCollection<ContentMapping> AutoDetectinator(LibraryMod LibraryMod,ObservableCollection<InternalModType> InternalModTypes, Game Game, ObservableCollection<GameData> GameDatas)
        {
            if(LibraryMod != null)
            {

                ObservableCollection<ContentMapping> mappings = new ObservableCollection<ContentMapping>();
                ModFileManager modFileManager = new ModFileManager(LibraryMod, Game);
                //Getting mod files
                IEnumerable<string> LibraryModFiles = Directory.EnumerateFiles(modFileManager.LibraryContentFolderPath, "*.*", SearchOption.AllDirectories);
                List<QuasarFileManager> QFMList = new List<QuasarFileManager>();
                foreach (string s in LibraryModFiles)
                {
                    QFMList.Add(new QuasarFileManager() { Path = s.Replace("\\", "/") });
                }

                foreach (InternalModType type in InternalModTypes)
                {
                    //Getting data corresponding to current type
                    GameData SpecificGameData = GameDatas.SingleOrDefault(g => g.GameID == LibraryMod.GameID);
                    GameDataCategory SpecificCategory = SpecificGameData.Categories.Find(gdc => gdc.ID == type.Association);

                    //Searching for matches for each of the files included in the Internal Mod Type
                    foreach (InternalModTypeFile IMTF in type.Files)
                    {
                        EvaluateForTypeFile(type, IMTF, QFMList, SpecificCategory);
                    }

                    //Formatting results as Content Mappings
                    foreach (QuasarFileManager qfm in QFMList)
                    {
                        if (qfm.ValidFile)
                        {
                            ContentMapping cm = null;

                            InternalModType IMT = InternalModTypes.SingleOrDefault(t => t.ID == qfm.InternalModTypeID);
                            SpecificCategory = SpecificGameData.Categories.Find(gdc => gdc.ID == IMT.Association);
                            if(SpecificCategory != null)
                            {
                                int GameDataItemID = SpecificCategory.Items.Find(i => i.Attributes[0].Value == qfm.GameData).ID;
                                if (mappings.Count != 0)
                                {
                                    cm = mappings.SingleOrDefault(existingMapping => existingMapping.Slot == qfm.SlotInt && existingMapping.InternalModType == qfm.InternalModTypeID && existingMapping.GameDataItemID == GameDataItemID);
                                }
                                if (cm == null)
                                {

                                    ContentMapping newMapping = new ContentMapping()
                                    {
                                        ID = IDGenerator.getNewContentID(),
                                        SlotName = qfm.Slot == null ? "00" : qfm.Slot,
                                        Slot = qfm.Slot == null ? 0 : int.Parse(qfm.Slot),
                                        ModID = LibraryMod.ID,
                                        InternalModType = qfm.InternalModTypeID,
                                        GameDataItemID = GameDataItemID,
                                        Name = String.Format("{0} - Slot {1}", LibraryMod.Name, (qfm.Slot == null ? 1 : int.Parse(qfm.Slot) +1).ToString()),

                                    };
                                    List<ContentMappingFile> Files = new List<ContentMappingFile>();
                                    newMapping.Files = Files;
                                    newMapping.Files.Add(new ContentMappingFile()
                                    {
                                        InternalModTypeFileID = qfm.InternalModTypeFileID,
                                        Path = qfm.Path,
                                        SourcePath = qfm.Path,
                                        FileFolders = qfm.FileFolders,
                                        FileParts = qfm.FileParts,
                                        FolderFolders = qfm.FolderFolders,
                                        FolderParts = qfm.FolderParts,
                                        AnyFile = qfm.AnyFile
                                    });
                                    mappings.Add(newMapping);
                                }
                                else
                                {
                                    ContentMappingFile ExpectedContentMapping = new ContentMappingFile()
                                    {
                                        InternalModTypeFileID = qfm.InternalModTypeFileID,
                                        Path = qfm.Path,
                                        SourcePath = qfm.Path,
                                        FileFolders = qfm.FileFolders,
                                        FileParts = qfm.FileParts,
                                        FolderFolders = qfm.FolderFolders,
                                        FolderParts = qfm.FolderParts,
                                        AnyFile = qfm.AnyFile
                                    };

                                    if(!cm.Files.Exists(cmf => cmf.Path == ExpectedContentMapping.Path))
                                    {
                                        cm.Files.Add(ExpectedContentMapping);
                                    }
                                }
                            }
                            
                        }
                        
                    }
                }
                return mappings;
            }
            return null;
        }

        public static void EvaluateForTypeFile(InternalModType IMT, InternalModTypeFile IMTF, List<QuasarFileManager> QFMList, GameDataCategory gdc)
        {
            Regex FileRegex = new Regex(Searchie.PrepareRegex(IMTF.SourceFile));
            Regex FolderRegex = new Regex(Searchie.PrepareRegex(IMTF.SourcePath));

            foreach(QuasarFileManager qfm in QFMList)
            {
                if (!qfm.ValidFile)
                {
                    Match fileMatch = FileRegex.Match(qfm.Path);
                    Match folderMatch = FolderRegex.Match(qfm.Path);

                    if (folderMatch.Success)
                    {
                        qfm.FolderMatch = folderMatch.Value;
                        getMatchValues(qfm, folderMatch.Groups, false);

                    }
                    else
                    {
                        qfm.FolderMatch = null;
                        qfm.FolderFolders = null;
                        qfm.FolderGameData = null;
                        qfm.FolderParts = null;
                    }
                    if (fileMatch.Success)
                    {
                        qfm.FileMatch = fileMatch.Value;
                        if (folderMatch.Success)
                        {
                            getMatchValues(qfm, fileMatch.Groups, true);
                            if(qfm.FileGameData == "ryu")
                            {
                                Console.Write("s");
                            }
                            //If there is a detected Folder Gamedata but not for the file
                            if ((qfm.FileGameData == "" || qfm.FileGameData == null) && qfm.FolderGameData != null)
                            {
                                if (ValidateGameData(qfm.FolderGameData, gdc))
                                {
                                    qfm.ValidFile = true;
                                    qfm.GameData = qfm.FolderGameData;
                                    qfm.InternalModType = IMT.Name;
                                    qfm.InternalModTypeID = IMT.ID;
                                    qfm.InternalModTypeFileID = IMTF.ID;
                                }
                            }
                            //If there is a detected File Gamedata
                            if (qfm.FileGameData != "" && qfm.FileGameData != null)
                            {
                                if (ValidateGameData(qfm.FileGameData, gdc))
                                {
                                    qfm.ValidFile = true;
                                    qfm.GameData = qfm.FileGameData;
                                    qfm.InternalModType = IMT.Name;
                                    qfm.InternalModTypeID = IMT.ID;
                                    qfm.InternalModTypeFileID = IMTF.ID;
                                }
                            }


                        }
                        else
                        {
                            getMatchValues(qfm, fileMatch.Groups, true);
                            //If there is a detected File Gamedata
                            if (qfm.FileGameData != "" && qfm.FileGameData != null)
                            {
                                if (ValidateGameData(qfm.FileGameData, gdc))
                                {
                                    qfm.ValidFile = true;
                                    qfm.InternalModType = IMT.Name;
                                    qfm.InternalModTypeID = IMT.ID;
                                    qfm.InternalModTypeFileID = IMTF.ID;
                                }
                            }
                        }
                    }
                }
            }

        }
        public static void getMatchValues(QuasarFileManager qfm, GroupCollection Groups, bool file)
        {
            try
            {
                
                if (file)
                {
                    qfm.FileGameData = null;
                    qfm.FileMatch = null;
                    qfm.FileParts = new List<string>();
                    qfm.FileFolders = new List<string>();
                }
                else
                {
                    qfm.FolderGameData = null;
                    qfm.FolderMatch = null;
                    qfm.FolderParts = new List<string>();
                    qfm.FolderFolders = new List<string>();
                }
                foreach (Group g in Groups)
                {
                    switch (g.Name)
                    {
                        default:
                            break;
                        case "Slot":
                            qfm.Slot = g.Value;
                            break;
                        case "GameData":
                            if (file)
                            {
                                qfm.FileGameData = g.Value;
                            }
                            else
                            {
                                qfm.FolderGameData = g.Value;
                            }

                            break;
                        case "AnyFile":
                            if (file)
                            {
                                qfm.AnyFile = g.Value;
                            }
                            else
                            {
                                qfm.AnyFile = g.Value;
                            }

                            break;
                        case "Folder":
                            if (file)
                            {
                                if (qfm.FileFolders.Count == 0)
                                {
                                    foreach (Capture c in g.Captures)
                                    {
                                        qfm.FileFolders.Add(c.Value);
                                    }
                                }


                            }
                            else
                            {
                                if (qfm.FolderFolders.Count == 0)
                                {
                                    foreach (Capture c in g.Captures)
                                    {
                                        qfm.FolderFolders.Add(c.Value);
                                    }
                                }
                            }
                            break;
                        case "Part":
                            if (file)
                            {
                                if (qfm.FileParts.Count == 0)
                                {
                                    foreach (Capture c in g.Captures)
                                    {
                                        qfm.FileParts.Add(c.Value);
                                    }
                                }


                            }
                            else
                            {
                                if (qfm.FolderParts.Count == 0)
                                {
                                    foreach (Capture c in g.Captures)
                                    {
                                        qfm.FolderParts.Add(c.Value);
                                    }
                                }
                            }
                            break;
                    }
                }
                if (qfm.Slot == null)
                {
                    qfm.Slot = "0";
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
        public static bool ValidateGameData(string Gamedatavalues, GameDataCategory gdc)
        {
            GameDataItem item = null;
            try
            {
                item = gdc.Items.Find(i => i.Attributes[0].Value == Gamedatavalues);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

            return item != null;
        }

        public static void Log(String input)
        {

        }

        public static string PrepareRegex(string input)
        {
            string output = input;

            //Replacing underscores for regex interpretation
            //output = output.Replace(@"_", "\\_");

            //Replacing the tags
            output = output.Replace(@"{Folder}", @"(?'Folder'[^\\\/]*)");
            output = output.Replace(@"{Part}", @"(?'Part'[a-zA-Z0-9]*)");
            output = output.Replace(@"{AnyFile}", @"(?'AnyFile'[a-zA-Z0-9\_]*)");
            output = output.Replace(@"{GameData}", @"(?'GameData'[a-zA-Z0-9\_]*)");

            //Replacing backslashes for regex interpretation
            output = output.Replace(@"\", @"\\");

            //Replacing points for regex interpretation
            output = output.Replace(@".", "\\.");

            //Replacing Slot digits
            output = output.Replace(@"{S000}", @"(?'Slot'\d{3})");
            output = output.Replace(@"{S00}", @"(?'Slot'\d{2})");
            output = output.Replace(@"{S0}", @"(?'Slot'\d{1})");

            return output;
        }

        public class QuasarFileManager
        {
            public string Path { get; set; }
            public bool ValidFile { get; set; }
            public string FileMatch { get; set; }
            public string FolderMatch { get; set; }

            public string OutputFileMatch { get; set; }
            public string OutputFolderMatch { get; set; }

            public List<string> FileParts { get; set; }
            public List<string> FileFolders { get; set; }

            public string FileGameData { get; set; }

            public List<string> FolderParts { get; set; }
            public List<string> FolderFolders { get; set; }

            public string FolderGameData { get; set; }

            public string GameData { get; set; }
            private string _Slot { get; set; }
            public string Slot 
            {
                get => _Slot; 
                set 
                {
                    _Slot = value;
                    SlotInt = int.Parse(value);
                } 
            }
            public int SlotInt { get; set; }

            public string AnyFile { get; set; }

            public string InternalModType { get; set; }
            public int InternalModTypeID { get; set; }
            public int InternalModTypeFileID { get; set; }
        }
    }
}
