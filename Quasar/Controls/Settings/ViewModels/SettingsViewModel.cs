using FluentFTP;
using Quasar.Controls.Common.Models;
using Quasar.Controls.Settings.Model;
using Quasar.Controls.Settings.View;
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
        #endregion

        public SettingsViewModel()
        {
            SettingItems = new ObservableCollection<SettingItemView>();
            SettingItems.Add(new SettingItemView("AppVersion"));
            //SettingItems.Add(new SettingItemView("Language"));
            SettingItems.Add(new SettingItemView("SupressBuildDeletion"));
            SettingItems.Add(new SettingItemView("SupressModDeletion"));
            SettingItems.Add(new SettingItemView("EnableWorkspaces"));

            SettingItems.Add(new SettingItemView("FTPAddress"));
            SettingItems.Add(new SettingItemView("FTPUN"));
            SettingItems.Add(new SettingItemView("FTPPW"));

            SettingItems.Add(new SettingItemView("EnableCreator"));
            SettingItems.Add(new SettingItemView("EnableAdvanced"));
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
                FtpClient ftpClient = new FtpClient(Properties.Settings.Default.FTPAddress.Split(':')[0]);
                ftpClient.Port = int.Parse(Properties.Settings.Default.FTPAddress.Split(':')[1]);
                if (Properties.Settings.Default.FTPUN != "")
                {
                    ftpClient.Credentials = new System.Net.NetworkCredential(Properties.Settings.Default.FTPUN, Properties.Settings.Default.FTPPW);
                }
                try
                {
                    ftpClient.ConnectTimeout = 3000;
                    ftpClient.Connect();
                    MessageBox.Show("Connection to the Switch is successful, you can now use FTP when transferring !", "Success", MessageBoxButton.OK);
                    Properties.Settings.Default.FTPValid = true;
                    Properties.Settings.Default.Save();
                }
                catch(Exception e)
                {
                    MessageBox.Show("Could not connect to the Switch, please check the address and Username/Password if you have them, also check that your Switch is reachable", "Connection Error", MessageBoxButton.OK);
                    Properties.Settings.Default.FTPValid = false;
                    Properties.Settings.Default.Save();
                }
            }
            else
            {
                MessageBox.Show("The Address is not valid", "Wrong IP / Port", MessageBoxButton.OK);
                Properties.Settings.Default.FTPValid = false;
                Properties.Settings.Default.Save();
            }
        }
        #endregion
    }
}
