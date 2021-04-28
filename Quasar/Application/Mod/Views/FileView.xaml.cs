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
using System.Windows.Shapes;

namespace Quasar.Controls
{
    /// <summary>
    /// Interaction logic for FileView.xaml
    /// </summary>
    public partial class FileView : Window
    {
        public FileView()
        {

            InitializeComponent();

        }

        public FileView(String path, String ModName)
        {
            if (Directory.Exists(path))
            {
                InitializeComponent();

                Trees.Items.Clear();

                foreach (string s in Directory.GetDirectories(path))
                {
                    var rootDirectory = new DirectoryInfo(s);
                    Trees.Items.Add(CreateDirectoryNode(rootDirectory));
                }

                ModNameLabel.Content = ModName;

                ExpandTree(Trees);
            }
        }

        private static TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem
            {
                Header = directoryInfo.Name,
                Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"]
        };
            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.Items.Add(CreateDirectoryNode(directory));

            foreach (var file in directoryInfo.GetFiles())
                directoryNode.Items.Add(new TreeViewItem { Header = file.Name, Foreground = (SolidColorBrush)App.Current.Resources["QuasarTextColor"] });

            return directoryNode;
        }

        private void ExpandTree(TreeView tv)
        {
            foreach (var item in tv.Items)
            {
                if (item is TreeViewItem tvi)
                {
                    tvi.ExpandSubtree();
                }
            }
        }

        private void MinimizeTree(TreeView tv)
        {
            foreach (var item in tv.Items)
            {
                if (item is TreeViewItem tvi)
                {
                    tvi.IsExpanded = false;
                }
            }
        }
    }
}
