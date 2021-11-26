using Quasar.Settings.ViewModels;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Quasar.Settings.Views
{
    /// <summary>
    /// Interaction logic for SettingItemView.xaml
    /// </summary>
    public partial class SettingItemView : UserControl, INotifyPropertyChanged
    {
        #region Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String PropertyName)
        {
            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(PropertyName);
                this.PropertyChanged(this, e);
            }
        }

        #endregion

        private SettingItemViewModel _ViewModel { get; set; }

        public SettingItemViewModel ViewModel
        {
            get => _ViewModel;
            set
            {
                if (_ViewModel == value)
                    return;

                _ViewModel = value;
                OnPropertyChanged("ViewModel");
            }
        }

        public SettingItemView(string Property, string DisplayName, string HoverComment, SettingItemType Type, string Values = "")
        {
            ViewModel = new SettingItemViewModel(Property, DisplayName, HoverComment, Type, Values);
            InitializeComponent();
            DataContext = ViewModel;
            
        }

        
    }
}
