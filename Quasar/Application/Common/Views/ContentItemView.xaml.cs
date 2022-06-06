using System;
using System.Collections.Generic;
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
using DataModels.User;
using Quasar.Common.ViewModels;

namespace Quasar.Common.Views
{
    /// <summary>
    /// Interaction logic for ContentItemView.xaml
    /// </summary>
    public partial class ContentItemView : UserControl
    {
        public ContentItemViewModel ViewModel { get; set; }

        public ContentItemView(AssignmentContent _assignment)
        {
            ViewModel = new(_assignment);

            InitializeComponent();

            this.DataContext = ViewModel;
        }
    }
}
