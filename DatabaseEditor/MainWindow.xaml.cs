using DatabaseEditor.ViewModels;
using System.Windows;

namespace DatabaseEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DatabaseEditorViewModel devm = new DatabaseEditorViewModel();
            this.DataContext = devm;

            InitializeComponent();
        }
    }
}
