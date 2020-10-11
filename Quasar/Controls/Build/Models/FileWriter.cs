using FluentFTP;
using log4net;
using MediaDevices;
using Quasar.Controls.Build.ViewModels;
using Quasar.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Build.Models
{
    public abstract class FileWriter
    {
        public abstract bool VerifyOK();
        public abstract bool CheckFolderExists(string FolderPath);
        public abstract bool CheckFileExists(string FilePath);
        public abstract bool SendFile(string SourceFilePath, string FilePath, bool OutsideFile, bool OverrideHash = false);
        public abstract bool DeleteFile(string FilePath);
        public abstract bool DeleteFolder(string FolderPath);
        public abstract void GetFile(string Remote, string DestinationFilePath);
        public abstract List<FileReference> GetRemoteFiles(string FolderPath);
        public abstract void SetHashes(List<Hash> _DistantHashes, List<Hash> _DistantOutsiderHashes, List<Hash> _LocalHashes);
        public abstract List<Hash> GetHashes();
        public abstract List<Hash> GetOutsiderHashes();
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
        BuildViewModel BVM { get; set; }

        public List<Hash> DistantHashes { get; set; }
        public List<Hash> DistantOutsiderHashes { get; set; }
        public List<Hash> LocalHashes { get; set; }
        public List<Hash> LocalOutsiderHashes { get; set; }

        List<FileReference> list { get; set; }
        public ILog Log { get; set; }
        #endregion
        public FTPWriter(BuildViewModel _BVM)
        {
            if (Properties.Settings.Default.FTPValid)
            {
                BVM = _BVM;

                Adress = Properties.Settings.Default.FTPAddress.Split(':')[0];
                Port = Properties.Settings.Default.FTPAddress.Split(':')[1];
                Username = Properties.Settings.Default.FTPUN;
                Password = Properties.Settings.Default.FTPPW;

                Client = new FtpClient(Adress);
                Client.Port = int.Parse(Port);
                if (Username != "")
                {
                    Client.Credentials = new System.Net.NetworkCredential(Username, Password);
                }

                Client.RecursiveList = true;

                Client.Connect();

                Progress = delegate (FtpProgress p) {
                    if (p.Progress != 100)
                    {
                        BVM.SetSpeed(String.Format("{0}/s, {1}%", WriterOperations.BytesToString(p.TransferSpeed), p.Progress.ToString().Substring(0, 2)));
                    }
                };

            }
        }
        public FTPWriter()
        {
            if (Properties.Settings.Default.FTPValid)
            {
                Adress = Properties.Settings.Default.FTPAddress.Split(':')[0];
                Port = Properties.Settings.Default.FTPAddress.Split(':')[1];
                Username = Properties.Settings.Default.FTPUN;
                Password = Properties.Settings.Default.FTPPW;

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

        public override bool VerifyOK()
        {
            return Client.IsConnected;
        }

        public override bool CheckFolderExists(string FolderPath)
        {
            
            return Client.DirectoryExists(FolderPath);
        }
        public override bool CheckFileExists(string FilePath)
        {
            return Client.FileExists(FilePath);
        }
        public override bool SendFile(string SourceFilePath, string FilePath,bool OutsideFile, bool OverrideHash = false)
        {
            if (!OverrideHash)
            {
                Hash distantHash = OutsideFile ? DistantOutsiderHashes.SingleOrDefault(H => H.FilePath == FilePath) : DistantHashes.SingleOrDefault(H => H.FilePath == FilePath);
                string localHash = WriterOperations.GetHash(SourceFilePath);
                if (distantHash != null)
                {
                    if (distantHash.HashString != localHash)
                    {
                        BVM.SetSize(String.Format("Current File Size : {0}", WriterOperations.BytesToString(new FileInfo(SourceFilePath).Length)));
                        Log.Debug(String.Format("Updating File {0} - {1}", SourceFilePath, FilePath));
                        FtpStatus Status = Client.UploadFile(SourceFilePath, FilePath, FtpRemoteExists.Overwrite, true, FtpVerify.None, Progress);
                        distantHash.HashString = localHash;
                        if (OutsideFile)
                        {
                            LocalOutsiderHashes.Add(distantHash);
                        }
                        else
                        {
                            LocalHashes.Add(distantHash);
                        }
                        

                        return Status.IsSuccess();
                    }
                    else
                    {
                        if (OutsideFile)
                        {
                            LocalOutsiderHashes.Add(distantHash);
                        }
                        else
                        {
                            LocalHashes.Add(distantHash);
                        }
                    }
                }
                else
                {
                    BVM.SetSize(String.Format("Current File Size : {0}", WriterOperations.BytesToString(new FileInfo(SourceFilePath).Length)));
                    FtpStatus Status = Client.UploadFile(SourceFilePath, FilePath, FtpRemoteExists.Overwrite, true, FtpVerify.None, Progress);

                    if (OutsideFile)
                    {
                        DistantOutsiderHashes.Add(new Hash() { HashString = localHash, FilePath = FilePath });
                        LocalOutsiderHashes.Add(new Hash() { HashString = localHash, FilePath = FilePath });
                    }
                    else
                    {
                        DistantHashes.Add(new Hash() { HashString = localHash, FilePath = FilePath });
                        LocalHashes.Add(new Hash() { HashString = localHash, FilePath = FilePath });
                    }
                    

                    Log.Debug(String.Format("Adding File {0} - {1}", SourceFilePath, FilePath));
                    return Status.IsSuccess();

                }
            }
            else
            {
                if(Progress == null)
                {
                    FtpStatus Status = Client.UploadFile(SourceFilePath, FilePath, FtpRemoteExists.Overwrite, true, FtpVerify.None);
                    Log.Debug(String.Format("Adding File disregarding Hash {0} - {1}", SourceFilePath, FilePath));
                }
                else
                {
                    FtpStatus Status = Client.UploadFile(SourceFilePath, FilePath, FtpRemoteExists.Overwrite, true, FtpVerify.None, Progress);
                    Log.Debug(String.Format("Adding File disregarding Hash {0} - {1}", SourceFilePath, FilePath));
                }
                
                
            }
            return true;
        }
        public override bool DeleteFile(string FilePath)
        {
            try
            {
                Hash x = DistantHashes.SingleOrDefault(h => h.FilePath == FilePath);
                if (x != null)
                    DistantHashes.Remove(x);
                Log.Debug(String.Format("Deleting File {0}", FilePath));
                Client.DeleteFile(FilePath);

                return true;
            }catch(Exception e)
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
        public override List<FileReference> GetRemoteFiles(string FolderPath)
        {
            list = new List<FileReference>();

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
        public override void SetHashes(List<Hash> _DistantHashes, List<Hash> _DistantOutsiderHashes, List<Hash> _LocalHashes)
        {
            DistantHashes = _DistantHashes;
            DistantOutsiderHashes = _DistantOutsiderHashes;
            LocalHashes = _LocalHashes;
            LocalOutsiderHashes = new List<Hash>();
        }
        public override List<Hash> GetHashes()
        {
            List<Hash> NewDistantHashes = new List<Hash>();
            foreach (Hash h in DistantHashes)
            {
                if (LocalHashes.Any(sh => sh.FilePath == h.FilePath))
                {
                    NewDistantHashes.Add(h);
                }
            }
            return NewDistantHashes;
        }
        public override List<Hash> GetOutsiderHashes()
        {
            List<Hash> NewDistantHashes = new List<Hash>();
            if (DistantOutsiderHashes != null)
            {
                foreach (Hash h in DistantOutsiderHashes)
                {
                    if (LocalOutsiderHashes.Any(sh => sh.FilePath == h.FilePath))
                    {
                        NewDistantHashes.Add(h);
                    }
                }
            }
            return NewDistantHashes;
        }

        public void ListFiles(FtpListItem[] Files)
        {
            try
            {
                foreach (FtpListItem item in Files)
                {
                    if (item.Type == FtpFileSystemObjectType.File)
                    {
                        list.Add(new FileReference()
                        {
                            OutputFilePath = item.FullName
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
        BuildViewModel BVM { get; set; }
        public List<Hash> DistantHashes { get; set; }
        public List<Hash> DistantOutsiderHashes { get; set; }
        public List<Hash> LocalHashes { get; set; }
        public List<Hash> LocalOutsiderHashes { get; set; }
        public ILog Log { get; set; }

        public SDWriter(BuildViewModel _BVM)
        {
            BVM = _BVM;
        }

        public override bool VerifyOK()
        {
            return true;
        }
        public override bool CheckFolderExists(string FolderPath)
        {
            return Directory.Exists(FolderPath);
        }
        public override bool CheckFileExists(string FilePath)
        {
            return File.Exists(FilePath);
        }
        public override bool SendFile(string SourceFilePath, string FilePath, bool OutsideFile, bool OverrideHash = false)
        {
            try
            {
                if (!OverrideHash)
                {
                    Hash distantHash = OutsideFile ? DistantOutsiderHashes.SingleOrDefault(H => H.FilePath == FilePath) : DistantHashes.SingleOrDefault(H => H.FilePath == FilePath);
                    string localHash = WriterOperations.GetHash(SourceFilePath);
                    if (distantHash != null)
                    {
                        if (distantHash.HashString != localHash)
                        {
                            BVM.SetSize(String.Format("Current File Size : {0}", WriterOperations.BytesToString(new FileInfo(SourceFilePath).Length)));
                            Folderino.CheckCopyFile(SourceFilePath, FilePath);
                            distantHash.HashString = localHash;
                            if (OutsideFile)
                            {
                                LocalOutsiderHashes.Add(distantHash);
                            }
                            else
                            {
                                LocalHashes.Add(distantHash);
                            }
                            Log.Debug(String.Format("Updating File {0} - {1}",SourceFilePath, FilePath));
                            return true;
                        }
                        else
                        {
                            if (OutsideFile)
                            {
                                LocalOutsiderHashes.Add(distantHash);
                            }
                            else
                            {
                                LocalHashes.Add(distantHash);
                            }
                        }
                    }
                    else
                    {
                        BVM.SetSize(String.Format("Current File Size : {0}", WriterOperations.BytesToString(new FileInfo(SourceFilePath).Length)));
                        Folderino.CheckCopyFile(SourceFilePath, FilePath);
                        if (OutsideFile)
                        {
                            DistantOutsiderHashes.Add(new Hash() { HashString = localHash, FilePath = FilePath });
                            LocalOutsiderHashes.Add(new Hash() { HashString = localHash, FilePath = FilePath });
                        }
                        else
                        {
                            DistantHashes.Add(new Hash() { HashString = localHash, FilePath = FilePath });
                            LocalHashes.Add(new Hash() { HashString = localHash, FilePath = FilePath });
                        }
                        Log.Debug(String.Format("Adding File {0} - {1}", SourceFilePath, FilePath));
                        return true;

                    }
                }
                else
                {
                    Folderino.CheckCopyFile(SourceFilePath, FilePath);
                }

            }
            catch(Exception e)
            {
                return false;
            }
            return true;
        }
        public override bool DeleteFile(string FilePath)
        {
            try
            {
                Hash x = DistantHashes.SingleOrDefault(h => h.FilePath == FilePath);
                if (x != null)
                    DistantHashes.Remove(x);

                File.Delete(FilePath);
                Log.Debug(String.Format("Deleting File {0}", FilePath));
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
                Directory.Delete(FolderPath,true);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public override void GetFile(string Remote, string DestinationFilePath)
        {
            File.Copy(Remote, DestinationFilePath, true);
        }
        public override List<FileReference> GetRemoteFiles(string FolderPath)
        {
            List<FileReference> list = new List<FileReference>();
            if (Directory.Exists(FolderPath))
            {
                foreach (string file in Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories))
                {
                    list.Add(new FileReference()
                    {
                        OutputFilePath = file
                    });
                }
            }
            return list;
        }
        public override void SetHashes(List<Hash> _DistantHashes, List<Hash> _DistantOutsiderHashes, List<Hash> _LocalHashes)
        {
            DistantHashes = _DistantHashes;
            LocalHashes = _LocalHashes;
    }
        public override List<Hash> GetHashes()
        {
            List<Hash> NewDistantHashes = new List<Hash>();
            foreach(Hash h in DistantHashes)
            {
                if (LocalHashes.Contains(h))
                {
                    NewDistantHashes.Add(h);
                }
            }
            return NewDistantHashes;
        }
        public override List<Hash> GetOutsiderHashes()
        {
            List<Hash> NewDistantHashes = new List<Hash>();
            if(DistantOutsiderHashes != null)
            {
                foreach (Hash h in DistantOutsiderHashes)
                {
                    if (LocalOutsiderHashes.Any(sh => sh.FilePath == h.FilePath))
                    {
                        NewDistantHashes.Add(h);
                    }
                }
            }
            return NewDistantHashes;
        }
    }
    public class MTPWriter : FileWriter
    {
        public string MediaDevice { get; set; }

        public MediaDevice MediaD { get; set; }
        public ILog Log { get; set; }
        public List<Hash> DistantHashes { get; set; }
        public List<Hash> DistantOutsiderHashes { get; set; }
        public List<Hash> LocalHashes { get; set; }
        public List<Hash> LocalOutsiderHashes { get; set; }

        public override bool VerifyOK()
        {
            return MediaD.IsConnected;
        }
        public override bool CheckFolderExists(string FolderPath)
        {
            return MediaD.DirectoryExists(FolderPath);
        }
        public override bool CheckFileExists(string FilePath)
        {
            return MediaD.FileExists(FilePath);
        }
        public override bool SendFile(string SourceFilePath, string FilePath, bool OutsideFile, bool OverrideHash = false)
        {

            return false;
        }
        public override bool DeleteFile(string FilePath)
        {
            return false;
        }
        public override bool DeleteFolder(string FolderPath)
        {
            return false;
        }
        public override void GetFile(string Remote, string DestinationFilePath)
        {
        }
        public override List<FileReference> GetRemoteFiles(string FolderPath)
        {
            return null;
        }

        public override void SetHashes(List<Hash> _DistantHashes, List<Hash> _DistantOutsiderHashes, List<Hash> _LocalHashes)
        {

        }
        public override List<Hash> GetHashes()
        {
            return null;
        }
        public override List<Hash> GetOutsiderHashes()
        {
            List<Hash> NewDistantHashes = new List<Hash>();
            foreach (Hash h in DistantOutsiderHashes)
            {
                if (LocalOutsiderHashes.Any(sh => sh.FilePath == h.FilePath))
                {
                    NewDistantHashes.Add(h);
                }
            }
            return NewDistantHashes;
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

