using System.IO;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using static AssetManager.SQLiteQueries;
using static AssetManager.GlobalClass;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class AssetViewModel : ViewModelBase
    {          
        //ICommand _showassembly;

        AssetModel asset;
        FullyObservableCollection<PhotoModel> photos;
        FullyObservableCollection<MaintenanceRecordModel> maintenance;
        FullyObservableCollection<MaintenanceRecordModel> scheduledmaintenance;
        FullyObservableCollection<AssetAreaModel> assetareas;
        FullyObservableCollection<AssetGroupModel> MasterAssetGroups;
        FullyObservableCollection<AssetTypeModel> MasterAssetTypes;
        FullyObservableCollection<AssetGroupModel> filteredassetgroups;
        FullyObservableCollection<AssetTypeModel> filteredassettypes;
        FullyObservableCollection<AssetModel> consumables;
        FullyObservableCollection<AssetGroupSpecificationValuesModel> assetgrpspecificationvalues;
        FullyObservableCollection<BaseModel> assetstatuses;      
        Dictionary<int, string> assetlabels = new Dictionary<int,string>();
        FullyObservableCollection<CustomerModel> customers;

        bool isdirty = false;
        bool _dirtyspecs;

        public AssetViewModel(int customerid, int parentid)
        {
            
            Asset = new AssetModel
            {
                ParentAssetID = parentid,
                CustomerID = customerid,
                DateInstalled = DateTime.Now,
                DatePurchased = DateTime.Now,
                NextAuditDate = DateTime.Now.AddMonths(12),
                Temporary = true
            };
            customers = GetCustomers();
            CustomerModel cm = customers.Where(x => x.ID == customerid).FirstOrDefault();
           
            Asset.Customer = cm.Name;
            Asset.Location = cm.Location;

            Asset.ID = AddAsset(Asset);
            LoadViewSettings(Asset.ID);

        }

        AssetTreeExViewModel atvm;
        TVAssetViewModel original;

        public AssetViewModel(AssetTreeExViewModel tva, TVAssetViewModel obj)
        {
            original = obj;
            atvm = tva;
            Asset = GetAsset(original.Asset.ID);
            LoadViewSettings(original.Asset.ID);
            IsError = false;
        }

        int photoindex;
        public int SelectedPhotoIndex
        {
            get { return photoindex; }
            set { SetField(ref photoindex, value); }
        }

        private void LoadViewSettings(int id)
        {
            AssetStatuses = Statuses; 
            AssetAreas = GetAssetAreas();
            MasterAssetGroups = GetAssetGroups();
            MasterAssetTypes = GetAssetTypes();
            //GetGroupSpecifications(Asset.AssetGroupID);

            Photos = GetAssetPhotos(id);
            if (Photos.Count > 0)
                SelectedPhotoIndex = 0;
                        
            MaintenanceRecords = GetMaintenanceRecords(Asset.ID);
            MaintenanceRecords.ItemPropertyChanged += maintenance_ItemPropertyChanged;
            ScheduledMaintenance = GetScheduledMaintenance(Asset.ID);
            ScheduledMaintenance.ItemPropertyChanged += ScheduledMaintenance_ItemPropertyChanged;
            Consumables = GetAssetConsumables(Asset.ID, false);
            Consumables.ItemPropertyChanged += consumables_ItemPropertyChanged;

            LabelTextBlock = GetParentLabel(Asset.ParentAssetID);
            asset.PropertyChanged += asset_PropertyChanged;
            ErrorMsg = string.Empty;

            assetlabels = GetAssetLabels();

            ValidateAsset();
           
            _enablemovements = true;  
            IsEnabled = IsAdministrator;

            ParentLabel = Asset.ParentAssetID.ToString();
        }

        private string GetParentLabel(int id)
        {
            if (id > 0)
                return MakeLabel(Asset.AssetAreaID, Asset.AssetGroupID, Asset.LabelID) + " is a component of Asset: " + Asset.ParentAssetID.ToString();
            else
                return MakeLabel(Asset.AssetAreaID, Asset.AssetGroupID, Asset.LabelID);
        }

        #region Event handling

        private void ScheduledMaintenance_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            ScheduledMaintenance[e.CollectionIndex].IsDirty = true;
            isdirty = true;
        }
   
        private void maintenance_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            MaintenanceRecords[e.CollectionIndex].IsDirty = true;
            isdirty = true;
        }

        private void consumables_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {         
            Consumables[e.CollectionIndex].IsDirty = true;
            isdirty = true;
        }

        private void AssetGroupSpecificationValues_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            _dirtyspecs = true;
            AssetGroupSpecificationValues[e.CollectionIndex].IsDirty = true;
            isdirty = true;
        }

        private void asset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            isdirty = true;

            if (e.PropertyName == "LabelID" || e.PropertyName == "AssetGroupID" || e.PropertyName == "AssetAreaID")
            {
                LabelTextBlock = MakeLabel(Asset.AssetAreaID, Asset.AssetGroupID, Asset.LabelID);
                //IsError = IsDuplicateLabel(LabelTextBlock);
                //if (IsError)
                //    ErrorMsg = "Duplicate Label";
            }

            ValidateAsset();
            
        }

        #endregion
        
        #region Public Properties

        AssetAreaModel _selectedarea;
        public AssetAreaModel SelectedArea
        {
            get { return _selectedarea; }
            set { SetField(ref _selectedarea, value);
                if (value != null)
                {
                    FilterAssetGroups(value.ID);
                }
            }
        }

        AssetGroupModel _selectedgroup;
        public AssetGroupModel SelectedGroup
        {
            get { return _selectedgroup; }
            set
            {
                SetField(ref _selectedgroup, value);
                if (value != null)
                {
                    GetGroupSpecifications(value.ID);
                    FilterAssetTypes(value.ID);
                }
            }
        }
                       
        public AssetModel Asset
        {
            get { return asset; }
            set { SetField(ref asset, value); }
        }

        AssetTypeModel _selectedAssetType;
        public AssetTypeModel SelectedAssetType
        {
            get { return _selectedAssetType; }
            set { SetField(ref _selectedAssetType, value); }
        }
        public FullyObservableCollection<AssetAreaModel> AssetAreas
        {
            get { return assetareas; }
            set { SetField(ref assetareas,value); }
        }       
        public FullyObservableCollection<AssetGroupModel> AssetGroups
        {
            get { return filteredassetgroups; }
            set { SetField(ref filteredassetgroups, value); }
        }
        public FullyObservableCollection<AssetTypeModel> AssetTypes
        {
            get { return filteredassettypes; }
            set { SetField(ref filteredassettypes, value); }
        }
        public FullyObservableCollection<BaseModel> AssetStatuses
        {
            get { return assetstatuses; }
            set { SetField(ref assetstatuses, value); }
        }
        public FullyObservableCollection<PhotoModel> Photos
        {
            get { return photos; }
            set { SetField(ref photos, value); }
        }
        public FullyObservableCollection<MaintenanceRecordModel> MaintenanceRecords
        {
            get { return maintenance; }
            set { SetField(ref maintenance, value); }
        }  
        public FullyObservableCollection<MaintenanceRecordModel> ScheduledMaintenance
        {
            get { return scheduledmaintenance; }
            set { SetField(ref scheduledmaintenance, value); }
        }
        public FullyObservableCollection<AssetModel> Consumables
        {
            get { return consumables; }
            set { SetField(ref consumables, value); }
        }
        public FullyObservableCollection<AssetGroupSpecificationValuesModel> AssetGroupSpecificationValues
        {
            get { return assetgrpspecificationvalues; }
            set { SetField(ref assetgrpspecificationvalues, value); }
        }

        bool _showusedconsumables;
        public bool ShowUsedConsumables
        {
            get { return _showusedconsumables; }
            set {
                SaveDirtyConsumables();
                Consumables = GetAssetConsumables(Asset.ID, value);
                SetField(ref _showusedconsumables, value); }
        }

        string _viewtitle;
        public string LabelTextBlock
        {
            get { return _viewtitle; }
            set { SetField(ref _viewtitle, value); }
        }

        string parentlabel;
        public string ParentLabel
        {
            get { return parentlabel; }
            set { SetField(ref parentlabel, value); }
        }

        PhotoModel _selectedphoto;
        public PhotoModel SelectedPhoto
        {
            get { return _selectedphoto; }
            set { SetField(ref _selectedphoto, value); }
        }

        bool iserror = false;
        public bool IsError
        {
            get { return iserror; }
            set { SetField(ref iserror, value); }
        }

        string errormsg;
        public string ErrorMsg
        {
            get { return errormsg; }
            set { SetField(ref errormsg, value); }
        }

        bool _enablemovements;
        public bool EnableMovements
        {
            get { return _enablemovements; }
            set { SetField(ref _enablemovements, value); }
        }

        #endregion


        private void GetGroupSpecifications(int groupid)
        {
            //get all specifications and values for this asset and group
            AssetGroupSpecificationValues = GetAssetGroupSpecificationValues(Asset.ID, groupid);            
            AssetGroupSpecificationValues.ItemPropertyChanged += AssetGroupSpecificationValues_ItemPropertyChanged;            
        }

        private void ValidateAsset()
        {
            bool assetareareqd = !(Asset.AssetAreaID > 0);
            bool assetgroupreqd = !(Asset.AssetGroupID > 0);
            bool assettypereqd = !(Asset.AssetTypeID > 0);
            bool labelidreqd = !(Asset.LabelID > 0);

            string label = MakeLabel(Asset.AssetAreaID, Asset.AssetGroupID, Asset.LabelID);
            bool duplicatelabel = IsDuplicateLabel(label);

            IsError = assetareareqd || assetgroupreqd || assettypereqd || labelidreqd || duplicatelabel;
            if (assetareareqd)
                ErrorMsg = "Asset Area Missing";
            else
               if (assetgroupreqd)
                ErrorMsg = "Asset Group Missing";
            else
            if (assettypereqd)
                ErrorMsg = "Asset Type Missing";
            else
            if (labelidreqd)
                ErrorMsg = "Asset Number Missing";
            else
                if (duplicatelabel)
                ErrorMsg = "Duplicate Label";
        }

        private bool IsDuplicateLabel(string label)
        {
            return (assetlabels.Count(x => x.Value == label && x.Key != Asset.ID)) > 0;           
        }


        private void DeleteTemporaryAsset()
        {
            if(Asset.Temporary == true)
            {
                DeleteItem(Asset.ID, "Assets");
            }
        }


        #region Commands

        bool _isenabled = true;
        public bool IsEnabled
        {
            get { return _isenabled; }
            set { SetField(ref _isenabled, value); }
        }
      
        ICommand _scheduleaudit;
        public ICommand ScheduleAudit
        {
            get
            {
                if (_scheduleaudit == null)
                    _scheduleaudit = new DelegateCommand(CanExecute, ExecuteScheduleAudit);

                return _scheduleaudit;
            }
        }

        private void ExecuteScheduleAudit(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            DateTime? _dt =  msg.OpenAssetAuditsDlg(Asset.ID);
            Asset.NextAuditDate = _dt;
        }


        private bool CanExecuteDeletePhoto(object obj)
        {
            if (SelectedPhoto == null)
                return false;
            return true;
        }

        ICommand _deletephoto;
        public ICommand DeletePhotoCommand
        {
            get
            {
                if (_deletephoto == null)
                    _deletephoto = new DelegateCommand(CanExecuteDeletePhoto, DeletePhoto);
                return _deletephoto;
            }
        }

        private void DeletePhoto(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            if (msg.ShowMessage("Are you sure you want to remove this photo?", "Removing Photo from Asset", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.Yes))
            {
                Photos.Remove(parameter as PhotoModel);
                SQLiteQueries.DeletePhoto((parameter as PhotoModel));
            }
            msg = null;
        }

        ICommand _addphoto;
        public ICommand AddPhotoCommand
        {
            get
            {
                if (_addphoto == null)
                    _addphoto = new DelegateCommand(CanExecute, AddPhoto);
                return _addphoto;
            }
        }

        private void AddPhoto(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string filename = msg.OpenFileDlg("Select the file containing the asset's photo", true, false, "Asset Photos(*.PNG; *.JPG)| *.PNG; *.JPG", string.Empty, null);
            
            if (!string.IsNullOrEmpty(filename) && !string.IsNullOrWhiteSpace(filename))
            {                
                FileInfo fi = new FileInfo(filename);
                if (fi.Length > Defaults.MaxPhotoSize)
                    msg.ShowMessage("The photo file size is too large (" + (Defaults.MaxPhotoSize/1000).ToString() + " kB)", "Photo Size", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Information);
                else
                {
                    //PhotoModel Photo = new PhotoModel(){ ID = 0, AssetID = Asset.ID, Photo = File.ReadAllBytes(filename), PhotoFileName = filename };

                    filename = Path.GetFileName(filename);

                    PhotoModel Photo = new PhotoModel() { ID = 0, AssetID = Asset.ID,  PhotoFileName = filename };

                    Photo.ID = SQLiteQueries.AddPhoto(Photo);

                    if (Photo.ID > 0)
                    {
                        Photos.Add(Photo);
                        SelectedPhotoIndex = Photos.Count - 1;
                    }
                }
            }
            else
            {
            //not added
            }
        }

        ICommand _addmaintenancecommand;
        public ICommand AddMaintenanceCommand
        {
            get
            {
                if (_addmaintenancecommand == null)
                    _addmaintenancecommand = new DelegateCommand(CanExecute, ExecuteAddMaintenance);
                return _addmaintenancecommand;
            }
        }

        private void ExecuteAddMaintenance(object parameter)
        {
            MaintenanceRecordModel mm = new MaintenanceRecordModel()
            {
                AssetID = Asset.ID,
                Name = string.Empty,
                MaintenanceDate = DateTime.Now,
                MaintainedBy = string.Empty,
                Completed = true
            };            
            mm.ID = AddMaintenanceRecord(mm);
            MaintenanceRecords.Add(mm);
        }

        ICommand _deletemaintenancecommand;
        public ICommand DeleteMaintenanceCommand
        {
            get
            {
                if (_deletemaintenancecommand == null)
                    _deletemaintenancecommand = new DelegateCommand(CanExecute, ExecuteDeleteMaintenance);
                return _deletemaintenancecommand;
            }
        }
        private void ExecuteDeleteMaintenance(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            if (parameter != null)
            {
                if (msg.ShowMessage("Are you sure you want to delete this maintenance record?", "Deleting Maintenance Record", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.Yes))
                {
                    MaintenanceRecords.Remove((MaintenanceRecordModel)parameter);
                    DeleteItem(((MaintenanceRecordModel)parameter).ID, "MaintenanceRecords");
                }
            }
            msg = null;
        }

        ICommand _addscheduledmaintenancecommand;
        public ICommand AddScheduledMaintenanceCommand
        {
            get
            {
                if (_addscheduledmaintenancecommand == null)
                    _addscheduledmaintenancecommand = new DelegateCommand(CanExecute, ExecuteAddScheduledMaintenance);
                return _addscheduledmaintenancecommand;
            }
        }

        private void ExecuteAddScheduledMaintenance(object parameter)
        {
            MaintenanceRecordModel mm = new MaintenanceRecordModel()
            {
                AssetID = Asset.ID,               
                Completed = false,
                Name = string.Empty,
                ScheduledMaintenanceDate = DateTime.Now
            };           
            mm.ID = AddScheduledMaintenance(mm);
            ScheduledMaintenance.Add(mm);        
        }

        ICommand _deletescheduledmaintenancecommand;
        public ICommand DeleteScheduledMaintenanceCommand
        {
            get
            {
                if (_deletescheduledmaintenancecommand == null)
                    _deletescheduledmaintenancecommand = new DelegateCommand(CanExecute, ExecuteDeleteScheduledMaintenance);
                return _deletescheduledmaintenancecommand;
            }
        }
        private void ExecuteDeleteScheduledMaintenance(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            if (parameter != null)
            {
                if (msg.ShowMessage("Are you sure you want to delete this scheduled maintenance?", "Deleting Scheduled Maintenance", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.Yes))
                {
                    ScheduledMaintenance.Remove((MaintenanceRecordModel)parameter);
                   // DeleteItem(((MaintenanceRecordModel)parameter).ID, "ScheduledMaintenance");
                }
            }
            msg = null;
        }

        private bool CanExecuteDeleteChildAsset(object obj)
        {
            return true;
        }
      
        ICommand _clearSelection;
        public ICommand ClearSelection
        {
            get
            {
                if (_clearSelection == null)
                    _clearSelection = new DelegateCommand(CanExecute, ClearObject);
                return _clearSelection;
            }
        }

        private void ClearObject(object parameter)
        {
            switch (parameter as string)
            {
                case "DateInstalled":
                    Asset.DateInstalled = null;
                    break;
            }
        }

        ICommand _addconsumablecommand;
        public ICommand AddConsumableCommand
        {
            get
            {
                if (_addconsumablecommand == null)
                    _addconsumablecommand = new DelegateCommand(CanExecute, ExecuteAddConsumable);
                return _addconsumablecommand;
            }
        }
        private void ExecuteAddConsumable(object parameter)
        {
            AssetModel cm = new AssetModel() {
                ID = 0,
                Name = string.Empty,
                ParentAssetID = Asset.ID,
                PONumber = string.Empty,
                DatePurchased = DateTime.Now
            };            
            cm.ID = AddConsumable(cm);
            Consumables.Add(cm);            
        }

        ICommand _deleteconsumablecommand;
        public ICommand DeleteConsumableCommand
        {
            get
            {
                if (_deleteconsumablecommand == null)
                    _deleteconsumablecommand = new DelegateCommand(CanExecute, ExecuteDeleteConsumable);
                return _deleteconsumablecommand;
            }
        }

        private void ExecuteDeleteConsumable(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            if (parameter != null)
            {
                if (msg.ShowMessage("Are you sure you want to delete this consumable item?", "Deleting Consumable Record", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.Yes))
                {
                    Consumables.Remove((AssetModel)parameter);
                    DeleteItem(((AssetModel)parameter).ID, "Assets");
                }
            }
            msg = null;
        }
        
        private bool canexecutecancel(object obj)
        {                                     
            return true;
        }
                
        ICommand cancel;
        public ICommand Cancel
        {
            get
            {
                if (cancel == null)
                    cancel = new DelegateCommand(canexecutecancel, ExecuteCancel);
                return cancel;
            }
        }

        private void ExecuteCancel(object parameter)
        {            
            CloseWindow();
        }

        private void UpdateMainWindow()
        {
            if(SelectedAssetType!=null)
                Asset.Category = SelectedAssetType.Name;
            if (original != null)           
                atvm.ProcessAssetChanges(original, Asset);                            
        }

        private bool canexecutesave(object obj)
        {
            if (!isdirty)
                return false;
            if (IsError)
                return false;

            return true;
        }

        ICommand save;
        public ICommand Save
        {
            get
            {
                if (save == null)
                    save = new DelegateCommand(canexecutesave, ExecuteSave);
                return save;
            }
        }

        private void ExecuteSave(object parameter)
        {
            SaveAll();
        }

        private bool CanExecuteShowMovements(object obj)
        {
            return true;
        }
        
        ICommand _showmovements;
        public ICommand ShowMovementsCommand
        {
            get
            {
                if (_showmovements == null)
                    _showmovements = new DelegateCommand(CanExecuteShowMovements, ShowMovements);
                return _showmovements;
            }
        }

        private void ShowMovements(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            msg.OpenAssetMovementsDlg((int)parameter);
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
                        //UpdateMainWindow();
                        return true;
                    }
                    else
                    {
                        DeleteTemporaryAsset();
                        return true;
                    }
                }
                else
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes with errors. Do you want to correct these?", "Unsaved Changes with Errors", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                        return false;
                    else
                    {
                        DeleteTemporaryAsset();
                        return true;
                    }                    
                }
            }
            else
            {
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


        #region Private Save functions

        private void SaveDirtyMaintenanceRecords()
        {                          
            foreach(MaintenanceRecordModel mrm in MaintenanceRecords)
            {
                if (mrm.IsDirty)
                {
                    UpdateMaintenanceRecord(mrm);
                    mrm.IsDirty = false;
                }
            }                         
        }
               
        private void SaveDirtyConsumables()
        {
            foreach(AssetModel cm in Consumables)
            {
                if (cm.IsDirty)
                {
                    UpdateConsumable(cm);
                    cm.IsDirty = false;
                }
            }                
        }

        private void SaveDirtySpecificationRecords()
        {
            if (_dirtyspecs)
            {
                foreach(AssetGroupSpecificationValuesModel am in AssetGroupSpecificationValues)
                {
                    if (am.IsDirty)
                    {
                        if (am.AssetGroupSpecificationValueID > 0)
                            UpdateAssetGroupSpecificationValue(am);
                        else
                            am.AssetGroupSpecificationValueID = AddAssetGroupSpecificationValue(am);
                    }
                    am.IsDirty = false;
                }
                _dirtyspecs=false;
            }
        }

        private void SaveDirtyScheduledMaintenance()
        {
           foreach(MaintenanceRecordModel mrm in ScheduledMaintenance)
            {
                if (mrm.IsDirty)
                {
                    if (mrm.Completed)
                        mrm.MaintenanceDate = mrm.ScheduledMaintenanceDate;
                    UpdateMaintenanceRecord(mrm);
                    mrm.IsDirty = false;
                }
            }
        }
        
        public void SaveAll()
        {           
            SaveDirtyMaintenanceRecords();
            SaveDirtyConsumables();
            SaveDirtySpecificationRecords();
            SaveDirtyScheduledMaintenance();
            Asset.Temporary = false;
            UpdateAsset(Asset);
            UpdateMainWindow();
            isdirty = false;
        }

        #endregion

        #region Filtering for areas, groups and types

        private void FilterAssetGroups(int assetareaid)
        {
            var subs = MasterAssetGroups.Where(x => x.AssetAreaID == assetareaid);
            bool _isfound = false;
            FullyObservableCollection<AssetGroupModel> _temp = new FullyObservableCollection<AssetGroupModel>();

            foreach (AssetGroupModel agm in subs)
            {
                _temp.Add(agm);
                if (agm.ID == Asset.AssetGroupID)
                    _isfound = true;                
            }
            if (!_isfound)
            {
                Asset.AssetGroupID = -1;
                Asset.AssetTypeID = -1;
                AssetTypes?.Clear();
            }
            AssetGroups = _temp;
   
        }

        private void FilterAssetTypes(int assetgroupid)
        {
            var subs = MasterAssetTypes.Where(x => x.AssetGroupID == assetgroupid);
            bool _isfound = false;
            FullyObservableCollection<AssetTypeModel> _temp = new FullyObservableCollection<AssetTypeModel>();
            foreach (AssetTypeModel atm in subs)
            {
                _temp.Add(atm);
                if (atm.ID == Asset.AssetTypeID)
                    _isfound = true;
            }
            if (!_isfound)            
                Asset.AssetTypeID = -1;                
            
            AssetTypes = _temp;
        }

        #endregion


    }
}
