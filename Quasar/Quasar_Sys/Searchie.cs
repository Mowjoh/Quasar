using Quasar.File;
using Quasar.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Quasar.Library;

namespace Quasar.Quasar_Sys
{
    public class Searchie
    {
        public static List<ContentMapping> AutoDetectinator(Mod mod,List<InternalModType> types, List<ModType> modTypes)
        {
            if(mod != null)
            {
                List<ContentMapping> mappings = new List<ContentMapping>();
                ModFileManager modFileManager = new ModFileManager(mod, modTypes);
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

        public static List<ContentMapping> EvaluatePossibility(InternalModType type, ModFileManager modFileManager)
        {
            List<ContentMapping> content = new List<ContentMapping>();
            List<String> paths = new List<string>();
            foreach(InternalModTypeFile IMTFile in type.Files)
            {
                if (!paths.Contains(IMTFile.Path))
                {
                    paths.Add(IMTFile.Path);
                }
            }
            foreach(string Parent in paths)
            {

                ContentMapping possibleMapping = new ContentMapping() { Files = new List<ContentMappingFile>() };
                IEnumerable<string> files = Directory.EnumerateFiles(modFileManager.libraryContentPath, "*.*", SearchOption.AllDirectories);
                foreach (InternalModTypeFile IMTFile in type.Files)
                {
                    //Setting up Regex
                    string regexFromstring = IMTFile.Path+@"\"+IMTFile.File;
                    regexFromstring = "(" + regexFromstring + ")";
                    regexFromstring = regexFromstring.Replace("*", ")(.*)(");
                    regexFromstring = regexFromstring.Replace(@"\", @"\\");
                    regexFromstring = regexFromstring.Replace(@"{00}", @")(\d+)(");
                    Regex fileRegex = new Regex(regexFromstring);
                    foreach (string filepath in files)
                    {
                        Match matcho = fileRegex.Match(filepath);
                        if (matcho.Success)
                        {
                            foreach(Capture s in matcho.Captures)
                            {
                                Console.WriteLine(s.Value);
                                //Maybe do a detection function
                                //Input a string, definitions
                                //It adds the specific regex element to the list
                            }
                            
                        }
                    }
                }
                if (possibleMapping.Files.Count > 0)
                {
                    content.Add(possibleMapping);
                }

            }
            

            return content;
        }
    }
}
