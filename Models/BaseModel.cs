using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class BaseModel : ViewModelBase
    {
        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        bool selected;
        public bool Selected
        {
            get { return selected; }
            set { SetField(ref selected, value); }
        }

        bool deletable;
        public bool IsDeletable
        {
            get { return deletable; }
            set { SetField(ref deletable, value); }
        }

        bool isdirty = false;
        public bool IsDirty
        {
            get { return isdirty; }
            set { SetField(ref isdirty, value); }
        }

        bool isenabled = false;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }
    }
}
