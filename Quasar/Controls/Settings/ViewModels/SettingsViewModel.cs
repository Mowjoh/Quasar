using Quasar.Controls.Common.Models;
using Quasar.Controls.Settings.Model;
using Quasar.Controls.Settings.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
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
        #endregion

        public SettingsViewModel()
        {
            SettingItems = new ObservableCollection<SettingItemView>();
            SettingItems.Add(new SettingItemView("AppVersion"));
            //SettingItems.Add(new SettingItemView("Language"));
            SettingItems.Add(new SettingItemView("SupressBuildDeletion"));
            SettingItems.Add(new SettingItemView("SupressModDeletion"));
            SettingItems.Add(new SettingItemView("EnableIMT"));
            SettingItems.Add(new SettingItemView("FTPAddress"));
            SettingItems.Add(new SettingItemView("FTPUN"));
            SettingItems.Add(new SettingItemView("FTPPW"));
        }
    }
}
