using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Models
{
   public class AvailableAssetModel : ViewModelBase
    {
        private int _ID;
        private string _Label;
        private string _Type;
        private int _ParentAssetID;
        private string _Description;
        private string _customer;

        public int ID { get { return _ID; } set { SetField(ref _ID, value); } }
        public string Label { get { return _Label; } set { SetField(ref _Label, value); } }
        public string Type { get { return _Type; } set { SetField(ref _Type, value); } }
        public int ParentAssetID { get { return _ParentAssetID; } set { SetField(ref _ParentAssetID, value); } }
        public string Description { get { return _Description; } set { SetField(ref _Description, value); } }
        public string Customer { get { return _customer; } set { SetField(ref _customer, value); } }
    }
}
