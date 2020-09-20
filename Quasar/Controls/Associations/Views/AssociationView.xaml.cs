using Quasar.Controls.Assignation.ViewModels;
using Quasar.Controls.Associations.ViewModels;
using Quasar.Controls.Associations.Views;
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

namespace Quasar.Controls.Assignation.Views
{
    /// <summary>
    /// Interaction logic for AssignationView.xaml
    /// </summary>
    public partial class AssociationView : UserControl, INotifyPropertyChanged
    {
        protected bool m_IsDraging = false;
        protected Point _dragStartPoint;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, e);
            }
        }

        private AssociationViewModel _AssociationViewModel { get; set; }
        public AssociationViewModel AssociationViewModel
        {
            get => _AssociationViewModel;
            set
            {
                if (_AssociationViewModel == value)
                    return;

                _AssociationViewModel = value;
                OnPropertyChanged("AssociationViewModel");
;            }
        }
        public AssociationView()
        {
            InitializeComponent();
        }

        private void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(null);
            Vector diff = _dragStartPoint - point;
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                var lb = sender as ListBox;
                var lbi = FindVisualParent<ListBoxItem>(((DependencyObject)e.OriginalSource));
                if (lbi != null)
                {
                    DragDrop.DoDragDrop(lbi, lbi.DataContext, DragDropEffects.Move);
                }
            }
        }
        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (sender is SlotItem)
            {
                var source = e.Data.GetData(typeof(SlotItem)) as SlotItem;
                var target = (SlotItem)sender;
                AssociationViewModel.SetSlot(source, target);
            }
        }

        private T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
                return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            return FindVisualParent<T>(parentObject);
        }

        
    }
}
