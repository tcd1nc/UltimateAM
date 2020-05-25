using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class MovementsReportDateFilterView : Window
    {
        public MovementsReportDateFilterView(Models.ReportModel _rm)         
        {
            InitializeComponent();
            DataContext = new ViewModels.MovementsReportDateFilterViewModel(_rm);
        }
    }
}
