using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditAssetGroup.xaml
    /// </summary>
    public partial class EditAssetGroup : Window
    {
        public EditAssetGroup()
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetGroupsViewModel();
        }
    }
}
