using Quasar.Data.V2;
using Quasar.Helpers.Tools;
using System;
using System.IO;
using System.Collections.ObjectModel;
using Quasar.Helpers.Json;
using System.Globalization;
using Quasar.Helpers.FileOperations;
using System.Windows.Forms;
using Quasar.Helpers;
using Quasar.Common.Models;

namespace Quasar.Helpers.Quasar_Management
{
    public static class InstallManager
    {
        #region Static paths
        static string LibraryPath = @"\Library\Library.json";
        static string LibraryBackupPath = @"\Backup\Library.json";
        static string ContentItemsPath = @"\Library\ContentItems.json";
        static string ContentItemsBackupPath = @"\Backup\ContentItems.json";
        static string WorkspacesPath = @"\Library\Workspaces.json";
        static string WorkspacesBackupPath = @"\Backup\Workspaces.json";
        #endregion

        /// <summary>
        /// Verifies needed folders for execution
        /// </summary>
        public static void CreateBaseFolders()
        {
            //Setting Paths
            String InstallationPath = Properties.Settings.Default.DefaultDir;
            String AppPath = Properties.Settings.Default.AppPath;
            String LibraryPath =  "\\Library\\";
            String ModsPath =  "\\Library\\Mods\\";
            String DownloadsPath =  "\\Library\\Downloads\\";
            String ScreenshotPath =  "\\Library\\Screenshots\\";
            String ResourcePath =  "\\Resources\\";

            FileOperation.CheckCreate(InstallationPath);
            FileOperation.CheckCreate(InstallationPath + LibraryPath);
            FileOperation.CheckCreate(InstallationPath + ModsPath);
            FileOperation.CheckCreate(InstallationPath + DownloadsPath);
            FileOperation.CheckCreate(InstallationPath + ScreenshotPath);

            FileOperation.CheckCopyFolder(AppPath + ResourcePath, InstallationPath + ResourcePath);
                         
            
        }

        /// <summary>
        /// Forcing references files and base Quasar Mod Types to be refreshed in the user folder
        /// </summary>
        public static void UpdateInstallation()
        {
            String ResourcePath = Properties.Settings.Default.DefaultDir + "\\Resources\\";
            String AppPath = Properties.Settings.Default.AppPath + "\\Resources\\";

            FileOperation.CheckCopyFolder(AppPath, ResourcePath, true,true);
        }

        /// <summary>
        /// Defines basic User settings
        /// </summary>
        /// <param name="ExternalPath">Install location</param>
        public static void CreateBaseUserSettings(string ExternalPath = "")
        {
            //Getting system language
            CultureInfo ci = CultureInfo.InstalledUICulture;
            Properties.Settings.Default.Language = (String)ci.Name;

            if(ExternalPath == "")
            {
                //Getting User's Documents folder for storage purposes
                String DocumentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                DocumentsFolderPath += "\\Quasar";
                Properties.Settings.Default.DefaultDir = DocumentsFolderPath;
            }
            else
            {
                Properties.Settings.Default.DefaultDir = ExternalPath;
            }

            //Getting execution path
            String AppPath = Environment.GetCommandLineArgs()[0];
            Properties.Settings.Default.AppPath = System.IO.Path.GetDirectoryName(AppPath);

        }

        /// <summary>
        /// Creates a default Workspace
        /// </summary>
        public static void CreateBaseWorkspace()
        {
            String AssociationsPath = Properties.Settings.Default.DefaultDir + @"\Library\Workspaces.json";
            if (!File.Exists(AssociationsPath))
            {
                Workspace defaultWorkspace = new Workspace() { Name = "Default", ID = IDHelper.getNewWorkspaceID(), Associations = new ObservableCollection<Association>(), BuildDate = "" };
                ObservableCollection<Workspace> DefaultFile = new ObservableCollection<Workspace>() { defaultWorkspace };
                JSonHelper.SaveWorkspaces(DefaultFile);

                Properties.Settings.Default.LastSelectedWorkspace = defaultWorkspace.ID;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Displays a Folder Browser to pick a new installation location
        /// Then saves the new default path
        /// </summary>
        public static void ChangeInstallLocationSetting()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult results = fbd.ShowDialog();

                if (results == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Properties.Settings.Default.DefaultDir = fbd.SelectedPath;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Thanks ! It's been saved and applied. Have fun modding !");
                }
                else
                {
                    Properties.Settings.Default.DefaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Quasar";
                    Properties.Settings.Default.Save();
                    MessageBox.Show("No path selected, using defaults. Don't worry you can still change later in the settings");
                }
            }
        }

        /// <summary>
        /// Moves the installation files to a new location
        /// </summary>
        /// <param name="newPath">New install folder path</param>
        public static void ChangeInstallLocation(string newPath)
        {
            string SourcePath = Properties.Settings.Default.DefaultDir;
            bool proceed = true;
            try
            {
                FileOperation.CheckCopyFolder(SourcePath + "\\Library", newPath + "\\Library");
                FileOperation.CheckCopyFolder(SourcePath + "\\Resources", newPath + "\\Resources");

                File.Copy(SourcePath + "\\Quasar.log", newPath + "\\Quasar.log", true);
            }
            catch(Exception e)
            {
                proceed = false;
            }

            if (proceed)
            {
                try
                {
                    Directory.Delete(SourcePath + "\\Library", true);
                }
                catch (Exception e)
                {
                    proceed = false;
                }
            }

            if (proceed)
            {
                Properties.Settings.Default.DefaultDir = newPath;
                Properties.Settings.Default.Save();
                ModalEvent Meuh = new ModalEvent()
                {
                    EventName = "MoveInstall",
                    Action = "LoadOK",
                };

                EventSystem.Publish<ModalEvent>(Meuh);
            }
            else
            {
                ModalEvent Meuh = new ModalEvent()
                {
                    EventName = "MoveInstall",
                    Action = "LoadKO",
                };

                EventSystem.Publish<ModalEvent>(Meuh);
            }
            

            

            

            
        }

        /// <summary>
        /// Backs up User data
        /// </summary>
        public static void BackupUserData()
        {
            string Base = Properties.Settings.Default.DefaultDir;

            FileOperation.CheckCopyFile(Base + LibraryPath, Base + LibraryBackupPath);
            FileOperation.CheckCopyFile(Base + ContentItemsPath, Base + ContentItemsBackupPath);
            FileOperation.CheckCopyFile(Base + WorkspacesPath, Base + WorkspacesBackupPath);

            Properties.Settings.Default.BackupDate = DateTime.Now;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Restores User data
        /// </summary>
        public static void RestoreUserData()
        {
            string Base = Properties.Settings.Default.DefaultDir;

            FileOperation.CheckCopyFile(Base + LibraryBackupPath, Base + LibraryPath);
            FileOperation.CheckCopyFile(Base + ContentItemsBackupPath, Base + ContentItemsPath);
            FileOperation.CheckCopyFile(Base + WorkspacesBackupPath, Base + WorkspacesPath);
        }
    }
}
