
namespace AssetManager.Models
{
    public class DefaultSettingsModel : ViewModelBase
    {
        public int MaxPhotoSize {get;set;}
        public int TargetPhotoQuality { get; set; }
        public int PhotoHeightAndWidth { get; set; }
        string photosfolder;
        public string PhotosFileLocation { get { return photosfolder; }  set { SetField(ref photosfolder, value); } }
        public string Delimiter { get; set; }
    }
}
