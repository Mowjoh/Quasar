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
            CheckCreate(BasePath);
            CheckCreate(LibraryPath);
            CheckCreate(ModsPath);
            CheckCreate(DownloadsPath);

        }

        public static void CheckCreate(String _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
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
