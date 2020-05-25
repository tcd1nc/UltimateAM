using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
    public class MovementActivityTypeModel : ViewModelBase
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

    }
}
