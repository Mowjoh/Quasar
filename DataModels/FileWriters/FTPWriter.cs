namespace DataModels.FileWriters
{
    //public class FTPWriter : FileWriter
    //{
    //    #region Properties
    //    public FtpClient Client { get; set; }
    //    public string Adress { get; set; }
    //    public string Port { get; set; }
    //    public string Username { get; set; }
    //    public string Password { get; set; }
    //    public Action<FtpProgress> Progress { get; set; }
    //    BuildViewModel BVM { get; set; }

    //    public List<Hash> DistantHashes { get; set; }

    //    ObservableCollection<ModFile> list { get; set; }
    //    public ILog Log { get; set; }
    //    #endregion
    //    public FTPWriter(BuildViewModel _BVM)
    //    {
    //        if (Properties.Settings.Default.FTPValid)
    //        {
    //            BVM = _BVM;

    //            Adress = Properties.Settings.Default.FTPAddress.Split(':')[0];
    //            Port = Properties.Settings.Default.FTPAddress.Split(':')[1];
    //            Username = Properties.Settings.Default.FTPUN;
    //            Password = Properties.Settings.Default.FTPPW;

    //            Client = new FtpClient(Adress);
    //            Client.Port = int.Parse(Port);
    //            Client.ConnectTimeout = 3000;
    //            if (Username != "")
    //            {
    //                Client.Credentials = new System.Net.NetworkCredential(Username, Password);
    //            }

    //            Client.RecursiveList = true;

    //            Progress = delegate (FtpProgress p) {
    //                if (p.Progress != 100)
    //                {
    //                    BVM.SetSpeed(String.Format("{0}/s, {1}%", WriterOperations.BytesToString(p.TransferSpeed), p.Progress.ToString().Substring(0, 2)));
    //                }
    //            };

    //        }
    //    }
    //    public FTPWriter()
    //    {
    //        if (Properties.Settings.Default.FTPValid)
    //        {
    //            Adress = Properties.Settings.Default.FTPAddress.Split(':')[0];
    //            Port = Properties.Settings.Default.FTPAddress.Split(':')[1];
    //            Username = Properties.Settings.Default.FTPUN;
    //            Password = Properties.Settings.Default.FTPPW;

    //            Client = new FtpClient(Adress);
    //            Client.Port = int.Parse(Port);
    //            if (Username != "")
    //            {
    //                Client.Credentials = new System.Net.NetworkCredential(Username, Password);
    //            }

    //            Client.RecursiveList = true;

    //            Client.Connect();

    //        }
    //    }

    //    public override async Task<bool> VerifyOK()
    //    {
    //        Client.ConnectTimeout = 3000;
    //        await Task.Run(() => {
    //            try
    //            {
    //                Client.Connect();
    //            }
    //            catch (Exception e)
    //            {
    //                Log.Error(e.Message);
    //            }
    //        });

    //        return Client.IsConnected;
    //    }

    //    public override bool CheckFolderExists(string FolderPath)
    //    {

    //        return Client.DirectoryExists(FolderPath);
    //    }
    //    public override bool CheckFileExists(string FilePath)
    //    {
    //        return Client.FileExists(FilePath);
    //    }
    //    public override bool SendFile(string SourceFilePath, string FilePath)
    //    {
    //        BVM.SetSize(String.Format("Current File Size : {0}", WriterOperations.BytesToString(new FileInfo(SourceFilePath).Length)));
    //        Log.Debug(String.Format("Updating File {0} - {1}", SourceFilePath, FilePath));
    //        FtpStatus Status = Client.UploadFile(SourceFilePath, FilePath, FtpRemoteExists.Overwrite, true, FtpVerify.None, Progress);
    //        return true;
    //    }
    //    public override bool DeleteFile(string FilePath)
    //    {
    //        try
    //        {
    //            Log.Debug(String.Format("Deleting File {0}", FilePath));
    //            if (CheckFileExists(FilePath))
    //            {
    //                Client.DeleteFile(FilePath);
    //            }


    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            return false;
    //        }
    //    }
    //    public override bool CreateFolder(string FolderPath)
    //    {
    //        try
    //        {
    //            Client.CreateDirectory(FolderPath);
    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            return false;
    //        }
    //    }
    //    public override bool DeleteFolder(string FolderPath)
    //    {
    //        try
    //        {
    //            Client.DeleteDirectory(FolderPath);
    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            return false;
    //        }
    //    }
    //    public override ObservableCollection<ModFile> GetRemoteFiles(string FolderPath)
    //    {
    //        list = new ObservableCollection<ModFile>();

    //        try
    //        {
    //            FtpListItem[] files = Client.GetListing(FolderPath);
    //            ListFiles(files);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e);
    //        }


    //        return list;
    //    }
    //    public override void GetFile(string Remote, string DestinationFilePath)
    //    {
    //        Client.DownloadFile(DestinationFilePath, Remote);
    //    }

    //    public void ListFiles(FtpListItem[] Files)
    //    {
    //        try
    //        {
    //            foreach (FtpListItem item in Files)
    //            {
    //                if (item.Type == FtpFileSystemObjectType.File)
    //                {
    //                    list.Add(new ModFile()
    //                    {
    //                        DestinationFilePath = item.FullName
    //                    });
    //                    Log.Debug(String.Format("Adding File to distant list {0}", item.FullName));
    //                }
    //                if (item.Type == FtpFileSystemObjectType.Directory)
    //                {
    //                    FtpListItem[] folderFiles = Client.GetListing(item.FullName);
    //                    ListFiles(folderFiles);
    //                }
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e);
    //        }

    //    }
    //}
}
