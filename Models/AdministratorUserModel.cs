
namespace AssetManager.Models
{
   public class AdministratorUserModel : BaseModel
    {
       
        string loginname;
        public string LoginName
        {
            get { return loginname; }
            set { SetField(ref loginname, value); }
        }
        
    }
}
