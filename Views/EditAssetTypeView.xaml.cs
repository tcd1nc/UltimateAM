using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditAssetType.xaml
    /// </summary>
    public partial class EditAssetType : Window
    {
        public EditAssetType()
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetTypesViewModel();
        }
    }
}
