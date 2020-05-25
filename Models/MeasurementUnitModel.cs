
using System.ComponentModel;

namespace AssetManager.Models
{
    public class zMeasurementUnitModel : ViewModelBase, IDataErrorInfo
    {
        public int ID
        {
            get; set;
        }

        string _measurement;
        public string MeasurementUnit
        {
            get { return _measurement; }
            set { SetField(ref _measurement, value); }
        }

    public string Error
    {
        get
        {
            IMessageBoxService _msg = new MessageBoxService();
            _msg.ShowMessage("Error in Asset Area Model", "Error in Asset Area Model", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            _msg = null;
            return "Error in Asset Area";
        }
    }

    public string this[string columnName]
    {
        get
        {
            string result = string.Empty;
            switch (columnName)
            {
                case "MeasurementUnit":
                    if (string.IsNullOrEmpty(MeasurementUnit))
                        result = "Name is required!";
                   
                    break;
            };
            return result;
        }
    }


}
}
