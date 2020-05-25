using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditCountryView.xaml
    /// </summary>
    public partial class EditCountryView : Window
    {
        public EditCountryView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.CountriesViewModel();
        }
    }
}
