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
using Quasar.Stages.ViewModels;

namespace Quasar.Stages.Views
{
    /// <summary>
    /// Interaction logic for StageItem.xaml
    /// </summary>
    public partial class StageItem : UserControl
    {
        public StageItemViewModel SIVM { get; set; }
        public StageItem(string _stage_name)
        {
            SIVM = new(_stage_name);
            this.DataContext = SIVM;

            InitializeComponent();
        }
    }
}
