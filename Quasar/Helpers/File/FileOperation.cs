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

            DirectoryCopy(source, destination, true);
        }

        public static void CheckCreate(String _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
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
