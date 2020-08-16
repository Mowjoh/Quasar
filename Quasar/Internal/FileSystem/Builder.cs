using FluentFTP;
using Quasar.FileSystem;
using Quasar.Quasar_Sys;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static async Task<int> SmashBuild(string _DriveFolder,int ModLoader, string ftp,int buildMode, List<LibraryMod> _Mods, List<ContentMapping> _ContentMappings, Workspace _Workspace, List<InternalModType> _InternalModTypes, Game _Game, List<GameData> _GameData, TextBlock _TextBlock, ProgressBar _ProgressBar)
        {
            SmashBuilder sb = new SmashBuilder(_DriveFolder, ModLoader, buildMode, ftp, _Mods,  _ContentMappings, _Workspace, _InternalModTypes, _Game, _GameData, _TextBlock, _ProgressBar);
            await sb.Build();
            return 0;
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


        int ModLoader;

        int BuildMode;
        

        TextBlock TextConsole;
        ProgressBar PeanutButter;

        string BuilderFTP;

        public SmashBuilder(string _DriveFolder, int _ModLoader, int buildMode, string ftp, List<LibraryMod> _Mods, List<ContentMapping> _ContentMappings, Workspace _Workspace,  List<InternalModType> _InternalModTypes, Game _Game, List<GameData> _GameData, TextBlock _TextBlock, ProgressBar _ProgressBar)
        {
            BuilderDriveFolder = _DriveFolder;
            BuilderMods = _Mods;
            BuilderContentMappings = _ContentMappings;
            BuilderWorkspace = _Workspace;
            BuilderInternalModTypes = _InternalModTypes;
            BuilderGame = _Game;
            BuilderGameData = _GameData.Find(gd => gd.GameID == BuilderGame.ID);

            TextConsole = _TextBlock;
            PeanutButter = _ProgressBar;

            ModLoader = _ModLoader;
            BuildMode = buildMode;
            BuilderFTP = ftp;
            
            UMMPath += @"Quasar - " + BuilderWorkspace.Name + @"/";
        }


        public async Task<int> Build()
        {
            await Task.Run(() => ExecuteBuildTask(TextConsole,PeanutButter));

            return 0;
        }

        public void ExecuteBuildTask(TextBlock TB, ProgressBar PB)
        {
            int ContentMappingCount = BuilderWorkspace.Associations.Count;
            int ContentMappingCurrentCount = 0;
            double ProgressValue = 0;

            bool FTP = false;
            FtpClient FTPClient = null;
            if(BuilderFTP != "")
            {
                FTP = true;
                string address = BuilderFTP.Split(':')[0];
                FTPClient = new FtpClient(BuilderFTP.Split(':')[0]);
                FTPClient.Port = Int32.Parse(BuilderFTP.Split(':')[1]);

                FTPClient.Connect();
            }
            
            
            try
            {
                if (BuildMode == 1)
                {
                    Write("Wiping mod folders", TB);
                    PB.Dispatcher.BeginInvoke((Action)(() => { PB.IsIndeterminate = true; }));
                    string basepath = FTP ? "" : BuilderDriveFolder;
                    if (ModLoader == 0 || ModLoader == 1)
                    {
                        BuilderDelete(basepath + UMMPath, FTP, FTPClient);
                    }
                    if (ModLoader == 0 || ModLoader == 2)
                    {
                        BuilderDelete(basepath + UMMPath, FTP, FTPClient);
                    }
                }

            }catch(Exception e)
            {
                Write("Something went wrong with the cleanup", TB);
                Write("Exception : " + e.Message, TB);
            }

            PB.Dispatcher.BeginInvoke((Action)(() => { PB.IsIndeterminate = false; }));
            //Looping through each association in the workspace
            foreach (Association ass in BuilderWorkspace.Associations)
            {
                ProgressValue = ((double)ContentMappingCurrentCount / (double)ContentMappingCount) * 100;
                PB.Dispatcher.BeginInvoke((Action)(() => { PB.Value = ProgressValue; }));

                try
                {
                    //Get associated Content Mapping
                    ContentMapping cm = BuilderContentMappings.Find(c => c.ID == ass.ContentMappingID);
                    InternalModType imt = BuilderInternalModTypes.Find(t => t.ID == cm.InternalModType);

                    bool succeded = true;
                    try
                    {
                        foreach (ContentMappingFile cmf in cm.Files)
                        {
                            //Looping through recognized files
                            string source = cmf.SourcePath;
                            string extension = source.Split('.')[source.Split('.').Length - 1];
                            string basedestination = "";

                            //Source Setup
                            LibraryMod lm = BuilderMods.Find(l => l.ID == cm.ModID);
                            ModFileManager mfm = new ModFileManager(lm, BuilderGame);
                            string FinalSource = mfm.LibraryContentFolderPath + source;

                            //Destination setup
                            basedestination = FormatPathForLoader(basedestination, extension);


                            InternalModTypeFile imtf = imt.Files.Find(f => f.ID == cmf.InternalModTypeFileID);


                            string FinalPath = imtf.Destination;

                            if (imtf.Destination != null)
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
                                            if (caps.Count > 1)
                                            {
                                                foreach (Capture cap in caps)
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
                                                        FinalPath = FinalPath.Replace("{Music}", item.Attributes[0].Value);
                                                    }
                                                }
                                                else
                                                {
                                                    FinalPath = FinalPath.Replace("{Characters}", item.Attributes[0].Value);
                                                    FinalPath = FinalPath.Replace("{Music}", item.Attributes[0].Value);
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
                            BuilderCopy(FinalSource, FinalDestination, FTP, FTPClient);
                        }
                    }
                    catch(Exception e)
                    {
                        succeded = false;
                        Write("Something went wrong trying to process Content Files", TB);
                        Write("Exception : " + e.Message, TB);
                        Write("Aborted : " + cm.Name + " of Type : " + imt.Name + " Into Slot " + (ass.Slot + 1), TB);
                    }


                    if (succeded)
                    {
                        Write("Finished with : " + cm.Name + " of Type : " + imt.Name + " Into Slot " + (ass.Slot + 1), TB);
                    }
                    else
                    {
                        Write("Aborted : " + cm.Name + " of Type : " + imt.Name + " Into Slot " + (ass.Slot + 1), TB);
                    }
                    
                    ContentMappingCurrentCount++;
                }
                catch(Exception e)
                {
                    Write("Something went wrong with finding the right Content", TB);
                    Write("Exception : "+e.Message, TB);
                }
            }
        }

        public void Write(string s, TextBlock TB)
        {
            TB.Dispatcher.BeginInvoke((Action)(() => { TB.Text += s + "\r\n"; }));
        }

        public string FormatPathForLoader(string _BasePath, string _FileExt)
        {
            string PreparedPath = _BasePath;
            if(ModLoader == 0)
            {
                if (DisabledARCropolisExtensions.Contains(_FileExt))
                {
                    PreparedPath += UMMPath;
                }
                else
                {
                    PreparedPath += ARCRopolisPath;
                }
            }
            if(ModLoader == 1)
            {
                PreparedPath += UMMPath;
            }
            if(ModLoader == 2)
            {
                PreparedPath += ARCRopolisPath;
            }
            

            return PreparedPath;
        }

        public void BuilderCopy(string source, string destination, Boolean FTP, FtpClient client = null)
        {
            if (FTP)
            {
                if(client != null)
                {
                    string filename = destination.Split('\\')[destination.Split('\\').Length-1];
                    string destinationWithoutFile = destination.Substring(0,destination.Length - filename.Length);
                    client.CreateDirectory(destinationWithoutFile);

                    client.UploadFile(source, destination);
                }
            }
            else
            {
                Folderino.CheckCopyFile(source, BuilderDriveFolder+destination);
            }
        }
        public void BuilderDelete(string destination, Boolean FTP, FtpClient client = null)
        {
            if (FTP)
            {
                client.DeleteDirectory(destination);
                client.CreateDirectory(destination);
            }
            else
            {
                Directory.Delete(destination, true);
                Directory.CreateDirectory(destination);
            }
        }
    }
}
