using DataModels.User;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DataModels.FileWriters
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

