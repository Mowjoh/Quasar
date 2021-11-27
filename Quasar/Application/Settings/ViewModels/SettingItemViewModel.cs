using Quasar.Common.Models;
using DataModels.Common;
using DataModels.User;
using DataModels.Resource;
using Quasar.Settings.Models;
using Quasar.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<SettingKeyValue> _AvailableValues { get; set; }
        private SettingKeyValue _SelectedBoxValue { get; set; }
        private string _TextBoxValue { get; set; }
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
        public SettingKeyValue SelectedBoxValue
        {
            get => _SelectedBoxValue;
            set
            {
                if (_SelectedBoxValue == value)
                    return;

                _SelectedBoxValue = value;
                OnPropertyChanged("SelectedBoxValue");

                if ((string)Properties.Settings.Default[SettingItem.SettingName] == value.Value)
                    return;

                Properties.Settings.Default[SettingItem.SettingName] = value.Value;
                Properties.Settings.Default.Save();

                if (SettingItem.SettingName == "Language")
                {
                    EventSystem.Publish<ModalEvent>(new()
                    {
                        Action = "Show",
                        Content = Properties.Resources.Settings_Modal_Content_ShutdownWarning,
                        Title = Properties.Resources.Settings_Modal_Title_ShutdownWarning,
                        OkButtonText = Properties.Resources.Settings_Modal_Button_ShutdownWarning,
                        EventName = "ShutdownWarning",
                        Type = ModalType.Warning
                    });
                }

            }
        }

        public ObservableCollection<SettingKeyValue> AvailableValues
        {
            get => _AvailableValues;
            set
            {
                if (_AvailableValues == value)
                    return;

                _AvailableValues = value;
                OnPropertyChanged("AvailableValues");
            }
        }
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

        public string TextBoxValue
        {
            get => _TextBoxValue;
            set
            {
                if (_TextBoxValue == value)
                    return;

                _TextBoxValue = value;
                OnPropertyChanged("TextBoxValue");
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Constructor that takes in the Property Name
        /// </summary>
        /// <param name="Property">The Setting to bind the control and UI to</param>
        public SettingItemViewModel(string Property, string DisplayName, string HoverComment, SettingItemType Type, string Values = "")
        {
            SettingItem = new SettingItem();

            //Parsing setting
            var ParsedProperty = Properties.Settings.Default[Property];

            //Parsing definition values
            SettingItem.SettingName = Property;
            SettingItem.DisplayName = DisplayName;
            SettingItem.DisplayComment = HoverComment;

            switch (Type)
            {
                //Setting up Checkbox View
                case SettingItemType.Toggle:

                    EnableCheckBox = true;
                    IsChecked = (bool)ParsedProperty;
                    break;


                case SettingItemType.Input:
                    EnableTextBox = true;
                    SettingItem.DisplayValue = (string) ParsedProperty;
                    break;

                //Setting up Combobox View
                case SettingItemType.List:
                    AvailableValues = new();
                    foreach (string s in Values.Split(','))
                    {
                        string key = s.Split('=')[0];
                        string value = s.Split('=')[1];
                        AvailableValues.Add(new SettingKeyValue(key, value));
                    }

                    SelectedBoxValue =
                        AvailableValues.SingleOrDefault(v => v.Value == Properties.Settings.Default[Property].ToString());
                    EnableComboBox = true;
                    break;

                case SettingItemType.Text:
                    EnableValue = true;
                    SettingItem.DisplayValue = Properties.Settings.Default[Property].ToString();
                    break;
            }

            NotifySettingChanged();
        }

        #region Actions

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

    public enum SettingItemType { Toggle, Input, List, Text}

    public class SettingKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public SettingKeyValue(string k, string v)
        {
            Key = k;
            Value = v;
        }
    }
}
