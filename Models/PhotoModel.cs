

namespace AssetManager.Models
{
    public class PhotoModel : BaseModel
    {
        int _assetid;
        string _photofilename;
      
        public int AssetID
        {
            get { return _assetid; }
            set { SetField(ref _assetid, value); }
        }

        public string PhotoFileName
        {
            get { return _photofilename; }
            set { SetField(ref _photofilename, value); }
        }

        byte[] photo;
        public byte[] Photo
        {
            get { return photo; }
            set { SetField(ref photo, value); }
        }
    }
}
