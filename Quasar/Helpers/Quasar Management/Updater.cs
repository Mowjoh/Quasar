using Quasar.FileSystem;
using Quasar.Helpers.Quasar_Management;
using Quasar.NamedPipes;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
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
        public static bool CheckExecuteUpdate()
        {
            bool UpdateSuccessful = false;
            if (NeedsUpdate())
            {
                if(NeedsInitialSetup()){
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
                UpgradeSettingFile();
            }
            else
            {
                UpdateSuccessful = true;
            }

            if (NeedsCleaning())
            {
                InstallManager.CreateBaseFolders();
                CheckCleanInstallation();
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
        public static bool NeedsUpdate()
        {
            bool Update = Properties.Settings.Default.UpgradeRequired;

            if (Update)
            {
                Properties.Settings.Default.Upgrade();
            }
            return Update;
        }

        /// <summary>
        /// Verifies if the app needs to clean it's folders
        /// </summary>
        /// <returns></returns>
        public static bool NeedsCleaning()
        {
            return false;
        }

        /// <summary>
        /// Verifies if it's the first startup
        /// </summary>
        /// <returns></returns>
        public static bool NeedsInitialSetup()
        {
            return Properties.Settings.Default.AppVersion == "0000";
        }

        /// <summary>
        /// Verifies if the mods need scanning
        /// </summary>
        /// <returns></returns>
        public static bool NeedsScanning()
        {
            return false;
        }

        /// <summary>
        /// Upgrades the settings file to it's new version
        /// </summary>
        public static void UpgradeSettingFile()
        {
            string executionVersion = GetExecutionVersion();
            executionVersion = executionVersion.Replace(".", "");
            Properties.Settings.Default.Upgrade();
            string previous = Properties.Settings.Default.AppVersion;
            Properties.Settings.Default.UpgradeRequired = false;
            Properties.Settings.Default.AppVersion = executionVersion;
            Properties.Settings.Default.PreviousVersion = previous;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Returns the app's version number
        /// </summary>
        /// <returns></returns>
        public static string GetExecutionVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
