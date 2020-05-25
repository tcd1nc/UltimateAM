using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditAssetAreaView.xaml
    /// </summary>
    public partial class EditAssetAreaView : Window
    {
        public EditAssetAreaView()
        {
            InitializeComponent();
            DataContext = new ViewModels.AssetAreasViewModel();
        }
    }
}
