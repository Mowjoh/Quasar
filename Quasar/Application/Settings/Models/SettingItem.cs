using Quasar.Common.Models;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using System.Collections.Generic;

namespace Quasar.Settings.Models
{
    public class SettingItem : ObservableObject
    {
        #region Private
        private string _SettingName { get; set; }

        private string _DisplayName { get; set; }

        private string _DisplayComment { get; set; }

        private string _DisplayValue { get; set; }

        private bool _IsChecked { get; set; }

        private Dictionary<string, string> _Values { get; set; }
        #endregion

        #region Public

        /// <summary>
        /// Represents the name of the Setting to change
        /// </summary>
        public string SettingName
        {
            get => _SettingName;
            set
            {
                if (_SettingName == value)
                    return;

                _SettingName = value;
                OnPropertyChanged("SettingName");
            }
        }

        /// <summary>
        /// Name of the setting to display
        /// </summary>
        public string DisplayName
        {
            get => _DisplayName;
            set
            {
                if (_DisplayName == value)
                    return;

                _DisplayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        /// <summary>
        /// Text of the comment to display
        /// </summary>
        public string DisplayComment
        {
            get => _DisplayComment;
            set
            {
                if (_DisplayComment == value)
                    return;

                _DisplayComment = value;
                OnPropertyChanged("DisplayComment");
            }
        }

        /// <summary>
        /// Text of the value to display
        /// </summary>
        public string DisplayValue
        {
            get => _DisplayValue;
            set
            {
                if (_DisplayValue == value)
                    return;

                _DisplayValue = value;
                OnPropertyChanged("DisplayValue");
            }
        }
        /// <summary>
        /// Value of the boolean setting
        /// </summary>
        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                if (_IsChecked == value)
                    return;

                _IsChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        /// <summary>
        /// List of Key/Values corresponding to the setting
        /// </summary>
        public Dictionary<string,string> Values
        {
            get => _Values;
            set
            {
                if (_Values == value)
                    return;

                _Values = value;
                OnPropertyChanged("Values");
            }
        }

        #endregion
    }
}
