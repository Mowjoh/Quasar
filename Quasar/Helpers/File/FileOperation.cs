using System;
using System.IO;
using System.Windows;

namespace Quasar.Helpers.FileOperations
{
    //@Raytwo I know you love it <3
    public static class FileOperation
    {
        //Checks if a File exists and copies it if it doesn't exist.
        public static void CheckCopyFolder(string source, string destination, bool CheckDestinationExists = true, bool Overwrite = true)
        {
            if (CheckDestinationExists)
            {
                if (!Directory.Exists(destination))
                    Directory.CreateDirectory(destination);
            }

            foreach (string s in Directory.GetFiles(source))
            {
                string filename = Path.GetFileName(s);
                string dest = destination + filename;
                if (!File.Exists(dest))
                {
                    File.Copy(s, dest);
                }
                else
                {
                    if (Overwrite)
                    {
                        File.Copy(s, dest, true);
                    }
                }
            }
        }

        public static void CheckCreate(String _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }

        public static void CheckCopyFile(string source, string destination)
        {
            string parent = Path.GetDirectoryName(destination);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
            }
            File.Copy(source, destination, true);
        }
        //Copies a Directory with the option to wipe the destination beforehand
        public static void CopyFolder(string SourceFolderPath, string DestinationFolderPath, bool ClearDestination)
        {
            if (ClearDestination)
            {
                ClearFolder(DestinationFolderPath);
            }

            if (!Directory.Exists(DestinationFolderPath))
                Directory.CreateDirectory(DestinationFolderPath);

            foreach (var dirPath in Directory.GetDirectories(SourceFolderPath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourceFolderPath, DestinationFolderPath));

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(SourceFolderPath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourceFolderPath, DestinationFolderPath), true);
        }

        //Make sure the folder is clean
        public static void ClearFolder(string SourceFolderPath)
        {
            if (Directory.Exists(SourceFolderPath))
            {
                Directory.Delete(SourceFolderPath, true);
            }
            Directory.CreateDirectory(SourceFolderPath);
        }
    }
}
