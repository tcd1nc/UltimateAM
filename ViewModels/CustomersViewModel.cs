using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class CustomersViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<CustomerModel> _customers = new FullyObservableCollection<CustomerModel>();
        FullyObservableCollection<CorporationModel> _corporations = new FullyObservableCollection<CorporationModel>();
        FullyObservableCollection<CountryModel> _countries = new FullyObservableCollection<CountryModel>();
        bool isdirty = false;

        public CustomersViewModel()
        {
            Customers = GetCustomers();
            Corporations = GetCorporations();
            Countries = GetCountries();

            Customers.ItemPropertyChanged += Customers_ItemPropertyChanged;
                   
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (Customers.Count > 0)
                ScrollToSelectedItem = 0;           
        }

        private void Customers_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
                IsError = IsDuplicateName(Customers[e.CollectionIndex].Name);
            if (e.PropertyName != "Selected")
            {
                Customers[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<CustomerModel> Customers
        {
            get { return _customers; }
            set { SetField(ref _customers, value); }
        }

        public FullyObservableCollection<CorporationModel> Corporations
        {
            get { return _corporations; }
            set { SetField(ref _corporations, value); }
        }

        public FullyObservableCollection<CountryModel> Countries
        {
            get { return _countries; }
            set { SetField(ref _countries, value); }
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
            Customers.Add(new CustomerModel() { ID = 0, Name = string.Empty, CustomerNumber = string.Empty, CountryID = 0, CorporationID = 0, Location = string.Empty});
            ScrollToSelectedItem = Customers.Count-1;
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
                foreach (CustomerModel cust in Customers)
                {
                    if (cust.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(cust.Name))
                            if (cust.ID == 0)
                                cust.ID = AddCustomer(cust);
                            else
                                UpdateCustomer(cust);
                        cust.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }

        private bool IsDuplicateName(string _name)
        {
            bool _isduplicate = false;

            var query = _customers.GroupBy(x => x.Name.ToUpper())
                         .Where(g => g.Count() > 1)
                         .Select(y => y.Key)
                         .ToList();
            if (query.Count > 0)
                return true;

            return _isduplicate;
        }
               
        private bool CanExecuteDelete(object obj)
        {
            return Customers.Count(x=>x.Selected) > 0;
        }
                
        private void ExecuteDelete(object parameter)
        {
            Collection<CustomerModel> deleteditems = new Collection<CustomerModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Customer";
            string confirmtxt = "Do you want to delete the selected item";
            if (Customers.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (CustomerModel si in Customers)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "Customers");
                        deleteditems.Add(si);
                    }
                }
                foreach (CustomerModel pm in deleteditems)
                {
                    Customers.Remove(pm);
                }
                deleteditems.Clear();               
            }
            msg = null;
        }
              
        #endregion


    }
}
