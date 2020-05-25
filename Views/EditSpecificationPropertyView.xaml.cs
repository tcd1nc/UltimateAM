using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditOperatingCompanyView.xaml
    /// </summary>
    public partial class EditSpecificationPropertyView : Window
    {
        public EditSpecificationPropertyView()
        {
            InitializeComponent();
            DataContext = new ViewModels.SpecificationPropertyViewModel();
        }
    }
}
