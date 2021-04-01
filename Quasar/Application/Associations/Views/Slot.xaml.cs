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
    public partial class Slot : UserControl, INotifyPropertyChanged
    {
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, e);
            }
        }

        private SlotViewModel _SlotViewModel { get; set; }
        public SlotViewModel SlotViewModel
        {
            get => _SlotViewModel;
            set
            {
                if (_SlotViewModel == value)
                    return;

                _SlotViewModel = value;
                OnPropertyChanged("SlotViewModel");
            }
        }
        public Slot()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
