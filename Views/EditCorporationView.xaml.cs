using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditCorporation.xaml
    /// </summary>
    public partial class EditCorporation : Window
    {
        public EditCorporation()
        {
            InitializeComponent();
            DataContext = new ViewModels.CorporationsViewModel();
        }
    }
}
