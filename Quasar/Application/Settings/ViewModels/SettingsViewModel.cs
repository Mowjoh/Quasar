using FluentFTP;
using Quasar.Common.Models;
using Quasar.Settings.Views;
using Quasar.Helpers.Quasar_Management;
using Quasar.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DataModels.Common;
using log4net;
using Quasar.Settings.Models;

namespace Quasar.Settings.ViewModels
{
    class SettingsViewModel : ObservableObject
    {
        #region Data

        #region Private
        ObservableCollection<SettingItemView> _AppSettings { get; set; }
        ObservableCollection<SettingItemView> _FTPSettings { get; set; }
        ObservableCollection<SettingItemView> _WarningSettings { get; set; }
        ObservableCollection<SettingItemView> _TransferSettings { get; set; }
        private bool _Setup { get; set; } = true;
        private bool SDSelected => Properties.QuasarSettings.Default.PreferredTransferMethod == "SD";
        private bool DiskSelected => Properties.QuasarSettings.Default.PreferredTransferMethod == "Disk";
        #endregion

        #region Public
        /// <summary>
        /// List of Setting Items views
        /// </summary>
        public ObservableCollection<SettingItemView> AppSettings
        {
            get => _AppSettings;
            set
            {
                if (_AppSettings == value)
                    return;

                _AppSettings = value;
                OnPropertyChanged("AppSettings");
            }
        }
        public ObservableCollection<SettingItemView> FTPSettings
        {
            get => _FTPSettings;
            set
            {
                if (_FTPSettings == value)
                    return;

                _FTPSettings = value;
                OnPropertyChanged("FTPSettings");
            }
        }
        public ObservableCollection<SettingItemView> WarningSettings
        {
            get => _WarningSettings;
            set
            {
                if (_WarningSettings == value)
                    return;

                _WarningSettings = value;
                OnPropertyChanged("WarningSettings");
            }
        }
        public ObservableCollection<SettingItemView> TransferSettings
        {
            get => _TransferSettings;
            set
            {
                if (_TransferSettings == value)
                    return;

                _TransferSettings = value;
                OnPropertyChanged("TransferSettings");
            }
        }

        public bool Setup
        {
            get => _Setup;
            set
            {
                if (_Setup == value)
                    return;

                _Setup = value;
                OnPropertyChanged("Setup");
            }
        }
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _ValidateFTPCommand { get; set; }
        private ICommand _MoveInstallCommand { get; set; }
        private ICommand _ReloadCommand { get; set; }
        #endregion

        #region Public
        public ICommand ValidateFTPCommand
        {
            get
            {
                if (_ValidateFTPCommand == null)
                {
                    _ValidateFTPCommand = new RelayCommand(param => ValidateFTP());
                }
                return _ValidateFTPCommand;
            }
        }
        public ICommand MoveInstallCommand
        {
            get
            {
                if (_MoveInstallCommand == null)
                {
                    _MoveInstallCommand = new RelayCommand(param => AskMoveInstall());
                }
                return _MoveInstallCommand;
            }
        }
        public ICommand ReloadCommand
        {
            get
            {
                if (_ReloadCommand == null)
                {
                    _ReloadCommand = new RelayCommand(param => LoadSettings());
                }
                return _ReloadCommand;
            }
        }
        #endregion

        #endregion

        public ILog QuasarLogger { get; set; }
        public SettingsViewModel(ILog quasar_log)
        {
            QuasarLogger = quasar_log;
            LoadSettings();

            EventSystem.Subscribe<ModalEvent>(ProcessModalEvent);
            EventSystem.Subscribe<SettingItem>(ProcessSettingChanged);

        }


