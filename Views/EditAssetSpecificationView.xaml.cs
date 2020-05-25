using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditAssetSpecificationView.xaml
    /// </summary>
    public partial class EditAssetSpecificationView : Window
    {
        public EditAssetSpecificationView()
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetSpecificationsViewModel();
        }
    }
}
