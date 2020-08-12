using Quasar.FileSystem;
using Quasar.Quasar_Sys;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Quasar.XMLResources.AssociationXML;
using static Quasar.XMLResources.Library;

namespace Quasar.Internal.FileSystem
{
    public class Builder
    {
        public static void Build(string _DriveFolder, List<LibraryMod> _Mods, List<ContentMapping> _ContentMappings, Workspace _Workspace, List<InternalModType> _InternalModTypes, Game _Game, List<GameData> _GameData, TextBlock _TextBlock)
        {
            SmashBuilder sb = new SmashBuilder(_DriveFolder, _Mods, _ContentMappings, _Workspace, _InternalModTypes, _Game, _GameData, _TextBlock);
            sb.Build();
        }
    }

    public class SmashBuilder
    {
        //Base variables
        string BuilderDriveFolder;
        List<LibraryMod> BuilderMods;
        List<ContentMapping> BuilderContentMappings;
        Workspace BuilderWorkspace;
        List<InternalModType> BuilderInternalModTypes;
        Game BuilderGame;
        GameData BuilderGameData;


        //Mod-loader Specifics
        string ARCRopolisBasePath = @"atmosphere/contents/01006A800016E000/romfs/skyline/plugins/";
        string ARCRopolisPath = @"atmosphere/contents/01006A800016E000/romfs/arc/";

        string UMMBasePath = @"switch/Ultimate Mod Manager/";
        string UMMPath = @"UltimateModManager/mods/";

        string[] DisabledARCropolisExtensions = new string[] { "nutexb", "eff" };

        
       
        
        

        TextBlock TextConsole;

        public SmashBuilder(string _DriveFolder,List<LibraryMod> _Mods, List<ContentMapping> _ContentMappings, Workspace _Workspace,  List<InternalModType> _InternalModTypes, Game _Game, List<GameData> _GameData, TextBlock _TextBlock)
        {
            BuilderDriveFolder = _DriveFolder;
            BuilderMods = _Mods;
            BuilderContentMappings = _ContentMappings;
            BuilderWorkspace = _Workspace;
            BuilderInternalModTypes = _InternalModTypes;
            BuilderGame = _Game;
            BuilderGameData = _GameData.Find(gd => gd.GameID == BuilderGame.ID);

            TextConsole = _TextBlock;
            
            UMMPath += @"Quasar - " + BuilderWorkspace.Name + @"/";
        }


