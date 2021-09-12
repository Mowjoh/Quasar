using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Utilitino;
using System.IO;
using DataModels.V2;

namespace Testie
{
    public class ResourceTests
    {
        [Fact]
        public void TestResourcePresence()
        {
            //Getting Resources information from install directory
            List<string> InstallResources = ResourceManager.ListInstallRersources();

            //Validating count and names
            Assert.True(InstallResources.Count == 4);

            foreach(string Resource in InstallResources)
            {
                //Validating they're not empty files
                Assert.True(new FileInfo(Resource).Length > 0);
            }

            //Validating each of the files are present and correctly named
            Assert.Equal("Gamebanana.json", Path.GetFileName(InstallResources[0]));
            Assert.Equal("Games.json", Path.GetFileName(InstallResources[1]));
            Assert.Equal("ModLoaders.json", Path.GetFileName(InstallResources[2]));
            Assert.Equal("ModTypes.json", Path.GetFileName(InstallResources[3]));
            
        }

        [Fact]
        public void TestGamebananaResource()
        {
            GamebananaAPI Gamebanana = ResourceManager.GetGamebananaAPI();
            Assert.True(Gamebanana.Games.Count > 0);
        }
    }
}
