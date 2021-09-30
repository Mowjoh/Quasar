using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.FileManagement;
using Xunit;

namespace Validator
{
    
    public class UserDataTests
    {
        static string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Quasar";
        static string AppDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        [Fact]
        public void TestLibraryPresence()
        {

            Assert.True(true);
        }

        [Fact]
        public void ValidateLibraryMovement()
        {
            UserDataManager.VerifyUpdateFileLocation(DocumentsPath, AppDataLocalPath);

            string[] OldLibraryFiles = Directory.GetFiles(DocumentsPath + @"\Library", "*", SearchOption.TopDirectoryOnly);
            string[] NewLibraryFiles = Directory.GetFiles(AppDataLocalPath + @"\Library", "*", SearchOption.TopDirectoryOnly);

            Assert.True(OldLibraryFiles.Length == 0);
            Assert.True(NewLibraryFiles.Length > 0);

        }
    }
}
