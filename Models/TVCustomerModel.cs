
namespace AssetManager.Models
{
    public class TVCustomerModel : BaseModel
    {
        byte[] logo;
        public byte[] Logo
        {
            get { return logo; }
            set { SetField(ref logo, value); }
        }

        string iconfile;
        public string IconFile
        {
            get { return iconfile; }
            set { SetField(ref iconfile, value); }
        }

    }
}
