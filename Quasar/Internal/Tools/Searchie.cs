using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Quasar.FileSystem;
using Quasar.Internal.Tools;
using Quasar.XMLResources;
using static Quasar.XMLResources.Library;

namespace Quasar.Quasar_Sys
{
    public class Searchie
    {
        //Launches Detection process for a specific Mod
        public static List<ContentMapping> AutoDetectinator(LibraryMod mod,List<InternalModType> types, Game _Game, List<GameData> gamedata)
        {
            if(mod != null)
            {
                List<ContentMapping> mappings = new List<ContentMapping>();
                ModFileManager modFileManager = new ModFileManager(mod, _Game);
                foreach (InternalModType type in types)
                {
                    List<ContentMapping> temp = EvaluatePossibility(type, modFileManager, mod, gamedata);
                    foreach(ContentMapping map in temp)
                    {
                        mappings.Add(map);
                    }
                }
                return mappings;
            }
            return null;
        }

        //Evaluates possibilities of instances of the selected InternalModType
        public static List<ContentMapping> EvaluatePossibility(InternalModType IMT, ModFileManager MFM, LibraryMod mod, List<GameData> gamedata)
        {
            //List of mapped contents
            List<ContentMapping> SearchResults = new List<ContentMapping>();

            ContentMapping PossibleMapping = new ContentMapping() { Files = new List<ContentMappingFile>() };
            IEnumerable<string> LibraryModFiles = Directory.EnumerateFiles(MFM.LibraryContentFolderPath, "*.*", SearchOption.AllDirectories);
            GameData SpecificGameData = gamedata.Find(g => g.GameID == mod.GameID);
            GameDataCategory SpecificCategory = SpecificGameData.Categories.Find(gdc => gdc.ID == IMT.Association);

            List<QuasarMatch> FullMatches = new List<QuasarMatch>();

            foreach (InternalModTypeFile IMTF in IMT.Files)
            {
                FullMatches.AddRange(EvaluateForType(IMTF, LibraryModFiles, MFM));
                SearchResults = ParseContentFromMatches(FullMatches, SearchResults, IMTF,IMT, MFM, SpecificCategory);
            }

            return SearchResults;
        }

