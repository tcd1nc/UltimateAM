using System;

namespace AssetManager.Models
{
    public class AssetMovementModel : ViewModelBase
    {
        public int ID
        {
            get; set;
        }

        public int AssetID
        {
            get; set;
        }
       
        public DateTime? DateMoved
        {
            get; set;
        }

        public int ActivityCodeID
        {
            get; set;
        }

        public int SourceCustomerID
        {
            get; set;
        }

        public int DestinationCustomerID
        {
            get; set;
        }       

    }
}
