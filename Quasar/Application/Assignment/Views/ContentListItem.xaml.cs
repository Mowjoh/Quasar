using System.Collections.Generic;
using DataModels.User;
using Quasar.Associations.ViewModels;

namespace Quasar.Associations.Views
{
    /// <summary>
    /// Interaction logic for ContentListItem.xaml
    /// </summary>
    public partial class ContentListItem 
    {
        private ContentListItemViewModel _ContentListItemViewModel { get; }
        public ContentListItem(AssignmentContent _assignment_content, string _type_name, string _element_name,string _origin, ContentTypes _type, List<Option> _options)
        {
            _ContentListItemViewModel = new ContentListItemViewModel(_assignment_content, _type_name, _element_name, _origin, _type, _options);
            InitializeComponent();
            this.DataContext = _ContentListItemViewModel;
        }
    }

    
}
