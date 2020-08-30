using FluentFTP;
using Quasar.FileSystem;
using Quasar.Internal.Tools;
using Quasar.Quasar_Sys;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shell;
using static Quasar.XMLResources.AssociationXML;
using static Quasar.XMLResources.Library;

namespace Quasar.Internal.FileSystem
{
    public class Builder
    {
        public static async Task<int> SmashBuild(string _DriveFolder,int ModLoader, string ftp, NetworkCredential NC, int buildMode, List<LibraryMod> _Mods, List<ContentMapping> _ContentMappings, Workspace _Workspace, List<InternalModType> _InternalModTypes, Game _Game, List<GameData> _GameData, TextBlock _TextBlock, ProgressBar _ProgressBar, GameBuilder GB, TaskbarItemInfo TB)
        {
            SmashBuilder sb = new SmashBuilder(_DriveFolder, ModLoader, buildMode, ftp, NC,  _Mods,  _ContentMappings, _Workspace, _InternalModTypes, _Game, _GameData, _TextBlock, _ProgressBar,GB, TB);
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

        GameBuilder GB;

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
        NetworkCredential Creds;

        TaskbarItemInfo TBII;

        public SmashBuilder(string _DriveFolder, int _ModLoader, int buildMode, string ftp,NetworkCredential NC, List<LibraryMod> _Mods, List<ContentMapping> _ContentMappings, Workspace _Workspace,  List<InternalModType> _InternalModTypes, Game _Game, List<GameData> _GameData, TextBlock _TextBlock, ProgressBar _ProgressBar, GameBuilder _GB, TaskbarItemInfo _TBII)
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
            Creds = NC;
            GB = _GB;
            TBII = _TBII;
            
            UMMPath += @"Quasar - " + BuilderWorkspace.Name + @"/";
        }


        public async Task<int> Build()
        {
            await Task.Run(() => ExecuteBuildTask(TextConsole,PeanutButter, TBII));

            return 0;
        }

