using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for AvailableAssets.xaml
    /// </summary>
    public partial class AvailableAssetsView : Window
    {
     

        public AvailableAssetsView(object[] _assetdata)
        {
            InitializeComponent();
            DataContext =new ViewModels.AvailableAssetsViewModel(_assetdata);
        }

            
    }
}
