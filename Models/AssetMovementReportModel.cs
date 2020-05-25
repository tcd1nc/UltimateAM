using System;

namespace AssetManager.Models
{
    public class AssetMovementReportModel
    {
        public int ID
        {
            get; set;
        }

        public int SAPID
        {
            get; set;
        }


        public string Description
        {
            get; set;
        }

        public string AssetLabel
        {
            get; set;
        }

        public string Activity
        {
            get; set;
        }

        public string SourceAssetLabel
        {
            get; set;
        }

        public string DestinationAssetLabel
        {
            get; set;
        }

        public DateTime? DateMoved
        {
            get; set;
        }

        public string CustomerName
        {
            get; set;
        }


        public string SourceCustomer
        {
            get; set;
        }

        public string DestinationCustomer
        {
            get; set;
        }



    }
}
