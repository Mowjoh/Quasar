using System;
using System.Collections.ObjectModel;
using System.IO;
using DataModels.Resource;
using Workshop.FileManagement;
using Xunit;

namespace Validator.Data
{
    /// <summary>
    /// Testing aimed at resource file management
    /// </summary>
    public class ResourceTests
    {
        private const string InstallDirectory = @"C:\Program Files (x86)\Quasar";
        private static string _appDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

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
            Assert.Equal("Games.json", Path.GetFileName(Resources[0]));
            Assert.Equal("ModTypes.json", Path.GetFileName(Resources[1]));

        }

        /// <summary>
        /// Validates that the Games deserializes properly and contains data
        /// </summary>
        [Fact]
        public void Loading_GamesFileLoadsAndContainsData()
        {
            ObservableCollection<Game> Resource = ResourceManager.GetGames(InstallDirectory);

            Assert.True(Resource.Count > 0);

            Assert.Equal("Smash Ultimate", Resource[0].Name);
        }

    }
}
