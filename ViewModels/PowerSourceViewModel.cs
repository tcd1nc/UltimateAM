using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class PowerSourceViewModel : ViewModelBase
    {
        FullyObservableCollection<Models.PowerSourceModel> _powersources = new FullyObservableCollection<Models.PowerSourceModel>();
        Models.PowerSourceModel _powersource;
        bool _isediting;
        const string selectedassetlabel = "Selected Power Source:";
        const string newassetlabel = "New Power Source:";
        
        public PowerSourceViewModel()
        {
            PowerSources = StaticClasses.GlobalClass.AssetPowerSources;

            //populate from database
            if (PowerSources.Count > 0)
                AssetLabel = selectedassetlabel;
            else
                AssetLabel = newassetlabel;
            _powersource = new Models.PowerSourceModel();
            _isediting = true;
            _scrolltolastitem = false;
            ScrollToSelectedItem = 0;
        }

        public FullyObservableCollection<Models.PowerSourceModel> PowerSources
        {
            get { return _powersources; }
            set { SetField(ref _powersources, value); }
        }

        public Models.PowerSourceModel PowerSource
        {
            get { return _powersource; }
            set
            {
                if (value != null)
                    SetField(ref _powersource, value);
            }
        }

        string _assetlabel;
        public string AssetLabel
        {
            get { return _assetlabel; }
            set { SetField(ref _assetlabel, value); }
        }

        public bool PowerSourcesListEnabled
        {
            get { return _isediting; }
            set { SetField(ref _isediting, value); }
        }

        bool _scrolltolastitem;
        public bool ScrollToLastItem
        {
            get { return _scrolltolastitem; }
            set { SetField(ref _scrolltolastitem, value); }
        }

        int _scrolltoselecteditem;
        public int ScrollToSelectedItem
        {
            get { return _scrolltoselecteditem; }
            set { SetField(ref _scrolltoselecteditem, value); }
        }

        int _selecteditemindex;
        public int SelectedItemIndex
        {
            get { return _selecteditemindex; }
            set
            {
                if (value > -1)
                    _canexecutedelete = true;
                SetField(ref _selecteditemindex, value);
            }
        }

        #region Commands

        ICommand _addnew;
        public ICommand AddNew
        {
            get
            {
                if (_addnew == null)
                    _addnew = new DelegateCommand(CanExecuteAddNew, ExecuteAddNew);
                return _addnew;
            }
        }

        bool _canexecuteadd = true;
        private bool CanExecuteAddNew(object obj)
        {
            string error = (_powersource as IDataErrorInfo)["PowerSource"];
            if (!string.IsNullOrEmpty(error))
                return false;

            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            _canexecuteadd = false;
            PowerSources.Add(new Models.PowerSourceModel());
            ScrollToLastItem = true;
            PowerSourcesListEnabled = false;
            AssetLabel = newassetlabel;
        }

        ICommand _cancel;
        public ICommand Cancel
        {
            get
            {
                if (_cancel == null)
                    _cancel = new DelegateCommand(CanExecute, ExecuteCancel);
                return _cancel;
            }
        }

        private void ExecuteCancel(object parameter)
        {
            if (!_canexecuteadd)
            {
                _canexecuteadd = true;
                PowerSourcesListEnabled = true;
                AssetLabel = selectedassetlabel;
            }
            else
            {
                DialogResult = false;
                CloseWindow();
            }
        }

        bool _canexecutesave = true;
        private bool CanExecuteSave(object obj)
        {
            string error = (_powersource as IDataErrorInfo)["PowerSource"];
            if (!string.IsNullOrEmpty(error))
                return false;

            return _canexecutesave;
        }

        ICommand _saveandclose;
        public ICommand SaveAndClose
        {
            get
            {
                if (_saveandclose == null)
                    _saveandclose = new DelegateCommand(CanExecuteSave, ExecuteSaveAndClose);
                return _saveandclose;
            }
        }

        private void ExecuteSaveAndClose(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
 //           _canexecuteadd = true;
            DialogResult = true;

            if (!_isediting)
            {
                Models.PowerSourceModel _newpowersource = new Models.PowerSourceModel();
                _newpowersource.PowerSource = PowerSource.PowerSource ?? string.Empty; 
      //          DataLayer.DatabaseQueries.AddPowerSource(_newpowersource);
            }
            else   
                foreach(Models.PowerSourceModel pm in PowerSources)         
                    DataLayer.DatabaseQueries.UpdatePowerSource(pm);
            
            StaticClasses.GlobalClass.LoadPowerSources();          
            CloseWindow();
        }

        ICommand _delete;
        public ICommand Delete
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
            if (SelectedItemIndex == -1)
                return false;
            return _canexecutedelete;
        }

        private void ExecuteDelete(object parameter)
        {
            int ctr = DataLayer.DatabaseQueries.CountUsedID("PowerSourceID", PowerSource.ID);
            IMessageBoxService _msgbox = new MessageBoxService();

            if (ctr > 0)
            {
                _msgbox.ShowMessage("This Country is assigned to an Asset and cannot be deleted", "Cannot Delete", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Asterisk);
                _canexecutedelete = false;
            }
            else
            {
                DataLayer.DatabaseQueries.DeleteItem(PowerSource.ID, DeleteSPName.DeleteAssetPowerSource);
            }
            _msgbox = null;
        }


        #endregion



    }
}
