using log4net;
using log4net.Repository.Hierarchy;
using Quasar.Controls.Build.Models;
using Quasar.Controls.Common.Models;
using Quasar.Internal;
using Quasar.XMLResources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace Quasar.Controls.Build.ViewModels
{
    public class BuildViewModel : ObservableObject
    {
        #region Fields

        #region Views
        private Workspace _ActiveWorkspace { get; set; }
        private ObservableCollection<USBDrive> _Drives { get; set; }
        private ModLoader _SelectedModLoader { get; set; }
        private USBDrive _SelectedDrive { get; set; }
        private bool _WirelessSelected { get; set; }
        private bool _LocalSelected { get; set; }
        private bool _SynchronizeSelected { get; set; }
        private bool _CleanSelected { get; set; }
        private bool _OverwriteSelected { get; set; }

        private bool _Building { get; set; } = false;

        private string _Logs { get; set; }
        private double _BuilderProgress { get; set; }
        private string _Steps { get; set; } = "Step :";
        private string _SubStep { get; set; } = "Sub-Step :";
        private string _Total { get; set; } = "0/0 Contents";
        private string _Speed { get; set; } = "0 MB";
        private string _Size { get; set; } = "0 MB/s";
        private bool _ProgressBarStyle { get; set; } = false;
        #endregion

        #region Data
        private ObservableCollection<LibraryMod> _Mods { get; set; }
        private ObservableCollection<ContentMapping> _ContentMappings { get; set; }
        private ObservableCollection<ModLoader> _ModLoaders { get; set; }
        private ObservableCollection<Workspace> _Workspaces { get; set; }
        private ObservableCollection<InternalModType> _InternalModTypes { get; set; }
        private ObservableCollection<GameData> _GameDatas { get; set; }
        private ObservableCollection<Game> _Games { get; set; }
        #endregion

        #region Commands
        private ICommand _RefreshUSBCommand { get; set; }
        private ICommand _BuildCommand { get; set; }
        #endregion

        #endregion

        #region Properties

        #region View
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
        public bool SynchronizeSelected
        {
            get => _SynchronizeSelected;
            set
            {
                if (_SynchronizeSelected == value)
                    return;

                _SynchronizeSelected = value;
                OnPropertyChanged("SynchronizeSelected");
            }
        }
        public bool CleanSelected
        {
            get => _CleanSelected;
            set
            {
                if (_CleanSelected == value)
                    return;

                _CleanSelected = value;
                OnPropertyChanged("CleanSelected");
            }
        }
        public bool OverwriteSelected
        {
            get => _OverwriteSelected;
            set
            {
                if (_OverwriteSelected == value)
                    return;

                _OverwriteSelected = value;
                OnPropertyChanged("OverwriteSelected");
            }
        }

        public bool Building
        {
            get => _Building;
            set
            {
                if (_Building == value)
                    return;

                _Building = value;
                OnPropertyChanged("Building");
            }
        }

        public string Logs
        {
            get => _Logs;
            set
            {
                _Logs = value;
                OnPropertyChanged("Logs");
            }
        }
        public double BuildProgress
        {
            get => _BuilderProgress;
            set
            {
                _BuilderProgress = value;
                OnPropertyChanged("BuildProgress");
            }
        }
        public string Steps
        {
            get => _Steps;
            set
            {
                _Steps = value;
                OnPropertyChanged("Steps");
            }
        }
        public string SubStep
        {
            get => _SubStep;
            set
            {
                _SubStep = value;
                OnPropertyChanged("SubStep");
            }
        }
        public string Total
        {
            get => _Total;
            set
            {
                _Total = value;
                OnPropertyChanged("Total");
            }
        }
        public string Speed
        {
            get => _Speed;
            set
            {
                _Speed = value;
                OnPropertyChanged("Speed");
            }
        }
        public string Size
        {
            get => _Size;
            set
            {
                _Size = value;
                OnPropertyChanged("Size");
            }
        }
        public bool ProgressBarStyle
        {
            get => _ProgressBarStyle;
            set
            {
                _ProgressBarStyle = value;
                OnPropertyChanged("ProgressBarStyle");
            }
        }
        #endregion

        #region Data
        /// <summary>
        /// List of all Library Mods
        /// </summary>
        public ObservableCollection<LibraryMod> Mods
        {
            get => _Mods;
            set
            {
                if (_Mods == value)
                    return;

                _Mods = value;
                OnPropertyChanged("Mods");
            }
        }
        /// <summary>
        /// List of all content mappings
        /// </summary>
        public ObservableCollection<ContentMapping> ContentMappings
        {
            get => _ContentMappings;
            set
            {
                if (_ContentMappings == value)
                    return;

                _ContentMappings = value;
                OnPropertyChanged("ContentMappings");
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
        /// <summary>
        /// List of all Internal Mod Types
        /// </summary>
        public ObservableCollection<InternalModType> InternalModTypes
        {
            get => _InternalModTypes;
            set
            {
                if (_InternalModTypes == value)
                    return;

                _InternalModTypes = value;
                OnPropertyChanged("InternalModTypes");
            }
        }
        /// <summary>
        /// List of all Game Data
        /// </summary>
        public ObservableCollection<GameData> GameDatas
        {
            get => _GameDatas;
            set
            {
                if (_GameDatas == value)
                    return;

                _GameDatas = value;
                OnPropertyChanged("GameDatas");
            }
        }
        /// <summary>
        /// List of all Games and their API Categories
        /// </summary>
        public ObservableCollection<Game> Games
        {
            get => _Games;
            set
            {
                if (_Games == value)
                    return;

                _Games = value;
                OnPropertyChanged("Games");
            }
        }
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

        public ILog Log { get; set; }
        #endregion

        public BuildViewModel(ObservableCollection<ModLoader> _ModLoaders,ObservableCollection<Workspace> _Workspaces, Workspace _Workspace, ObservableCollection<LibraryMod> _Mods, ObservableCollection<ContentMapping> _ContentMappings, ObservableCollection<InternalModType> _InternalModTypes, ObservableCollection<GameData> _GameDatas, ObservableCollection<Game> _Games)
        {
            ModLoaders = _ModLoaders;
            ActiveWorkspace = _Workspace;
            Workspaces = _Workspaces;
            Mods = _Mods;
            ContentMappings = _ContentMappings;
            InternalModTypes = _InternalModTypes;
            GameDatas = _GameDatas;
            Games = _Games;

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
                CleanSelected = true;
            }
            else
            {
                SynchronizeSelected = true;
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

        public async void Build()
        {
            ResetLogs();
            BuildLog("Info","Transfer Process Start :");


            FileWriter FW;
            if (WirelessSelected)
            {
                FW = new FTPWriter(this) { Log = Log };
            }
            else
            {
                FW = new SDWriter(this) { LetterPath = SelectedDrive.Info.Name, Log = Log };
            }

            SmashBuilder SB = new SmashBuilder(FW, CleanSelected ? (int)BuildModes.Clean : SynchronizeSelected ? (int)BuildModes.Synchronize : (int)BuildModes.Overwrite, ModLoaders[0].ModLoaderID, this);

            await Task.Run(() => {
                SB.StartBuild();
            });

            Building = false;
            BuildLog("Info", "Transfer Process End");
        }

        #endregion

        #region Events
        public void SelectWorkspace(Workspace w)
        {
            ActiveWorkspace = w;
        }
        #endregion

        #region Async UI Modifiers
        public void SetStep(string s)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Steps = String.Format("Step : {0}", s);
            }));
        }
        public void SetSubStep(string s)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                SubStep = String.Format("Sub-Step : {0}", s);
            }));
        }

        public void SetTotal(string current,string total)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Total = String.Format("{0}/{1} Files", current, total);
            }));
        }
        public void SetProgression(double value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                BuildProgress = value;
            }));
        }

        public void SetSpeed(string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Speed = value;
            }));
        }
        public void SetSize(string value)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Size = value;
            }));
        }
        public void SetProgressionStyle(bool IsIndeterminate)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                ProgressBarStyle = IsIndeterminate;
            }));
        }

        public void BuildLog(string type, string log)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Logs += String.Format("{0} - {1} \r\n",type, log);
            }));
        }

        public void ResetLogs()
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => {
                Logs ="";
            }));
        }
        #endregion
    }
}
