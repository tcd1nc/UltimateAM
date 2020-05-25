using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditCustomerView.xaml
    /// </summary>
    public partial class EditCustomerView : Window
    {
        public EditCustomerView(int id)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.CustomerViewModel(id);
        }
    }
}
