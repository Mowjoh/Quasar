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
using System.Windows.Forms;
using Quasar.Models;
using Quasar.Internal;

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
                    MessageBox.Show("No path selected, using defaults. Don't worry you can still change later in the settings");
                }
            }
        }

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
    }
}
