using MahApps.Metro.Controls;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for DataErrorsView.xaml
    /// </summary>
    public partial class DataErrorsView : MetroWindow
    {
        public DataErrorsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.DataErrorsViewModel();
        }
    }
}
