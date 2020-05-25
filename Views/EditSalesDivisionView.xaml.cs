using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for EditSalesDivisionView.xaml
    /// </summary>
    public partial class EditSalesDivisionView : Window
    {
        public EditSalesDivisionView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.SalesDivisionsViewModel();
        }
    }
}
