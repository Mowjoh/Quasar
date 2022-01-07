using Quasar.Settings.ViewModels;
using System.Windows.Controls;
using log4net;

namespace Quasar.Settings.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        SettingsViewModel SVM { get; set; }

        public SettingsView()
        {
           
            InitializeComponent();
        }

        public void Load(ILog quasar_logger)
        {
            SVM = new SettingsViewModel(quasar_logger);
            DataContext = SVM;
        }
    }
}
