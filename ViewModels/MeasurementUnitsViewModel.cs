using System.Linq;
using System.ComponentModel;
using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class MeasurementUnitsViewModel :  ViewModelBase
    {
        FullyObservableCollection<Models.MeasurementUnitModel> _measurementunits = new FullyObservableCollection<Models.MeasurementUnitModel>();
        Models.MeasurementUnitModel _measurementunit;
        bool _isediting;
        const string selectedassetlabel = "Selected Measurement Unit:";
        const string newassetlabel = "New Measurement Unit:";
        const string _closelabel = "Close";
        const string _cancellabel = "Cancel";

        public MeasurementUnitsViewModel()
        {
            MeasurementUnits = DatabaseQueries.GetMeasurementUnits();
            MeasurementUnits.ItemPropertyChanged += MeasurementUnits_ItemPropertyChanged;
            if (MeasurementUnits.Count > 0)
                AssetLabel = selectedassetlabel;
            else
                AssetLabel = newassetlabel;

            //populate from database
            AssetLabel = selectedassetlabel;
            _measurementunit = new Models.MeasurementUnitModel();
            _isediting = true;
            _scrolltolastitem = false;
            ScrollToSelectedItem = 0;
            CloseBtnLabel = _closelabel;
        }

        private void MeasurementUnits_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MeasurementUnit")
            {
                DuplicateName = (IsDuplicateName(MeasurementUnits[e.CollectionIndex].MeasurementUnit));
            }

        }

        public FullyObservableCollection<Models.MeasurementUnitModel> MeasurementUnits
        {
            get { return _measurementunits; }
            set { SetField(ref _measurementunits, value); }
        }

        public Models.MeasurementUnitModel MeasurementUnit
        {
            get { return _measurementunit; }
            set
            {
                if (value != null)
                    SetField(ref _measurementunit, value);
            }
        }

        string _assetlabel;
        public string AssetLabel
        {
            get { return _assetlabel; }
            set { SetField(ref _assetlabel, value); }
        }

        public bool MeasurementUnitsListEnabled
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
            string error = (_measurementunit as IDataErrorInfo)["MeasurementUnit"];
            if (!string.IsNullOrEmpty(error))
                return false;

            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            _canexecuteadd = false;
            MeasurementUnits.Add(new Models.MeasurementUnitModel());
            SelectedItemIndex = MeasurementUnits.Count - 1;
            ScrollToLastItem = true;
            MeasurementUnitsListEnabled = false;
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
                MeasurementUnitsListEnabled = true;
                AssetLabel = selectedassetlabel;
                CloseBtnLabel = _closelabel;
                MeasurementUnits.Remove(MeasurementUnit);
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
            string error = (_measurementunit as IDataErrorInfo)["MeasurementUnit"];
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
            
            if (!MeasurementUnitsListEnabled)
            {
                Models.MeasurementUnitModel _newmeasurementunit = new Models.MeasurementUnitModel();
                _newmeasurementunit.MeasurementUnit = MeasurementUnit.MeasurementUnit ?? string.Empty;
                DatabaseQueries.AddMeasurementUnit(_newmeasurementunit);
                MeasurementUnits = DatabaseQueries.GetMeasurementUnits();
                MeasurementUnits.ItemPropertyChanged += MeasurementUnits_ItemPropertyChanged;
            }
            else
                foreach (Models.MeasurementUnitModel am in MeasurementUnits)
                    DatabaseQueries.UpdateMeasurementUnit(am);

            _canexecuteadd = true;
            // _isediting = true;
            MeasurementUnitsListEnabled = true;
            AssetLabel = selectedassetlabel;
            CloseBtnLabel = _closelabel;
        }

        private bool IsDuplicateName(string _name)
        {
            var q = MeasurementUnits.Count(x => x.MeasurementUnit == _name);
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

           
                int _id = ((Models.MeasurementUnitModel)parameter).ID;

                if (_id != 0)
                {
                    int ctr = DatabaseQueries.CountUsedID(CountSPName.CountUsedMeasurementUnitID, _id);
                    IMessageBoxService _msgbox = new MessageBoxService();

                    if (ctr > 0)
                    {
                        _msgbox.ShowMessage(((Models.MeasurementUnitModel)parameter).MeasurementUnit + " is assigned to an Asset and cannot be deleted", "Cannot Delete", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Asterisk);
              //          _canexecutedelete = false;
                    }
                    else
                        if (_msgbox.ShowMessage("Do you want to delete " + ((Models.MeasurementUnitModel)parameter).MeasurementUnit + "?", "Delete Measurement Unit", GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question) == GenericMessageBoxResult.OK)
                    {
                        MeasurementUnits.Remove((Models.MeasurementUnitModel)parameter);
                        DatabaseQueries.DeleteItem(_id, DeleteSPName.DeleteMeasurementUnit);
                    }

                    _msgbox = null;
                }
                else
                    MeasurementUnits.Remove((Models.MeasurementUnitModel)parameter);
            


        }




        #endregion






    }
}
