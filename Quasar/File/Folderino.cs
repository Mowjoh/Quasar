using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Quasar.File
{
    static class Folderino
    {
        public static void CheckBaseFolders()
        {
            String BasePath = Properties.Settings.Default.DefaultDir;
            String LibraryPath = BasePath + "\\Library\\";
            String ModsPath = BasePath + "\\Library\\Mods\\";
            String DownloadsPath = BasePath + "\\Library\\Downloads\\";
            String ResourcePath = BasePath + "\\Resources\\";
            CheckCreate(BasePath);
            CheckCreate(LibraryPath);
            CheckCreate(ModsPath);
            CheckCreate(DownloadsPath);
            CheckCreate(ResourcePath);
            

        }

        public static void CheckResources()
        {
            
        }

        public static void UpdateBaseFiles()
        {
            String ResourcePath = Properties.Settings.Default.DefaultDir + "\\Resources\\";
            String AppPath = Properties.Settings.Default.AppPath + "\\Resources\\";
            FileManager.CopyFolder(AppPath, ResourcePath, true);
        }

        public static void CheckCreate(String _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }

        public static void CompareResources()
        {
            String AppPath = Properties.Settings.Default.AppPath + "\\Resources";
            String DocumentsResourcePath = Properties.Settings.Default.DefaultDir + "\\Resources\\";

            foreach (string s in Directory.GetFiles(AppPath))
            {
                string filename = Path.GetFileName(s);
                string dest = DocumentsResourcePath + filename;
                if (!System.IO.File.Exists(dest))
                {
                    
                    System.IO.File.Copy(s, dest);
                }
            }
        }

        public static void DeleteDocumentsFolder()
        {
            var DocumentsFolder = Properties.Settings.Default.DefaultDir;
            Directory.Delete(DocumentsFolder, true);
            Application.Current.Shutdown();
        }
    }
}
