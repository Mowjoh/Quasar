﻿using FluentFTP.Rules;
using Quasar.Controls.Build.ViewModels;
using Quasar.Controls.Common.Models;
using Quasar.FileSystem;
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

        public abstract Task DeleteDifferences();
        public abstract Task StartTransfer();

        public abstract void GetDistantFileList();
        public abstract void CleanWorkspace();
        
    }

    public class SmashBuilder : Builder
    {
        #region Fields
        private BuildViewModel _ViewModel { get; set; }
        #endregion

        #region Properties

        /// <summary>
        /// List of all Library Mods
        /// </summary>
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
        int BuildMode { get; set; }
        int ModLoader { get; set; }
        List<Hash> Hashes { get; set; }

        string WorkspacePath { get; set; }

        List<FileReference> FilesToBuild { get; set; }
        List<FileReference> DistantFiles { get; set; }

        #endregion

        public SmashBuilder(FileWriter _Writer, int _BuildMode, int _ModLoader, BuildViewModel _ViewModel)
        {
            Writer = _Writer;
            BuildMode = _BuildMode;
            ModLoader = _ModLoader;
            ViewModel = _ViewModel;

        }

        public override async Task<bool> StartBuild()
        {
            if (Writer.VerifyOK())
            {
                ViewModel.Building = true;

                await CopyModLoader();

                //Base Operations
                await StartCheck();
                await GetLocalFileList();

                //File Operations
                await DeleteDifferences();
                await StartTransfer();
            }
            ViewModel.Log.Info("Transfer Finished");
            ViewModel.SetStep("Finished");
            ViewModel.SetSubStep("");
            ViewModel.SetProgression(100);
            ViewModel.SetSize("");
            ViewModel.SetSpeed("");
            return false;
        }

        public override async Task CopyModLoader()
        {
            Console.Write("");
        }
        public override async Task StartCheck()
        {
            //Base File List Setup
            FilesToBuild = new List<FileReference>();
            DistantFiles = new List<FileReference>();

            //Base Path Setup
            WorkspacePath = "";
            if (typeof(SDWriter).Equals(Writer.GetType()))
            {
                SDWriter wrote = (SDWriter)Writer;
                WorkspacePath = wrote.LetterPath;
            }

            ModLoader sml = ViewModel.ModLoaders.Single(ml => ml.ModLoaderID == ModLoader);
            WorkspacePath += sml.BasePath;


            if (BuildMode == (int)BuildModes.Clean)
            {
                CleanWorkspace();
                GetDistantFileList();
            }
            if (BuildMode == (int)BuildModes.Synchronize || BuildMode == (int)BuildModes.Overwrite)
            {
                GetDistantFileList();
            }
            
        }
        public override async Task GetLocalFileList()
        {
            ViewModel.SetStep("Listing Files");
            ViewModel.Log.Debug("Listing Local Files");
            GameData GD = ViewModel.GameDatas.Single(g => g.GameID == ViewModel.Games[1].ID);

            foreach (Association ass in ViewModel.ActiveWorkspace.Associations)
            {
                ListAssociationFiles(ass,GD);
            }
        }
        public override void GetDistantFileList()
        {
            try
            {
                ViewModel.SetStep("Listing Distant Files");
                ViewModel.SetProgressionStyle(true);

                string DistantWorkspacePath = WorkspacePath.Replace("{Workspace}", ViewModel.ActiveWorkspace.Name);
                string localHashFilePath = Properties.Settings.Default.DefaultDir + @"\Library\Downloads\Hashes.xml";

                if (Writer.CheckFileExists(DistantWorkspacePath + @"Hashes.xml"))
                {
                    Writer.GetFile(DistantWorkspacePath + @"Hashes.xml", localHashFilePath);
                    ViewModel.Log.Debug(String.Format("Distant Hash File Exists : {0}", DistantWorkspacePath + @"Hashes.xml"));
                }
                    

                DistantFiles = Writer.GetRemoteFiles(DistantWorkspacePath);

                if (File.Exists(localHashFilePath))
                {
                    Hashes = XML.GetHashes(localHashFilePath);
                }
                else
                {
                    ViewModel.Log.Debug("Hash List Created");
                    Hashes = new List<Hash>();
                }
                Writer.SetHashes(Hashes);
            }
            catch(Exception e)
            {
                ViewModel.Log.Error(e.Message);
            }
            
        }
        public override async Task DeleteDifferences()
        {
            try
            {
                ViewModel.SetStep("Deleting Differences");
                if (BuildMode == (int)BuildModes.Synchronize)
                {
                    foreach (FileReference reference in DistantFiles)
                    {
                        FileReference LocalReference = FilesToBuild.FirstOrDefault(r => "/" + r.OutputFilePath.Replace("\\", "/") == reference.OutputFilePath);
                        if (LocalReference == null)
                        {
                            if (reference.OutputFilePath.Split('/')[reference.OutputFilePath.Split('/').Length - 1] != "Hashes.xml")
                            {
                                Writer.DeleteFile(reference.OutputFilePath);
                                ViewModel.Log.Debug(String.Format("Deleted File : {0}", reference.OutputFilePath));
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ViewModel.Log.Error(e.Message);
            }
            
        }
        public override async Task StartTransfer()
        {
            ViewModel.SetStep("Transfering Files");
            ViewModel.SetProgressionStyle(false);
            int TotalFiles = FilesToBuild.Count;
            while (FilesToBuild.Count > 0)
            {
                try
                {
                    FileReference FRef = FilesToBuild[0];
                    List<FileReference> AssociatedReferences = FilesToBuild.Where(r => r.LibraryMod == FRef.LibraryMod).ToList();

                    ViewModel.SetSubStep("Copying files for " + FRef.LibraryMod.Name);
                    foreach (FileReference Ferb in AssociatedReferences)
                    {
                        SetProgression((TotalFiles - FilesToBuild.Count), TotalFiles);
                        ViewModel.Log.Debug(String.Format("Copying File {0} to {1}",Ferb.SourceFilePath,Ferb.OutputFilePath));
                        Writer.SendFile(Ferb.SourceFilePath, Ferb.OutputFilePath);
                        FilesToBuild.Remove(Ferb);
                    }
                    ViewModel.Log.Info(String.Format("Finished copying files for {0}", FRef.LibraryMod.Name));
                    ViewModel.BuildLog("Mod", String.Format("Finished copying files for {0}", FRef.LibraryMod.Name));
                }
                catch(Exception e)
                {
                    ViewModel.Log.Error(e.Message);
                }
                
            }
            SetProgression((TotalFiles - FilesToBuild.Count), TotalFiles);

            //Saving, sending Hashes file
            string DistantWorkspacePath = WorkspacePath.Replace("{Workspace}", ViewModel.ActiveWorkspace.Name);
            string localHashFilePath = Properties.Settings.Default.DefaultDir + @"\Library\Downloads\Hashes.xml";
            XML.SaveHashes(localHashFilePath, Writer.GetHashes());
            ViewModel.Log.Debug("Saved Hashes");
            Writer.SendFile(localHashFilePath, DistantWorkspacePath + @"Hashes.xml",true);
            ViewModel.Log.Debug("Sent Hashes");
            File.Delete(localHashFilePath);
            ViewModel.Log.Debug("Deleted Hashes");

        }
        public override void CleanWorkspace()
        {
            ViewModel.SetStep("Cleaning Workspace");
            ViewModel.SetProgressionStyle(true);
            Writer.DeleteFolder(WorkspacePath.Replace("{Workspace}", ViewModel.ActiveWorkspace.Name));
            ViewModel.Log.Debug("Workspace Cleaned");

        }

        public void ListAssociationFiles(Association ass, GameData GD)
        {
            //References
            ContentMapping cm = ViewModel.ContentMappings.Single(l => l.ID == ass.ContentMappingID);
            InternalModType imt = ViewModel.InternalModTypes.Single(t => t.ID == cm.InternalModType);
            GameDataCategory GDC = GD.Categories.Find(c => c.ID == imt.Association);
            GameDataItem GDI = GDC.Items.Find(i => i.ID == cm.GameDataItemID);

            //Mod
            LibraryMod lm = ViewModel.Mods.Single(m => m.ID == cm.ModID);
            ModFileManager mfm = new ModFileManager(lm, ViewModel.Games[1]);

            foreach (ContentMappingFile file in cm.Files)
            {
                //Looping through recognized files
                string source = file.SourcePath;

                InternalModTypeFile imtf = imt.Files.Find(f => f.ID == file.InternalModTypeFileID);

                BuilderFile bfi = imtf.Files[ModLoader];

                string[] output = BuilderActions.FormatOutput(bfi.File, bfi.Path, GDI.Attributes[0].Value, file, cm);
                string FinalDestination = WorkspacePath + output[0] + "/" + output[1];
                FinalDestination = FinalDestination.Replace(@"{Workspace}", ViewModel.ActiveWorkspace.Name);
                FinalDestination = FinalDestination.Replace(@"/", @"\");
                FilesToBuild.Add(new FileReference() { LibraryMod = lm, SourceFilePath = source, OutputFilePath = FinalDestination });

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
        public static string[] FormatOutput(string file, string path, string GameDataItem, ContentMappingFile cmf, ContentMapping cm)
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

            return new string[2] { OutputPath, OutputFile };
        }
    }

    public class FileReference
    {
        public LibraryMod LibraryMod { get; set; }
        public string SourceFilePath { get; set; }
        public string OutputFilePath { get; set; }
    }



}