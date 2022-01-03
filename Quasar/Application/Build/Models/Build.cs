using Quasar.Build.ViewModels;
using DataModels.User;
using DataModels.Common;
using DataModels.Resource;
using Quasar.Helpers.ModScanning;
using Quasar.Helpers.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Workshop.FileManagement;

namespace Quasar.Build.Models
{

    enum BuildModes { Clean, Synchronize, Overwrite }

    public abstract class Builder: ObservableObject
    {
        public abstract Task<bool> StartBuild();

        public abstract Task<bool> CheckModLoader(int ModLoader);
        public abstract Task<bool> SetupModLoader(int ModLoader, string WorkspaceName);
        public abstract Task<bool> InstallModLoader(int ModLoader, string WorkspaceName);
        
        public abstract Task<bool> StartCheck();
        public abstract Task<bool> GetLocalFileList();
        public abstract Task<bool> GetDistantFileList();

        public abstract Task<bool> ProcessTransferList();
        public abstract Task<bool> DeleteDifferences();
        public abstract Task<bool> CopyFiles();

        
        public abstract void CleanWorkspace();
        
    }

    public class SmashBuilder : Builder
    {

        private BuildViewModel _ViewModel { get; set; }
        public BuildViewModel ViewModel
        {
            get => _ViewModel;
            set
            {
                if (_ViewModel == value)
                    return;

                _ViewModel = value;
                OnPropertyChanged("ViewModel");
            }
        }
        FileWriter Writer { get; set; }
        bool CleanSelected { get; set; }
        bool OverwriteSelected { get; set; }
        int ModLoader { get; set; }
        
        string WorkspacePath { get; set; }

        ObservableCollection<ModFile> WorkspaceIndex { get; set; }
        ObservableCollection<ModFile> DistantFiles { get; set; }
        List<ModFile> WorkspaceFilesToCopy { get; set; }
        List<ModFile> WorkspaceFilesToDelete { get; set; }
        ObservableCollection<ModFile> DistantIndex { get; set; }
        public bool ModLoaderInstalled { get; set; }
        public bool ModLoaderSetup { get; set; }

        public SmashBuilder(FileWriter _Writer, int _ModLoader, bool _CleanSelected, bool _OverwriteSelected, BuildViewModel _ViewModel)
        {
            Writer = _Writer;
            ModLoader = _ModLoader;
            ViewModel = _ViewModel;

            CleanSelected = _CleanSelected;
            OverwriteSelected = _OverwriteSelected;

        }

