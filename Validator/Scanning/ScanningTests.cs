using DataModels.Resource;
using DataModels.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Workshop.FileManagement;
using Workshop.Scanners;
using Xunit;

namespace Validator.Scanning
{
    /// <summary>
    /// Testing for the File Scanner
    /// </summary>
    public class ScanningTests
    {
        //Environment paths setup
        static string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Quasar";
        static string AppDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";
        static string InstallPath = @"C:\Program Files (x86)\Quasar";

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
                                GameFolderName = "luigi",
                                isDLC = false,
                                FilterValue = ""
                            },
                            new()
                            {
                                ID = 2,
                                Name = "Yoshi",
                                GameFolderName = "yoshi",
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
        static List<ScanFile> ScanFileTestData = new List<ScanFile>()
        {
            new ScanFile() { SourcePath = @"C:\Users\Test\Documents\Quasar\Library\Mods\5f649369-b260-48e5-ae28-b808590e6ba7\fighter\mario\model\body\c01\alp_mario_001_col.nutexb" },
            new ScanFile() { SourcePath = @"C:\Users\Test\Documents\Quasar\Library\Mods\5f649369-b260-48e5-ae28-b808590e6ba7\fighter\mario\model\body\c01\def_mario_001_col.nutexb" },
            new ScanFile() { SourcePath = @"C:\Users\Test\Documents\Quasar\Library\Mods\5f649369-b260-48e5-ae28-b808590e6ba7\fighter\mario\model\dokan\c03\metal_dokan_001_col.nutexb" }
        };

        #endregion

        #region Test Content Item Data

        #endregion

        #region Filtering Data
        //Data definition for Filtering tests
        static List<ScanFile> ExtensionTestData = new List<ScanFile>()
        {
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\model.numdlb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\model.numshb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c02\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c07\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\tobeignored.csv" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\tobeignored.csv" },
        };

