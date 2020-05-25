using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class AssetSpecificationsViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<AssetSpecificationModel> _assetspecifications = new FullyObservableCollection<AssetSpecificationModel>();
      
        Collection<EnumValue> _measurementunits;
        bool isdirty = false;

        public AssetSpecificationsViewModel()
        {
            MeasurementUnits = EnumerationManager.GetEnumList(typeof(MeasurementUnits));

            AssetSpecifications = GetSpecifications();
            AssetSpecifications.ItemPropertyChanged += AssetSpecifications_ItemPropertyChanged;

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (AssetSpecifications.Count > 0)
                ScrollToSelectedItem = 0;

        }

        private void AssetSpecifications_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
                IsError = (IsDuplicateName(AssetSpecifications[e.CollectionIndex].Name));
            if (e.PropertyName != "Selected")
            {
                AssetSpecifications[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<AssetSpecificationModel> AssetSpecifications
        {
            get { return _assetspecifications; }
            set { SetField(ref _assetspecifications, value); }
        }
       
        public Collection<EnumValue> MeasurementUnits
        {
            get { return _measurementunits; }
            set {  _measurementunits= value; }
        }

        private bool IsDuplicateName(string name)
        {
            var q = AssetSpecifications.Count(x => x.Name == name);
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

        private bool CanExecuteAddNew(object obj)
        {
            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            AssetSpecifications.Add(new AssetSpecificationModel() {ID=0, Name=string.Empty, MeasurementUnitID=0 });
            ScrollToSelectedItem = AssetSpecifications.Count - 1;
        }
        
        private void ExecuteCancel(object parameter)
        {
            DialogResult = false;
        }
                
        private bool CanExecuteSave(object obj)
        {
            if (!isdirty)
                return false;
            return _canexecutesave;
        }
        
        private void ExecuteSave(object parameter)
        {
            if (isdirty)
            {
                foreach (AssetSpecificationModel assspec in AssetSpecifications)
                {
                    if (assspec.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(assspec.Name) && assspec.MeasurementUnitID > 0)
                            if (assspec.ID == 0)
                                assspec.ID = AddSpecification(assspec);
                            else
                                UpdateSpecification(assspec);

                        assspec.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }
               
        private bool CanExecuteDelete(object obj)
        {
            return AssetSpecifications.Count(x=> x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<AssetSpecificationModel> deleteditems = new Collection<AssetSpecificationModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Asset Specification";
            string confirmtxt = "Do you want to delete the selected item";
            if (AssetSpecifications.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (AssetSpecificationModel si in AssetSpecifications)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "Specifications");
                        deleteditems.Add(si);
                    }
                }
                foreach (AssetSpecificationModel pm in deleteditems)
                {
                    AssetSpecifications.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }
        #endregion

    }
}