        public override async Task<bool> StartBuild()
        {
            ViewModel.Building = true;
            ViewModel.QuasarLogger.Debug("Building set to true");

            try
            {
                //Base Operations
                if (!await StartCheck())
                    return false;
                ViewModel.QuasarLogger.Debug("Start Check Finished");

                if (!await GetLocalFileList())
                    return false;
                ViewModel.QuasarLogger.Debug("Got Local File List");

                if (!await GetDistantFileList())
                    return false;
                ViewModel.QuasarLogger.Debug("Got Distant File List");

                if (!await ProcessTransferList())
                    return false;
                ViewModel.QuasarLogger.Debug("Finished processing transfer list");


                //File Operations
                if (WorkspaceFilesToDelete.Count != 0)
                {
                    if (!await DeleteDifferences())
                        return false;
                    ViewModel.BuildLog("Info", "All Differences have been deleted");
                }
                else
                {
                    ViewModel.BuildLog("Info", "No files to delete");
                    ViewModel.QuasarLogger.Debug("No files to delete");
                }
                if (WorkspaceFilesToCopy.Count != 0)
                {
                    if (!await CopyFiles())
                        return false;
                    ViewModel.BuildLog("Info", "All Files have been copied");
                }
                else
                {
                    ViewModel.BuildLog("Info", "No files to copy");
                    ViewModel.QuasarLogger.Debug("No files to copy");
                }

                SaveAndSendIndex();
                return true;
            }
            catch(Exception e)
            {
                ViewModel.BuildLog("Error", "Global Build Error");
                ViewModel.QuasarLogger.Error("Global Build Error");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }
            
            
            
        }
        public override async Task<bool> CheckModLoader(int ModLoader)
        {
            try
            {
                ViewModel.QuasarLogger.Debug("Checking Mod Loader existence");
                if (ModLoader != 4)
                {
                    string tomlfile = "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis\\arcropolis.toml";
                    string arcropolisfile = "atmosphere\\contents\\01006A800016E000\\romfs\\skyline\\plugins\\libarcropolis.nro";
                    if (Writer.CheckFileExists(arcropolisfile))
                    {
                        ModLoaderInstalled = true;
                    }
                    if (Writer.CheckFileExists(tomlfile))
                    {
                        ModLoaderSetup = true;
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                ViewModel.BuildLog("Error", "Could not Check Mod Loader");
                ViewModel.QuasarLogger.Error("Mod Loader Check");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }

            
        }
        public override async Task<bool> InstallModLoader(int ModLoader, string WorkspaceName)
        {
            try
            {
                if (ModLoader != 4)
                {
                    ARCropolisHelper.SendTouchmARC(Writer, WorkspaceName);
                }
                ViewModel.BuildLog("Info", "Finished installing modloader");

                return true;
            }
            catch (Exception e)
            {
                ViewModel.BuildLog("Error", "Could not install ModLoader");
                ViewModel.QuasarLogger.Error("Mod Loader Install");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }

            
        }
        public override async Task<bool> SetupModLoader(int ModLoader, string WorkspaceName)
        {
            try
            {
                if (ModLoader != 4)
                {
                    File.Copy(Properties.Settings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis\\arcropolis.toml", Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml", true);

                    if (ModLoader == 1)
                    {
                        ARCropolisHelper.ModifyTouchmARCConfig(WorkspaceName);
                    }
                    else
                    {
                        ARCropolisHelper.ModifyTouchmARCConfig(WorkspaceName);
                    }

                    Writer.SendFile(Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml", "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml");

                    File.Delete(Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml");
                }
                ViewModel.BuildLog("Info", "Finished modloader setup");

                return true;
            }
            catch (Exception e)
            {
                ViewModel.BuildLog("Error", "Could not setup Mod Loader");
                ViewModel.QuasarLogger.Error("Mod Loader Setup");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }

            
        }
        public override async Task<bool> StartCheck()
        {
            try
            {
                //Base File List Setup
                WorkspaceIndex = new ObservableCollection<ModFile>();
                DistantFiles = new ObservableCollection<ModFile>();
                WorkspaceFilesToCopy = new List<ModFile>();
                WorkspaceFilesToDelete = new List<ModFile>();

                //Base Path Setup
                ModLoader sml = ViewModel.MUVM.ModLoaders.Single(ml => ml.ID == ModLoader);
                WorkspacePath = sml.WorkspacePath.Replace("{Workspace}", ViewModel.MUVM.ActiveWorkspace.Name);

                if (CleanSelected)
                {
                    CleanWorkspace();
                }
                return true;
            }
            catch (Exception e)
            {
                ViewModel.BuildLog("Error", "Could not execute Startup Check");
                ViewModel.QuasarLogger.Error("Startup Check Error");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }
            
        }
        public override async Task<bool> GetLocalFileList()
        {
            try
            {
                //UI Setup
                ViewModel.SetStep("Listing Local Files");
                ViewModel.QuasarLogger.Debug("Listing Local Files");
                WorkspaceIndex = new ObservableCollection<ModFile>();
                ObservableCollection<BasicInfoData> InfoData = new ObservableCollection<BasicInfoData>();

                foreach (Association ass in ViewModel.MUVM.ActiveWorkspace.Associations)
                {
                    ContentItem ci = ViewModel.MUVM.ContentItems.Single(c => c.Guid == ass.ContentItemGuid);
                    LibraryItem li = ViewModel.MUVM.Library.Single(l => l.Guid == ci.LibraryItemGuid);
                    QuasarModType qmt = ViewModel.MUVM.QuasarModTypes.Single(t => t.ID == ass.QuasarModTypeID);
                    GameElementFamily gef = ViewModel.MUVM.Games[0].GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID);

                    if (ModLoader == 2)
                    {
                        if (InfoData.Any(d => d.LibraryItem.Guid == li.Guid))
                        {
                            BasicInfoData data = InfoData.Single(d => d.LibraryItem.Guid == li.Guid);
                            data.Description += String.Format("Slot #{0} - {1} - {2}\r\n", (ci.SlotNumber + 1).ToString(), qmt.Name, ci.Name);
                        }
                        else
                        {
                            BasicInfoData data = new BasicInfoData()
                            {
                                LibraryItem = li,
                                Description = "Contains : \r\n"
                            };
                            data.Description += String.Format("Slot #{0} - {1} - {2}\r\n", (ci.SlotNumber + 1).ToString(), qmt.Name, ci.Name);

                            InfoData.Add(data);
                        }

                    }

                    foreach (ModFile mf in Scannerino.GetModFiles(qmt, gef, ci, ass.SlotNumber, ModLoader, li, ViewModel.MUVM.Games[0]))
                    {
                        mf.Hash = BuilderActions.GetHash(mf.SourceFilePath);
                        WorkspaceIndex.Add(mf);
                    }
                }

                foreach (BasicInfoData d in InfoData)
                {
                    ARCropolisHelper.CreateInfoFile(d.LibraryItem, d.Description);
                }

                return true;
            }
            catch (Exception e)
            {
                ViewModel.BuildLog("Error", "Could not list local files");
                ViewModel.QuasarLogger.Error("Local File Listing");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }

            
            

        }
        public override async Task<bool> GetDistantFileList()
        {
            try
            {
                //UI Setup
                ViewModel.QuasarLogger.Debug("Starting to list distant files");
                ViewModel.SetStep("Listing Distant Files, please wait as it might take a while");
                ViewModel.SetProgressionStyle(true);

                //Getting distant hashes
                GetDistantIndex();

                if (OverwriteSelected)
                {
                    getDistantFileList();
                }
                else
                {
                    DistantFiles = new ObservableCollection<ModFile>();
                }

                return true;
            }
            catch(Exception e)
            {
                ViewModel.BuildLog("Error", "Could not get Distant File List");
                ViewModel.QuasarLogger.Error("Distant File List Error");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }
        }
        public override async Task<bool> ProcessTransferList()
        {
            try
            {
                //List files for copy
                getCopyFileList();

                //List files for deletion
                getDeleteFileList();

                SetupWorkspaceFolders();

                return true;
            }
            catch (Exception e)
            {
                ViewModel.BuildLog("Error", "Could not process transfer list");
                ViewModel.QuasarLogger.Error("Transfer List Processing");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }

            
        }
        public override async Task<bool> DeleteDifferences()
        {
            try
            {
                ViewModel.SetStep("Deleting Differences");
                foreach (ModFile reference in WorkspaceFilesToDelete)
                {
                    ViewModel.QuasarLogger.Debug("Deleting " + reference.DestinationFilePath);
                    Writer.DeleteFile(WorkspacePath + reference.DestinationFilePath);
                }
                return true;

            }
            catch(Exception e)
            {
                ViewModel.BuildLog("Error", "Could not delete differences");
                ViewModel.QuasarLogger.Error("Diff Delete");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }
            
        }
        public override async Task<bool> CopyFiles()
        {
            try
            {
                ViewModel.SetStep("Transfering Files");
                ViewModel.SetProgressionStyle(false);
                int TotalFiles = WorkspaceFilesToCopy.Count;
                while (WorkspaceFilesToCopy.Count > 0)
                {
                    try
                    {
                        ModFile MF = WorkspaceFilesToCopy[0];
                        string description = "Contains :\r\n";
                        List<ModFile> AssociatedFiles = WorkspaceFilesToCopy.Where(r => r.LibraryItemGuid == MF.LibraryItemGuid).ToList();
                        LibraryItem Item = ViewModel.MUVM.Library.Single(li => li.Guid == MF.LibraryItemGuid);
                        GamebananaRootCategory RCat = ViewModel.MUVM.API.Games[0].RootCategories.SingleOrDefault(c => c.Guid == Item.GBItem?.RootCategoryGuid);
                        ViewModel.SetSubStep("Copying files for " + Item.Name);
                        foreach (ModFile Ferb in AssociatedFiles)
                        {
                            SetProgression((TotalFiles - WorkspaceFilesToCopy.Count), TotalFiles);
                            Writer.SendFile(Ferb.SourceFilePath, WorkspacePath + Ferb.DestinationFilePath);
                            WorkspaceFilesToCopy.Remove(Ferb);
                        }
                        //Sending ARCadia files
                        if (ModLoader == 2)
                        {
                            string ModConfigInputPath = String.Format(@"{0}\Library\Mods\{1}\info.toml", Properties.Settings.Default.DefaultDir, Item.Guid);
                            string ModConfigOutputPath = String.Format(@"{0}\{1}\info.toml", WorkspacePath, Item.Name.Replace(".", ""));
                            string ModScreenInputPath = String.Format(@"{0}\Library\Screenshots\{1}.webp", Properties.Settings.Default.DefaultDir, Item.Guid);
                            string ModDefaultScreenInputPath = String.Format(@"{0}\Resources\images\NoScreenshot.webp", Properties.Settings.Default.DefaultDir);
                            string ModScreenOutputPath = String.Format(@"{0}\{1}\preview.webp", WorkspacePath, Item.Name.Replace(".", ""));

                            Writer.SendFile(ModConfigInputPath, ModConfigOutputPath);

                            if (File.Exists(ModScreenInputPath))
                            {
                                Writer.SendFile(ModScreenInputPath, ModScreenOutputPath);
                            }
                            else
                            {
                                Writer.SendFile(ModDefaultScreenInputPath, ModScreenOutputPath);
                            }

                        }
                        ViewModel.QuasarLogger.Info(String.Format("Finished copying files for {0}", Item.Name));
                        ViewModel.BuildLog("Mod", String.Format("Finished copying files for {0}", Item.Name));
                    }
                    catch (Exception e)
                    {
                        ViewModel.BuildLog("Error", "Could not copy a file");
                        ViewModel.QuasarLogger.Error("Error");
                        ViewModel.QuasarLogger.Error(e.Message);
                        ViewModel.QuasarLogger.Error(e.StackTrace);
                        return false;
                    }

                }
                SetProgression((TotalFiles - WorkspaceIndex.Count), TotalFiles);

                return true;
            }
            catch (Exception e)
            {
                ViewModel.BuildLog("Error", "Could not copy all the files");
                ViewModel.QuasarLogger.Error("Error");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }

            
           

        }
        public override void CleanWorkspace()
        {
            ViewModel.SetStep("Cleaning Workspace");
            ViewModel.SetProgressionStyle(true);
            Writer.DeleteFolder(WorkspacePath);
            ViewModel.QuasarLogger.Debug("Workspace Cleaned");
            ViewModel.BuildLog("Info", "Workspace Cleaned");

        }

        public void GetDistantIndex()
        {
            string DistantWorkspacePath = WorkspacePath;
            string localHashFilePath = Properties.Settings.Default.DefaultDir + @"\Library\Downloads\Files.json";

            //Getting Workspace Hashes
            if (Writer.CheckFileExists(DistantWorkspacePath + @"Files.json"))
            {
                Writer.GetFile(DistantWorkspacePath + @"Files.json", localHashFilePath);
                ViewModel.QuasarLogger.Debug(String.Format("Distant Hash File Exists : {0}", DistantWorkspacePath + @"Files.json"));
            }

            //Loading distant Workspace Hashes if found
            if (File.Exists(localHashFilePath))
            {
                //TODO Edit this
                ViewModel.QuasarLogger.Debug("File Exists, Loading Hashes");
                //DistantIndex = UserDataManager.GetModFiles();
            }
            else
            {
                ViewModel.QuasarLogger.Debug("No Remote Hash File");
                ViewModel.QuasarLogger.Debug("Hash List Created");
                DistantIndex = new ObservableCollection<ModFile>();
            }
        }
        public void getDistantFileList()
        {
            //Getting Workspace Distant Files
            DistantFiles = Writer.GetRemoteFiles(WorkspacePath);
        }

        public void getCopyFileList()
        {
            if (DistantIndex.Count == 0)
            {
                //No distant index means first copy
                WorkspaceFilesToCopy = WorkspaceIndex.ToList();
            }
            else
            {
                //Copy over means find the differences
                foreach (ModFile reference in WorkspaceIndex)
                {
                    ModFile distantFile = DistantIndex.FirstOrDefault(mf => mf.DestinationFilePath == reference.DestinationFilePath);
                    //If there is no distant match
                    if (distantFile == null)
                    {
                        WorkspaceFilesToCopy.Add(reference);
                    }
                    else
                    {
                        //Different hash means overwrite
                        if (reference.Hash != distantFile.Hash)
                        {
                            WorkspaceFilesToCopy.Add(reference);
                        }
                    }

                }
            }
        }
        public void getDeleteFileList()
        {
            //No distant hashes means nothing to compare with
            if(DistantIndex.Count != 0)
            {
                foreach(ModFile mf in DistantIndex)
                {
                    if(!WorkspaceIndex.Any(fr => fr.DestinationFilePath == mf.DestinationFilePath))
                    {
                        WorkspaceFilesToDelete.Add(new ModFile() { DestinationFilePath = mf.DestinationFilePath });
                    }
                }
            }
        }

        public void SaveAndSendIndex()
        {
           //Saving, sending Hashes file
           string LocalIndexFilePath = Properties.Settings.Default.DefaultDir + @"\Library\Downloads\Files.json";

            foreach(ModFile mf in WorkspaceIndex)
            {
                mf.SourceFilePath = "";
            }
            //TODO Edit This
            UserDataManager.SaveModFiles(WorkspaceIndex, "");
            ViewModel.QuasarLogger.Debug("Saved Index");

           //Sending Hash Files
           Writer.SendFile(LocalIndexFilePath, WorkspacePath + @"Files.json");

           ViewModel.QuasarLogger.Debug("Sent Index");
           File.Delete(LocalIndexFilePath);
           ViewModel.QuasarLogger.Debug("Deleted Index");
        }
        public void SetProgression(int cnt, int tot)
        {
            ViewModel.SetProgression(((double)cnt / (double)tot) * 100);
            ViewModel.SetTotal(cnt.ToString(), tot.ToString());
        }

        public void SetupWorkspaceFolders()
        {
            if (WorkspacePath.Split('/')[WorkspacePath.Split('/').Count() - 2] == "arc")
            {
                if (!Writer.CheckFolderExists(WorkspacePath.Substring(0, WorkspacePath.Length - 4) + @"/umm/"))
                    Writer.CreateFolder(WorkspacePath.Substring(0, WorkspacePath.Length -4) + @"/umm/");
            }

            if (WorkspacePath.Split('/')[WorkspacePath.Split('/').Count() - 2] == "umm")
            {
                if (!Writer.CheckFolderExists(WorkspacePath.Substring(0, WorkspacePath.Length - 4) + @"/arc/"))
                    Writer.CreateFolder(WorkspacePath.Substring(0, WorkspacePath.Length - 4) + @"/arc/");
            }
            
        }
        
    }

    static class BuilderActions
    {
        public static string[] FormatOutput(string file, string path, string GameDataItem, ModFile cmf, ContentItem cm,Association ass, string DLC)
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
            /*
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

            OutputFile = SlotReplacinatorSingle.Replace(OutputFile, ass.SlotNumber.ToString("0"), 1);
            OutputFile = SlotReplacinatorDouble.Replace(OutputFile, ass.SlotNumber.ToString("00"), 1);
            OutputFile = SlotReplacinatorTriple.Replace(OutputFile, ass.SlotNumber.ToString("000"), 1);

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

            OutputPath = SlotReplacinatorSingle.Replace(OutputPath, ass.SlotNumber.ToString("0"), 1);
            OutputPath = SlotReplacinatorDouble.Replace(OutputPath, ass.SlotNumber.ToString("00"), 1);
            OutputPath = SlotReplacinatorTriple.Replace(OutputPath, ass.SlotNumber.ToString("000"), 1);

            OutputPath = OutputPath.Replace("{DLC}", DLC == "True" ? "_patch" : "");
            */
            return new string[2] { OutputPath, OutputFile };
        }

        public static string GetHash(string SourceFilePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(SourceFilePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                }
            }
        }
    }

    public class FileReference
    {
        public LibraryItem LibraryItem { get; set; }
        public string SourceFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public bool OutsideFile { get; set; }
    }

    public class BasicInfoData
    {
        public LibraryItem LibraryItem { get; set; }
        public string Description { get; set; }
    }

    

}
