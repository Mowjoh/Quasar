using Quasar.Controls.Associations.ViewModels;
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

namespace Quasar.Controls.Associations.Views
{
    /// <summary>
    /// Interaction logic for SlotItem.xaml
    /// </summary>
    public partial class SlotItem : UserControl, INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, e);
            }
        }

        private SlotItemViewModel _SlotItemViewModel { get; set; }
        public SlotItemViewModel SlotItemViewModel
        {
            get => _SlotItemViewModel;
            set
            {
                if (_SlotItemViewModel == value)
                    return;

                _SlotItemViewModel = value;
                OnPropertyChanged("SlotItemViewModel");
            }
        }
        public SlotItem()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