        public void Build()
        {
            //Looping through each association in the workspace
            foreach(Association ass in BuilderWorkspace.Associations)
            {
                Write("Content Mapping ID : "+ass.ContentMappingID + " for Slot : "+ass.Slot);
                //Get associated Content Mapping
                ContentMapping cm = BuilderContentMappings.Find(c => c.ID == ass.ContentMappingID);

                foreach (ContentMappingFile cmf in cm.Files)
                {
                    //Looping through recognized files
                    string source = cmf.SourcePath;
                    string extension = source.Split('.')[source.Split('.').Length - 1];
                    string basedestination = BuilderDriveFolder;
                    Write("Internal Mod Type File ID is `" + cmf.InternalModTypeFileID + "`");

                    //Source Setup
                    LibraryMod lm = BuilderMods.Find(l => l.ID == cm.ModID);
                    ModFileManager mfm = new ModFileManager(lm, BuilderGame);
                    string FinalSource = mfm.LibraryContentFolderPath + source;
                    Write("Source is `" + FinalSource + "`");

                    //Destination setup
                    basedestination = FormatPathForLoader(basedestination, extension);
                    Write("Base Destination is `" + basedestination + "`");

                    InternalModType imt = BuilderInternalModTypes.Find(t => t.ID == cm.InternalModType);
                    InternalModTypeFile imtf = imt.Files.Find(f => f.ID == cmf.InternalModTypeFileID);

                    
                    string FinalPath = imtf.Destination;

                    if(imtf.Destination != null)
                    {
                        //Setting up regex to prepare final path
                        string RegexPattern = Searchie.PrepareRegex(FinalPath.Replace(@"/", @"\"));
                        Regex FileRegex = new Regex(RegexPattern);
                        Regex Replacinator = new Regex("{Folder}");

                        //Matching with replaceable values
                        Match matchoum = FileRegex.Match(FinalSource);
                        if (matchoum.Groups.Count > 0)
                        {
                            foreach (Group group in matchoum.Groups)
                            {
                                if (group.Name == "folder")
                                {
                                    CaptureCollection caps = group.Captures;
                                    if(caps.Count > 1)
                                    {
                                        foreach(Capture cap in caps)
                                        {
                                            FinalPath = Replacinator.Replace(FinalPath, cap.Value, 1);
                                        }
                                    }
                                    else
                                    {
                                        FinalPath = Replacinator.Replace(FinalPath, group.Value, 1);
                                    }


                                }
                                if (group.Name.Length > 8)
                                {
                                    if (group.Name.Substring(0, 8) == "gamedata")
                                    {
                                        GameDataCategory gdc = BuilderGameData.Categories.Find(c => c.ID == imt.Association);
                                        GameDataItem item = gdc.Items.Find(i => i.ID == cm.GameDataItemID);
                                        CaptureCollection caps = group.Captures;

                                        if (caps.Count > 1)
                                        {
                                            foreach (Capture cap in caps)
                                            {
                                                FinalPath = FinalPath.Replace("{Characters}", item.Attributes[0].Value);
                                            }
                                        }
                                        else
                                        {
                                            FinalPath = FinalPath.Replace("{Characters}", item.Attributes[0].Value);
                                        }

                                        
                                    }
                                }
                                if (group.Name.Length > 4)
                                {
                                    if (group.Name.Substring(0, 4) == "Slot")
                                    {
                                        int desiredSlot = ass.Slot;
                                        string newvalue = "";
                                        switch (group.Name)
                                        {

                                            case "SlotSingleDigit":
                                                newvalue = desiredSlot.ToString("0");
                                                FinalPath = FinalPath.Replace("{S0}", newvalue);
                                                break;
                                            case "SlotDoubleDigit":
                                                newvalue = desiredSlot.ToString("00");
                                                FinalPath = FinalPath.Replace("{S00}", newvalue);
                                                break;
                                            case "SlotTripleDigit":
                                                newvalue = desiredSlot.ToString("000");
                                                FinalPath = FinalPath.Replace("{S000}", newvalue);
                                                break;
                                        }
                                        
                                    }
                                    if (group.Name.Substring(0, 5) == "Digit")
                                    {
                                        int desiredSlot = ass.Slot;
                                        string newvalue = "";
                                        switch (group.Name)
                                        {

                                            case "DigitSingle":
                                                newvalue = desiredSlot.ToString("0");
                                                FinalPath = FinalPath.Replace("{0}", newvalue);
                                                break;
                                            case "DigitDouble":
                                                newvalue = desiredSlot.ToString("00");
                                                FinalPath = FinalPath.Replace("{00}", newvalue);
                                                break;
                                            case "DigitTriple":
                                                newvalue = desiredSlot.ToString("000");
                                                FinalPath = FinalPath.Replace("{000}", newvalue);
                                                break;
                                        }

                                    }
                                }

                            }
                        }
                    }

                   
                    string FinalDestination = basedestination + FinalPath;
                    FinalDestination = FinalDestination.Replace(@"/", @"\");
                    Write("Destination is `" + FinalDestination + "`");

                    Write("Attempt to copy file");
                    Folderino.CheckCopyFile(@FinalSource, @FinalDestination);
                    Write("File Copied");


                }
                Write("");
            }
        }

        public void Write(string s)
        {
            TextConsole.Text += s + "\r\n";
        }

        public string FormatPathForLoader(string _BasePath, string _FileExt)
        {
            string PreparedPath = _BasePath;

            if (DisabledARCropolisExtensions.Contains(_FileExt))
            {
                PreparedPath += UMMPath;
            }
            else
            {
                PreparedPath += ARCRopolisPath;
            }

            return PreparedPath;
        }
    }
}
