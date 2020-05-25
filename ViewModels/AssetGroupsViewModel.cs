using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class AssetGroupsViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<AssetGroupModel> _assetgroups = new FullyObservableCollection<AssetGroupModel>();
        FullyObservableCollection<AssetAreaModel> _assetareas = new FullyObservableCollection<AssetAreaModel>();
        FullyObservableCollection<AssetGroupModel> _filteredassetgroups;
        bool isdirty = false;

        public AssetGroupsViewModel()
        {
            _assetgroups = GetAssetGroups();
            AssetAreas = GetAssetAreas();
           


            AssetGroups = FilterAssetGroups(0);
            AssetGroups.ItemPropertyChanged += AssetGroups_ItemPropertyChanged;

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            
            if (AssetAreas.Count > 0)
                AssetAreaID = AssetAreas[0].ID;
            if(AssetGroups.Count > 0 )
                ScrollToSelectedItem = 0;
            
        }

        private void AssetGroups_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                IsError = (IsDuplicateName(_filteredassetgroups[e.CollectionIndex].Name));
                if(IsError)
                    ErrorMsg = "Duplicate Name";
            }
            else
           if (e.PropertyName == "AssetGroupIDText")
            {
                IsError = (IsDuplicateCode(_filteredassetgroups[e.CollectionIndex].AssetGroupIDText));
                if (IsError)
                    ErrorMsg = "Duplicate Asset Group";
            }
            if (e.PropertyName != "Selected")
            {
                _filteredassetgroups[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }


        public FullyObservableCollection<AssetGroupModel> AssetGroups
        {
            get { return _filteredassetgroups; }
            set { SetField(ref _filteredassetgroups, value); }
        }
     
        public FullyObservableCollection<Models.AssetAreaModel> AssetAreas
        {
            get { return _assetareas; }
            set { SetField(ref _assetareas, value); }
        }

        int _assetareaid;
        public int AssetAreaID
        {
            get { return _assetareaid; }
            set {
                if (_assetgroups != null)
                {
                    AssetGroups = FilterAssetGroups(value);
                    AssetGroups.ItemPropertyChanged += AssetGroups_ItemPropertyChanged;
                }
                SetField(ref _assetareaid, value);
            }
        }

        private FullyObservableCollection<AssetGroupModel> FilterAssetGroups(int _assetareaid)
        {
            FullyObservableCollection<AssetGroupModel> _temp = new FullyObservableCollection<AssetGroupModel>();
            foreach (AssetGroupModel agm in _assetgroups)
                if (agm.AssetAreaID == _assetareaid)
                    _temp.Add(agm);
            return _temp;
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
       
        private bool _isduplicatecode;
        public bool DuplicateCode
        {
            get { return _isduplicatecode; }
            set { SetField(ref _isduplicatecode, value); }
        }

        private bool IsDuplicateName(string _name)
        {
            var q = AssetGroups.Count(x => x.Name == _name);
            return (q > 1);
        }

        private bool IsDuplicateCode(string _name)
        {
            var q = _filteredassetgroups.Count(x => x.AssetGroupIDText == _name);
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
            AssetGroups.Add(new AssetGroupModel() {ID=0, AssetGroupIDText=string.Empty, AssetAreaID=0, Name=string.Empty });        
            ScrollToSelectedItem = AssetGroups.Count - 1;
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
            if (_isduplicatecode)
                return false;
            
            return _canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {
            if (isdirty)
            {
                foreach (AssetGroupModel assgrp in AssetGroups)
                {
                    if (assgrp.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(assgrp.Name) && !string.IsNullOrEmpty(assgrp.AssetGroupIDText))
                            if (assgrp.ID == 0)
                                assgrp.ID = AddAssetGroup(assgrp);
                            else
                                UpdateAssetGroup(assgrp);
                        assgrp.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }
        
        //Delete               
        private bool CanExecuteDelete(object obj)
        {
            return AssetGroups.Count(x=>x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<AssetGroupModel> deleteditems = new Collection<AssetGroupModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Asset Group";
            string confirmtxt = "Do you want to delete the selected item";
            if (AssetGroups.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (AssetGroupModel si in AssetGroups)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "AssetGroups");
                        deleteditems.Add(si);
                    }
                }
                foreach (Models.AssetGroupModel pm in deleteditems)
                {
                    AssetGroups.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }


        #endregion

    }
   
}
