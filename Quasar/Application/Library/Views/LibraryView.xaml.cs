using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Quasar.Controls.ModManagement.Views
{
    /// <summary>
    /// Interaction logic for ModManagementView.xaml
    /// </summary>
    public partial class LibraryView : UserControl
    {
        public LibraryView()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseRightButtonUp(object _sender, MouseButtonEventArgs _e)
        {
            TextBlock tb = (TextBlock) _sender;
            Clipboard.SetText(tb.Text);
        }
    }
}
