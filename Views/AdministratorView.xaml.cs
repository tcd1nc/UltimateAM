using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditUserView.xaml
    /// </summary>
    public partial class AdministratorView : Window
    {
        public AdministratorView()
        {
            InitializeComponent();
            DataContext = new ViewModels.AdministratorsViewModel();
        }
    }
}
