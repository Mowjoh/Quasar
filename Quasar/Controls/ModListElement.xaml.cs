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
        public int modAssociation;

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
            modAssociation = _mod.association;
            Title.Content = _mod.Name;
            Status.Content = "Up to date";
        }

        public void refreshUI()
        {
            Title.Content = LocalMod.Name;
            Status.Content = "Up to date";
        }
    }
}
