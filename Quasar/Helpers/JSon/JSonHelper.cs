using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quasar.Data.V2;

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
                SaveJSonFile(@"\Resources\ModTypes.json", _QuasarModTypes.OrderBy(i => i.ID), _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Resources\ModTypes.json", _QuasarModTypes.OrderBy(i => i.ID));
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

        //User Saves
        public static void SaveLibrary(ObservableCollection<LibraryItem> _LibraryItems, string _ExternalPath = "")
        {
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Library\Library.json", _LibraryItems.OrderBy(i => i.ID), _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Library\Library.json", _LibraryItems.OrderBy(i => i.ID));
            }
            
        }
        public static void SaveWorkspaces(ObservableCollection<Workspace> _Workspaces, string _ExternalPath = "")
        {
            
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Library\Workspaces.json", _Workspaces.OrderBy(i => i.ID), _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Library\Workspaces.json", _Workspaces.OrderBy(i => i.ID));
            }
        }
        public static void SaveContentItems(ObservableCollection<ContentItem> _ContentItems, string _ExternalPath = "")
        {
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Library\ContentItems.json", _ContentItems.OrderBy(i => i.ID),_ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Library\ContentItems.json", _ContentItems.OrderBy(i => i.ID));
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
        //Resource Loads
        public static ObservableCollection<Game> GetGames(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<Game> Games = new ObservableCollection<Game>();
            string Path;
            if (!External)
            {
                Path = Properties.Settings.Default.DefaultDir + @"\Resources\Games.json";
            }
            else
            {
                Path = ExternalPath;
            }
            

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                Games = (ObservableCollection<Game>)serializer.Deserialize(file, typeof(ObservableCollection<Game>));
            }

            return Games;
        }
        public static ObservableCollection<QuasarModType> GetQuasarModTypes(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<QuasarModType> QuasarModTypes = new ObservableCollection<QuasarModType>();
            string Path;
            if (!External)
            {
                Path = Properties.Settings.Default.DefaultDir + @"\Resources\ModTypes.json";
            }
            else
            {
                Path = ExternalPath;
            }

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                QuasarModTypes = (ObservableCollection<QuasarModType>)serializer.Deserialize(file, typeof(ObservableCollection<QuasarModType>));
            }

            return QuasarModTypes;
        }
        public static ObservableCollection<ModLoader> GetModLoaders(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<ModLoader> ModLoaders = new ObservableCollection<ModLoader>();
            string Path;
            if (!External)
            {
                Path = Properties.Settings.Default.DefaultDir + @"\Resources\ModLoaders.json";
            }
            else
            {
                Path = ExternalPath;
            }

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                ModLoaders = (ObservableCollection<ModLoader>)serializer.Deserialize(file, typeof(ObservableCollection<ModLoader>));
            }


            return ModLoaders;
        }

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

        private static void SaveJSonFile(string _Fullpath, Object _Source, string _ExternalPath = "")
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
                serializer.Serialize(file, _Source);
            }
        }
    }
}
