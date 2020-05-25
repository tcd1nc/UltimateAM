using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;

namespace AssetManager.DataLayer
{
    static class zDatabaseQueries
    {
        static int _namelength = 50;
        static int _descriptionlength = 50;
        static int _datelength = 20;
        static int _addresslength = 255;
        static int _prefixlength = 3;
        static int _filenamelength = 255;
        

        #region Get Queries
        public static FullyObservableCollection<Models.AssetModel>  GetAssets()
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssets";
            try {                                
                oc.Connection = StaticClasses.GlobalClass.Conn;                
                OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                while (or.Read())
                {
                    _assets.Add(new Models.AssetModel
                    {
                        AssetID =  Convert.ToInt32(or["AssetID"]),
                        Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString(),
                        Label = (or["Label"] is DBNull) ? string.Empty : or["Label"].ToString(),
                        StatusID = (or["StatusID"] is DBNull) ? 0 : Convert.ToInt32(or["StatusID"]),
                        ParentAssetID = (or["ParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["ParentAssetID"])
                    });
                }
                or.Close();                
            }
            catch { }
            finally
            {
                try
                {
                    oc.Dispose();
                }
                catch { }
            }
            return _assets;                        
        }

        public static Models.AssetModel GetAsset(int _assetid)
        {
            Models.AssetModel _asset = new Models.AssetModel();
            //run Access query
            OleDbCommand oc = new OleDbCommand();            
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAsset";
            oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
            try {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                while (or.Read())
                {
                    _asset.AssetID = _assetid;
                    _asset.Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString();
                    _asset.ApplicationType = (or["ApplicationType"] is DBNull) ? string.Empty : or["ApplicationType"].ToString();
                    _asset.ManufacturerName = (or["ManufacturerName"] is DBNull) ? string.Empty : or["ManufacturerName"].ToString();
                    _asset.ModelNo = (or["ModelNo"] is DBNull) ? string.Empty : or["ModelNo"].ToString();
                    _asset.SerialNo = (or["SerialNo"] is DBNull) ? string.Empty : or["SerialNo"].ToString();
                    _asset.DateInstalled = (string.IsNullOrEmpty(or["DateInstalled"].ToString())) ? (DateTime?)null : Convert.ToDateTime(or["DateInstalled"].ToString());
                    _asset.PurchasePrice = (or["PurchasePrice"] is DBNull) ? 0 : Convert.ToDecimal(or["PurchasePrice"]);
                    _asset.Comments = (or["Comments"] is DBNull) ? string.Empty : or["Comments"].ToString();
                    _asset.ChemicalsUsed = (or["ChemicalsUsed"] is DBNull) ? string.Empty : or["ChemicalsUsed"].ToString();
                    _asset.Label = (or["Label"] is DBNull) ? string.Empty : or["Label"].ToString();
                    _asset.DatePurchased = (string.IsNullOrEmpty(or["DatePurchased"].ToString())) ? (DateTime?)null : Convert.ToDateTime(or["DatePurchased"].ToString());
                    _asset.PONumber = (or["PONumber"] is DBNull) ? string.Empty : or["PONumber"].ToString();
                    _asset.AssetTypeID = (or["TypeID"] is DBNull) ? 0 : Convert.ToInt32(or["TypeID"]);
                    _asset.AssetGroupID = (or["GroupID"] is DBNull) ? 0 : Convert.ToInt32(or["GroupID"]);
                    _asset.CustomerID = (or["CustomerID"] is DBNull) ? 0 : Convert.ToInt32(or["CustomerID"]);
                    _asset.SalesDivisionID = (or["SalesDivisionID"] is DBNull) ? 0 : Convert.ToInt32(or["SalesDivisionID"]);
                    _asset.AssetAreaID = (or["PrefixID"] is DBNull) ? 0 : Convert.ToInt32(or["PrefixID"]);
                    _asset.StatusID = (or["StatusID"] is DBNull) ? 0 : Convert.ToInt32(or["StatusID"]);                                   
                    _asset.ParentAssetID = (or["ParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["ParentAssetID"]);
                    _asset.LastAuditDate = (string.IsNullOrEmpty(or["LastAuditDate"].ToString())) ? (DateTime?)null : Convert.ToDateTime(or["LastAuditDate"].ToString());
                    _asset.Dimensions = (or["Dimensions"] is DBNull) ? string.Empty : or["Dimensions"].ToString();
                    _asset.SupplierName = (or["SupplierName"] is DBNull) ? string.Empty : or["SupplierName"].ToString();
                }
                or.Close();                
            }
            catch { }
            finally
            {
                try
                {
                    oc.Dispose();
                }
                catch { }
            }
            return _asset;
        }

        public static FullyObservableCollection<Models.AssetModel> GetChildAssets(int _parentassetid)
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetChildAssets";
            oc.Parameters.Add("@_parentid", OleDbType.Integer).Value = _parentassetid;

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(new Models.AssetModel
                {
                    AssetID =  Convert.ToInt32(or["AssetID"]),
                    Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString(),
                    Label = (or["Label"] is DBNull) ? string.Empty : or["Label"].ToString(),
                    ParentAssetID = _parentassetid                                         
                });
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }
        

        public static FullyObservableCollection<Models.AssetSubItem> GetSubAssets(int _parentassetid)
        {
            FullyObservableCollection<Models.AssetSubItem> _assets = new FullyObservableCollection<Models.AssetSubItem>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetSubAssets";
            oc.Parameters.Add("@_parentid", OleDbType.Integer).Value = _parentassetid;

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(new Models.AssetSubItem
                {
                    ID =  Convert.ToInt32(or["AssetID"]),
                    Label = (or["Label"] is DBNull) ? string.Empty : or["Label"].ToString(),
                    TypeID = (or["TypeID"] is DBNull) ? 0 : Convert.ToInt32(or["TypeID"]),
                    Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }


        public static FullyObservableCollection<Models.AssetModel> GetCustomerChildAssets(int _customer)
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetCustomerChildAssets";
            oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customer;

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(new Models.AssetModel
                {
                    AssetID =  Convert.ToInt32(or["AssetID"]),
                    Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString(),
                    Label = (or["Label"] is DBNull) ? string.Empty : or["Label"].ToString(),
                    ParentAssetID = (or["ParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["ParentAssetID"])
                });
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }

        public static string GetParentLabel(int _parentassetid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            string label = string.Empty;
            try
            { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "GetParentAssetLabel";
                oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _parentassetid;
                label = (string)oc.ExecuteScalar();

                oc.Connection.Close();             
            }
            catch { }
            finally
            {
                try
                {
                    oc.Dispose();
                }
                catch { }
            }
            return label;
        }

        public static FullyObservableCollection<Models.CustomerModel> GetCustomers()
        {
            FullyObservableCollection<Models.CustomerModel> _customers = new FullyObservableCollection<Models.CustomerModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetCustomers";
           
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _customers.Add(new Models.CustomerModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    CustomerNumber = (or["CustomerNumber"] is DBNull) ? string.Empty : or["CustomerNumber"].ToString(),
                    CustomerName = (or["CustomerName"] is DBNull) ? string.Empty : or["CustomerName"].ToString(),
                    Location = (or["Location"] is DBNull) ? string.Empty : or["Location"].ToString(),
                    IconFileName = (or["IconFile"] is DBNull) ? string.Empty : or["IconFile"].ToString(),
                    CountryID = (or["CountryID"] is DBNull) ? 0 : Convert.ToInt32(or["CountryID"]),
                    CorporationID = (or["CorporationID"] is DBNull) ? 0 : Convert.ToInt32(or["CorporationID"]),
                    Default = (or["Default"] is DBNull) ? false : Convert.ToBoolean( or["Default"])
                });
            }
            or.Close();
            oc.Dispose();
            return _customers;
        }        

        public static FullyObservableCollection<Models.AssociateModel> GetAssociates()
        {
            FullyObservableCollection<Models.AssociateModel> _associates = new FullyObservableCollection<Models.AssociateModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssociates";
         
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _associates.Add(new Models.AssociateModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    AssociateName = (or["AssociateName"] is DBNull) ? string.Empty : or["AssociateName"].ToString(),
                    Manager = (or["Administrator"] is DBNull) ? false : Convert.ToBoolean(or["Administrator"]),
                    LoginName = (or["LoginName"] is DBNull) ? string.Empty : or["LoginName"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _associates;
        }

        public static Models.AssociateModel GetAssociateNameFromLogin(string _loginname)
        {
            Models.AssociateModel _associatename = new Models.AssociateModel();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssociateNameFromLogin";
            oc.Parameters.Add("@_loginName", OleDbType.VarChar, 50).Value = _loginname;

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _associatename.AssociateName = (or["AssociateName"] is DBNull) ? string.Empty : or["AssociateName"].ToString();
                _associatename.Manager = (or["Administrator"] is DBNull) ? false : Convert.ToBoolean(or["Administrator"]);
            }
            or.Close();
            oc.Dispose();
            return _associatename;
        }

        
        public static Models.AdministratorUserModel GetAdministratorNameFromUserLogin(string _loginname)
        {
            Models.AdministratorUserModel _administrator = new Models.AdministratorUserModel();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAdministratorNameFromUserLogin";
            oc.Parameters.Add("@_loginName", OleDbType.VarChar, 50).Value = _loginname;

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _administrator.AdministratorName = (or["AdministratorName"] is DBNull) ? string.Empty : or["AdministratorName"].ToString();
            }
            or.Close();
            oc.Dispose();
            return _administrator;
        }

        public static FullyObservableCollection<Models.CountryModel> GetCountries()
        {
            FullyObservableCollection<Models.CountryModel> _countries = new FullyObservableCollection<Models.CountryModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetCountries";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _countries.Add(new Models.CountryModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    CountryName = (or["CountryName"] is DBNull) ? string.Empty : or["CountryName"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _countries;
        }

                
        public static FullyObservableCollection<Models.SalesDivisionModel> GetSalesDivisions()
        {
            FullyObservableCollection<Models.SalesDivisionModel> _salesdivisions = new FullyObservableCollection<Models.SalesDivisionModel>();         
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetSalesDivisions";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _salesdivisions.Add(new Models.SalesDivisionModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    SalesDivisionName = (or["SalesDivisionName"] is DBNull) ? string.Empty : or["SalesDivisionName"].ToString()
                    
                });
            }
            or.Close();
            oc.Dispose();
            return _salesdivisions;
        }

