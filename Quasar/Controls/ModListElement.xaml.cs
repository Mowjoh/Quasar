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
using static Quasar.Library;

namespace Quasar.Controls
{
    /// <summary>
    /// Interaction logic for SkinItem.xaml
    /// </summary>
    public partial class ModListElement : UserControl
    {
        public int modID;
        public int modType;
        public int modCategory;
        public bool modProcessed;

        public Mod LocalMod;

        public bool Downloaded = false;

        public ModListElement()
        {
            InitializeComponent();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        public void setMod(Mod _mod)
        {
            LocalMod = _mod;

            modID = _mod.id;
            modType = _mod.type;
            modCategory = _mod.category;
            modProcessed = _mod.processed;
            Title.Content = _mod.Name;
            Category.Content = _mod.categoryName;
            TypeLabel.Content = _mod.typeName;
            setModTypeColor(modType);
            Status.Content = "Up to date";

        }

        public void refreshUI()
        {
            Title.Content = LocalMod.Name;
            Category.Content = LocalMod.categoryName;
            TypeLabel.Content = LocalMod.typeName;
            setModTypeColor(modType);
            Status.Content = "Up to date";
        }

        public void setModTypeColor(int type)
        {
            SolidColorBrush brush;
            switch (type)
            {
                case 0:
                    brush = FindResource("BlueText") as SolidColorBrush;
                    break;
                case 1:
                    brush = FindResource("RedText") as SolidColorBrush;
                    break;
                case 2:
                    brush = FindResource("GreenText") as SolidColorBrush;
                    break;
                case 3:
                    brush = FindResource("PurpleText") as SolidColorBrush;
                    break;
                case 4:
                    brush = FindResource("DarkBlueText") as SolidColorBrush;
                    break;
                default:
                    brush = FindResource("BlackText") as SolidColorBrush;
                    break;
            }
            Title.Foreground = brush;
            Category.Foreground = brush;
            TypeLabel.Foreground = brush;
            Status.Foreground = brush;
            Border.Stroke = brush;
        }
    }
}
