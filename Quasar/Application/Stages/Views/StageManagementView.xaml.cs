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
using Grpc.Core.Logging;
using log4net;
using Quasar.MainUI.ViewModels;
using Quasar.Stages.ViewModels;

namespace Quasar.Stages.Views
{
    /// <summary>
    /// Interaction logic for StageManagementView.xaml
    /// </summary>
    public partial class StageManagementView : UserControl
    {
        
        public StageManagementView(MainUIViewModel _muvm, ILog _quasar_logger)
        {
            StageManagementViewModel SMVM = new StageManagementViewModel(_muvm);
            this.DataContext = SMVM;

            InitializeComponent();
        }
    }
}
