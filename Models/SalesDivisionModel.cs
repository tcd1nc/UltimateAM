
namespace AssetManager.Models
{
    public class SalesDivisionModel : BaseModel
    {

        string _iconfilename;
        public string IconFileName
        {
            get { return _iconfilename; }
            set { SetField(ref _iconfilename, value); }
        }

        int _opcoid;
        public int OperatingCompanyID
        {
            get { return _opcoid; }
            set { SetField(ref _opcoid, value); }
        }
        
    }
}
