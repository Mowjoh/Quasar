using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Quasar.FileSystem;
using Quasar.XMLResources;
using static Quasar.XMLResources.Library;

namespace Quasar.Quasar_Sys
{
    public class Searchie
    {
        //Launches Detection process for a specific Mod
        public static List<ContentMapping> AutoDetectinator(LibraryMod mod,List<InternalModType> types, Game _Game)
        {
            if(mod != null)
            {
                List<ContentMapping> mappings = new List<ContentMapping>();
                ModFileManager modFileManager = new ModFileManager(mod, _Game);
                foreach (InternalModType type in types)
                {
                    List<ContentMapping> temp = EvaluatePossibility(type, modFileManager);
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
        public static List<ContentMapping> EvaluatePossibility(InternalModType type, ModFileManager modFileManager)
        {
            //List of mapped contents
            List<ContentMapping> content = new List<ContentMapping>();

            ContentMapping possibleMapping = new ContentMapping() { Files = new List<ContentMappingFile>() };
            IEnumerable<string> files = Directory.EnumerateFiles(modFileManager.LibraryContentFolderPath, "*.*", SearchOption.AllDirectories);
            foreach (InternalModTypeFile IMTFile in type.Files)
            {
                //Setting up Regex
                Regex fileRegex = new Regex(PrepareRegex(IMTFile.Path.Replace(@"/",@"\") + @"\" + IMTFile.File));

                //Foreach file in the mod
                foreach (string filepath in files)
                {
                    string outputPath = filepath.Replace(modFileManager.LibraryContentFolderPath, "");
                    //Try to match with current file in the internal type
                    Match matcho = fileRegex.Match(filepath);
                    if (matcho.Success)
                    {
                        //Getting match groups
                        GroupCollection Groups = matcho.Groups;
                        if(Groups.Count != 0)
                        {
                            //Getting full match Value
                            string match = Groups[0].Value;
                            string previousPath = filepath.Substring(0, matcho.Index);
                            if(Groups.Count > 1)
                            {
                                
                                ContentMapping newMapping = content.Find(map => map.Name == Groups[Groups.Count-1].Value && map.InternalModType == type.ID && map.Folder == previousPath && map.ModID.ToString() == modFileManager.ModID);
                                if (newMapping != null)
                                {
                                    newMapping.Files.Add(new ContentMappingFile() { Path = IMTFile.Path, InternalModTypeFileID = IMTFile.ID, SourcePath = outputPath });
                                }
                                else
                                {
                                    newMapping = new ContentMapping() { Name = Groups[Groups.Count - 1].Value, InternalModType = type.ID, Files = new List<ContentMappingFile>(), Folder = previousPath,ModID = Int32.Parse(modFileManager.ModID) };
                                    newMapping.Files.Add(new ContentMappingFile() { Path = IMTFile.Path, InternalModTypeFileID = IMTFile.ID, SourcePath = outputPath });
                                    content.Add(newMapping);
                                }
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

            //Replacing the any tag
            output = output.Replace(@"*", @"([A-Za-z0-9\_\-]*)");
            //Replacing backslashes for regex interpretation
            output = output.Replace(@"\", @"\\");
            //Replacing double digits
            output = output.Replace(@"{00}", @"(?'DoubleDigit'\d{2})");
            output = output.Replace(@"{0}", @"(?'SingleDigit'\d{1})");
            //Replacing points for regex interpretation
            output = output.Replace(@".", "\\.");

            return output;
        }
    }
}
