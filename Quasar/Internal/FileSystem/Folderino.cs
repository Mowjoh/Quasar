using System;
using System.IO;
using System.Windows;

namespace Quasar.FileSystem
{
    //@Raytwo I know you love it <3
    static class Folderino
    {
        //Verifies needed folders for execution
        public static void CheckBaseFolders()
        {
            //Setting Paths
            String BasePath = Properties.Settings.Default.DefaultDir;
            String LibraryPath = BasePath + "\\Library\\";
            String ModsPath = BasePath + "\\Library\\Mods\\";
            String DownloadsPath = BasePath + "\\Library\\Downloads\\";
            String ScreenshotPath = BasePath + "\\Library\\Screenshots\\";
            String ReferencePath = BasePath + "\\References\\";

            //Verifying they exist, creating them if not
            CheckCreate(BasePath);
            CheckCreate(LibraryPath);
            CheckCreate(ModsPath);
            CheckCreate(DownloadsPath);
            CheckCreate(ScreenshotPath);
            CheckCreate(ReferencePath);
        }

        //Forcing references files and base Internal Mod Types to be refreshed in the user folder
        public static void UpdateBaseFiles()
        {
            String ReferencePath = Properties.Settings.Default.DefaultDir + "\\References\\";
            String AppPath = Properties.Settings.Default.AppPath + "\\References\\";

            Folderino.CopyFolder(AppPath, ReferencePath, true);
        }

        //Checks if a Directory exists and creates it if it doesn't exist.
        public static void CheckCreate(String _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }

        //Checks if a File exists and copies it if it doesn't exist.
        public static void CheckCopy(string source, string destination)
        {
            foreach (string s in Directory.GetFiles(source))
            {
                string filename = Path.GetFileName(s);
                string dest = destination + filename;
                if (!File.Exists(dest))
                {

                    File.Copy(s, dest);
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

        //Copies non-existant resources in the user folder
        public static void CompareReferences()
        {
            string AppResPath = Properties.Settings.Default.AppPath + "\\References";
            string DocumentsResourcePath = Properties.Settings.Default.DefaultDir + "\\References\\";
            foreach(string FolderPath in Directory.GetDirectories(AppResPath))
            {
                string DocumentsPath = DocumentsResourcePath + Path.GetFileName(FolderPath) + @"\";
                CheckCreate(DocumentsPath);
                CheckCopy(FolderPath, DocumentsPath);
            }
            
        }

        //Destroys everything Quasar has so I can reset the state to default easily
        public static void DeleteDocumentsFolder()
        {
            var DocumentsFolder = Properties.Settings.Default.DefaultDir;
            foreach(String s in Directory.GetDirectories(DocumentsFolder))
            {
                Directory.Delete(s, true);
            }
            foreach(string s in Directory.GetFiles(DocumentsFolder))
            {
                System.IO.File.Delete(s);
            }
            
            Application.Current.Shutdown();
        }


        //Copies a Directory with the option to wipe the destination beforehand
        public static void CopyFolder(string SourceFolderPath, string DestinationFolderPath, bool ClearDestination)
        {
            if (ClearDestination)
            {
                CheckClearFolder(DestinationFolderPath);
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
        public static void CheckClearFolder(string SourceFolderPath)
        {
            if (Directory.Exists(SourceFolderPath))
            {
                Directory.Delete(SourceFolderPath, true);
            }
            Directory.CreateDirectory(SourceFolderPath);
        }
    }
}
