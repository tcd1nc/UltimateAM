using System.Windows;
namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : Window
    {
        public ReportView(Models.ReportModel _rm)         
        {
            InitializeComponent();
            DataContext = new ViewModels.ReportViewModel(_rm);
        }
    }
}
