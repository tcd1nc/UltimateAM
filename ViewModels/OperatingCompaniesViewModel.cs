using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
   public  class OperatingCompaniesViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<OperatingCompanyModel> _opcos = new FullyObservableCollection<OperatingCompanyModel>();
        bool isdirty = false;

        public OperatingCompaniesViewModel()
        {
            OperatingCompanies = GetOperatingCompanies();
            OperatingCompanies.ItemPropertyChanged += OperatingCompanies_ItemPropertyChanged;
                    
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (OperatingCompanies.Count > 0)
                ScrollToSelectedItem = 0;
            
        }

        private void OperatingCompanies_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")            
                IsError = (IsDuplicateName(OperatingCompanies[e.CollectionIndex].Name));
            if (e.PropertyName != "Selected")
            {
                OperatingCompanies[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<OperatingCompanyModel> OperatingCompanies
        {
            get { return _opcos; }
            set { SetField(ref _opcos, value); }
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
           return OperatingCompanies.Count(x => x.Name == _name) > 1;             
        }
       
        #region Commands
       
        private bool CanExecuteAddNew(object obj)
        {          
            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            OperatingCompanies.Add(new OperatingCompanyModel() {ID = 0, Name=string.Empty });
            ScrollToSelectedItem = OperatingCompanies.Count - 1;
        }

        
        private void ExecuteCancel(object parameter)
        {
            DialogResult = false;
        }

        
        private bool CanExecuteSave(object obj)
        {
            if (!isdirty)
                return false;
            if (IsError)
                return false;
            return _canexecutesave;
        }
        
        private void ExecuteSave(object parameter)
        {
            if (isdirty)
            {
                foreach (OperatingCompanyModel opco in OperatingCompanies)
                {
                    if (opco.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(opco.Name))
                            if (opco.ID == 0)
                                opco.ID = AddOperatingCompany(opco);
                            else
                                UpdateOperatingCompany(opco);
                        opco.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }
      
        private bool CanExecuteDelete(object obj)
        {
            return OperatingCompanies.Count(x=>x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<OperatingCompanyModel> deleteditems = new Collection<OperatingCompanyModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Operating Company";
            string confirmtxt = "Do you want to delete the selected item";
            if (OperatingCompanies.Count(x => x.Selected) > 1)
            {
                title = "Deleting Operating Companies";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (OperatingCompanyModel si in OperatingCompanies)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "OperatingCompany");
                        deleteditems.Add(si);
                    }
                }
                foreach (OperatingCompanyModel pm in deleteditems)
                {
                    OperatingCompanies.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }

        #endregion


    }
}
