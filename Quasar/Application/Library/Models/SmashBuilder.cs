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

namespace Quasar.Library.Models
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
        
        string WorkspacePath { get; set; }

        List<FileReference> _LibraryIndex { get; set; }
        List<FileReference> _AssignedIndex { get; set; }
        List<FileReference> _IgnoredIndex { get; set; }
        List<FileReference> _DistantIndex { get; set; }
        List<FileReference> _DistantFiles { get; set; }
        public List<FileReference> TransferIndex { get; set; }

        public SmashBuilder(FileWriter _writer, LibraryViewModel _view_model)
        {
            Writer = _writer;
            ViewModel = _view_model;
        }

        public async Task<bool> StartBuild()
        {
            ViewModel.Building = true;
            ViewModel.QuasarLogger.Debug("Building set to true");

            try
            {
                ViewModel.BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_ProcessingLists);
                TransferIndex = await Builder.ParseFileList(ViewModel.MUVM.Library, Properties.QuasarSettings.Default.DefaultDir, Properties.QuasarSettings.Default.TransferPath);
                _LibraryIndex = await Builder.ParseFileList(ViewModel.MUVM.Library, Properties.QuasarSettings.Default.DefaultDir, Properties.QuasarSettings.Default.TransferPath);
                _AssignedIndex = await Builder.ProcessContentFileList(ViewModel.MUVM.Library, ViewModel.MUVM.ContentItems, ViewModel.MUVM.QuasarModTypes,ViewModel.MUVM.Games[0], Properties.QuasarSettings.Default.DefaultDir, Properties.QuasarSettings.Default.TransferPath);
                _IgnoredIndex = await Builder.ProcessIgnoreFileList(ViewModel.MUVM.Library, ViewModel.MUVM.ContentItems, Properties.QuasarSettings.Default.DefaultDir, Properties.QuasarSettings.Default.TransferPath);
                ViewModel.QuasarLogger.Debug("Got Local File List");

                if (!await GetDistantFileList())
                    return false;
                ViewModel.QuasarLogger.Debug("Got Distant File List");

                if (!await ProcessTransferList())
                    return false;
                ViewModel.QuasarLogger.Debug("Finished processing transfer list");

                if (Properties.QuasarSettings.Default.TransferQuasarFoldersOnly && (_DistantIndex.Count == 0 || (_DistantIndex.Count != 0 && Properties.QuasarSettings.Default.CleanupMade == false)))
                {
                    //Cleanup requested with no distant index
                    ViewModel.BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_Cleanup);
                    ViewModel.QuasarLogger.Debug("cleaning files");
                    await CleanupModDirectory();
                }

                //File Operations
                if (TransferIndex.Any(_f => _f.Status == FileStatus.Delete))
                {
                    if (!await DeleteDifferences())
                        return false;
                    ViewModel.BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_AllFilesDeleted);
                }
                else
                {
                    ViewModel.BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_NoDelete);
                    ViewModel.QuasarLogger.Debug("No files to delete");
                }

                if (TransferIndex.Any(_f => _f.Status == FileStatus.Copy || _f.Status == FileStatus.CopyEdited))
                {
                    if (!await CopyFiles())
                        return false;
                    ViewModel.BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_AllFilesCopied);
                }
                else
                {
                    ViewModel.BuildLog(Properties.Resources.Transfer_Log_Info, Properties.Resources.Transfer_Log_NoCopy);
                    ViewModel.QuasarLogger.Debug("No files to copy");
                }

                SaveAndSendIndex();
                return true;
            }
            catch(Exception e)
            {
                ViewModel.BuildLog(Properties.Resources.Transfer_Log_Error, Properties.Resources.Transfer_Log_GlobalError);
                ViewModel.QuasarLogger.Error("Global Build Error");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }
            
        }

        #region Builder Actions
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

                _DistantFiles = new List<FileReference>();

                return true;
            }
            catch (Exception e)
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
                TransferIndex = Builder.CompareAssignments(TransferIndex, _AssignedIndex);
                TransferIndex = Builder.CompareIgnored(TransferIndex, _IgnoredIndex);
                TransferIndex = Builder.CompareDistant(TransferIndex, _DistantIndex);

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
                foreach (FileReference reference in TransferIndex.Where(_fr => _fr.Status == FileStatus.Delete))
                {
                    ViewModel.QuasarLogger.Debug("Deleting " + reference.OutputFilePath);
                    Writer.DeleteFile(WorkspacePath + reference.OutputFilePath);
                }
                return true;

            }
            catch (Exception e)
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

                ViewModel.SetStep(Properties.Resources.Transfer_Step_TransferingFilesStepText);
                ViewModel.SetProgressionStyle(false);
                List<FileReference> CopyList = TransferIndex.Where(_fr => _fr.Status == FileStatus.Copy || _fr.Status == FileStatus.CopyEdited).ToList();
                int TotalFiles = CopyList.Count;
                while (CopyList.Count > 0)
                {
                    try
                    {
                        FileReference FileReference = CopyList[0];
                        List<FileReference> AssociatedFiles = CopyList.Where(_r => _r.LibraryItem.Guid == FileReference.LibraryItem.Guid).ToList();
                        ViewModel.SetSubStep(String.Format(Properties.Resources.Transfer_Step_TransferingFilesSubStepText, FileReference.LibraryItem.Name));
                        foreach (FileReference Ferb in AssociatedFiles)
                        {
                            SetProgression((TotalFiles - CopyList.Count), TotalFiles);
                            Writer.SendFile(Ferb.SourceFilePath, Ferb.OutputFilePath);
                            CopyList.Remove(Ferb);
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

                        ViewModel.QuasarLogger.Info(String.Format("Finished copying files for {0}", FileReference.LibraryItem.Name));
                        ViewModel.BuildLog("Mod", String.Format(Properties.Resources.Transfer_Log_FinishedMod, FileReference.LibraryItem.Name));
                    }
                    catch (Exception e)
                    {
                        ViewModel.BuildLog(Properties.Resources.Transfer_Log_Error, Properties.Resources.Transfer_Log_FileError);
                        ViewModel.QuasarLogger.Error("Error");
                        ViewModel.QuasarLogger.Error(e.Message);
                        ViewModel.QuasarLogger.Error(e.StackTrace);
                        return false;
                    }

                }
                SetProgression((TotalFiles - CopyList.Count), TotalFiles);

                return true;
            }
            catch (Exception e)
            {
                ViewModel.BuildLog(Properties.Resources.Transfer_Log_Error, Properties.Resources.Transfer_Log_FileError);
                ViewModel.QuasarLogger.Error("Error");
                ViewModel.QuasarLogger.Error(e.Message);
                ViewModel.QuasarLogger.Error(e.StackTrace);
                return false;
            }




        }

        public async Task<bool> CleanupModDirectory()
        {
            Writer.DeleteFolder(Properties.QuasarSettings.Default.TransferPath);
            Writer.CreateFolder(Properties.QuasarSettings.Default.TransferPath);
            Properties.QuasarSettings.Default.CleanupMade = true;
            Properties.QuasarSettings.Default.Save();
            return true;
        }
        #endregion

        #region Index
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
                    ViewModel.QuasarLogger.Debug("File Exists, Loading Hashes");
                    _DistantIndex = UserDataManager.GetModFiles(Properties.QuasarSettings.Default.DefaultDir).ToList();
                }
                else
                {
                    ViewModel.QuasarLogger.Debug("No Remote Hash File");
                    ViewModel.QuasarLogger.Debug("Hash List Created");
                    _DistantIndex = new List<FileReference>();
                }
            }
            else
            {
                ViewModel.QuasarLogger.Debug("No Remote Hash File");
                ViewModel.QuasarLogger.Debug("Hash List Created");
                _DistantIndex = new List<FileReference>();
            }

        }
        public void SaveAndSendIndex()
        {
            //Saving, sending Hashes file
            string LocalIndexFilePath = Properties.QuasarSettings.Default.DefaultDir + @"\Library\Downloads\Quasar.json";

            foreach (FileReference mf in _LibraryIndex)
            {
                mf.SourceFilePath = "";
                mf.LibraryItem = null;
            }

            //TODO Edit This
            UserDataManager.SaveModFiles(_LibraryIndex, Properties.QuasarSettings.Default.DefaultDir);
            ViewModel.QuasarLogger.Debug("Saved Index");

            //Sending Hash Files
            Writer.SendFile(LocalIndexFilePath, @"ultimate\Quasar.json");

            ViewModel.QuasarLogger.Debug("Sent Index");
            File.Delete(LocalIndexFilePath);
            ViewModel.QuasarLogger.Debug("Deleted Index");
        }
        #endregion

        #region View
        public void SetProgression(int cnt, int tot)
        {
            ViewModel.SetProgression(((double)cnt / (double)tot) * 100);
            ViewModel.SetTotal(cnt.ToString(), tot.ToString());
        }
        #endregion


    }

}
