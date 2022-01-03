namespace DataModels.FileWriters
{
    //public class MTPWriter : FileWriter
    //{
    //    public string MediaDevice { get; set; }

    //    public MediaDevice MediaD { get; set; }
    //    public ILog Log { get; set; }
    //    BuildViewModel BVM { get; set; }

    //    public MTPWriter(BuildViewModel _BVM)
    //    {
    //        BVM = _BVM;
    //    }

    //    public override async Task<bool> VerifyOK()
    //    {
    //        MediaD.Connect();
    //        return MediaD.IsConnected;
    //    }
    //    public override bool CheckFolderExists(string FolderPath)
    //    {
    //        return MediaD.DirectoryExists("sdcard\\" + FolderPath);
    //    }
    //    public override bool CheckFileExists(string FilePath)
    //    {
    //        return MediaD.FileExists("sdcard\\" + FilePath);
    //    }
    //    public override bool SendFile(string SourceFilePath, string FilePath)
    //    {
    //        string destinationpath = FilePath.Substring(0, FilePath.Length - FilePath.Split('\\')[FilePath.Split('\\').Length - 1].Length);

    //        if (!MediaD.DirectoryExists("sdcard\\" + destinationpath))
    //        {
    //            MediaD.CreateDirectory("sdcard\\" + destinationpath);
    //        }

    //        if (MediaD.FileExists("sdcard\\" + FilePath))
    //        {
    //            MediaD.DeleteFile("sdcard\\" + FilePath);
    //        }

    //        using (FileStream fsSource = new FileStream(SourceFilePath, FileMode.Open, FileAccess.Read))
    //        {
    //            MediaD.UploadFile(fsSource, "sdcard\\" + FilePath);
    //        }
    //        return true;
    //    }
    //    public override bool DeleteFile(string FilePath)
    //    {
    //        try
    //        {
    //            if (MediaD.FileExists("sdcard\\" + FilePath))
    //            {
    //                MediaD.DeleteFile("sdcard\\" + FilePath);
    //            }

    //        }
    //        catch (Exception e)
    //        {
    //            BVM.QuasarLogger.Error(e.Message);
    //        }

    //        return false;
    //    }
    //    public override bool CreateFolder(string FolderPath)
    //    {
    //        MediaD.DeleteDirectory("sdcard\\" + FolderPath, true);
    //        return false;
    //    }
    //    public override bool DeleteFolder(string FolderPath)
    //    {
    //        MediaD.DeleteDirectory("sdcard\\" + FolderPath, true);
    //        return false;
    //    }
    //    public override void GetFile(string Remote, string DestinationFilePath)
    //    {
    //        using (FileStream fsDestination = new FileStream(DestinationFilePath, FileMode.Create, FileAccess.ReadWrite))
    //        {
    //            MediaD.DownloadFile("sdcard\\" + Remote, fsDestination);
    //        }
    //    }
    //    public override ObservableCollection<ModFile> GetRemoteFiles(string FolderPath)
    //    {
    //        return null;
    //    }

    //}
}