        public static FullyObservableCollection<Models.AssetAreaModel> GetAssetAreas()
        {
            FullyObservableCollection<Models.AssetAreaModel> _assetareas = new FullyObservableCollection<Models.AssetAreaModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetAreas";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assetareas.Add(new Models.AssetAreaModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    Prefix = (or["Prefix"] is DBNull) ? string.Empty : or["Prefix"].ToString(),
                    Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString(),
                    Default = (or["Default"] is DBNull) ? false : Convert.ToBoolean(or["Default"].ToString())
                    
                });
            }
            or.Close();
            oc.Dispose();
            return _assetareas;
        }

        public static FullyObservableCollection<Models.AssetGroupModel> GetAssetGroups()
        {
            FullyObservableCollection<Models.AssetGroupModel> _assetgroups = new FullyObservableCollection<Models.AssetGroupModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetGroups";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assetgroups.Add(new Models.AssetGroupModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    AssetAreaID = (or["AssetPrefixID"] is DBNull) ? 0 : Convert.ToInt32(or["AssetPrefixID"]),
                    Description = (or["Group"] is DBNull) ? string.Empty : or["Group"].ToString(),
                    AssetGroupIDText = (string.IsNullOrEmpty(or["AssetGroupIDText"].ToString()) ? "00" : or["AssetGroupIDText"].ToString()),
                    CanBeParent = (or["CanBeParent"] is DBNull) ? false : Convert.ToBoolean(or["CanBeParent"].ToString())
                });
            }
            or.Close();
            oc.Dispose();
            return _assetgroups;
        }

        public static FullyObservableCollection<Models.AssetTypeModel> GetAssetTypes()
        {
            FullyObservableCollection<Models.AssetTypeModel> _assettypes = new FullyObservableCollection<Models.AssetTypeModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetTypes";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assettypes.Add(new Models.AssetTypeModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    AssetGroupID = (or["AssetGroupID"] is DBNull) ? 0 : Convert.ToInt32(or["AssetGroupID"]),
                    Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString()                    
                });
            }
            or.Close();
            oc.Dispose();
            return _assettypes;
        }

        //public static FullyObservableCollection<Models.AssetActivityTypeModel> GetAssetActivityTypes()
        //{
        //    FullyObservableCollection<Models.AssetActivityTypeModel> _assetactivitytypes = new FullyObservableCollection<Models.AssetActivityTypeModel>();
        //    //run Access query
        //    OleDbCommand oc = new OleDbCommand();
        //    oc.Connection = StaticClasses.GlobalClass.Conn;
        //    oc.CommandType = CommandType.StoredProcedure;
        //    oc.CommandText = "GetAssetActivityTypes";

        //    OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //    while (or.Read())
        //    {
        //        _assetactivitytypes.Add(new Models.AssetActivityTypeModel
        //        {
        //            ID = Convert.ToInt32(or["ID"]),
        //            Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString()
        //        });
        //    }
        //    or.Close();
        //    oc.Dispose();
        //    return _assetactivitytypes;
        //}

        public static FullyObservableCollection<Models.AssetGroupSpecificationValuesModel> GetAssetGroupSpecificationValues(int _assetid, int _assetgroupid)
        {
            FullyObservableCollection<Models.AssetGroupSpecificationValuesModel> _assetspecifications = new FullyObservableCollection<Models.AssetGroupSpecificationValuesModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetGroupSpecificationValues";
            oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
            oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _assetgroupid;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assetspecifications.Add(new Models.AssetGroupSpecificationValuesModel
                {
                    AssetID = _assetid,
                    AssetGroupID = _assetgroupid,
                    ID = Convert.ToInt32(or["ID"]),
                    AssetGroupSpecificationID = Convert.ToInt32(or["AssetGroupSpecificationID"]),
                    SpecificationValue = (or["SpecificationValue"] is DBNull) ? string.Empty : or["SpecificationValue"].ToString()                      
                });
            }
            or.Close();
            oc.Dispose();
            return _assetspecifications;
        }

        public static FullyObservableCollection<Models.AssetSpecificationModel> GetSpecifications()
        {
            FullyObservableCollection<Models.AssetSpecificationModel> _assetspecifications = new FullyObservableCollection<Models.AssetSpecificationModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetSpecifications";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assetspecifications.Add(new Models.AssetSpecificationModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    SpecificationName = (or["SpecificationName"] is DBNull) ? string.Empty : or["SpecificationName"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _assetspecifications;
        }

        public static FullyObservableCollection<Models.MaintenanceRecordModel> GetMaintenanceRecords(int _assetid)
        {
            FullyObservableCollection<Models.MaintenanceRecordModel> _maintenancerecords = new FullyObservableCollection<Models.MaintenanceRecordModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetMaintenanceRecords";
            oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _maintenancerecords.Add(new Models.MaintenanceRecordModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    MaintenanceDate = (string.IsNullOrEmpty( or["MaintenanceDate"].ToString())) ? (DateTime?)null : Convert.ToDateTime(or["MaintenanceDate"].ToString()),
                    Description = (string.IsNullOrEmpty(or["Description"].ToString()) ? string.Empty : or["Description"].ToString()),
                    Cost = (or["Cost"] is DBNull) ? 0 : Convert.ToDecimal(or["Cost"].ToString()),
                    MaintainedBy = (or["MaintainedBy"] is DBNull) ? string.Empty : or["MaintainedBy"].ToString(),
                    AssetID = _assetid

                });
            }
            or.Close();
            oc.Dispose();
            return _maintenancerecords;
        }

        public static FullyObservableCollection<Models.MaintenanceTypeModel> GetMaintenanceTypes()
        {
            FullyObservableCollection<Models.MaintenanceTypeModel> _maintenancetypes = new FullyObservableCollection<Models.MaintenanceTypeModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetMaintenanceTypes";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _maintenancetypes.Add(new Models.MaintenanceTypeModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    MaintenanceType = (string.IsNullOrEmpty(or["Description"].ToString())) ? string.Empty : or["AssetID"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _maintenancetypes;
        }

        public static FullyObservableCollection<Models.OperatingCompanyModel> GetOperatingCompanies()
        {
            FullyObservableCollection<Models.OperatingCompanyModel> _operatingcompanies = new FullyObservableCollection<Models.OperatingCompanyModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetOperatingCompany";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _operatingcompanies.Add(new Models.OperatingCompanyModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    OperatingCompany = (string.IsNullOrEmpty(or["OperatingCompany"].ToString())) ? string.Empty : or["OperatingCompany"].ToString(),
                    Default = (or["Default"] is DBNull) ? false : Convert.ToBoolean(or["Default"].ToString())
                });
            }
            or.Close();
            oc.Dispose();
            return _operatingcompanies;
        }

        public static FullyObservableCollection<Models.CorporationModel> GetCorporations()
        {
            FullyObservableCollection<Models.CorporationModel> _corporations = new FullyObservableCollection<Models.CorporationModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetCorporations";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _corporations.Add(new Models.CorporationModel
                {
                    ID = Convert.ToInt32(or["ID"]),
                    CorporationName = (string.IsNullOrEmpty(or["CorporationName"].ToString())) ? string.Empty : or["CorporationName"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _corporations;
        }


        public static FullyObservableCollection<Models.StatusModel> GetStatuses()
        {
            FullyObservableCollection<Models.StatusModel> _statuses = new FullyObservableCollection<Models.StatusModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetStatuses";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _statuses.Add(new Models.StatusModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    Status = (string.IsNullOrEmpty(or["Status"].ToString())) ? string.Empty : or["Status"].ToString(),
                    Default = (or["Default"] is DBNull) ? false : Convert.ToBoolean(or["Default"].ToString())
                });
            }
            or.Close();
            oc.Dispose();
            return _statuses;
        }

        public static FullyObservableCollection<Models.AssetMovementModel> GetAssetMovements(int _assetid)
        {
            FullyObservableCollection<Models.AssetMovementModel> _movements = new FullyObservableCollection<Models.AssetMovementModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetMovements";
            oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _movements.Add(new Models.AssetMovementModel
                {
                    ID = Convert.ToInt32(or["ID"]),
                    ActivityCodeID = (or["ActivityCodeID"] is DBNull) ? 0 : Convert.ToInt32(or["ActivityCodeID"]),
                    DateMoved = (or["DateMoved"] is DBNull) ? (DateTime?)null : Convert.ToDateTime(or["DateMoved"]),
                    SourceCustomerID = (or["SourceCustomerID"] is DBNull) ? 0 : Convert.ToInt32(or["SourceCustomerID"]),
                    SourceParentAssetID = (or["SourceParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["SourceParentAssetID"]),
                    DestinationCustomerID = (or["DestinationCustomerID"] is DBNull) ? 0 : Convert.ToInt32(or["DestinationCustomerID"]),
                    DestinationParentAssetID = (or["DestinationParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["DestinationParentAssetID"])
                });
            }
            or.Close();
            oc.Dispose();
            return _movements;
        }
      
        public static FullyObservableCollection<Models.PhotoModel> GetAssetPhotos(int _assetid)
        {
            FullyObservableCollection<Models.PhotoModel> _photos = new FullyObservableCollection<Models.PhotoModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetPhotos";
            oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _photos.Add(new Models.PhotoModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    PhotoFileName = (string.IsNullOrEmpty(or["PhotoFileName"].ToString())) ? string.Empty : or["PhotoFileName"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _photos;
        }

        public static FullyObservableCollection<Models.ConsumableModel> GetAssetConsumables(int _assetid, bool _selectused)
        {
            FullyObservableCollection<Models.ConsumableModel> _consumables = new FullyObservableCollection<Models.ConsumableModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetConsumables";
            oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
            oc.Parameters.Add("@_used", OleDbType.Boolean).Value = _selectused;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _consumables.Add(new Models.ConsumableModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    AssetID = _assetid,
                    Description = (string.IsNullOrEmpty(or["Description"].ToString())) ? string.Empty : or["Description"].ToString(),
                    Cost = (or["Cost"] is DBNull) ? 0 : Convert.ToDecimal(or["Cost"]),
                    DatePurchased = (string.IsNullOrEmpty(or["DatePurchased"].ToString())) ? (DateTime?)null : Convert.ToDateTime(or["DatePurchased"].ToString()),
                    OrderReference = (string.IsNullOrEmpty(or["OrderReference"].ToString())) ? string.Empty : or["OrderReference"].ToString(),
                    Supplier = (string.IsNullOrEmpty(or["Supplier"].ToString())) ? string.Empty : or["Supplier"].ToString(),
                    Used = (or["Used"] is DBNull) ? false : Convert.ToBoolean(or["Used"].ToString())
            });
            }
            or.Close();
            oc.Dispose();
            return _consumables;
        }

        public static FullyObservableCollection<Models.AssetSummaryModel> GetAvailableAssets(int _statusid, int _assetid)
        {
            FullyObservableCollection<Models.AssetSummaryModel> _availableassets = new FullyObservableCollection<Models.AssetSummaryModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAvailableAssets";
            oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _statusid;
            oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _availableassets.Add(new Models.AssetSummaryModel
                {
                    ID =  Convert.ToInt32(or["AssetID"]),
                    Label = (string.IsNullOrEmpty(or["Label"].ToString())) ? string.Empty : or["Label"].ToString(),
                    Description = (string.IsNullOrEmpty(or["Description"].ToString())) ? string.Empty : or["Description"].ToString(),
                    Customer = (string.IsNullOrEmpty(or["CustomerName"].ToString())) ? string.Empty : or["CustomerName"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _availableassets;
        }


        public static Collection<ViewModels.CircularError> GetCircularAssets()
        {
            Collection<ViewModels.CircularError> _circularassets = new Collection<ViewModels.CircularError>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetCircularAssets";
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            ViewModels.CircularError _newasset;
            while (or.Read())
            {
                _newasset = new ViewModels.CircularError();
                _newasset.AssetID = Convert.ToInt32(or["AssetID"]);
                _newasset.ParentAssetID = (or["ParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["ParentAssetID"]);
                _circularassets.Add(_newasset);
            }
            or.Close();
            oc.Dispose();
            return _circularassets;
        }




        public static FullyObservableCollection<Models.AssetGroupSpecificationModel> GetAssetGroupSpecifications(int _groupid)
        {
            FullyObservableCollection<Models.AssetGroupSpecificationModel> _specifications = new FullyObservableCollection<Models.AssetGroupSpecificationModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetGroupSpecifications";
            oc.Parameters.Add("@_groupid", OleDbType.Integer).Value = _groupid;

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _specifications.Add(new Models.AssetGroupSpecificationModel
                {
                    ID = Convert.ToInt32(or["ID"]),
                    AssetGroupID = _groupid,
                    SpecificationNameID  = Convert.ToInt32(or["SpecificationNameID"]),
                    PropertyUnitID = Convert.ToInt32(or["PropertyUnitID"]),
                    MeasurementUnitID = Convert.ToInt32(or["MeasurementUnitID"]),
                    SpecificationOptions = (string.IsNullOrEmpty(or["SpecificationOptions"].ToString())) ? string.Empty : or["SpecificationOptions"].ToString()

                });
            }
            or.Close();
            oc.Dispose();
            return _specifications;
        }

        public static FullyObservableCollection<Models.AssetGroupSpecificationModel> GetAllAssetGroupSpecifications()
        {
            FullyObservableCollection<Models.AssetGroupSpecificationModel> _specifications = new FullyObservableCollection<Models.AssetGroupSpecificationModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAllAssetGroupSpecifications";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _specifications.Add(new Models.AssetGroupSpecificationModel
                {
                    ID = Convert.ToInt32(or["ID"]),
                    AssetGroupID = Convert.ToInt32(or["AssetGroupID"]),
                    SpecificationNameID = Convert.ToInt32(or["SpecificationNameID"]),
                    PropertyUnitID = Convert.ToInt32(or["PropertyUnitID"]),
                    MeasurementUnitID = Convert.ToInt32(or["MeasurementUnitID"]),
                    SpecificationOptions = (string.IsNullOrEmpty(or["SpecificationOptions"].ToString())) ? string.Empty : or["SpecificationOptions"].ToString()

                });
            }
            or.Close();
            oc.Dispose();
            return _specifications;
        }

        public static FullyObservableCollection<Models.AssetGroupSpecDisplayDataModel> GetAssetGroupSpecificationsForDisplay(int _groupid)
        {
            FullyObservableCollection<Models.AssetGroupSpecDisplayDataModel> _specifications = new FullyObservableCollection<Models.AssetGroupSpecDisplayDataModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetGroupSpecificationsForDisplay";
            oc.Parameters.Add("@_groupid", OleDbType.Integer).Value = _groupid;

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _specifications.Add(new Models.AssetGroupSpecDisplayDataModel
                {
                    AssetGroupSpecificationID = Convert.ToInt32(or["ID"]),
                    AssetGroupID = _groupid,
                    SpecificationName = (string.IsNullOrEmpty(or["SpecificationName"].ToString())) ? string.Empty : or["SpecificationName"].ToString(),
                    SpecificationPropertyName = (string.IsNullOrEmpty(or["PropertyUnit"].ToString())) ? string.Empty : or["PropertyUnit"].ToString(),
                    SpecificationOptions = (string.IsNullOrEmpty(or["SpecificationOptions"].ToString())) ? string.Empty : or["SpecificationOptions"].ToString(),
                    DataTypeID = Convert.ToInt32(or["MeasurementUnitID"])                                         
                });
            }
            or.Close();
            oc.Dispose();
            return _specifications;
        }

        public static FullyObservableCollection<Models.SearchFieldModel> GetSearchFields()
        {
            FullyObservableCollection<Models.SearchFieldModel> _searchfields = new FullyObservableCollection<Models.SearchFieldModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetSearchFields";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _searchfields.Add(new Models.SearchFieldModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    Label = (string.IsNullOrEmpty(or["Label"].ToString())) ? string.Empty : or["Label"].ToString(),
                    QueryName = (string.IsNullOrEmpty(or["QueryName"].ToString())) ? string.Empty : or["QueryName"].ToString(),
                    SearchField = (string.IsNullOrEmpty(or["SearchField"].ToString())) ? string.Empty : or["SearchField"].ToString(),
                    SearchFieldType = (string.IsNullOrEmpty(or["SearchFieldType"].ToString())) ? string.Empty : or["SearchFieldType"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _searchfields;
        }
        
         public static Collection<Models.ReportModel> GetReports()
         {
            Collection<Models.ReportModel> _reports = new Collection<Models.ReportModel>();

            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetReports";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _reports.Add(new Models.ReportModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    Header = (string.IsNullOrEmpty(or["Header"].ToString())) ? string.Empty : or["Header"].ToString(),
                    Parameter = (string.IsNullOrEmpty(or["Parameter"].ToString())) ? string.Empty : or["Parameter"].ToString(),
                    Tooltip = (string.IsNullOrEmpty(or["Tooltip"].ToString())) ? string.Empty : or["Tooltip"].ToString(),
                    IconfileName = (string.IsNullOrEmpty(or["IconfileName"].ToString())) ? string.Empty : or["IconfileName"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _reports;
        }

        public static FullyObservableCollection<Models.MeasurementUnitModel> GetMeasurementUnits()
        {
            FullyObservableCollection<Models.MeasurementUnitModel> _measurementunits = new FullyObservableCollection<Models.MeasurementUnitModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetMeasurementUnits";
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _measurementunits.Add(new Models.MeasurementUnitModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    MeasurementUnit = (string.IsNullOrEmpty(or["MeasurementUnit"].ToString())) ? string.Empty : or["MeasurementUnit"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _measurementunits;
        }


        public static FullyObservableCollection<Models.SpecificationPropertyModel> GetSpecificationProperties()
        {
            FullyObservableCollection<Models.SpecificationPropertyModel> _properties = new FullyObservableCollection<Models.SpecificationPropertyModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetSpecificationProperties";
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _properties.Add(new Models.SpecificationPropertyModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    PropertyUnit = (string.IsNullOrEmpty(or["PropertyUnit"].ToString())) ? string.Empty : or["PropertyUnit"].ToString()
                });
            }
            or.Close();
            oc.Dispose();
            return _properties;
        }

        public static DataTable GetExcelReportData(string _excelreport)
        {
            DataTable _reportdata = new DataTable(_excelreport);
            OleDbCommand oc = new OleDbCommand();
            try {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = _excelreport;
                OleDbDataAdapter da = new OleDbDataAdapter(oc);
                da.Fill(_reportdata);
                da.Dispose();
                          
            }
            catch   {  }
            finally
            {
                try
                {
                    
oc.Connection.Close(); 
oc.Dispose();
                }
                catch { }
            }
            return _reportdata;
        }

        public static int CountCustomerChildAssets(int _customerid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            int count = 0;
            try {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "CountCustomerChildAssets";
                oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customerid;
                count = (int)oc.ExecuteScalar();
                              
             }
            catch { }
            finally
            {
                try
                {
                    
oc.Connection.Close(); 
oc.Dispose();
                }
                catch { }
            }
            return count;
        }

        public static int CountChildAssets(int _assetid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            int count = 0;
            try {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "CountChildAssets";
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                count = (int)oc.ExecuteScalar();
            }            
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                    
                }
                catch { }
            }
            return count;
        }

       
        public static Collection<int> GetAssetSearch(string _queryname, string _criteria)
        {
            Collection<int> _assets = new Collection<int>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = _queryname;
            oc.Parameters.Add("@_string", OleDbType.VarChar, _descriptionlength).Value = _criteria;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(Convert.ToInt32(or["AssetID"]));
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }

        public static Collection<int> GetAssetSearch(string _queryname, int _id)
        {
            Collection<int> _assets = new Collection<int>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = _queryname;
            oc.Parameters.Add("@_id", OleDbType.Integer).Value = _id;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(Convert.ToInt32(or["AssetID"]));
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }

        public static Collection<int> GetAssetSearch(string _queryname, string _criteria, int _specificationid)
        {
            Collection<int> _assets = new Collection<int>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = _queryname;
            oc.Parameters.Add("@_string", OleDbType.VarChar, _descriptionlength).Value = _criteria;
            oc.Parameters.Add("@_specificationid", OleDbType.Integer).Value = _specificationid;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(Convert.ToInt32(or["AssetID"]));
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }

        public static Collection<int> GetAssetSearch(string _queryname, DateTime? _startdate, DateTime? _enddate)
        {
            Collection<int> _assets = new Collection<int>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = _queryname;
            oc.Parameters.Add("@_startdate", OleDbType.Date).Value = _startdate;
            oc.Parameters.Add("@_enddate", OleDbType.Date).Value = _enddate;
            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(Convert.ToInt32(or["AssetID"]));
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }


        public static int CountUsedID(string _tableid, int _id)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            int count = 0;
            try
            {            
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "CountUsed" + _tableid;
                oc.Parameters.Add("@_id", OleDbType.Integer).Value = _id;            
                count =  (int)oc.ExecuteScalar();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
            return count;
        }

        public static FullyObservableCollection<Models.AssetModel> GetDuplicateAssetLabels()
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetDuplicateAssetLabels";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(new Models.AssetModel
                {
                     AssetID =  Convert.ToInt32(or["AssetID"]),
                     Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString(),
                     Label = (or["Label"] is DBNull) ? string.Empty : or["Label"].ToString(),
                     ParentAssetID = (or["ParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["ParentAssetID"])                 
                });
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }

        public static FullyObservableCollection<Models.AssetModel> GetAssetsWithNoLocationInformation()
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAssetsWithNoLocationInformation";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(new Models.AssetModel
                {
                    AssetID =  Convert.ToInt32(or["AssetID"]),
                    Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString(),
                    Label = (or["Label"] is DBNull) ? string.Empty : or["Label"].ToString(),
                    ParentAssetID = (or["ParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["ParentAssetID"])
                });
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }
       
        public static FullyObservableCollection<Models.AssetSummaryModel> GetDeletedAssets()
        {
            FullyObservableCollection<Models.AssetSummaryModel> _assets = new FullyObservableCollection<Models.AssetSummaryModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetDeletedAssets";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _assets.Add(new Models.AssetSummaryModel
                {
                    ID =  Convert.ToInt32(or["AssetID"]),
                    Description = (or["Description"] is DBNull) ? string.Empty : or["Description"].ToString(),
                    Label = (or["Label"] is DBNull) ? string.Empty : or["Label"].ToString(),
                    ParentAssetID = (or["ParentAssetID"] is DBNull) ? 0 : Convert.ToInt32(or["ParentAssetID"])
                });
            }
            or.Close();
            oc.Dispose();
            return _assets;
        }

        public static FullyObservableCollection<Models.AdministratorUserModel> GetAdministrators()
        {
            FullyObservableCollection<Models.AdministratorUserModel> _administrators = new FullyObservableCollection<Models.AdministratorUserModel>();
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            oc.Connection = StaticClasses.GlobalClass.Conn;
            oc.CommandType = CommandType.StoredProcedure;
            oc.CommandText = "GetAdministrators";

            OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
            while (or.Read())
            {
                _administrators.Add(new Models.AdministratorUserModel
                {
                    ID =  Convert.ToInt32(or["ID"]),
                    AdministratorName = (string.IsNullOrEmpty(or["AdministratorName"].ToString())) ? string.Empty : or["AdministratorName"].ToString(),
                    LoginName = (string.IsNullOrEmpty(or["UserLogin"].ToString())) ? string.Empty : or["UserLogin"].ToString()

                });
            }
            or.Close();
            oc.Dispose();
            return _administrators;
        }

        public static int GetIDFromGUID(string _query, string _guidstring)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            int newassetid = 0;
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = _query;
                oc.Parameters.Add("@_guidstring", OleDbType.VarChar, 255).Value = _guidstring;
                newassetid = (int)oc.ExecuteScalar();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
            return newassetid;
        }

        public static bool IsDuplicateLabel(string _label, int _assetid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            int count = 0;
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "CountMatchingAssetLabels";
                oc.Parameters.Add("@_label", OleDbType.VarChar,_descriptionlength).Value = _label;
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                count = (int)oc.ExecuteScalar();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
            return (count > 0);
        }



        #endregion

        #region Insert Queries
        //====================================================================================================================================
        //Insert queries

        public static void AddAsset(Models.AssetModel _asset)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddAsset";
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _asset.Description ?? string.Empty;
                oc.Parameters.Add("@_applicationtype", OleDbType.VarChar, _descriptionlength).Value = _asset.ApplicationType ?? string.Empty;
                oc.Parameters.Add("@_sitelocation", OleDbType.VarChar, _descriptionlength).Value = _asset.SiteLocation ?? string.Empty;
                oc.Parameters.Add("@_manufacturername", OleDbType.VarChar, _descriptionlength).Value = _asset.ManufacturerName ?? string.Empty;
                oc.Parameters.Add("@_modelno", OleDbType.VarChar, _descriptionlength).Value = _asset.ModelNo;
                oc.Parameters.Add("@_serialno", OleDbType.VarChar, _descriptionlength).Value = _asset.SerialNo;
                oc.Parameters.Add("@_dateinstalled", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_asset.DateInstalled);
                oc.Parameters.Add("@_purchaseprice", OleDbType.Decimal).Value = _asset.PurchasePrice;
                oc.Parameters.Add("@_comments", OleDbType.VarChar, _descriptionlength).Value = _asset.Comments ?? string.Empty;
                oc.Parameters.Add("@_chemicalsused", OleDbType.VarChar, _descriptionlength).Value = _asset.ChemicalsUsed ?? string.Empty;
                oc.Parameters.Add("@_label", OleDbType.VarChar, _descriptionlength).Value = _asset.Label ?? string.Empty;
                oc.Parameters.Add("@_datepurchased", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_asset.DatePurchased);
                oc.Parameters.Add("@_ponumber", OleDbType.VarChar, _descriptionlength).Value = _asset.PONumber ?? string.Empty;
                oc.Parameters.Add("@_assettypeid", OleDbType.Integer).Value = _asset.AssetTypeID;
                oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _asset.CustomerID;
                oc.Parameters.Add("@_salesdivisionid", OleDbType.Integer).Value = _asset.SalesDivisionID;
                oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _asset.StatusID;
                oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _asset.ParentAssetID;
                oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _asset.AssetGroupID;
                oc.Parameters.Add("@_prefixid", OleDbType.Integer).Value = _asset.AssetAreaID;
                oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _asset.OperatingCompanyID;
                oc.Parameters.Add("@_lastauditdate", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_asset.LastAuditDate);
                oc.Parameters.Add("@_dimensions", OleDbType.VarChar, _descriptionlength).Value = _asset.Dimensions ?? string.Empty;
                oc.Parameters.Add("@_suppliername", OleDbType.VarChar, _descriptionlength).Value = _asset.SupplierName ?? string.Empty;
                //// execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                _msg = null;
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddCustomer(Models.CustomerModel _customer)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddCustomer";
                oc.Parameters.Add("@_customernumber", OleDbType.VarChar, _namelength).Value = _customer.CustomerNumber;
                oc.Parameters.Add("@_customername", OleDbType.VarChar, _namelength).Value = _customer.CustomerName;
                oc.Parameters.Add("@_location", OleDbType.VarChar, 50).Value = _customer.Location;
                oc.Parameters.Add("@_countryid", OleDbType.Integer).Value = _customer.CountryID;
                oc.Parameters.Add("@_corporationid", OleDbType.Integer).Value = _customer.CorporationID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                _msg = null;
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddAssociate(Models.AssociateModel _associate)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddAssociate";
                oc.Parameters.Add("@_associatename", OleDbType.VarChar, _namelength).Value = _associate.AssociateName;
                oc.Parameters.Add("@_loginname", OleDbType.VarChar, _namelength).Value = _associate.LoginName;
                oc.Parameters.Add("@_manager", OleDbType.Boolean).Value = _associate.Manager;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddAdministrator(Models.AdministratorUserModel _administrator)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddAdministrator";
                oc.Parameters.Add("@_administratorname", OleDbType.VarChar, _namelength).Value = _administrator.AdministratorName;
                oc.Parameters.Add("@_loginname", OleDbType.VarChar, _namelength).Value = _administrator.LoginName;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddCountry(Models.CountryModel _country)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddCountry";
                oc.Parameters.Add("@_countryname", OleDbType.VarChar, _namelength).Value = _country.CountryName;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }
        
        public static void AddSalesDivision(Models.SalesDivisionModel _salesdivision)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddSalesDivision";
                oc.Parameters.Add("@_salesdivisionname", OleDbType.VarChar, _namelength).Value = _salesdivision.SalesDivisionName;
                oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _salesdivision.OperatingCompanyID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddMaintenanceRecord(Models.MaintenanceRecordModel _maintenancerecord)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddMaintenanceRecord";
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _maintenancerecord.AssetID;
                oc.Parameters.Add("@_maintenancedate", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_maintenancerecord.MaintenanceDate);
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _maintenancerecord.Description;
                oc.Parameters.Add("@_cost", OleDbType.Decimal).Value = _maintenancerecord.Cost;
                oc.Parameters.Add("@_maintainedby", OleDbType.VarChar, _namelength).Value = _maintenancerecord.MaintainedBy;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        //public static void AddAssetActivityType(Models.AssetActivityTypeModel _assetactivitytype)
        //{
        //    OleDbCommand oc = new OleDbCommand();
        //    try
        //    {
        //        //run Access query               
        //        oc.Connection = StaticClasses.GlobalClass.Conn;
        //        oc.CommandType = CommandType.StoredProcedure;
        //        oc.CommandText = "AddAssetActivityType";
        //        oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assetactivitytype.Description;
        //        // execute
        //        oc.ExecuteNonQuery();
        //    }
        //    catch (Exception e)
        //    {
        //        IMessageBoxService _msg = new MessageBoxService();
        //        _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        try
        //        {
        //            oc.Connection.Close();
        //            oc.Dispose();
        //        }
        //        catch { }
        //    }
        //}

        public static void AddAssetArea(Models.AssetAreaModel _assetarea)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddAssetArea";
                oc.Parameters.Add("@_prefix", OleDbType.VarChar, _prefixlength).Value = _assetarea.Prefix;
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assetarea.Description;
                oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _assetarea.Default;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddAssetGroup(Models.AssetGroupModel _assetgroup)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddAssetGroup";
                oc.Parameters.Add("@_assetprefixid", OleDbType.Integer).Value = _assetgroup.AssetAreaID;
                oc.Parameters.Add("@_group", OleDbType.VarChar, _namelength).Value = _assetgroup.Description;
                oc.Parameters.Add("@_assetgroupidtext", OleDbType.VarChar, 255).Value = _assetgroup.AssetGroupIDText;
                oc.Parameters.Add("@_canbeparent", OleDbType.Boolean).Value = _assetgroup.CanBeParent;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddAssetType(Models.AssetTypeModel _assettype)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddAssetType";
                oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _assettype.AssetGroupID;
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assettype.Description;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddAssetMovement(Models.AssetMovementModel _assetmovement)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddAssetMovement";
                oc.Parameters.Add("@_datemoved", OleDbType.VarChar,255).Value = ConvertMonthDateToString(_assetmovement.DateMoved);
                oc.Parameters.Add("@_activitycode", OleDbType.Integer).Value = _assetmovement.ActivityCodeID;
                //oc.Parameters.Add("@_sourcecustomer", OleDbType.Integer).Value = _assetmovement.SourceCustomerID;
                //oc.Parameters.Add("@_destinationcustomer", OleDbType.Integer).Value = _assetmovement.DestinationCustomerID;
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetmovement.AssetID;
                oc.Parameters.Add("@_sourceparentassetid", OleDbType.Integer).Value = _assetmovement.SourceAssetID;
                oc.Parameters.Add("@_destinationparentassetid", OleDbType.Integer).Value = _assetmovement.DestinationAssetID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddOperatingCompany(Models.OperatingCompanyModel _opco)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddOperatingCompany";
                oc.Parameters.Add("@_operatingcompany", OleDbType.VarChar, _namelength).Value = _opco.OperatingCompany;
                oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _opco.Default;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddCorporation(Models.CorporationModel _opco)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddOperatingCompany";
                oc.Parameters.Add("@_operatingcompany", OleDbType.VarChar, _namelength).Value = _opco.CorporationName;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddStatus(Models.StatusModel _status)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddStatus";
                oc.Parameters.Add("@_status", OleDbType.VarChar, _namelength).Value = _status.Status;
                oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _status.Default;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddPhoto(Models.PhotoModel _photo)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddPhoto";
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _photo.AssetID;
                oc.Parameters.Add("@_photofilename", OleDbType.VarChar, _filenamelength).Value = _photo.PhotoFileName ?? string.Empty;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddSpecification(Models.AssetSpecificationModel _spec)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddSpecification";
                oc.Parameters.Add("@_specification", OleDbType.VarChar, _namelength).Value = _spec.SpecificationName;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddMeasurementUnit(Models.MeasurementUnitModel _measurementunit)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddMeasurementUnit";
                oc.Parameters.Add("@_measurementunit", OleDbType.VarChar, _namelength).Value = _measurementunit.MeasurementUnit;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddSpecificationProperty(Models.SpecificationPropertyModel _specprop)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddSpecificationProperty";
                oc.Parameters.Add("@_specpropertyunit", OleDbType.VarChar, _namelength).Value = _specprop.PropertyUnit;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddConsumable(Models.ConsumableModel _consumable)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddConsumable";
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _consumable.AssetID;
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _consumable.Description ?? string.Empty;
                oc.Parameters.Add("@_cost", OleDbType.Decimal).Value = _consumable.Cost;
                oc.Parameters.Add("@_datepurchased", OleDbType.VarChar, _datelength).Value =ConvertMonthDateToString(_consumable.DatePurchased);
                oc.Parameters.Add("@_orderreference", OleDbType.VarChar, _namelength).Value = _consumable.OrderReference ?? string.Empty;
                oc.Parameters.Add("@_supplier", OleDbType.VarChar, _namelength).Value = _consumable.Supplier ?? string.Empty;
                oc.Parameters.Add("@_used", OleDbType.Boolean).Value = _consumable.Used;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        
        public static void AddAssetGroupSpecification(Models.AssetGroupSpecificationModel _specification)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "AddAssetGroupSpecification";
                oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _specification.AssetGroupID;
                oc.Parameters.Add("@_specificationnameid", OleDbType.Integer).Value = _specification.SpecificationNameID;
                oc.Parameters.Add("@_propertyunitid", OleDbType.Integer).Value = _specification.PropertyUnitID;
                oc.Parameters.Add("@_measurementunitid", OleDbType.Integer).Value = _specification.MeasurementUnitID;
                oc.Parameters.Add("@_specificationoptions", OleDbType.VarChar,255).Value = _specification.SpecificationOptions;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void AddGuid(string _query,  string _guidstring)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = _query;
                oc.Parameters.Add("@_guidstring", OleDbType.VarChar, 255).Value = _guidstring;
                // execute
                oc.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error during save operation\n" + e.Message.ToString(), "Save Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        #endregion

        #region Update Queries
        //====================================================================================================================================
        //Update queries

        public static void UpdateAsset(Models.AssetModel _asset)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAsset";
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _asset.Description ?? string.Empty;
                oc.Parameters.Add("@_applicationtype", OleDbType.VarChar, _descriptionlength).Value = _asset.ApplicationType ?? string.Empty;
                oc.Parameters.Add("@_sitelocation", OleDbType.VarChar, _descriptionlength).Value = _asset.SiteLocation ?? string.Empty;
                oc.Parameters.Add("@_manufacturername", OleDbType.VarChar, _descriptionlength).Value = _asset.ManufacturerName ?? string.Empty;
                oc.Parameters.Add("@_modelno", OleDbType.VarChar, _descriptionlength).Value = _asset.ModelNo ?? string.Empty;
                oc.Parameters.Add("@_serialno", OleDbType.VarChar, _descriptionlength).Value = _asset.SerialNo ?? string.Empty;
                oc.Parameters.Add("@_dateinstalled", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_asset.DateInstalled);
                oc.Parameters.Add("@_purchaseprice", OleDbType.Decimal).Value = _asset.PurchasePrice;
                oc.Parameters.Add("@_comments", OleDbType.VarChar, _descriptionlength).Value = _asset.Comments ?? string.Empty;
                oc.Parameters.Add("@_chemicalsused", OleDbType.VarChar, _descriptionlength).Value = _asset.ChemicalsUsed ?? string.Empty;
                oc.Parameters.Add("@_label", OleDbType.VarChar, _descriptionlength).Value = _asset.Label ?? string.Empty;
                oc.Parameters.Add("@_datepurchased", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_asset.DatePurchased);
                oc.Parameters.Add("@_ponumber", OleDbType.VarChar, _descriptionlength).Value = _asset.PONumber ?? string.Empty;
                oc.Parameters.Add("@_assettypeid", OleDbType.Integer).Value = _asset.AssetTypeID;
                oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _asset.CustomerID;
                oc.Parameters.Add("@_salesdivisionid", OleDbType.Integer).Value = _asset.SalesDivisionID;
                oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _asset.StatusID;
                oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _asset.ParentAssetID;
                oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _asset.AssetGroupID;
                oc.Parameters.Add("@_prefixid", OleDbType.Integer).Value = _asset.AssetAreaID;
                oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _asset.OperatingCompanyID;
                oc.Parameters.Add("@_lastauditdate", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_asset.LastAuditDate);
                oc.Parameters.Add("@_dimensions", OleDbType.VarChar, _descriptionlength).Value = _asset.Dimensions ?? string.Empty;
                oc.Parameters.Add("@_suppliername", OleDbType.VarChar, _descriptionlength).Value = _asset.SupplierName ?? string.Empty;
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _asset.AssetID;
                // execute
                oc.ExecuteNonQuery();
              
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateChildAsset(int _assetid, int _customerid, int _statusid)
        {
            OleDbCommand oc = new OleDbCommand();
            try
            {
                //run Access query                
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateChildAsset";
                oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customerid;
                oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _statusid;
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }



        public static void UpdateCustomer(Models.CustomerModel _customer)
        {          
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateCustomer";
                oc.Parameters.Add("@_customernumber", OleDbType.VarChar, _namelength).Value = _customer.CustomerNumber ?? string.Empty;
                oc.Parameters.Add("@_customername", OleDbType.VarChar, _namelength).Value = _customer.CustomerName ?? string.Empty;
                oc.Parameters.Add("@_location", OleDbType.VarChar, _namelength).Value = _customer.Location ;
                oc.Parameters.Add("@_countryid", OleDbType.Integer).Value = _customer.CountryID;
                oc.Parameters.Add("@_corporationid", OleDbType.Integer).Value = _customer.CorporationID;
                oc.Parameters.Add("@_iconfilename", OleDbType.VarChar, _filenamelength).Value = _customer.IconFileName ?? string.Empty;
                oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customer.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateAssociate(Models.AssociateModel _associate)
        {          
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAssociate";
                oc.Parameters.Add("@_associatename", OleDbType.VarChar, _namelength).Value = _associate.AssociateName ?? string.Empty;
                oc.Parameters.Add("@_loginname", OleDbType.VarChar, _namelength).Value = _associate.LoginName ?? string.Empty;
                oc.Parameters.Add("@_manager", OleDbType.Boolean).Value = _associate.Manager;
                oc.Parameters.Add("@_associateid", OleDbType.Integer).Value = _associate.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateAdministrator(Models.AdministratorUserModel _administrator)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAdministrator";
                oc.Parameters.Add("@_administratorname", OleDbType.VarChar, _namelength).Value = _administrator.AdministratorName ?? string.Empty;
                oc.Parameters.Add("@_loginname", OleDbType.VarChar, _namelength).Value = _administrator.LoginName ?? string.Empty;
                oc.Parameters.Add("@_administratorid", OleDbType.Integer).Value = _administrator.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateCountry(Models.CountryModel _country)
        {        
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateCountry";
                oc.Parameters.Add("@_countryname", OleDbType.VarChar, _namelength).Value = _country.CountryName ?? string.Empty;
                oc.Parameters.Add("@_countryid", OleDbType.Integer).Value = _country.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }
        
        public static void UpdateSalesDivision(Models.SalesDivisionModel _salesdivision)
        {            
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateSalesDivision";
                oc.Parameters.Add("@_salesdivisionname", OleDbType.VarChar, _namelength).Value = _salesdivision.SalesDivisionName ?? string.Empty;
                oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _salesdivision.OperatingCompanyID;
                oc.Parameters.Add("@_salesdivisionid", OleDbType.Integer).Value = _salesdivision.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateOperatingCompany(Models.OperatingCompanyModel _operatingcompany)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateOperatingCompany";
                oc.Parameters.Add("@_operatingcompanyname", OleDbType.VarChar, _namelength).Value = _operatingcompany.OperatingCompany ?? string.Empty;
                oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _operatingcompany.Default;
                oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _operatingcompany.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateStatus(Models.StatusModel _status)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateStatus";
                oc.Parameters.Add("@_status", OleDbType.VarChar, _namelength).Value = _status.Status ?? string.Empty;
                oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _status.Default;
                oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _status.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateMaintenanceRecord(Models.MaintenanceRecordModel _maintenancerecord)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateMaintenanceRecord";
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _maintenancerecord.AssetID;
                oc.Parameters.Add("@_maintenancedate", OleDbType.VarChar, _datelength).Value =ConvertMonthDateToString( _maintenancerecord.MaintenanceDate);
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _maintenancerecord.Description;
                oc.Parameters.Add("@_cost", OleDbType.Decimal).Value = _maintenancerecord.Cost;
                oc.Parameters.Add("@_maintainedby", OleDbType.VarChar, _namelength).Value = _maintenancerecord.MaintainedBy;
                oc.Parameters.Add("@_maintenancerecordid", OleDbType.Integer).Value = _maintenancerecord.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch
            { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateMaintenanceType(Models.MaintenanceTypeModel _maintenancetype)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateMaintenanceType";
                oc.Parameters.Add("@_maintenancetype", OleDbType.VarChar, _namelength).Value = _maintenancetype.MaintenanceType;
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _maintenancetype.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateAssetArea(Models.AssetAreaModel _assetarea)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAssetArea";
                oc.Parameters.Add("@_prefix", OleDbType.VarChar, _prefixlength).Value = _assetarea.Prefix;
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assetarea.Description;
                oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _assetarea.Default;
                oc.Parameters.Add("@_assetareaid", OleDbType.Integer).Value = _assetarea.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        //public static void UpdateAssetActivityType(Models.AssetActivityTypeModel _assetactivitytype)
        //{
        //    //run Access query
        //    OleDbCommand oc = new OleDbCommand();
        //    try { 
        //        oc.Connection = StaticClasses.GlobalClass.Conn;
        //        oc.CommandType = CommandType.StoredProcedure;
        //        oc.CommandText = "UpdateAssetActivityType";
        //        oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assetactivitytype.Description;
        //        oc.Parameters.Add("@_assetactivitytypeid", OleDbType.Integer).Value = _assetactivitytype.ID;
        //        // execute
        //        oc.ExecuteNonQuery();
        //    }
        //    catch { }
        //    finally
        //    {
        //        try
        //        {
        //            oc.Connection.Close();
        //            oc.Dispose();
        //        }
        //        catch { }
        //    }
        //}

        public static void UpdateAssetGroup(Models.AssetGroupModel _assetgroup)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAssetGroup";
                oc.Parameters.Add("@_assetprefix", OleDbType.Integer).Value = _assetgroup.AssetAreaID;
                oc.Parameters.Add("@_group", OleDbType.VarChar, _namelength).Value = _assetgroup.Description;
                oc.Parameters.Add("@_assetgrouptextid", OleDbType.VarChar, _prefixlength).Value = _assetgroup.AssetGroupIDText;
                oc.Parameters.Add("@_canbeparent", OleDbType.Boolean).Value = _assetgroup.CanBeParent;
                oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _assetgroup.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateAssetType(Models.AssetTypeModel _assettype)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAssetType";
                oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _assettype.AssetGroupID;
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assettype.Description;
                oc.Parameters.Add("@_assettypeid", OleDbType.Integer).Value = _assettype.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateCorporation(Models.CorporationModel _corporation)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateCorporation";
                oc.Parameters.Add("@_corporationname", OleDbType.VarChar, _namelength).Value = _corporation.CorporationName ?? string.Empty;
                oc.Parameters.Add("@_corporationid", OleDbType.Integer).Value =  _corporation.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateAssetMovement(Models.AssetMovementModel _assetmovement)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAssetMovement";
                oc.Parameters.Add("@_datemoved", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_assetmovement.DateMoved);
                oc.Parameters.Add("@_activitycode", OleDbType.Integer).Value = _assetmovement.ActivityCodeID;
                oc.Parameters.Add("@_sourcecustomerid", OleDbType.Integer).Value = _assetmovement.SourceCustomerID;
                oc.Parameters.Add("@_destinationcustomerid", OleDbType.Integer).Value = _assetmovement.DestinationCustomerID;
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetmovement.AssetID;
                oc.Parameters.Add("@_sourceparentassetid", OleDbType.Integer).Value = _assetmovement.SourceParentAssetID;
                oc.Parameters.Add("@_destinationparentassetid", OleDbType.Integer).Value = _assetmovement.DestinationParentAssetID;
                oc.Parameters.Add("@_assetmovementid", OleDbType.Integer).Value = _assetmovement.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateAssetPhoto(Models.PhotoModel _photo)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAssetPhoto";
                oc.Parameters.Add("@_photofilename", OleDbType.VarChar, _filenamelength).Value = _photo.PhotoFileName ?? string.Empty;
                oc.Parameters.Add("@_photoid", OleDbType.Integer).Value = _photo.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateSpecification(Models.AssetSpecificationModel _spec)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateSpecification";
                oc.Parameters.Add("@_specificationname", OleDbType.VarChar, _namelength).Value = _spec.SpecificationName;
                oc.Parameters.Add("@_specificationtypeid", OleDbType.Integer).Value = _spec.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateAssetGroupSpecification(Models.AssetGroupSpecificationModel _spec)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateAssetGroupSpecification";
                oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _spec.AssetGroupID;
                oc.Parameters.Add("@_specificationnameid", OleDbType.Integer).Value = _spec.SpecificationNameID;
                oc.Parameters.Add("@_propertyunitid", OleDbType.Integer).Value = _spec.PropertyUnitID;
                oc.Parameters.Add("@_measurementunitid", OleDbType.Integer).Value = _spec.MeasurementUnitID;
                oc.Parameters.Add("@_specificationoptions", OleDbType.VarChar, 255).Value = _spec.SpecificationOptions;
                oc.Parameters.Add("@_id", OleDbType.Integer).Value = _spec.ID;                
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateSpecificationRecord(Models.AssetGroupSpecDisplayDataModel _spec)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                
                oc.Parameters.Add("@_specificationvalue", OleDbType.VarChar, _descriptionlength).Value = _spec.SpecificationValue;
                if (_spec.ID != 0)
                {
                    oc.CommandText = "UpdateSpecificationRecord";
                    oc.Parameters.Add("@_specificationid", OleDbType.Integer).Value = _spec.ID;
                }
                else
                {
                    oc.CommandText = "AddSpecificationRecord";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _spec.AssetID;
                    oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _spec.AssetGroupID;
                    oc.Parameters.Add("@_groupspecificationid", OleDbType.Integer).Value = _spec.AssetGroupSpecificationID;
                }
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateSpecificationProperty(Models.SpecificationPropertyModel _specprop)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateSpecificationProperty";
                oc.Parameters.Add("@_specproperty", OleDbType.VarChar, _namelength).Value = _specprop.PropertyUnit;
                oc.Parameters.Add("@_specpropertyid", OleDbType.Integer).Value = _specprop.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateMeasurementUnit(Models.MeasurementUnitModel _measurementunit)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateMeasurementUnit";
                oc.Parameters.Add("@_measurementunit", OleDbType.VarChar, _namelength).Value = _measurementunit.MeasurementUnit;
                oc.Parameters.Add("@_measurementunitid", OleDbType.Integer).Value = _measurementunit.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }


        public static void UpdateConsumable(Models.ConsumableModel _consumable)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateConsumable";
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _consumable.AssetID;
                oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _consumable.Description ?? string.Empty;
                oc.Parameters.Add("@_cost", OleDbType.Decimal).Value = _consumable.Cost;
                oc.Parameters.Add("@_datepurchased", OleDbType.VarChar, _datelength).Value = ConvertMonthDateToString(_consumable.DatePurchased);
                oc.Parameters.Add("@_orderreference", OleDbType.VarChar, _namelength).Value = _consumable.OrderReference ?? string.Empty;
                oc.Parameters.Add("@_supplier", OleDbType.VarChar, _namelength).Value = _consumable.Supplier ?? string.Empty;
                oc.Parameters.Add("@_used", OleDbType.Boolean).Value = _consumable.Used;
                oc.Parameters.Add("@_consumableid", OleDbType.Integer).Value = _consumable.ID;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UpdateParentAssetID(int _assetid, int _parentassetid, int _customerid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UpdateParentAssetID";
                oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _parentassetid;
                oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customerid;
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void UnDeleteAsset(int _assetid, int _defaultcustomerid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "UnDeleteAsset";
                oc.Parameters.Add("@_defaultcustomerid", OleDbType.Integer).Value = _defaultcustomerid;
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }


        public static void SetDefaultCustomer(int _defaultcustomerid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "SetDefaultCustomer";
                oc.Parameters.Add("@_defaultcustomerid", OleDbType.Integer).Value = _defaultcustomerid;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void ResetDefaultCustomer()
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "ResetDefaultCustomer";
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }


        #endregion


        #region Delete Queries

        public static void RemoveChildAsset(int _childassetid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "RemoveChildAsset";
                oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _childassetid;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void SetConsumableToUsed(int _consumableid)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try { 
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = "SetConsumableToUsed";
                oc.Parameters.Add("@_consumableid", OleDbType.Integer).Value = _consumableid;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }

        public static void DeleteItem(int _id, DeleteSPName _sp)
        {
            //run Access query
            OleDbCommand oc = new OleDbCommand();
            try
            {
                oc.Connection = StaticClasses.GlobalClass.Conn;
                oc.CommandType = CommandType.StoredProcedure;
                oc.CommandText = _sp.ToString();
                oc.Parameters.Add("@_id", OleDbType.Integer).Value = _id;
                // execute
                oc.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                try
                {
                    oc.Connection.Close();
                    oc.Dispose();
                }
                catch { }
            }
        }



        #endregion


        #region Report Queries



        #endregion

        #region Helpers
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


        #endregion

    }



}

