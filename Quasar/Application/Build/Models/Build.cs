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
using Quasar.Controls.ModManagement.ViewModels;
using Workshop.FileManagement;
using Workshop.Builder;

namespace Quasar.Build.Models
{

    public class SmashBuilder
    {

        private LibraryViewModel _ViewModel { get; set; }
        public LibraryViewModel ViewModel
        {
            get => _ViewModel;
            set
            {
                if (_ViewModel == value)
                    return;

                _ViewModel = value;
            }
        }
        FileWriter Writer { get; set; }
        bool CleanSelected { get; set; }
        bool OverwriteSelected { get; set; }
        int ModLoader { get; set; }
        
        string WorkspacePath { get; set; }

        List<FileReference> LibraryIndex { get; set; }
        List<FileReference> AssignedIndex { get; set; }
        List<FileReference> IgnoredIndex { get; set; }
        List<FileReference> DistantIndex { get; set; }
        List<FileReference> DistantFiles { get; set; }
        List<FileReference> FilesToCopy { get; set; }
        List<FileReference> FilesToDelete { get; set; }
        
        private List<FileReference> LibraryFiles { get; set; }
        public bool ModLoaderInstalled { get; set; }
        public bool ModLoaderSetup { get; set; }

        public SmashBuilder(FileWriter _Writer, LibraryViewModel _ViewModel)
        {
            Writer = _Writer;
            ViewModel = _ViewModel;
        }

