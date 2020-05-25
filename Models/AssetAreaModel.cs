
namespace AssetManager.Models
{
    public class AssetAreaModel : BaseModel
    {
        string _prefix;
        public string Prefix
        {
            get { return _prefix; }
            set { SetField(ref _prefix, value); }
        }
    
    }
}
