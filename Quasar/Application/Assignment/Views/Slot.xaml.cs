using Quasar.Associations.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;

namespace Quasar.Associations.Views
{
    /// <summary>
    /// Interaction logic for SlotItem.xaml
    /// </summary>
    public partial class Slot : UserControl, INotifyPropertyChanged
    {
        #region Properties

        //Events
        public event PropertyChangedEventHandler PropertyChanged;

        //Data View Model
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

        #endregion
        

        /// <summary>
        /// Constructor
        /// </summary>
        public Slot()
        {
            InitializeComponent();
        }

        #region Actions
        /// <summary>
        /// OnPropertyChanged Event
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, e);
            }
        }
        #endregion

    }
}
