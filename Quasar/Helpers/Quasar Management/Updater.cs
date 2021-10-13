using log4net;
using log4net.Appender;
using Quasar.FileSystem;
using Quasar.Helpers.FileOperations;
using Quasar.Helpers.Quasar_Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Quasar.Internal.Tools
{
    public static class Updater
    {
        /// <summary>
        /// Runs checks and updates if necessary
        /// </summary>
        /// <returns></returns>
        public static bool CheckExecuteUpdate(ILog QuasarLogger)
        {
            bool UpdateSuccessful = false;

            
            //If the loaded conf file is a new update file
            if (Properties.Settings.Default.UpgradeRequired)
            {
                NeedsUpdate = true;

                //Making file up to date with previous data
                UpgradeSettingFile();

                //If it's Quasar's first launch
                if (NeedsInitialSetup)
                {
                    InstallManager.CreateBaseUserSettings();

                    MessageBoxResult result = System.Windows.MessageBox.Show("Hi ! It seems it's Quasar's first launch. Do you want to change where Quasar is gonna store mods?", "First Launch Warning", MessageBoxButton.YesNo);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            InstallManager.ChangeInstallLocationSetting();
                            break;
                        case MessageBoxResult.No:
                            Properties.Settings.Default.DefaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Quasar";
                            Properties.Settings.Default.Save();
                            break;
                    }  
                    
                    InstallManager.CreateBaseFolders();

                }
                //If it's just a regular update
                else
                {
                    SetupLogger(QuasarLogger);
                    QuasarLogger.Debug("Updating");
                    QuasarLogger.Debug("Tagging for Scan");
                    NeedsScanning = true;
                }
            }
            else
            {
                NeedsUpdate = false;
                QuasarLogger.Debug("No Update");
                UpdateSuccessful = true;
            }

            

            return UpdateSuccessful;
        }

        /// <summary>
        /// Verifies if the install folders need cleaning
        /// </summary>
        public static void CheckCleanInstallation()
        {

        }

        /// <summary>
        /// Verifies if the app needs to run an update process
        /// </summary>
        /// <returns></returns>
        public static bool NeedsUpdate { get; set; }
        public static bool UltraCleanState { get; set; }

        /// <summary>
        /// Verifies if the app needs to clean it's folders
        /// </summary>
        /// <returns></returns>
        public static bool NeedsCleaning()
        {
            int curVer = int.Parse(Properties.Settings.Default.AppVersion);
            int prevVer = int.Parse(Properties.Settings.Default.PreviousVersion);
            
            //If updating from a 1.5.X build
            if (curVer >= 2100 && prevVer < 2040)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifies if the app needs to clean it's folders
        /// </summary>
        /// <returns></returns>
        public static bool NeedsUltraCleaning()
        {
            /*
            int curVer = int.Parse(Properties.Settings.Default.AppVersion);
            int prevVer = int.Parse(Properties.Settings.Default.PreviousVersion);

            //If updating from a 1.5.X build
            if (curVer >= 2100 && prevVer > 0000 && prevVer < 2000)
            {
                return true;
            }
            */
            return false;
        }

        /// <summary>
        /// Verifies if it's the first startup
        /// </summary>
        /// <returns></returns>
        public static bool NeedsInitialSetup { get; set; }

        public static bool NeedsScanning { get; set; }

        /// <summary>
        /// Upgrades the settings file to it's new version
        /// </summary>
        public static void UpgradeSettingFile()
        {
            string executionVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            executionVersion = executionVersion.Replace(".", "");
            Properties.Settings.Default.Upgrade();
            string previous = Properties.Settings.Default.AppVersion;
            Properties.Settings.Default.UpgradeRequired = false;
            Properties.Settings.Default.AppVersion = executionVersion;
            Properties.Settings.Default.PreviousVersion = previous;
            Properties.Settings.Default.Save();
            if (previous == "0000")
            {
                NeedsInitialSetup = true;
            }
        }

        /// <summary>
        /// Returns the app's version number
        /// </summary>
        /// <returns></returns>
        public static string GetExecutionVersion()
        {
            try
            {
                //TODO FIX THIS
                //return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                return "ERROR";
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static void SetupLogger(ILog QuasarLogger)
        {
            QuasarLogger = LogManager.GetLogger("QuasarAppender");
            FileAppender appender = (FileAppender)QuasarLogger.Logger.Repository.GetAppenders()[0];
            appender.File = Properties.Settings.Default.DefaultDir + "\\Quasar.log";
            appender.Threshold = log4net.Core.Level.Debug;

            appender.ActivateOptions();

            QuasarLogger.Info("------------------------------");
            QuasarLogger.Warn("Quasar Start");
            QuasarLogger.Info("------------------------------");
        }
    }


}
