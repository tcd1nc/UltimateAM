using System.Windows;
namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditOperatingCompanyView.xaml
    /// </summary>
    public partial class EditOperatingCompanyView : Window
    {
        public EditOperatingCompanyView()
        {
            InitializeComponent();
            DataContext = new ViewModels.OperatingCompaniesViewModel();
        }
    }
}
