using System;
using System.Collections.ObjectModel;
using System.IO;
using DataModels.User;
using Workshop.FileManagement;
using Xunit;

namespace Validator.Data
{
    /// <summary>
    /// Testing aimed at validating User Data
    /// </summary>
    public class UserDataTests
    {
        private static readonly string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Quasar";
        private static readonly string AppDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        /// <summary>
        /// Validates that the library file is in the right place and properly read
        /// </summary>
        [Fact]
        public void Loading_LibraryFileLoadsAndContainsData()
        {
            ObservableCollection<LibraryItem> LibraryItems = UserDataManager.GetLibrary(AppDataLocalPath);
            Assert.True(LibraryItems.Count > 0);
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
        /// Validates the functionality that moves the library file to the proper location if needed
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
        public void Backup_UserDataBackupComplete()
        {
            Assert.True(UserDataManager.BackupUserDataFiles(AppDataLocalPath));

        }
    }
}
