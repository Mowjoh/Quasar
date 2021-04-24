using log4net;
using MediaDevices;
using Quasar.Build.Models;
using Quasar.Common.Models;
using Quasar.Data.V2;
using Quasar.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Quasar.MainUI.ViewModels;
using Quasar.Settings.Models;

namespace Quasar.Build.ViewModels
{
    public class BuildViewModel : ObservableObject
    {
        #region View

        #region Private
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

        #region Public
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

        #endregion

        #region Data

        #region Private
        private MainUIViewModel _MUVM { get; set; }
        private SmashBuilder _SB { get; set; }
        #endregion

        #region Public
        public MainUIViewModel MUVM
        {
            get => _MUVM;
            set
            {
                _MUVM = value;
                OnPropertyChanged("MUVM");
            }
        }
        public SmashBuilder SB
        {
            get => _SB;
            set
            {
                _SB = value;
                OnPropertyChanged("SB");
            }
        }
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _RefreshUSBCommand { get; set; }
        private ICommand _BuildCommand { get; set; }
        #endregion

        #region Public
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

        #endregion

        public ILog QuasarLogger { get; set; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="_MUVM"></param>
        /// <param name="_QuasarLogger"></param>
        public BuildViewModel(MainUIViewModel _MUVM, ILog _QuasarLogger)
        {
            QuasarLogger = _QuasarLogger;

            MUVM = _MUVM;

            getSDCards();
            getMTPDrives();

            LoadUI();

            EventSystem.Subscribe<Workspace>(SelectWorkspace);
            EventSystem.Subscribe<ModalEvent>(ProcessIncomingModalEvent);

        }

        #region Actions
        
        /// <summary>
        /// Sets up the UI with preffered options
        /// </summary>
        private void LoadUI()
        {
            SelectedModLoader = MUVM.ModLoaders[0];
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

        }

        /// <summary>
        /// Retreives the list of Flash Drives
        /// </summary>
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
            var devices = MediaDevice.GetDevices();
            foreach (MediaDevice device in devices)
            {
                if(device.FriendlyName == "")
                {
                    //Drives.Add(new USBDrive(device));
                }
            }
        }

        /// <summary>
        /// Retreives the list of connected MTP Drives
        /// </summary>
        public void getMTPDrives()
        {
            
            
        }

        

        /// <summary>
        /// Sets UI for the end build process
        /// </summary>
        public async void EndBuildProcess()
        {
            QuasarLogger.Info("Transfer Finished");
            SetStep("Finished");
            SetSubStep("");
            SetProgression(100);
            SetProgressionStyle(false);
            SetSize("");
            SetSpeed("");
            SetTotal("0" , "0");
            Building = false;
            BuildLog("Info", "Transfer Process End");

            EventSystem.Publish<SettingItem>(new SettingItem
            {
                IsChecked = false,
                SettingName = "TabLock"
            });

        }
        #endregion

        #region User Actions
        /// <summary>
        /// Launches the build process
        /// </summary>
        public async void Build()
        {
            //Setting Tab Lock ON
            EventSystem.Publish<SettingItem>(new SettingItem
            {
                IsChecked = true,
                SettingName = "TabLock"
            });

            ResetLogs();
            BuildLog("Info", "Transfer Process Start :");
            bool ok = true;

            FileWriter FW;
            if (WirelessSelected)
            {
                BuildLog("Info", "Attempting to connect to the Switch...");
                FW = new FTPWriter(this) { Log = QuasarLogger };
                ok = await FW.VerifyOK();
                if (!ok)
                {
                    BuildLog("Error", "Could not connect.");
                }
                else
                {
                    BuildLog("Info", "Connection successful.");
                }
            }
            else
            {
                if(SelectedDrive == null)
                {
                    BuildLog("Error", "No SD selected");
                    ok = false;
                    FW = null;
                }
                else
                {
                    if (SelectedDrive.MediaD != null)
                    {
                        FW = new MTPWriter(this) { MediaD = SelectedDrive.MediaD };
                    }
                    else
                    {
                        FW = new SDWriter(this) { LetterPath = SelectedDrive.Info.Name, Log = QuasarLogger };
                    }
                }
            }

            if (ok)
            {
                bool proceed = false;
                try
                {
                    SB = new SmashBuilder(FW, SelectedModLoader.ID, CleanSelected, OverwriteSelected, this);
                    proceed = true;
                }
                catch(Exception e)
                {
                    QuasarLogger.Error(e.Message);
                }

                if (proceed)
                {
                    await Task.Run(() => {
                        SB.StartBuild();
                    });

                    SB.CheckModLoader(SelectedModLoader.ID);
                    if (!SB.ModLoaderInstalled)
                    {
                        SetStep("ModLoader Configuration");
                        ModalEvent meuh = new ModalEvent()
                        {
                            Type = ModalType.OkCancel,
                            Action = "Show",
                            EventName = "AskModLoaderInstall",
                            Title = "ARCropolis setup",
                            Content = "Arcropolis is not detected on your Switch\rIt's required to load mods\rDo you want Quasar to install and setup ARCRopolis for you?",
                            OkButtonText = "Yes please",
                            CancelButtonText = "No, I want to do it myself"
                        };

                        EventSystem.Publish<ModalEvent>(meuh);
                    }
                    else
                    {
                        if (!Properties.Settings.Default.ModLoaderSetup)
                        {
                            SetStep("ModLoader Configuration");
                            ModalEvent meuh = new ModalEvent()
                            {
                                Type = ModalType.OkCancel,
                                Action = "Show",
                                EventName = "AskModLoaderSetup",
                                Title = "ARCropolis setup",
                                Content = "Do you want Quasar to change ARCRopolis'\ractive workspace to this one ?",
                                OkButtonText = "Yes please",
                                CancelButtonText = "No"
                            };

                            EventSystem.Publish<ModalEvent>(meuh);
                        }
                        else
                        {
                            if (Properties.Settings.Default.ModLoaderSetupState)
                            {
                                await Task.Run(() => {

                                    SB.SetupModLoader(SelectedModLoader.ID, MUVM.ActiveWorkspace.Name);
                                });

                            }

                            EndBuildProcess();

                        }
                    }
                }
                else
                {
                    BuildLog("Error", "Could not launch build");
                    EndBuildProcess();
                   
                }
            }
            else
            {
                BuildLog("Error", "Could not launch build");
                EndBuildProcess();
            }

            
        }

        #endregion

        #region Events

        /// <summary>
        /// Processes all incoming Modal Event responses
        /// </summary>
        /// <param name="meuh"></param>
        public async void ProcessIncomingModalEvent(ModalEvent meuh)
        {
            switch (meuh.EventName)
            {
                case "AskModLoaderSetup":
                    if(meuh.Action == "OK")
                    {
                        await Task.Run(() => {
                            SB.SetupModLoader(SelectedModLoader.ID, MUVM.ActiveWorkspace.Name);
                        });
                        EndBuildProcess();
                    }
                    if(meuh.Action == "KO")
                    {
                        EndBuildProcess();
                    }
                    break;
                case "AskModLoaderInstall":
                    if (meuh.Action == "OK")
                    {
                        await Task.Run(() => {
                            SB.InstallModLoader(SelectedModLoader.ID, MUVM.ActiveWorkspace.Name);
                        });
                        EndBuildProcess();
                    }
                    if(meuh.Action == "KO")
                    {
                        EndBuildProcess();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Responds to a workspace change trigger
        /// </summary>
        /// <param name="w"></param>
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
