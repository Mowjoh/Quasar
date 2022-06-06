using log4net;
using log4net.Appender;
using Quasar.Helpers.Quasar_Management;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using DataModels.Resource;
using DataModels.User;
using Microsoft.VisualBasic.ApplicationServices;
using Quasar.Common.Models;
using Quasar.Helpers;
using Workshop.FileManagement;
using Workshop.Updates;

namespace Quasar.Internal.Tools
{
    public static class UpdateCommander
    {
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Quasar";

        /// <summary>
        /// Runs checks and updates if necessary
        /// </summary>
        /// <returns></returns>
        public static UpdateStatus CheckUpdateStatus(ILog QuasarLogger)
        {
            //If the loaded conf file is a new update file
            if (Properties.QuasarSettings.Default.UpgradeRequired)
            {
                //Making file up to date with previous data
                UpgradeSettingFile();

                //If it's Quasar's first launch
                if (InitialSetupRequested)
                {
                    

                    QuasarLogger.Debug("First Boot detected");
                    return UpdateStatus.FirstBoot;

                }

                //An updated version was detected
                QuasarLogger.Debug("Update detected");
                return UpdateStatus.ToUpdate;
            }

            QuasarLogger.Debug("No Update");
            return UpdateStatus.Regular;
        }

        public static UpdateStatus CheckPreviousInstall(ILog QuasarLogger, UpdateStatus prev_status, string PathToSearch)
        {
            if (prev_status == UpdateStatus.FirstBoot)
            {
                //If files are already there and valid
                if (Updater.CheckForRecoverableData(PathToSearch))
                {
                    QuasarLogger.Debug("previous install detected");
                    return UpdateStatus.PreviouslyInstalled;
                }
            }

            return prev_status;
        }

        public static void LaunchFirstBootSequence()
        {
            InstallManager.CreateBaseUserSettings();

            MessageBoxResult result = System.Windows.MessageBox.Show("Hi ! It seems it's Quasar's first launch. Do you want to change where Quasar is gonna store mods? If you're updating from 2.1 please select your Quasar Folder if you've changed it", "First Launch Warning", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    InstallManager.ChangeInstallLocationSetting();
                    break;
                case MessageBoxResult.No:
                    Properties.QuasarSettings.Default.DefaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Quasar";
                    Properties.QuasarSettings.Default.Save();
                    break;
            }

            InstallManager.CreateBaseFolders();
        }

        public static void LaunchUpdateSequence(ILog QuasarLogger, ObservableCollection<LibraryItem> Library, GamebananaAPI API)
        {
            SetupLogger(QuasarLogger);
            QuasarLogger.Debug("Updating");
            if (File.Exists(Properties.QuasarSettings.Default.DefaultDir + @"\Library\Library.json"))
            {
                //Before beta update
                LaunchOldInstallChanges(Library, API);
            }


            ModalEvent Meuh = new ModalEvent()
            {
                Action = "LoadOK",
                EventName = "Updating",
                Title = Properties.Resources.MainUI_Modal_UpdateFinishedTitle,
                Content = Properties.Resources.MainUI_Modal_UpdateFinishedContent,
                OkButtonText = "OK",
                Type = ModalType.Loader
            };

            EventSystem.Publish<ModalEvent>(Meuh);
        }

        public static ObservableCollection<LibraryItem> LaunchInstallRecoverySequence(ObservableCollection<LibraryItem> Library, GamebananaAPI API, ILog QuasarLogger)
        {
            string ModsPath = Properties.QuasarSettings.Default.DefaultDir + @"\Library\Mods\";
            string[] ModFolders = Directory.GetDirectories(ModsPath, "*", SearchOption.TopDirectoryOnly);

            bool FoundRecoverableMods = false;
            QuasarLogger.Debug("Found Recoverable mods " + FoundRecoverableMods);
            if (ModFolders.Length > 0)
            {

                foreach (string ModFolder in ModFolders)
                {
                    if (File.Exists(ModFolder + @"\ModInformation.json"))
                    {
                        FoundRecoverableMods = true;
                    }
                }
            }
            if (FoundRecoverableMods)
            {
                ModalEvent meuh = new ModalEvent()
                {
                    Action = "Show",
                    EventName = "RecoverMods",
                    Title = "Recovery possible",
                    Content = "Recoverable mods have been found, please wait while Quasar loads them up",
                    OkButtonText = "OK",
                    Type = ModalType.Loader
                };

                EventSystem.Publish<ModalEvent>(meuh);
                Library = UserDataManager.RecoverMods(Properties.QuasarSettings.Default.DefaultDir, AppDataPath, Library, API, QuasarLogger);

                meuh.Action = "LoadOK";
                EventSystem.Publish<ModalEvent>(meuh);
            }

            return Library;
        }

