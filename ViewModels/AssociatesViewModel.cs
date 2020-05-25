using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class AssociatesViewModel : ViewModelBase
    {
        FullyObservableCollection<Models.AssociateModel> _associates;
        Models.AssociateModel _associate;
        bool _isediting;
        private Dictionary<string, bool> validProperties;

        public AssociatesViewModel()
        {
           // _associates = new FullyObservableCollection<Models.AssociateModel>();
            _associates = DataLayer.DatabaseQueries.GetAssociates();
         
            //populate from database
            validProperties = new Dictionary<string, bool>();
            validProperties.Add("AssociateName", false);

            _associate = new Models.AssociateModel();
            _isediting = true;
            _scrolltolastitem = false;         
            ScrollToSelectedItem = 0;
           
        }
     

        public FullyObservableCollection<Models.AssociateModel> Associates
        {
            get { return _associates; }
            set { SetField(ref _associates, value); }
        }

        public Models.AssociateModel Associate
        {
            get { return _associate; }
            set {
                if (value != null)
                    SetField(ref _associate, value); }
        }

        #region Commands

        ICommand _addnew;
        public ICommand AddNew
        {
            get
            {
                if (_addnew == null)
                    _addnew = new DelegateCommand(CanExecuteAddNew, ExecuteAddNew);
                return _addnew;
            }
        }

        bool _canexecuteadd = true;
        private bool CanExecuteAddNew(object obj)
        {
            string error = (_associate as IDataErrorInfo)["AssociateName"];
            if (!string.IsNullOrEmpty(error))
                return false;

            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            _canexecuteadd = false;
            Associates.Add(new Models.AssociateModel());
            ScrollToLastItem = true;
            AssociateListEnabled = false;
        }

        public bool AssociateListEnabled
        {
            get { return _isediting; }
            set { SetField(ref _isediting, value); }
        }

        bool _scrolltolastitem;
        public bool ScrollToLastItem
        {
            get { return _scrolltolastitem; }
            set { SetField(ref _scrolltolastitem, value); }
        }

        int _scrolltoselecteditem;
        public int ScrollToSelectedItem
        {
            get { return _scrolltoselecteditem; }
            set { SetField(ref _scrolltoselecteditem, value); }
        }


        //save
        ICommand _cancel;
        public ICommand Cancel
        {
            get
            {
                if (_cancel == null)
                    _cancel = new DelegateCommand(CanExecute, ExecuteCancel);
                return _cancel;
            }
        }

        private void ExecuteCancel(object parameter)
        {
           // _canexecuteadd = true;
            DialogResult = false;
            CloseWindow();
        }

        bool _canexecutesave = true;
        private bool CanExecuteSave(object obj)
        {
            string error = (_associate as IDataErrorInfo)["AssociateName"];
            if (!string.IsNullOrEmpty(error))
                return false;

            return _canexecutesave;
        }

        ICommand _saveandclose;
        public ICommand SaveAndClose
        {
            get
            {
                if (_saveandclose == null)
                    _saveandclose = new DelegateCommand(CanExecuteSave, ExecuteSaveAndClose);
                return _saveandclose;
            }
        }
        
        private void ExecuteSaveAndClose(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            _canexecuteadd = true;
            DialogResult = true;
            if (!_isediting)
            {
                Models.AssociateModel _newassoc = new Models.AssociateModel();
                _newassoc.AssociateName = Associate.AssociateName;// _associatename;
                _newassoc.LoginName = Associate.LoginName;// _associatelogin ?? string.Empty; 

                DataLayer.DatabaseQueries.AddAssociate(_newassoc);                
            }
            else
            {
                DataLayer.DatabaseQueries.UpdateAssociate(_associate);
            }              
            CloseWindow();                   
        }

        #endregion

    }
}
