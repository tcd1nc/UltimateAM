
namespace AssetManager.Models
{
    public  class AssetGroupSpecDisplayDataModel : BaseModel
    {
        int _assetid;
        public int AssetID
        {
            get { return _assetid; }
            set { SetField(ref _assetid, value); }
        }

        int _assetgroupid;
        public int AssetGroupID
        {
            get { return _assetgroupid; }
            set { SetField(ref _assetgroupid, value); }
        }

        public int AssetGroupSpecificationID
        {
            get; set;
        }

        string _specificationvalue;
        public string SpecificationValue
        {
            get { return _specificationvalue; }
            set { SetField(ref _specificationvalue, value); }
        }

        int _datatypeid;
        public int DataTypeID
        {
            get { return _datatypeid; }
            set { SetField(ref _datatypeid, value); }
        }

        string _specificationoptions;
        public string SpecificationOptions
        {
            get { return _specificationoptions; }
            set { SetField(ref _specificationoptions, value); }
        }

        string _specificationname;
        public string SpecificationName
        {
            get { return _specificationname; }
            set { SetField(ref _specificationname, value); }
        }

        string _specificationpropertyname;
        public string SpecificationPropertyName
        {
            get { return _specificationpropertyname; }
            set { SetField(ref _specificationpropertyname, value); }
        }

    }
}