        #region Actions
        /// <summary>
        /// Adds specific settings to the collection
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                QuasarLogger.Info("Settings Load");
                Setup = true;
                AppSettings = new ObservableCollection<SettingItemView>
            {
                new("AppVersion",Properties.Resources.Settings_Label_AppVersion, "", SettingItemType.Text),
                new("Language", Properties.Resources.Settings_Label_Language,Properties.Resources.Settings_Comment_Language,SettingItemType.List,"English=EN,Français=FR")
            };
                FTPSettings = new ObservableCollection<SettingItemView>
            {
                new("FtpIP", Properties.Resources.Settings_Label_FtpIP, Properties.Resources.Settings_Comment_FtpIP, SettingItemType.Input),
                new("FtpPort", Properties.Resources.Settings_Label_FtpPort, Properties.Resources.Settings_Comment_FtpPort, SettingItemType.Input),
                new("FtpUsername", Properties.Resources.Settings_Label_FtpUsername, Properties.Resources.Settings_Comment_FtpUsername, SettingItemType.Input),
                new("FtpPassword", Properties.Resources.Settings_Label_FtpPassword, Properties.Resources.Settings_Comment_FtpPassword, SettingItemType.Input),
            };
                WarningSettings = new ObservableCollection<SettingItemView>
            {
                new("SupressModDeletion", Properties.Resources.Settings_Label_SupressModDeletion, Properties.Resources.Settings_Comment_SupressModDeletion, SettingItemType.Toggle),
            };

                TransferSettings = new ObservableCollection<SettingItemView>
                        {
                            new("PreferredTransferMethod", Properties.Resources.Settings_Label_PreferredTransferMethod, Properties.Resources.Settings_Comment_PreferredTransferMethod, SettingItemType.List,Properties.Resources.Settings_Values_PreferredTransferMethod),
                        };

                if (SDSelected)
                {
                    TransferSettings.Add(new("TransferPath", Properties.Resources.Settings_Label_ModsPath, Properties.Resources.Settings_Comment_ModsPath, SettingItemType.Input));
                    TransferSettings.Add(new("SelectedSD", Properties.Resources.Settings_Label_SDSelect, Properties.Resources.Settings_Comment_SDSelect, SettingItemType.List, getSDCards()));
                }
                else
                {
                    if (DiskSelected)
                    {
                        TransferSettings.Add(new("DiskPath", Properties.Resources.Settings_Label_Diskpath, Properties.Resources.Settings_Comment_Diskpath, SettingItemType.Input));
                    }
                    else
                    {
                        TransferSettings.Add(new("TransferPath", Properties.Resources.Settings_Label_ModsPath, Properties.Resources.Settings_Comment_ModsPath, SettingItemType.Input));
                    }
                }

