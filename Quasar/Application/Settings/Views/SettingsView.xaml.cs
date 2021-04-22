using Quasar.Settings.ViewModels;
using System.Windows.Controls;

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

        public void Load()
        {
            SVM = new SettingsViewModel();
            DataContext = SVM;
        }
    }
}
