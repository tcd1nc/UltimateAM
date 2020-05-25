using System;

namespace AssetManager.Models
{
    public class ConsumableModel : BaseModel
    {
       
        int _assetid;
        public int AssetID
        {
            get { return _assetid; }
            set { SetField(ref _assetid, value); }
        }
                
        decimal _cost;
        public decimal Cost
        {
            get { return _cost; }
            set { SetField(ref _cost, value); }
        }

        DateTime? _datepurchased;
        public DateTime? DatePurchased
        {
            get { return _datepurchased; }
            set { SetField(ref _datepurchased, value); }
        }

        string _orderreference;
        public string OrderReference
        {
            get { return _orderreference; }
            set { SetField(ref _orderreference, value); }
        }

        string _supplier;
        public string Supplier
        {
            get { return _supplier; }
            set { SetField(ref _supplier, value); }
        }

        bool _used;
        public bool Used
        {
            get { return _used; }
            set { SetField(ref _used, value); }
        }

        int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { SetField(ref _quantity, value); }
        }

    }
}
