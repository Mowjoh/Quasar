using DataModels.User;
using DataModels.Resource;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Workshop.FileManagement
{
    public class ResourceManager
    {
        static string InstallDirectory = @"C:\Program Files (x86)\Quasar";
        static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        //Resource Saves
        /// <summary>
        /// Saves the Gamebanana API resource file to the specific path
        /// </summary>
        /// <param name="_API">The resource to save</param>
        /// <param name="_AppDataPath">the user's App Data Path</param>
        public static void SaveGamebananaAPI(GamebananaAPI _API, string _AppDataPath)
        {
            if (!Directory.Exists(_AppDataPath + @"\Resources\"))
                Directory.CreateDirectory(_AppDataPath + @"\Resources\");

            SaveJSonFile(_AppDataPath + @"\Resources\Gamebanana.json", _API);
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
        public static GamebananaAPI GetGamebananaAPI(string QuasarPath = "")
        {
            GamebananaAPI API = new GamebananaAPI();

            string Path = QuasarPath + @"\Resources\Gamebanana.json";
            if (!File.Exists(Path))
            {
                Path = InstallDirectory + @"\Resources\Gamebanana.json";
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
        public static GamebananaAPI CleanGamebananaAPIFile(GamebananaAPI API, ObservableCollection<LibraryItem> Library)
        {
            GamebananaAPI NewAPI = new()
            {
                Games = new()
                {
                    new()
                    {
                        Guid = API.Games[0].Guid,
                        ID = API.Games[0].ID,
                        Name = API.Games[0].Name,
                        RootCategories = new()
                    }
                }
            };

            foreach(GamebananaRootCategory RC in API.Games[0].RootCategories)
            {
                if(Library.Any(li => li.GBItem?.RootCategoryGuid == RC.Guid))
                {
                    GamebananaRootCategory NewRootCategory = new()
                    {
                        Guid = RC.Guid,
                        Name = RC.Name,
                        SubCategories = new()
                    };

                    foreach(LibraryItem li in Library.Where(l => l.GBItem?.RootCategoryGuid == RC.Guid).ToList())
                    {
                        if(!NewRootCategory.SubCategories.Any(SC => SC.Guid == li.GBItem?.SubCategoryGuid))
                        {
                            GamebananaSubCategory MatchingSubCat = RC.SubCategories.Single(sc => sc.Guid == li.GBItem?.SubCategoryGuid);
                            NewRootCategory.SubCategories.Add(new()
                            {
                                Guid = MatchingSubCat.Guid,
                                ID = MatchingSubCat.ID,
                                Name = MatchingSubCat.Name
                            });
                        }
                    }

                    NewAPI.Games[0].RootCategories.Add(NewRootCategory);
                }
            }

            return NewAPI;
        }
    }
}
