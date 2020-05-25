
namespace AssetManager.Models
{
    public class SearchFieldModel : ViewModelBase
    {

        public int ID
        {
            get; set;
        }

        public string TableName
        {
            get; set;
        }

        public string FieldName
        {
            get; set;
        }

        public string Label
        {
            get; set;
        }

        public string Mask
        {
            get; set;
        }
    }
}
