using Quasar.Controls.Build.Models;
using Quasar.Controls.Common.Models;
using Quasar.Internal;
using Quasar.XMLResources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace Quasar.Controls.Build.ViewModels
{
    class BuildViewModel : ObservableObject
    {
        #region Fields
        private string _Logs { get; set; }
        private ObservableCollection<ModLoader> _ModLoaders { get; set; }
        private ObservableCollection<Workspace> _Workspaces { get; set; }
        private Workspace _ActiveWorkspace { get; set; }
        private ObservableCollection<USBDrive> _Drives { get; set; }
        private ModLoader _SelectedModLoader { get; set; }
        private USBDrive _SelectedDrive { get; set; }
        private bool _WirelessSelected { get; set; }
        private bool _LocalSelected { get; set; }

        private bool _ComparativeSelected { get; set; }
        private bool _WipeSelected { get; set; }

        private ICommand _RefreshUSBCommand { get; set; }
        private ICommand _BuildCommand { get; set; }
        #endregion

        #region Commands
        public ICommand RefreshUSBCommand
        {
            get
            {
                if (_RefreshUSBCommand == null)
                {
                    _RefreshUSBCommand = new RelayCommand(param => getSDCards());
                }
                return _RefreshUSBCommand;
            }
        }

        public ICommand BuildCommand
        {
            get
            {
                if (_BuildCommand == null)
                {
                    _BuildCommand = new RelayCommand(param => Build());
                }
                return _BuildCommand;
            }
        }
        #endregion

        #region Properties
        public string Logs
        {
            get => _Logs;
            set
            {
                _Logs = value;
                OnPropertyChanged("Logs");
            }
        }
        public ObservableCollection<ModLoader> ModLoaders
        {
            get => _ModLoaders;
            set
            {
                if (_ModLoaders == value)
                    return;

                _ModLoaders = value;
                OnPropertyChanged("ModLoaders");
            }
        }

        public ObservableCollection<Workspace> Workspaces
        {
            get => _Workspaces;
            set
            {
                if (_Workspaces == value)
                    return;

                _Workspaces = value;
                OnPropertyChanged("Workspaces");
            }
        }
        public Workspace ActiveWorkspace
        {
            get => _ActiveWorkspace;
            set
            {
                if (_ActiveWorkspace == value)
                    return;

                _ActiveWorkspace = value;
                OnPropertyChanged("ActiveWorkspace");
            }
        }

        public ObservableCollection<USBDrive> Drives
        {
            get => _Drives;
            set
            {
                if (_Drives == value)
                    return;

                _Drives = value;
                OnPropertyChanged("Drives");
            }
        }

        public ModLoader SelectedModLoader
        {
            get => _SelectedModLoader;
            set
            {
                if (_SelectedModLoader == value)
                    return;

                _SelectedModLoader = value;
                OnPropertyChanged("SelectedModLoader");
            }
        }

        public USBDrive SelectedDrive
        {
            get => _SelectedDrive;
            set
            {
                if (_SelectedDrive == value)
                    return;

                _SelectedDrive = value;
                OnPropertyChanged("SelectedDrive");
            }
        }

        public bool WirelessSelected
        {
            get => _WirelessSelected;
            set
            {
                if (_WirelessSelected == value)
                    return;

                _WirelessSelected = value;
                Properties.Settings.Default.Wireless = value;
                Properties.Settings.Default.Save();

                OnPropertyChanged("WirelessSelected");
                
            }
        }

        public bool LocalSelected
        {
            get => _LocalSelected;
            set
            {
                if (_LocalSelected == value)
                    return;

                _LocalSelected = value;
                OnPropertyChanged("LocalSelected");
            }
        }

        public bool ComparativeSelected
        {
            get => _ComparativeSelected;
            set
            {
                if (_ComparativeSelected == value)
                    return;

                _ComparativeSelected = value;
                OnPropertyChanged("ComparativeSelected");
            }
        }

        public bool WipeSelected
        {
            get => _WipeSelected;
            set
            {
                if (_WipeSelected == value)
                    return;

                _WipeSelected = value;
                OnPropertyChanged("WipeSelected");
            }
        }
        #endregion

        public BuildViewModel(ObservableCollection<ModLoader> _ModLoaders,ObservableCollection<Workspace> _Workspaces, Workspace _Workspace)
        {
            ModLoaders = _ModLoaders;
            ActiveWorkspace = _Workspace;
            Workspaces = _Workspaces;

            getSDCards();

            LoadUI();

            EventSystem.Subscribe<Workspace>(SelectWorkspace);

        }

        #region Actions
        
        private void LoadUI()
        {
            SelectedModLoader = ModLoaders[0];
            if (Drives.Count > 0)
            {
                SelectedDrive = Drives[0];
            }

            if (Properties.Settings.Default.Wireless)
            {
                WirelessSelected = true;
            }
            else
            {
                LocalSelected = true;
            }

            if (Properties.Settings.Default.Wipe)
            {
                WipeSelected = true;
            }
            else
            {
                ComparativeSelected = true;
            }

            Logs += "Hello, you're in the right place if you want those mods on your Switch.\r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "- FTP is a wireless means of File Transfer, select this if you want to use FTP.\r\n";
            Logs += "   !Please note that you have to setup the FTP in the settings first!\r\n";
            Logs += "- Local Transfer is for transfers using an SD reader.If you don't see the SD on the list, click the refresh button. \r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "- Mod Loader:\r\n";
            Logs += "There are two options, ARCropolis and UMM. If you haven't modded before I recommend ARCropolis which is the easiest option. UMM Will require you to have data.arc already dumped.\r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "- Comparative Mode: This will only change the necessary files.\r\n";
            Logs += "- Wipe and Recreate : This will completely empty the workspace folder and copy everything.\r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "-This button will start the process of transferring the files to your Switch according to the options you've selected above\r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "\r\n";
            Logs += "- Here is some nice info on how it's going !\r\n";
            Logs += "\r\n";

        }
        public void getSDCards()
        {
            Drives = new ObservableCollection<USBDrive>();

            DriveInfo[] CurrentDrives = DriveInfo.GetDrives();
            foreach (DriveInfo di in CurrentDrives)
            {
                if (di.DriveType == DriveType.Removable && di.IsReady)
                {
                    Drives.Add(new USBDrive(di));
                }
            }
        }

        public void Build()
        {
            Logs = "Transfer Process Start :\r\n\r\n";
            FileWriter FW;
            if (WirelessSelected)
            {
                FW = new FTPWriter();
            }
            else
            {
                FW = new SDWriter();
            }
            SmashBuilder SB = new SmashBuilder(FW, WipeSelected ? (int)BuildModes.WipeRecreate : (int)BuildModes.Comparative);
            SB.StartBuild();
            Logs += "Transfer Process End";
        }
        private void Build_Button()
        {
            /*
            bool willrun = true;
            string address = BuildFTPAddress.Text;
            string port = BuildFTPPort.Text;

            //Checking ModLoader
            if (BuilderModLoaderCombo.SelectedIndex == -1)
            {
                BuilderLogs.Text += "Please select a modloader first\r\n";
                willrun = false;
            }

            //Checking FTP
            if (BuilderFTPRadio.IsChecked == true)
            {
                if (!validateIP() || !validatePort())
                {
                    willrun = false;
                }
            }

            //Checking Local Transfer
            if (BuilderSDCombo.SelectedIndex == -1 && BuilderLocalRadio.IsChecked == true)
            {
                BuilderLogs.Text += "Please select a SD Drive first\r\n";
                willrun = false;
            }

           


            if (willrun)
            {
                Properties.Settings.Default.ModLoader = BuilderModLoaderCombo.SelectedIndex;
                Properties.Settings.Default.Wireless = (bool)BuilderFTPRadio.IsChecked;
                Properties.Settings.Default.Save();

                BuilderBuild.IsEnabled = false;
                BuilderFTPTest.IsEnabled = false;
                Boolean proceed = false;
                if (!Properties.Settings.Default.SupressBuildDeletion)
                {
                    MessageBoxResult result = MessageBox.Show("You are about to build the workspace. This will wipe your Workspace on your Switch to avoid conflicts. Do you wish to proceed with the build process?", "File Deletion Warning", MessageBoxButton.YesNo);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            proceed = true;
                            break;
                        case MessageBoxResult.No:
                            break;
                    }
                }
                if (proceed || Properties.Settings.Default.SupressBuildDeletion)
                {
                    BuilderProgress.IsIndeterminate = true;
                    QuasarTaskBar.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;

                    string pathname = BuilderSDCombo.SelectedIndex == -1 ? "" : USBDrives[BuilderSDCombo.SelectedIndex].Name;
                    string ftpPath = address + ":" + port;
                    NetworkCredential NC = null;
                    if (BuildPWRadio.IsChecked == true)
                    {
                        NC = new NetworkCredential(BuildFTPUN.Text, BuildFTPPW.Text);
                        
                    }
                    if (BuilderLocalRadio.IsChecked == true)
                    {
                        ftpPath = "";
                    }


                    ModLoader gamubuilder = (ModLoader)BuilderModLoaderCombo.SelectedItem;
                    await Builder.SmashBuild(pathname, gamubuilder.ID, ftpPath, NC, BuilderWipeCreateRadio.IsChecked == true ? 1 : -1, Mods, ContentMappings, CurrentWorkspace, InternalModTypes, CurrentGame, GameData, BuilderLogs, BuilderProgress,ModLoaders.ElementAt(BuilderModLoaderCombo.SelectedIndex), QuasarTaskBar);
                    BuilderProgress.Value = 100;
                    QuasarTaskBar.ProgressValue = 100;
                    BuilderLogs.Text += "Done\r\n";
                    BuilderBuild.IsEnabled = true;
                    BuilderFTPTest.IsEnabled = true;

                    CurrentWorkspace.Built = true;
                    WorkspaceXML.WriteWorkspaces(QuasarWorkspaces);
                }

            }*/
        }
        #endregion

        #region Events
        public void SelectWorkspace(Workspace w)
        {
            ActiveWorkspace = w;
        }
        #endregion
    }
}
