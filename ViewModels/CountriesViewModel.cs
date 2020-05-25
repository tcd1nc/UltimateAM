using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{  
    public class CountriesViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<CountryModel> _countries;
        FullyObservableCollection<OperatingCompanyModel> _opcos;
        bool isdirty = false;

        public CountriesViewModel()
        {
            Countries = GetCountries();
            Countries.ItemPropertyChanged += Countries_ItemPropertyChanged;
            OperatingCompanies = GetOperatingCompanies();

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (Countries.Count > 0)
                ScrollToSelectedItem = 0;
           
        }
                
        private void Countries_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                IsError = (IsDuplicateName(Countries[e.CollectionIndex].Name));
                if(IsError)
                    ErrorMsg = "Duplicate Name";
            }
            else
            if (e.PropertyName == "OperatingCompanyID")
            {
                RequiredOpco = (Countries[e.CollectionIndex].OperatingCompanyID == 0);
                if(RequiredOpco)
                    ErrorMsg = "Required Opco";
            }
            if (e.PropertyName != "Selected")
            {
                Countries[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<CountryModel> Countries
        {
            get { return _countries; }
            set { SetField(ref _countries, value); }
        }

        public FullyObservableCollection<OperatingCompanyModel> OperatingCompanies
        {
            get { return _opcos; }
            set { SetField(ref _opcos, value); }
        }
                
        private bool _isrequiredfield;
        public bool RequiredOpco
        {
            get { return _isrequiredfield; }
            set { SetField(ref _isrequiredfield, value); }
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
            var q = Countries.Count(x => x.Name == _name);
            return (q > 1);
        }
        
        
        #region Commands
        
        private bool CanExecuteAddNew(object obj)
        {
            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            Countries.Add(new CountryModel() {ID=0, Name=string.Empty, OperatingCompanyID=0 });
            ScrollToSelectedItem = Countries.Count - 1;
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
                foreach (CountryModel ctry in Countries)
                {
                    if (ctry.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(ctry.Name) && ctry.OperatingCompanyID > 0)
                            if (ctry.ID == 0)
                                ctry.ID = AddCountry(ctry);
                            else
                                UpdateCountry(ctry);
                        ctry.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }
               
        private bool CanExecuteDelete(object obj)
        {
            return Countries.Count(x=>x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<CountryModel> deleteditems = new Collection<CountryModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Country";
            string confirmtxt = "Do you want to delete the selected item";
            if (Countries.Count(x => x.Selected) > 1)
            {
                title = "Deleting Countries";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (CountryModel si in Countries)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "Countries");
                        deleteditems.Add(si);
                    }
                }
                foreach (CountryModel pm in deleteditems)
                {
                    Countries.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }

        #endregion

    }
}
