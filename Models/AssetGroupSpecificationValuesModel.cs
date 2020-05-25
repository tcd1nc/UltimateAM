
namespace AssetManager.Models
{
   public class AssetGroupSpecificationValuesModel : BaseModel
    {
        int assetid;
        public int AssetID
        {
            get { return assetid; }
            set { SetField(ref assetid, value); }
        }

        int assetgroupid;
        public int AssetGroupID
        {
            get { return assetgroupid; }
            set { SetField(ref assetgroupid,value); }
        }

        int assetgroupspecvalueid;
        public int AssetGroupSpecificationValueID
        {
            get { return assetgroupspecvalueid; }
            set { SetField(ref assetgroupspecvalueid, value); }
        }

        int assetgroupspecificationid;
        public int AssetGroupSpecificationID
        {
            get { return assetgroupspecificationid; }
            set { SetField(ref assetgroupspecificationid, value); }
        }

        string specvalue;        
        public string SpecificationValue
        {
            get { return specvalue; }
            set { SetField(ref specvalue, value); }
        }

        string measurementunit;
        public string MeasurementUnit
        {
            get { return measurementunit; }
            set { SetField(ref measurementunit, value); }
        }

        string specificationoptions;
        public string SpecificationOptions
        {
            get { return specificationoptions; }
            set { SetField(ref specificationoptions, value); }
        }

        int _datatypeid;
        public int DataTypeID
        {
            get { return _datatypeid; }
            set { SetField(ref _datatypeid, value); }
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
