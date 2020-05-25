using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public  class GroupSpecificationsViewModel : ViewModelBase
    {
        FullyObservableCollection<AssetGroupModel> _assetgroups;
        FullyObservableCollection<AssetGroupSpecificationModel> _filteredassetgroupspecifications = new FullyObservableCollection<AssetGroupSpecificationModel>();
        FullyObservableCollection<AssetSpecificationModel> _assetspecifications;
        Collection<SpecificationPropertyModel> _propertyunits;

        AssetGroupSpecificationModel _assetgroupspecification;
        public GroupSpecificationsViewModel()
        {
            AssetGroups = GetAssetGroups();
            _assetspecifications = GetSpecifications();
            _propertyunits = GetSpecificationProperties();
            
        }

        public FullyObservableCollection<AssetGroupSpecificationModel> AssetGroupSpecifications
        {
            get { return _filteredassetgroupspecifications; }
            set { SetField(ref _filteredassetgroupspecifications, value); }
        }

        public AssetGroupSpecificationModel AssetGroupSpecification
        {
            get { return _assetgroupspecification; }
            set { SetField(ref _assetgroupspecification, value); }
        }

        public FullyObservableCollection<AssetGroupModel> AssetGroups
        {
            get { return _assetgroups; }
            set { SetField(ref _assetgroups, value); }
        }

        //public FullyObservableCollection<Models.AssetSpecificationModel> AssetSpecifications
        //{
        //    get { return _assetspecifications; }
        //    internal set { }
        //}

        public FullyObservableCollection<AssetSpecificationModel> AssetSpecifications => _assetspecifications;
      

        //public Collection<Models.SpecificationPropertyModel> PropertyUnits
        //{
        //    get { return _propertyunits; }
        //    internal set { }
        //}

        public Collection<SpecificationPropertyModel> PropertyUnits => _propertyunits;

        bool iserror = false;
        public bool IsError
        {
            get { return iserror; }
            set { SetField(ref iserror, value); }
        }

        string errormsg = "Duplicate Specification";
        public string ErrorMsg
        {
            get { return errormsg; }
            set { SetField(ref errormsg, value); }
        }


        int _selectedgroupid;
        public int SelectedGroupID
        {
            get { return _selectedgroupid; }
            set
            {
                SaveDirtySpecificationRecords();
                filterAssetGroupSpecifications(value);
                SetField(ref _selectedgroupid, value);               
            }
        }

        private void filterAssetGroupSpecifications(int _groupid)
        {
            AssetGroupSpecifications = GetAssetGroupSpecifications(_groupid);
            AssetGroupSpecifications.ItemPropertyChanged += _specification_ItemPropertyChanged;
        }

        private void _specification_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {           
            if (e.PropertyName == "SpecificationNameID")
            {
                IsError = IsDuplicateSpec(_filteredassetgroupspecifications[e.CollectionIndex].SpecificationNameID);

                var q = from v in AssetSpecifications
                        where v.ID == AssetGroupSpecifications[e.CollectionIndex].SpecificationNameID
                        select v.MeasurementUnitID;
                AssetGroupSpecifications[e.CollectionIndex].MeasurementUnitID = q.FirstOrDefault();
            }
            AssetGroupSpecifications[e.CollectionIndex].IsDirty = true;
        }

        private bool IsDuplicateSpec(int _id)
        {
            var query = _filteredassetgroupspecifications.GroupBy(x => x.SpecificationNameID)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            return (query.Count > 0);
        }

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            return SelectedGroupID > 0;
        }


        ICommand _addspecificationcommand;
        public ICommand AddSpecificationCommand
        {
            get
            {
                if (_addspecificationcommand == null)
                    _addspecificationcommand = new DelegateCommand(CanExecuteAddNew, ExecuteAddSpecification);
                return _addspecificationcommand;
            }
        }
        private void ExecuteAddSpecification(object parameter)
        {
            AssetGroupSpecifications.Add(new AssetGroupSpecificationModel()
                {ID=0, AssetGroupID = SelectedGroupID, AssetID=0, MeasurementUnit=string.Empty,PropertyUnitID=-1,
                        MeasurementUnitID =0,SpecificationNameID=-1, SpecificationOptions=string.Empty});              
        }

        ICommand _deletespecificationcommand;
        public ICommand DeleteSpecificationCommand
        {
            get
            {
                if (_deletespecificationcommand == null)
                    _deletespecificationcommand = new DelegateCommand(CanExecute, ExecuteDeleteSpecification);
                return _deletespecificationcommand;
            }
        }
        private void ExecuteDeleteSpecification(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            if (parameter != null)
            {
                if (_msgboxcommand.ShowMessage("Are you sure you want to delete this specification?", "Deleting Specification", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.Yes))
                {
                    //save changes to db
                    SaveDirtySpecificationRecords();
                    //Delete from DB
                    DeleteItem((int)parameter, "AssetGroupSpecifications");
                    filterAssetGroupSpecifications(SelectedGroupID);
                }
            }
            _msgboxcommand = null;
        }

        bool _canexecutesave = true;
        private bool CanExecuteSave(object obj)
        {
            if (IsError)
                return false;
            return _canexecutesave;
        }

        ICommand save;
        public ICommand Save
        {
            get
            {
                if (save == null)
                    save = new DelegateCommand(CanExecuteSave, ExecuteSave);
                return save;
            }
        }

        private void ExecuteSave(object parameter)
        {
            SaveDirtySpecificationRecords();
            
        }

        ICommand _close;
        public ICommand Cancel
        {
            get
            {
                if (_close == null)
                    _close = new DelegateCommand(CanClose, ExecuteClose);
                return _close;
            }
        }

        bool _canclose = true;
        private bool CanClose(object obj)
        {

            return _canclose;
        }

        private void ExecuteClose(object parameter)
        {
            DialogResult = false;
        }

        #endregion


        private void SaveDirtySpecificationRecords()
        {
            foreach (AssetGroupSpecificationModel agsm in AssetGroupSpecifications)
            {
                if (agsm.IsDirty)
                {
                    if (agsm.ID == 0)
                        agsm.ID = AddAssetGroupSpecification(agsm);
                    else
                        UpdateAssetGroupSpecification(agsm);
                    agsm.IsDirty = false;
                }
            }
        }
       
    }
}
