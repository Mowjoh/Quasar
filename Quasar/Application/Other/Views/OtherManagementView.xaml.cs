using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DataModels.Common;
using log4net;
using Quasar.Skins.ViewModels;
using Quasar.MainUI.ViewModels;
using Quasar.Other.ViewModels;

namespace Quasar.Other.Views
{
    /// <summary>
    /// Interaction logic for SkinManagementView.xaml
    /// </summary>
    public partial class OtherManagementView : UserControl
    {
        public OtherManagementViewModel OMVM { get; set; }

        public OtherManagementView(MainUIViewModel _MUVM, ILog _QuasarLogger)
        {
            InitializeComponent();
            OMVM = new OtherManagementViewModel(_MUVM);
            this.DataContext = OMVM;
        }
    }
}