        public async Task<bool> StartBuild()
        {
            ViewModel.Building = true;
            ViewModel.QuasarLogger.Debug("Building set to true");

            try
            {

                LibraryIndex = await Builder.CreateFileList(_ViewModel.MUVM.Library, Properties.QuasarSettings.Default.DefaultDir, Properties.QuasarSettings.Default.TransferPath);
                AssignedIndex = await Builder.CreateContentFileList(_ViewModel.MUVM.Library, _ViewModel.MUVM.ContentItems, _ViewModel.MUVM.QuasarModTypes,_ViewModel.MUVM.Games[0], Properties.QuasarSettings.Default.DefaultDir, Properties.QuasarSettings.Default.TransferPath);
                IgnoredIndex = await Builder.CreateIgnoreFileList(_ViewModel.MUVM.Library, _ViewModel.MUVM.ContentItems, Properties.QuasarSettings.Default.DefaultDir, Properties.QuasarSettings.Default.TransferPath);
                ViewModel.QuasarLogger.Debug("Got Local File List");

                if (!await GetDistantFileList())
                    return false;
                ViewModel.QuasarLogger.Debug("Got Distant File List");

                if (!await ProcessTransferList())
                    return false;
                ViewModel.QuasarLogger.Debug("Finished processing transfer list");

                //File Operations
                if (FilesToDelete.Count != 0)
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

                if (FilesToCopy.Count != 0)
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
        public async Task<bool> CheckModLoader(int ModLoader)
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
        public async Task<bool> InstallModLoader(int ModLoader, string WorkspaceName)
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
        public async Task<bool> SetupModLoader(int ModLoader, string WorkspaceName)
        {
            try
            {
                if (ModLoader != 4)
                {
                    File.Copy(Properties.QuasarSettings.Default.DefaultDir + "\\Resources\\ModLoaders\\ARCropolis\\arcropolis.toml", Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml", true);

                    if (ModLoader == 1)
                    {
                        ARCropolisHelper.ModifyTouchmARCConfig(WorkspaceName);
                    }
                    else
                    {
                        ARCropolisHelper.ModifyTouchmARCConfig(WorkspaceName);
                    }

                    Writer.SendFile(Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml", "atmosphere\\contents\\01006A800016E000\\romfs\\arcropolis.toml");

                    File.Delete(Properties.QuasarSettings.Default.DefaultDir + "\\Library\\arcropolis.toml");
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
        public async Task<bool> GetDistantFileList()
        {
            try
            {
                //UI Setup
                ViewModel.QuasarLogger.Debug("Starting to list distant files");
                ViewModel.SetStep("Listing Distant Files, please wait as it might take a while");
                ViewModel.SetProgressionStyle(true);

                //Getting distant hashes
                GetDistantIndex();

                DistantFiles = new List<FileReference>();

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
        public async Task<bool> ProcessTransferList()
        {
            try
            {
                FilesToDelete = new();

                //Comparing Library and Assignment Indexes
                processAssignmentList();

                //Comparing Library and Ignored Indexes
                processIgnoreList();

                //List files for copy
                getCopyFileList();

                //List files for deletion
                getDeleteFileList();

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
        public async Task<bool> DeleteDifferences()
        {
            try
            {
                ViewModel.SetStep("Deleting Differences");
                foreach (FileReference reference in FilesToDelete)
                {
                    ViewModel.QuasarLogger.Debug("Deleting " + reference.OutputFilePath);
                    Writer.DeleteFile(WorkspacePath + reference.OutputFilePath);
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
        public async Task<bool> CopyFiles()
        {
            try
            {
                if (!Writer.CheckFolderExists(Properties.QuasarSettings.Default.TransferPath))
                    Writer.CreateFolder(Properties.QuasarSettings.Default.TransferPath);

                ViewModel.SetStep("Transfering Files");
                ViewModel.SetProgressionStyle(false);
                int TotalFiles = FilesToCopy.Count;
                while (FilesToCopy.Count > 0)
                {
                    try
                    {
                        FileReference MF = FilesToCopy[0];
                        string description = "Contains :\r\n";
                        List<FileReference> AssociatedFiles = FilesToCopy.Where(r => r.LibraryItem.Guid == MF.LibraryItem.Guid).ToList();
                        ViewModel.SetSubStep("Copying files for " + MF.LibraryItem.Name);
                        foreach (FileReference Ferb in AssociatedFiles)
                        {
                            SetProgression((TotalFiles - FilesToCopy.Count), TotalFiles);
                            Writer.SendFile(Ferb.SourceFilePath, Ferb.OutputFilePath);
                            FilesToCopy.Remove(Ferb);
                        }
                        ////Sending ARCadia files
                        //string ModConfigInputPath = String.Format(@"{0}\Library\Mods\{1}\info.toml", Properties.QuasarSettings.Default.DefaultDir, Item.Guid);
                        //string ModConfigOutputPath = String.Format(@"{0}\{1}\info.toml", WorkspacePath, Item.Name.Replace(".", ""));
                        //string ModScreenInputPath = String.Format(@"{0}\Library\Screenshots\{1}.webp", Properties.QuasarSettings.Default.DefaultDir, Item.Guid);
                        //string ModDefaultScreenInputPath = String.Format(@"{0}\Resources\images\NoScreenshot.webp", Properties.QuasarSettings.Default.DefaultDir);
                        //string ModScreenOutputPath = String.Format(@"{0}\{1}\preview.webp", WorkspacePath, Item.Name.Replace(".", ""));

                        //Writer.SendFile(ModConfigInputPath, ModConfigOutputPath);

                        //if (File.Exists(ModScreenInputPath))
                        //{
                        //    Writer.SendFile(ModScreenInputPath, ModScreenOutputPath);
                        //}
                        //else
                        //{
                        //    Writer.SendFile(ModDefaultScreenInputPath, ModScreenOutputPath);
                        //}

                        ViewModel.QuasarLogger.Info(String.Format("Finished copying files for {0}", MF.LibraryItem.Name));
                        ViewModel.BuildLog("Mod", String.Format("Finished copying files for {0}", MF.LibraryItem.Name));
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
                SetProgression((TotalFiles - FilesToCopy.Count), TotalFiles);

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

        public void GetDistantIndex()
        {
            string DistantWorkspacePath = WorkspacePath;
            string localHashFilePath = Properties.QuasarSettings.Default.DefaultDir + @"\Library\Downloads\Quasar.json";

            if (Writer.CheckFolderExists("ultimate"))
            {
                //Getting Workspace Hashes
                if (Writer.CheckFileExists(@"ultimate\Quasar.json"))
                {
                    Writer.GetFile(@"ultimate\Quasar.json", localHashFilePath);
                    ViewModel.QuasarLogger.Debug(String.Format("Distant Hash File Exists : {0}", @"ultimate\Quasar.json"));
                }

                //Loading distant Workspace Hashes if found
                if (File.Exists(localHashFilePath))
                {
                    //TODO Edit this
                    ViewModel.QuasarLogger.Debug("File Exists, Loading Hashes");
                    DistantIndex = UserDataManager.GetModFiles(Properties.QuasarSettings.Default.DefaultDir).ToList();
                }
                else
                {
                    ViewModel.QuasarLogger.Debug("No Remote Hash File");
                    ViewModel.QuasarLogger.Debug("Hash List Created");
                    DistantIndex = new List<FileReference>();
                }
            }
            else
            {
                ViewModel.QuasarLogger.Debug("No Remote Hash File");
                ViewModel.QuasarLogger.Debug("Hash List Created");
                DistantIndex = new List<FileReference>();
            }
            
        }
        public void getDistantFileList()
        {
            //Getting Workspace Distant Files
            //DistantFiles = Writer.GetRemoteFiles(WorkspacePath);
        }

        public void processAssignmentList()
        {
            foreach(FileReference file in AssignedIndex)
            {
                FileReference MatchedFile = LibraryIndex.SingleOrDefault(f => f.SourceFilePath.Replace(@"\",@"/") == file.SourceFilePath.Replace(@"\", @"/"));
                if (file.Status == FileStatus.Edited)
                {
                    if(MatchedFile == null)
                    {
                        LibraryIndex.Add(file);
                    }
                    else
                    {
                        FilesToDelete.Add(new FileReference 
                        {
                             FileHash = MatchedFile.FileHash,
                             LibraryItem = MatchedFile.LibraryItem,
                             OutputFilePath = MatchedFile.OutputFilePath,
                             OutsideFile = MatchedFile.OutsideFile,
                             SourceFilePath = MatchedFile.SourceFilePath,
                             Status = MatchedFile.Status
                        });
                        MatchedFile.OutputFilePath = file.OutputFilePath;
                    }
                    
                }
            }
        }

        public void processIgnoreList()
        {
            foreach (FileReference file in AssignedIndex)
            {
                FileReference MatchedFile = LibraryIndex.SingleOrDefault(f => f.SourceFilePath == file.SourceFilePath);
                if (file.Status == FileStatus.Ignored)
                {
                    LibraryIndex.Remove(MatchedFile);
                }

            }
        }
        public void getCopyFileList()
        {
            FilesToCopy = new();

            if (DistantIndex.Count == 0)
            {
                //No distant index means first copy
                foreach (FileReference FileReference in LibraryIndex)
                {
                    FilesToCopy.Add(new FileReference()
                    {
                        FileHash = FileReference.FileHash,
                        LibraryItem = FileReference.LibraryItem,
                        OutputFilePath = FileReference.OutputFilePath,
                        OutsideFile = FileReference.OutsideFile,
                        SourceFilePath = FileReference.SourceFilePath
                    });
                }
            }
            else
            {
                //Copy over means find the differences
                foreach (FileReference reference in LibraryIndex)
                {
                    FileReference distantFile = DistantIndex.FirstOrDefault(mf => mf.OutputFilePath == reference.OutputFilePath);
                    //If there is no distant match
                    if (distantFile == null)
                    {
                        FilesToCopy.Add(reference);
                    }
                    else
                    {
                        //Different hash means overwrite
                        if (reference.FileHash != distantFile.FileHash)
                        {
                            FilesToCopy.Add(reference);
                        }
                    }
                }
            }
        }
        public void getDeleteFileList()
        {
            if (DistantIndex == null)
            {
                DistantIndex = new();
            }
            if(DistantIndex.Count != 0)
            {
                foreach(FileReference FileReference in DistantIndex)
                {
                    if(!LibraryIndex.Any(fr => fr.OutputFilePath == FileReference.OutputFilePath))
                    {
                        FilesToDelete.Add(new FileReference() { OutputFilePath = FileReference.OutputFilePath });
                    }
                }
            }
        }

        public void SaveAndSendIndex()
        {
           //Saving, sending Hashes file
           string LocalIndexFilePath = Properties.QuasarSettings.Default.DefaultDir + @"\Library\Downloads\Quasar.json";

            foreach(FileReference mf in LibraryIndex)
            {
                mf.SourceFilePath = "";
                mf.LibraryItem = null;
            }

            //TODO Edit This
            UserDataManager.SaveModFiles(LibraryIndex, Properties.QuasarSettings.Default.DefaultDir);
            ViewModel.QuasarLogger.Debug("Saved Index");

           //Sending Hash Files
           Writer.SendFile(LocalIndexFilePath,  @"ultimate\Quasar.json");

           ViewModel.QuasarLogger.Debug("Sent Index");
           File.Delete(LocalIndexFilePath);
           ViewModel.QuasarLogger.Debug("Deleted Index");
        }
        public void SetProgression(int cnt, int tot)
        {
            ViewModel.SetProgression(((double)cnt / (double)tot) * 100);
            ViewModel.SetTotal(cnt.ToString(), tot.ToString());
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

    public class BasicInfoData
    {
        public LibraryItem LibraryItem { get; set; }
        public string Description { get; set; }
    }

}
