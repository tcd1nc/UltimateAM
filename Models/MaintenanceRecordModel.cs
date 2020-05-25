using System;

namespace AssetManager.Models
{
    public class MaintenanceRecordModel : BaseModel
    {
        int _assetid;
        public int AssetID
        {
            get { return _assetid; }
            set { SetField(ref _assetid, value); }
        }

        DateTime? _maintenancedate;
        public DateTime? MaintenanceDate
        {
            get { return _maintenancedate; }
            set { SetField(ref _maintenancedate, value); }
        }
              
        decimal _cost;
        public decimal Cost
        {
            get { return _cost; }
            set { SetField(ref _cost, value); }
        }

        string _maintainedby;
        public string MaintainedBy
        {
            get { return _maintainedby; }
            set { SetField(ref _maintainedby, value); }
        }

        DateTime? _scheduledmaintenancedate;
        public DateTime? ScheduledMaintenanceDate
        {
            get { return _scheduledmaintenancedate; }
            set { SetField(ref _scheduledmaintenancedate, value); }
        }

        bool _completed;
        public bool Completed
        {
            get { return _completed; }
            set { SetField(ref _completed, value); }
        }

        string _customer;
        public string CustomerName
        {
            get { return _customer; }
            set { SetField(ref _customer, value); }
        }
        
        int labelid;
        public int LabelID
        {
            get { return labelid; }
            set { SetField(ref labelid, value); }
        }

        int groupid;
        public int GroupID
        {
            get { return groupid; }
            set { SetField(ref groupid, value); }
        }

        int areaid;
        public int AreaID
        {
            get { return areaid; }
            set { SetField(ref areaid, value); }
        }
    }
}
