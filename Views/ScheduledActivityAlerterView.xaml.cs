using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for ScheduledActivityAlerterView.xaml
    /// </summary>
    public partial class ScheduledActivityAlerterView : Window
    {
        public ScheduledActivityAlerterView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ScheduledActivityAlerterViewModel();
        }
    }
}
