using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.FileWriters
{
    //public class SDWriter : FileWriter
    //{
    //    public string LetterPath { get; set; }
    //    BuildViewModel BVM { get; set; }
    //    public ILog Log { get; set; }

    //    public SDWriter(BuildViewModel _BVM)
    //    {
    //        BVM = _BVM;
    //    }

    //    public override async Task<bool> VerifyOK()
    //    {
    //        return true;
    //    }
    //    public override bool CheckFolderExists(string FolderPath)
    //    {
    //        return Directory.Exists(LetterPath + FolderPath);
    //    }
    //    public override bool CheckFileExists(string FilePath)
    //    {
    //        return File.Exists(LetterPath + FilePath);
    //    }
    //    public override bool SendFile(string SourceFilePath, string FilePath)
    //    {
    //        FileOperation.CheckCopyFile(SourceFilePath, LetterPath + FilePath);
    //        return true;
    //    }
    //    public override bool DeleteFile(string FilePath)
    //    {
    //        if (File.Exists(LetterPath + FilePath))
    //        {
    //            File.Delete(LetterPath + FilePath);
    //        }
    //        else
    //        {

    //        }
    //        return true;
    //    }
    //    public override bool CreateFolder(string FolderPath)
    //    {
    //        try
    //        {
    //            Directory.CreateDirectory(LetterPath + FolderPath);
    //        }
    //        catch (Exception e)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //    public override bool DeleteFolder(string FolderPath)
    //    {
    //        try
    //        {
    //            Directory.Delete(LetterPath + FolderPath, true);
    //        }
    //        catch (Exception e)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //    public override void GetFile(string Remote, string DestinationFilePath)
    //    {
    //        File.Copy(LetterPath + Remote, DestinationFilePath, true);
    //    }
    //    public override ObservableCollection<ModFile> GetRemoteFiles(string FolderPath)
    //    {
    //        ObservableCollection<ModFile> list = new ObservableCollection<ModFile>();
    //        if (Directory.Exists(FolderPath))
    //        {
    //            foreach (string file in Directory.GetFiles(FolderPath, "*", SearchOption.AllDirectories))
    //            {
    //                list.Add(new ModFile()
    //                {
    //                    DestinationFilePath = file
    //                });
    //            }
    //        }
    //        return list;
    //    }
    //}
}
