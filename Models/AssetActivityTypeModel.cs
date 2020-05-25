using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class AssetActivityTypeModel :ViewModelBase, IDataErrorInfo
    {
        public int ID
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }


        public string Error
        {
            get
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error in Asset Activity Type Model", "Error in Asset Activity Type Model", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                _msg = null;
                return "Error in Asset Activity Type";
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case "Description":
                        if (string.IsNullOrEmpty(Description))
                            result = "Name is required!";

                        foreach (Models.AssetActivityTypeModel am in StaticClasses.GlobalClass.AssetActivityTypes)
                        {
                            if (ID != am.ID && am.Description == Description)
                                result = "Name is not unique";
                        }
                        break;
                };
                return result;
            }
        }
    }
}
