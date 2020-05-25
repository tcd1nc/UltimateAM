using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using AssetManager.Models;
using System.Runtime.CompilerServices;
using static AssetManager.GlobalClass;
using System.Data.SQLite;
using System.IO;

namespace AssetManager
{
    static class SQLiteQueries
    {
        static int _namelength = 50;
        static int _descriptionlength = 50;
        static int _prefixlength = 3;
        static int _filenamelength = 255;
        static int descrlen = 255;

     
        #region Get Queries

        public static Dictionary<int, string> GetAssetLabels()
        {
            Dictionary<int, string> _assets = new Dictionary<int, string>();
            try
            {
                string sql = "SELECT ID, Label FROM Assets WHERE Label Is Not Null";
                using (  SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())                           
                        while (or.Read())
                        {
                            _assets.Add(Convert.ToInt32(or["ID"]), or["Label"].ToString() ?? string.Empty);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }

        public static AssetModel GetAsset(int assetid)
        {
            AssetModel asset = new AssetModel();
            try
            {
                string sql = "SELECT Assets.ID, Assets.Name, Assets.ApplicationType, Assets.SiteLocation, Assets.ManufacturerName, Assets.ModelNo, Assets.SerialNo, "
                    + " Assets.DateInstalled, Assets.PurchasePrice, Assets.Comments, Assets.ChemicalsUsed, Assets.DatePurchased, Assets.PONumber,"
                    + " Assets.TypeID, Assets.CustomerID, Assets.SalesDivisionID, Assets.StatusID, Assets.ParentAssetID, Assets.GroupID, Assets.PrefixID,"
                    + " Assets.Dimensions, Assets.SupplierName, Customers.Name AS Customer, Customers.Location, Assets.LabelID, "
                    + " (SELECT MIN(AuditDate) AS NextAuditDate FROM AuditDates "
                    + " WHERE AuditDates.AssetID = Assets.ID AND AuditDates.AuditDate >= date('now')) AS NextAuditDate, AssetTypes.Name AS Category, Assets.BAANID "
                    + " FROM Assets LEFT JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID "
                    + " LEFT JOIN Customers ON Customers.ID = Assets.CustomerID"
                    + " WHERE Assets.ID = @assetid ";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@assetid", assetid);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                asset.ID = assetid;
                                asset.Name = or["Name"].ToString() ?? string.Empty;
                                asset.Category = or["Category"].ToString() ?? string.Empty;
                                asset.ApplicationType = or["ApplicationType"].ToString() ?? string.Empty;
                                asset.SiteLocation = or["SiteLocation"].ToString() ?? string.Empty;
                                asset.ManufacturerName = or["ManufacturerName"].ToString() ?? string.Empty;
                                asset.ModelNo = or["ModelNo"].ToString() ?? string.Empty;
                                asset.SerialNo = or["SerialNo"].ToString() ?? string.Empty;
                                asset.DateInstalled = ConvertDefaultDateToNull(or["DateInstalled"]);
                                asset.PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]);
                                asset.Comments = or["Comments"].ToString() ?? string.Empty;
                                asset.ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty;
                                asset.DatePurchased = ConvertDefaultDateToNull(or["DatePurchased"]);
                                asset.PONumber = or["PONumber"].ToString() ?? string.Empty;
                                asset.AssetTypeID = ConvertObjToInt(or["TypeID"]);
                                asset.AssetGroupID = ConvertObjToInt(or["GroupID"]);
                                asset.LabelID = ConvertObjToInt(or["LabelID"]);
                                asset.CustomerID = ConvertObjToInt(or["CustomerID"]);
                                asset.SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]);
                                asset.AssetAreaID = ConvertObjToInt(or["PrefixID"]);
                                asset.StatusID = ConvertObjToInt(or["StatusID"]);
                                asset.ParentAssetID = ConvertObjToInt(or["ParentAssetID"]);
                                asset.NextAuditDate = ConvertDefaultDateToNull(or["NextAuditDate"]);
                                asset.Dimensions = or["Dimensions"].ToString() ?? string.Empty;
                                asset.SupplierName = or["SupplierName"].ToString() ?? string.Empty;
                                asset.SAPID = or["BAANID"].ToString() ?? string.Empty;
                                asset.Customer = or["Customer"].ToString() ?? string.Empty;
                                asset.Location = or["Location"].ToString() ?? string.Empty;
                                asset.LabelID = ConvertObjToInt(or["LabelID"]);
                                asset.Temporary = false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return asset;
        }

