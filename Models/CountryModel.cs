
namespace AssetManager.Models
{
    public class CountryModel : BaseModel
    {
        int _opcoid;
        public int OperatingCompanyID
        {
            get { return _opcoid; }
            set { SetField(ref _opcoid, value); }
        }
   
    }

}
