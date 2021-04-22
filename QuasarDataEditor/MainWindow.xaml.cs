using Quasar.Common.Models;
using Quasar.Data.V2;
using Quasar.Helpers.Json;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuasarDataEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            QuasarDataEditorViewModel qdevm = new QuasarDataEditorViewModel();
            this.DataContext = qdevm;

            InitializeComponent();

            #if DEBUG
            MaximizeToSecondaryMonitor();
            #endif
        }
        public void MaximizeToSecondaryMonitor()
        {
            var secondaryScreen = Screen.AllScreens[2];

            if (secondaryScreen != null)
            {
                var workingArea = secondaryScreen.WorkingArea;
                this.Left = workingArea.Left;
                this.Top = workingArea.Top;
                this.Width = workingArea.Width;
                this.Height = workingArea.Height;

                if (this.IsLoaded)
                {
                    this.WindowState = WindowState.Maximized;
                }
            }
        }

    }
}
