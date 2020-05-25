using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class zScheduledMaintenanceModel :ViewModelBase
    {
        int _id;
        public int ID
        {
            get { return _id; }
            set { SetField(ref _id, value); }
        }

        int _assetid;
        public int AssetID
        {
            get { return _assetid; }
            set { SetField(ref _assetid, value); }
        }

        DateTime? _maintenancedate;
        public DateTime? DateScheduled
        {
            get { return _maintenancedate; }
            set { SetField(ref _maintenancedate, value); }
        }

        int _maintenancetypeid;
        public int MaintenanceTypeID
        {
            get { return _maintenancetypeid; }
            set { SetField(ref _maintenancetypeid, value); }
        }

        string _description;
        public string Description
        {
            get { return _description; }
            set { SetField(ref _description, value); }
        }

        bool _completed;
        public bool Completed
        {
            get { return _completed; }
            set { SetField(ref _completed, value); }
        }

        string _label;
        public string Label
        {
            get { return _label; }
            set { SetField(ref _label, value); }
        }

        string _customer;
        public string CustomerName
        {
            get { return _customer; }
            set { SetField(ref _customer, value); }
        }

    }
}
