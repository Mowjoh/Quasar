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
using Quasar.Skins.ViewModels;

namespace Quasar.Skins.Views
{
    /// <summary>
    /// Interaction logic for CharacterItem.xaml
    /// </summary>
    public partial class CharacterItem : UserControl
    {
        public CharacterItemViewModel CIVM { get; set; }
        public CharacterItem(string ch)
        {
            
            CIVM = new CharacterItemViewModel(ch);
            this.DataContext = CIVM;

            InitializeComponent();
        }
    }
}
