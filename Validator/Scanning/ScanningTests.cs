using DataModels.Resource;
using DataModels.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.FileManagement;
using Workshop.Scanners;
using Xunit;

namespace Testing_Scanner
{
    /// <summary>
    /// Testing for the File Scanner
    /// </summary>
    public class ScanningTests
    {
        //Environment paths setup
        static string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Quasar";
        static string AppDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        #region Test Game Data
        public ObservableCollection<Game> Games = new()
        {
            new()
            {
                ID = 0,
                Name = "Test Game",
                APIGameName = "API Test Game",
                GameElementFamilies = new()
                {
                    new()
                    {
                        Name = "Characters",
                        ID = 0,
                        GameElements = new()
                        {
                            new()
                            {
                                ID = 0,
                                Name = "Mario",
                                GameFolderName = "mario",
                                isDLC = false,
                                FilterValue = ""
                            },
                            new()
                            {
                                ID = 1,
                                Name = "Luigi",
                                GameFolderName = "mario",
                                isDLC = false,
                                FilterValue = ""
                            },
                            new()
                            {
                                ID = 2,
                                Name = "Yoshi",
                                GameFolderName = "mario",
                                isDLC = false,
                                FilterValue = ""
                            },
                        }
                    }
                }
                
           }
        };
        #endregion

        #region Test Quasar Mod Types Data
        public ObservableCollection<QuasarModType> QuasarModTypes = new()
        {
            new()
            {
                ID = 0,
                TypePriority = 10,
                Name = "Fighter Skin",
                ExternalFolderPath = "",
                GameElementFamilyID = 0,
                GroupName = "Fighter",
                IgnoreGameElementFamily = false,
                IsExternal = false,
                NoGameElement = false,
                SlotCount = 8,
                QuasarModTypeFileDefinitions = new()
                {
                    new()
                    {
                        ID = 0,
                        FilePriority = 10,
                        SearchFileName = "",
                        SearchPath = ""
                    }
                }
            }
        };
        #endregion

        #region Test Library Data
        static ObservableCollection<LibraryItem> TestLibrary = new ObservableCollection<LibraryItem>()
        {
            new LibraryItem()
            {
                GameID = 0,
                Time = DateTime.Now,
                Guid = Guid.NewGuid(),
                Name = "01_TestMod",
                GBItem = new GamebananaItem()
                {
                    Name = "01_TestMod",
                    GamebananaItemID = 150000,
                    GameName = "Super Smash Bros. Ultimate",
                    UpdateCount = 0,
                    RootCategoryGuid = Guid.NewGuid(),
                    SubCategoryGuid = Guid.NewGuid()
                }

            },
            new LibraryItem()
            {
                GameID = 0,
                Time = DateTime.Now,
                Guid = Guid.NewGuid(),
                Name = "02_TestMod",
                GBItem = new GamebananaItem()
                {
                    Name = "02_TestMod",
                    GamebananaItemID = 150000,
                    GameName = "Super Smash Bros. Ultimate",
                    UpdateCount = 0,
                    RootCategoryGuid = Guid.NewGuid(),
                    SubCategoryGuid = Guid.NewGuid()
                }

            },
            new LibraryItem()
            {
                GameID = 0,
                Time = DateTime.Now,
                Guid = Guid.NewGuid(),
                Name = "03_TestMod",
                GBItem = new GamebananaItem()
                {
                    Name = "03_TestMod",
                    GamebananaItemID = 150000,
                    GameName = "Super Smash Bros. Ultimate",
                    UpdateCount = 0,
                    RootCategoryGuid = Guid.NewGuid(),
                    SubCategoryGuid = Guid.NewGuid()
                }

            },
            new LibraryItem()
            {
                GameID = 0,
                Time = DateTime.Now,
                Guid = Guid.NewGuid(),
                Name = "04_TestMod",
                GBItem = new GamebananaItem()
                {
                    Name = "04_TestMod",
                    GamebananaItemID = 150000,
                    GameName = "Super Smash Bros. Ultimate",
                    UpdateCount = 0,
                    RootCategoryGuid = Guid.NewGuid(),
                    SubCategoryGuid = Guid.NewGuid()
                }

            },
            new LibraryItem()
            {
                GameID = 0,
                Time = DateTime.Now,
                Guid = Guid.NewGuid(),
                Name = "05_TestMod",
                GBItem = new GamebananaItem()
                {
                    Name = "05_TestMod",
                    GamebananaItemID = 150000,
                    GameName = "Super Smash Bros. Ultimate",
                    UpdateCount = 0,
                    RootCategoryGuid = Guid.NewGuid(),
                    SubCategoryGuid = Guid.NewGuid()
                }

            }
        };
        #endregion

