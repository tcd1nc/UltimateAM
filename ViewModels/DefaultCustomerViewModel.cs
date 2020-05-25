using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class DefaultCustomerViewModel : ViewModelBase
    {
        FullyObservableCollection<Models.CustomerModel> _customers = new FullyObservableCollection<Models.CustomerModel>();
        public DefaultCustomerViewModel()
        {
            _customers = SQLiteQueries.GetCustomers();// GlobalClass.Customers;
            
        }

        public FullyObservableCollection<Models.CustomerModel> DefaultCustomers
        {
            get { return _customers; }
            set { SetField(ref _customers, value); }
        }

        ICommand _selectdefaultcustomer;
        public ICommand SelectDefaultCustomerCommand
        {
            get
            {
                if (_selectdefaultcustomer == null)
                    _selectdefaultcustomer = new DelegateCommand(CanExecute, SelectDefaultCustomer);
                return _selectdefaultcustomer;
            }
        }

        int _selectedDefault;
        public int CustomerID
        {
            get { return _selectedDefault; }
            set { SetField(ref _selectedDefault, value); }
        }

        private void SelectDefaultCustomer(object parameter)
        {
            ObjDialogResult = CustomerID;
            DialogResult = true;
        }
    }
}
