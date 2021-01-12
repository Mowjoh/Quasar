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
        public static void SaveGamesFile(ObservableCollection<Game> _Games)
        {

            SaveJSonFile(@"\Resources\Sources\Games.json", _Games.OrderBy(i => i.ID));
        }
        public static void SaveQuasarModTypes(ObservableCollection<QuasarModType> _QuasarModTypes)
        {
            SaveJSonFile(@"\Resources\Sources\ModTypes.json", _QuasarModTypes.OrderBy(i => i.ID));
        }
        public static void SaveModLoaders(ObservableCollection<ModLoader> _ModLoaders)
        {
            SaveJSonFile(@"\Resources\Sources\ModLoaders.json", _ModLoaders.OrderBy(i => i.ID));
        }

        //User Saves
        public static void SaveLibrary(ObservableCollection<LibraryItem> _LibraryItems)
        {
            SaveJSonFile(@"\Library\Library.json", _LibraryItems.OrderBy(i => i.ID));
        }
        public static void SaveWorkspaces(ObservableCollection<Workspace> _Workspaces)
        {
            SaveJSonFile(@"\Library\Workspaces.json", _Workspaces.OrderBy(i => i.ID));
        }
        public static void SaveContentItems(ObservableCollection<ContentItem> _ContentItems)
        {
            SaveJSonFile(@"\Library\ContentItems.json", _ContentItems.OrderBy(i => i.ID));
        }
        #endregion

        #region Loading
        //Resource Loads
        public static ObservableCollection<Game> GetGames()
        {
            ObservableCollection<Game> Games = new ObservableCollection<Game>();
            string Path = Properties.Settings.Default.DefaultDir + @"\Resources\Sources\Games.json";

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                Games = (ObservableCollection<Game>)serializer.Deserialize(file, typeof(ObservableCollection<Game>));
            }

            return Games;
        }
        public static ObservableCollection<QuasarModType> GetQuasarModTypes()
        {
            ObservableCollection<QuasarModType> QuasarModTypes = new ObservableCollection<QuasarModType>();
            string Path = Properties.Settings.Default.DefaultDir + @"\Resources\Sources\ModTypes.json";

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                QuasarModTypes = (ObservableCollection<QuasarModType>)serializer.Deserialize(file, typeof(ObservableCollection<QuasarModType>));
            }

            return QuasarModTypes;
        }
        public static ObservableCollection<ModLoader> GetModLoaders()
        {
            ObservableCollection<ModLoader> ModLoaders = new ObservableCollection<ModLoader>();
            string Path = Properties.Settings.Default.DefaultDir + @"\Resources\Sources\ModLoaders.json";

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                ModLoaders = (ObservableCollection<ModLoader>)serializer.Deserialize(file, typeof(ObservableCollection<ModLoader>));
            }


            return ModLoaders;
        }

        //User Loads
        public static ObservableCollection<LibraryItem> GetLibrary()
        {
            ObservableCollection<LibraryItem> Library = new ObservableCollection<LibraryItem>();
            string Path = Properties.Settings.Default.DefaultDir + @"\Library\Library.json";

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                Library = (ObservableCollection<LibraryItem>)serializer.Deserialize(file, typeof(ObservableCollection<LibraryItem>));
            }


            return Library;
        }
        public static ObservableCollection<Workspace> GetWorkspaces()
        {
            ObservableCollection<Workspace> Workspaces = new ObservableCollection<Workspace>();
            string Path = Properties.Settings.Default.DefaultDir + @"\Library\Workspaces.json";

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                Workspaces = (ObservableCollection<Workspace>)serializer.Deserialize(file, typeof(ObservableCollection<Workspace>));
            }


            return Workspaces;
        }
        public static ObservableCollection<ContentItem> GetContentItems()
        {
            ObservableCollection<ContentItem> ContentItems = new ObservableCollection<ContentItem>();
            string Path = Properties.Settings.Default.DefaultDir + @"\Library\ContentItems.json";

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                ContentItems = (ObservableCollection<ContentItem>)serializer.Deserialize(file, typeof(ObservableCollection<ContentItem>));
            }


            return ContentItems;
        }
        #endregion

        private static void SaveJSonFile(string _Fullpath, Object _Source)
        {
            string Path = Properties.Settings.Default.DefaultDir + _Fullpath;
            using (StreamWriter file = File.CreateText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, _Source);
            }
        }
    }
}
