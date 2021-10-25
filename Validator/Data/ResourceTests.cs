using DataModels.Resource;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Workshop.FileManagement;
using Xunit;

namespace Testing_Data
{
    /// <summary>
    /// Testing aimed at resource file management
    /// </summary>
    public class ResourceTests
    {
        static string InstallDirectory = @"C:\Program Files (x86)\Quasar";
        static string AppDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        /// <summary>
        /// Validates that all resource files are present and properly named
        /// </summary>
        [Fact]
        public void Installation_ResourceFilesArePresent()
        {
            //Getting Resources
            string[] Resources = Directory.GetFiles(InstallDirectory + @"\Resources", "*", SearchOption.TopDirectoryOnly);

            //Validating file size
            foreach (string ResourcePath in Resources)
            {
                Assert.True(new FileInfo(ResourcePath).Length > 0);
            }

            //Checking names
            Assert.Equal("Gamebanana.json", Path.GetFileName(Resources[0]));
            Assert.Equal("Games.json", Path.GetFileName(Resources[1]));
            Assert.Equal("ModLoaders.json", Path.GetFileName(Resources[2]));
            Assert.Equal("ModTypes.json", Path.GetFileName(Resources[3]));

        }

        /// <summary>
        /// Validates that the Gamebanana deserializes properly and contains data
        /// </summary>
        [Fact]
        public void Loading_GamebananaFileLoadsAndContainsData()
        {
            GamebananaAPI Resource = ResourceManager.GetGamebananaAPI();

            Assert.True(Resource.Games.Count > 0);

            Assert.Equal("Super Smash Bros. Ultimate", Resource.Games[0].Name);
        }

        /// <summary>
        /// Validates that the Games deserializes properly and contains data
        /// </summary>
        [Fact]
        public void Loading_GamesFileLoadsAndContainsData()
        {
            ObservableCollection<Game> Resource = ResourceManager.GetGames();

            Assert.True(Resource.Count > 0);

            Assert.Equal("Smash Ultimate", Resource[0].Name);
        }

        /// <summary>
        /// Validates that the Mod Loaders deserializes properly and contains data
        /// </summary>
        [Fact]
        public void Loading_ModLoadersFileLoadsAndContainsData()
        {
            ObservableCollection<ModLoader> Resource = ResourceManager.GetModLoaders();

            Assert.True(Resource.Count == 3);

            Assert.Equal("ARCropolis", Resource[0].Name);
            Assert.Equal("ARCadia style", Resource[1].Name);
            Assert.Equal("UMM", Resource[2].Name);
        }

        /// <summary>
        /// Validates that the Mod Types deserializes properly and contains data
        /// </summary>
        [Fact]
        public void Loading_ModTypesFileLoadsAndContainsData()
        {
            ObservableCollection<QuasarModType> Resource = ResourceManager.GetQuasarModTypes();
            
            Assert.True(Resource.Count > 0);
        }

    }
}
