using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class AssetAreasViewModel: ObjectCRUDViewModel
    {
        FullyObservableCollection<AssetAreaModel> _assetareas = new FullyObservableCollection<AssetAreaModel>();
        bool isdirty = false;

        public AssetAreasViewModel()
        {
            AssetAreas = GetAssetAreas();
            AssetAreas.ItemPropertyChanged += AssetAreas_ItemPropertyChanged;
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (AssetAreas.Count > 0)
                ScrollToSelectedItem = 0;
            
        }

        private void AssetAreas_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                IsError = (IsDuplicateName(AssetAreas[e.CollectionIndex].Name));
                if(IsError)
                    ErrorMsg = "Duplicate Name";
            }
            else
            if (e.PropertyName == "Prefix")
            {
                IsError = (IsDuplicateCode(AssetAreas[e.CollectionIndex].Prefix));
                if (IsError)
                    ErrorMsg = "Duplicate Prefix";
            }
            if (e.PropertyName != "Selected")
            {
                AssetAreas[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<AssetAreaModel> AssetAreas
        {
            get { return _assetareas; }
            set { SetField(ref _assetareas, value); }
        }

        private bool _isduplicatecode;
        public bool DuplicateCode
        {
            get { return _isduplicatecode; }
            set { SetField(ref _isduplicatecode, value); }
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

        private bool IsDuplicateName(string _name)
        {
            var q = AssetAreas.Count(x => x.Name == _name);
            return (q > 1);
        }

        private bool IsDuplicateCode(string _name)
        {
            var q = AssetAreas.Count(x => x.Prefix == _name);
            return (q > 1);
        }        
        
        #region Commands

        //Add New            
        private bool CanExecuteAddNew(object obj)
        {
            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            AssetAreas.Add(new AssetAreaModel() {ID=0, Name = string.Empty, Prefix=string.Empty });
            ScrollToSelectedItem = AssetAreas.Count - 1;
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
                foreach (AssetAreaModel assarea in AssetAreas)
                {
                    if (assarea.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(assarea.Prefix) && !string.IsNullOrEmpty(assarea.Name))
                            if (assarea.ID == 0)
                                assarea.ID = AddAssetArea(assarea);
                            else
                                UpdateAssetArea(assarea);
                        assarea.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }

        //Delete               
        private bool CanExecuteDelete(object obj)
        {
            return AssetAreas.Count(x=>x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<AssetAreaModel> deleteditems = new Collection<AssetAreaModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Asset Area";
            string confirmtxt = "Do you want to delete the selected item";
            if (AssetAreas.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (AssetAreaModel si in AssetAreas)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "AssetAreas");
                        deleteditems.Add(si);
                    }
                }
                foreach (AssetAreaModel pm in deleteditems)
                {
                    AssetAreas.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }
        #endregion

    }
}
