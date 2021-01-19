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

namespace Quasar.Internal.Tools
{
    public static class Updater
    {
        //Updater Actions
        public static bool CheckExecuteUpdate()
        {
            bool UpdateSuccessful = false;
            if (NeedsUpdate())
            {
                if(NeedsInitialSetup()){
                    InstallManager.CreateBaseUserSettings();
                    InstallManager.CreateBaseFolders();
                }
                else
                {
                    
                                        
                }

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

        public static void CheckCleanInstallation()
        {

        }

        //Updater Checks
        public static bool NeedsUpdate()
        {
            

            bool Update = Properties.Settings.Default.UpgradeRequired;


            if (Update)
            {
                Properties.Settings.Default.Upgrade();
            }
            return Update;
        }
        public static bool NeedsCleaning()
        {
            return true;
        }
        public static bool NeedsInitialSetup()
        {
            return Properties.Settings.Default.AppVersion == "0000";
        }
        public static bool NeedsScanning()
        {
            return false;
        }

        //Setting File Action
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
