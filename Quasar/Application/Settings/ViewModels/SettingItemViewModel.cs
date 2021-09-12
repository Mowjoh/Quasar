using Quasar.Common.Models;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.Settings.Models;
using Quasar.Helpers;
using System;
using System.Linq;
using System.Windows.Input;

namespace Quasar.Settings.ViewModels
{
    public class SettingItemViewModel : ObservableObject
    {
        #region Data

        #region Private
        private SettingItem _SettingItem { get; set; }
        #endregion

        #region Public
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
        #endregion

        #endregion

        #region View

        #region Private
        private bool _IsChecked { get; set; }

        private bool _EnableValue { get; set; }

        private bool _EnableCheckBox { get; set; }

        private bool _EnableComboBox { get; set; }

        private bool _EnableTextBox { get; set; }
        #endregion

        #region Public
        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                if (_IsChecked == value)
                    return;

                _IsChecked = value;
                SettingItem.IsChecked = value;
                OnPropertyChanged("IsChecked");

                if (SettingItem.SettingName != null)
                {
                    NotifySettingChanged();
                }

                if ((bool)Properties.Settings.Default[SettingItem.SettingName] == value)
                    return;

                Properties.Settings.Default[SettingItem.SettingName] = value;
                Properties.Settings.Default.Save();

            }
        }

        /// <summary>
        /// Trigger to enable the Value Display mode
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
        /// Enables the Checkbox Display mode
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
        /// Enables the ComboBox Display mode
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

        /// <summary>
        /// Enables the TextBox Display mode
        /// </summary>
        public bool EnableTextBox
        {
            get => _EnableTextBox;
            set
            {
                if (_EnableTextBox == value)
                    return;

                _EnableTextBox = value;
                OnPropertyChanged("EnableTextBox");
            }
        }
        #endregion

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

        #region Actions
        /// <summary>
        /// Loads data associated to the Property
        /// </summary>
        /// <param name="PropertyName">Name of the Property to look for</param>
        void GetPropertyValues(string PropertyName)
        {
            //Parsing setting
            var Property = Properties.Settings.Default[PropertyName];

            //Parsing definition values
            SettingItem.SettingName = PropertyName;
            SettingItem.DisplayName = (string)Properties.SettingsDefinition.Default[PropertyName + "Name"];
            SettingItem.DisplayComment = (string)Properties.SettingsDefinition.Default[PropertyName + "Comment"];
            string Values = (string)Properties.SettingsDefinition.Default[PropertyName + "Values"];

            switch (Property.GetType().Name)
            {
                //Setting up Checkbox View
                case "Boolean":

                    EnableCheckBox = true;
                    IsChecked = (bool)Property;
                    break;


                case "String":
                    //Setting up Combobox View
                    if (Values != "")
                    {
                        SettingItem.Values = Values.Split(',')
                         .Select(x => x.Split('='))
                         .ToDictionary(x => x[0], x => x[1]);
                        if (SettingItem.Values.ContainsKey("USER"))
                        {
                            SettingItem.DisplayValue = Property.ToString();
                            EnableTextBox = true;
                        }
                        else
                        {
                            EnableComboBox = true;
                        }

                    }
                    //Setting up Text View
                    else
                    {
                        EnableValue = true;
                        SettingItem.DisplayValue = Property.ToString();
                    }

                    break;
                //Setting up Combobox View
                case "Integer":
                    SettingItem.Values = Values.Split(',')
                         .Select(x => x.Split('='))
                         .ToDictionary(x => x[0], x => x[1]);

                    EnableComboBox = true;
                    break;
            }

            //Notify UI
            NotifySettingChanged();
        }

        /// <summary>
        /// Publishes a Setting Item System Event
        /// </summary>
        public void NotifySettingChanged()
        {
            EventSystem.Publish<SettingItem>(SettingItem);
        }
        #endregion

        #region User Action

        #endregion

    }
}
