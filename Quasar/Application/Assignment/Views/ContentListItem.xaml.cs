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
using DataModels.User;
using Quasar.Associations.ViewModels;

namespace Quasar.Associations.Views
{
    /// <summary>
    /// Interaction logic for ContentListItem.xaml
    /// </summary>
    public partial class ContentListItem : UserControl
    {
        private ContentListItemViewModel ContentListItemViewModel { get; set; }
        public ContentListItem(ContentItem ci, string TypeName, string ElementName, ContentTypes Type, List<Option> Options)
        {
            ContentListItemViewModel = new(ci, TypeName, ElementName, Type, Options);
            InitializeComponent();
            this.DataContext = ContentListItemViewModel;
        }
    }

    
}
