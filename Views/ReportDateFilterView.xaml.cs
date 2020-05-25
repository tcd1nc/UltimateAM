using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportDateFilterView : Window
    {
        public ReportDateFilterView(Models.ReportModel _rm)         
        {
            InitializeComponent();
            DataContext = new ViewModels.ReportDateFilterViewModel(_rm);
        }
    }
}
