using System;
using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using AssetManager.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace AssetManager.ViewModels
{
    public class ScheduledAuditsViewModel : ViewModelBase
    {
        FullyObservableCollection<AuditDateModel> _auditdates = new FullyObservableCollection<AuditDateModel>();
        bool isdirty = false;

        public ScheduledAuditsViewModel(int id)
        {
            AssetID = id;
            Audits = GetAssetAuditDates(id);
            Audits.ItemPropertyChanged += Audits_ItemPropertyChanged;  
            
            if(Audits.Count>0)
                ScrollToSelectedItem = 0;
           
        }

        private void Audits_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Selected")
            {
                Audits[e.CollectionIndex].IsDirty = true;
                isdirty = true;
            }
        }

        int id;
        public int AssetID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        public FullyObservableCollection<AuditDateModel> Audits
        {
            get { return _auditdates; }
            set { SetField(ref _auditdates, value); }
        }
                
        string _assetlabel;
        public string AssetLabel
        {
            get { return _assetlabel; }
            set { SetField(ref _assetlabel, value); }
        }
        
        int _scrolltoselecteditem;
        public int ScrollToSelectedItem
        {
            get { return _scrolltoselecteditem; }
            set { SetField(ref _scrolltoselecteditem, value); }
        }        

        #region Commands

        ICommand _addnew;
        public ICommand Add
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
            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {             
            Audits.Add(new AuditDateModel { DateAudit = DateTime.Now, AssetID = AssetID, IsDirty = true });            
            ScrollToSelectedItem = Audits.Count - 1;                  
        }

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
            CloseWindow();
        }

        bool _canexecutesave = true;
        private bool CanExecuteSave(object obj)
        {
            if (!isdirty)
                return false;
            if (Audits.Count(x => x.IsDirty) == 0)
                return false;
            return _canexecutesave;
        }

        ICommand save;
        public ICommand Save
        {
            get
            {
                if (save == null)
                    save = new DelegateCommand(CanExecuteSave, ExecuteSave);
                return save;
            }
        }

        private void ExecuteSave(object parameter)
        {
            if (isdirty)
            { 
                foreach (AuditDateModel audit in Audits)
                {
                    if (audit.IsDirty)
                    {
                        if (audit.ID == 0)
                            audit.ID = AddAuditDate(AssetID, audit.DateAudit);
                        else
                            UpdateAuditDate(audit.ID, audit.DateAudit);
                        audit.IsDirty = false;
                    }
                }
                isdirty = false;
            }   
            ObjDialogResult = GetNextAudit();
            
        }

        private DateTime? GetNextAudit()
        {
            DateTime _n = DateTime.Now;
            DateTime? _temp = null;
            foreach (AuditDateModel am in Audits)
            {
                //if (am.DateAudit >= _n)
                    if (am.DateAudit < _temp || _temp == null)
                        _temp = am.DateAudit;
            }
            return _temp;
        }


        ICommand _delete;
        public ICommand Delete
        {
            get
            {
                if (_delete == null)
                    _delete = new DelegateCommand(CanExecuteDelete, ExecuteDelete);
                return _delete;
            }
        }


        private bool CanExecuteDelete(object obj)
        {
            return Audits.Count(x => x.Selected) > 0;
        }

        private void ExecuteDelete(object parameter)
        {
            Collection<AuditDateModel> deleteditems = new Collection<AuditDateModel>();
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Audit Date";
            string confirmtxt = "Do you want to delete the selected item";
            if (Audits.Count(x => x.Selected) > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (AuditDateModel si in Audits)
                {
                    if (si.Selected)
                    {
                        if (si.ID > 0)
                            DeleteItem(si.ID, "AuditDates");
                        deleteditems.Add(si);
                    }
                }
                foreach (AuditDateModel pm in deleteditems)
                {
                    Audits.Remove(pm);
                }
                deleteditems.Clear();

            }
            msg = null;
        }
        #endregion

    }

    

}