        static List<ScanFile> FileNameTestData = new List<ScanFile>()
        {
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\model.numdlb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\model.numshb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c01\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c02\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\fighter\samus\model\body\c07\met_samus_001_col.nutexb" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\ModInformations.json" },
            new ScanFile(){ SourcePath = @"C:\TestPath\ModFolder\ModInformation.json" },
        };
        #endregion


        /// <summary>
        /// Asserts that the files with an extension specified in IgnoreExtensions gets flagged as ignored and scanned
        /// </summary>
        [Fact]
        public void Filtering_FilesWithSpecificExtensionsAreFiltered()
        {
            List<ScanFile> TestedFiles = FileScanner.FilterIgnoredFiles(ExtensionTestData);

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
            List<ScanFile> TestedFiles = FileScanner.FilterIgnoredFiles(FileNameTestData);

            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.Scanned == true);
            Assert.DoesNotContain(TestedFiles, sf => sf.Ignored == true && sf.Scanned == false);
            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.SourcePath == @"C:\TestPath\ModFolder\ModInformation.json");
            Assert.Contains(TestedFiles, sf => sf.Ignored == false && sf.SourcePath == @"C:\TestPath\ModFolder\ModInformations.json");
        }

        [Theory]
        [InlineData(@"{Empty}", @"")]
        [InlineData(@"{AnyFile}", @"(?'AnyFile'[a-zA-Z0-9\\_]*)")]
        [InlineData(@"{AnyExtension}", @"(?'AnyExtension'[a-zA-Z0-9\\_]*)")]
        [InlineData(@"{AnyPath}", @"(?'AnyPath'[a-zA-Z0-9\\_\\\\\\/]*[\\\\\\/])")]
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

        [Theory]
        [InlineData(@"C:\Users\Mowjoh\Documents\Quasar\Library\Mods\5f649369-b260-48e5-ae28-b808590e6ba7\fighter\mario\model\body\c01\alp_mario_001_col.nutexb")]
        [InlineData(@"C:\Users\Mowjoh\Documents\Quasar\Library\Mods\5f649369-b260-48e5-ae28-b808590e6ba7\test\fighter\mario\model\body\c01\alp_mario_001_col.nutexb")]
        public void Regex_FilePathRootIsProperlyFound(string _path)
        {
            string[] SplitStrings = FileScanner.GetRootFolder(_path);

            Assert.True(SplitStrings.Length > 0);
            Assert.NotEqual("",SplitStrings[0]);
            Assert.NotEqual("",SplitStrings[1]);
        }

        [Theory]
        [InlineData(@"C:\Users\Mowjoh\Documents\Quasar\Library\Mods\5f649369-b260-48e5-ae28-b808590e6ba7\fighter\mario\model\body\c01\alp_mario_001_col.nutexb")]
        [InlineData(@"C:\Users\Mowjoh\Documents\Quasar\Library\Mods\5f649369-b260-48e5-ae28-b808590e6ba7\test\fighter\mario\model\body\c01\alp_mario_001_col.nutexb")]
        public void Registration_BasicDataIsProperlyRegistered(string _source_path)
        {
            ScanFile scanFile = new ScanFile(){ FilePath = _source_path };
            scanFile = FileScanner.RegisterMatchData(scanFile, QuasarModTypes[0], QuasarModTypes[0].QuasarModTypeFileDefinitions[0]);

            Assert.True(scanFile.Scanned);
            Assert.Equal(QuasarModTypes[0].ID,scanFile.QuasarModTypeID);
            Assert.Equal(QuasarModTypes[0].QuasarModTypeFileDefinitions[0].ID, scanFile.QuasarModTypeFileDefinitionID);
            Assert.Equal(QuasarModTypes[0].ID, scanFile.GameElementID);
            Assert.Equal("00", scanFile.Slot);
        }


        [Theory]
        [InlineData("test",5,0,"","01",5)]
        [InlineData("test",5,0,"01","",5)]
        [InlineData("test",0,7,"01","",7)]
        [InlineData("test",0,7,"","01",7)]
        [InlineData("test",7,7,"","01",7)]
        [InlineData("test",7,7,"01","",7)]
        [InlineData("test",3,7,"01","",3)]
        [InlineData("test",3,7,"", "01", 3)]
        public void Registration_AdvancedDataIsProperlyRegistered(string _source_path, int _file_element, int _folder_element, string _file_slot, string _folder_slot, int expectedElementID)
        {
            GameElement fileElement = _file_element == 0 ? null : new GameElement()
            {
                Name = "FileElement",
                ID = _file_element
            };

            GameElement folderElement = _folder_element == 0 ? null : new GameElement()
            {
                Name = "FolderElement",
                ID = _folder_element
            };

            ScanFile scanFile = new ScanFile() { FilePath = _source_path };
            scanFile = FileScanner.RegisterMatchData(scanFile, QuasarModTypes[0], QuasarModTypes[0].QuasarModTypeFileDefinitions[0], fileElement, folderElement, _file_slot, _folder_slot);

            Assert.True(scanFile.Scanned);
            Assert.Equal(QuasarModTypes[0].ID, scanFile.QuasarModTypeID);
            Assert.Equal(QuasarModTypes[0].QuasarModTypeFileDefinitions[0].ID, scanFile.QuasarModTypeFileDefinitionID);
            Assert.Equal(expectedElementID, scanFile.GameElementID);
            Assert.Equal("01", scanFile.Slot);

        }

        [Theory]
        [InlineData(true, false, false, true, false, true)]
        [InlineData(false, true, false, true, false, true)]
        [InlineData(true, true, false, true, false, true)]
        [InlineData(false, false, false, true, false, false)]
        [InlineData(false, false, false, true, true, true)]
        [InlineData(false, false, true, false, true, true)]
        [InlineData(false, false, true, false, false, false)]
        [InlineData(true, false, true, false, false, true)]
        [InlineData(false, true, true, false, false, true)]
        [InlineData(true, true, true, false, false, true)]
        public void Validation_AdvancedMatchIsProperlyValidated(bool _file_element_present, bool _folder_element_present, bool _empty_folder_definition, bool folder_success, bool _no_game_element, bool expected)
        {
            Assert.Equal(expected,FileScanner.MatchIsValid(_file_element_present, _folder_element_present, _empty_folder_definition, folder_success, _no_game_element));
        }

        [Theory]
        [InlineData("mario", 0, "Mario")]
        [InlineData("luigi", 1,"Luigi")]
        [InlineData("yoshi", 2,"Yoshi")]
        [InlineData("", 0,"")]
        public void Tools_GameDataIsProperlyMatched(string _text_to_match, int _id, string _expected_element_name)
        {
            Regex regex = new Regex(@"(?'GameData'[a-zA-Z0-9\\_]*)");
            Match m = regex.Match(_text_to_match);
            GameElement ge = FileScanner.GetAssociatedGameData(m, Games[0].GameElementFamilies[0]);

            if (_text_to_match != "")
            {
                Assert.Equal(_id, ge.ID);
                Assert.Equal(_text_to_match, ge.GameFolderName);
                Assert.Equal(_expected_element_name, ge.Name);
            }
            else
            {
                Assert.Null(ge);
            }
            
        }
        [Theory]
        [InlineData(@"(?'Slot'\d{2})", @"c00", "00")]
        [InlineData(@"(?'Slot'\d{2})", @"c85", "85")]
        [InlineData(@"(?'Slot'\d{3})", @"c851", "851")]
        [InlineData(@"(?'Slot'\d{1})", @"c8", "8")]
        [InlineData(@"(?'Slot'\d{1})", @"cXX", "")]
        public void Tools_SlotIsProperlyParsed(string _slot_regex_to_use, string _text_to_match, string _expected_slot_value)
        {
            Regex regex = new Regex(_slot_regex_to_use);
            Match m = regex.Match(_text_to_match);

            string slot = FileScanner.GetSlot(m);

            Assert.Equal(_expected_slot_value, slot);
        }
    }
}
