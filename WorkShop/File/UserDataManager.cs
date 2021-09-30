using DataModels.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshop.FileManagement
{
    public class UserDataManager
    {
        //User Loads
        public static ObservableCollection<LibraryItem> GetLibrary(string _QuasarFolderPath)
        {
            ObservableCollection<LibraryItem> Library = new ObservableCollection<LibraryItem>();

            string Path = _QuasarFolderPath + @"\Library\Library.json";

            if (File.Exists(Path))
            {
                using (StreamReader file = File.OpenText(Path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Library = (ObservableCollection<LibraryItem>)serializer.Deserialize(file, typeof(ObservableCollection<LibraryItem>));
                }
            }

            return Library;
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
        public static ObservableCollection<ModFile> GetModFiles(string _QuasarFolderPath)
        {
            ObservableCollection<ModFile> ModFiles = new ObservableCollection<ModFile>();

            string Path = _QuasarFolderPath + @"\Library\Downloads\Files.json";

            if (File.Exists(Path))
            {
                using (StreamReader file = File.OpenText(Path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    ModFiles = (ObservableCollection<ModFile>)serializer.Deserialize(file, typeof(ObservableCollection<ModFile>));
                }
            }

            return ModFiles;
        }

        //User Saves
        public static void SaveLibrary(ObservableCollection<LibraryItem> _LibraryItems, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\Library.json", _LibraryItems);
        }
        public static void SaveModInformation(LibraryItem _LibraryItem, string _QuasarModFolder)
        {
            SaveJSonFile(_QuasarModFolder + String.Format(@"\Library\Mods\{0}\ModInformation.json",_LibraryItem.Guid), _LibraryItem);
        }
        public static void SaveWorkspaces(ObservableCollection<Workspace> _Workspaces, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\Workspaces.json", _Workspaces.OrderBy(i => i.Guid));
        }
        public static void SaveContentItems(ObservableCollection<ContentItem> _ContentItems, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\ContentItems.json", _ContentItems.OrderBy(i => i.Guid));
        }
        public static void SaveModFiles(ObservableCollection<ModFile> _ModFiles, string _QuasarFolderPath)
        {
            SaveJSonFile(_QuasarFolderPath + @"\Library\Downloads\Files.json", _ModFiles);
        }
        public static void SaveSharedWorkspaces(ShareableWorkspace SW, string _DestinationFolderPath)
        {
            SaveJSonFile(_DestinationFolderPath + @"\SharedWorkspace.json", SW);
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
        /// Saves a collection to a JSon File
        /// </summary>
        /// <param name="_Fullpath">Destination file path</param>
        /// <param name="_Source">Source collection</param>
        /// <param name="_ExternalPath">Override Destination file path</param>
        private static void SaveJSonFile(string _Fullpath, Object _Source, bool Headless = false)
        {
            using (StreamWriter file = File.CreateText(_Fullpath))
            {
                JsonSerializer serializer = new JsonSerializer();
                if (Headless)
                    serializer.TypeNameHandling = TypeNameHandling.None;
                serializer.Serialize(file, _Source);
            }
        }

        //Updater Mechanism
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
    }
}
