using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quasar.Library;

namespace Quasar.File
{
    public class FileManager
    {
        public string modID { get; set; }
        public string modType { get; set; }
        public string APIType { get; set; }
        public string modArchiveFormat { get; set; }
        public string downloadURL { get; set; }

        //Setting file management paths
        public string downloadDest { get; set; }
        public string archiveContentDest { get; set; }
        public string libraryContentPath { get; set; }

        public FileManager(string _quasarURL)
        {
            string parameters = _quasarURL.Substring(7);
            downloadURL = parameters.Split(',')[0];
            modID = parameters.Split(',')[2];
            modArchiveFormat = parameters.Split(',')[3];
            APIType = parameters.Split(',')[1];
            switch (parameters.Split(',')[1])
            {
                case "Skin":
                    modType = "0";
                    break;
                default:
                    break;
            }

            downloadDest = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + modID + "." + modArchiveFormat;
            archiveContentDest = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + modID + "\\";
            libraryContentPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\" + modType + "\\" + modID + "\\";
        }

        public FileManager(Mod _mod)
        {
            modID = _mod.id.ToString();
            modType = _mod.type.ToString();
            libraryContentPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\" + modType + "\\" + modID + "\\";
        }

        public async Task<int> MoveDownload()
        {
            await Task.Run(() => CopyFolder(archiveContentDest, libraryContentPath));

            return 0;
        }

        private void CopyFolder(string _sourceDir, string _destDir)
        {
            checkClearFolder(_destDir);
            if (!Directory.Exists(_destDir))
                Directory.CreateDirectory(_destDir);

            foreach (var dirPath in Directory.GetDirectories(_sourceDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(_sourceDir, _destDir));

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.GetFiles(_sourceDir, "*.*", SearchOption.AllDirectories))
                System.IO.File.Copy(newPath, newPath.Replace(_sourceDir, _destDir), true);
        }

        public void checkClearFolder(string _sourceDir)
        {
            if (Directory.Exists(_sourceDir))
            {
                Directory.Delete(_sourceDir, true);
            }
            Directory.CreateDirectory(_sourceDir);
        }

        public void ClearDownloadContents()
        {
            if (System.IO.File.Exists(downloadDest))
            {
                System.IO.File.Delete(downloadDest);
            }
            if (Directory.Exists(archiveContentDest))
            {
                Directory.Delete(archiveContentDest, true);
            }
        }
    }
}
