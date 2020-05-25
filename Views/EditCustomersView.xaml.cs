using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditCustomerView.xaml
    /// </summary>
    public partial class EditCustomersView : Window
    {
        public EditCustomersView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.CustomersViewModel();
        }
    }
}
