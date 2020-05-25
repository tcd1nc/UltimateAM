
namespace AssetManager.Models
{
    public class CustomerModel : BaseModel
    {
              
        string _location;
        public string Location {
            get { return _location; }
            set { SetField(ref _location, value); }
        }
             
        string _customernumber;
        public string CustomerNumber {
            get { return _customernumber; }
            set { SetField(ref _customernumber, value); }
        }

        int _corpid;
        public int CorporationID
        {
            get { return _corpid; }
            set { SetField(ref _corpid, value); }
        }

        int _countryid;
        public int CountryID
        {
            get { return _countryid; }
            set { SetField(ref _countryid, value); }
        }

        bool _default;
        public bool Default
        {
            get { return _default; }
            set { SetField(ref _default, value); }
        }

        string _countryname;
        public string CountryName
        {
            get { return _countryname; }
            set { SetField(ref _countryname, value); }
        }


        byte[] logo;
        public byte[] Logo
        {
            get { return logo; }
            set { SetField(ref logo, value); }
        }

        string iconfilename;
        public string IconFileName
        {
            get { return iconfilename; }
            set { SetField(ref iconfilename, value); }
        }
    }
}
