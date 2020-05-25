
namespace AssetManager.Models
{
    public class AssetGroupSpecificationModel : BaseModel
    {
               
        int specificationnameid;
        public int SpecificationNameID
        {
            get { return specificationnameid; }
            set { SetField(ref specificationnameid, value); }
        }

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
            set { SetField(ref assetgroupid, value); }
        }

        int propertyunitid;
        public int PropertyUnitID
        {
            get { return propertyunitid; }
            set { SetField(ref propertyunitid, value); }
        }

        int measurementid;
        public int MeasurementUnitID
        {
            get { return measurementid; }
            set { SetField(ref measurementid, value); }
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

        string specificationname;
        public string SpecificationName
        {
            get { return specificationname; }
            set { SetField(ref specificationname, value); }
        }
        
        string specificationpropertyname;
        public string SpecificationPropertyName
        {
            get { return specificationpropertyname; }
            set { SetField(ref specificationpropertyname, value); }
        }

        string specificationvalue;
        public string SpecificationValue
        {
            get { return specificationvalue; }
            set { SetField(ref specificationvalue, value); }
        }

        public int AssetGroupSpecificationID
        {
            get; set;
        }

    }
}
