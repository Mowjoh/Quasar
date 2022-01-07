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
using Quasar.Helpers.ModScanning;
using log4net;
using Quasar.Helpers.Mod_Scanning;
using Quasar.Common.Models;
using Workshop.FileManagement;

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
        /// Creates a default Workspace
        /// </summary>
        public static void CreateBaseWorkspace()
        {
            String AssociationsPath = Properties.QuasarSettings.Default.DefaultDir + @"\Library\Workspaces.json";
            if (!File.Exists(AssociationsPath))
            {
                Workspace defaultWorkspace = new Workspace() { Name = "Default", Guid = Guid.NewGuid(), Associations = new ObservableCollection<Association>(), BuildDate = "" };
                ObservableCollection<Workspace> DefaultFile = new ObservableCollection<Workspace>() { defaultWorkspace };
                UserDataManager.SaveWorkspaces(DefaultFile, AppDataPath);

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
                FileOperation.CheckCopyFolder(SourcePath + "\\Resources", newPath + "\\Resources");

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
            string Base = Properties.QuasarSettings.Default.DefaultDir;

            FileOperation.CheckCopyFile(Base + LibraryPath, Base + LibraryBackupPath);
            FileOperation.CheckCopyFile(Base + ContentItemsPath, Base + ContentItemsBackupPath);
            FileOperation.CheckCopyFile(Base + WorkspacesPath, Base + WorkspacesBackupPath);

            Properties.QuasarSettings.Default.BackupDate = DateTime.Now;
            Properties.QuasarSettings.Default.Save();
        }

        /// <summary>
        /// Restores User data
        /// </summary>
        public static void RestoreUserData()
        {
            string Base = Properties.QuasarSettings.Default.DefaultDir;

            FileOperation.CheckCopyFile(Base + LibraryBackupPath, Base + LibraryPath);
            FileOperation.CheckCopyFile(Base + ContentItemsBackupPath, Base + ContentItemsPath);
            FileOperation.CheckCopyFile(Base + WorkspacesBackupPath, Base + WorkspacesPath);
        }

        /// <summary>
        /// Launches the update process from 1.5 to 2.1
        /// </summary>
        /// <param name="MUVM"></param>
        public static void LaunchUltraCleaning(MainUIViewModel MUVM)
        {
            try
            {
                MUVM.QuasarLogger.Info("Starting Ultra Clean");
                MUVM.QuasarLogger.Info("Creating base Folders");
                CreateBaseFolders();
                MUVM.QuasarLogger.Info("Creating base Workspace");
                CreateBaseWorkspace();
                MUVM.QuasarLogger.Info("Moving Old Library");
                //GetMoveOldLibrary(MUVM.QuasarLogger);


                MUVM.LoadData();
                Scannerino.ScanAllMods(MUVM);
                UserDataManager.SaveContentItems(MUVM.ContentItems, AppDataPath);

                LastCleanup();

                EventSystem.Publish<ModalEvent>(new ModalEvent()
                {
                    Action = "LoadOK",
                    EventName = "UltraCleaning"
                });
            }
            catch (Exception e)
            {
                MUVM.QuasarLogger.Error("Error While processing old files");
                MUVM.QuasarLogger.Error(e.Message);
                MUVM.QuasarLogger.Error(e.StackTrace);
            }

        }

        public static void TransferMods(ObservableCollection<LibraryItem> Library, ILog QuasarLogger)
        {
            //Source path
            string ModPath = String.Format(@"{0}\Library\Mods\SmashUltimate", Properties.QuasarSettings.Default.DefaultDir);

            //Foreach possible old mod folder
            foreach (string d in Directory.GetDirectories(ModPath))
            {
                foreach (string PreviousModFolder in Directory.GetDirectories(d, "*", SearchOption.AllDirectories))
                {
                    string ModFolder = Path.GetFileName(PreviousModFolder);
                    int GamebananaItemID = 0;
                    int.TryParse(ModFolder, out GamebananaItemID);

                    if (GamebananaItemID != 0)
                    {
                        //Trying to match to a parsed Library Item
                        LibraryItem li = Library.SingleOrDefault(l => l.GBItem.GamebananaItemID == GamebananaItemID);
                        if (li != null)
                        {

                            string NewModFolder = String.Format(@"{0}\Library\Mods\{1}", Properties.QuasarSettings.Default.DefaultDir, li.Guid);
                            //Copying Files
                            FileOperation.CopyFolder(PreviousModFolder, NewModFolder, true);

                            string[] Screenshots = Directory.GetFiles(String.Format(@"{0}\Library\Screenshots\", Properties.QuasarSettings.Default.DefaultDir), "*");
                            string PreviousScreenFilePath = Screenshots.SingleOrDefault(s => s.Contains(ModFolder));
                            FileInfo fi = new FileInfo(PreviousScreenFilePath);
                            string NewScreenFilePath = String.Format(@"{0}\Library\Screenshots\{1}.{2}", Properties.QuasarSettings.Default.DefaultDir, li.Guid, fi.Extension);

                            //Converting screenshot
                            ConvertScreenshot(PreviousScreenFilePath, NewScreenFilePath, QuasarLogger);
                        }
                    }
                }
            }

            Directory.Delete(ModPath, true);
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

        public static void LastCleanup()
        {
            File.Delete(String.Format(@"{0}\Library\Library.xml", Properties.QuasarSettings.Default.DefaultDir));
            File.Delete(String.Format(@"{0}\Library\ContentMapping.xml", Properties.QuasarSettings.Default.DefaultDir));
            File.Delete(String.Format(@"{0}\Library\Workspaces.xml", Properties.QuasarSettings.Default.DefaultDir));
            Directory.Delete(String.Format(@"{0}\References", Properties.QuasarSettings.Default.DefaultDir), true);
        }

        public static void Rescan(MainUIViewModel MUVM)
        {
            MUVM.ActiveWorkspace.Associations = new ObservableCollection<Association>();
            MUVM.ContentItems = new ObservableCollection<ContentItem>();

            Scannerino.ScanAllMods(MUVM);
            UserDataManager.SaveContentItems(MUVM.ContentItems, AppDataPath);

            foreach (LibraryItem li in MUVM.Library)
            {
                List<ContentItem> contentItems = MUVM.ContentItems.Where(ci => ci.LibraryItemGuid == li.Guid).ToList();
                MUVM.ActiveWorkspace = Slotter.AutomaticSlot(contentItems, MUVM.ActiveWorkspace, MUVM.QuasarModTypes);
            }

            UserDataManager.SaveWorkspaces(MUVM.Workspaces, AppDataPath);

            EventSystem.Publish<ModalEvent>(new ModalEvent()
            {
                Action = "LoadOK",
                EventName = "UltraScanning"
            });

        }
    }
}
