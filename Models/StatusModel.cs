using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class StatusModel : ViewModelBase, IDataErrorInfo
    {
        public int ID
        {
            get; set;
        }

        string _status;
        public string Status
        {
            get { return _status; }
            set { SetField(ref _status, value); }
        }

        bool _default;
        public bool Default
        {
            get { return _default; }
            set { SetField(ref _default, value); }
        }

        public string Error
        {
            get
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error in Status Model", "Error in Status Model", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                _msg = null;
                return "Error in Status";
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case "Status":
                        if (string.IsNullOrEmpty(Status))
                            result = "Name is required!";
                        //else
                        //    foreach (Models.StatusModel am in GlobalClass.AssetStatuses)
                        //    {
                        //        if (ID != am.ID && am.Status == Status)
                        //            result = "Name is not unique";
                        //    }
                        break;
                };
                return result;
            }
        }


    }
}