        public void ExecuteBuildTask(TextBlock TB, ProgressBar PB, TaskbarItemInfo TBII)
        {
            int ContentMappingCount = BuilderWorkspace.Associations.Count;
            int ContentMappingCurrentCount = 0;
            double ProgressValue = 0;

            bool FTP = false;
            FtpClient FTPClient = null;
            bool ftpOK = false;
            if(BuilderFTP != "")
            {
                Write("Trying to connect to the Switch, Please Wait...", TB);
                FTP = true;
                string address = BuilderFTP.Split(':')[0];
                FTPClient = new FtpClient(BuilderFTP.Split(':')[0]);
                FTPClient.Port = Int32.Parse(BuilderFTP.Split(':')[1]);
                if(Creds != null)
                {
                    FTPClient.Credentials = Creds;
                }
                try
                {
                    FTPClient.Connect();
                    ftpOK = true;
                    Write("FTP OK", TB);
                }
                catch(Exception e)
                {
                    Write("FTP Error : " + e.Message, TB);
                }
                
            }
            
            if(!FTP || ftpOK)
            {
                string basepath = "";
                try
                {
                    
                    if (BuildMode == 1)
                    {
                        Write("Wiping mod folders", TB);
                        basepath = FTP ? GB.BasePath.Replace("{Workspace}", BuilderWorkspace.Name) : BuilderDriveFolder + GB.BasePath.Replace("{Workspace}", BuilderWorkspace.Name);
                        if (ModLoader == 2)
                        {
                            string based = FTP ? "" : BuilderDriveFolder;
                            BuilderDelete(based + @"UltimateModManager/mods/"+ BuilderWorkspace.Name+"/", FTP, FTPClient);
                            BuilderDelete(based + @"atmosphere/contents/01006A800016E000/romfs/arc/" + BuilderWorkspace.Name + "/", FTP, FTPClient);
                        }
                        else
                        {
                            BuilderDelete(basepath, FTP, FTPClient);
                        }
                    }

                }
                catch (Exception e)
                {
                    Write("Something went wrong with the cleanup", TB);
                    Write("Exception : " + e.Message, TB);
                }


                if(ModLoader == 1 ||ModLoader == 2)
                {
                    if (FTP)
                    {
                        bool distant = TouchmARC.GetDistantConfig(FTPClient);
                        if (!distant)
                        {
                            TouchmARC.sendRemote(FTPClient);
                            TouchmARC.GetLocalConfig();
                        }
                        else
                        {
                            if (TouchmARC.LocalNewer())
                            {
                                TouchmARC.sendRemote(FTPClient);
                            }
                        }
                    }
                    else
                    {
                        bool local = TouchmARC.GetSDConfig(BuilderDriveFolder);
                        if (!local)
                        {
                            TouchmARC.sendLocal(BuilderDriveFolder);
                            TouchmARC.GetLocalConfig();
                        }
                        else
                        {
                            if (TouchmARC.LocalNewer())
                            {
                                TouchmARC.sendLocal(BuilderDriveFolder);
                            }
                        }
                    }

                    TouchmARC.ModifyTouchmARCConfig(BuilderWorkspace.Name + "/arc", BuilderWorkspace.Name + "/stream");

                    if (FTP)
                    {
                        TouchmARC.SendDistantConfig(FTPClient);
                    }
                    else
                    {
                        TouchmARC.SendSDConfig(BuilderDriveFolder);
                    }
                }

                PB.Dispatcher.BeginInvoke((Action)(() => { PB.IsIndeterminate = false; }));
                TBII.Dispatcher.BeginInvoke((Action)(() => { TBII.ProgressState = TaskbarItemProgressState.Normal; }));
                //Looping through each association in the workspace
                foreach (Association ass in BuilderWorkspace.Associations)
                {
                    ProgressValue = ((double)ContentMappingCurrentCount / (double)ContentMappingCount) * 100;
                    double WinProgressValue = ((double)ContentMappingCurrentCount / (double)ContentMappingCount);
                    PB.Dispatcher.BeginInvoke((Action)(() => { PB.Value = ProgressValue; }));
                    TBII.Dispatcher.BeginInvoke((Action)(() => { TBII.ProgressValue = WinProgressValue; }));

                    try
                    {
                        //Get associated Content Mapping
                        ContentMapping cm = BuilderContentMappings.Find(c => c.ID == ass.ContentMappingID);
                        InternalModType imt = BuilderInternalModTypes.Find(t => t.ID == cm.InternalModType);
                        GameDataCategory GDC = BuilderGameData.Categories.Find(c => c.ID == imt.Association);
                        GameDataItem GDI = GDC.Items.Find(i => i.ID == cm.GameDataItemID);

                        bool succeded = true;
                        try
                        {
                            foreach (ContentMappingFile cmf in cm.Files)
                            {
                                //Looping through recognized files
                                string source = cmf.SourcePath;
                                string basedestination = basepath;

                                InternalModTypeFile imtf = imt.Files.Find(f => f.ID == cmf.InternalModTypeFileID);

                                BuilderFile bfi = imtf.Files[ModLoader];
                                BuilderFolder bfo = imtf.Destinations[ModLoader];

                                string[] output = FormatOutput(bfi.Path, bfo.Path, GDI.Attributes[0].Value, cmf, cm);
                                string FinalDestination = basedestination + output[0] + "/" + output[1];
                                FinalDestination = FinalDestination.Replace(@"{Workspace}", BuilderWorkspace.Name);
                                FinalDestination = FinalDestination.Replace(@"/", @"\");
                                BuilderCopy(source, FinalDestination, FTP, FTPClient);
                            }
                        }
                        catch (Exception e)
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
                    catch (Exception e)
                    {
                        Write("Something went wrong with finding the right Content", TB);
                        Write("Exception : " + e.Message, TB);
                    }
                }
                
            }
            else
            {
                Write("Cannot Build", TB);
            }
            
        }

        public string[] FormatOutput(string file, string path,string GameDataItem, ContentMappingFile cmf, ContentMapping cm)
        {
            string OutputFile = file;
            string OutputPath = path;
            Regex FolderReplacinator = new Regex("{Folder}");
            Regex PartReplacinator = new Regex("{Part}");
            Regex GameDataReplacinator = new Regex("{GameData}");
            Regex SlotReplacinatorSingle = new Regex("{S0}");
            Regex SlotReplacinatorDouble = new Regex("{S00}");
            Regex SlotReplacinatorTriple = new Regex("{S000}");
            Regex AnyReplacinator = new Regex("{AnyFile}");

            //Processing File Output
            if (cmf.FileParts != null)
            {
                foreach (string s in cmf.FileParts)
                {
                    OutputFile = PartReplacinator.Replace(OutputFile, s, 1);
                }
            }
            if (cmf.FileFolders != null)
            {
                foreach (string s in cmf.FileFolders)
                {
                    OutputFile = FolderReplacinator.Replace(OutputFile, s, 1);
                }
            }

            OutputFile = GameDataReplacinator.Replace(OutputFile, GameDataItem, 1);

            OutputFile = SlotReplacinatorSingle.Replace(OutputFile, cm.Slot.ToString("0"), 1);
            OutputFile = SlotReplacinatorDouble.Replace(OutputFile, cm.Slot.ToString("00"), 1);
            OutputFile = SlotReplacinatorTriple.Replace(OutputFile, cm.Slot.ToString("000"), 1);

            if (cmf.AnyFile != null)
            {
                OutputFile = AnyReplacinator.Replace(OutputFile, cmf.AnyFile, 1);
            }

            //Processing Folder Output
            if (cmf.FolderParts != null)
            {
                foreach (string s in cmf.FolderParts)
                {
                    OutputPath = PartReplacinator.Replace(OutputPath, s, 1);
                }
            }

            if (cmf.FolderFolders != null)
            {
                foreach (string s in cmf.FolderFolders)
                {
                    OutputPath = FolderReplacinator.Replace(OutputPath, s, 1);
                }
            }

            OutputPath = GameDataReplacinator.Replace(OutputPath, GameDataItem, 1);

            OutputPath = SlotReplacinatorSingle.Replace(OutputPath, cm.Slot.ToString("0"), 1);
            OutputPath = SlotReplacinatorDouble.Replace(OutputPath, cm.Slot.ToString("00"), 1);
            OutputPath = SlotReplacinatorTriple.Replace(OutputPath, cm.Slot.ToString("000"), 1);

            return new string[2] { OutputPath, OutputFile};
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
                Folderino.CheckCopyFile(source, destination);
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
