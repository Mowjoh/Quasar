using FluentFTP.Rules;
using log4net;
using Quasar.Build.ViewModels;
using Quasar.Common.Models;
using Quasar.Data.V2;
using Quasar.FileSystem;
using Quasar.Helpers.Json;
using Quasar.Helpers.ModScanning;
using Quasar.Helpers.Tools;
using Quasar.Helpers.XML;
using Quasar.Internal.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Quasar.Build.Models
{

    enum BuildModes { Clean, Synchronize, Overwrite }

    public abstract class Builder: ObservableObject
    {
        public abstract Task<bool> StartBuild();

        public abstract Task CheckModLoader(int ModLoader);
        public abstract Task SetupModLoader(int ModLoader, string WorkspaceName);
        public abstract Task InstallModLoader(int ModLoader, string WorkspaceName);
        
        public abstract Task StartCheck();
        public abstract Task GetLocalFileList();
        public abstract Task GetDistantFileList();

        public abstract Task ProcessTransferList();
        public abstract Task DeleteDifferences();
        public abstract Task CopyFiles();

        
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

            //Base Operations
            await StartCheck();
            ViewModel.QuasarLogger.Debug("Start Check Finished");

            await GetLocalFileList();
            ViewModel.QuasarLogger.Debug("Got Local File List");

            await GetDistantFileList();
            ViewModel.QuasarLogger.Debug("Got Distant File List");

            await ProcessTransferList();
            ViewModel.QuasarLogger.Debug("Finished processing transfer list");


            //File Operations
            if (WorkspaceFilesToDelete.Count != 0)
            {
                await DeleteDifferences();
                ViewModel.BuildLog("Info", "All Differences have been deleted");
            }
            else
            {
                ViewModel.BuildLog("Info", "No files to delete");
                ViewModel.QuasarLogger.Debug("No files to delete");
            }
            if (WorkspaceFilesToCopy.Count != 0)
            {
                await CopyFiles();
                ViewModel.BuildLog("Info", "All Files have been copied");
            }
            else
            {
                ViewModel.BuildLog("Info", "No files to copy");
                ViewModel.QuasarLogger.Debug("No files to copy");
            }

            SaveAndSendIndex();
            
            return false;
        }
        public override async Task CheckModLoader(int ModLoader)
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
        }
        public override async Task InstallModLoader(int ModLoader, string WorkspaceName)
        {
            if(ModLoader != 4)
            {
                ARCropolisHelper.SendTouchmARC(Writer, WorkspaceName);
            }
            ViewModel.BuildLog("Info", "Finished installing modloader");
        }
        public override async Task SetupModLoader(int ModLoader, string WorkspaceName)
        {
            if (ModLoader != 4)
            {
                if(ModLoader == 1)
                {
                    ARCropolisHelper.ModifyTouchmARCConfig(WorkspaceName);
                }
                else
                {
                    ARCropolisHelper.ModifyTouchmARCConfig(WorkspaceName);
                }
                Writer.SendFile(Properties.Settings.Default.DefaultDir + "\\Library\\arcropolis.toml", "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml");
                
            }
            ViewModel.BuildLog("Info", "Finished modloader setup");
        }
        public override async Task StartCheck()
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
        }
        public override async Task GetLocalFileList()
        {
            //UI Setup
            ViewModel.SetStep("Listing Local Files");
            ViewModel.QuasarLogger.Debug("Listing Local Files");
            WorkspaceIndex = new ObservableCollection<ModFile>();

            foreach (Association ass in ViewModel.MUVM.ActiveWorkspace.Associations)
            {
                ContentItem ci = ViewModel.MUVM.ContentItems.Single(c => c.ID == ass.ContentItemID);
                LibraryItem li = ViewModel.MUVM.Library.Single(l => l.ID == ci.LibraryItemID);
                QuasarModType qmt = ViewModel.MUVM.QuasarModTypes.Single(t => t.ID == ass.QuasarModTypeID);
                GameElementFamily gef = ViewModel.MUVM.Games[0].GameElementFamilies.Single(f => f.ID == qmt.GameElementFamilyID);

                foreach (ModFile mf in Scannerino.GetModFiles(qmt, gef, ci, ass.SlotNumber, ModLoader, li, ViewModel.MUVM.Games[0]))
                {
                    mf.Hash = BuilderActions.GetHash(mf.SourceFilePath);
                    WorkspaceIndex.Add(mf);
                }
            }
            
        }
        public override async Task GetDistantFileList()
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
            }
            catch(Exception e)
            {
                ViewModel.QuasarLogger.Error(e.Message);
            }
        }
        public override async Task ProcessTransferList()
        {
            //List files for copy
            getCopyFileList();

            //List files for deletion
            getDeleteFileList();

            SetupWorkspaceFolders();
        }
        public override async Task DeleteDifferences()
        {
            try
            {
                ViewModel.SetStep("Deleting Differences");
                foreach (ModFile reference in WorkspaceFilesToDelete)
                {
                    ViewModel.QuasarLogger.Debug("Deleting " + reference.DestinationFilePath);
                    Writer.DeleteFile(WorkspacePath + reference.DestinationFilePath);
                }

            }
            catch(Exception e)
            {
                ViewModel.QuasarLogger.Error(e.Message);
            }
            
        }
        public override async Task CopyFiles()
        {
            ViewModel.SetStep("Transfering Files");
            ViewModel.SetProgressionStyle(false);
            int TotalFiles = WorkspaceFilesToCopy.Count;
            while (WorkspaceFilesToCopy.Count > 0)
            {
                try
                {
                    ModFile MF = WorkspaceFilesToCopy[0];
                    
                    List<ModFile> AssociatedFiles = WorkspaceFilesToCopy.Where(r => r.LibraryItemID == MF.LibraryItemID).ToList();
                    string ItemName = ViewModel.MUVM.Library.Single(li => li.ID == MF.LibraryItemID).Name;
                    ViewModel.SetSubStep("Copying files for " + ItemName);
                    foreach (ModFile Ferb in AssociatedFiles)
                    {
                        SetProgression((TotalFiles - WorkspaceFilesToCopy.Count), TotalFiles);
                        Writer.SendFile(Ferb.SourceFilePath, WorkspacePath + Ferb.DestinationFilePath);
                        WorkspaceFilesToCopy.Remove(Ferb);
                    }
                    ViewModel.QuasarLogger.Info(String.Format("Finished copying files for {0}", ItemName));
                    ViewModel.BuildLog("Mod", String.Format("Finished copying files for {0}", ItemName));
                }
                catch (Exception e)
                {
                    ViewModel.QuasarLogger.Error(e.Message);
                }
                
            }
            SetProgression((TotalFiles - WorkspaceIndex.Count), TotalFiles);
           

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
                ViewModel.QuasarLogger.Debug("File Exists, Loading Hashes");
                DistantIndex = JSonHelper.GetModFiles();
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
            JSonHelper.SaveModFiles(WorkspaceIndex);
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

    

}
