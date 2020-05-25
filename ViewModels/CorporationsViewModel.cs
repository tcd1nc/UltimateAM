using System.Collections.ObjectModel;
using System.Linq;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class CorporationsViewModel :ObjectCRUDViewModel
    {
        FullyObservableCollection<CorporationModel> _corporations = new FullyObservableCollection<CorporationModel>();
        bool isdirty = false;

        public CorporationsViewModel()
        {
            Corporations = GetCorporations();
            Corporations.ItemPropertyChanged += Corporations_ItemPropertyChanged;
           
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            if (Corporations.Count > 0)
                ScrollToSelectedItem = 0;
           
        }

        private void Corporations_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")            
                IsError = (IsDuplicateName(Corporations[e.CollectionIndex].Name));
            if (e.PropertyName != "Selected")
            {
                Corporations[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        public FullyObservableCollection<CorporationModel> Corporations
        {
            get { return _corporations; }
            set { SetField(ref _corporations, value); }
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

        private bool IsDuplicateName(string name)
        {
           return  Corporations.Count(x => x.Name == name)>1;            
        }
        
        #region Commands
       
        private bool CanExecuteAddNew(object obj)
        {         
            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            Corporations.Add(new CorporationModel() {ID=0, Name=string.Empty, LogoID=0 });
            ScrollToSelectedItem = Corporations.Count - 1;            
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
                foreach (CorporationModel corp in Corporations)
                {
                    if (corp.IsDirty)
                    {
                        if (!string.IsNullOrEmpty(corp.Name))
                            if (corp.ID == 0)
                                corp.ID = AddCorporation(corp);
                            else
                                UpdateCorporation(corp);

                        corp.IsDirty = false;
                    }
                }
            }
            isdirty = false;
        }
        
        private bool CanExecuteDelete(object obj)
        {
            return Corporations.Count(x=>x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<CorporationModel> deleteditems = new Collection<CorporationModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Corporation";
            string confirmtxt = "Do you want to delete the selected item";
            if (Corporations.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (CorporationModel si in Corporations)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "Corporations");
                        deleteditems.Add(si);
                    }
                }
                foreach (CorporationModel pm in deleteditems)
                {
                    Corporations.Remove(pm);
                }
                deleteditems.Clear();
            }
            msg = null;
        }

        //bool canexecuteaddimage = true;
        //private bool CanExecuteAddImage(object obj)
        //{
        //    return canexecuteaddimage;
        //}
        
        //ICommand addimage;
        //public ICommand AddImage
        //{
        //    get
        //    {
        //        if (addimage == null)
        //            addimage = new DelegateCommand(CanExecuteAddImage, ExecuteAddImage);
        //        return addimage;
        //    }
        //}

        //private void ExecuteAddImage(object parameter)
        //{
        //    IMessageBoxService msg = new MessageBoxService();
        //    string filename = msg.OpenFileDlg("Select the file containing the corporation's logo", true, false, "Logo Files(*.ICO; *.PNG; *.JPG)| *.ICO; *.PNG; *.JPG", string.Empty, null);
        //    msg = null;

        //    if (!string.IsNullOrEmpty(filename))
        //    {
        //       // ((CorporationModel)parameter).Logo = File.ReadAllBytes(filename);
        //        if (((CorporationModel)parameter).ID > 0)
        //        {
        //            //if (((CorporationModel)parameter).LogoID == 0)
        //            //    AddCorporationLogo(((CorporationModel)parameter).ID, ((CorporationModel)parameter).Logo);
        //            //else
        //            //    UpdateCorporationLogo(((CorporationModel)parameter).LogoID, ((CorporationModel)parameter).Logo);
        //        }
        //    }
        //}

        //bool canexecutedeleteimage = true;
        //private bool CanExecuteDeleteImage(object obj)
        //{
        //    return canexecutedeleteimage;
        //}
        
        //ICommand deleteimage;
        //public ICommand DeleteImage
        //{
        //    get
        //    {
        //        if (deleteimage == null)
        //            deleteimage = new DelegateCommand(CanExecuteDeleteImage, ExecuteDeleteImage);
        //        return deleteimage;
        //    }
        //}

        //private void ExecuteDeleteImage(object parameter)
        //{
        //    ((CorporationModel)parameter).Logo = new byte[0];
            
        //    //delete logo
        //    //if (((CorporationModel)parameter).ID > 0)
        //    //    UpdateCorporationLogo(((CorporationModel)parameter).LogoID, ((CorporationModel)parameter).Logo);

        //    //((CorporationModel)parameter).LogoID = 0;
        //}




        #endregion

    }
}
