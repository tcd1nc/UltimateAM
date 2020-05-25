
namespace AssetManager.Models
{
    public class AssetTypeModel : BaseModel
    {
       
        int _assetgroupid;
        public int AssetGroupID
        {
            get { return _assetgroupid; }
            set { SetField(ref _assetgroupid, value); }
        }
               

    }
}
