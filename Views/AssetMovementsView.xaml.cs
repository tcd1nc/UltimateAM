using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for AssetMovements.xaml
    /// </summary>
    public partial class AssetMovementsView : Window
    {

        public AssetMovementsView()
        {
            InitializeComponent();
        }

        public AssetMovementsView(int _assetid)
        {
            InitializeComponent();

            DataContext = new ViewModels.AssetMovementsViewModel(_assetid);


        }


    }
}
