using System;
using System.Windows;

namespace AssetManager.Models
{
    public class AssetModel : BaseModel
    {        
        
        private string _Category;
        private string _ApplicationType;
        private string _SiteLocation;
        private string _ManufacturerName;
        private string _ModelNo;
        private string _SerialNo;
        private DateTime? _DateInstalled;
        private decimal _PurchasePrice;
        private string _Comments;
        private string _ChemicalsUsed;
        private DateTime? _DatePurchased;
        private string _PONumber;
        private int _ParentAssetID;
        private DateTime? _NextAuditDate;
        private string _Dimensions;
        private string _SupplierName;
        private bool _Deleted;
        private int _AssetTypeID;
        private int _AssetGroupID;
        private int _AssetAreaID;
        private int labelid;
        private int _StatusID;
        private int _SalesDivisionID;
        private int _CustomerID;
        private string _SAPID;
        private int quantity;
        private string customer;
        private string location;
        private string label;
        private bool consumable;
        private DateTime dateadded;
        private bool temporary;
        public string Category { get { return _Category; } set { SetField(ref _Category, value); } }
        public string ApplicationType { get { return _ApplicationType; } set { SetField(ref _ApplicationType, value);} }
        public string SiteLocation { get { return _SiteLocation; } set { SetField(ref _SiteLocation, value); } }
        public string ManufacturerName { get { return _ManufacturerName; } set { SetField(ref _ManufacturerName, value);} }
        public string ModelNo { get { return _ModelNo; } set { SetField(ref _ModelNo, value); } }
        public string SerialNo { get { return _SerialNo; } set { SetField(ref _SerialNo, value); } }
        public DateTime? DateInstalled { get { return _DateInstalled; } set { SetField(ref _DateInstalled, value); } }
        public decimal PurchasePrice { get { return _PurchasePrice; } set { SetField(ref _PurchasePrice, value); } }
        public string Comments { get { return _Comments; } set { SetField(ref _Comments, value); } }
        public string ChemicalsUsed { get { return _ChemicalsUsed; } set { SetField(ref _ChemicalsUsed, value); } }         
        public DateTime? DatePurchased { get { return _DatePurchased; } set { SetField(ref _DatePurchased, value); } }
        public string PONumber { get { return _PONumber; } set { SetField(ref _PONumber, value); } }
        public int AssetTypeID { get { return _AssetTypeID; } set { SetField(ref _AssetTypeID, value); } }
        public int LabelID { get { return labelid; } set { SetField(ref labelid, value); } }
        public int AssetGroupID { get { return _AssetGroupID; } set { SetField(ref _AssetGroupID, value); } }
        public int AssetAreaID { get { return _AssetAreaID; } set { SetField(ref _AssetAreaID, value); } }
        public int StatusID { get { return _StatusID; } set { SetField(ref _StatusID, value); } }
        public int SalesDivisionID { get { return _SalesDivisionID; } set { SetField(ref _SalesDivisionID, value); } }
        public int CustomerID { get { return _CustomerID; } set { SetField(ref _CustomerID, value); } }
        public int ParentAssetID { get { return _ParentAssetID; } set { SetField(ref _ParentAssetID, value); } }       
        public DateTime? NextAuditDate { get { return _NextAuditDate; } set { SetField(ref _NextAuditDate, value); } }
        public string Dimensions { get { return _Dimensions; } set { SetField(ref _Dimensions, value); } }
        public string SupplierName { get { return _SupplierName; } set { SetField(ref _SupplierName, value); } }
        public bool Deleted { get { return _Deleted; } set { SetField(ref _Deleted, value); } }
        public string SAPID { get { return _SAPID; } set { SetField(ref _SAPID, value); } }
        public int Quantity { get { return quantity; } set { SetField(ref quantity, value); } }
        public string Customer { get { return customer; } set { SetField(ref customer, value); } }
        public string Location { get { return location; } set { SetField(ref location, value); } }
        public string Label { get { return label; } set { SetField(ref label, value); } }
        public bool Consumable { get { return consumable; } set { SetField(ref consumable, value); } }
        public DateTime DateAdded { get { return dateadded; } set { SetField(ref dateadded, value); } }
        public bool Temporary { get { return temporary; } set { SetField(ref temporary, value); } }

    }

}
