using Quasar.Associations.ViewModels;
using System.ComponentModel;

namespace Quasar.Associations.Views
{
    /// <summary>
    /// Interaction logic for AssignationView.xaml
    /// </summary>
    public partial class AssignmentView : INotifyPropertyChanged
    {
        #region Properties
        //Events
        public event PropertyChangedEventHandler PropertyChanged;

        //Data View Model
        private AssociationViewModel _AssociationViewModel { get; set; }
        public AssociationViewModel AssignmentViewModel
        {
            get => _AssociationViewModel;
            set
            {
                if (_AssociationViewModel == value)
                    return;

                _AssociationViewModel = value;
                OnPropertyChanged("AssignmentViewModel");
            }
        }
        #endregion

        public AssignmentView()
        {
            InitializeComponent();
        }

        #region Actions
        /// <summary>
        /// OnPropertyChanged Event
        /// </summary>
        /// <param name="_property_name"></param>
        protected virtual void OnPropertyChanged(string _property_name)
        {
            if (this.PropertyChanged == null) return;
            PropertyChangedEventArgs e = new (_property_name);
            this.PropertyChanged(this, e);
        }

        #endregion


    }
}
