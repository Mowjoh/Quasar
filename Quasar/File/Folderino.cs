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

        public static void CheckBaseFiles()
        {
            String BasePath = Properties.Settings.Default.DefaultDir;
            String CharactersResourcePath = "\\Resources\\Characters.xml";
            String FamiliesResourcePath = "\\Resources\\Families.xml";
            String ModTypesResourcePath = "\\Resources\\ModTypes.xml";

            if (!System.IO.File.Exists(BasePath + CharactersResourcePath))
            {
                ParseFromInstallation(CharactersResourcePath, CharactersResourcePath);
            }

            if (!System.IO.File.Exists(BasePath + FamiliesResourcePath))
            {
                ParseFromInstallation(FamiliesResourcePath, FamiliesResourcePath);
            }

            if (!System.IO.File.Exists(BasePath + ModTypesResourcePath))
            {
                ParseFromInstallation(ModTypesResourcePath, ModTypesResourcePath);
            }

        }

        public static void CheckCreate(String _Path)
        {
            if (!Directory.Exists(_Path))
            {
                Directory.CreateDirectory(_Path);
            }
        }

        public static void ParseFromInstallation(String _localFilePath, String _DocumentsPath)
        {
            String AppPath = Properties.Settings.Default.AppPath;
            String DocumentsFolderPath = Properties.Settings.Default.DefaultDir;

            String SourceFilePath = AppPath + _localFilePath;
            String DestinationFilePath = DocumentsFolderPath + _DocumentsPath;

            System.IO.File.Copy(SourceFilePath, DestinationFilePath, true);

        }

        public static void DeleteDocumentsFolder()
        {
            var DocumentsFolder = Properties.Settings.Default.DefaultDir;
            Directory.Delete(DocumentsFolder, true);
            Application.Current.Shutdown();
        }
    }
}
