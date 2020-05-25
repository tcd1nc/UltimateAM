
namespace AssetManager.Models
{
    public class CustomReportParametersModel : ViewModelBase
    {
        int id;
        public int ID {
            get { return id; }
            set { SetField(ref id, value); }
        }

        string parametertype = string.Empty;
        public string ParameterType {
            get { return parametertype; }
            set { SetField(ref parametertype, value); }
        }

        string name = string.Empty;
        public string Name {
            get { return name; }
            set { SetField(ref name, value); }
        }

        string defaultvalue = string.Empty;
        public string DefaultValue {
            get { return defaultvalue; }
            set { SetField(ref defaultvalue, value); }
        }

        string tooltip = string.Empty;
        public string ToolTip {
            get { return tooltip; }
            set { SetField(ref tooltip, value); }
        }

        int customreportid;
        public int CustomReportID {
            get { return customreportid; }
            set { SetField(ref customreportid, value); }    
        }

        string value1 = string.Empty;
        public string Value {
            get { return value1; }
            set { SetField(ref value1, value); }
        }

        string displayname = string.Empty;
        public string DisplayName {
            get { return displayname; }
            set { SetField(ref displayname, value); }
        }

        int defaultvaluecalcid;
        public int DefaultValueCalcID
        {
            get { return defaultvaluecalcid; }
            set { SetField(ref defaultvaluecalcid, value); }
        }
    }
}
