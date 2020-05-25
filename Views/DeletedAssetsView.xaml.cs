using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for DeletedAssetsView.xaml
    /// </summary>
    public partial class DeletedAssetsView : Window
    {
        public DeletedAssetsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.DeletedAssetsViewModel();
        }
    }
}
