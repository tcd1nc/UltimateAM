using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class PowerSourceModel : ViewModelBase, IDataErrorInfo
    {
        int _id;
        public int ID
        {
            get { return _id; }
            set { SetField(ref _id, value); }
        }

        string _powersourcename;
        public string PowerSource
        {
            get { return _powersourcename; }
            set { SetField(ref _powersourcename, value); }
        }

        public string Error
        {
            get
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error in Power Source Model", "Error in Power Source Model", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                _msg = null;
                return "Error in Power Source";
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case "PowerSource":
                        if (string.IsNullOrEmpty(PowerSource))
                            result = "Name is required!";

                        //foreach (Models.PowerSourceModel am in StaticClasses.GlobalClass.AssetPowerSources)
                        //{
                        //    if (ID != am.ID && am.PowerSource == PowerSource)
                        //        result = "Name is not unique";
                        //}
                        break;
                };
                return result;
            }
        }


    }
}
