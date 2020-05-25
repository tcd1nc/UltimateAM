using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for TypeSpecificationsView.xaml
    /// </summary>
    public partial class GroupSpecificationsView : Window
    {
        public GroupSpecificationsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.GroupSpecificationsViewModel();
        }
    }
}