        #region Test ScanFile Data
        static ObservableCollection<ScanFile> ScanFileTestData = new ObservableCollection<ScanFile>()
        {
            new ScanFile() { SourcePath = @"DummyLocation" },
            new ScanFile() { SourcePath = @"DummyLocation" },
            new ScanFile() { SourcePath = @"DummyLocation" }
        };

        #endregion

        #region Test Content Item Data

        #endregion

        #region Filtering Data
        //Data definition for Filtering tests
        static ObservableCollection<ScanFile> ExtensionTestData = new ObservableCollection<ScanFile>()
        {
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\model.numdlb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\model.numshb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c02\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c07\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\tobeignored.csv" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\tobeignored.csv" },
        };

        static ObservableCollection<ScanFile> FileNameTestData = new ObservableCollection<ScanFile>()
        {
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\model.numdlb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\model.numshb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c02\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c07\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\ModInformation.json" },
        };
        #endregion

        [Fact]
        public void Scanning_Specific_FilesAreFlaggedAsScanned()
        {
            ObservableCollection<Game> Games = ResourceManager.GetGames();
            ObservableCollection<QuasarModType> QuasarModTypes = ResourceManager.GetQuasarModTypes();
            string ModFolder = "";

            ////Launching scan process
            //FilesToScan = FileScanner.MatchScanFiles(FilesToScan, QuasarModTypes, Games[0], ModFolder);

            ////Every file must be scanned
            //Assert.DoesNotContain(FilesToScan, sf => sf.Scanned == false);
        }

        /// <summary>
        /// Asserts that the files with an extension specified in IgnoreExtensions gets flagged as ignored and scanned
        /// </summary>
        [Fact]
        public void Filtering_FilesWithSpecificExtensionsAreFiltered()
        {
            ObservableCollection<ScanFile> TestedFiles = FileScanner.FilterIgnoredFiles(ExtensionTestData);

            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.Scanned == true);
            Assert.DoesNotContain(TestedFiles, sf => sf.Ignored == true && sf.Scanned == false);
            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.SourcePath == @"C:\TestPath\ModFolder\tobeignored.csv");
            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.SourcePath == @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\tobeignored.csv");
        }

        /// <summary>
        /// Asserts that the files with a name specified in IgnoreFiles gets flagged as ignored and scanned
        /// </summary>
        [Fact]
        public void Filtering_FilesWithSpecificFilenamesAreFiltered()
        {
            ObservableCollection<ScanFile> TestedFiles = FileScanner.FilterIgnoredFiles(FileNameTestData);

            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.Scanned == true);
            Assert.DoesNotContain(TestedFiles, sf => sf.Ignored == true && sf.Scanned == false);
            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.SourcePath == @"C:\TestPath\ModFolder\ModInformation.json");
        }

        [Theory]
        [InlineData(@"{Empty}", @"")]
        [InlineData(@"{AnyFile}", @"(?'AnyFile'[a-zA-Z0-9\\_]*)")]
        [InlineData(@"{Folder}", @"(?'Folder'[^\\\\\\/]*)")]
        [InlineData(@"{Part}", @"(?'Part'[a-zA-Z0-9]*)")]
        [InlineData(@"{GameData}", @"(?'GameData'[a-zA-Z0-9\\_]*)")]
        public void Regex_FlagsAreProperlyConverted(string flag, string flagOutput)
        {
            string ProcessedRule = FileScanner.PrepareRegex(flag);
            Assert.Equal(flagOutput, ProcessedRule);
        }

        [Theory]
        [InlineData(@"{S0}", @"(?'Slot'\d{1})")]
        [InlineData(@"{S00}", @"(?'Slot'\d{2})")]
        [InlineData(@"{S000}", @"(?'Slot'\d{3})")]
        public void Regex_SlotFlagsAreProperlyConverted(string flag, string flagOutput)
        {
            string ProcessedRule = FileScanner.PrepareRegex(flag);
            Assert.Equal(flagOutput, ProcessedRule);
        }

        [Theory]
        [InlineData(@"a+b", @"a\+b")]
        [InlineData(@"filename.extension", @"filename\.extension")]
        [InlineData(@"folder\another", @"folder\\another")]
        public void Regex_ExtrasAreProperlyConverted(string flag, string flagOutput)
        {
            string ProcessedRule = FileScanner.PrepareRegex(flag);
            Assert.Equal(flagOutput, ProcessedRule);
        }

        
    }
}
