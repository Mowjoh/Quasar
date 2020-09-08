using System.Windows;

namespace Quasar
{
    public partial class MainWindow : Window
    {
        MainUIViewModel MUVM { get; set; }
        public MainWindow()
        {
            MUVM = new MainUIViewModel();
            
            //Aww, here we go again
            InitializeComponent();

            QuasarGrid.DataContext = MUVM;
        }

    }
    
}
