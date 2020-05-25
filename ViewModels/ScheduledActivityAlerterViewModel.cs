using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class ScheduledActivityAlerterViewModel : ViewModelBase
    {
        FullyObservableCollection<MaintenanceRecordModel> _scheduledaudits = new FullyObservableCollection<MaintenanceRecordModel>();
        FullyObservableCollection<MaintenanceRecordModel> _scheduledmaintenance = new FullyObservableCollection<MaintenanceRecordModel>();

        public ScheduledActivityAlerterViewModel()
        {
            Audits = GetAllScheduledAssetAudits();
            Maintenance = GetAllScheduledMaintenance();
           
        }


        public FullyObservableCollection<MaintenanceRecordModel> Audits
        {
            get { return _scheduledaudits; }
            set { SetField(ref _scheduledaudits, value); }
        }

        public FullyObservableCollection<MaintenanceRecordModel> Maintenance
        {
            get { return _scheduledmaintenance; }
            set { SetField(ref _scheduledmaintenance, value); }
        }


        #region Commands
        bool _canexecuteclose = true;
        private bool CanExecuteClose(object obj)
        {
            return _canexecuteclose;
        }
        ICommand _saveandclose;
        public ICommand SaveAndClose
        {
            get
            {
                if (_saveandclose == null)
                    _saveandclose = new DelegateCommand(CanExecuteClose, ExecuteClose);
                return _saveandclose;
            }
        }

        private void ExecuteClose(object parameter)
        {
            CloseWindow();
        }


        ICommand _delete;
        public ICommand DeleteAudit
        {
            get
            {
                if (_delete == null)
                    _delete = new DelegateCommand(CanExecuteDelete, ExecuteDelete);
                return _delete;
            }
        }

        bool _canexecutedelete = true;
        private bool CanExecuteDelete(object obj)
        {
            return _canexecutedelete;
        }

        private void ExecuteDelete(object parameter)
        {
            int _id = ((MaintenanceRecordModel)parameter).ID;
            if (_id != 0)
            {
                IMessageBoxService _msgbox = new MessageBoxService();

                if (_msgbox.ShowMessage("Do you want to delete this Audit?", "Deleting Audit", GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question) == GenericMessageBoxResult.OK)
                {
                    DeleteItem(_id, "AuditDates");
                    Audits.Remove((MaintenanceRecordModel)parameter);
                }
                _msgbox = null;
            }
        }
        

        ICommand _deletemaintenance;
        public ICommand DeleteMaintenance
        {
            get
            {
                if (_deletemaintenance == null)
                    _deletemaintenance = new DelegateCommand(CanExecuteDelete, ExecuteDeleteMaintenance);
                return _deletemaintenance;
            }
        }

        private void ExecuteDeleteMaintenance(object parameter)
        {
            int _id = ((MaintenanceRecordModel)parameter).ID;
            if (_id != 0)
            {
                IMessageBoxService _msgbox = new MessageBoxService();

                if (_msgbox.ShowMessage("Do you want to delete this Maintenance?", "Deleting Maintenance", GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question) == GenericMessageBoxResult.OK)
                {
                    DeleteItem(_id, "MaintenanceRecords");
                    Maintenance.Remove((MaintenanceRecordModel)parameter);
                }
                _msgbox = null;
            }
        }

        #endregion


    }
}
