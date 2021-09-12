using DataModels.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Utilitino
{
    public static class ResourceManager
    {
        public static string InstallationPath = @"C:\Program Files (x86)\Quasar\";

        public static List<string> ListInstallRersources()
        {
            List<string> InstallResources = new List<string>();

            string[] ResourceFiles = Directory.GetFiles(InstallationPath + @"Resources\");
            foreach(string ResourceFile in ResourceFiles)
            {
                InstallResources.Add(ResourceFile);
            }

            return InstallResources;
        }

        //Resource Loads
        public static ObservableCollection<Game> GetGames(bool External = false, string ExternalPath = "")
        {
            ObservableCollection<Game> Games = new ObservableCollection<Game>();
            string Path;
            if (!External)
            {
                Path = InstallationPath + @"Resources\Games.json";
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
                Path = InstallationPath + @"Resources\ModTypes.json";
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
                Path = InstallationPath + @"Resources\ModLoaders.json";
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
                Path = InstallationPath + @"Resources\Gamebanana.json";
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

    public class QuasarResource
    {
        public string ResourceName { get; set; }
        public int ResourceVersion { get; set; }
        public long FileSize { get; set; }

        public QuasarResource(string FilePath)
        {
            String FileName = Path.GetFileName(FilePath);
            FileSize = new FileInfo(FilePath).Length;
            GetValuesFromFileName(FileName);
        }

        public void GetValuesFromFileName(string FileName)
        {
            string FileNameWithoutExtension = FileName.Split('.')[0];
            if (!FileNameWithoutExtension.Contains('_'))
            {
                ResourceName = FileNameWithoutExtension;
                ResourceVersion = 0;
            }
            else
            {
                ResourceName = FileNameWithoutExtension.Split('_')[0];
                ResourceVersion = int.Parse(FileNameWithoutExtension.Split('_')[0]);
            }

        }
    }

}
