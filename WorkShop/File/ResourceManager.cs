using DataModels.User;
using DataModels.Resource;
using DataModels.Common;
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
    public class ResourceManager
    {
        static string InstallDirectory = @"C:\Program Files (x86)\Quasar";
        static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        //Resource Saves
        public static void SaveGamebananaAPI(GamebananaAPI _API, string _QuasarFolderPath)
        {
            if (!Directory.Exists(_QuasarFolderPath + @"\Resources\"))
                Directory.CreateDirectory(_QuasarFolderPath + @"\Resources\");

            SaveJSonFile(_QuasarFolderPath + @"\Resources\Gamebanana.json", _API);
        }
        public static void SaveGamesFile(ObservableCollection<Game> _Games, string SpecificFolder)
        {
            SaveJSonFile(SpecificFolder + @"\Games.json", _Games.OrderBy(i => i.ID));
        }
        public static void SaveQuasarModTypes(ObservableCollection<QuasarModType> _QuasarModTypes, string _ExternalPath = "")
        {
            /*
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Resources\ModTypes.json", _QuasarModTypes.OrderBy(i => i.TypePriority), _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Resources\ModTypes.json", _QuasarModTypes.OrderBy(i => i.TypePriority));
            }*/
        }
        public static void SaveModLoaders(ObservableCollection<ModLoader> _ModLoaders, string _ExternalPath = "")
        {
            /*
            if (_ExternalPath != "")
            {
                SaveJSonFile(@"\Resources\ModLoaders.json", _ModLoaders.OrderBy(i => i.ID), _ExternalPath);
            }
            else
            {
                SaveJSonFile(@"\Resources\ModLoaders.json", _ModLoaders.OrderBy(i => i.ID));
            }*/
        }

        //Resource Loads
        public static ObservableCollection<Game> GetGames(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<Game> Games = new ObservableCollection<Game>();
            string Path;
            if (!External)
            {
                Path = InstallDirectory + @"\Resources\Games.json";
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
                Path = InstallDirectory + @"\Resources\ModTypes.json";
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
                Path = InstallDirectory + @"\Resources\ModLoaders.json";
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
        public static GamebananaAPI GetGamebananaAPI(bool External = false, string ExternalPath = "")
        {
            GamebananaAPI API = new GamebananaAPI();
            string Path;
            if (!File.Exists(AppDataPath + @"\Resources\Gamebanana.json"))
            {
                Path = InstallDirectory + @"\Resources\Gamebanana.json";
            }
            else
            {
                Path = AppDataPath + @"\Resources\Gamebanana.json";
            }

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                API = (GamebananaAPI)serializer.Deserialize(file, typeof(GamebananaAPI));
            }


            return API;
        }

        /// <summary>
        /// Saves a collection to a JSon File
        /// </summary>
        /// <param name="_Fullpath">Destination file path</param>
        /// <param name="_Source">Source collection</param>
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

        public static GamebananaAPI UpdateGamebananaAPI(GamebananaRootCategory RootCategory, GamebananaAPI API)
        {
            if(!API.Games[0].RootCategories.Any(c => c.Guid == RootCategory.Guid))
            {
                API.Games[0].RootCategories.Add(RootCategory);
            }
            else
            {
                GamebananaRootCategory LocalRC = API.Games[0].RootCategories.Single(c => c.Guid == RootCategory.Guid);
                if(LocalRC.SubCategories.Any(c => c.Guid == RootCategory.SubCategories[0].Guid))
                {
                    API.Games[0].RootCategories.Single(c => c.Guid == RootCategory.Guid).SubCategories.Add(RootCategory.SubCategories[0]);
                }
            }


            return API;
        }
    }
}
