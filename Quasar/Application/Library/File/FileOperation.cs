using System;
using System.IO;

namespace Quasar.Helpers.FileOperations
{
    public static class FileOperation
    {
        /// <summary>
        /// Copies a folder to it's destination
        /// Can check if the destination exists first
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="CheckDestinationExists"></param>
        /// <param name="Overwrite"></param>
        public static void CheckCopyFolder(string source, string destination, bool CheckDestinationExists = true, bool Overwrite = true)
        {
            if (CheckDestinationExists)
                CheckCreate(destination);

            DirectoryCopy(source, destination, true);
        }

        /// <summary>
        /// Checks if a folder exists and creates it if necessary
        /// </summary>
        /// <param name="_Path"></param>
        public static void CheckCreate(String _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }

        /// <summary>
        /// Checks if the destination exists, then copies the file.
        /// Will overwrite
        /// </summary>
        /// <param name="source">Source File path</param>
        /// <param name="destination">Destination File path</param>
        public static void CheckCopyFile(string source, string destination)
        {
            string parent = Path.GetDirectoryName(destination);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
            }
            File.Copy(source, destination, true);
        }

        /// <summary>
        /// Copies a Directory with the option to wipe the destination beforehand
        /// </summary>
        /// <param name="SourceFolderPath">Source Folder Path</param>
        /// <param name="DestinationFolderPath">Destination Folder Path</param>
        /// <param name="ClearDestination">Destination cleanup trigger</param>
        public static void CopyFolder(string SourceFolderPath, string DestinationFolderPath, bool ClearDestination)
        {
            if (ClearDestination)
            {
                RecreateFolder(DestinationFolderPath);
            }

            if (!Directory.Exists(DestinationFolderPath))
                Directory.CreateDirectory(DestinationFolderPath);

            foreach (var dirPath in Directory.GetDirectories(SourceFolderPath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourceFolderPath, DestinationFolderPath));

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(SourceFolderPath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourceFolderPath, DestinationFolderPath), true);
        }

        /// <summary>
        /// Directory Copy Logic
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
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

        /// <summary>
        /// Re-creates the folder
        /// </summary>
        /// <param name="SourceFolderPath">Folder path</param>
        public static void RecreateFolder(string SourceFolderPath)
        {
            if (Directory.Exists(SourceFolderPath))
            {
                Directory.Delete(SourceFolderPath, true);
            }
            Directory.CreateDirectory(SourceFolderPath);
        }
    }
}
