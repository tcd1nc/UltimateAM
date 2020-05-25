
namespace AssetManager.Models
{
    public class AssetSummaryModel : ViewModelBase
    {
        private int _ID;
        private string _Label;
        private string _Type;
        private int _ParentAssetID;
        private string _Description;
        private string _customer;
        private int _AssetGroupID;
        private int _AssetAreaID;
        private int labelid;
        public int ID { get { return _ID; } set { SetField(ref _ID, value); } }
        public string Label { get { return _Label; } set { SetField(ref _Label, value); } }
        public string Type { get { return _Type; } set { SetField(ref _Type, value); } }
        public int ParentAssetID { get { return _ParentAssetID; } set { SetField(ref _ParentAssetID, value); } }
        public string Description { get { return _Description; } set { SetField(ref _Description, value); } }
        public string Customer { get { return _customer; } set { SetField(ref _customer, value); } }
        public int LabelID { get { return labelid; } set { SetField(ref labelid, value); } }
        public int AssetGroupID { get { return _AssetGroupID; } set { SetField(ref _AssetGroupID, value); } }
        public int AssetAreaID { get { return _AssetAreaID; } set { SetField(ref _AssetAreaID, value); } }
    }
}