                TransferSettings.Add(new("TransferQuasarFoldersOnly", Properties.Resources.Settings_Label_ManageAllMods, Properties.Resources.Settings_Comment_ManageAllMods, SettingItemType.Toggle));
                
                
                Setup = false;
                QuasarLogger.Info("Settings Load Finished");
            }
            catch (Exception e)
            {
                QuasarLogger.Error(e.Message);
                QuasarLogger.Error(e.StackTrace);
            }
            
        }

        private void ProcessSettingChanged(SettingItem SettingItem)
        {
            if (!Setup)
            {
                switch (SettingItem.SettingName)
                {
                    default:
                        break;
                    case "Language":
                        EventSystem.Publish<ModalEvent>(new()
                        {
                            Action = "Show",
                            Content = Properties.Resources.Settings_Modal_Content_ShutdownWarning,
                            Title = Properties.Resources.Settings_Modal_Title_ShutdownWarning,
                            OkButtonText = Properties.Resources.Settings_Modal_Button_ShutdownWarning,
                            EventName = "ShutdownWarning",
                            Type = ModalType.Warning
                        });
                        break;
                    case "PreferredTransferMethod":
                        LoadSettings();
                        break;
                    case "TransferPath":
                        Properties.QuasarSettings.Default.TransferPath = SettingItem.DisplayValue;
                        Properties.QuasarSettings.Default.Save();
                        break;
                    case "DiskPath":
                        Properties.QuasarSettings.Default.DiskPath = SettingItem.DisplayValue;
                        Properties.QuasarSettings.Default.Save();
                        break;

                }
            }
        }
        #endregion

        #region User Actions
        /// <summary>
        /// Launches the FTP validation task and Modal
        /// </summary>
        public void ValidateFTP()
        {
            //Parsing values
            foreach(SettingItemView SIV in FTPSettings)
            {
                switch (SIV.ViewModel.SettingItem.SettingName)
                {
                    case "FtpIP":
                        Properties.QuasarSettings.Default.FtpIP = SIV.ViewModel.SettingItem.DisplayValue;
                        break;
                    case "FtpPort":
                        Properties.QuasarSettings.Default.FtpPort = SIV.ViewModel.SettingItem.DisplayValue;
                        break;
                    case "FtpUsername":
                        Properties.QuasarSettings.Default.FtpUsername = SIV.ViewModel.SettingItem.DisplayValue;
                        break;
                    case "FtpPassword":
                        Properties.QuasarSettings.Default.FtpPassword = SIV.ViewModel.SettingItem.DisplayValue;
                        break;
                }
                Properties.QuasarSettings.Default.Save();
            }

            //Validating values
            Regex AddressRegex = new Regex(@"(?'IP'(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}))");
            Regex PortRegex = new Regex(@"(?'Port'(\d{1,5}))");
            if (Properties.QuasarSettings.Default.FtpIP != null && Properties.QuasarSettings.Default.FtpPort != null)
            {
                if (AddressRegex.IsMatch(Properties.QuasarSettings.Default.FtpIP) && PortRegex.IsMatch(Properties.QuasarSettings.Default.FtpPort))
                {
                    //Showing connection Modal
                    ModalEvent Meuh = new ModalEvent()
                    {
                        Type = ModalType.Loader,
                        Title = Properties.Resources.Settings_Modal_Title_ConnectionWait,
                        Content = Properties.Resources.Settings_Modal_Content_ConnectionWait,
                        OkButtonText = Properties.Resources.Modal_Label_DefaultOK,
                        Action = "Show",
                        EventName = "FTPConnectionTest"
                    };
                    EventSystem.Publish(Meuh);

                    //Configuring FTP Client
                    FtpClient ftpClient = new FtpClient(Properties.QuasarSettings.Default.FtpIP);
                    ftpClient.Port = int.Parse(Properties.QuasarSettings.Default.FtpPort);
                    if (Properties.QuasarSettings.Default.FtpUsername != "")
                    {
                        ftpClient.Credentials = new System.Net.NetworkCredential(Properties.QuasarSettings.Default.FtpUsername, Properties.QuasarSettings.Default.FtpPassword);
                    }

                    //Launching connection Task
                    Task.Run(() => {
                        try
                        {
                            ftpClient.ConnectTimeout = 3000;
                            ftpClient.Connect();

                            //Changing Modal state for success
                            Meuh.Content = Properties.Resources.Settings_Modal_Content_SuccessfulConnection;
                            Meuh.Title = Properties.Resources.Settings_Modal_Title_SuccessfulConnection;
                            Meuh.Action = "LoadOK";
                            EventSystem.Publish(Meuh);

                            //Saving FTP state
                            Properties.QuasarSettings.Default.FTPValid = true;
                            Properties.QuasarSettings.Default.Save();
                        }
                        catch (Exception e)
                        {
                            //Changing Modal state for failure
                            Meuh.Content = Properties.Resources.Settings_Modal_Content_UnsuccessfulConnection;
                            Meuh.Title = Properties.Resources.Settings_Modal_Title_UnsuccessfulConnection;
                            Meuh.Action = "LoadKO";
                            EventSystem.Publish(Meuh);

                            //Saving FTP state
                            Properties.QuasarSettings.Default.FTPValid = false;
                            Properties.QuasarSettings.Default.Save();
                        }
                    });
                }
                else
                {
                    //Showing error Modal
                    ModalEvent Meuh = new ModalEvent()
                    {
                        Type = ModalType.Warning,
                        Title = Properties.Resources.Settings_Modal_Title_WrongIpFormat,
                        Content = Properties.Resources.Settings_Modal_Content_WrongIpFormat,
                        OkButtonText = Properties.Resources.Modal_Label_DefaultOK,
                        Action = "Show",
                        EventName = "FTPConnectionWarning"
                    };
                    EventSystem.Publish(Meuh);

                    //Saving FTP state
                    Properties.QuasarSettings.Default.FTPValid = false;
                    Properties.QuasarSettings.Default.Save();
                }
            }
            else
            {
                //Showing error Modal
                ModalEvent Meuh = new ModalEvent()
                {
                    Type = ModalType.Warning,
                    Title = Properties.Resources.Settings_Modal_Title_WrongIpFormat,
                    Content = Properties.Resources.Settings_Modal_Content_WrongIpFormat,
                    OkButtonText = Properties.Resources.Modal_Label_DefaultOK,
                    Action = "Show",
                    EventName = "FTPConnectionWarning"
                };
                EventSystem.Publish(Meuh);

                //Saving FTP state
                Properties.QuasarSettings.Default.FTPValid = false;
                Properties.QuasarSettings.Default.Save();
            }

        }

        /// <summary>
        /// Processes any Modal Event incoming
        /// </summary>
        /// <param name="meuh">The received Modal Event</param>
        public void ProcessModalEvent(ModalEvent meuh)
        {
            switch (meuh.EventName)
            {
                case "AskMoveInstall":
                    //If the user wants to change the install location
                    if(meuh.Action == "OK")
                        MoveInstall();
                    break;
                case "MoveInstall":
                    //If the install location change is finished
                    if(meuh.Action == "OK")
                        Application.Current.Shutdown();
                    break;
                case "ShutdownWarning":
                    //If the install location change is finished
                    if (meuh.Action == "OK")
                        Application.Current.Shutdown();
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Asks the user if he really wants to change the install location
        /// </summary>
        public void AskMoveInstall()
        {
            ModalEvent Meuh = new ModalEvent()
            {
                EventName = "AskMoveInstall",
                Action = "Show",
                Type = ModalType.OkCancel,
                Title = Properties.Resources.Settings_Modal_Title_AskMoveInstall,
                Content = Properties.Resources.Settings_Modal_Content_AskMoveInstall,
                OkButtonText = "OK",
                CancelButtonText = "CANCEL"
            };
            EventSystem.Publish<ModalEvent>(Meuh);
        }

        /// <summary>
        /// Asks the user for a location then the 
        /// install gets transfered to that location
        /// </summary>
        public void MoveInstall()
        {
            //Folder browser dialog
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //Showing user a Modal to block UI while it processes
                string NewInstallPath = dialog.SelectedPath;
                ModalEvent Meuh = new ModalEvent()
                {
                    EventName = "MoveInstall",
                    Action = "Show",
                    Type = ModalType.Loader,
                    Title = Properties.Resources.Settings_Modal_Title_MovingInstall,
                    Content = Properties.Resources.Settings_Modal_Content_MovingInstall,
                    OkButtonText = Properties.Resources.Settings_Modal_OkButton_MovingInstall,
                };
                EventSystem.Publish<ModalEvent>(Meuh);

                //Launching the move task
                Task.Run(() => {
                    InstallManager.ChangeInstallLocation(NewInstallPath);
                });
            }
        }

        /// <summary>
        /// Asks the user if he wants to see the onboarding demonstration
        /// </summary>
        public void AskLaunchOnboarding()
        {
            ModalEvent meuh = new ModalEvent()
            {
                EventName = "Onboarding",
                Action = "Show",
                Type = ModalType.OkCancel,
                Title = "Do you want to see the demo again?",
                Content = "If it's your first time using Quasar, or if you want\ra tour of what you can do, you can have a little demo.",
                OkButtonText = "Yes, please show me around",
                CancelButtonText = "On second thoughts, no"

            };
            EventSystem.Publish<ModalEvent>(meuh);
        }

        public string getSDCards()
        {
            string DriveString = "";

            DriveInfo[] CurrentDrives = DriveInfo.GetDrives();
            foreach (DriveInfo di in CurrentDrives)
            {
                if (di.DriveType == DriveType.Removable && di.IsReady)
                {
                    DriveString += String.Format("{0} ({1})={1}",di.VolumeLabel, di.Name);
                }
            }

            return DriveString;
        }
        #endregion

        #region USB Detection

        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            LoadSettings();
        }

        private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            LoadSettings();
        }

    #endregion
}
}
