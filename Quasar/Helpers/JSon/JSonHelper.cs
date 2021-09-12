using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using DataModels.User;
using DataModels.Resource;
using Quasar.Workspaces.Models;

namespace Quasar.Helpers.Json
{
    public static class JSonHelper
    {
        #region Saving
        //Dev Saves
        public static void SaveGamesFile(ObservableCollection<Game> _Games, string _ExternalPath = "")
        {
            if(_ExternalPath != "")
            {
                SaveJSonFile(@"\Resources\Games.json", _Games.OrderBy(i => i.ID),_ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Resources\Games.json", _Games.OrderBy(i => i.ID));
            }
            
        }
        public static void SaveQuasarModTypes(ObservableCollection<QuasarModType> _QuasarModTypes, string _ExternalPath = "")
        {
            
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Resources\ModTypes.json", _QuasarModTypes.OrderBy(i => i.TypePriority), _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Resources\ModTypes.json", _QuasarModTypes.OrderBy(i => i.TypePriority));
            }
        }
        public static void SaveModLoaders(ObservableCollection<ModLoader> _ModLoaders, string _ExternalPath = "")
        {
            
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Resources\ModLoaders.json", _ModLoaders.OrderBy(i => i.ID), _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Resources\ModLoaders.json", _ModLoaders.OrderBy(i => i.ID));
            }
        }
        public static void SaveGamebananaAPI(GamebananaAPI _API, string _ExternalPath = "")
        {

            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Resources\Gamebanana.json", _API, _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Resources\Gamebanana.json", _API);
            }
        }

        //User Saves
        public static void SaveLibrary(ObservableCollection<LibraryItem> _LibraryItems, string _ExternalPath = "")
        {
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Library\Library.json", _LibraryItems, _ExternalPath, true);
            }
            else
            {
                SaveJSonFile(@"\Library\Library.json", _LibraryItems, "" , true);
            }
            
        }
        public static void SaveWorkspaces(ObservableCollection<Workspace> _Workspaces, string _ExternalPath = "")
        {
            
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Library\Workspaces.json", _Workspaces.OrderBy(i => i.Guid), _ExternalPath, true);
            }
            else
            {
                SaveJSonFile(@"\Library\Workspaces.json", _Workspaces.OrderBy(i => i.Guid), "", true);
            }
        }
        public static void SaveSharedWorkspaces(ShareableWorkspace SW, string _ExternalPath = "")
        {

            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\SharedWorkspace.json", SW, _ExternalPath, true);
            }
            else
            {
                
                SaveJSonFile(@"\SharedWorkspace.json", SW, "", true);
            }
        }
        public static void SaveContentItems(ObservableCollection<ContentItem> _ContentItems, string _ExternalPath = "")
        {
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Library\ContentItems.json", _ContentItems.OrderBy(i => i.Guid),_ExternalPath, true);
            }
            else
            {
                SaveJSonFile(@"\Library\ContentItems.json", _ContentItems.OrderBy(i => i.Guid), "", true);
            }
        }
        public static void SaveModFiles(ObservableCollection<ModFile> _ModFiles, string _ExternalPath = "")
        {
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Library\Downloads\Files.json", _ModFiles, _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Library\Downloads\Files.json", _ModFiles);
            }
        }
        #endregion

        #region Loading

        //User Loads
        public static ObservableCollection<LibraryItem> GetLibrary(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<LibraryItem> Library = new ObservableCollection<LibraryItem>();
            string Path;
            if (!External)
            {
                Path = Properties.Settings.Default.DefaultDir + @"\Library\Library.json";
            }
            else
            {
                Path = ExternalPath;
            }
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
        public static ObservableCollection<Workspace> GetWorkspaces(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<Workspace> Workspaces = new ObservableCollection<Workspace>();
            string Path;
            if (!External)
            {
                Path = Properties.Settings.Default.DefaultDir + @"\Library\Workspaces.json";
            }
            else
            {
                Path = ExternalPath;
            }
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
        public static ShareableWorkspace GetSharedWorkspace(bool External = false, string ExternalPath = "")
        {
            ShareableWorkspace SW = new ShareableWorkspace();
            string Path;
            if (!External)
            {
                Path = Properties.Settings.Default.DefaultDir + @"\Library\Workspaces.json";
            }
            else
            {
                Path = ExternalPath;
            }
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
        public static ObservableCollection<ContentItem> GetContentItems(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<ContentItem> ContentItems = new ObservableCollection<ContentItem>();
            string Path;
            if (!External)
            {
                Path = Properties.Settings.Default.DefaultDir + @"\Library\ContentItems.json";
            }
            else
            {
                Path = ExternalPath;
            }

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
        public static ObservableCollection<ModFile> GetModFiles(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<ModFile> ModFiles = new ObservableCollection<ModFile>();
            string Path;
            if (!External)
            {
                Path = Properties.Settings.Default.DefaultDir + @"\Library\Downloads\Files.json";
            }
            else
            {
                Path = ExternalPath;
            }

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
        #endregion

        /// <summary>
        /// Saves a collection to a JSon File
        /// </summary>
        /// <param name="_Fullpath">Destination file path</param>
        /// <param name="_Source">Source collection</param>
        /// <param name="_ExternalPath">Override Destination file path</param>
        private static void SaveJSonFile(string _Fullpath, Object _Source, string _ExternalPath = "", bool Headless = false)
        {

            string Path;
            if (_ExternalPath != "")
            {
                Path = _ExternalPath;
            }
            else
            {
                Path = Properties.Settings.Default.DefaultDir + _Fullpath;
            }

            using (StreamWriter file = File.CreateText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                if (Headless)
                    serializer.TypeNameHandling = TypeNameHandling.None;
                serializer.Serialize(file, _Source);
            }
        }
    }
}
