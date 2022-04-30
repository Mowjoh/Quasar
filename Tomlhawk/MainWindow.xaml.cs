using System;
using System.Collections.Generic;
using System.IO;
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
using Workshop;
using DataModels;

namespace Tomlhawk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            OverwriteCheckBox.IsChecked = Settings.Default.OverwriteChecked;
            GetFlashDrives();
        }

        private void LaunchTomlProcess(object sender, RoutedEventArgs e)
        {

        }

        private void OverwriteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.OverwriteChecked = OverwriteCheckBox.IsChecked != null && (OverwriteCheckBox.IsChecked == true ? true : false);
            Settings.Default.Save();
        }

        public string GetFlashDrives()
        {
            string DriveString = "";

            DriveInfo[] CurrentDrives = DriveInfo.GetDrives();

            foreach (DriveInfo di in CurrentDrives)
            {
                SDComboBox.Items.Add(new FlashDrive()
                {
                    Name = di.VolumeLabel,
                    VolumeLabel = di.Name
                });
            }

            return DriveString;
        }

    }

    public class FlashDrive
    {
        public string Name { get; set; }
        public string VolumeLabel { get; set; }
    }
}
