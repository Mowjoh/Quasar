﻿using FluentFTP;
using log4net;
using MediaDevices;
using DataModels.User;
using Quasar.Helpers.FileOperations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Quasar.Controls.ModManagement.ViewModels;

namespace Quasar.Library.Models
{
    public abstract class FileWriter
    {
        public abstract Task<bool> VerifyOK();
        public abstract bool CheckFolderExists(string FolderPath);
        public abstract bool CheckFileExists(string FilePath);
        public abstract bool SendFile(string SourceFilePath, string FilePath);
        public abstract bool DeleteFile(string FilePath);
        public abstract bool CreateFolder(string FolderPath);
        public abstract bool DeleteFolder(string FolderPath);
        public abstract void GetFile(string Remote, string DestinationFilePath);
        public abstract ObservableCollection<ModFile> GetRemoteFiles(string FolderPath);

    }

    public class FTPWriter : FileWriter
    {
        #region Properties
        public FtpClient Client { get; set; }
        public string Adress { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Action<FtpProgress> Progress { get; set; }
        LibraryViewModel Lvm { get; set; }

        public List<Hash> DistantHashes { get; set; }

        ObservableCollection<ModFile> list { get; set; }
        public ILog Log { get; set; }
        #endregion
        public FTPWriter(LibraryViewModel _LVM)
        {
            if (Properties.QuasarSettings.Default.FTPValid)
            {
                Lvm = _LVM;

                Adress = Properties.QuasarSettings.Default.FtpIP;
                Port = Properties.QuasarSettings.Default.FtpPort;
                Username = Properties.QuasarSettings.Default.FtpUsername;
                Password = Properties.QuasarSettings.Default.FtpPassword;

                Client = new FtpClient(Adress);
                Client.Port = int.Parse(Port);
                Client.ConnectTimeout = 3000;
                if (Username != "")
                {
                    Client.Credentials = new System.Net.NetworkCredential(Username, Password);
                }

                Client.RecursiveList = true;

                Progress = delegate (FtpProgress p) {
                    if (p.Progress != 100)
                    {
                        Lvm.SetSpeed(String.Format("{0}/s, {1}%", WriterOperations.BytesToString(p.TransferSpeed), p.Progress.ToString().Substring(0, 2)));
                    }
                };

            }
        }
        public FTPWriter()
        {
            if (Properties.QuasarSettings.Default.FTPValid)
            {
                Adress = Properties.QuasarSettings.Default.FtpAddress.Split(':')[0];
                Port = Properties.QuasarSettings.Default.FtpAddress.Split(':')[1];
                Username = Properties.QuasarSettings.Default.FtpUsername;
                Password = Properties.QuasarSettings.Default.FtpPassword;

                Client = new FtpClient(Adress);
                Client.Port = int.Parse(Port);
                if (Username != "")
                {
                    Client.Credentials = new System.Net.NetworkCredential(Username, Password);
                }

                Client.RecursiveList = true;

                Client.Connect();

            }
        }

        public override async Task<bool> VerifyOK()
        {
            Client.ConnectTimeout = 3000;
            await Task.Run(() => {
                try
                {
                    Client.Connect();
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            });
            
            return Client.IsConnected;
        }

        public override bool CheckFolderExists(string FolderPath)
        {
            
            return Client.DirectoryExists(FolderPath);
        }
        public override bool CheckFileExists(string FilePath)
        {
            return Client.FileExists(FilePath.Replace(@"\", @"/"));
        }
        public override bool SendFile(string SourceFilePath, string FilePath)
        {
            Lvm.SetSize(String.Format("Current File Size : {0}", WriterOperations.BytesToString(new FileInfo(SourceFilePath).Length)));
            Log.Debug(String.Format("Sending File {0} \r\n {1}", SourceFilePath, FilePath));
            string CorrectedPath = FilePath.Replace(@"\", @"/");
            string Directory =
                CorrectedPath.Replace(CorrectedPath.Split(@"/")[CorrectedPath.Split(@"/").Length - 1], "");
            Client.CreateDirectory(Directory);
            FtpStatus Status = Client.UploadFile(SourceFilePath.Replace(@"\", @"/"), CorrectedPath, FtpRemoteExists.Overwrite, true, FtpVerify.None, Progress);
            return true;
        }
        public override bool DeleteFile(string FilePath)
        {
            try
            {
                Log.Debug(String.Format("Deleting File {0}", FilePath));
                if (CheckFileExists(FilePath))
                {
                    Client.DeleteFile(FilePath);
                }
                

                return true;
            }catch(Exception e)
            {
                return false;
            }
        }
        public override bool CreateFolder(string FolderPath)
        {
            try
            {
                Client.CreateDirectory(FolderPath);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public override bool DeleteFolder(string FolderPath)
        {
            try
            {
                Client.DeleteDirectory(FolderPath);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public override ObservableCollection<ModFile> GetRemoteFiles(string FolderPath)
        {
            list = new ObservableCollection<ModFile>();

            try
            {
                FtpListItem[] files = Client.GetListing(FolderPath);
                ListFiles(files);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            

            return list;
        }
        public override void GetFile(string Remote, string DestinationFilePath)
        {
            Client.DownloadFile(DestinationFilePath, Remote);
        }

        public void ListFiles(FtpListItem[] Files)
        {
            try
            {
                foreach (FtpListItem item in Files)
                {
                    if (item.Type == FtpFileSystemObjectType.File)
                    {
                        list.Add(new ModFile()
                        {
                             DestinationFilePath = item.FullName
                        });
                        Log.Debug(String.Format("Adding File to distant list {0}", item.FullName));
                    }
                    if (item.Type == FtpFileSystemObjectType.Directory)
                    {
                        FtpListItem[] folderFiles = Client.GetListing(item.FullName);
                        ListFiles(folderFiles);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
    public class SDWriter : FileWriter
    {
        public string LetterPath { get; set; }
        LibraryViewModel BVM { get; set; }
        public ILog Log { get; set; }

        public SDWriter(LibraryViewModel _BVM)
        {
            BVM = _BVM;
        }

        public override async Task<bool> VerifyOK()
        {
            return true;
        }
        public override bool CheckFolderExists(string FolderPath)
        {
            return Directory.Exists(LetterPath + FolderPath);
        }
        public override bool CheckFileExists(string FilePath)
        {
            return File.Exists(LetterPath + FilePath);
        }
        public override bool SendFile(string SourceFilePath, string FilePath)
        {
            Log.Debug(String.Format("Sending File {0} \r\n {1}", SourceFilePath, FilePath));
            FileOperation.CheckCopyFile(SourceFilePath, LetterPath + FilePath);
            return true;
        }
        public override bool DeleteFile(string FilePath)
        {
            if (File.Exists(LetterPath + FilePath))
            {
                Log.Debug(String.Format("Deleting File {0}", FilePath));
                File.Delete(LetterPath + FilePath);
            }
            else
            {

            }
            return true;
        }
        public override bool CreateFolder(string FolderPath)
        {
            try
            {
                Directory.CreateDirectory(LetterPath + FolderPath);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public override bool DeleteFolder(string FolderPath)
        {
            try
            {
                Directory.Delete(LetterPath + FolderPath, true);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public override void GetFile(string Remote, string DestinationFilePath)
        {
            File.Copy(LetterPath + Remote, DestinationFilePath, true);
        }
        public override ObservableCollection<ModFile> GetRemoteFiles(string FolderPath)
        {
            ObservableCollection<ModFile> list = new ObservableCollection<ModFile>();
            if (Directory.Exists(FolderPath))
            {
                foreach (string file in Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories))
                {
                    list.Add(new ModFile(){
                     DestinationFilePath = file
                    });
                }
            }
            return list;
        }
    }

    public class DiskWriter : FileWriter
    {
        public string DiskPath { get; set; }
        LibraryViewModel LVM { get; set; }
        public ILog Log { get; set; }

        public DiskWriter(LibraryViewModel _BVM)
        {
            LVM = _BVM;
        }

        public override async Task<bool> VerifyOK()
        {
            return true;
        }
        public override bool CheckFolderExists(string FolderPath)
        {
            return Directory.Exists(DiskPath + @"\" +FolderPath);
        }
        public override bool CheckFileExists(string FilePath)
        {
            return File.Exists(DiskPath + @"\" +FilePath);
        }
        public override bool SendFile(string SourceFilePath, string FilePath)
        {
            Log.Debug(String.Format("Sending File {0} \r\n {1}", SourceFilePath, FilePath));
            FileOperation.CheckCopyFile(SourceFilePath, DiskPath + @"\" +FilePath);
            return true;
        }
        public override bool DeleteFile(string FilePath)
        {
            if (File.Exists(DiskPath + @"\" +FilePath))
            {
                Log.Debug(String.Format("Deleting File {0}", FilePath));
                File.Delete(DiskPath + @"\" +FilePath);
            }
            else
            {

            }
            return true;
        }
        public override bool CreateFolder(string FolderPath)
        {
            try
            {
                Directory.CreateDirectory(DiskPath + @"\" +FolderPath);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public override bool DeleteFolder(string FolderPath)
        {
            try
            {
                Directory.Delete(DiskPath + @"\" +FolderPath, true);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public override void GetFile(string Remote, string DestinationFilePath)
        {
            File.Copy(DiskPath + @"\" +Remote, DestinationFilePath, true);
        }
        public override ObservableCollection<ModFile> GetRemoteFiles(string FolderPath)
        {
            ObservableCollection<ModFile> list = new ObservableCollection<ModFile>();
            if (Directory.Exists(FolderPath))
            {
                foreach (string file in Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories))
                {
                    list.Add(new ModFile()
                    {
                        DestinationFilePath = file
                    });
                }
            }
            return list;
        }
    }


    public class MTPWriter : FileWriter
    {
        public string MediaDevice { get; set; }

        public MediaDevice MediaD { get; set; }
        public ILog Log { get; set; }
        LibraryViewModel BVM { get; set; }

        public MTPWriter(LibraryViewModel _BVM)
        {
            BVM = _BVM;
        }
        
        public override async Task<bool> VerifyOK()
        {
            MediaD.Connect();
            return MediaD.IsConnected;
        }
        public override bool CheckFolderExists(string FolderPath)
        {
            return MediaD.DirectoryExists("sdcard\\" + FolderPath);
        }
        public override bool CheckFileExists(string FilePath)
        {
            return MediaD.FileExists("sdcard\\" + FilePath);
        }
        public override bool SendFile(string SourceFilePath, string FilePath)
        {
            string destinationpath = FilePath.Substring(0, FilePath.Length - FilePath.Split('\\')[FilePath.Split('\\').Length - 1].Length);

            if (!MediaD.DirectoryExists("sdcard\\"+destinationpath))
            {
                MediaD.CreateDirectory("sdcard\\" + destinationpath);
            }

            if (MediaD.FileExists("sdcard\\" + FilePath))
            {
                MediaD.DeleteFile("sdcard\\" + FilePath);
            }

            using (FileStream fsSource = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
            {
                MediaD.UploadFile(fsSource, "sdcard\\" + FilePath);
            }
            return true;
        }
        public override bool DeleteFile(string FilePath)
        {
            try
            {
                if(MediaD.FileExists("sdcard\\" + FilePath))
                {
                    MediaD.DeleteFile("sdcard\\" + FilePath);
                }
                    
            }
            catch(Exception e)
            {
                BVM.QuasarLogger.Error(e.Message);
            }
            
            return false;
        }
        public override bool CreateFolder(string FolderPath)
        {
            MediaD.DeleteDirectory("sdcard\\" + FolderPath, true);
            return false;
        }
        public override bool DeleteFolder(string FolderPath)
        {
            MediaD.DeleteDirectory("sdcard\\" + FolderPath,true);
            return false;
        }
        public override void GetFile(string Remote, string DestinationFilePath)
        {
            using (FileStream fsDestination = new FileStream(DestinationFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                MediaD.DownloadFile("sdcard\\" + Remote, fsDestination);
            }
        }
        public override ObservableCollection<ModFile> GetRemoteFiles(string FolderPath)
        {
            return null;
        }

    }

    public static class WriterOperations
    {
        public static String BytesToString(long len)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
        }
        public static String BytesToString(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
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
}

