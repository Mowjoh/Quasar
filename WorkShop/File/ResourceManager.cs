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
            if (!External)
            {
                Path = InstallDirectory + @"\Resources\Gamebanana.json";
            }
            else
            {
                Path = ExternalPath;
            }

            using (StreamReader file = File.OpenText(Path))
            {
                JsonSerializer serializer = new JsonSerializer();
                API = (GamebananaAPI)serializer.Deserialize(file, typeof(GamebananaAPI));
            }


            return API;
        }
    }
}
