using Quasar.Singleton;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Deployment;
using System.Deployment.Application;
using System.Reflection;
using System.IO;
using static Quasar.XMLResources.AssociationXML;
using Quasar.Internal.Tools;
using Quasar.XMLResources;
using Quasar.Properties;

namespace Quasar.Quasar_Sys
{
    static class Checker
    {
        public static Mutex Instances(Mutex _serverMutex,ObservableCollection<string> _DLS)
        {
            string[] Args = Environment.GetCommandLineArgs();
            //Checking if Quasar is running alright
            if (Mutex.TryOpenExisting("Quasarite", out Mutex mt))
            {
                //Client
                if (Args.Length == 2)
                {
                    PipeClient.StartPipeClient("Quasarite", Args[1]);
                }
                mt.Close();
                Environment.Exit(0);
            }
            else
            {
                //Server
                _serverMutex = new Mutex(true, "Quasarite");
                if (Args.Length == 2)
                {
                    new PipeServer("Quasarite", _DLS, Args[1]);
                }
                else
                {
                    new PipeServer("Quasarite", _DLS, "");
                }

            }

            return _serverMutex;
        }

        public static void BaseUserSettings()
        {
            //Getting system language
            CultureInfo ci = CultureInfo.InstalledUICulture;
            Properties.Settings.Default.Language = (String)ci.Name;

            //Getting User's Documents folder for storage purposes
            String DocumentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DocumentsFolderPath += "\\Quasar";
            Properties.Settings.Default.DefaultDir = DocumentsFolderPath;

            //Getting execution path
            String AppPath = Environment.GetCommandLineArgs()[0];
            Properties.Settings.Default.AppPath = System.IO.Path.GetDirectoryName(AppPath);

        }

        public static void BaseWorkspace()
        {
            String AssociationsPath = Properties.Settings.Default.DefaultDir + @"\Library\Associations.xml";
            if (!File.Exists(AssociationsPath))
            {
                Workspace defaultWorkspace = new Workspace() { ID = IDGenerator.getNewWorkspaceID(), Name = "Default Workspace" };
                List<Workspace> DefaultFile = new List<Workspace>() { defaultWorkspace };
                AssociationXML.WriteAssociationFile(DefaultFile);

                Properties.Settings.Default.LastSelectedWorkspace = defaultWorkspace.ID;
                Properties.Settings.Default.Save();
            }
        }

        public static bool CheckQuasarUpdated()
        {
            string executionVersion = ExecutionVersion();
            executionVersion = executionVersion.Replace(".", "");

            bool Update = Properties.Settings.Default.UpgradeRequired;


            if (Update)
            {
                Properties.Settings.Default.Upgrade();
                if(Properties.Settings.Default.AppVersion == "0000")
                {
                    BaseUserSettings();
                }
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.AppVersion = executionVersion;
                Properties.Settings.Default.Save();
            }
            return Update;
        }

        public static string ExecutionVersion()
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
