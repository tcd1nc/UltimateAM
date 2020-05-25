using System.Windows;
using System.Windows.Input;


namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for SelectCustomerView.xaml
    /// </summary>
    public partial class SelectCustomerView : Window
    {
        public SelectCustomerView()
        {
            InitializeComponent();
            DataContext = new ViewModels.SelectCustomerViewModel();
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
