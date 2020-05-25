
namespace AssetManager.Models
{
    public class CustomReportModel : BaseModel
    {
        string spname;
        public string SP
        {
            get { return spname; }
            set { SetField(ref spname, value); }
        }

        bool combinetables;
        public bool CombineTables
        {
            get { return combinetables; }
            set { SetField(ref combinetables, value); }
        }
    }
}
