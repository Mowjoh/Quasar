using Quasar.Data.V2;
using Quasar.Internal.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Quasar.Helpers.Json;
using System.Globalization;
using Quasar.Helpers.FileOperations;

namespace Quasar.Helpers.Quasar_Management
{
    public static class InstallManager
    {
        //Verifies needed folders for execution
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

        //Forcing references files and base Internal Mod Types to be refreshed in the user folder
        public static void UpdateInstallation()
        {
            String ResourcePath = Properties.Settings.Default.DefaultDir + "\\Resources\\";
            String AppPath = Properties.Settings.Default.AppPath + "\\Resources\\";

            FileOperation.CheckCopyFolder(AppPath, ResourcePath, true,true);
        }

        public static void CreateBaseUserSettings()
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
    }
}
