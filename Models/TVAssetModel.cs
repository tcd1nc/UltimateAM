
namespace AssetManager.Models
{
    public class TVAssetModel : BaseModel
    {
        int parentid;
        public int ParentID
        {
            get { return parentid; }
            set { SetField(ref parentid, value); }
        }

        string label;
        public string Label
        {
            get { return label; }
            set { SetField(ref label, value); }
        }

        bool isexpanded;
        public bool IsExpanded
        {
            get { return isexpanded; }
            set { SetField(ref isexpanded, value); }
        }

        NodeType nodetype;
        public NodeType NodeType
        {
            get { return nodetype; }
            set { SetField(ref nodetype, value); }
        }


        NodeType parenttype;
        public NodeType ParentType
        {
            get { return parenttype; }
            set { SetField(ref parenttype, value); }
        }


    }
}
