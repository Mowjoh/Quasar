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
        static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        //Resource Saves
        
        public static void SaveGamesFile(ObservableCollection<Game> _Games, string SpecificFolder)
        {
            SaveJSonFile(SpecificFolder + @"\Games.json", _Games.OrderBy(i => i.ID));
        }
        public static void SaveQuasarModTypes(ObservableCollection<QuasarModType> _QuasarModTypes, string SpecificFolder)
        {
            SaveJSonFile(SpecificFolder+ @"\Resources\ModTypes.json", _QuasarModTypes.OrderBy(i => i.TypePriority));
        }

        //Resource Loads
        public static ObservableCollection<Game> GetGames(string InstallDirectory)
        {
            ObservableCollection<Game> Games = new ObservableCollection<Game>();
            string Path = InstallDirectory + @"\Resources\Games.json";

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                Games = (ObservableCollection<Game>)serializer.Deserialize(file, typeof(ObservableCollection<Game>));
            }

            return Games;
        }
        public static ObservableCollection<QuasarModType> GetQuasarModTypes(string InstallDirectory)
        {
            ObservableCollection<QuasarModType> QuasarModTypes = new ObservableCollection<QuasarModType>();
            string Path = InstallDirectory + @"\Resources\ModTypes.json";

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                QuasarModTypes = (ObservableCollection<QuasarModType>)serializer.Deserialize(file, typeof(ObservableCollection<QuasarModType>));
            }

            return QuasarModTypes;
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
                if(!LocalRC.SubCategories.Any(c => c.Guid == RootCategory.SubCategories[0].Guid))
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
