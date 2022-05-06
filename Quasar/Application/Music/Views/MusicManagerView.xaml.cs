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
using log4net;
using Quasar.Music.ViewModels;
using Quasar.MainUI.ViewModels;

namespace Quasar.Music.Views
{
    /// <summary>
    /// Interaction logic for MusicManagerView.xaml
    /// </summary>
    public partial class MusicManagerView : UserControl
    {
        public MusicManagerViewModel MMVM { get; set; }

        public MusicManagerView(MainUIViewModel _muvm, ILog _quasar_logger)
        {
            MMVM = new(_muvm);
            this.DataContext = MMVM;

            InitializeComponent();
        }
    }
}
