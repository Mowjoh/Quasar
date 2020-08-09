using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Quasar.XMLResources.Library;

namespace Quasar.FileSystem
{
    public class ModFileManager
    {
        public string ModID { get; set; }
        public string ModTypeID { get; set; }
        public string ModTypeFolderName { get; set; }
        public string APIType { get; set; }
        public string ModArchiveFormat { get; set; }
        public string DownloadURL { get; set; }

        //Setting file management paths
        public string DownloadDestinationFilePath { get; set; }
        public string ArchiveContentFolderPath { get; set; }
        public string LibraryContentFolderPath { get; set; }

        //Sets up default paths from the Quasar URL
        public ModFileManager(string _QuasarURL,Game _Game)
        {
            string parameters = _QuasarURL.Substring(7);
            DownloadURL = parameters.Split(',')[0];
            ModID = parameters.Split(',')[2];
            ModArchiveFormat = parameters.Split(',')[3];
            APIType = parameters.Split(',')[1];

            
            GameModType mt = _Game.GameModTypes.Find(m => m.APIName == parameters.Split(',')[1]);
            ModTypeID = mt.ID.ToString();
            ModTypeFolderName = mt.LibraryFolder;

            DownloadDestinationFilePath = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + ModID + "." + ModArchiveFormat;
            ArchiveContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + ModID + "\\";
            LibraryContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\" + ModTypeFolderName + "\\" + ModID + "\\";
        }

        public ModFileManager(string _QuasarURL)
        {
            string parameters = _QuasarURL.Substring(7);
            DownloadURL = parameters.Split(',')[0];
            ModID = parameters.Split(',')[2];
            ModArchiveFormat = parameters.Split(',')[3];
            APIType = parameters.Split(',')[1];

            DownloadDestinationFilePath = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + ModID + "." + ModArchiveFormat;
            ArchiveContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + ModID + "\\";
            LibraryContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\" + ModTypeFolderName + "\\" + ModID + "\\";
        }

        //Sets up default paths from the Library Mod
        public ModFileManager(LibraryMod _mod, Game _Game)
        {
            ModID = _mod.ID.ToString();
            ModTypeID = _mod.TypeID.ToString();

            GameModType mt = _Game.GameModTypes.Find(m => m.ID == _mod.TypeID);
            ModTypeFolderName = mt.LibraryFolder;
            LibraryContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\" + ModTypeFolderName + "\\" + ModID + "\\";
        }

        //Moves the files to the Library Content path
        public async Task<int> MoveDownload()
        {
            await Task.Run(() => Folderino.CopyFolder(ArchiveContentFolderPath, LibraryContentFolderPath,true));

            return 0;
        }

        //Cleans up Downloads
        public void ClearDownloadContents()
        {
            if (File.Exists(DownloadDestinationFilePath))
            {
                File.Delete(DownloadDestinationFilePath);
            }
            if (Directory.Exists(ArchiveContentFolderPath))
            {
                Directory.Delete(ArchiveContentFolderPath, true);
            }
        }




    }
}
