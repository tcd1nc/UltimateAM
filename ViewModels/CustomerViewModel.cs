using System.IO;
using System.Linq;
using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class CustomerViewModel : ObjectCRUDViewModel
    {
        CustomerModel customer = new CustomerModel();
        FullyObservableCollection<CorporationModel> _corporations = new FullyObservableCollection<CorporationModel>();
        FullyObservableCollection<CountryModel> _countries = new FullyObservableCollection<CountryModel>();
        FullyObservableCollection<CustomerModel> _customers;

        bool isdirty = false;

        public CustomerViewModel(int id)
        {
            _customers = GetCustomers();
            Customer = GetCustomer(id);
            Corporations = GetCorporations();
            Countries = GetCountries();

            Customer.PropertyChanged += Customer_PropertyChanged1;
                   
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
           
        }

        private void Customer_PropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
                IsError = IsDuplicateName(Customer.Name);
            if(e.PropertyName != "IconFileName")
                isdirty = true;
        }               

        public CustomerModel Customer
        {
            get { return customer; }
            set { SetField(ref customer, value); }
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
        
        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
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
            SaveAll();                 
        }

        private void SaveAll()
        {
            if (isdirty)
            {                
                if (!string.IsNullOrEmpty(Customer.Name))
                {
                    UpdateCustomer(Customer);                    
                }
                 isdirty = false;
            }           
        }

        private bool IsDuplicateName(string _name)
        {
            return (_customers.Count(x => x.Name == _name && x.ID !=Customer.ID) > 0);            
        }

        bool canexecuteaddimage = true;
        private bool CanExecuteAddImage(object obj)
        {
            return canexecuteaddimage;
        }

        ICommand addimage;
        public ICommand AddImage
        {
            get
            {
                if (addimage == null)
                    addimage = new DelegateCommand(CanExecuteAddImage, ExecuteAddImage);
                return addimage;
            }
        }

        private void ExecuteAddImage(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string filename = msg.OpenFileDlg("Select the file containing the customer's logo", true, false, "Logo Files(*.ICO; *.PNG; *.JPG)| *.ICO; *.PNG; *.JPG", string.Empty, null);
            msg = null;

            if (!string.IsNullOrEmpty(filename) && (Customer.ID > 0))
            {
                filename = Path.GetFileName(filename);
                // ((CorporationModel)parameter).Logo = File.ReadAllBytes(filename);

                Customer.IconFileName = filename;
                    UpdateCustomerLogo(Customer.ID, filename);

                    //if (((CorporationModel)parameter).LogoID == 0)
                    //    AddCorporationLogo(((CorporationModel)parameter).ID, ((CorporationModel)parameter).Logo);
                    //else
                    //    UpdateCorporationLogo(((CorporationModel)parameter).LogoID, ((CorporationModel)parameter).Logo);
                
            }
        }

        bool canexecutedeleteimage = true;
        private bool CanExecuteDeleteImage(object obj)
        {
            return canexecutedeleteimage;
        }

        ICommand deleteimage;
        public ICommand DeleteImage
        {
            get
            {
                if (deleteimage == null)
                    deleteimage = new DelegateCommand(CanExecuteDeleteImage, ExecuteDeleteImage);
                return deleteimage;
            }
        }

        private void ExecuteDeleteImage(object parameter)
        {
            ((CustomerModel)parameter).Logo = new byte[0];

            //delete logo
            //if (((CustomerModel)parameter).ID > 0)
            UpdateCustomerLogo(((CustomerModel)parameter).ID, string.Empty);

            //((CorporationModel)parameter).LogoID = 0;
        }

        ICommand windowclosing;

        private void ExecuteClosing(object parameter)
        {
        }

        private bool CanCloseWindow(object obj)
        {
            if (isdirty)
            {
                if (!IsError)
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                                        
                    if (result.Equals(GenericMessageBoxResult.Yes))
                    {
                        SaveAll();
                        ObjDialogResult = this;
                        return true;
                    }
                    else
                    {
                        this.Customer.ID = -1;
                        ObjDialogResult = this;
                        return true;
                    }                        
                }
                else                    
                {
                    this.Customer.ID = -1;
                    ObjDialogResult = this;
                    return true;
                }
            }
            else
            {
                this.Customer.ID = -1;
                ObjDialogResult = this;
                return true;
            }
        }

        public ICommand WindowClosing
        {
            get
            {
                if (windowclosing == null)
                    windowclosing = new DelegateCommand(CanCloseWindow, ExecuteClosing);
                return windowclosing;
            }
        }

        #endregion

    }
}
