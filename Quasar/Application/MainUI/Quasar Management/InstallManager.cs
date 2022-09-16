using DataModels.User;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Globalization;
using Quasar.Helpers.FileOperations;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Quasar.MainUI.ViewModels;
using log4net;
using Quasar.Common.Models;
using Workshop.FileManagement;
using Quasar.Common;

namespace Quasar.Helpers.Quasar_Management
{
    public static class InstallManager
    {
        #region Static paths
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";
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
            String InstallationPath = Properties.QuasarSettings.Default.DefaultDir;
            String AppPath = Properties.QuasarSettings.Default.AppPath;
            String LibraryPath = "\\Library\\";
            String ModsPath = "\\Library\\Mods\\";
            String DownloadsPath = "\\Library\\Downloads\\";
            String ScreenshotPath = "\\Library\\Screenshots\\";

            FileOperation.CheckCreate(InstallationPath);
            FileOperation.CheckCreate(InstallationPath + LibraryPath);
            FileOperation.CheckCreate(InstallationPath + ModsPath);
            FileOperation.CheckCreate(InstallationPath + DownloadsPath);
            FileOperation.CheckCreate(InstallationPath + ScreenshotPath);


        }

        /// <summary>
        /// Defines basic User settings
        /// </summary>
        /// <param name="ExternalPath">Install location</param>
        public static void CreateBaseUserSettings(string ExternalPath = "")
        {
            //Getting system language
            CultureInfo ci = CultureInfo.InstalledUICulture;
            Properties.QuasarSettings.Default.Language = (String)ci.Name;

            if (ExternalPath == "")
            {
                //Getting User's Documents folder for storage purposes
                String DocumentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                DocumentsFolderPath += "\\Quasar";
                Properties.QuasarSettings.Default.DefaultDir = DocumentsFolderPath;
            }
            else
            {
                Properties.QuasarSettings.Default.DefaultDir = ExternalPath;
            }

            //Getting execution path
            String AppPath = Environment.GetCommandLineArgs()[0];
            Properties.QuasarSettings.Default.AppPath = System.IO.Path.GetDirectoryName(AppPath);

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
                    Properties.QuasarSettings.Default.DefaultDir = fbd.SelectedPath;
                    Properties.QuasarSettings.Default.Save();
                    MessageBox.Show("Thanks ! It's been saved and applied. Have fun modding !");
                }
                else
                {
                    Properties.QuasarSettings.Default.DefaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Quasar";
                    Properties.QuasarSettings.Default.Save();
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
            string SourcePath = Properties.QuasarSettings.Default.DefaultDir;
            bool proceed = true;
            try
            {
                FileOperation.CheckCopyFolder(SourcePath + "\\Library", newPath + "\\Library");

                File.Copy(SourcePath + "\\Quasar.log", newPath + "\\Quasar.log", true);
            }
            catch (Exception e)
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
                Properties.QuasarSettings.Default.DefaultDir = newPath;
                Properties.QuasarSettings.Default.Save();
                Popup.UpdateModalStatus(Modal.MoveInstall, true);
            }
            else
            {
                Popup.UpdateModalStatus(Modal.MoveInstall, false);
            }
        }

        public static void ConvertScreenshot(string Source, string Destination, ILog QuasarLogger)
        {
            if (Source != "")
            {

                try
                {
                    QuasarLogger.Debug(String.Format("Copying '{0}' to {1}", Source, Destination));

                    File.Copy(Source, Destination, true);


                }
                catch (Exception e)
                {
                    QuasarLogger.Error(String.Format(e.Message));
                    QuasarLogger.Error(String.Format(e.StackTrace));
                }
            }
        }

    }
}
