using DataModels.Resource;
using DataModels.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Workshop.Builder;

namespace Workshop.FileManagement
{
    public class UserDataManager
    {
        //User Loads
        public static ObservableCollection<LibraryItem> GetLibrary(string _QuasarFolderPath)
        {
            ObservableCollection<LibraryItem> Library = new ObservableCollection<LibraryItem>();

            string Path = _QuasarFolderPath + @"\Library\Library.json";

            try
            {
                if (File.Exists(Path))
                {
                    using (StreamReader file = File.OpenText(Path))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Library = (ObservableCollection<LibraryItem>)serializer.Deserialize(file, typeof(ObservableCollection<LibraryItem>));
                    }
                }
            }
            catch(Exception e)
            {
                
            }

            return Library;
        }

        public static ModInformation GetModInformation(string _ModInformationPath)
        {
            ModInformation Item = new ModInformation();
            try
            {
                if (File.Exists(_ModInformationPath))
                {
                    using (StreamReader file = File.OpenText(_ModInformationPath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Item = (ModInformation)serializer.Deserialize(file, typeof(ModInformation));
                    }
                }
            }
            catch (Exception e)
            {

            }

            return Item;
        }

        public static ObservableCollection<Workspace> GetWorkspaces(string _QuasarFolderPath)
        {
            ObservableCollection<Workspace> Workspaces = new ObservableCollection<Workspace>();

            string Path = _QuasarFolderPath + @"\Library\Workspaces.json";

            if (File.Exists(Path))
            {
                using (StreamReader file = File.OpenText(Path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Workspaces = (ObservableCollection<Workspace>)serializer.Deserialize(file, typeof(ObservableCollection<Workspace>));
                }
            }

            return Workspaces;
        }
        public static ShareableWorkspace GetSharedWorkspace(string _QuasarFolderPath)
        {
            ShareableWorkspace SW = new ShareableWorkspace();

            string Path = _QuasarFolderPath + @"\Library\Workspaces.json";

            if (File.Exists(Path))
            {
                using (StreamReader file = File.OpenText(Path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    SW = (ShareableWorkspace)serializer.Deserialize(file, typeof(ShareableWorkspace));
                }
            }

            return SW;
        }
        public static ObservableCollection<ContentItem> GetContentItems(string _QuasarFolderPath)
        {
            ObservableCollection<ContentItem> ContentItems = new ObservableCollection<ContentItem>();

            string Path = _QuasarFolderPath + @"\Library\ContentItems.json";

            if (File.Exists(Path))
            {
                using (StreamReader file = File.OpenText(Path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    ContentItems = (ObservableCollection<ContentItem>)serializer.Deserialize(file, typeof(ObservableCollection<ContentItem>));
                }
            }

            return ContentItems;
        }
        public static ObservableCollection<FileReference> GetModFiles(string _QuasarFolderPath)
        {
            ObservableCollection<FileReference> ModFiles = new ObservableCollection<FileReference>();

            string Path = _QuasarFolderPath + @"\Library\Downloads\Quasar.json";

            if (File.Exists(Path))
            {
                using (StreamReader file = File.OpenText(Path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    ModFiles = (ObservableCollection<FileReference>)serializer.Deserialize(file, typeof(ObservableCollection<FileReference>));
                }
            }

            return ModFiles;
        }

        //User Saves
        public static void SaveLibrary(ObservableCollection<LibraryItem> _LibraryItems, string _QuasarFolderPath)
        {
            if (Directory.Exists(_QuasarFolderPath + @"\Library"))
                Directory.CreateDirectory(_QuasarFolderPath + @"\Library");

            SaveJSonFile(_QuasarFolderPath + @"\Library\Library.json", _LibraryItems);
        }
        public static void SaveModInformation(ModInformation _ModInformation, string _QuasarModFolder)
        {
            SaveJSonFile(_QuasarModFolder + String.Format(@"\Library\Mods\{0}\ModInformation.json",_ModInformation.LibraryItem.Guid), _ModInformation);
        }
        public static void SaveWorkspaces(ObservableCollection<Workspace> _Workspaces, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\Workspaces.json", _Workspaces.OrderBy(i => i.Guid));
        }
        public static void SaveContentItems(ObservableCollection<ContentItem> _ContentItems, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\ContentItems.json", _ContentItems.OrderBy(i => i.Guid));
        }
        public static void SaveModFiles(List<FileReference> _file_references, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\Downloads\Quasar.json", _file_references);
        }
        public static void SaveSharedWorkspaces(ShareableWorkspace SW, string _DestinationFolderPath)
        {
            SaveJSonFile(_DestinationFolderPath + @"\SharedWorkspace.json", SW);
        }

        /// <summary>
        /// Saves a collection to a JSon File
        /// </summary>
        /// <param name="_Fullpath">Destination file path</param>
        /// <param name="_Source">Source collection</param>
        /// <param name="_ExternalPath">Override Destination file path</param>
        private static void SaveJSonFile(string _Fullpath, Object _Source, bool Headless = false)
        {
            string directory = _Fullpath.Replace(@"/", @"\");
            directory = _Fullpath.Replace(_Fullpath.Split(@"\")[_Fullpath.Split(@"\").Length - 1], "");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (StreamWriter file = File.CreateText(_Fullpath))
            {
                JsonSerializer serializer = new JsonSerializer();
                if (Headless)
                    serializer.TypeNameHandling = TypeNameHandling.None;
                serializer.Serialize(file, _Source);
            }
        }

        /// <summary>
        /// Creates a default Workspace
        /// </summary>
        public static Guid CreateBaseWorkspace(string _QuasarFolderPath)
        {
            String AssociationsPath = _QuasarFolderPath + @"\Library\Workspaces.json";

            if (!Directory.Exists(_QuasarFolderPath + @"\Library\"))
                Directory.CreateDirectory(_QuasarFolderPath + @"\Library\");

            Workspace defaultWorkspace = new Workspace() { Name = "Default", Guid = Guid.NewGuid(), Associations = new ObservableCollection<Association>(), BuildDate = "" };
            ObservableCollection<Workspace> DefaultFile = new ObservableCollection<Workspace>() { defaultWorkspace };
            SaveWorkspaces(DefaultFile, _QuasarFolderPath);

            return defaultWorkspace.Guid;
        }

        /// <summary>
        /// Moves all user data Json files to the proper location
        /// </summary>
        /// <param name="_OldDataPath">Quasar's Mod folder</param>
        /// <param name="_NewDataPath">Quasar's App Data Folder</param>
        public static void VerifyUpdateFileLocation(string _OldDataPath, string _NewDataPath)
        {
            string[] OldLibraryFiles = Directory.GetFiles(_OldDataPath + @"\Library", "*", SearchOption.TopDirectoryOnly);

            //If there are old files
            if (OldLibraryFiles.Length > 0)
            {
                string NewLibraryDataPath = _NewDataPath + @"\Library\";

                if (!Directory.Exists(NewLibraryDataPath))
                {
                    Directory.CreateDirectory(NewLibraryDataPath);
                }

                //Moving the files to a new location
                foreach (string LibraryFile in OldLibraryFiles)
                {
                    string FileName = Path.GetFileName(LibraryFile);
                    string NewFilePath = _NewDataPath + @"\Library\" + FileName;

                    File.Copy(LibraryFile, NewFilePath);
                    File.Delete(LibraryFile);
                }
            }
        }

        public static bool BackupUserDataFiles(string AppDataPath)
        {
            try
            {
                string BackupPath = AppDataPath + @"\Backups\";

                if (Directory.Exists(BackupPath))
                {
                    Directory.Delete(BackupPath, true);
                }

                Directory.CreateDirectory(BackupPath);
                Directory.CreateDirectory(String.Format(@"{0}\Backups\Library", AppDataPath));
                Directory.CreateDirectory(String.Format(@"{0}\Backups\Resources", AppDataPath));


                foreach (string filepath in Directory.GetFiles(String.Format(@"{0}\Library", AppDataPath)))
                {
                    string newfilepath = String.Format(@"{0}\Backups\Library\{1}", AppDataPath, Path.GetFileName(filepath));
                    File.Copy(filepath, newfilepath);
                }

                foreach (string filepath in Directory.GetFiles(String.Format(@"{0}\Resources", AppDataPath)))
                {
                    string newfilepath = String.Format(@"{0}\Backups\Resources\{1}", AppDataPath, Path.GetFileName(filepath));
                    File.Copy(filepath, newfilepath);
                }

                if (Directory.GetFiles(BackupPath, "*.json", SearchOption.AllDirectories).Length == 0)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }

        public static bool RestoreUserDataFiles(string AppDataPath)
        {
            try
            {
                string BackupPath = AppDataPath + @"\Backups\";

                foreach (string filepath in Directory.GetFiles(String.Format(@"{0}\Backups\Library", AppDataPath)))
                {
                    string newfilepath = String.Format(@"{0}\Library\{1}", AppDataPath, Path.GetFileName(filepath));
                    File.Copy(filepath, newfilepath,true);
                }

                foreach (string filepath in Directory.GetFiles(String.Format(@"{0}\Backups\Resources", AppDataPath)))
                {
                    string newfilepath = String.Format(@"{0}\Resources\{1}", AppDataPath, Path.GetFileName(filepath));
                    File.Copy(filepath, newfilepath, true);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static ObservableCollection<LibraryItem> RecoverMods(string _QuasarFolderPath,string _AppDataPath, ObservableCollection<LibraryItem> Library, GamebananaAPI API)
        {
            string[] ModFolders = Directory.GetDirectories(_QuasarFolderPath + @"\Library\Mods\", "*", SearchOption.TopDirectoryOnly);

            foreach(string ModFolder in ModFolders)
            {
                string ModInfoPath = ModFolder + @"\ModInformation.json";
                if (File.Exists(ModInfoPath))
                {
                    ModInformation mi = GetModInformation(ModInfoPath);
                    LibraryItem li = mi.LibraryItem;

                    if(!Library.Any(l => l.GBItem.GamebananaItemID == li.GBItem.GamebananaItemID))
                    {
                        Library.Add(li);
                    }
                    API = ResourceManager.UpdateGamebananaAPI(mi.GamebananaRootCategory, API);
                }
            }
            ResourceManager.SaveGamebananaAPI(API, _AppDataPath);

            return Library;
        }
    }
}
