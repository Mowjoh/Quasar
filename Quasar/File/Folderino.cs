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
            String InternalModTypesPath = BasePath + "\\InternalModTypes\\";
            CheckCreate(BasePath);
            CheckCreate(LibraryPath);
            CheckCreate(ModsPath);
            CheckCreate(DownloadsPath);
            CheckCreate(ResourcePath);
            CheckCreate(InternalModTypesPath);


        }

        public static void CheckResources()
        {
            
        }

        public static void UpdateBaseFiles()
        {
            String ResourcePath = Properties.Settings.Default.DefaultDir + "\\Resources\\";
            String InternalModTypesPath = Properties.Settings.Default.DefaultDir + "\\InternalModTypes\\";
            String AppPath = Properties.Settings.Default.AppPath + "\\Resources\\";
            String IMTApp = Properties.Settings.Default.AppPath + "\\InternalModTypes\\";

            ModFileManager.CopyFolder(AppPath, ResourcePath, true);
            ModFileManager.CopyFolder(IMTApp, InternalModTypesPath, true);
        }

        public static void CheckCreate(String _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }

        public static void checkCopy(string source, string destination)
        {
            foreach (string s in Directory.GetFiles(source))
            {
                string filename = Path.GetFileName(s);
                string dest = destination + filename;
                if (!System.IO.File.Exists(dest))
                {

                    System.IO.File.Copy(s, dest);
                }
            }
        }

        public static void CompareResources()
        {
            String AppResPath = Properties.Settings.Default.AppPath + "\\Resources";
            String DocumentsResourcePath = Properties.Settings.Default.DefaultDir + "\\Resources\\";
            String AppIMTPath = Properties.Settings.Default.AppPath + "\\InternalModTypes";
            String InternalModTypesPath = Properties.Settings.Default.DefaultDir + "\\InternalModTypes\\";

            checkCopy(AppResPath, DocumentsResourcePath);
            checkCopy(AppIMTPath, InternalModTypesPath);
        }

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
    }
}
