using DataModels.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshop.FileManagement;
using Xunit;

namespace Testing_Data
{
    /// <summary>
    /// Testing aimed at validating User Data
    /// </summary>
    public class UserDataTests
    {
        static string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Quasar";
        static string AppDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        /// <summary>
        /// Validates that the library file is in the right place and properly read
        /// </summary>
        [Fact]
        public void Loading_LibraryFileLoadsAndContainsData()
        {
            ObservableCollection<LibraryItem> Library = UserDataManager.GetLibrary(AppDataLocalPath);
            Assert.True(Library.Count > 0);
        }
        [Fact]
        public void Loading_WorkspaceFileLoadsAndContainsData()
        {
            ObservableCollection<Workspace> Workspaces = UserDataManager.GetWorkspaces(AppDataLocalPath);
            Assert.True(Workspaces.Count > 0);
        }
        [Fact]
        public void Loading_ContentItemsFileLoadsAndContainsData()
        {
            ObservableCollection<ContentItem> ContentItems = UserDataManager.GetContentItems(AppDataLocalPath);
            Assert.True(ContentItems.Count > 0);
        }

        /// <summary>
        /// Validates the functionnality that moves the library file to the proper location if needed
        /// </summary>
        [Fact]
        public void ToMove_Updates_LibraryFilesGetsMoved()
        {
            UserDataManager.VerifyUpdateFileLocation(DocumentsPath, AppDataLocalPath);

            string[] OldLibraryFiles = Directory.GetFiles(DocumentsPath + @"\Library", "*", SearchOption.TopDirectoryOnly);
            string[] NewLibraryFiles = Directory.GetFiles(AppDataLocalPath + @"\Library", "*", SearchOption.TopDirectoryOnly);

            Assert.True(OldLibraryFiles.Length == 0);
            Assert.True(NewLibraryFiles.Length > 0);

        }

        [Fact]
        public void Backup_UserDataIsBackuped()
        {
            Assert.True(UserDataManager.BackupUserDataFiles(AppDataLocalPath));

        }
    }
}
