
using System.ComponentModel;


namespace AssetManager.Models
{
    public class zAssetSpecificationModel : ViewModelBase, IDataErrorInfo
    {

        public int ID
        {
            get; set;
        }

        public int AssetID
        {
            get; set;
        }

        public int SpecificationID
        {
            get; set;
        }

        public string SpecificationName
        {
            get; set;
        }

        public string Error
        {
            get
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error in Specification Type Model", "Error in Specification Type Model", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                _msg = null;
                return "Error in Specification Type";
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case "SpecificationName":
                        if (string.IsNullOrEmpty(SpecificationName))
                            result = "Name is required!";

                        foreach (Models.AssetSpecificationTypeModel am in StaticClasses.GlobalClass.AssetSpecificationTypes)
                        {
                            if (ID != am.ID && am.SpecificationName == SpecificationName)
                                result = "Name is not unique";
                        }
                        break;
                };
                return result;
            }
        }

    }
}
