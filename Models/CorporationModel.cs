
namespace AssetManager.Models
{
    public class CorporationModel : BaseModel
    {
        int logoid;
        public int LogoID
        {
            get { return logoid; }
            set { SetField(ref logoid, value); }
        }

        byte[] logo;
        public byte[] Logo
        {
            get { return logo; }
            set { SetField(ref logo, value); }
        }

        string logofilename;
        public string LogoFileName
        {
            get { return logofilename; }
            set { SetField(ref logofilename, value); }
        }


    }
}
