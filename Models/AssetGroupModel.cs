
namespace AssetManager.Models
{
    public class AssetGroupModel : BaseModel
    {
        int _assetareaid;
        public int AssetAreaID
        {
            get { return _assetareaid; }
            set { SetField(ref _assetareaid, value); }
        }
                
        string _assetgroupidtext;
        public string AssetGroupIDText
        {
            get { return _assetgroupidtext; }
            set { SetField(ref _assetgroupidtext, value); }
        }

        bool _canbeparent;
        public bool CanBeParent
        {
            get { return _canbeparent; }
            set { SetField(ref _canbeparent, value); }
        }
    }
}
