using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class SalesDivisionsViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<SalesDivisionModel> _salesdivisions;
        bool isdirty = false;

        public SalesDivisionsViewModel()
        {
            SalesDivisions = GetSalesDivisions();
            SalesDivisions.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;
           
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (SalesDivisions.Count > 0)
                ScrollToSelectedItem = 0;
            
        }

        private void SalesDivisions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")            
                IsError = (IsDuplicateName(SalesDivisions[e.CollectionIndex].Name));
            if (e.PropertyName != "Selected")
            {
                SalesDivisions[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<SalesDivisionModel> SalesDivisions
        {
            get { return _salesdivisions; }
            set { SetField(ref _salesdivisions, value); }
        }

        private bool IsDuplicateName(string _name)
        {
           return SalesDivisions.Count(x => x.Name == _name) > 1;             
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
            SalesDivisions.Add(new SalesDivisionModel() {ID=0, Name=string.Empty, OperatingCompanyID=0 });
            ScrollToSelectedItem = SalesDivisions.Count - 1;
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
                foreach (SalesDivisionModel salesdiv in SalesDivisions)
                {
                    if (salesdiv.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(salesdiv.Name))
                            if (salesdiv.ID == 0)
                                AddSalesDivision(salesdiv);
                            else
                                UpdateSalesDivision(salesdiv);
                        salesdiv.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }

       
       
        private bool CanExecuteDelete(object obj)
        {        
            return SalesDivisions.Count(x => x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<SalesDivisionModel> deleteditems = new Collection<SalesDivisionModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Sales Division";
            string confirmtxt = "Do you want to delete the selected item";
            if (SalesDivisions.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (SalesDivisionModel si in SalesDivisions)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "SalesDivisions");
                        deleteditems.Add(si);
                    }
                }
                foreach (SalesDivisionModel pm in deleteditems)
                {
                    SalesDivisions.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }

            #endregion

        }
}