        public static void LaunchOldInstallChanges(ObservableCollection<LibraryItem> Library, GamebananaAPI API)
        {
            ////First we load the library
            //ObservableCollection<LibraryItem> OldLibrary =
            //    UserDataManager.GetSingleFileLibrary(Properties.QuasarSettings.Default.DefaultDir);
            ////GamebananaAPI OldAPI = ResourceManager.GetGamebananaAPI(Properties.QuasarSettings.Default.DefaultDir);

            //foreach (LibraryItem LibraryItem in OldLibrary)
            //{
            //    GamebananaRootCategory Cat = OldAPI.Games[0].RootCategories
            //        .Single(c => c.Guid == LibraryItem.GBItem.RootCategoryGuid);

            //    GamebananaSubCategory SCat =
            //        Cat.SubCategories.Single(s => s.Guid == LibraryItem.GBItem.SubCategoryGuid);

            //    if (API.Games[0].RootCategories.Any(rc => rc.Guid == Cat.Guid))
            //    {
            //        GamebananaRootCategory ACat = API.Games[0].RootCategories.Single(rc => rc.Guid == Cat.Guid);
            //        if (!ACat.SubCategories.Any(sc => sc.Guid == SCat.Guid))
            //        {
            //            ACat.SubCategories.Add(new()
            //            {
            //                Guid = SCat.Guid,
            //                ID = SCat.ID,
            //                Name = SCat.Name
            //            });
            //        }
            //    }
            //    else
            //    {
            //        API.Games[0].RootCategories.Add(new()
            //        {
            //            Guid = Cat.Guid,
            //            Name = Cat.Name,
            //            SubCategories = new()
            //            {
            //                new()
            //                {
            //                    Guid = SCat.Guid,
            //                    ID = SCat.ID,
            //                    Name = SCat.Name
            //                }
            //            }
            //        });
            //    }

            //    LibraryItem.Included = true;
            //    Library.Add(LibraryItem);

            //}

            //UserDataManager.SaveGamebananaAPI(API,Library, Properties.QuasarSettings.Default.DefaultDir);
            //UserDataManager.SaveLibrary(Library, AppDataPath);

            //if (Directory.Exists(Properties.QuasarSettings.Default.DefaultDir + @"\Backup"))
            //    Directory.Delete(Properties.QuasarSettings.Default.DefaultDir + @"\Backup", true);
            //if (Directory.Exists(Properties.QuasarSettings.Default.DefaultDir + @"\Resources"))
            //    Directory.Delete(Properties.QuasarSettings.Default.DefaultDir + @"\Resources", true);
            //if (File.Exists(Properties.QuasarSettings.Default.DefaultDir + @"\Library\Library.json"))
            //    File.Delete(Properties.QuasarSettings.Default.DefaultDir + @"\Library\Library.json");
            //if (File.Exists(Properties.QuasarSettings.Default.DefaultDir + @"\Library\ContentItems.json"))
            //    File.Delete(Properties.QuasarSettings.Default.DefaultDir + @"\Library\ContentItems.json");
            //if (File.Exists(Properties.QuasarSettings.Default.DefaultDir + @"\Library\Workspaces.json"))
            //    File.Delete(Properties.QuasarSettings.Default.DefaultDir + @"\Library\Workspaces.json");
        }

        /// <summary>
        /// Verifies if it's the first startup
        /// </summary>
        /// <returns></returns>
        public static bool InitialSetupRequested { get; set; }

        public static bool ScanRequested { get; set; }

        /// <summary>
        /// Upgrades the settings file to it's new version
        /// </summary>
        public static void UpgradeSettingFile()
        {
            string executionVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            executionVersion = executionVersion.Replace(".", "");
            Properties.QuasarSettings.Default.Upgrade();
            string previous = Properties.QuasarSettings.Default.AppVersion;
            Properties.QuasarSettings.Default.UpgradeRequired = false;
            Properties.QuasarSettings.Default.AppVersion = executionVersion;
            Properties.QuasarSettings.Default.PreviousVersion = previous;
            Properties.QuasarSettings.Default.Save();
            if (previous == "0000")
            {
                InitialSetupRequested = true;
            }
        }

        public static void SetupLogger(ILog QuasarLogger)
        {
            QuasarLogger = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)QuasarLogger.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.QuasarSettings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;

            appender.ActivateOptions();

            QuasarLogger.Info("------------------------------");
            QuasarLogger.Warn("Quasar Start");
            QuasarLogger.Info("------------------------------");
        }
    }

    public enum UpdateStatus { Regular, FirstBoot, ToUpdate, PreviouslyInstalled}
}
