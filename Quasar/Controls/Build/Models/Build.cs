using FluentFTP.Rules;
using log4net;
using Quasar.Controls.Build.ViewModels;
using Quasar.Controls.Common.Models;
using Quasar.FileSystem;
using Quasar.Internal.Tools;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Quasar.Controls.Build.Models
{

    enum BuildModes { Clean, Synchronize, Overwrite }

    public abstract class Builder: ObservableObject
    {
        public abstract Task<bool> StartBuild();

        public abstract Task CopyModLoader();
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
        bool ManualModsSelected { get; set; }
        int ModLoader { get; set; }
        
        string WorkspacePath { get; set; }

        List<FileReference> WorkspaceFiles { get; set; }
        List<FileReference> DistantFiles { get; set; }
        List<FileReference> FilesToCopy { get; set; }
        List<FileReference> FilesToDelete { get; set; }
        List<Hash> WorkspaceHashes { get; set; }
        List<Hash> DistantHashes { get; set; }

        public SmashBuilder(FileWriter _Writer, int _ModLoader, bool _CleanSelected, bool _ManualModsSelected, BuildViewModel _ViewModel)
        {
            Writer = _Writer;
            ModLoader = _ModLoader;
            ViewModel = _ViewModel;

            CleanSelected = _CleanSelected;
            ManualModsSelected = _ManualModsSelected;

        }

        public override async Task<bool> StartBuild()
        {
            if (Writer.VerifyOK())
            {
                ViewModel.Building = true;
                ViewModel.Log.Debug("Building set to true");

                //Base Operations
                await StartCheck();
                ViewModel.Log.Debug("Start Check Finished");

                await GetLocalFileList();
                ViewModel.Log.Debug("Got Local File List");

                await GetDistantFileList();
                ViewModel.Log.Debug("Got Distant File List");

                await ProcessTransferList();
                ViewModel.Log.Debug("Finished processing transfer list");

                //File Operations
                if(FilesToDelete.Count != 0)
                {
                    await DeleteDifferences();
                }
                else
                {
                    ViewModel.BuildLog("Info","No files to delete");
                    ViewModel.Log.Debug("No files to delete");
                }
                if(FilesToCopy.Count != 0)
                {
                    await CopyFiles();
                }
                else
                {
                    ViewModel.BuildLog("Info", "No files to copy");
                    ViewModel.Log.Debug("No files to copy");
                }

                SaveAndSendHashes();

                await CopyModLoader();
                ViewModel.Log.Debug("Copy ModLoader Finished");

            }

            ViewModel.Log.Info("Transfer Finished");
            ViewModel.SetStep("Finished");
            ViewModel.SetSubStep("");
            ViewModel.SetProgression(100);
            ViewModel.SetProgressionStyle(false);
            ViewModel.SetSize("");
            ViewModel.SetSpeed("");
            return false;
        }

        public override async Task CopyModLoader()
        {
            TouchmARC.CheckTouchmARC(Writer, ViewModel.ActiveWorkspace.Name);
        }
        public override async Task StartCheck()
        {
            //Base File List Setup
            WorkspaceFiles = new List<FileReference>();
            DistantFiles = new List<FileReference>();
            FilesToCopy = new List<FileReference>();
            FilesToDelete = new List<FileReference>();
            WorkspaceHashes = new List<Hash>();

            //Base Path Setup
            ModLoader sml = ViewModel.ModLoaders.Single(ml => ml.ModLoaderID == ModLoader);
            WorkspacePath = sml.BasePath;

            if (CleanSelected)
            {
                CleanWorkspace();
            }
        }
        public override async Task GetLocalFileList()
        {
            //UI Setup
            ViewModel.SetStep("Listing Files");
            ViewModel.Log.Debug("Listing Local Files");

            //Getting associated gamedata
            GameData GD = ViewModel.GameDatas.Single(g => g.GameID == ViewModel.Games[1].ID);

            //Getting files foreach association in the workspace
            foreach (Association ass in ViewModel.ActiveWorkspace.Associations)
            {
                ViewModel.Log.Debug("getting files for association CMID"+ass.ContentMappingID +" - GDIID "+ass.GameDataItemID + " - Slot "+ass.Slot);
                ListAssociationFiles(ass,GD);
            }
        }
        public override async Task GetDistantFileList()
        {
            try
            {
                //UI Setup
                ViewModel.Log.Debug("Starting to list distant files");
                ViewModel.SetStep("Listing Distant Files, please wait as it might take a while");
                ViewModel.SetProgressionStyle(true);

                //Getting distant hashes
                getDistantHashes();

                if (!ManualModsSelected)
                {
                    getDistantFileList();
                }
                else
                {
                    DistantFiles = new List<FileReference>();
                }
            }
            catch(Exception e)
            {
                ViewModel.Log.Error(e.Message);
            }
        }
        public override async Task ProcessTransferList()
        {
            //List files for copy
            getCopyFileList();

            //List files for deletion
            getDeleteFileList();

        }
        public override async Task DeleteDifferences()
        {
            try
            {
                ViewModel.SetStep("Deleting Differences");
                foreach (FileReference reference in FilesToDelete)
                {
                    ViewModel.Log.Debug("Deleting " + reference.OutputFilePath);
                    Writer.DeleteFile(reference.OutputFilePath);
                }
            }
            catch(Exception e)
            {
                ViewModel.Log.Error(e.Message);
            }
            
        }
        public override async Task CopyFiles()
        {
            ViewModel.SetStep("Transfering Files");
            ViewModel.SetProgressionStyle(false);
            int TotalFiles = FilesToCopy.Count;
            while (FilesToCopy.Count > 0)
            {
                try
                {
                    FileReference FRef = FilesToCopy[0];
                    List<FileReference> AssociatedReferences = FilesToCopy.Where(r => r.LibraryMod == FRef.LibraryMod).ToList();

                    ViewModel.SetSubStep("Copying files for " + FRef.LibraryMod.Name);
                    foreach (FileReference Ferb in AssociatedReferences)
                    {
                        SetProgression((TotalFiles - FilesToCopy.Count), TotalFiles);
                        Writer.SendFile(Ferb.SourceFilePath, Ferb.OutputFilePath);
                        FilesToCopy.Remove(Ferb);
                    }
                    ViewModel.Log.Info(String.Format("Finished copying files for {0}", FRef.LibraryMod.Name));
                    ViewModel.BuildLog("Mod", String.Format("Finished copying files for {0}", FRef.LibraryMod.Name));
                }
                catch(Exception e)
                {
                    ViewModel.Log.Error(e.Message);
                }
                
            }
            SetProgression((TotalFiles - WorkspaceFiles.Count), TotalFiles);
           

        }
        public override void CleanWorkspace()
        {
            ViewModel.SetStep("Cleaning Workspace");
            ViewModel.SetProgressionStyle(true);
            Writer.DeleteFolder(WorkspacePath.Replace("{Workspace}", ViewModel.ActiveWorkspace.Name));
            ViewModel.Log.Debug("Workspace Cleaned");

        }

        public void getDistantHashes()
        {
            string DistantWorkspacePath = WorkspacePath.Replace("{Workspace}", ViewModel.ActiveWorkspace.Name);
            string localHashFilePath = Properties.Settings.Default.DefaultDir + @"\Library\Downloads\Hashes.xml";

            //Getting Workspace Hashes
            if (Writer.CheckFileExists(DistantWorkspacePath + @"Hashes.xml"))
            {
                Writer.GetFile(DistantWorkspacePath + @"Hashes.xml", localHashFilePath);
                ViewModel.Log.Debug(String.Format("Distant Hash File Exists : {0}", DistantWorkspacePath + @"Hashes.xml"));
            }

            //Loading distant Workspace Hashes if found
            if (File.Exists(localHashFilePath))
            {
                ViewModel.Log.Debug("File Exists, Loading Hashes");
                DistantHashes = XML.GetHashes(localHashFilePath);
            }
            else
            {
                ViewModel.Log.Debug("No Remote Hash File");
                ViewModel.Log.Debug("Hash List Created");
                DistantHashes = new List<Hash>();
            }
        }
        public void getDistantFileList()
        {
            //Path Setup
            string DistantWorkspacePath = WorkspacePath.Replace("{Workspace}", ViewModel.ActiveWorkspace.Name);

            //Getting Workspace Distant Files
            DistantFiles = Writer.GetRemoteFiles(DistantWorkspacePath);
        }

        public void getCopyFileList()
        {
            if (DistantHashes.Count == 0)
            {
                //No distant hashes means first copy
                FilesToCopy = WorkspaceFiles;
            }
            else
            {
                //Copy over means find the differences
                foreach (FileReference reference in WorkspaceFiles)
                {
                    Hash distantHash = DistantHashes.FirstOrDefault(h => h.FilePath == reference.OutputFilePath);

                    //If there is no distant match
                    if (distantHash == null)
                    {
                        FilesToCopy.Add(reference);
                    }
                    else
                    {
                        //If there is a distant match, compare for copy
                        Hash localHash = WorkspaceHashes.FirstOrDefault(h => h.FilePath == reference.OutputFilePath);
                        //Different hash means overwrite
                        if (localHash.HashString != distantHash.HashString)
                        {
                            FilesToCopy.Add(reference);
                        }
                    }

                }
            }
        }
        public void getDeleteFileList()
        {
            //No distant hashes means nothing to compare with
            if(DistantHashes.Count != 0)
            {
                foreach(Hash h in DistantHashes)
                {
                    if(!WorkspaceFiles.Exists(fr => fr.OutputFilePath == h.FilePath))
                    {
                        FilesToDelete.Add(new FileReference() { OutputFilePath = h.FilePath });
                    }
                }
            }
        }

        public void SaveAndSendHashes()
        {
           //Saving, sending Hashes file
           string DistantWorkspacePath = WorkspacePath.Replace("{Workspace}", ViewModel.ActiveWorkspace.Name);
           string localHashFilePath = Properties.Settings.Default.DefaultDir + @"\Library\Downloads\Hashes.xml";

           XML.SaveHashes(localHashFilePath, WorkspaceHashes);
           ViewModel.Log.Debug("Saved Hashes");

           //Sending Hash Files
           Writer.SendFile(localHashFilePath, DistantWorkspacePath + @"Hashes.xml");

           ViewModel.Log.Debug("Sent Hashes");
           File.Delete(localHashFilePath);
           ViewModel.Log.Debug("Deleted Hashes");
        }

        public void ListAssociationFiles(Association ass, GameData GD)
        {
            try
            {
                //References
                ContentMapping cm = ViewModel.ContentMappings.Single(l => l.ID == ass.ContentMappingID);
                InternalModType imt = ViewModel.InternalModTypes.Single(t => t.ID == cm.InternalModType);
                GameDataCategory GDC = GD.Categories.Find(c => c.ID == imt.Association);
                GameDataItem GDI = GDC.Items.Find(i => i.ID == ass.GameDataItemID);

                //Mod
                LibraryMod lm = ViewModel.Mods.Single(m => m.ID == cm.ModID);
                ModFileManager mfm = new ModFileManager(lm, ViewModel.Games[1]);

                foreach (ContentMappingFile file in cm.Files)
                {
                    //Looping through recognized files
                    string source = file.SourcePath;

                    InternalModTypeFile imtf = imt.Files.Find(f => f.ID == file.InternalModTypeFileID);

                    BuilderFile bfi = imtf.Files.Single(f => f.BuilderID == ModLoader);

                    string[] output = BuilderActions.FormatOutput(bfi.File, bfi.Path, GDI.Attributes[0].Value, file, cm, ass, GDI.Attributes[1].Value);
                    string FinalDestination = WorkspacePath + output[0] + "/" + output[1];
                    if (imt.OutsideFolder)
                    {
                        FinalDestination = imt.OutsideFolderPath + output[0] + "/" + output[1];
                    }
                    FinalDestination = FinalDestination.Replace(@"{Workspace}", ViewModel.ActiveWorkspace.Name);
                    FinalDestination = FinalDestination.Replace(@"/", @"\");
                    WorkspaceHashes.Add(new Hash() { Category = imt.OutsideFolder ? "Outsider" : "Workspace", FilePath = FinalDestination, HashString = WriterOperations.GetHash(source) });
                    WorkspaceFiles.Add(new FileReference() { LibraryMod = lm, SourceFilePath = source, OutputFilePath = FinalDestination, OutsideFile = imt.OutsideFolder });
                }
            }
            catch(Exception e)
            {
                ViewModel.Log.Error(e.Message);
            }
            
        }
        public void SetProgression(int cnt, int tot)
        {
            ViewModel.SetProgression(((double)cnt / (double)tot) * 100);
            ViewModel.SetTotal(cnt.ToString(), tot.ToString());
        }

    }

    static class BuilderActions
    {
        public static string[] FormatOutput(string file, string path, string GameDataItem, ContentMappingFile cmf, ContentMapping cm,Association ass, string DLC)
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

            OutputFile = SlotReplacinatorSingle.Replace(OutputFile, ass.Slot.ToString("0"), 1);
            OutputFile = SlotReplacinatorDouble.Replace(OutputFile, ass.Slot.ToString("00"), 1);
            OutputFile = SlotReplacinatorTriple.Replace(OutputFile, ass.Slot.ToString("000"), 1);

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

            OutputPath = SlotReplacinatorSingle.Replace(OutputPath, ass.Slot.ToString("0"), 1);
            OutputPath = SlotReplacinatorDouble.Replace(OutputPath, ass.Slot.ToString("00"), 1);
            OutputPath = SlotReplacinatorTriple.Replace(OutputPath, ass.Slot.ToString("000"), 1);

            OutputPath = OutputPath.Replace("{DLC}", DLC == "True" ? "_patch" : "");

            return new string[2] { OutputPath, OutputFile };
        }
    }

    public class FileReference
    {
        public LibraryMod LibraryMod { get; set; }
        public string SourceFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public bool OutsideFile { get; set; }
    }



}
