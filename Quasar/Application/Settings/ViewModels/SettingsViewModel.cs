using FluentFTP;
using Quasar.Controls.Common.Models;
using Quasar.Controls.Settings.Model;
using Quasar.Controls.Settings.View;
using Quasar.Helpers.Quasar_Management;
using Quasar.Internal;
using Quasar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Quasar
{
    class SettingsViewModel : ObservableObject
    {
        #region Fields
        /// <summary>
        /// List of Setting Items views
        /// </summary>
        ObservableCollection<SettingItemView> _SettingItems { get; set; }

        private ICommand _ValidateFTPCommand { get; set; }
        private ICommand _MoveInstallCommand { get; set; }
        private ICommand _OnboardingCommand { get; set; }
        #endregion

        #region Parameters
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
                    _MoveInstallCommand = new RelayCommand(param => MoveInstall());
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
                    _OnboardingCommand = new RelayCommand(param => Onboarding());
                }
                return _OnboardingCommand;
            }
        }
        #endregion

        public SettingsViewModel()
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

            EventSystem.Subscribe<ModalEvent>(ModalEvent);
        }

        #region Actions
        public void ValidateFTP()
        {
 

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

            Regex AddressRegex = new Regex(@"(?'IP'(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})):(?'Port'(\d{1,5}))");
            if (AddressRegex.IsMatch(Properties.Settings.Default.FTPAddress))
            {
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

                FtpClient ftpClient = new FtpClient(Properties.Settings.Default.FTPAddress.Split(':')[0]);
                ftpClient.Port = int.Parse(Properties.Settings.Default.FTPAddress.Split(':')[1]);
                if (Properties.Settings.Default.FTPUN != "")
                {
                    ftpClient.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.FTPUN, Properties.Settings.Default.FTPPW);
                }

                Task.Run(() => {
                    try
                    {
                        ftpClient.ConnectTimeout = 3000;
                        ftpClient.Connect();
                        Meuh.Content = "Connection to the Switch is successful\r you can now use FTP when transferring !";
                        Meuh.Title = "Success";
                        Meuh.Action = "LoadOK";
                        EventSystem.Publish(Meuh);

                        Properties.Settings.Default.FTPValid = true;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception e)
                    {
                        Meuh.Content = "Connection to the Switch is unsuccessful\r please check the address and Username/Password if you have them,\ralso check that your Switch is reachable";
                        Meuh.Title = "Connection Error";
                        Meuh.Action = "LoadKO";
                        EventSystem.Publish(Meuh);

                        Properties.Settings.Default.FTPValid = false;
                        Properties.Settings.Default.Save();
                    }
                });
            }
            else
            {
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

                Properties.Settings.Default.FTPValid = false;
                Properties.Settings.Default.Save();
            }
        }

        public void ModalEvent(ModalEvent meuh)
        {
            switch (meuh.EventName)
            {
                case "AskMoveInstall":
                    if(meuh.Action == "OK")
                    {
                        var dialog = new System.Windows.Forms.FolderBrowserDialog();
                        System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                        if(result == System.Windows.Forms.DialogResult.OK)
                        {
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

                            Task.Run(() => {
                                InstallManager.ChangeInstallLocation(NewInstallPath);
                            });
                        }
                        
                    }
                    break;
                case "MoveInstall":
                    if(meuh.Action == "OK")
                    {
                        Application.Current.Shutdown();
                    }
                    break;
                default: break;
            }
        }

        public void MoveInstall()
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

        public void Onboarding()
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
