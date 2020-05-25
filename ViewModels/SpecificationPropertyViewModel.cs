using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
   public  class SpecificationPropertyViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<SpecificationPropertyModel> _specproperties = new FullyObservableCollection<SpecificationPropertyModel>();
        bool isdirty = false;

        public SpecificationPropertyViewModel()
        {
            SpecificationProperties = GetSpecificationProperties();
            SpecificationProperties.ItemPropertyChanged += SpecificationProperties_ItemPropertyChanged;
           
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (SpecificationProperties.Count > 0)
                ScrollToSelectedItem = 0;
            
        }

        private void SpecificationProperties_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                IsError = (IsDuplicateName(SpecificationProperties[e.CollectionIndex].PropertyUnit));
            }
            if (e.PropertyName != "Selected")
            {
                SpecificationProperties[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<SpecificationPropertyModel> SpecificationProperties
        {
            get { return _specproperties; }
            set { SetField(ref _specproperties, value); }
        }

        private bool IsDuplicateName(string _name)
        {
            return SpecificationProperties.Count(x => x.PropertyUnit == _name)>1;             
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
            SpecificationProperties.Add(new SpecificationPropertyModel() {ID=0, PropertyUnit=string.Empty });
            ScrollToSelectedItem = SpecificationProperties.Count - 1;
        }
        
        private void ExecuteCancel(object parameter)
        {
            DialogResult = false;
        }
       
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
                foreach (SpecificationPropertyModel spec in SpecificationProperties)
                {
                    if (spec.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(spec.PropertyUnit))
                            if (spec.ID == 0)
                                spec.ID = AddSpecificationProperty(spec);
                            else
                                UpdateSpecificationProperty(spec);
                        spec.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }

       

        private bool CanExecuteDelete(object obj)
        {
            return SpecificationProperties.Count(x=>x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<SpecificationPropertyModel> deleteditems = new Collection<SpecificationPropertyModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Specification Property";
            string confirmtxt = "Do you want to delete the selected item";
            if (SpecificationProperties.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (SpecificationPropertyModel si in SpecificationProperties)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "SpecificationPropertyUnits");
                        deleteditems.Add(si);
                    }
                }
                foreach (SpecificationPropertyModel pm in deleteditems)
                {
                    SpecificationProperties.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }

        #endregion


    }
}
