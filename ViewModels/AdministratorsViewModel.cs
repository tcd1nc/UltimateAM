using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;
using System.Collections.ObjectModel;

namespace AssetManager.ViewModels
{
    public class AdministratorsViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<AdministratorUserModel> _administrators = new FullyObservableCollection<AdministratorUserModel>();
        bool isdirty = false;

        public AdministratorsViewModel()
        {
            Administrators = GetAdministrators();
            Administrators.ItemPropertyChanged += Administrators_ItemPropertyChanged;
           
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (Administrators.Count>0)
                ScrollToSelectedItem = 0;      
            
        }

        private void Administrators_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")            
                IsError = (IsDuplicateName(Administrators[e.CollectionIndex].Name));
            if (e.PropertyName != "Selected")
            {
                Administrators[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<AdministratorUserModel> Administrators
        {
            get { return _administrators; }
            set { SetField(ref _administrators, value); }
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
            var q = Administrators.Count(x => x.Name == _name);
            return (q > 1);
        }
                
        #region Commands
        
        private bool CanExecuteAddNew(object obj)
        {            
            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            Administrators.Add(new AdministratorUserModel(){ID=0, Name = string.Empty, LoginName=string.Empty});
            ScrollToSelectedItem = Administrators.Count - 1;
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
                foreach (AdministratorUserModel admin in Administrators)
                {
                    if (admin.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(admin.Name) && !string.IsNullOrEmpty(admin.LoginName))
                            if (admin.ID == 0)
                                admin.ID = AddAdministrator(admin);
                            else
                                UpdateAdministrator(admin);
                        admin.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }

        //Delete
        private bool CanExecuteDelete(object obj)
        {
            return Administrators.Count(x=>x.Selected) > 0;
        }
        
        private void ExecuteDelete(object parameter)
        {
            Collection<AdministratorUserModel> deleteditems = new Collection<AdministratorUserModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Administrator";
            string confirmtxt = "Do you want to delete the selected item";
            if (Administrators.Where(x => x.Selected).Count() > 1)
            {
                title += "s";
                confirmtxt += "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (AdministratorUserModel si in Administrators)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "Administrators");
                        deleteditems.Add(si);
                    }
                }
                foreach (AdministratorUserModel pm in deleteditems)
                {
                    Administrators.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }
        
        #endregion
    }
}
