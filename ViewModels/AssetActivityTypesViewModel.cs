using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class AssetActivityTypesViewModel : ViewModelBase
    {
        FullyObservableCollection<Models.AssetActivityTypeModel> _assetactivitytypes = new FullyObservableCollection<Models.AssetActivityTypeModel>();
        Models.AssetActivityTypeModel _assetactivitytype;
        bool _isediting;
        const string _selectedactivitylabel = "Selected Activity Type:";
        const string _newactivitylabel = "New Activity Type:";

        public AssetActivityTypesViewModel()
        {
            AssetActivityTypes = DataLayer.DatabaseQueries.GetAssetActivityTypes();

            if (AssetActivityTypes.Count > 0)
                AssetLabel = _selectedactivitylabel;
            else
                AssetLabel = _newactivitylabel;

            //populate from database
            AssetLabel = _selectedactivitylabel;
            _assetactivitytype = new Models.AssetActivityTypeModel();
            _isediting = true;
            _scrolltolastitem = false;
            ScrollToSelectedItem = 0;
        }

        public FullyObservableCollection<Models.AssetActivityTypeModel> AssetActivityTypes
        {
            get { return _assetactivitytypes; }
            set { SetField(ref _assetactivitytypes, value); }
        }

        public Models.AssetActivityTypeModel AssetActivityType
        {
            get { return _assetactivitytype; }
            set
            {
                if (value != null)
                    SetField(ref _assetactivitytype, value);
            }
        }

        string _assetlabel;
        public string AssetLabel
        {
            get { return _assetlabel; }
            set { SetField(ref _assetlabel, value); }
        }

        public bool AssetActivityTypesListEnabled
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
            string error = (_assetactivitytype as IDataErrorInfo)["Description"];
            if (!string.IsNullOrEmpty(error))
                return false;

            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            _canexecuteadd = false;
            AssetActivityTypes.Add(new Models.AssetActivityTypeModel());
            ScrollToLastItem = true;
            AssetActivityTypesListEnabled = false;
            AssetLabel = _newactivitylabel;
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
                AssetActivityTypesListEnabled = true;
                AssetLabel = _selectedactivitylabel;
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
            string error = (_assetactivitytype as IDataErrorInfo)["Description"];
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
            _canexecuteadd = true;
            DialogResult = true;
            if (!_isediting)
            {
                Models.AssetActivityTypeModel _newassetactivitytype = new Models.AssetActivityTypeModel();
                _newassetactivitytype.Description = AssetActivityType.Description ?? string.Empty; 

                DataLayer.DatabaseQueries.AddAssetActivityType(_newassetactivitytype);
            }
            else
                foreach(Models.AssetActivityTypeModel am in AssetActivityTypes)
                    DataLayer.DatabaseQueries.UpdateAssetActivityType(am);
            StaticClasses.GlobalClass.LoadAssetActivityTypes();
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
            //if (SelectedItemIndex == -1)
            //    return false;
            return _canexecutedelete;
        }

        private void ExecuteDelete(object parameter)
        {
            int _id = ((Models.AssetActivityTypeModel)parameter).ID;
            if (_id != 0)
            {
                int ctr = DataLayer.DatabaseQueries.CountUsedID("AssetActivityTypeID", _id);
                IMessageBoxService _msgbox = new MessageBoxService();

                if (ctr > 0)
                {
                    _msgbox.ShowMessage("This Asset Activity Type is assigned to an Asset and cannot be deleted", "Cannot Delete", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Asterisk);
                    _canexecutedelete = false;
                }
                else
                    if (_msgbox.ShowMessage("Do you want to delete this Asset Activity Type", "Delete Asset Activity Type", GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question) == GenericMessageBoxResult.OK)
                {
                    AssetActivityTypes.Remove((Models.AssetActivityTypeModel)parameter);
                    DataLayer.DatabaseQueries.DeleteItem(_id, DeleteSPName.DeleteAssetActivityType);
                }
                _msgbox = null;
            }
            else
                AssetActivityTypes.Remove((Models.AssetActivityTypeModel)parameter);
        }


        #endregion

    }
}
