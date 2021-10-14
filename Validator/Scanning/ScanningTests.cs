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

        /// <summary>
        /// Asserts that files from specific mods gets listed
        /// WARNING This test requires to have downloaded these mods (replace ID with the inline data) : https://gamebanana.com/mods/[ID]
        /// </summary>
        /// <param name="ModID">The Gamebanana Item ID associated to the test mod</param>
        [Theory]
        [InlineData(326774)]
        [InlineData(328164)]
        [InlineData(12942)]
        public void VerifyModFileListing(int ModID)
        {
            //Loading Library
            ObservableCollection<LibraryItem> Library = UserDataManager.GetLibrary(AppDataLocalPath);
            Assert.True(Library.Count > 0);
            
            //Getting specific LibraryItem
            LibraryItem SpecificMod = Library.SingleOrDefault(l => l.GBItem.GamebananaItemID == ModID);
            Assert.True(SpecificMod != null);

            //Setting Mod Path & getting Scan Files
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", DocumentsPath, SpecificMod.Guid);
            ObservableCollection<ScanFile> FilesToScan = FileScanner.GetScanFiles(ModFolder);

            //Validating that there are files to be further processed
            Assert.True(FilesToScan.Count > 0);
        }

        /// <summary>
        /// Asserts that all files of a specific mod are properly scanned, identified or filtered out
        /// WARNING This test requires to have downloaded these mods (replace ID with the inline data) : https://gamebanana.com/mods/[ID]
        /// </summary>
        /// <param name="ModID">The Gamebanana Item ID associated to the test mod</param>
        [Theory]
        [InlineData(326774)]
        [InlineData(328164)]
        [InlineData(12942)]
        public void VerifyModFilesScanning(int ModID)
        {
            //Loading Library, Game and QuasarModTypes
            ObservableCollection<LibraryItem> Library = UserDataManager.GetLibrary(AppDataLocalPath);
            ObservableCollection<Game> Games = ResourceManager.GetGames();
            ObservableCollection<QuasarModType> QuasarModTypes = ResourceManager.GetQuasarModTypes();

            Assert.True(Library.Count > 0);

            //Getting specific LibraryItem
            LibraryItem SpecificMod = Library.SingleOrDefault(l => l.GBItem.GamebananaItemID == ModID);
            Assert.True(SpecificMod != null);

            //Setting Mod Path & getting Scan Files
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", DocumentsPath, SpecificMod.Guid);
            ObservableCollection<ScanFile> FilesToScan = FileScanner.GetScanFiles(ModFolder);
            
            //Filtering files
            FilesToScan = FileScanner.FilterIgnoredFiles(FilesToScan);
            Assert.DoesNotContain(FilesToScan, sf => sf.Scanned == false && sf.Ignored == true);

            //Launching scan process
            FilesToScan = FileScanner.MatchScanFiles(FilesToScan, QuasarModTypes, Games[0], ModFolder);

            //Outputing results for debugging
            FileScanner.OutputScanTests(FilesToScan, SpecificMod.GBItem.GamebananaItemID);

            //Every file must be scanned
            Assert.DoesNotContain(FilesToScan, sf => sf.Scanned == false);
        }

        /// <summary>
        /// Asserts that the Content Items are properly processed from the specific mods
        /// WARNING This test requires to have downloaded these mods (replace ID with the inline data) : https://gamebanana.com/mods/[ID]
        /// </summary>
        /// <param name="ModID">The Gamebanana Item ID associated to the test mod</param>
        [Theory]
        [InlineData(326774)]
        [InlineData(328164)]
        [InlineData(12942)]
        public void VerifyLibraryModScanning(int ModID)
        {
            //Loading Library, Game and QuasarModTypes
            ObservableCollection<LibraryItem> Library = UserDataManager.GetLibrary(AppDataLocalPath);
            ObservableCollection<Game> Games = ResourceManager.GetGames();
            ObservableCollection<QuasarModType> QuasarModTypes = ResourceManager.GetQuasarModTypes();
            Assert.True(Library.Count > 0);

            //Getting specific LibraryItem
            LibraryItem SpecificMod = Library.SingleOrDefault(l => l.GBItem.GamebananaItemID == ModID);
            Assert.True(SpecificMod != null);

            //Setting Mod Path
            string ModFolder = String.Format(@"{0}\Library\Mods\{1}\", DocumentsPath, SpecificMod.Guid);

            //Getting content ites
            ObservableCollection<ContentItem> ParsedContents = FileScanner.ScanMod(ModFolder, QuasarModTypes, Games[0], SpecificMod);

            Assert.True(ParsedContents.Count > 0);
        }

        /// <summary>
        /// Asserts that the files with an extension specified in IgnoreExtensions gets flagged as ignored and scanned
        /// </summary>
        [Fact]
        public void VerifyExtensionFiltering()
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
        public void VerifyFilenameFiltering()
        {
            ObservableCollection<ScanFile> TestedFiles = FileScanner.FilterIgnoredFiles(FileNameTestData);

            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.Scanned == true);
            Assert.DoesNotContain(TestedFiles, sf => sf.Ignored == true && sf.Scanned == false);
            Assert.Contains(TestedFiles, sf => sf.Ignored == true && sf.SourcePath == @"C:\TestPath\ModFolder\ModInformation.json");
        }
        
        /// <summary>
        /// Verifies that the flags in an input string gets properly processed to a real working regex
        /// </summary>
        /// <param name="InputRule">A Quasar Mod Type File or Folder definition</param>
        /// <param name="ExpectedOutput">The output string expected after processing</param>
        [Theory]
        [InlineData(@"chara_{Part}_{GameData}_{S00}.bntx", @"chara_(?'Part'[a-zA-Z0-9]*)_(?'GameData'[a-zA-Z0-9\\_]*)_(?'Slot'\d{2})\.bntx")]
        [InlineData(@"fighter/{GameData}/model/{Folder}/c{S000}", @"fighter/(?'GameData'[a-zA-Z0-9\\_]*)/model/(?'Folder'[^\\\\\\/]*)/c(?'Slot'\d{3})")]
        [InlineData(@"{AnyFile}.eff", @"(?'AnyFile'[a-zA-Z0-9\\_]*)\.eff")]
        [InlineData(@"{Empty}", @"")]
        public void VerifyRuleRegexPreparation(string InputRule, string ExpectedOutput)
        {
            string ProcessedRule = FileScanner.PrepareRegex(InputRule);
            Assert.Equal(ExpectedOutput, ProcessedRule);
        }
    }
}
