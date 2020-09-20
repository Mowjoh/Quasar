using FluentFTP;
using MediaDevices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Build.Models
{
    public abstract class FileWriter
    {
        public abstract bool VerifyOK();
        public abstract bool CheckFolderExists(string FolderPath);
        public abstract bool CheckFileExists(string FilePath);
        public abstract bool SendFile(string SourceFilePath, string FilePath);
        public abstract bool DeleteFile(string FilePath);
        public abstract bool DeleteFolder(string FolderPath);
    }

    public class FTPWriter : FileWriter
    {
        public FtpClient Client { get; set; }
        public string Adress { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

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
        public override bool SendFile(string SourceFilePath, string FilePath)
        {
            FtpStatus Status = Client.UploadFile(SourceFilePath, FilePath, FtpRemoteExists.Overwrite, true);
            return Status.IsSuccess();
        }
        public override bool DeleteFile(string FilePath)
        {
            try
            {
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

    }
    public class SDWriter : FileWriter
    {
        public string LetterPath { get; set; }
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
        public override bool SendFile(string SourceFilePath, string FilePath)
        {
            try
            {
                File.Copy(SourceFilePath, FilePath, true);
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
                File.Delete(FilePath);
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
    }
    public class MTPWriter : FileWriter
    {
        public string MediaDevice { get; set; }

        public MediaDevice MediaD { get; set; }

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
        public override bool SendFile(string SourceFilePath, string FilePath)
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
    }
}

