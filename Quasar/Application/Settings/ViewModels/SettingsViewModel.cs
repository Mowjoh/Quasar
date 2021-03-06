﻿using FluentFTP;
using Quasar.Common.Models;
using Quasar.Settings.Views;
using Quasar.Helpers.Quasar_Management;
using Quasar.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Quasar.Settings.ViewModels
{
    class SettingsViewModel : ObservableObject
    {
        #region Data

        #region Private
        ObservableCollection<SettingItemView> _SettingItems { get; set; }
        #endregion

        #region Public
        /// <summary>
        /// List of Setting Items views
        /// </summary>
        public ObservableCollection<SettingItemView> SettingItems
        {
            get => _SettingItems;
            set
            {
                if (_SettingItems == value)
                    return;

                _SettingItems = value;
                OnPropertyChanged("SettingItems");
            }
        }
        #endregion

        #endregion

        #region Commands

        #region Private
        private ICommand _ValidateFTPCommand { get; set; }
        private ICommand _MoveInstallCommand { get; set; }
        private ICommand _OnboardingCommand { get; set; }
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
        public ICommand OnboardingCommand
        {
            get
            {
                if (_OnboardingCommand == null)
                {
                    _OnboardingCommand = new RelayCommand(param => AskLaunchOnboarding());
                }
                return _OnboardingCommand;
            }
        }
        #endregion

        #endregion

        public SettingsViewModel()
        {
            LoadSettings();

            EventSystem.Subscribe<ModalEvent>(ProcessModalEvent);
        }

        #region Actions
        /// <summary>
        /// Adds specific settings to the collection
        /// </summary>
        private void LoadSettings()
        {
            SettingItems = new ObservableCollection<SettingItemView>();
            SettingItems.Add(new SettingItemView("AppVersion"));
            //SettingItems.Add(new SettingItemView("Language"));

            SettingItems.Add(new SettingItemView("FTPAddress"));
            SettingItems.Add(new SettingItemView("FTPUN"));
            SettingItems.Add(new SettingItemView("FTPPW"));

            SettingItems.Add(new SettingItemView("SupressModDeletion"));

            SettingItems.Add(new SettingItemView("ModLoaderSetup"));
            SettingItems.Add(new SettingItemView("ModLoaderSetupState"));

            SettingItems.Add(new SettingItemView("EnableWorkspaces"));
            SettingItems.Add(new SettingItemView("EnableCreator"));
            SettingItems.Add(new SettingItemView("EnableAdvanced"));
        }
        #endregion

        #region User Actions
        /// <summary>
        /// Launches the FTP validation task and Modal
        /// </summary>
        public void ValidateFTP()
        {
            //Parsing values
            foreach(SettingItemView SIV in SettingItems)
            {
                switch (SIV.ViewModel.SettingItem.SettingName)
                {
                    case "FTPAddress":
                        Properties.Settings.Default.FTPAddress = SIV.ViewModel.SettingItem.DisplayValue;
                        break;
                    case "FTPUN":
                        Properties.Settings.Default.FTPUN = SIV.ViewModel.SettingItem.DisplayValue;
                        break;
                    case "FTPPW":
                        Properties.Settings.Default.FTPPW = SIV.ViewModel.SettingItem.DisplayValue;
                        break;
                }
                Properties.Settings.Default.Save();
            }

            //Validating values
            Regex AddressRegex = new Regex(@"(?'IP'(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})):(?'Port'(\d{1,5}))");
            if (AddressRegex.IsMatch(Properties.Settings.Default.FTPAddress))
            {
                //Showing connection Modal
                ModalEvent Meuh = new ModalEvent()
                {
                    Type = ModalType.Loader,
                    Title = "Connection Test",
                    Content = "Please wait while Quasar attempts to connect to the Switch",
                    OkButtonText = "OK",
                    Action = "Show",
                    EventName = "FTPConnectionTest"
                };
                EventSystem.Publish(Meuh);

                //Configuring FTP Client
                FtpClient ftpClient = new FtpClient(Properties.Settings.Default.FTPAddress.Split(':')[0]);
                ftpClient.Port = int.Parse(Properties.Settings.Default.FTPAddress.Split(':')[1]);
                if (Properties.Settings.Default.FTPUN != "")
                {
                    ftpClient.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.FTPUN, Properties.Settings.Default.FTPPW);
                }

                //Launching connection Task
                Task.Run(() => {
                    try
                    {
                        ftpClient.ConnectTimeout = 3000;
                        ftpClient.Connect();

                        //Changing Modal state for success
                        Meuh.Content = "Connection to the Switch is successful\r you can now use FTP when transferring !";
                        Meuh.Title = "Success";
                        Meuh.Action = "LoadOK";
                        EventSystem.Publish(Meuh);

                        //Saving FTP state
                        Properties.Settings.Default.FTPValid = true;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception e)
                    {
                        //Changing Modal state for failure
                        Meuh.Content = "Connection to the Switch is unsuccessful\r please check the address and Username/Password if you have them,\ralso check that your Switch is reachable";
                        Meuh.Title = "Connection Error";
                        Meuh.Action = "LoadKO";
                        EventSystem.Publish(Meuh);

                        //Saving FTP state
                        Properties.Settings.Default.FTPValid = false;
                        Properties.Settings.Default.Save();
                    }
                });
            }
            else
            {
                //Showing error Modal
                ModalEvent Meuh = new ModalEvent()
                {
                    Type = ModalType.Warning,
                    Title = "Invalid Info",
                    Content = "The Address is not valid, please check that\r you have entered the right IP/Port.\rAlso check that your FTP server is running",
                    OkButtonText = "OK",
                    Action = "Show",
                    EventName = "FTPConnectionWarning"
                };
                EventSystem.Publish(Meuh);

                //Saving FTP state
                Properties.Settings.Default.FTPValid = false;
                Properties.Settings.Default.Save();
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
                Title = "CHANGE INSTALLATION",
                Content = "Do you want to change where Quasar stores it's files ? \rIt will take some time to transfer then will shutdown.",
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
                    Title = "Moving files",
                    Content = "The operation is in progress, please wait for it to finish",
                    OkButtonText = "Shutdown Quasar",
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
        #endregion
    }
}
