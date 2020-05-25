using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class SelectCustomerViewModel :ViewModelBase
    {
        FullyObservableCollection<CustomerModel> _customers;// = new FullyObservableCollection<Models.CustomerModel>();
        public SelectCustomerViewModel()
        {
            Customers = GetCustomers();
            InstructionLabel = "Select Customer for new Asset";
          
        }

        public FullyObservableCollection<CustomerModel> Customers
        {
            get { return _customers; }
            set { SetField(ref _customers, value); }
        }

        int _selectedDefault;
        public int CustomerID
        {
            get { return _selectedDefault; }
            set { SetField(ref _selectedDefault, value); }
        }

        public string InstructionLabel
        {
            get;
            internal set;
        }

        ICommand _selectdefaultcustomer;
        public ICommand SelectCustomerCommand
        {
            get
            {
                if (_selectdefaultcustomer == null)
                    _selectdefaultcustomer = new DelegateCommand(CanExecute, SelectDefaultCustomer);
                return _selectdefaultcustomer;
            }
        }

        private void SelectDefaultCustomer(object parameter)
        {
            ObjDialogResult = CustomerID;

            CloseWindow();

        }
    }
}
