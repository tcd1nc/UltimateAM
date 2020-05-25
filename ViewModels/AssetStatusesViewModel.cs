using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class AssetStatusesViewModel : ViewModelBase
    {

        FullyObservableCollection<Models.StatusModel> _assetstatuses = new FullyObservableCollection<Models.StatusModel>();
        Models.StatusModel _assetstatus;
        bool _isediting;
        const string selectedassetlabel = "Selected Status:";
        const string newassetlabel = "New Status:";
        const string _closelabel = "Close";
        const string _cancellabel = "Cancel";

        public AssetStatusesViewModel()
        {
            AssetStatuses = DatabaseQueries.GetStatuses();
            AssetStatuses.ItemPropertyChanged += AssetStatuses_ItemPropertyChanged;

            //populate from database
            AssetLabel = selectedassetlabel;
            if (AssetStatuses.Count > 0)
                AssetLabel = selectedassetlabel;
            else
                AssetLabel = newassetlabel;

            _assetstatus = new Models.StatusModel();
            _isediting = true;
            _scrolltolastitem = false;
            ScrollToSelectedItem = 0;
            CloseBtnLabel = _closelabel;
        }

        private void AssetStatuses_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
            {
                DuplicateName = (IsDuplicateName(AssetStatuses[e.CollectionIndex].Status));
            }
        }

        public FullyObservableCollection<Models.StatusModel> AssetStatuses
        {
            get { return _assetstatuses; }
            set { SetField(ref _assetstatuses, value); }
        }

        public Models.StatusModel AssetStatus
        {
            get { return _assetstatus; }
            set
            {
                if (value != null)
                    SetField(ref _assetstatus, value);
            }
        }

        string _assetlabel;
        public string AssetLabel
        {
            get { return _assetlabel; }
            set { SetField(ref _assetlabel, value); }
        }

        public bool AssetStatusesListEnabled
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

        string _closebtnlabel;
        public string CloseBtnLabel
        {
            get { return _closebtnlabel; }
            set { SetField(ref _closebtnlabel, value); }
        }

        private bool _isduplicatename;
        public bool DuplicateName
        {
            get { return _isduplicatename; }
            set { SetField(ref _isduplicatename, value); }
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
            string error = (_assetstatus as IDataErrorInfo)["Status"];
                if (!string.IsNullOrEmpty(error))
                return false;

            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            _canexecuteadd = false;
            AssetStatuses.Add(new Models.StatusModel());
            SelectedItemIndex = AssetStatuses.Count - 1;
            ScrollToLastItem = true;
            AssetStatusesListEnabled = false;
            AssetLabel = newassetlabel;
            CloseBtnLabel = _cancellabel;
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
                AssetStatusesListEnabled = true;
                AssetLabel = selectedassetlabel;
                CloseBtnLabel = _closelabel;
                AssetStatuses.Remove(AssetStatus);

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
            string error = (_assetstatus as IDataErrorInfo)["Status"];
            if (!string.IsNullOrEmpty(error))
                return false;
            if (_isduplicatename)
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
           
            if (!AssetStatusesListEnabled)
            {
                Models.StatusModel _newassetstatus = new Models.StatusModel();
                _newassetstatus.Status = AssetStatus.Status ?? string.Empty;
                DatabaseQueries.AddStatus(_newassetstatus);
                AssetStatuses = DatabaseQueries.GetStatuses();
                AssetStatuses.ItemPropertyChanged += AssetStatuses_ItemPropertyChanged;

            }
                else
                    foreach (Models.StatusModel sm in AssetStatuses)
                        DatabaseQueries.UpdateStatus(sm);
            _canexecuteadd = true;
            // _isediting = true;
            AssetStatusesListEnabled = true;
            AssetLabel = selectedassetlabel;
            CloseBtnLabel = _closelabel;

        }

        private bool IsDuplicateName(string _name)
        {
            var q = AssetStatuses.Count(x => x.Status == _name);
            return (q > 1);
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
            return _canexecutedelete;
        }

        private void ExecuteDelete(object parameter)
        {
            int _id = ((Models.StatusModel)parameter).ID;
            if (_id != 0)
            {
                int ctr = DatabaseQueries.CountUsedID(CountSPName.CountUsedStatusID, _id);
                IMessageBoxService _msgbox = new MessageBoxService();

                if (ctr > 0)
                {
                    _msgbox.ShowMessage(((Models.StatusModel)parameter).Status + " is assigned to an Asset and cannot be deleted", "Cannot Delete", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Asterisk);
            //        _canexecutedelete = false;
                }
                else
                    if (_msgbox.ShowMessage("Do you want to delete " + ((Models.StatusModel)parameter).Status + "?", "Delete Asset Status", GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question) == GenericMessageBoxResult.OK)
                {
                    AssetStatuses.Remove((Models.StatusModel)parameter);
                    DatabaseQueries.DeleteItem(_id, DeleteSPName.DeleteAssetStatus);
                }
                _msgbox = null;
            }
            else
                AssetStatuses.Remove((Models.StatusModel)parameter);
        }

        #endregion
    }
}
