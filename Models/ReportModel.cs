
namespace AssetManager.Models
{
   public class ReportModel
    {

        public int ID
        {
            get; set;
        }

        public string Header
        {
            get; set;
        }

        public string Parameter
        {
            get; set;
        }

        public string Tooltip
        {
            get; set;
        }

        public string IconfileName
        {
            get; set;
        }

        public bool HasDateFilter
        {
            get; set;
        }
    }
}
