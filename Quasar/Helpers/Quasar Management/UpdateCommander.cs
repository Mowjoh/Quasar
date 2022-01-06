using log4net;
using log4net.Appender;
using Quasar.Helpers.Quasar_Management;
using System;
using System.Reflection;
using System.Windows;
using Workshop.Updates;

namespace Quasar.Internal.Tools
{
    public static class UpdateCommander
    {
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
                    //If files are already there and valid
                    if (Updater.CheckForValidData())
                    {
                        QuasarLogger.Debug("previous install detected");
                        return UpdateStatus.PreviouslyInstalled;
                    }

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

        public static void LaunchFirstBootSequence()
        {
            InstallManager.CreateBaseUserSettings();

            MessageBoxResult result = System.Windows.MessageBox.Show("Hi ! It seems it's Quasar's first launch. Do you want to change where Quasar is gonna store mods?", "First Launch Warning", MessageBoxButton.YesNo);
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

        public static void LaunchUpdateSequence(ILog QuasarLogger)
        {
            SetupLogger(QuasarLogger);
            QuasarLogger.Debug("Updating");
        }

        public static void LaunchInstallRecoverySequence()
        {

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
