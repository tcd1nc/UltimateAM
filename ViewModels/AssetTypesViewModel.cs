using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class AssetTypesViewModel: ObjectCRUDViewModel
    {
        FullyObservableCollection<AssetAreaModel> _assetareas = new FullyObservableCollection<AssetAreaModel>();
        FullyObservableCollection<AssetGroupModel> _assetgroups = new FullyObservableCollection<AssetGroupModel>();
        FullyObservableCollection<AssetTypeModel> _assettypes = new FullyObservableCollection<AssetTypeModel>();
        FullyObservableCollection<AssetGroupModel> _filteredassetgroups;
        FullyObservableCollection<AssetTypeModel> _filteredassettypes;
        bool isdirty = false;

        public AssetTypesViewModel()
        {
            _assettypes = GetAssetTypes();
            _assetgroups = GetAssetGroups();
            AssetAreas = GetAssetAreas();
           
            if (AssetAreas.Count > 0)
            {
                AssetAreaID = AssetAreas[0].ID;
                if (AssetAreaID > 0 && AssetGroups.Count>0)
                    AssetGroupID = AssetGroups[0].ID;
            }

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if(AssetTypes.Count > 0)
                ScrollToSelectedItem = 0;
            
        }

        public FullyObservableCollection<AssetAreaModel> AssetAreas
        {
            get { return _assetareas; }
            set { SetField(ref _assetareas, value); }
        }

        public FullyObservableCollection<AssetGroupModel> AssetGroups
        {
            get { return _filteredassetgroups; }
            set { SetField(ref _filteredassetgroups, value); }
        }


        public FullyObservableCollection<AssetTypeModel> AssetTypes
        {
            get { return _filteredassettypes; }
            set { SetField(ref _filteredassettypes, value); }
        }
                             
        int _assetareaid;
        public int AssetAreaID
        {
            get { return _assetareaid; }
            set
            {                
                if (_assetgroups != null)
                {
                    AssetGroups = FilterAssetGroups(value);
                    //C# 6
                    AssetTypes?.Clear();
                }
                SetField(ref _assetareaid, value);
            }
        }

        int _assetgroupid;
        public int AssetGroupID
        {
            get { return _assetgroupid; }
            set
            {
                if (_assettypes != null)
                {
                    AssetTypes = FilterAssetTypes(value);
                    AssetTypes.ItemPropertyChanged += AssetTypes_ItemPropertyChanged;
                }
                SetField(ref _assetgroupid, value);
            }
        }

        private void AssetTypes_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                IsError = (IsDuplicateName(_filteredassettypes[e.CollectionIndex].Name));
                if(IsError)
                    ErrorMsg = "Duplicate Name";
            }
            if (e.PropertyName != "Selected")
            {
                _filteredassettypes[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        int _assettypeid;
        public int AssetTypeID
        {
            get { return _assettypeid; }
            set { SetField(ref _assettypeid, value); }
        }

        private FullyObservableCollection<AssetGroupModel> FilterAssetGroups(int _assetprefixid)
        {
            FullyObservableCollection<Models.AssetGroupModel> _temp = new FullyObservableCollection<Models.AssetGroupModel>();
            foreach (Models.AssetGroupModel agm in _assetgroups)
                if (agm.AssetAreaID == _assetprefixid)
                    _temp.Add(agm);
            return _temp;
        }

        private FullyObservableCollection<AssetTypeModel> FilterAssetTypes(int _assetgroupid)
        {
            FullyObservableCollection<AssetTypeModel> _temp = new FullyObservableCollection<AssetTypeModel>();
            foreach (Models.AssetTypeModel atm in _assettypes)
                if (atm.AssetGroupID == _assetgroupid)
                    _temp.Add(atm);
            return _temp;
        }

        private bool IsDuplicateName(string _name)
        {
            var q = _assettypes.Count(x => x.Name == _name);
            return (q > 1);
        }

        bool iserror = false;
        public bool IsError
        {
            get { return iserror; }
            set { SetField(ref iserror, value); }
        }

        string errormsg = "Duplicate Name";
        public string ErrorMsg
        {
            get { return errormsg; }
            set { SetField(ref errormsg, value); }
        }

        #region Commands

        //Add New
        private bool CanExecuteAddNew(object obj)
        {
            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            AssetTypes.Add(new AssetTypeModel{ID=0, Name=string.Empty, AssetGroupID = this.AssetGroupID });
            ScrollToSelectedItem = AssetTypes.Count - 1;
        }

       //Cancel
        private void ExecuteCancel(object parameter)
        {
            DialogResult = false;
        }

        //Save
        private bool CanExecuteSave(object obj)
        {
            if (!isdirty)
                return false;
            if (DuplicateName)
                return false;
            return _canexecutesave;
        }
               
        private void ExecuteSave(object parameter)
        {
            if (isdirty)
            {
                foreach (AssetTypeModel asstype in AssetTypes)
                {
                    if (asstype.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(asstype.Name) && asstype.AssetGroupID > 0)
                            if (asstype.ID == 0)
                                asstype.ID = AddAssetType(asstype);
                            else
                                UpdateAssetType(asstype);
                        asstype.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }

        //Delete
        private bool CanExecuteDelete(object obj)
        {
            return AssetTypes.Count(x=>x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<AssetTypeModel> deleteditems = new Collection<AssetTypeModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Asset Type";
            string confirmtxt = "Do you want to delete the selected item";
            if (AssetTypes.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (AssetTypeModel si in AssetTypes)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "AssetTypes");
                        deleteditems.Add(si);
                    }
                }
                foreach (AssetTypeModel pm in deleteditems)
                {
                    AssetTypes.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }
        #endregion


    }
}
