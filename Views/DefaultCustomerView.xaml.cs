using System.Windows.Input;
using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for DefaultCustomerView.xaml
    /// </summary>
    public partial class DefaultCustomerView : Window
    {
        public DefaultCustomerView()
        {
            InitializeComponent();
            DataContext = new ViewModels.DefaultCustomerViewModel();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }

    }
}
