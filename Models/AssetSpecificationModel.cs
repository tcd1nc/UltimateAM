
namespace AssetManager.Models
{
    public class AssetSpecificationModel : BaseModel
    {
       
        int _measurementid;
        public int MeasurementUnitID
        {
            get { return _measurementid; }
            set { SetField(ref _measurementid, value); }
        }
        
            
    }
}
