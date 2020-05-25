using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for UserSettingsView.xaml
    /// </summary>
    public partial class UserSettingsView : Window
    {
        public UserSettingsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.UserSettingsViewModel();
        }
    }
}
