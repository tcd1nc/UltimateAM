using System.Collections.ObjectModel;
using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class AssetMovementsViewModel : ViewModelBase
    {
        Collection<AssetMovementReportModel> _assetmovements = new Collection<AssetMovementReportModel>();
        const string _title = "Asset Movements and Activity Log";
        bool canExecute = true;

        public AssetMovementsViewModel(int _id)
        {
            _assetmovements = GetAssetMovementsReport(_id);
            CloseCommand = new RelayCommand(SelectClose, param => this.canExecute);
        }
       
        public Collection<AssetMovementReportModel> AssetMovements => _assetmovements;

        public string Title => _title;
        
        private void SelectClose(object obj)
        {
            DialogResult = false;
        }

        ICommand _selectclosecommand;
        public ICommand CloseCommand
        {
            get { return _selectclosecommand; }
            set { _selectclosecommand = value; }
        }
               

    }
}
