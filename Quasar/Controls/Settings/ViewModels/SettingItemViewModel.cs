using Quasar.Controls.Common.Models;
using Quasar.Controls.Settings.Model;
using System;
using System.Linq;
using System.Windows.Input;

namespace Quasar.Controls.Settings.ViewModels
{
    class SettingItemViewModel : ObservableObject
    {
        #region Fields
        
        private SettingItem _SettingItem { get; set; }

        private bool _IsChecked { get; set; }

        private bool _EnableValue { get; set; }

        private bool _EnableCheckBox { get; set; }

        private bool _EnableComboBox { get; set; }

        #endregion

        #region Parameters

        /// <summary>
        /// Setting Item containing the data
        /// </summary>
        public SettingItem SettingItem
        {
            get => _SettingItem;
            set
            {
                if (_SettingItem == value)
                    return;

                _SettingItem = value;
                OnPropertyChanged("SettingItem");
            }
        }

        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                if (_IsChecked == value)
                    return;
                
                _IsChecked = value;
                OnPropertyChanged("IsChecked");

                if (SettingItem.SettingName == null)
                    return;

                if ((bool)Properties.Settings.Default[SettingItem.SettingName] == value)
                    return;

                Properties.Settings.Default[SettingItem.SettingName] = value;
                Properties.Settings.Default.Save();
                
            }
        }

        /// <summary>
        /// Trigger to enable the Value display
        /// </summary>
        public bool EnableValue
        {
            get => _EnableValue;
            set
            {
                if (_EnableValue == value)
                    return;

                _EnableValue = value;
                OnPropertyChanged("EnableValue");
            }
        }

        /// <summary>
        /// Enables the Checkbox
        /// </summary>
        public bool EnableCheckBox
        {
            get => _EnableCheckBox;
            set
            {
                if (_EnableCheckBox == value)
                    return;

                _EnableCheckBox = value;
                OnPropertyChanged("EnableCheckBox");
            }
        }

        /// <summary>
        /// Enables the ComboBox
        /// </summary>
        public bool EnableComboBox
        {
            get => _EnableComboBox;
            set
            {
                if (_EnableComboBox == value)
                    return;

                _EnableComboBox = value;
                OnPropertyChanged("EnableComboBox");
            }
        }

        #endregion

        /// <summary>
        /// Constructor that takes in the Property Name
        /// </summary>
        /// <param name="Property">The Setting to bind the control and UI to</param>
        public SettingItemViewModel(string Property)
        {
            SettingItem = new SettingItem();
            GetPropertyValues(Property);
        }

        /// <summary>
        /// Loads data associated to the Property
        /// </summary>
        /// <param name="PropertyName">Name of the Property to look for</param>
        void GetPropertyValues(string PropertyName)
        {
            var Property = Properties.Settings.Default[PropertyName];

            SettingItem.SettingName = PropertyName;
            SettingItem.DisplayName = (string)Properties.SettingsDefinition.Default[PropertyName + "Name"];
            SettingItem.DisplayComment = (string)Properties.SettingsDefinition.Default[PropertyName + "Comment"];
            string Values = (string)Properties.SettingsDefinition.Default[PropertyName + "Values"];

            switch (Property.GetType().Name)
            {
                case "Boolean":

                    EnableCheckBox = true;
                    IsChecked = (bool)Property;
                    break;

                case "String":
                    if (Values != "")
                    {
                        SettingItem.Values = Values.Split(',')
                         .Select(x => x.Split('='))
                         .ToDictionary(x => x[0], x => x[1]);

                        EnableComboBox = true;
                    }
                    else
                    {
                        EnableValue = true;
                        SettingItem.DisplayValue = Property.ToString();
                    }

                    break;

                case "Integer":
                    SettingItem.Values = Values.Split(',')
                         .Select(x => x.Split('='))
                         .ToDictionary(x => x[0], x => x[1]);

                    EnableComboBox = true;
                    break;
            }
        }

        

        
    }
}
