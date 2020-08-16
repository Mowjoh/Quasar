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
        public static List<ContentMapping> EvaluatePossibility(InternalModType type, ModFileManager modFileManager, LibraryMod mod, List<GameData> gamedata)
        {
            //List of mapped contents
            List<ContentMapping> content = new List<ContentMapping>();

            ContentMapping possibleMapping = new ContentMapping() { Files = new List<ContentMappingFile>() };
            IEnumerable<string> files = Directory.EnumerateFiles(modFileManager.LibraryContentFolderPath, "*.*", SearchOption.AllDirectories);
            GameData gd = gamedata.Find(g => g.GameID == mod.GameID);
            GameDataCategory cat = gd.Categories.Find(gdc => gdc.ID == type.Association);
            foreach (InternalModTypeFile IMTFile in type.Files)
            {
                
                //Setting up Regex
                Regex fileRegex = new Regex(PrepareRegex(IMTFile.Path.Replace(@"/",@"\") + @"\" + IMTFile.File));

                //Foreach file in the mod
                foreach (string filepath in files)
                {
                    string outputPath = filepath.Replace(modFileManager.LibraryContentFolderPath, "");
                    //Try to match with current file in the internal type
                    Match matchoum = fileRegex.Match(filepath);
                    if (matchoum.Success)
                    {
                        //Getting match groups
                        GroupCollection Groups = matchoum.Groups;
                        if(Groups.Count != 0)
                        {
                            //Getting full match Value
                            string match = Groups[0].Value;
                            string previousPath = filepath.Substring(0, matchoum.Index);

                            string slot = "Default";
                            int SlotNumber = 0;

                            string gamedatavalue = "";
                            string gamedatatype = "";

                            foreach(Group g in Groups)
                            {
                                if(g.Name.Length > 4)
                                {
                                    if (g.Name.Substring(0, 4).Equals("Slot"))
                                    {
                                        slot = "Slot " + g.Value;
                                        SlotNumber = int.Parse(g.Value);
                                    }
                                    if (g.Name.Substring(0, 8).Equals("gamedata"))
                                    {
                                        if (g.Value.Contains(@"\"))
                                        {
                                            gamedatavalue = g.Value.Split('\\')[g.Value.Split('\\').Length-1];
                                        }
                                        else
                                        {
                                            gamedatavalue = g.Value;
                                        }
                                        
                                    }
                                }
                            }


                            string MatchGroup = mod.Name+ " - " + slot;

                            ContentMapping newMapping = content.Find(map => map.Name == MatchGroup && map.InternalModType == type.ID && map.Folder == previousPath && map.ModID.ToString() == modFileManager.ModID);
                            if (newMapping != null)
                            {
                                newMapping.Files.Add(new ContentMappingFile() { Path = IMTFile.Path, InternalModTypeFileID = IMTFile.ID, SourcePath = outputPath });
                            }
                            else
                            {
                                int gdiID = -1;
                                GameDataItem gdi = cat.Items.Find(i => i.Attributes[0].Value == gamedatavalue);
                                if(gdi != null)
                                {
                                    gdiID = gdi.ID;
                                }
                                newMapping = new ContentMapping() {ID=IDGenerator.getNewContentID(), Name = MatchGroup, InternalModType = type.ID, Files = new List<ContentMappingFile>(), Folder = previousPath, ModID = Int32.Parse(modFileManager.ModID), SlotName = slot, Slot = SlotNumber, GameDataItemID = gdiID };
                                newMapping.Files.Add(new ContentMappingFile() { Path = IMTFile.Path, InternalModTypeFileID = IMTFile.ID, SourcePath = outputPath });
                                content.Add(newMapping);
                            }
                        }
                    }
                }
            }

            return content;
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
    }
}
