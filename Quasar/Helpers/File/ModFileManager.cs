using log4net;
using log4net.Appender;
using DataModels.User;
using DataModels.Common;
using DataModels.Resource;
using Quasar.Helpers.FileOperations;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public ILog Log { get; set; }

        public bool Failed { get; set; } = false;

        //Sets up default paths from the Quasar URL
        public ModFileManager(string _QuasarURL,Game _Game)
        {
            SetupLogger();
            string parameters = _QuasarURL.Substring(7);
            DownloadURL = parameters.Split(',')[0];
            ModID = parameters.Split(',')[2];
            ModArchiveFormat = parameters.Split(',')[3];
            APIType = parameters.Split(',')[1];

            try
            {
                GameAPICategory mt = _Game.GameAPICategories.Single(m => m.APICategoryName == parameters.Split(',')[1]);
                if( mt != null)
                {
                    ModTypeID = mt.ID.ToString();
                    ModTypeFolderName = mt.LibraryFolderName;

                    DownloadDestinationFilePath = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + ModID + "." + ModArchiveFormat;
                    ArchiveContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + ModID + "\\";
                    LibraryContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\" + ModTypeFolderName + "\\" + ModID + "\\";
                }
                else
                {
                    Log.Error("No ModType Found");
                    Failed = true;
                }
                
            }
            catch(Exception e)
            {
                Log.Error(e.Message);
                Failed = true;
            }
            
        }

        public void SetupLogger()
        {
            Log = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)Log.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            if (Properties.Settings.Default.EnableAdvanced)
            {
                appender.Threshold = log4net.Core.Level.Debug;
            }
            else
            {
                appender.Threshold = log4net.Core.Level.Info;
            }
            appender.ActivateOptions();
        }

        //Sets up default paths from the Library Mod
        public ModFileManager(LibraryItem _mod, string ModArchiveFormat = "")
        {        
            DownloadDestinationFilePath = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + _mod.Guid + "." + ModArchiveFormat;
            LibraryContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\" + _mod.Guid + "\\";
            ArchiveContentFolderPath = Properties.Settings.Default.DefaultDir + "\\Library\\Downloads\\" + _mod.Guid + "\\";
        }

        public void CheckOldFolderPath()
        {
            string OldContentPath = Properties.Settings.Default.DefaultDir + "\\Library\\Mods\\SmashUltimate\\" + ModID + "\\";
            if (Directory.Exists(OldContentPath))
            {
                if (Directory.Exists(LibraryContentFolderPath))
                {
                    Directory.Delete(LibraryContentFolderPath,true);
                }
                try
                {
                    Directory.Move(OldContentPath, LibraryContentFolderPath);
                }
                catch(Exception e)
                {
                    Log.Error(e.Message + e.StackTrace);
                }
                
            }
        }

        //Moves the files to the Library Content path
        public async Task<int> MoveDownload()
        {
            await Task.Run(() => FileOperation.CopyFolder(ArchiveContentFolderPath, LibraryContentFolderPath,true));

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

        //Deletes Mod Files
        public void DeleteFiles()
        {
            if (Directory.Exists(LibraryContentFolderPath))
            {
                Directory.Delete(LibraryContentFolderPath, true);
            }
        }

        public void ImportFolder(string FolderPath)
        {
            //Creating destination
            if (!Directory.Exists(LibraryContentFolderPath))
            {
                Directory.CreateDirectory(LibraryContentFolderPath);
            }
            //Emptying it if necessary
            FileOperation.RecreateFolder(LibraryContentFolderPath);

            //Copying mod files
            FileOperation.CheckCopyFolder(FolderPath, LibraryContentFolderPath);
        }


    }
}
