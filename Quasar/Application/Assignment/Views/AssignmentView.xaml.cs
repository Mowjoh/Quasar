using Quasar.Associations.ViewModels;
using Quasar.Associations.Views;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Quasar.Associations.Views
{
    /// <summary>
    /// Interaction logic for AssignationView.xaml
    /// </summary>
    public partial class AssignmentView : UserControl, INotifyPropertyChanged
    {
        #region Properties
        //Drag and Drop properties
        protected bool m_IsDraging = false;
        protected Point _dragStartPoint;

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
                ;
            }
        }
        #endregion

        public AssignmentView()
        {
            InitializeComponent();
        }

        #region User Actions
 
        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (sender is Slot)
            {
                var source = e.Data.GetData(typeof(Slot)) as Slot;
                var target = (Slot)sender;
                AssignmentViewModel.SetSlot(source, target);
            }
        }
        #endregion

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

        /// <summary>
        /// Displays the Drag Enter mouse visual
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// Displays Drag and Drop visuals
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        /// <summary>
        /// Displays Drag and Drop visuals
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Returns the parent item where the drop occured
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        private T FindVisualParent<T>(DependencyObject child)
            where T : DependencyObject
        {
            try
            {
                var parentObject = VisualTreeHelper.GetParent(child);
                if (parentObject == null)
                    return null;
                T parent = parentObject as T;
                if (parent != null)
                    return parent;
                return FindVisualParent<T>(parentObject);
            }
            catch (Exception e)
            {
                return null;
            }

        }
        #endregion


    }
}
