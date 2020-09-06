using Quasar.Controls.Settings.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quasar.Controls.Settings.View
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

        SettingItemViewModel ViewModel
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

        public SettingItemView(string Property)
        {
            ViewModel = new SettingItemViewModel(Property);
            InitializeComponent();
            DataContext = ViewModel;
            
        }

        
    }
}
