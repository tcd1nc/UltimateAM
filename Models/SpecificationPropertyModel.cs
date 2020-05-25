
namespace AssetManager.Models
{
    public class SpecificationPropertyModel : BaseModel
    {
        string _propertyunit;
        public string PropertyUnit
        {
            get { return _propertyunit; }
            set { SetField(ref _propertyunit, value); }
        }
       
    }
}
