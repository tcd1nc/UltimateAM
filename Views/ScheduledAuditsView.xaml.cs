using System.Windows;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for ScheduledAuditsView.xaml
    /// </summary>
    public partial class ScheduledAuditsView : Window
    {
        public ScheduledAuditsView(int _id)
        {
            InitializeComponent();
            DataContext = new ViewModels.ScheduledAuditsViewModel(_id);
        }

    }
}