        public static List<QuasarMatch> EvaluateForType(InternalModTypeFile IMTF, IEnumerable<string> ListOfFiles, ModFileManager MFM)
        {
            List<QuasarMatch> quasarMatches = new List<QuasarMatch>();
            //Setting up Regex
            Regex fileRegex = new Regex(PrepareRegex(IMTF.Path.Replace(@"/", @"\") + @"\" + IMTF.File));

            //Foreach file in the mod
            foreach (string filepath in ListOfFiles)
            {
                //Try to match with current file in the internal type
                Match matchoum = fileRegex.Match(filepath);
                if (matchoum.Success)
                {
                    //Getting match groups
                    GroupCollection Groups = matchoum.Groups;
                    if (Groups.Count != 0)
                    {
                        QuasarMatch QM = GetRegexMatchData(Groups, "", filepath.Substring(0, matchoum.Index));
                        QM.OutputPath = filepath.Replace(MFM.LibraryContentFolderPath, "");
                        quasarMatches.Add(QM);
                    }
                }
            }

            return quasarMatches;
        }

        public static QuasarMatch GetRegexMatchData(GroupCollection RegexGroups, string _ModName, string _OriginalParentFolder)
        {
            QuasarMatch MatchInfo = new QuasarMatch();

            //Getting full match Value
            MatchInfo.MatchValue = RegexGroups[0].Value;

            MatchInfo.OriginalParentFolder = _OriginalParentFolder;

            MatchInfo.SlotName = "Default";
            MatchInfo.SlotNumber = 0;

            MatchInfo.GameDataValue = "";

            foreach (Group g in RegexGroups)
            {
                if (g.Name.Length > 4)
                {
                    if (g.Name.Substring(0, 4).Equals("Slot"))
                    {
                        MatchInfo.SlotName = "Slot " + g.Value;
                        MatchInfo.SlotNumber = int.Parse(g.Value);
                    }
                    if (g.Name.Substring(0, 8).Equals("gamedata"))
                    {
                        if (g.Value.Contains(@"\"))
                        {
                            MatchInfo.GameDataValue = g.Value.Split('\\')[g.Value.Split('\\').Length - 1];
                        }
                        else
                        {
                            MatchInfo.GameDataValue = g.Value;
                        }
                    }
                }
            }
            MatchInfo.MatchGroup = _ModName + " - " + MatchInfo.SlotName;

            return MatchInfo;

        }

        public static List<ContentMapping> ParseContentFromMatches(List<QuasarMatch> _QuasarMatches, List<ContentMapping> _ContentMappings, InternalModTypeFile IMTF, InternalModType IMT, ModFileManager MFM, GameDataCategory GDC)
        {
            foreach(QuasarMatch QM in _QuasarMatches)
            {
                ContentMapping newMapping = _ContentMappings.Find(map => map.Name == QM.MatchGroup && map.InternalModType == IMT.ID && map.Folder == QM.OriginalParentFolder && map.ModID.ToString() == MFM.ModID);
                if (newMapping != null)
                {
                    newMapping.Files.Add(new ContentMappingFile() { Path = IMTF.Path, InternalModTypeFileID = IMTF.ID, SourcePath = QM.OutputPath });
                }
                else
                {
                    int gdiID = -1;
                    GameDataItem gdi = GDC.Items.Find(i => i.Attributes[0].Value == QM.GameDataValue);
                    if (gdi != null)
                    {
                        gdiID = gdi.ID;
                    }
                    newMapping = new ContentMapping() { ID = IDGenerator.getNewContentID(), Name = QM.MatchGroup, InternalModType = IMT.ID, Files = new List<ContentMappingFile>(), Folder = QM.OriginalParentFolder, ModID = Int32.Parse(MFM.ModID), SlotName = QM.SlotName, Slot = QM.SlotNumber, GameDataItemID = gdiID };
                    newMapping.Files.Add(new ContentMappingFile() { Path = IMTF.Path, InternalModTypeFileID = IMTF.ID, SourcePath = QM.OutputPath });
                    _ContentMappings.Add(newMapping);
                }
            }

            return _ContentMappings;
        }

        public static void Log(String input)
        {

        }

        public static string PrepareRegex(string input)
        {
            string output = input;

            //Replacing underscores for regex interpretation
            //output = output.Replace(@"_", "\\_");

            // Replacing the any tag
            output = output.Replace(@"*", @"([^\\]*)");

            //Replacing the folder tag
            output = output.Replace(@"{Folder}", @"(?'folder'[^\\]*)");
            output = output.Replace(@"{Part}", @"(?'part'[^\\]*)");

            //Replacing game data
            output = output.Replace(@"{Characters}", @"(?'gamedata_characters'[^\_\\]*)");
            output = output.Replace(@"{Stages}", @"(?'gamedata_stages'[A-Za-z0-9\_\-]*)");
            output = output.Replace(@"{Music}", @"(?'gamedata_music'[^\\\/]*)");

            //Replacing backslashes for regex interpretation
            output = output.Replace(@"\", @"\\");

            //Replacing points for regex interpretation
            output = output.Replace(@".", "\\.");


            //Replacing base digits
            output = output.Replace(@"{000}", @"(?'DigitTriple'\d{3})");
            output = output.Replace(@"{00}", @"(?'DigitDouble'\d{2})");
            output = output.Replace(@"{0}", @"(?'DigitSingle'\d{1})");

            //Replacing Slot digits
            output = output.Replace(@"{S000}", @"(?'SlotTripleDigit'\d{3})");
            output = output.Replace(@"{S00}", @"(?'SlotDoubleDigit'\d{2})");
            output = output.Replace(@"{S0}", @"(?'SlotSingleDigit'\d{1})");

            return output;
        }

        public class QuasarMatch
        {
            public string MatchGroup { get; set; }
            public string MatchValue { get; set; }
            public string OriginalParentFolder { get; set; }
            public bool fileonly { get; set; }
            public string SlotName { get; set; }
            public int SlotNumber { get; set; }
            public string GameDataValue { get; set; }

            public string OutputPath { get; set; }

        }
    }
}