        public static FullyObservableCollection<AssetModel> GetChildAssets(int parentassetid)
        {
            FullyObservableCollection<AssetModel> assets = new FullyObservableCollection<AssetModel>();
            AssetModel asset = null;
            try
            {
                string sql = "SELECT Assets.ID, Assets.Name, Assets.LabelID, Assets.CustomerID, Assets.ApplicationType, Assets.ManufacturerName, Assets.ModelNo, Assets.SerialNo, Assets.DateInstalled, Assets.PurchasePrice, Assets.Comments, Assets.ChemicalsUsed, Assets.DatePurchased, Assets.PONumber, Assets.TypeID, Assets.GroupID, Assets.SalesDivisionID, Assets.PrefixID, Assets.StatusID, Assets.Dimensions, Assets.SupplierName, "
                    + " (SELECT MIN(AuditDate) AS NextAuditDate FROM AuditDates "
                    + " WHERE AuditDates.AssetID = Assets.ID AND AuditDates.AuditDate >= date('now')) AS NextAuditDate, Assets.SiteLocation, AssetTypes.Name AS Category, Assets.BAANID "
                    + " FROM Assets LEFT JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID "
                    + " WHERE Assets.ParentAssetID = @parentid AND Assets.Deleted = 0 AND Assets.Consumable = 0"
                    + " ORDER BY Assets.Label";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@parentid", parentassetid);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                            while (or.Read())
                        {
                            asset = new AssetModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Category = or["Category"].ToString() ?? string.Empty,
                                ApplicationType = or["ApplicationType"].ToString() ?? string.Empty,
                                SiteLocation = or["SiteLocation"].ToString() ?? string.Empty,
                                ManufacturerName = or["ManufacturerName"].ToString() ?? string.Empty,
                                ModelNo = or["ModelNo"].ToString() ?? string.Empty,
                                SerialNo = or["SerialNo"].ToString() ?? string.Empty,
                                DateInstalled = ConvertDefaultDateToNull(or["DateInstalled"]),
                                PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]),
                                Comments = or["Comments"].ToString() ?? string.Empty,
                                ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty,
                                Label = MakeLabel(ConvertObjToInt(or["PrefixID"]), ConvertObjToInt(or["GroupID"]), ConvertObjToInt(or["LabelID"])),
                                DatePurchased = ConvertDefaultDateToNull(or["DatePurchased"]),
                                PONumber = or["PONumber"].ToString() ?? string.Empty,
                                AssetTypeID = ConvertObjToInt(or["TypeID"]),
                                AssetGroupID = ConvertObjToInt(or["GroupID"]),
                                LabelID = ConvertObjToInt(or["LabelID"]),
                                CustomerID = ConvertObjToInt(or["CustomerID"]),
                                SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]),
                                AssetAreaID = ConvertObjToInt(or["PrefixID"]),
                                StatusID = ConvertObjToInt(or["StatusID"]),
                                ParentAssetID = parentassetid,
                                NextAuditDate = ConvertDefaultDateToNull(or["NextAuditDate"]),
                                Dimensions = or["Dimensions"].ToString() ?? string.Empty,
                                SupplierName = or["SupplierName"].ToString() ?? string.Empty,
                                SAPID = or["BAANID"].ToString() ?? string.Empty
                            };
                            assets.Add(asset);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assets;
        }

        public static FullyObservableCollection<AssetModel> GetDeletedChildAssets(int parentassetid)
        {
            FullyObservableCollection<AssetModel> assets = new FullyObservableCollection<AssetModel>();
            try
            {
                string sql = "SELECT Assets.ID, AssetTypes.Name, Assets.LabelID, Assets.CustomerID, Assets.PrefixID, Assets.GroupID "
                    + " FROM Assets LEFT JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID "
                    + " WHERE Assets.ParentAssetID = @parentid AND Assets.Deleted = 1 "
                    ;

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@parentid", parentassetid);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                            while (or.Read())
                            {
                                assets.Add(new AssetModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    AssetAreaID = ConvertObjToInt(or["PrefixID"]),
                                    AssetGroupID = ConvertObjToInt(or["GroupID"]),
                                    LabelID = ConvertObjToInt(or["LabelID"]),
                                    CustomerID = ConvertObjToInt(or["CustomerID"]),
                                    ParentAssetID = parentassetid
                                });
                            }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assets;
        }

        public static FullyObservableCollection<AssetModel> GetCustomerChildAssets(int customerid)
        {
            FullyObservableCollection<AssetModel> assets = new FullyObservableCollection<AssetModel>();
            AssetModel asset = null;
            try
            {
                string sql = "SELECT Assets.ID, Assets.Name, LabelID, Assets.CustomerID, Assets.ParentAssetID, Assets.ApplicationType, Assets.ManufacturerName, Assets.ModelNo, Assets.SerialNo, Assets.DateInstalled, Assets.PurchasePrice, Assets.Comments, Assets.ChemicalsUsed, Assets.DatePurchased, Assets.PONumber, Assets.TypeID, Assets.SalesDivisionID, Assets.StatusID, Assets.GroupID, Assets.PrefixID, Assets.Dimensions, Assets.SupplierName, "
                    + " (SELECT MIN(AuditDate) AS NextAuditDate "
                    + " FROM AuditDates WHERE AuditDates.AssetID = Assets.ID AND AuditDates.AuditDate >= date('now')) AS NextAuditDate, Assets.SiteLocation, AssetTypes.Name AS Category, Assets.BAANID "
                    + " FROM Assets LEFT JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID "
                    + " WHERE Assets.CustomerID = @customerid AND (Assets.ParentAssetID < 1 Or Assets.ParentAssetID Is Null) AND (Assets.Deleted = 0  AND Assets.Consumable = 0) "
                    ;

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@customerid", customerid);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                            while (or.Read())
                        {
                            asset = new AssetModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                Category = or["Category"].ToString() ?? string.Empty,
                                ApplicationType = or["ApplicationType"].ToString() ?? string.Empty,
                                SiteLocation = or["SiteLocation"].ToString() ?? string.Empty,
                                ManufacturerName = or["ManufacturerName"].ToString() ?? string.Empty,
                                ModelNo = or["ModelNo"].ToString() ?? string.Empty,
                                SerialNo = or["SerialNo"].ToString() ?? string.Empty,
                                DateInstalled = ConvertDefaultDateToNull(or["DateInstalled"]),
                                PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]),
                                Comments = or["Comments"].ToString() ?? string.Empty,
                                ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty,
                                Label = MakeLabel(ConvertObjToInt(or["PrefixID"]), ConvertObjToInt(or["GroupID"]), ConvertObjToInt(or["LabelID"])),
                                DatePurchased = ConvertDefaultDateToNull(or["DatePurchased"]),
                                PONumber = or["PONumber"].ToString() ?? string.Empty,
                                AssetTypeID = ConvertObjToInt(or["TypeID"]),
                                AssetGroupID = ConvertObjToInt(or["GroupID"]),
                                LabelID = ConvertObjToInt(or["LabelID"]),
                                CustomerID = customerid,
                                SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]),
                                AssetAreaID = ConvertObjToInt(or["PrefixID"]),
                                StatusID = ConvertObjToInt(or["StatusID"]),
                                ParentAssetID = ConvertObjToInt(or["ParentAssetID"]),
                                NextAuditDate = ConvertDefaultDateToNull(or["NextAuditDate"]),
                                Dimensions = or["Dimensions"].ToString() ?? string.Empty,
                                SupplierName = or["SupplierName"].ToString() ?? string.Empty,
                                SAPID = or["BAANID"].ToString() ?? string.Empty
                            };
                            assets.Add(asset);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assets;
        }

        public static string GetParentLabel(int parentassetid)
        {
            string label = string.Empty;
            try
            {
                string sql = "SELECT Label FROM Assets WHERE AssetID = @parentassetid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@parentassetid", parentassetid);
                        label = (string)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return label;
        }

        public static CustomerModel GetCustomer(int id)
        {
            CustomerModel customer = new CustomerModel();
            string sql = "SELECT CustomerNumber, Name, Location, CountryID, CorporationID, IconFile FROM Customers WHERE ID = @id";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {                        
                        oc.Parameters.Add("@id", DbType.Int32).Value = id;
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                customer.ID = id;
                                customer.CustomerNumber = or["CustomerNumber"].ToString() ?? string.Empty;
                                customer.Name = or["Name"].ToString() ?? string.Empty;
                                customer.Location = or["Location"].ToString() ?? string.Empty;
                                customer.CountryID = ConvertObjToInt(or["CountryID"]);
                                customer.CorporationID = ConvertObjToInt(or["CorporationID"]);
                                customer.IconFileName = or["IconFile"].ToString() ?? string.Empty;
                            }
                        }
                    }
                }
            }

            catch (Exception e)
            {
                ShowError(e);
            }
            return customer;
        }

        public static FullyObservableCollection<CustomerModel> GetCustomers()
        {
            FullyObservableCollection<CustomerModel> _customers = new FullyObservableCollection<CustomerModel>();
            try
            {
                string sql = "SELECT Customers.ID, Customers.CustomerNumber, Customers.Name, IconFile, Customers.CorporationID, Countries.ID AS CountryID, Countries.Name AS Country, Customers.Location, "
                    + " CASE WHEN T2.CustomerID is null THEN 1 "
		            + " ELSE"
                    + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                    + " END"
                    + " AS IsDeletable FROM Customers"
                    + " LEFT JOIN(select CustomerID, count(CustomerID) as CNT from Assets group by CustomerID) AS T2 ON T2.CustomerID = Customers.ID"
                    + " LEFT JOIN Countries ON Customers.CountryID = Countries.ID"
                    + " ORDER BY Countries.Name, Customers.Name";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _customers.Add(new CustomerModel
                                {
                                    ID = Convert.ToInt32(or["ID"]),
                                    CustomerNumber = or["CustomerNumber"].ToString() ?? string.Empty,
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Location = or["Location"].ToString() ?? string.Empty,
                                    CountryID = ConvertObjToInt(or["CountryID"]),
                                    CorporationID = ConvertObjToInt(or["CorporationID"]),
                                    CountryName = or["Country"].ToString() ?? string.Empty,
                                    IconFileName = or["IconFile"].ToString() ?? string.Empty,
                                    // Logo = (or["Logo"] == DBNull.Value) ? null : (byte[])or["Logo"],
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                    Selected = false,
                                    IsEnabled = true
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _customers;
        }

        public static FullyObservableCollection<TVCustomerModel> GetTVCustomers()
        {
            FullyObservableCollection<TVCustomerModel> customers = new FullyObservableCollection<TVCustomerModel>();
            try
            {
                string sql = "SELECT Customers.ID, Customers.Name AS Customer, Customers.IconFile "
               + " FROM Customers LEFT JOIN Countries ON Customers.CountryID = Countries.ID "
               + " ORDER BY Customers.Name, Countries.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                customers.Add(new TVCustomerModel
                                {
                                    ID = Convert.ToInt32(or["ID"]),
                                    Name = or["Customer"].ToString() ?? string.Empty,
                                    // Logo = (or["Logo"] == DBNull.Value) ? null : (byte[])or["Logo"],                                   
                                    IconFile = Path.Combine(Defaults.PhotosFileLocation, or["IconFile"].ToString() ?? string.Empty),
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return customers;
        }


        public static AdministratorUserModel GetAdministratorNameFromUserLogin(string loginname)
        {
            AdministratorUserModel administrator = new AdministratorUserModel();
            try
            {
                string sql = "SELECT ID, Name FROM Administrators WHERE UserLogin = @userlogin";
                                             
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@userlogin", loginname);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                administrator.ID = ConvertObjToInt(or["ID"]);
                                administrator.Name = or["Name"].ToString() ?? string.Empty;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return administrator;
        }

        public static FullyObservableCollection<CountryModel> GetCountries()
        {
            FullyObservableCollection<CountryModel> _countries = new FullyObservableCollection<CountryModel>();
            try
            {
                string sql = "SELECT Countries.ID, Countries.Name, Countries.OperatingCompanyID,"
                    + " CASE WHEN T2.CountryID is null THEN 1 "
		            + " ELSE"
                    + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                    + " END"
                    + " AS IsDeletable FROM Countries"
                    + " LEFT JOIN(select CountryID, count(CountryID) as CNT from Customers group by CountryID) AS T2 ON T2.CountryID = Countries.ID"
                    + " ORDER BY Countries.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _countries.Add(new CountryModel
                                {
                                    ID = Convert.ToInt32(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    OperatingCompanyID = ConvertObjToInt(or["OperatingCompanyID"]),
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _countries;
        }

        public static FullyObservableCollection<SalesDivisionModel> GetSalesDivisions()
        {
            FullyObservableCollection<SalesDivisionModel> _salesdivisions = new FullyObservableCollection<SalesDivisionModel>();
            try
            {
                string sql = "SELECT SalesDivisions.ID, SalesDivisions.Name," 
                            + " CASE WHEN T2.SalesDivisionID is null THEN 1"
		                    + " ELSE"
                            + "CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                            + " END"
                            + " AS IsDeletable FROM SalesDivisions"
                            + " LEFT JOIN(select SalesDivisionID, count(SalesDivisionID) as CNT from Assets group by SalesDivisionID) AS T2 ON T2.SalesDivisionID = SalesDivisions.ID"
                            + " ORDER BY SalesDivisions.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _salesdivisions.Add(new SalesDivisionModel
                                {
                                    ID = Convert.ToInt32(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _salesdivisions;
        }

        public static FullyObservableCollection<AssetAreaModel> GetAssetAreas()
        {
            FullyObservableCollection<AssetAreaModel> _assetareas = new FullyObservableCollection<AssetAreaModel>();
            try
            {
                string sql = "SELECT AssetAreas.ID, AssetAreas.Name, AssetAreas.Prefix,"
                        + " CASE WHEN T2.PrefixID is null THEN 1 "
		                + " ELSE"
                        + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                        + " END"
                        + " AS IsDeletable FROM AssetAreas"
                        + " LEFT JOIN(select PrefixID, count(PrefixID) as CNT from Assets WHERE Assets.Deleted = 0 group by PrefixID) AS T2 ON T2.PrefixID = AssetAreas.ID"
                        + " ORDER BY AssetAreas.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _assetareas.Add(new AssetAreaModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Prefix = or["Prefix"].ToString() ?? string.Empty,
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetareas;
        }

        public static FullyObservableCollection<AssetGroupModel> GetAssetGroups()
        {
            FullyObservableCollection<AssetGroupModel> _assetgroups = new FullyObservableCollection<AssetGroupModel>();
            try
            {
                string sql = "SELECT AssetGroups.ID, AssetGroups.Name, AssetGroups.AssetPrefixID, AssetGroups.AssetGroupIDText, AssetGroups.CanBeParent,"
                    + " CASE WHEN T2.GroupID is null THEN 1"
		            + " ELSE"
                    + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                    + " END"
                    + " AS IsDeletable FROM AssetGroups"
                    + " LEFT JOIN(select GroupID, count(GroupID) as CNT from Assets WHERE Assets.Deleted = 0 group by GroupID) AS T2 ON T2.GroupID = AssetGroups.ID"
                    + " ORDER BY AssetGroups.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _assetgroups.Add(new AssetGroupModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    AssetAreaID = ConvertObjToInt(or["AssetPrefixID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    AssetGroupIDText = (string.IsNullOrEmpty(or["AssetGroupIDText"].ToString()) ? "00" : or["AssetGroupIDText"].ToString()),
                                    CanBeParent = ConvertObjIntToBool(or["CanBeParent"]),
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetgroups;
        }

        public static FullyObservableCollection<AssetTypeModel> GetAssetTypes()
        {
            FullyObservableCollection<AssetTypeModel> assettypes = new FullyObservableCollection<AssetTypeModel>();
            try
            {
                string sql = "SELECT AssetTypes.ID, AssetTypes.Name, AssetTypes.AssetGroupID,"
                        + " CASE WHEN T2.TypeID is null THEN 1"
		                + " ELSE"
                        + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                        + " END"
                        + " AS IsDeletable FROM AssetTypes"
                        + " LEFT JOIN(select TypeID, count(TypeID) as CNT from Assets WHERE Assets.Deleted = 0 group by TypeID) AS T2 ON T2.TypeID = AssetTypes.ID"
                        + " ORDER BY AssetTypes.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                assettypes.Add(new AssetTypeModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    AssetGroupID = ConvertObjToInt(or["AssetGroupID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assettypes;
        }

        public static FullyObservableCollection<AssetGroupSpecificationValuesModel> GetAssetGroupSpecificationValues(int assetid, int assetgroupid)
        {
            FullyObservableCollection<AssetGroupSpecificationValuesModel> _assetspecifications = new FullyObservableCollection<AssetGroupSpecificationValuesModel>();
            try
            {
                string sql = "SELECT Specifications.Name, SpecificationPropertyUnits.PropertyUnit, AssetGroupSpecifications.SpecificationOptions, Specifications.MeasurementUnitID, "
                    + " AGSV.SpecificationValue, IFNULL(AGSV.ID, 0) AS AGSVID, AGSV.AssetGroupSpecificationID "
                    + " FROM AssetGroupSpecifications "
                    + " LEFT JOIN Specifications ON AssetGroupSpecifications.SpecificationNameID = Specifications.ID"
                    + " LEFT JOIN SpecificationPropertyUnits ON AssetGroupSpecifications.PropertyUnitID = SpecificationPropertyUnits.ID"
                    + " LEFT JOIN(SELECT* FROM AssetGroupSpecificationValues WHERE AssetID= @assetid) AGSV ON AGSV.AssetGroupID = AssetGroupSpecifications.AssetGroupID AND AGSV.AssetGroupSpecificationID = AssetGroupSpecifications.ID"
                    + " WHERE AssetGroupSpecifications.AssetGroupID = @groupid"
                    + " ORDER BY Specifications.Name, SpecificationPropertyUnits.PropertyUnit";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@assetid", assetid);
                        oc.Parameters.AddWithValue("@groupid", assetgroupid);
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _assetspecifications.Add(new AssetGroupSpecificationValuesModel
                                {
                                    AssetID = assetid,
                                    AssetGroupID = assetgroupid,
                                    //ID = ConvertObjToInt(or["ID"]),
                                    AssetGroupSpecificationValueID = ConvertObjToInt(or["AGSVID"]),
                                    SpecificationName = or["Name"].ToString() ?? string.Empty,
                                    SpecificationPropertyName = or["PropertyUnit"].ToString() ?? string.Empty,
                                    SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty,
                                    AssetGroupSpecificationID = ConvertObjToInt(or["AssetGroupSpecificationID"]),
                                    SpecificationValue = or["SpecificationValue"].ToString() ?? string.Empty,
                                    DataTypeID = ConvertObjToInt(or["MeasurementUnitID"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetspecifications;
        }

        public static FullyObservableCollection<AssetSpecificationModel> GetSpecifications()
        {
            FullyObservableCollection<AssetSpecificationModel> _assetspecifications = new FullyObservableCollection<AssetSpecificationModel>();
            try
            {
                string sql = "SELECT Specifications.ID, Specifications.Name, Specifications.MeasurementUnitID,"
                        + " CASE WHEN T2.SpecificationNameID is null THEN 1"
                        + " ELSE"
                        + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                        + " END"
                        + " AS IsDeletable FROM Specifications"
                        + " LEFT JOIN(select SpecificationNameID, count(SpecificationNameID) as CNT from AssetGroupSpecifications group by SpecificationNameID) AS T2 ON T2.SpecificationNameID = Specifications.ID"
                        + " ORDER BY Specifications.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _assetspecifications.Add(new AssetSpecificationModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    MeasurementUnitID = ConvertObjToInt(or["MeasurementUnitID"]),
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetspecifications;
        }

        public static FullyObservableCollection<MaintenanceRecordModel> GetMaintenanceRecords(int assetid)
        {
            FullyObservableCollection<MaintenanceRecordModel> _maintenancerecords = new FullyObservableCollection<MaintenanceRecordModel>();
            try
            {
                string sql = "SELECT MaintenanceRecords.ID, MaintenanceRecords.MaintenanceDate, MaintenanceRecords.Description, MaintenanceRecords.Cost, MaintenanceRecords.MaintainedBy, MaintenanceRecords.AssetID, MaintenanceRecords.Completed"
                            + " FROM MaintenanceRecords"
                            + " WHERE MaintenanceRecords.AssetID = @assetid AND MaintenanceRecords.Completed = 1"
                            + " ORDER BY MaintenanceRecords.MaintenanceDate DESC";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@assetid", assetid);
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _maintenancerecords.Add(new MaintenanceRecordModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    MaintenanceDate = ConvertDefaultDateToNull(or["MaintenanceDate"]),
                                    Name = or["Description"].ToString() ?? string.Empty,
                                    Cost = ConvertObjToDecimal(or["Cost"]),
                                    Completed = ConvertObjIntToBool(or["Completed"]),
                                    MaintainedBy = or["MaintainedBy"].ToString() ?? string.Empty,
                                    AssetID = assetid
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _maintenancerecords;
        }

        public static FullyObservableCollection<MaintenanceRecordModel> GetScheduledMaintenance(int assetid)
        {
            FullyObservableCollection<MaintenanceRecordModel> _scheduledmaintenance = new FullyObservableCollection<MaintenanceRecordModel>();
            try
            {
                string sql = "SELECT DISTINCT MaintenanceRecords.ID, MaintenanceRecords.Description, MaintenanceRecords.MaintenanceDate, MaintenanceRecords.ScheduledMaintenanceDate"
                            + " FROM MaintenanceRecords"
                            + " WHERE MaintenanceRecords.AssetID = @assetid AND MaintenanceRecords.Completed = 0"
                            + " ORDER BY MaintenanceRecords.MaintenanceDate";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@assetid", assetid);
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _scheduledmaintenance.Add(new MaintenanceRecordModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    ScheduledMaintenanceDate = ConvertDefaultDateToNull(or["ScheduledMaintenanceDate"]),
                                    Name = or["Description"].ToString() ?? string.Empty,
                                    AssetID = assetid
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _scheduledmaintenance;
        }

        public static FullyObservableCollection<MaintenanceRecordModel> GetAllScheduledMaintenance()
        {
            FullyObservableCollection<MaintenanceRecordModel> _scheduledmaintenance = new FullyObservableCollection<MaintenanceRecordModel>();
            try
            {
                string sql = "SELECT MaintenanceRecords.ID, MaintenanceRecords.AssetID, MaintenanceRecords.Description, MaintenanceRecords.ScheduledMaintenanceDate, Assets.Label, Customers.Name AS Customer, "
                            + " MaintenanceRecords.Completed, Assets.LabelID, Assets.GroupID, Assets.PrefixID as AreaID"
                            + " FROM Assets"
                            + " INNER JOIN Customers ON Assets.CustomerID = Customers.ID"
                            + " INNER JOIN MaintenanceRecords ON Assets.ID = MaintenanceRecords.AssetID"
                            + " WHERE MaintenanceRecords.Completed = 0"
                            + " ORDER BY MaintenanceRecords.ScheduledMaintenanceDate";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _scheduledmaintenance.Add(new MaintenanceRecordModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    AssetID = ConvertObjToInt(or["AssetID"]),
                                    Name = or["Description"].ToString() ?? string.Empty,
                                    ScheduledMaintenanceDate = ConvertObjToDate(or["ScheduledMaintenanceDate"]),
                                    CustomerName = or["Customer"].ToString() ?? string.Empty,
                                    AreaID = ConvertObjToInt(or["AreaID"]),
                                    GroupID = ConvertObjToInt(or["GroupID"]),
                                    LabelID = ConvertObjToInt(or["LabelID"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _scheduledmaintenance;
        }

        public static FullyObservableCollection<MaintenanceRecordModel> GetAllScheduledAssetAudits()
        {
            FullyObservableCollection<MaintenanceRecordModel> _scheduledaudits = new FullyObservableCollection<MaintenanceRecordModel>();
            try
            {
                string sql = "SELECT AuditDates.ID, AuditDates.AssetID, AuditDates.AuditDate, Assets.Label, Customers.Name AS Customer"
                           + " FROM AuditDates"
                           + " INNER JOIN Assets ON AuditDates.AssetID = Assets.ID"
                           + " INNER JOIN Customers ON Assets.CustomerID = Customers.ID"
                           + " ORDER BY AuditDates.AuditDate";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _scheduledaudits.Add(new MaintenanceRecordModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    AssetID = ConvertObjToInt(or["AssetID"]),
                                    ScheduledMaintenanceDate = ConvertObjToDate(or["AuditDate"]),
                                    CustomerName = or["Customer"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _scheduledaudits;
        }

        public static FullyObservableCollection<MaintenanceTypeModel> GetMaintenanceTypes()
        {
            FullyObservableCollection<MaintenanceTypeModel> _maintenancetypes = new FullyObservableCollection<MaintenanceTypeModel>();
            try
            {
                string sql = "";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _maintenancetypes.Add(new MaintenanceTypeModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Description"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _maintenancetypes;
        }

        public static FullyObservableCollection<OperatingCompanyModel> GetOperatingCompanies()
        {
            FullyObservableCollection<OperatingCompanyModel> _operatingcompanies = new FullyObservableCollection<OperatingCompanyModel>();
            try
            {
                string sql = "SELECT OperatingCompany.ID, OperatingCompany.Name, "
                        + " CASE WHEN T2.OperatingCompanyID is null THEN 1 "
		                + " ELSE"
                        + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                        + " END" 
                        + " AS IsDeletable FROM OperatingCompany"
                        + " LEFT JOIN(select OperatingCompanyID, count(OperatingCompanyID) as CNT from Countries group by OperatingCompanyID) AS T2 ON T2.OperatingCompanyID = OperatingCompany.ID"
                        + " ORDER BY OperatingCompany.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _operatingcompanies.Add(new OperatingCompanyModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    // Default = ConvertObjToBool(or["Default"])
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                });

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _operatingcompanies;
        }

        public static FullyObservableCollection<CorporationModel> GetCorporations()
        {
            FullyObservableCollection<CorporationModel> _corporations = new FullyObservableCollection<CorporationModel>();
            try
            {
                string sql = "SELECT Corporations.ID, Corporations.Name, LogoFileName, "
                    + " CASE WHEN T2.CorporationID is null THEN 1 "
		            + " ELSE"
                    + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                    + " END"
                    + " AS IsDeletable FROM Corporations"
                    + " LEFT JOIN(select CorporationID, count(CorporationID) as CNT from Customers group by CorporationID) AS T2 ON T2.CorporationID = Corporations.ID"
                    + " ORDER BY Corporations.Name";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _corporations.Add(new CorporationModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    LogoFileName = or["LogoFileName"].ToString() ?? string.Empty,
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"]),
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _corporations;
        }

        private static AssetMovementReportModel ProcessMovementCode(int assetid, ActivityType activitycode, DateTime? datemoved, string label, string destinationcustomer, string sourcecustomer)
        {
            AssetMovementReportModel _reportedmovement = new AssetMovementReportModel();
            if (datemoved != null)
                _reportedmovement.DateMoved = datemoved;
            _reportedmovement.ID = assetid;
            _reportedmovement.AssetLabel = label;
            switch (activitycode)
            {

                case ActivityType.NewAsset:
                    _reportedmovement.Description = "New Asset " + label + " added to " + destinationcustomer;
                    break;

                case ActivityType.Transfer:
                    _reportedmovement.Description = "Asset " + label + " moved from " + sourcecustomer + " to " + destinationcustomer;
                    break;

                case ActivityType.Deleted:
                    _reportedmovement.Description = "Deleted Asset " + label + " from " + sourcecustomer;
                    break;

                case ActivityType.Undeleted:
                    _reportedmovement.Description = "Undeleted Asset " + label + " and added to " + destinationcustomer;
                    break;

            }
            return _reportedmovement;
        }

        public static FullyObservableCollection<PhotoModel> GetAssetPhotos(int assetid)
        {
            FullyObservableCollection<PhotoModel> photos = new FullyObservableCollection<PhotoModel>();
            try
            {
                string sql = "SELECT ID, PhotoFileName FROM Photos WHERE AssetID = @assetid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@assetid", assetid);
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                photos.Add(new PhotoModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    PhotoFileName = or["PhotoFileName"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return photos;
        }

        public static FullyObservableCollection<AssetModel> GetAssetConsumables(int assetid, bool selectused)
        {
            FullyObservableCollection<AssetModel> _consumables = new FullyObservableCollection<AssetModel>();
            try
            {
                string sql = "SELECT Assets.ID, Assets.ParentAssetID, Assets.Name, Assets.PurchasePrice AS Cost, Assets.DatePurchased, Assets.PONumber AS OrderReference, "
                            + " Assets.SupplierName AS Supplier, Quantity"
                            + " FROM Assets"
                            + " JOIN AssetTypes on AssetTypes.ID = Assets.TypeID"
                            + " WHERE Assets.ID = @assetid AND AssetTypes.Consumable = 1";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@assetid", assetid);
                        oc.Parameters.AddWithValue("@used", selectused);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _consumables.Add(new AssetModel
                                {
                                    ID = assetid,
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]),
                                    DatePurchased = ConvertDefaultDateToNull(or["DatePurchased"]),
                                    PONumber = or["PONumber"].ToString() ?? string.Empty,
                                    SupplierName = or["SupplierName"].ToString() ?? string.Empty,
                                    // Used = ConvertObjIntToBool(or["Used"]),
                                    Quantity = ConvertObjToInt(or["Quantity"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _consumables;
        }

        public static FullyObservableCollection<AssetSummaryModel> GetAvailableAssets(int statusid, int assetid)
        {
            FullyObservableCollection<AssetSummaryModel> availableassets = new FullyObservableCollection<AssetSummaryModel>();
            try
            {
                string sql = "";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@statusid", statusid);
                        oc.Parameters.AddWithValue("@assetid", assetid);
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                availableassets.Add(new AssetSummaryModel
                                {
                                    ID = ConvertObjToInt(or["AssetID"]),
                                    Label = or["Label"].ToString() ?? string.Empty,
                                    Description = or["Description"].ToString() ?? string.Empty,
                                    Customer = or["CustomerName"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return availableassets;
        }

        public static FullyObservableCollection<AssetGroupSpecificationModel> GetAssetGroupSpecifications(int groupid)
        {
            FullyObservableCollection<AssetGroupSpecificationModel> specifications = new FullyObservableCollection<AssetGroupSpecificationModel>();
            try
            {
                string sql = "SELECT AssetGroupSpecifications.PropertyUnitID, AssetGroupSpecifications.ID, AssetGroupSpecifications.AssetGroupID, AssetGroupSpecifications.SpecificationNameID, "
                        + " AssetGroupSpecifications.SpecificationOptions, Specifications.MeasurementUnitID"
                        + " FROM(AssetGroupSpecifications"
                        + " LEFT JOIN SpecificationPropertyUnits ON AssetGroupSpecifications.PropertyUnitID = SpecificationPropertyUnits.ID)"
                        + " LEFT JOIN Specifications ON AssetGroupSpecifications.SpecificationNameID = Specifications.ID"
                        + " WHERE AssetGroupSpecifications.AssetGroupID = @groupid"
                        + " ORDER BY SpecificationPropertyUnits.PropertyUnit";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@groupid", groupid);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                specifications.Add(new AssetGroupSpecificationModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    AssetGroupID = groupid,
                                    SpecificationNameID = ConvertObjToInt(or["SpecificationNameID"]),
                                    PropertyUnitID = ConvertObjToInt(or["PropertyUnitID"]),
                                    MeasurementUnitID = ConvertObjToInt(or["MeasurementUnitID"]),
                                    SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty

                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return specifications;
        }

        public static FullyObservableCollection<AssetGroupSpecificationModel> GetAllAssetGroupSpecifications()
        {
            FullyObservableCollection<AssetGroupSpecificationModel> specifications = new FullyObservableCollection<AssetGroupSpecificationModel>();
            try
            {
                string sql = "SELECT AssetGroupSpecifications.ID, AssetGroupSpecifications.AssetGroupID, AssetGroupSpecifications.SpecificationNameID, AssetGroupSpecifications.PropertyUnitID, "
                            + " AssetGroupSpecifications.SpecificationOptions"
                            + " FROM AssetGroupSpecifications"
                            + " LEFT JOIN Specifications ON AssetGroupSpecifications.SpecificationNameID = Specifications.ID"
                            + " LEFT JOIN SpecificationPropertyUnits ON AssetGroupSpecifications.PropertyUnitID = SpecificationPropertyUnits.ID";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                specifications.Add(new AssetGroupSpecificationModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    AssetGroupID = ConvertObjToInt(or["AssetGroupID"]),
                                    SpecificationNameID = ConvertObjToInt(or["SpecificationNameID"]),
                                    PropertyUnitID = ConvertObjToInt(or["PropertyUnitID"]),
                                    //MeasurementUnitID = ConvertObjToInt(or["MeasurementUnitID"]),
                                    SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return specifications;
        }

        public static FullyObservableCollection<AssetGroupSpecDisplayDataModel> GetAssetGroupSpecificationsForDisplay(int groupid)
        {
            FullyObservableCollection<AssetGroupSpecDisplayDataModel> specifications = new FullyObservableCollection<AssetGroupSpecDisplayDataModel>();
            try
            {
                string sql = "SELECT AssetGroupSpecifications.ID, Specifications.Name, SpecificationPropertyUnits.Name AS PropertyUnit, AssetGroupSpecifications.SpecificationOptions, Specifications.MeasurementUnitID"
                           + " FROM AssetGroupSpecifications LEFT JOIN Specifications ON AssetGroupSpecifications.SpecificationNameID = Specifications.ID"
                            + " LEFT JOIN SpecificationPropertyUnits ON AssetGroupSpecifications.PropertyUnitID = SpecificationPropertyUnits.ID"
                            + " WHERE AssetGroupSpecifications.AssetGroupID = @groupid"
                            + " ORDER BY Specifications.Name, SpecificationPropertyUnits.Name";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@assetgroupid", groupid);
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                specifications.Add(new AssetGroupSpecDisplayDataModel
                                {
                                    AssetGroupSpecificationID = ConvertObjToInt(or["ID"]),
                                    AssetGroupID = groupid,
                                    SpecificationName = or["SpecificationName"].ToString() ?? string.Empty,
                                    SpecificationPropertyName = or["PropertyUnit"].ToString() ?? string.Empty,
                                    SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty,
                                    DataTypeID = ConvertObjToInt(or["MeasurementUnitID"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return specifications;
        }

        public static FullyObservableCollection<SearchFieldModel> GetSearchFields()
        {
            FullyObservableCollection<SearchFieldModel> _searchfields = new FullyObservableCollection<SearchFieldModel>();
            try
            {
                string sql = "SELECT Label, TableName, FieldName, Mask FROM SearchFields ORDER BY Label";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _searchfields.Add(new SearchFieldModel
                                {
                                    Label = or["Label"].ToString() ?? string.Empty,
                                    TableName = or["TableName"].ToString() ?? string.Empty,
                                    FieldName = or["FieldName"].ToString() ?? string.Empty,
                                    Mask = or["Mask"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _searchfields;
        }

        public static Collection<ReportModel> GetReports()
        {
            Collection<ReportModel> _reports = new Collection<ReportModel>();
            try
            {
                string sql = "SELECT ID, Header, Parameter, Tooltip, IconFileName, HasDateFilter FROM Reports ORDER BY Header";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                _reports.Add(new ReportModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Header = or["Header"].ToString() ?? string.Empty,
                                    Parameter = or["Parameter"].ToString() ?? string.Empty,
                                    Tooltip = or["Tooltip"].ToString() ?? string.Empty,
                                    IconfileName = or["IconfileName"].ToString() ?? string.Empty,
                                    HasDateFilter = ConvertObjIntToBool(or["HasDateFilter"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _reports;
        }

        public static FullyObservableCollection<SpecificationPropertyModel> GetSpecificationProperties()
        {
            FullyObservableCollection<SpecificationPropertyModel> _properties = new FullyObservableCollection<SpecificationPropertyModel>();
            try
            {
                string sql = "SELECT SpecificationPropertyUnits.ID, SpecificationPropertyUnits.PropertyUnit, "
                    + " CASE WHEN T2.MeasurementUnitID is null THEN 1"
		            + " ELSE "
                    + " CASE WHEN T2.CNT = 0 THEN 1 ELSE 0 END"
                    + " END"
                    + " AS IsDeletable FROM SpecificationPropertyUnits"
                    + " LEFT JOIN(select MeasurementUnitID, count(MeasurementUnitID) as CNT from Specifications group by MeasurementUnitID) AS T2 ON T2.MeasurementUnitID = SpecificationPropertyUnits.ID"
                    + " ORDER BY SpecificationPropertyUnits.PropertyUnit";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _properties.Add(new SpecificationPropertyModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    PropertyUnit = or["PropertyUnit"].ToString() ?? string.Empty,
                                    IsDeletable = ConvertObjIntToBool(or["IsDeletable"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _properties;
        }

        //public static DataTable GetExcelReportData(string _excelreport)
        //{
        //    DataTable _reportdata = new DataTable(_excelreport);
        //    try
        //    {
        //        string sql = _excelreport;
        //        using (SQLiteConnection conn = new SQLiteConnection(connstr))
        //        {
        //            conn.Open();
        //            using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
        //            {
        //                //run Access query                
        //                //oc.Connection = Conn;
        //                //oc.CommandType = CommandType.StoredProcedure;
        //                oc.CommandText = _excelreport;
        //                SQLiteDataAdapter da = new SQLiteDataAdapter(oc);
        //                da.Fill(_reportdata);
        //                da.Dispose();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return _reportdata;
        //}

        public static Dictionary<int, int> GetAssetSearch(string tablename, string fieldname, string searchword)
        {
            Dictionary<int, int> assetids = new Dictionary<int, int>();

            try
            {                                
                string areaprefix = string.Empty;
                string groupidtext = string.Empty;
                string sql = string.Empty;
                int labelid = 0;
                bool isID = false;
                if (fieldname == "Label")
                {
                    int firstdelimiter = searchword.IndexOf('-');
                    int lastdelimiter = searchword.LastIndexOf('-');
                    isID = int.TryParse(searchword.Substring(lastdelimiter + 1, searchword.Length - (lastdelimiter + 1)), out labelid);
                    if (isID)
                    {
                        areaprefix = searchword.Substring(0, firstdelimiter);
                        groupidtext = searchword.Substring(firstdelimiter + 1, lastdelimiter - (firstdelimiter + 1));

                        //System.Diagnostics.Debug.Print(">>>>>>>" + areaprefix);
                        //System.Diagnostics.Debug.Print(">>>>>>>" + groupidtext);
                        //System.Diagnostics.Debug.Print(">>>>>>>" + labelid.ToString());

                        sql = "SELECT DISTINCT Assets.ID FROM Assets JOIN AssetAreas ON AssetAreas.ID = Assets.PrefixID"
                                    + " JOIN AssetGroups ON AssetGroups.ID = Assets.GroupID"
                                    + " WHERE LabelID = @labelid"
                                    + " AND AssetAreas.Prefix = @areaprefix"
                                    + " AND AssetGroups.AssetGroupIDText = @groupidtext";
                    }                   
                }
                else
                {                                    
                    sql = "SELECT DISTINCT Assets.ID FROM Assets WHERE " + fieldname + " Like '%" + searchword + "%'";
                }
                
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        if (fieldname == "Label")
                        {
                            if (isID)
                            {
                                oc.Parameters.AddWithValue("@labelid", labelid.ToString() ?? string.Empty);
                                oc.Parameters.AddWithValue("@areaprefix", areaprefix ?? string.Empty);
                                oc.Parameters.AddWithValue("@groupidtext", groupidtext ?? string.Empty);
                                using (SQLiteDataReader or = oc.ExecuteReader())
                                {
                                    while (or.Read())
                                    {
                                        assetids.Add(ConvertObjToInt(or["ID"]), ConvertObjToInt(or["ID"]));
                                    }
                                }
                            }
                        }
                        else
                        {
                            oc.Parameters.AddWithValue("@tblname", tablename ?? string.Empty);
                            oc.Parameters.AddWithValue("@fldname", fieldname ?? string.Empty);
                            oc.Parameters.AddWithValue("@searchword", searchword ?? string.Empty);
                            using (SQLiteDataReader or = oc.ExecuteReader())
                            {
                                while (or.Read())
                                {
                                    assetids.Add(ConvertObjToInt(or["ID"]), ConvertObjToInt(or["ID"]));
                                }
                            }
                        }
                    }
                }
            }           
            catch (Exception e)
            {
                ShowError(e);
            }
            return assetids;
        }


        //public static Dictionary<int, int> GetAssetSearch(string _queryname, string _criteria)
        //{
        //    Dictionary<int, int> _assetids = new Dictionary<int, int>();
        //    try
        //    {
        //        string sql = _queryname;
        //        using (SQLiteConnection conn = new SQLiteConnection(connstr))
        //        {
        //            conn.Open();
        //            using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
        //            {
        //                //oc.Connection = Conn;
        //                //oc.CommandType = CommandType.StoredProcedure;
        //                oc.CommandText = _queryname;
        //                oc.Parameters.Add("@string", DbType.String, _descriptionlength).Value = _criteria;
        //                SQLiteDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //                while (or.Read())
        //                {
        //                    _assetids.Add(ConvertObjToInt(or["AssetID"]), ConvertObjToInt(or["AssetID"]));
        //                }
        //                or.Close();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return _assetids;
        //}

        //public static Dictionary<int, int> GetAssetSearch(string _queryname, int _id)
        //{
        //    Dictionary<int, int> _assetids = new Dictionary<int, int>();
        //    try
        //    {
        //        string sql = _queryname;
        //        using (SQLiteConnection conn = new SQLiteConnection(connstr))
        //        {
        //            conn.Open();
        //            using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
        //            {
        //                //oc.Connection = Conn;
        //                //oc.CommandType = CommandType.StoredProcedure;
        //                oc.CommandText = _queryname;
        //                oc.Parameters.Add("@id", DbType.Int32).Value = _id;
        //                SQLiteDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //                while (or.Read())
        //                {
        //                    _assetids.Add(ConvertObjToInt(or["AssetID"]), ConvertObjToInt(or["AssetID"]));
        //                }
        //                or.Close();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return _assetids;
        //}

        //public static Dictionary<int, int> GetAssetSearch(string _queryname, string _criteria, int _specificationid)
        //{
        //    Dictionary<int, int> _assetids = new Dictionary<int, int>();
        //    try
        //    {
        //        string sql = _queryname;
        //        using (SQLiteConnection conn = new SQLiteConnection(connstr))
        //        {
        //            conn.Open();
        //            using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
        //            {
        //                //oc.Connection = Conn;
        //                //oc.CommandType = CommandType.StoredProcedure;
        //                oc.CommandText = _queryname;
        //                oc.Parameters.Add("@string", DbType.String, _descriptionlength).Value = _criteria;
        //                oc.Parameters.Add("@specificationid", DbType.Int32).Value = _specificationid;
        //                SQLiteDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //                while (or.Read())
        //                {
        //                    _assetids.Add(ConvertObjToInt(or["AssetID"]), ConvertObjToInt(or["AssetID"]));
        //                }
        //                or.Close();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return _assetids;
        //}

        //public static Dictionary<int, int> GetAssetSearch(string _queryname, DateTime? _startdate, DateTime? _enddate)
        //{
        //    Dictionary<int, int> _assetids = new Dictionary<int, int>();
        //    try
        //    {
        //        string sql = _queryname;
        //        using (SQLiteConnection conn = new SQLiteConnection(connstr))
        //        {
        //            conn.Open();
        //            using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
        //            {
        //                //oc.Connection = Conn;
        //                //oc.CommandType = CommandType.StoredProcedure;
        //                oc.CommandText = _queryname;
        //                oc.Parameters.Add("@startdate", DbType.String).Value = _startdate;
        //                oc.Parameters.Add("@enddate", DbType.String).Value = _enddate;

        //                //oc.Parameters.AddWithValue("@startdate", _startdate);
        //                //oc.Parameters.AddWithValue("@enddate", _enddate);

        //                SQLiteDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //                while (or.Read())
        //                {
        //                    _assetids.Add(ConvertObjToInt(or["AssetID"]), ConvertObjToInt(or["AssetID"]));
        //                }
        //                or.Close();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return _assetids;
        //}

        //public static int CountUsedID(CountSPName _spname, int _id)
        //{
        //    int count = 0;
        //    try
        //    {
        //        string sql = "";
        //        using (SQLiteConnection conn = new SQLiteConnection(connstr))
        //        {
        //            conn.Open();
        //            using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
        //            {
        //                oc.Parameters.AddWithValue("@id", _id);
        //                count = (int)oc.ExecuteScalar();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return count;
        //}

        public static FullyObservableCollection<AssetModel> GetDuplicateAssetLabels()
        {
            FullyObservableCollection<AssetModel> assets = new FullyObservableCollection<AssetModel>();
            try
            {
                string sql = "SELECT Assets.Label, Assets.AssetID, Assets.ParentAssetID, Assets.Description, AssetAreas.Description AS Area"
                + " FROM Assets"
                + " LEFT JOIN Customers ON Assets.CustomerID = Customers.ID"
                + " LEFT JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID"
                + " LEFT JOIN AssetGroups ON Assets.GroupID = AssetGroups.ID"
                + " LEFT JOIN AssetAreas ON Assets.PrefixID = AssetAreas.ID"
                + " WHERE Assets.Label In (SELECT[Label] FROM[Assets] As Tmp GROUP BY[Label] HAVING Count(*) > 1)) AND (Assets.Deleted = False))"
                + " ORDER BY Assets.Label";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader()) {
                            while (or.Read())
                            {
                                assets.Add(new AssetModel
                                {
                                    ID = ConvertObjToInt(or["AssetID"]),
                                    Name = or["Description"].ToString() ?? string.Empty,
                                    Label = or["Label"].ToString() ?? string.Empty,
                                    ParentAssetID = ConvertObjToInt(or["ParentAssetID"])
                                });
                            }
                        }                       
                    }                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assets;
        }

        public static FullyObservableCollection<AssetModel> GetAssetsWithNoLocationInformation()
        {
            FullyObservableCollection<AssetModel> _assets = new FullyObservableCollection<AssetModel>();
            try
            {
                string sql = "SELECT Assets.ID, AssetTypes.Name, Assets.ParentAssetID, Assets.Label"
                + " FROM Assets LEFT JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID"
                + " WHERE Assets.Deleted = False AND (Assets.CustomerID Is Null Or Assets.CustomerID < 1)"
                + " ORDER BY Assets.Label";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _assets.Add(new AssetModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    Label = or["Label"].ToString() ?? string.Empty,
                                    ParentAssetID = ConvertObjToInt(or["ParentAssetID"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }

        public static FullyObservableCollection<AssetSummaryModel> GetDeletedAssets()
        {
            FullyObservableCollection<AssetSummaryModel> _assets = new FullyObservableCollection<AssetSummaryModel>();
            try
            {
                string sql = "SELECT Assets.ID, Assets.PrefixID, Assets.GroupID, Assets.LabelID, Assets.ParentAssetID, AssetTypes.Name"
                + " FROM Assets"
                + " LEFT JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID"
                + " LEFT JOIN Customers ON Assets.CustomerID = Customers.ID"
                + " LEFT JOIN Statuses ON Assets.StatusID = Statuses.ID"
                + " WHERE (Assets.ParentAssetID = 0 Or Assets.ParentAssetID Is Null) AND Assets.Deleted = 1"
                ;

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _assets.Add(new AssetSummaryModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Description = or["Name"].ToString() ?? string.Empty,
                                    AssetAreaID = ConvertObjToInt(or["PrefixID"]),
                                    AssetGroupID = ConvertObjToInt(or["GroupID"]),
                                    LabelID = ConvertObjToInt(or["LabelID"]),
                                    ParentAssetID = ConvertObjToInt(or["ParentAssetID"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }

        public static FullyObservableCollection<AdministratorUserModel> GetAdministrators()
        {
            FullyObservableCollection<AdministratorUserModel> _administrators = new FullyObservableCollection<AdministratorUserModel>();
            try
            {
                string sql = "SELECT ID, Name, UserLogin FROM Administrators";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _administrators.Add(new AdministratorUserModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    LoginName = or["UserLogin"].ToString() ?? string.Empty
                                });
                            }
                        }
                    }                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _administrators;
        }
                
        //public static int[] GetLabelMask()
        //{
        //    int[] _mask = new int[3];
        //    try
        //    {
        //        string sql = "SELECT AreaChars, GroupChars, MaximumIDValue FROM LabelMask";
        //        using (SQLiteConnection conn = new SQLiteConnection(connstr))
        //        {
        //            conn.Open();
        //            using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
        //            {
        //                SQLiteDataReader or = oc.ExecuteReader();
        //                while (or.Read())
        //                {
        //                    _mask[0] = ConvertObjToInt(or["AreaChars"]);
        //                    _mask[1] = ConvertObjToInt(or["GroupChars"]);
        //                    _mask[2] = (ConvertObjToInt(or["MaximumIDValue"]) - 1).ToString().Length;
        //                }
        //                or.Close();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return _mask;
        //}

        public static FullyObservableCollection<AuditDateModel> GetAssetAuditDates(int assetid)
        {
            AuditDateModel nextaudit;
            FullyObservableCollection<AuditDateModel> audits = new FullyObservableCollection<AuditDateModel>();
            try
            {
                string sql = "SELECT ID, AuditDate FROM AuditDates WHERE AssetID = @assetid ORDER BY AuditDate DESC";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.AddWithValue("@assetid", assetid);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                nextaudit = new AuditDateModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    DateAudit = ConvertDefaultDateToNull(or["AuditDate"])
                                };
                                audits.Add(nextaudit);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return audits;
        }

        public static FullyObservableCollection<MovementActivityTypeModel> GetMovementActivityTypes()
        {
            FullyObservableCollection<MovementActivityTypeModel> types = new FullyObservableCollection<MovementActivityTypeModel>();
            string sql = "SELECT ID, Name FROM MovementActivityTypes ORDER BY Name";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                types.Add(new MovementActivityTypeModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty
                                   
                                });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return types;
        }

        public static FullyObservableCollection<CustomReportModel> GetCustomReports()
        {
            FullyObservableCollection<CustomReportModel> customreports = new FullyObservableCollection<CustomReportModel>();
            string sql = "SELECT ID, Name, SP, CombineTables FROM CustomReports ORDER BY Name";

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                customreports.Add(new CustomReportModel
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    SP = or["SP"].ToString() ?? string.Empty,
                                    CombineTables = ConvertObjToBool(or["CombineTables"])
                                });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            
            catch (Exception e)
            {
                ShowError(e);
            }
            return customreports;
        }

        public static FullyObservableCollection<CustomReportParametersModel> GetCustomReportParameters(int customreportid)
        {
            FullyObservableCollection<CustomReportParametersModel> paramcol = new FullyObservableCollection<CustomReportParametersModel>();
            string sql = "SELECT ID, ParameterType, Name, "
                    + " CASE"
                    + " WHEN DefaultValueCalcID = 0 THEN DefaultValue"
                    + " WHEN DefaultValueCalcID = 1 THEN CAST(strftime('%Y', date('now')) AS Text) || '-' || CAST(strftime('%m', date('now')) AS Text) || '-1'"
                    + " WHEN DefaultValueCalcID = 2 THEN CAST(strftime('%Y', date('now')) AS Text) || '-1-1'"
                    + " WHEN DefaultValueCalcID = 3 THEN CAST(strftime('%Y', date('now')) AS Text) || '-12-1'"
                    + " WHEN DefaultValueCalcID = 4 THEN cast(strftime('%Y', DateTime('Now', 'LocalTime', '+1 Month')) AS Text) || '-' || CAST(strftime('%m', DateTime('Now', 'LocalTime', '+1 Month')) AS Text) || '-1'"
                    + " WHEN DefaultValueCalcID = 5 THEN cast(strftime('%Y', DateTime('Now', 'LocalTime', '-1 Month')) AS Text) || '-' || CAST(strftime('%m', DateTime('Now', 'LocalTime', '-1 Month')) AS Text) || '-1'"
                    + " ELSE DefaultValue"
                    + " END as DefaultValue, "
                    + " ToolTip, DisplayName FROM CustomReportParameters"
                    + " WHERE CustomReportID = @customreportid";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@customreportid", DbType.Int32).Value = customreportid;
                        CustomReportParametersModel cm;

                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                cm = new CustomReportParametersModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    ParameterType = or["ParameterType"].ToString() ?? string.Empty,
                                    DefaultValue = or["DefaultValue"].ToString() ?? string.Empty,
                                    ToolTip = or["ToolTip"].ToString() ?? string.Empty,
                                    DisplayName = or["DisplayName"].ToString() ?? string.Empty
                                };

                                switch (cm.ParameterType)
                                {
                                    case "DateTime":
                                        if (string.IsNullOrEmpty(cm.DefaultValue))
                                            cm.Value = ConvertDateToMonth(DateTime.Now).ToString();
                                        else
                                        {
                                            bool blnDate = DateTime.TryParse(cm.DefaultValue, out DateTime enteredDate);
                                            if (blnDate)
                                                cm.Value = cm.DefaultValue;
                                            else
                                                cm.Value = ConvertDateToMonth(DateTime.Now).ToString();
                                        }
                                        break;

                                    case "Int32":
                                        if (string.IsNullOrEmpty(cm.DefaultValue))
                                            cm.Value = default(int).ToString();
                                        else
                                        {
                                            bool blnInteger = int.TryParse(cm.DefaultValue, out int enteredInteger);
                                            if (blnInteger)
                                                cm.Value = cm.DefaultValue;
                                            else
                                                cm.Value = default(int).ToString();
                                        }
                                        break;

                                    case "Decimal":
                                        if (string.IsNullOrEmpty(cm.DefaultValue))
                                            cm.Value = default(decimal).ToString();
                                        else
                                        {
                                            bool blnDecimal = decimal.TryParse(cm.DefaultValue, out decimal enteredDecimal);
                                            if (blnDecimal)
                                                cm.Value = cm.DefaultValue;
                                            else
                                                cm.Value = default(decimal).ToString();
                                        }
                                        break;

                                    case "Boolean":
                                        if (string.IsNullOrEmpty(cm.DefaultValue))
                                            cm.Value = default(bool).ToString();
                                        else
                                        {
                                            bool blnBool = bool.TryParse(cm.DefaultValue, out bool enteredBool);
                                            if (blnBool)
                                                cm.Value = cm.DefaultValue;
                                            else
                                                cm.Value = default(bool).ToString();
                                        }
                                        break;

                                    case "String":
                                        if (string.IsNullOrEmpty(cm.DefaultValue))
                                            cm.Value = string.Empty;
                                        else
                                            cm.Value = cm.DefaultValue;
                                        break;

                                }
                                paramcol.Add(cm);
                            }
                        }
                    }
                }
            }
            
            catch (Exception e)
            {
                ShowError(e);
            }
            return paramcol;
        }

        public static DataSet GetCustomReportData(CustomReportModel custreport, FullyObservableCollection<CustomReportParametersModel> parameters)
        {
            DataSet ds = new DataSet();
            string sql = custreport.SP;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {                       
                        if (parameters.Count > 0)
                        {
                            for (int i = 0; i < parameters.Count; i++)
                            {
                                switch (parameters[i].ParameterType)
                                {
                                    case "String":
                                        oc.Parameters.Add(parameters[i].Name, DbType.String, descrlen).Value = parameters[i].Value ?? string.Empty;
                                        break;

                                    case "DateTime":
                                        oc.Parameters.Add(parameters[i].Name, DbType.String).Value = Convert.ToDateTime(parameters[i].Value);
                                        break;

                                    case "Int32":
                                        oc.Parameters.Add(parameters[i].Name, DbType.Int32).Value = Convert.ToInt32(parameters[i].Value);
                                        break;

                                    case "Decimal":
                                        oc.Parameters.Add(parameters[i].Name, DbType.Decimal).Value = Convert.ToDecimal(parameters[i].Value);
                                        break;

                                    case "Boolean":
                                        oc.Parameters.Add(parameters[i].Name, DbType.Boolean).Value = ConvertObjToBool(parameters[i].Value);
                                        break;

                                    default:
                                        oc.Parameters.Add(parameters[i].Name, DbType.String, descrlen).Value = parameters[i].Value ?? string.Empty;
                                        break;
                                }
                            }
                        }

                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(oc))
                        {
                            da.Fill(ds);
                        }

                        //for each datatable...
                        //get fields from ReportFields wher custreport.name
                        //match fields with table fields
                        //update table caption and format and extended properties
                        Collection<CustomReportFieldsModel> rptflds = GetReportFields(custreport.Name);
                        bool found = false;
                        int tblctr = 0;
                        foreach (DataTable dt in ds.Tables)
                        {
                            dt.TableName = "Table" + tblctr.ToString();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                found = false;
                                foreach (CustomReportFieldsModel f in rptflds)
                                {
                                    if (f.ColumnName == dc.ColumnName)
                                    {
                                        dc.Caption = f.Caption;
                                        //dc.DataType = System.Type.GetType("System." + f.DataType); <======cannot change datatype after column is filled
                                        dc.ExtendedProperties.Add("Alignment", f.Alignment);
                                        dc.ExtendedProperties.Add("Format", f.Format);
                                        dc.ExtendedProperties.Add("FieldType", f.FieldTypeID);
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    dc.ExtendedProperties.Add("Alignment", "Left");
                                    dc.ExtendedProperties.Add("FieldType", (int)ReportFieldType.Standard);
                                }
                            };
                            tblctr++;
                        }
                    };
                }
            }
           
            catch (Exception e)
            {
                ShowError(e);
            }
            return ds;
        }

        public static FullyObservableCollection<CustomReportFieldsModel> GetReportFields(string reportname)
        {
            FullyObservableCollection<CustomReportFieldsModel> fields = new FullyObservableCollection<CustomReportFieldsModel>();

            string sql = "SELECT ReportFields.ID,  ReportFields.Caption, ReportFieldsDataTypes.DataType, Alignment, ReportFieldsDataTypes.DataFormat AS [Format], "
                        + " FieldTypeID, ReportFields.DataTypeID"
                        + " from ReportFields"
                        + " INNER JOIN ReportFieldsDataTypes ON ReportFieldsDataTypes.ID = ReportFields.DataTypeID"
                        + " JOIN CustomReports ON CustomReports.ID = ReportFields.ReportID"
                        + " where CustomReports.Name = @reportname";
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@reportname", DbType.String, descrlen).Value = reportname;

                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                fields.Add(new CustomReportFieldsModel()
                                {
                                    ID = ConvertObjToInt(or["ID"]),
                                    Caption = or["Caption"].ToString() ?? string.Empty,
                                    ColumnName = or["ColumnName"].ToString() ?? string.Empty,
                                    DataType = or["DataType"].ToString() ?? string.Empty,
                                    Alignment = or["Alignment"].ToString() ?? string.Empty,
                                    Format = or["Format"].ToString() ?? string.Empty,
                                    FieldTypeID = ConvertObjToInt(or["FieldType"]),
                                    DataTypeID = ConvertObjToInt(or["DataTypeID"])
                                });
                            }
                        }
                    }
                }
            }
            
            catch (Exception e)
            {
                ShowError(e);
            }
            return fields;
        }


        #endregion


        #region Reports

        public static Collection<AssetMovementReportModel> GetAssetMovementsReport(int assetid)
        {
            Collection<AssetMovementReportModel> reportedmovements = new Collection<AssetMovementReportModel>();
            try
            {
                string sql = "SELECT AssetMovements.DateMoved, AssetMovements.AssetID, AssetMovements.ActivityCodeID, Assets.Label, Customers.Name AS [Source], Customers_2.Name AS Destination,  "
                        + " Assets.BAANID"
                        + " FROM AssetMovements"
                        + " LEFT JOIN Assets ON AssetMovements.AssetID = Assets.ID"
                        + " LEFT JOIN Customers AS Customers_2 ON AssetMovements.DestinationCustomerID = Customers_2.ID"
                        + " LEFT JOIN Customers ON AssetMovements.SourceCustomerID = Customers.ID   "
                        + " WHERE AssetMovements.AssetID = @id"
                        + "ORDER BY AssetMovements.DateMoved DESC";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = assetid;
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                reportedmovements.Add(ProcessMovementCode(assetid, (ActivityType)Convert.ToInt32(or["ActivityCodeID"]), ConvertDefaultDateToNull(or["DateMoved"]),
                                    or["Label"].ToString(), or["DestinationCustomer"].ToString(), or["SourceCustomer"].ToString()));

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return reportedmovements;
        }

        public static DataTable GetDateFilteredMovementReportData(string _month)
        {
            DataTable _reportdata = new DataTable("MovementsReport");
            DataColumn _dc;
            DataRow _dr;

            _dc = new DataColumn
            {
                Caption = "Asset ID",
                ColumnName = "ID",
                DataType = Type.GetType("System.Int32")
            };
            _reportdata.Columns.Add(_dc);

            _dc = new DataColumn
            {
                Caption = "BAAN ID",
                ColumnName = "SAPID",
                DataType = Type.GetType("System.Int32")
            };
            _reportdata.Columns.Add(_dc);

            _dc = new DataColumn
            {
                Caption = "Date Moved",
                ColumnName = "DateMoved",
                DataType = Type.GetType("System.TextTime")
            };
            _reportdata.Columns.Add(_dc);

            _dc = new DataColumn
            {
                Caption = "Label",
                ColumnName = "Label",
                DataType = Type.GetType("System.Text")
            };
            _reportdata.Columns.Add(_dc);

            _dc = new DataColumn
            {
                Caption = "Description",
                ColumnName = "Description",
                DataType = Type.GetType("System.Text")
            };
            _reportdata.Columns.Add(_dc);

            AssetMovementReportModel _reportedmovement = new AssetMovementReportModel();
            try
            {
                string sql = " SELECT AssetMovements.DateMoved, AssetMovements.AssetID, AssetMovements.ActivityCodeID, Assets.Label, Customers.Name AS SourceCustomer, "
                        + " Customers_2.Name AS DestinationCustomer, Assets.BAANID"
                        + " FROM AssetMovements"
                        + " LEFT JOIN Assets ON AssetMovements.AssetID = Assets.ID"
                        + " LEFT JOIN Customers AS Customers_2 ON AssetMovements.DestinationCustomerID = Customers_2.ID"
                        + " LEFT JOIN Customers ON AssetMovements.SourceCustomerID = Customers.ID"
                        + " WHERE Month(DateMoved) = Month(@monthmoved) AND Year(DateMoved)= YEAR(@monthmoved)"
                        + " ORDER BY AssetMovements.DateMoved DESC";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@monthmoved", DbType.String, _namelength).Value = _month;
                        oc.Parameters.AddWithValue("@monthmoved", _month);
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                _reportedmovement = (ProcessMovementCode(ConvertObjToInt(or["AssetID"]), (ActivityType)Convert.ToInt32(or["ActivityCodeID"]), ConvertDefaultDateToNull(or["DateMoved"]),
                                   or["Label"].ToString(), or["DestinationCustomer"].ToString(), or["SourceCustomer"].ToString()));
                                _dr = _reportdata.NewRow();
                                _dr["ID"] = _reportedmovement.ID;
                                if (!string.IsNullOrEmpty(or["SAPID"].ToString()))
                                    _dr["SAPID"] = Convert.ToInt32(or["SAPID"]);
                                _dr["DateMoved"] = _reportedmovement.DateMoved;
                                _dr["Label"] = _reportedmovement.AssetLabel;
                                _dr["Description"] = _reportedmovement.Description;
                                _reportdata.Rows.Add(_dr);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _reportdata;
        }

        public static DataTable GetDateFilteredReportData(string _reportname, string _month)
        {
            DataTable _reportdata = new DataTable(_reportname);
            try
            {
                string sql = _reportname;
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@month", DbType.String, _namelength).Value = _month;

                        using (SQLiteDataAdapter da = new SQLiteDataAdapter(oc))
                        {
                            da.Fill(_reportdata);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _reportdata;
        }
        
        public static FullyObservableCollection<BaseModel> GetStatuses()
        {
            FullyObservableCollection<BaseModel> items = new FullyObservableCollection<BaseModel>();

            try
            {
                string sql = "SELECT ID, Name FROM Statuses";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                items.Add(new BaseModel
                                {
                                    ID = Convert.ToInt32(or["ID"]),
                                    Name = or["Name"].ToString() ?? string.Empty,
                                    IsDeletable = false,
                                    Selected = false
                                });
                            }
                        }
                    }
                }
            }
           
            catch (Exception e)
            {
                ShowError(e);
            }
            return items;
        }

        public static DefaultSettingsModel GetDefaultSettings()
        {
            DefaultSettingsModel defsettings = new DefaultSettingsModel();
            try
            {
                string sql = "SELECT MaxPhotoSize, PhotoHeightAndWidth, TargetPhotoQuality, PhotosFileLocation, Delimiter FROM DefaultSettings";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader or = oc.ExecuteReader())
                        {
                            while (or.Read())
                            {
                                defsettings.MaxPhotoSize = ConvertObjToInt(or["MaxPhotoSize"]);
                                defsettings.PhotoHeightAndWidth = ConvertObjToInt(or["PhotoHeightAndWidth"]);
                                defsettings.TargetPhotoQuality = ConvertObjToInt(or["TargetPhotoQuality"]);
                                defsettings.PhotosFileLocation = or["PhotosFileLocation"].ToString() ?? string.Empty;
                                defsettings.Delimiter = or["Delimiter"].ToString() ?? string.Empty;
                            }
                        }
                    }
                }
            }
           
            catch (Exception e)
            {
                ShowError(e);
            }
            return defsettings;
        }

        #endregion


        #region Insert Queries
        //====================================================================================================================================
        //Insert queries

        public static int AddAsset(AssetModel asset)
        {
            long LastRowID64 = 0;
            string sql = "INSERT INTO Assets ( CustomerID, ParentAssetID, DateAdded, Temporary, Deleted )"
	                + " VALUES(@customerid, @parentassetid, @dateadded, 1, 0)";
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                conn.Open();                                       
                using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                try
                {
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        oc.Parameters.Add("@customerid", DbType.Int32).Value = asset.CustomerID;                       
                        oc.Parameters.Add("@parentassetid", DbType.Int32).Value = asset.ParentAssetID;
                        oc.Parameters.Add("@dateadded", DbType.Date).Value = DateTime.Now;
                        oc.ExecuteNonQuery();                        
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }            
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
            return (int) LastRowID64;
        }

        public static int AddCustomer(CustomerModel customer)
        {
            long LastRowID64 = 0;
            string sql = "INSERT INTO Customers ( CustomerNumber, Name, Location, CountryID, CorporationID, IconFile )"
                        + " VALUES(@customernumber, @customername, @location, @countryid, @corporationid, @iconfilename)";
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                conn.Open();
                using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                try
                {
                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        oc.Parameters.Add("@customernumber", DbType.String, _namelength).Value = customer.CustomerNumber ?? string.Empty;
                        oc.Parameters.Add("@customername", DbType.String, _namelength).Value = customer.Name ?? string.Empty;
                        oc.Parameters.Add("@location", DbType.String, _descriptionlength).Value = customer.Location ?? string.Empty;
                        oc.Parameters.Add("@countryid", DbType.Int32).Value = customer.CountryID;
                        oc.Parameters.Add("@corporationid", DbType.Int32).Value = customer.CorporationID;
                        oc.Parameters.Add("@iconfilename", DbType.String, _filenamelength).Value = customer.IconFileName ?? string.Empty;
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }            
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
            return (int)LastRowID64;
        }

        public static int AddAdministrator(AdministratorUserModel administrator)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO Administrators ( Name, UserLogin ) VALUES (@administratorname, @loginname)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@administratorname", DbType.String, _namelength).Value = administrator.Name ?? string.Empty;
                        oc.Parameters.Add("@loginname", DbType.String, _namelength).Value = administrator.LoginName ?? string.Empty;
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddCountry(CountryModel country)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO Countries ( Name, OperatingCompanyID ) VALUES (@countryname, @operatingcompanyid)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@countryname", DbType.String, _namelength).Value = country.Name ?? string.Empty;
                        oc.Parameters.Add("@operatingcompanyid", DbType.Int32).Value = country.OperatingCompanyID;
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64; 
        }

        public static int AddSalesDivision(SalesDivisionModel salesdivision)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO SalesDivisions ( Name, OperatingCompanyID ) VALUES (@salesdivisionname, @operatingcompanyid)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@salesdivisionname", DbType.String, _namelength).Value = salesdivision.Name ?? string.Empty;
                        oc.Parameters.Add("@operatingcompanyid", DbType.Int32).Value = salesdivision.OperatingCompanyID;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }               

        public static int AddMaintenanceRecord(MaintenanceRecordModel maintenancerecord)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO MaintenanceRecords ( AssetID, MaintenanceDate, Description, Cost, MaintainedBy, Completed, ScheduledMaintenanceDate )"
                + " VALUES(@assetid, @maintenancedate, @description, @cost, @maintainedby, 1, @scheduledmaintenancedate)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = maintenancerecord.AssetID;
                        oc.Parameters.Add("@maintenancedate", DbType.Date).Value = (maintenancerecord.MaintenanceDate == null) ? DefaultDate() : maintenancerecord.MaintenanceDate;
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = maintenancerecord.Name ?? string.Empty;
                        oc.Parameters.Add("@cost", DbType.Currency).Value = maintenancerecord.Cost;
                        oc.Parameters.Add("@maintainedby", DbType.String, _namelength).Value = maintenancerecord.MaintainedBy;
                        oc.Parameters.Add("@scheduledmaintenancedate", DbType.Date).Value = (maintenancerecord.ScheduledMaintenanceDate == null) ? DefaultDate() : maintenancerecord.ScheduledMaintenanceDate;
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddScheduledMaintenance(MaintenanceRecordModel scheduledmaintenance)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO MaintenanceRecords ( AssetID, ScheduledMaintenanceDate, Description, Cost, MaintainedBy, Completed )"
                    + " VALUES(@assetid, @scheduledmaintenancedate, @description, @cost, @maintainedby, 0)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = scheduledmaintenance.AssetID;
                        oc.Parameters.Add("@scheduledmaintenancedate", DbType.Date).Value = (scheduledmaintenance.ScheduledMaintenanceDate == null) ? DefaultDate() : scheduledmaintenance.ScheduledMaintenanceDate;
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = scheduledmaintenance.Name ?? string.Empty;
                        oc.Parameters.Add("@cost", DbType.Currency).Value = scheduledmaintenance.Cost;
                        oc.Parameters.Add("@maintainedby", DbType.String).Value = scheduledmaintenance.MaintainedBy;                                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddAssetArea(AssetAreaModel assetarea)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO AssetAreas ( Prefix, Name ) VALUES (@prefix, @description)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@prefix", DbType.String, _prefixlength).Value = assetarea.Prefix ?? string.Empty;
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = assetarea.Name ?? string.Empty;                     
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddAssetGroup(AssetGroupModel assetgroup)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO AssetGroups ( AssetPrefixID, [Group], AssetGroupIDText, CanBeParent ) VALUES (@assetprefixid, @group, @assetgroupidtext, @canbeparent)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetprefixid", DbType.Int32).Value = assetgroup.AssetAreaID;
                        oc.Parameters.Add("@group", DbType.String, _namelength).Value = assetgroup.Name ?? string.Empty;
                        oc.Parameters.Add("@assetgroupidtext", DbType.String, 255).Value = assetgroup.AssetGroupIDText ?? string.Empty;
                        oc.Parameters.Add("@canbeparent", DbType.Int32).Value = assetgroup.CanBeParent;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddAssetType(AssetTypeModel assettype)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO AssetTypes ( AssetGroupID, Name ) VALUES (@assetgroupid, @description)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetgroupid", DbType.Int32).Value = assettype.AssetGroupID;
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = assettype.Name ?? string.Empty;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddAssetMovement(AssetMovementModel assetmovement)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO AssetMovements ( DateMoved, ActivityCodeID, AssetID, SourceCustomerID, DestinationCustomerID )"
                + " VALUES(@datemoved, @activitycode, @assetid, @sourcecustomer, @destinationcustomer)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@datemoved", DbType.Date).Value = (assetmovement.DateMoved == null) ? DefaultDate() : assetmovement.DateMoved;
                        oc.Parameters.Add("@activitycode", DbType.Int32).Value = assetmovement.ActivityCodeID;
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = assetmovement.AssetID;
                        oc.Parameters.Add("@sourcecustomer", DbType.Int32).Value = assetmovement.SourceCustomerID;
                        oc.Parameters.Add("@destinationcustomer", DbType.Int32).Value = assetmovement.DestinationCustomerID;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddOperatingCompany(OperatingCompanyModel opco)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO OperatingCompany ( Name ) VALUES (@operatingcompany)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@operatingcompany", DbType.String, _namelength).Value = opco.Name;                      
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddCorporation(CorporationModel opco)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO Corporations ( Name ) VALUES (@corporation)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@corporation", DbType.String, _namelength).Value = opco.Name;                       
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddPhoto(PhotoModel photo)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO Photos ( AssetID, PhotoFileName ) VALUES (@assetid, @photofilename)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = photo.AssetID;
                        oc.Parameters.Add("@photofilename", DbType.String, _filenamelength).Value = photo.PhotoFileName ?? string.Empty;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddSpecification(AssetSpecificationModel spec)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO Specifications ( Name, MeasurementUnitID ) VALUES (@specification, @measurementunitid)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@specification", DbType.String, _namelength).Value = spec.Name;
                        oc.Parameters.Add("@measurementunitid", DbType.Int32).Value = spec.MeasurementUnitID;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddSpecificationProperty(SpecificationPropertyModel specprop)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO SpecificationPropertyUnits ( PropertyUnit ) VALUES (@propertyunit)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@propertyunit", DbType.String, _namelength).Value = specprop.PropertyUnit;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

        public static int AddConsumable(AssetModel consumable)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO Assets( ParentAssetID, Name, PurchasePrice, DatePurchased, PONumber, SupplierName, Quantity, Consumable )"
                        + " VALUES(@assetid, @description, @cost, @datepurchased, @orderreference, @supplier, @quantity, 1)";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = consumable.ID;
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = consumable.Name ?? string.Empty;
                        oc.Parameters.Add("@cost", DbType.Currency).Value = consumable.PurchasePrice;
                        oc.Parameters.Add("@datepurchased", DbType.Date).Value = (consumable.DatePurchased == null) ? DefaultDate() : consumable.DatePurchased;
                        oc.Parameters.Add("@orderreference", DbType.String, _namelength).Value = consumable.PONumber ?? string.Empty;
                        oc.Parameters.Add("@supplier", DbType.String, _namelength).Value = consumable.SupplierName ?? string.Empty;
                        //oc.Parameters.Add("@used", DbType.Boolean).Value = false;// _consumable.Used;
                        oc.Parameters.Add("@quantity", DbType.Int32).Value = consumable.Quantity;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }
              
        public static int AddAssetGroupSpecification(AssetGroupSpecificationModel specification)
        {
            long LastRowID64 = 0;
            try
            {
                string sql = "INSERT INTO AssetGroupSpecifications ( AssetGroupID, SpecificationNameID, PropertyUnitID, SpecificationOptions )"
                    + " VALUES(@assetgroupid, @specificationnameid, @propertyunitid, @specificationoptions)";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetgroupid", DbType.Int32).Value = specification.AssetGroupID;
                        oc.Parameters.Add("@specificationnameid", DbType.Int32).Value = specification.SpecificationNameID;
                        oc.Parameters.Add("@propertyunitid", DbType.Int32).Value = specification.PropertyUnitID;
                        oc.Parameters.Add("@specificationoptions", DbType.String, 255).Value = specification.SpecificationOptions;                        
                        oc.ExecuteNonQuery();
                        oc.CommandText = "select last_insert_rowid()";
                        LastRowID64 = (long)oc.ExecuteScalar();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return (int)LastRowID64;
        }

       
        public static int AddAuditDate(int assetid, DateTime? dtaudit)
        {
            long LastRowID64 = 0;
            string sql = "INSERT INTO AuditDates ( AuditDate, AssetID ) VALUES (@auditdate, @assetid)";
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                conn.Open();
                using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    try
                    {
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {
                            oc.Parameters.Add("@auditdate", DbType.Date).Value = dtaudit;
                            oc.Parameters.Add("@assetid", DbType.Int32).Value = assetid;
                            oc.ExecuteNonQuery();
                            oc.CommandText = "select last_insert_rowid()";
                            LastRowID64 = (long)oc.ExecuteScalar();
                            transaction.Commit();
                        }
                    }

                    catch (Exception e)
                    {
                        ShowError(e);
                    }
            }
            return (int)LastRowID64;
        }

        public static int AddAssetGroupSpecificationValue(AssetGroupSpecificationValuesModel spec)
        {
            long LastRowID64 = 0;
            string sql = "INSERT INTO AssetGroupSpecificationValues ( SpecificationValue, AssetID, AssetGroupID, AssetGroupSpecificationID )"
                + " VALUES(@specificationvalue, @assetid, @assetgroupid, @groupspecificationid)";
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                conn.Open();
                using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    try
                    {
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {

                            oc.Parameters.Add("@specificationvalue", DbType.String, descrlen).Value = spec.SpecificationValue ?? string.Empty;
                            oc.Parameters.Add("@assetid", DbType.Int32).Value = spec.AssetID;
                            oc.Parameters.Add("@assetgroupid", DbType.Int32).Value = spec.AssetGroupID;
                            oc.Parameters.Add("@groupspecificationid", DbType.Int32).Value = spec.ID;
                            oc.ExecuteNonQuery();
                            oc.CommandText = "select last_insert_rowid()";
                            LastRowID64 = (long)oc.ExecuteScalar();
                            transaction.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        ShowError(e);
                        
                    }
            }
            return (int)LastRowID64;
        }
        #endregion

        #region Update Queries
        //====================================================================================================================================
        //Update queries

        public static void UpdateAsset(AssetModel asset)
        {
            try
            {
                string sql = "UPDATE Assets SET Name = @name, ApplicationType = @applicationtype, SiteLocation = @sitelocation,"
                    + " ManufacturerName = @manufacturername, ModelNo = @modelno, SerialNo = @serialno, DateInstalled = @dateinstalled,"
                    + " PurchasePrice = @purchaseprice, Comments = @comments, ChemicalsUsed = @chemicalsused, "
                    + " DatePurchased = @datepurchased, PONumber = @ponumber, TypeID = @assettypeid, CustomerID = @customerid,"
                    + " SalesDivisionID = @salesdivisionid, StatusID = @statusid, ParentAssetID = @parentassetid, GroupID = @assetgroupid,"
                    + " PrefixID = @prefixid, LabelID = @labelid, Dimensions = @dimensions, SupplierName = @suppliername,"
                    + " BAANID = @sapid, Quantity = @quantity, Temporary = 0, Deleted = 0, Consumable = 0 WHERE ID = @id";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@name", DbType.String, descrlen).Value = asset.Name ?? string.Empty;
                        oc.Parameters.Add("@applicationtype", DbType.String, descrlen).Value = asset.ApplicationType ?? string.Empty;
                        oc.Parameters.Add("@sitelocation", DbType.String, descrlen).Value = asset.SiteLocation ?? string.Empty;
                        oc.Parameters.Add("@manufacturername", DbType.String, descrlen).Value = asset.ManufacturerName ?? string.Empty;
                        oc.Parameters.Add("@modelno", DbType.String, descrlen).Value = asset.ModelNo ?? string.Empty;
                        oc.Parameters.Add("@serialno", DbType.String, descrlen).Value = asset.SerialNo ?? string.Empty;
                        if (asset.DateInstalled == null)
                            oc.Parameters.Add("@dateinstalled", DbType.Date).Value = DBNull.Value;
                        else
                            oc.Parameters.Add("@dateinstalled", DbType.Date).Value = asset.DateInstalled;
                        oc.Parameters.Add("@purchaseprice", DbType.Currency).Value = asset.PurchasePrice;
                        oc.Parameters.Add("@comments", DbType.String, descrlen).Value = asset.Comments ?? string.Empty;
                        oc.Parameters.Add("@chemicalsused", DbType.String, descrlen).Value = asset.ChemicalsUsed ?? string.Empty;
                        oc.Parameters.Add("@datepurchased", DbType.Date).Value = asset.DatePurchased;
                        oc.Parameters.Add("@ponumber", DbType.String, descrlen).Value = asset.PONumber ?? string.Empty;
                        oc.Parameters.Add("@assettypeid", DbType.Int32).Value = asset.AssetTypeID;
                        oc.Parameters.Add("@customerid", DbType.Int32).Value = asset.CustomerID;
                        oc.Parameters.Add("@salesdivisionid", DbType.Int32).Value = asset.SalesDivisionID;
                        oc.Parameters.Add("@statusid", DbType.Int32).Value = asset.StatusID;
                        oc.Parameters.Add("@parentassetid", DbType.Int32).Value = asset.ParentAssetID;
                        oc.Parameters.Add("@assetgroupid", DbType.Int32).Value = asset.AssetGroupID;
                        oc.Parameters.Add("@prefixid", DbType.Int32).Value = asset.AssetAreaID;                        
                        oc.Parameters.Add("@labelid", DbType.Int32).Value = asset.LabelID;
                        oc.Parameters.Add("@dimensions", DbType.String, descrlen).Value = asset.Dimensions ?? string.Empty;
                        oc.Parameters.Add("@suppliername", DbType.String, descrlen).Value = asset.SupplierName ?? string.Empty;
                        oc.Parameters.Add("@sapid", DbType.String, descrlen).Value = asset.SAPID ?? string.Empty;
                        oc.Parameters.Add("@quantity", DbType.Int32).Value = 1;                        
                        oc.Parameters.Add("@id", DbType.Int32).Value = asset.ID;
                        // execute
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateChildAsset(int assetid, int customerid, int statusid)
        {
            try
            {
                string sql = "UPDATE Assets SET CustomerID = @customerid, StatusID = @statusid WHERE ID = @assetid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@customerid", DbType.Int32).Value = customerid;
                        oc.Parameters.Add("@statusid", DbType.Int32).Value = statusid;
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = assetid;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateCustomer(CustomerModel customer)
        {
            try
            {
                string sql = "UPDATE Customers SET CustomerNumber = @customernumber, Name = @customername, Location = @location, CountryID = @countryid, CorporationID = @corporationid, IconFile = @iconfilename"
                + " WHERE ID = @customerid";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@customernumber", DbType.String, _namelength).Value = customer.CustomerNumber ?? string.Empty;
                        oc.Parameters.Add("@customername", DbType.String, _namelength).Value = customer.Name ?? string.Empty;
                        oc.Parameters.Add("@location", DbType.String, _namelength).Value = customer.Location ?? string.Empty;
                        oc.Parameters.Add("@countryid", DbType.Int32).Value = customer.CountryID;
                        oc.Parameters.Add("@corporationid", DbType.Int32).Value = customer.CorporationID;
                        oc.Parameters.Add("@iconfilename", DbType.String, _filenamelength).Value = customer.IconFileName ?? string.Empty;
                        oc.Parameters.Add("@customerid", DbType.Int32).Value = customer.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAdministrator(AdministratorUserModel administrator)
        {
            try
            {
                string sql = "UPDATE Administrators SET Name = @administratorname, UserLogin = @loginname WHERE ID = @administratorid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@administratorname", DbType.String, _namelength).Value = administrator.Name ?? string.Empty;
                        oc.Parameters.Add("@loginname", DbType.String, _namelength).Value = administrator.LoginName ?? string.Empty;
                        oc.Parameters.Add("@administratorid", DbType.Int32).Value = administrator.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateCountry(CountryModel country)
        {
            try
            {
                string sql = "UPDATE Countries SET Name = @countryname, OperatingCompanyID = @operatingcompanyid WHERE ID = @countryid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@countryname", DbType.String, _namelength).Value = country.Name ?? string.Empty;
                        oc.Parameters.Add("@operatingcompanyid", DbType.Int32).Value = country.OperatingCompanyID;
                        oc.Parameters.Add("@countryid", DbType.Int32).Value = country.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateSalesDivision(SalesDivisionModel salesdivision)
        {
            try
            {
                string sql = "UPDATE SalesDivisions SET Name = @salesdivisionname, OperatingCompanyID = @operatingcompanyid WHERE ID = @salesdivisionid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@salesdivisionname", DbType.String, _namelength).Value = salesdivision.Name ?? string.Empty;
                        oc.Parameters.Add("@operatingcompanyid", DbType.Int32).Value = salesdivision.OperatingCompanyID;
                        oc.Parameters.Add("@salesdivisionid", DbType.Int32).Value = salesdivision.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateOperatingCompany(OperatingCompanyModel operatingcompany)
        {
            try
            {
                string sql = "UPDATE OperatingCompany SET Name = @operatingcompany WHERE ID = @operatingcompanyid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@operatingcompany", DbType.String, _namelength).Value = operatingcompany.Name ?? string.Empty;                       
                        oc.Parameters.Add("@operatingcompanyid", DbType.Int32).Value = operatingcompany.ID;
                        // execute
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateMaintenanceRecord(MaintenanceRecordModel maintenancerecord)
        {
            try
            {
                string sql = "UPDATE MaintenanceRecords SET AssetID = @assetid, MaintenanceDate = @maintenancedate, Description = @description, Cost = @cost,"
                    + " MaintainedBy = @maintainedby, Completed = @completed, ScheduledMaintenanceDate = @scheduledmaintenancedate WHERE ID = @maintenancerecordid";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = maintenancerecord.AssetID;
                        oc.Parameters.Add("@maintenancedate", DbType.Date).Value = (maintenancerecord.MaintenanceDate == null) ? DefaultDate() : maintenancerecord.MaintenanceDate;
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = maintenancerecord.Name ?? string.Empty;
                        oc.Parameters.Add("@cost", DbType.Currency).Value = maintenancerecord.Cost;
                        oc.Parameters.Add("@maintainedby", DbType.String, _namelength).Value = maintenancerecord.MaintainedBy ?? string.Empty;
                        oc.Parameters.Add("@completed", DbType.Boolean).Value = maintenancerecord.Completed;
                        oc.Parameters.Add("@scheduledmaintenancedate", DbType.Date).Value = (maintenancerecord.ScheduledMaintenanceDate == null) ? DefaultDate() : maintenancerecord.ScheduledMaintenanceDate;
                        oc.Parameters.Add("@maintenancerecordid", DbType.Int32).Value = maintenancerecord.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateMaintenanceType(MaintenanceTypeModel maintenancetype)
        {
            try
            {
                string sql = "";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@maintenancetype", DbType.String, _namelength).Value = maintenancetype.Name ?? string.Empty;
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = maintenancetype.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetArea(AssetAreaModel assetarea)
        {
            try
            {
                string sql = "UPDATE AssetAreas SET Prefix = @prefix, Name = @description WHERE ID = @assetareadid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@prefix", DbType.String, _prefixlength).Value = assetarea.Prefix ?? string.Empty;
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = assetarea.Name ?? string.Empty;
                       // oc.Parameters.Add("@default", DbType.Int32).Value = _assetarea.Default;
                        oc.Parameters.Add("@assetareaid", DbType.Int32).Value = assetarea.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetGroup(AssetGroupModel assetgroup)
        {
            try
            {
                string sql = "UPDATE AssetGroups SET AssetPrefixID = @assetprefixid, [Group] = @group, AssetGroupIDText = @assetgroupidtext, CanBeParent = @canbeparent"
                + " WHERE ID = @assetgroupid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetprefix", DbType.Int32).Value = assetgroup.AssetAreaID;
                        oc.Parameters.Add("@group", DbType.String, _namelength).Value = assetgroup.Name ?? string.Empty;
                        oc.Parameters.Add("@assetgrouptextid", DbType.String, _prefixlength).Value = assetgroup.AssetGroupIDText ?? string.Empty;
                        oc.Parameters.Add("@canbeparent", DbType.Int32).Value = assetgroup.CanBeParent;
                        oc.Parameters.Add("@assetgroupid", DbType.Int32).Value = assetgroup.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetType(AssetTypeModel assettype)
        {
            try
            {
                string sql = "UPDATE AssetTypes SET AssetGroupID = @assetgroupid, Name = @description WHERE ID = @assettypeid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetgroupid", DbType.Int32).Value = assettype.AssetGroupID;
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = assettype.Name ?? string.Empty;
                        oc.Parameters.Add("@assettypeid", DbType.Int32).Value = assettype.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateCorporation(CorporationModel corporation)
        {
            try
            {
                string sql = "UPDATE Corporations SET Name = @corporationname WHERE ID = @corporationid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@corporationname", DbType.String, _namelength).Value = corporation.Name ?? string.Empty;
                        oc.Parameters.Add("@corporationid", DbType.Int32).Value = corporation.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetPhoto(PhotoModel photo)
        {
            try
            {
                string sql = "UPDATE Photos SET PhotoFileName = @photofilename WHERE ID = @photoid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@photofilename", DbType.String, _filenamelength).Value = photo.PhotoFileName ?? string.Empty;
                        oc.Parameters.Add("@photoid", DbType.Int32).Value = photo.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateSpecification(AssetSpecificationModel spec)
        {
            try
            {
                string sql = "UPDATE Specifications SET Name = @specificationname, MeasurementUnitID = @measurementunitid WHERE ID = @specificationtypeid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@specificationname", DbType.String, _namelength).Value = spec.Name ?? string.Empty;
                        oc.Parameters.Add("@measurementunitid", DbType.Int32).Value = spec.MeasurementUnitID;
                        oc.Parameters.Add("@specificationtypeid", DbType.Int32).Value = spec.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetGroupSpecification(AssetGroupSpecificationModel spec)
        {
            try
            {
                string sql = "UPDATE AssetGroupSpecifications SET AssetGroupSpecifications.AssetGroupID = @assetgroupid, AssetGroupSpecifications.SpecificationNameID = @specificationnameid, "
                    + " AssetGroupSpecifications.PropertyUnitID = @propertyunitid, AssetGroupSpecifications.SpecificationOptions = @specificationoptions"
                    + " WHERE AssetGroupSpecifications.ID = @id";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetgroupid", DbType.Int32).Value = spec.AssetGroupID;
                        oc.Parameters.Add("@specificationnameid", DbType.Int32).Value = spec.SpecificationNameID;
                        oc.Parameters.Add("@propertyunitid", DbType.Int32).Value = spec.PropertyUnitID;
                        oc.Parameters.Add("@specificationoptions", DbType.String, 255).Value = spec.SpecificationOptions ?? string.Empty;
                        oc.Parameters.Add("@id", DbType.Int32).Value = spec.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetGroupSpecificationValue(AssetGroupSpecificationValuesModel spec)
        {
            string sql = "UPDATE AssetGroupSpecificationValues SET AssetGroupSpecificationValues.SpecificationValue = @specificationvalue WHERE ID = @id";
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                conn.Open();
                using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                try
                {                                        
                    using(SQLiteTransaction transaction = conn.BeginTransaction())
                    {                                        
                        oc.Parameters.Add("@specificationvalue", DbType.String, descrlen).Value = spec.SpecificationValue ?? string.Empty;
                        oc.Parameters.Add("@id", DbType.Int32).Value = spec.AssetGroupSpecificationValueID;
                        oc.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }                             
                catch (Exception e)
                {
                    ShowError(e);                   
                }
            }
        }

        //public static void UpdateSpecificationRecord(AssetGroupSpecDisplayDataModel _spec)
        //{
        //    try
        //    {
        //        string sql = "";
        //        using (SQLiteConnection conn = new SQLiteConnection(connstr))
        //        {
        //            conn.Open();
        //            using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
        //            {
        //                //oc.CommandType = CommandType.StoredProcedure;

        //                oc.Parameters.Add("@specificationvalue", DbType.String, _descriptionlength).Value = _spec.SpecificationValue ?? string.Empty;
        //                if (_spec.ID != 0)
        //                {
        //                    oc.CommandText = "UpdateSpecificationRecord";
        //                    oc.Parameters.Add("@specificationid", DbType.Int32).Value = _spec.ID;
        //                }
        //                else
        //                {
        //                    oc.CommandText = "AddSpecificationRecord";
        //                    oc.Parameters.Add("@assetid", DbType.Int32).Value = _spec.AssetID;
        //                    oc.Parameters.Add("@assetgroupid", DbType.Int32).Value = _spec.AssetGroupID;
        //                    oc.Parameters.Add("@groupspecificationid", DbType.Int32).Value = _spec.AssetGroupSpecificationID;
        //                }
        //                // execute
        //                oc.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //}

        public static void UpdateSpecificationProperty(SpecificationPropertyModel specprop)
        {
            try
            {
                string sql = "UPDATE SpecificationPropertyUnits SET PropertyUnit = @specproperty WHERE ID = @specpropertyid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@specproperty", DbType.String, _namelength).Value = specprop.PropertyUnit ?? string.Empty;
                        oc.Parameters.Add("@specpropertyid", DbType.Int32).Value = specprop.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateConsumable(AssetModel consumable)
        {
            try
            {
                string sql = "UPDATE Assets SET Name = @description, Cost = @cost, DatePurchased = @datepurchased, OrderReference = @orderreference,"
                    + " Supplier = @supplier, Quantity = @quantity WHERE ID = @id";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {                       
                        oc.Parameters.Add("@description", DbType.String, _descriptionlength).Value = consumable.Name ?? string.Empty;
                        oc.Parameters.Add("@cost", DbType.Currency).Value = consumable.PurchasePrice;
                        oc.Parameters.Add("@datepurchased", DbType.Date).Value = (consumable.DatePurchased == null) ? DefaultDate() : consumable.DatePurchased;
                        oc.Parameters.Add("@orderreference", DbType.String, _namelength).Value = consumable.PONumber ?? string.Empty;
                        oc.Parameters.Add("@supplier", DbType.String, _namelength).Value = consumable.SupplierName ?? string.Empty;
                        //oc.Parameters.Add("@used", DbType.Boolean).Value = false;// consumable.Used;
                        oc.Parameters.Add("@quantity", DbType.Int32).Value = consumable.Quantity;
                        oc.Parameters.Add("@id", DbType.Int32).Value = consumable.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateParentAssetID(int assetid, int parentassetid, int customerid)
        {
            try
            {
                string sql = "UPDATE Assets SET ParentAssetID = @parentassetid, CustomerID = @customerid WHERE ID = @assetid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@parentassetid", DbType.Int32).Value = parentassetid;
                        oc.Parameters.Add("@customerid", DbType.Int32).Value = customerid;
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = assetid;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UnDeleteAsset(int assetid, int defaultcustomerid, int statusid)
        {
            try
            {
                string sql = "UPDATE Assets SET Deleted = 0, CustomerID = @defaultcustomerid, StatusID = @statusid WHERE AssetID = @assetid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@defaultcustomerid", DbType.Int32).Value = defaultcustomerid;
                        oc.Parameters.Add("@statusid", DbType.Int32).Value = statusid;
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = assetid;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAuditDate(int id, DateTime? dtaudit)
        {
            try
            {
                string sql = "UPDATE AuditDates SET AuditDate = @auditdate WHERE ID = @id";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@auditdate", DbType.Date).Value = dtaudit;
                        oc.Parameters.Add("@id", DbType.Int32).Value = id;
                        // execute
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateDefaultSettings(DefaultSettingsModel settings)
        {
            try
            {
                string sql = "UPDATE DefaultSettings SET PhotosFileLocation = @photosfolder";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@photosfolder", DbType.String).Value = settings.PhotosFileLocation ?? string.Empty;                       
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateCustomerLogo(int customerid, string filename)
        {           
            string sql = "UPDATE Customers SET IconFile = @filename WHERE ID = @id";
            using (SQLiteConnection conn = new SQLiteConnection(connstr))
            {
                conn.Open();
                using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                {
                    try
                    {
                        using (SQLiteTransaction transaction = conn.BeginTransaction())
                        {
                            oc.Parameters.Add("@filename", DbType.String).Value = filename;
                            oc.Parameters.Add("@id", DbType.Int32).Value = customerid;
                            oc.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        ShowError(e);
                    }
                }
            }
        }


        #endregion

        #region Delete Queries

        public static void RemoveChildAsset(int childassetid)
        {
            try
            {
                string sql = "UPDATE Assets SET ParentAssetID = 0 WHERE ID = @assetid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@assetid", DbType.Int32).Value = childassetid;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void SetConsumableToUsed(int consumableid)
        {
            try
            {
                string sql = "UPDATE Assets SET Quantity = 0 WHERE ID = @consumableid";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@consumableid", DbType.Int32).Value = consumableid;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void DeleteItem(int id, string sp)
        {
            try
            {
                string sql = "DELETE FROM " + sp.ToString() + " WHERE ID = @id ";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@id", DbType.Int32).Value = id;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void DeleteAsset( AssetModel asset)
        {
            try
            {
                string sql = "UPDATE Assets SET Deleted = 1 WHERE ID = @id ";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@id", DbType.Int32).Value = asset.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void DeletePhoto(PhotoModel photo)
        {
            try
            {
                string sql = "DELETE FROM Photos WHERE ID = @id";
                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@id", DbType.Int32).Value = photo.ID;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void SetParentAssetID(int assetid, int parentassetid)
        {
            try
            {
                string sql = "UPDATE Assets SET ParentAssetID = @parentassetid WHERE ID = @id";

                using (SQLiteConnection conn = new SQLiteConnection(connstr))
                {
                    conn.Open();
                    using (SQLiteCommand oc = new SQLiteCommand(sql, conn))
                    {
                        oc.Parameters.Add("@parentassetid", DbType.Int32).Value = parentassetid;
                        oc.Parameters.Add("@id", DbType.Int32).Value = assetid;
                        oc.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        #endregion

        #region Helpers

        public static DateTime? ConvertDateToMonth(DateTime? dt)
        {
            if (dt == null) return null;
            return new DateTime(((DateTime)dt).Year, ((DateTime)dt).Month, 1);
        }

        private static int ConvertObjToInt(object _obj)
        {
            bool _isnumber = int.TryParse(_obj.ToString(), out int _id);
            return _id;
        }

        private static decimal ConvertObjToDecimal(object _obj)
        {
            bool _isnumber = decimal.TryParse(_obj.ToString(), out decimal _id);
            return _id;
        }

        private static bool ConvertObjToBool(object _obj)
        {
            bool _isbool = bool.TryParse(_obj.ToString(), out bool _bool);
            return _bool;
        }

        private static bool ConvertObjIntToBool(object _obj)
        {
            bool _isnumber = int.TryParse(_obj.ToString(), out int _id);
            return (_id == 1);
        }

        private static DateTime? ConvertObjToDate(object _obj)
        {
            bool _isdate = DateTime.TryParse(_obj.ToString(), out DateTime _dt);
            if (_isdate)
                return _dt;
            else
                return null;
        }

        public static int ConvertDateToMonthInt(DateTime? _dt)
        {
            return ((DateTime)_dt).Month - 1 + ((DateTime)_dt).Year * 12;
        }

        public static DateTime ConvertMonthIntToDateTime(int _monthint)
        {
            int _month = _monthint % 12 + 1;
            int _year = (_monthint - _monthint % 12) / 12;
            return new DateTime(_year, _month, 1);
        }

        public static string ConvertMonthDateToString(DateTime? _dt)
        {
            if (_dt == null) return string.Empty;
            return ((DateTime)_dt).ToString("dd") + "-" + ((DateTime)_dt).ToString("MMM") + "-" + ((DateTime)_dt).ToString("yyyy");
        }

        public static DateTime DefaultDate()
        {
            return new DateTime(1900, 1, 1);
        }

        public static DateTime? ConvertDefaultDateToNull(object _dt)
        {
            DateTime _newdt;
            bool _isdate = DateTime.TryParse(_dt.ToString(), out _newdt);

            if (_isdate)
                if (_newdt == DefaultDate() || _dt == null)
                    return (DateTime?)null;
                else
                    return _newdt;
            else
                return (DateTime?)null;

        }

        private static void ShowError(Exception e, [CallerMemberName] string _operationtype = null)
        {

            IMessageBoxService _msg = new MessageBoxService();
            _msg.ShowMessage("Error during " + _operationtype + " operation\n" + e.Message.ToString(), _operationtype + " Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            _msg = null;
        }


    }
    #endregion

}
