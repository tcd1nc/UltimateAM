using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Runtime.CompilerServices;
using static AssetManager.GlobalClass;

namespace AssetManager
{
    static class DatabaseQueries //Access
    {
        static int _namelength = 50;
        static int _descriptionlength = 50;
        static int _prefixlength = 3;
        static int _filenamelength = 255;
        static int _baanidlength = 15;


        #region Get Queries
        
        public static Dictionary<int, string> GetAssetLabels()
        {
            Dictionary<int, string> _assets = new Dictionary<int, string>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetLabels";
                    oc.Connection = Conn;
                    
                    OleDbDataReader or = oc.ExecuteReader();
                    int _id = 0;
                    string _temp = string.Empty;
                    while (or.Read())
                    {
                        _id = Convert.ToInt32(or["AssetID"]);
                        _temp = or["Label"].ToString() ?? string.Empty;
                        _assets.Add(_id, _temp);
                    }
                    or.Close();                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }

        public static Models.AssetModel GetAsset(int _assetid)
        {
            Models.AssetModel _asset = new Models.AssetModel();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAsset";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    oc.Connection = Conn;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _asset.AssetID = _assetid;
                        _asset.Description = or["Description"].ToString() ?? string.Empty;
                        _asset.Category = or["Category"].ToString() ?? string.Empty;
                        _asset.ApplicationType = or["ApplicationType"].ToString() ?? string.Empty;
                        _asset.SiteLocation = or["SiteLocation"].ToString() ?? string.Empty;
                        _asset.ManufacturerName = or["ManufacturerName"].ToString() ?? string.Empty;
                        _asset.ModelNo = or["ModelNo"].ToString() ?? string.Empty;
                        _asset.SerialNo = or["SerialNo"].ToString() ?? string.Empty;
                        _asset.DateInstalled = ConvertDefaultDateToNull(or["DateInstalled"]);
                        _asset.PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]);
                        _asset.Comments = or["Comments"].ToString() ?? string.Empty;
                        _asset.ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty;
                        _asset.Label = or["Label"].ToString() ?? string.Empty;
                        _asset.DatePurchased = ConvertDefaultDateToNull(or["DatePurchased"]);
                        _asset.PONumber = or["PONumber"].ToString() ?? string.Empty;
                        _asset.AssetTypeID = ConvertObjToInt(or["TypeID"]);
                        _asset.AssetGroupID = ConvertObjToInt(or["GroupID"]);
                        _asset.CustomerID = ConvertObjToInt(or["CustomerID"]);
                        _asset.SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]);
                        _asset.AssetAreaID = ConvertObjToInt(or["PrefixID"]);
                        _asset.StatusID = ConvertObjToInt(or["StatusID"]);
                        _asset.ParentAssetID = ConvertObjToInt(or["ParentAssetID"]);
                        _asset.NextAuditDate = ConvertDefaultDateToNull(or["NextAuditDate"]);
                        _asset.Dimensions = or["Dimensions"].ToString() ?? string.Empty;
                        _asset.SupplierName = or["SupplierName"].ToString() ?? string.Empty;
                        _asset.BAANID = or["BAANID"].ToString() ?? string.Empty;
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _asset;
        }

        public static  FullyObservableCollection<Models.AssetModel> GetChildAssets(int _parentassetid)
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            Models.AssetModel _asset = null;
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetChildAssets";
                    oc.Parameters.Add("@_parentid", OleDbType.Integer).Value = _parentassetid;
                    //leave connection open  - it will be closed once the entire collection of assets is retrieved
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _asset = new Models.AssetModel();
                        _asset.AssetID = ConvertObjToInt(or["AssetID"]);
                        _asset.Description = or["Description"].ToString() ?? string.Empty;
                        _asset.Category = or["Category"].ToString() ?? string.Empty;
                        _asset.ApplicationType = or["ApplicationType"].ToString() ?? string.Empty;
                        _asset.SiteLocation = or["SiteLocation"].ToString() ?? string.Empty;
                        _asset.ManufacturerName = or["ManufacturerName"].ToString() ?? string.Empty;
                        _asset.ModelNo = or["ModelNo"].ToString() ?? string.Empty;
                        _asset.SerialNo = or["SerialNo"].ToString() ?? string.Empty;
                        _asset.DateInstalled = ConvertDefaultDateToNull(or["DateInstalled"]);
                        _asset.PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]);
                        _asset.Comments = or["Comments"].ToString() ?? string.Empty;
                        _asset.ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty;
                        _asset.Label = or["Label"].ToString() ?? string.Empty;
                        _asset.DatePurchased = ConvertDefaultDateToNull(or["DatePurchased"]);
                        _asset.PONumber = or["PONumber"].ToString() ?? string.Empty;
                        _asset.AssetTypeID = ConvertObjToInt(or["TypeID"]);
                        _asset.AssetGroupID = ConvertObjToInt(or["GroupID"]);
                        _asset.CustomerID = ConvertObjToInt(or["CustomerID"]);
                        _asset.SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]);
                        _asset.AssetAreaID = ConvertObjToInt(or["PrefixID"]);
                        _asset.StatusID = ConvertObjToInt(or["StatusID"]);
                        _asset.ParentAssetID = _parentassetid;
                        _asset.NextAuditDate = ConvertDefaultDateToNull(or["NextAuditDate"]);
                        _asset.Dimensions = or["Dimensions"].ToString() ?? string.Empty;
                        _asset.SupplierName = or["SupplierName"].ToString() ?? string.Empty;
                        _asset.BAANID = or["BAANID"].ToString() ?? string.Empty;
                        _assets.Add(_asset);
                    }
                    or.Close();                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }

        public static FullyObservableCollection<Models.AssetModel> GetDeletedChildAssets(int _parentassetid)
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetDeletedChildAssets";
                    oc.Parameters.Add("@_parentid", OleDbType.Integer).Value = _parentassetid;

                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assets.Add(new Models.AssetModel
                        {
                            AssetID = ConvertObjToInt(or["AssetID"]),
                            Description = or["Description"].ToString() ?? string.Empty,
                            Label = or["Label"].ToString() ?? string.Empty,
                            CustomerID = ConvertObjToInt(or["CustomerID"]),
                            ParentAssetID = _parentassetid
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }
        
        public static FullyObservableCollection<Models.AssetModel> GetCustomerChildAssets(int _customerid)
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            Models.AssetModel _asset = null;
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomerChildAssets";
                    oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customerid;

                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _asset = new Models.AssetModel();
                        _asset.AssetID = ConvertObjToInt(or["AssetID"]);
                        _asset.Description = or["Description"].ToString() ?? string.Empty;
                        _asset.Category = or["Category"].ToString() ?? string.Empty;
                        _asset.ApplicationType = or["ApplicationType"].ToString() ?? string.Empty;
                        _asset.SiteLocation = or["SiteLocation"].ToString() ?? string.Empty;
                        _asset.ManufacturerName = or["ManufacturerName"].ToString() ?? string.Empty;
                        _asset.ModelNo = or["ModelNo"].ToString() ?? string.Empty;
                        _asset.SerialNo = or["SerialNo"].ToString() ?? string.Empty;
                        _asset.DateInstalled = ConvertDefaultDateToNull(or["DateInstalled"]);
                        _asset.PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]);
                        _asset.Comments = or["Comments"].ToString() ?? string.Empty;
                        _asset.ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty;
                        _asset.Label = or["Label"].ToString() ?? string.Empty;
                        _asset.DatePurchased = ConvertDefaultDateToNull(or["DatePurchased"]);
                        _asset.PONumber = or["PONumber"].ToString() ?? string.Empty;
                        _asset.AssetTypeID = ConvertObjToInt(or["TypeID"]);
                        _asset.AssetGroupID = ConvertObjToInt(or["GroupID"]);
                        _asset.CustomerID = _customerid;
                        _asset.SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]);
                        _asset.AssetAreaID = ConvertObjToInt(or["PrefixID"]);
                        _asset.StatusID = ConvertObjToInt(or["StatusID"]);
                        _asset.ParentAssetID = ConvertObjToInt(or["ParentAssetID"]);
                        _asset.NextAuditDate = ConvertDefaultDateToNull(or["NextAuditDate"]);
                        _asset.Dimensions = or["Dimensions"].ToString() ?? string.Empty;
                        _asset.SupplierName = or["SupplierName"].ToString() ?? string.Empty;
                        _asset.BAANID = or["BAANID"].ToString() ?? string.Empty;
                        _assets.Add(_asset);
                    }
                    or.Close();
                }               
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }

        public static string GetParentLabel(int _parentassetid)
        {
            //run Access query
            string label = string.Empty;
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetParentAssetLabel";
                    oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _parentassetid;
                    label = (string)oc.ExecuteScalar();
                    
                }                     
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return label;
        }

        public static FullyObservableCollection<Models.CustomerModel> GetCustomers()
        {
            FullyObservableCollection<Models.CustomerModel> _customers = new FullyObservableCollection<Models.CustomerModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomers";
           
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _customers.Add(new Models.CustomerModel
                        {
                            ID = Convert.ToInt32(or["ID"]),
                            CustomerNumber = or["CustomerNumber"].ToString() ?? string.Empty,
                            CustomerName = or["CustomerName"].ToString() ?? string.Empty,
                            Location = or["Location"].ToString() ?? string.Empty,
                            IconFileName = or["IconFile"].ToString() ?? string.Empty,
                            CountryID = ConvertObjToInt(or["CountryID"]),
                            CorporationID = ConvertObjToInt(or["CorporationID"]),
                            CountryName = or["CountryName"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _customers;
        }        
               
        public static Models.AdministratorUserModel GetAdministratorNameFromUserLogin(string _loginname)
        {
            Models.AdministratorUserModel _administrator = new Models.AdministratorUserModel();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAdministratorNameFromUserLogin";
                    oc.Parameters.Add("@_loginName", OleDbType.VarChar, _namelength).Value = _loginname;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _administrator.AdministratorName = or["AdministratorName"].ToString() ?? string.Empty;
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _administrator;
        }

        public static FullyObservableCollection<Models.CountryModel> GetCountries()
        {
            FullyObservableCollection<Models.CountryModel> _countries = new FullyObservableCollection<Models.CountryModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCountries";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _countries.Add(new Models.CountryModel
                        {
                            ID =  Convert.ToInt32(or["ID"]),
                            CountryName = or["CountryName"].ToString() ?? string.Empty,
                            OperatingCompanyID = ConvertObjToInt(or["OperatingCompanyID"])
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _countries;
        }
                
        public static FullyObservableCollection<Models.SalesDivisionModel> GetSalesDivisions()
        {
            FullyObservableCollection<Models.SalesDivisionModel> _salesdivisions = new FullyObservableCollection<Models.SalesDivisionModel>();         
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSalesDivisions";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _salesdivisions.Add(new Models.SalesDivisionModel
                        {
                            ID =  Convert.ToInt32(or["ID"]),
                            SalesDivisionName = or["SalesDivisionName"].ToString() ?? string.Empty                    
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _salesdivisions;
        }

        public static FullyObservableCollection<Models.AssetAreaModel> GetAssetAreas()
        {
            FullyObservableCollection<Models.AssetAreaModel> _assetareas = new FullyObservableCollection<Models.AssetAreaModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetAreas";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assetareas.Add(new Models.AssetAreaModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Prefix = or["Prefix"].ToString() ?? string.Empty,
                            Description = or["Description"].ToString() ?? string.Empty,
                            Default = ConvertObjToBool(or["Default"])                    
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetareas;
        }

        public static FullyObservableCollection<Models.AssetGroupModel> GetAssetGroups()
        {
            FullyObservableCollection<Models.AssetGroupModel> _assetgroups = new FullyObservableCollection<Models.AssetGroupModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetGroups";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assetgroups.Add(new Models.AssetGroupModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetAreaID = ConvertObjToInt(or["AssetPrefixID"]),
                            Description = or["Group"].ToString() ?? string.Empty,
                            AssetGroupIDText = (string.IsNullOrEmpty(or["AssetGroupIDText"].ToString()) ? "00" : or["AssetGroupIDText"].ToString()),
                            CanBeParent = ConvertObjToBool(or["CanBeParent"])
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetgroups;
        }

        public static FullyObservableCollection<Models.AssetTypeModel> GetAssetTypes()
        {
            FullyObservableCollection<Models.AssetTypeModel> _assettypes = new FullyObservableCollection<Models.AssetTypeModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetTypes";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assettypes.Add(new Models.AssetTypeModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetGroupID = ConvertObjToInt(or["AssetGroupID"]),
                            Description = or["Description"].ToString() ?? string.Empty                    
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assettypes;
        }
        
        public static FullyObservableCollection<Models.AssetGroupSpecificationValuesModel> GetAssetGroupSpecificationValues(int _assetid, int _assetgroupid)
        {
            FullyObservableCollection<Models.AssetGroupSpecificationValuesModel> _assetspecifications = new FullyObservableCollection<Models.AssetGroupSpecificationValuesModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetGroupSpecificationValues";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _assetgroupid;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assetspecifications.Add(new Models.AssetGroupSpecificationValuesModel
                        {
                            AssetID = _assetid,
                            AssetGroupID = _assetgroupid,
                            ID = ConvertObjToInt(or["ID"]),
                            AssetGroupSpecificationID = ConvertObjToInt(or["AssetGroupSpecificationID"]),
                            SpecificationValue = or["SpecificationValue"].ToString() ?? string.Empty                      
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetspecifications;
        }

        public static FullyObservableCollection<Models.AssetSpecificationModel> GetSpecifications()
        {
            FullyObservableCollection<Models.AssetSpecificationModel> _assetspecifications = new FullyObservableCollection<Models.AssetSpecificationModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSpecifications";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assetspecifications.Add(new Models.AssetSpecificationModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            SpecificationName = or["SpecificationName"].ToString() ?? string.Empty,
                            MeasurementUnitID = ConvertObjToInt(or["MeasurementUnitID"])
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetspecifications;
        }

        public static FullyObservableCollection<Models.MaintenanceRecordModel> GetMaintenanceRecords(int _assetid)
        {
            FullyObservableCollection<Models.MaintenanceRecordModel> _maintenancerecords = new FullyObservableCollection<Models.MaintenanceRecordModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMaintenanceRecords";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _maintenancerecords.Add(new Models.MaintenanceRecordModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            MaintenanceDate = ConvertDefaultDateToNull(or["MaintenanceDate"]),
                            Description = or["Description"].ToString() ?? string.Empty,
                            Cost = ConvertObjToDecimal(or["Cost"]),
                            Completed = ConvertObjToBool( or["Completed"]),
                            MaintainedBy = or["MaintainedBy"].ToString() ?? string.Empty,
                            AssetID = _assetid
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _maintenancerecords;
        }

        public static FullyObservableCollection<Models.MaintenanceRecordModel> GetScheduledMaintenance(int _assetid)
        {
            FullyObservableCollection<Models.MaintenanceRecordModel> _scheduledmaintenance = new FullyObservableCollection<Models.MaintenanceRecordModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetScheduledMaintenance";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _scheduledmaintenance.Add(new Models.MaintenanceRecordModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            ScheduledMaintenanceDate = ConvertDefaultDateToNull(or["ScheduledMaintenanceDate"]),
                            Description = or["Description"].ToString() ?? string.Empty,
                            AssetID = _assetid
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _scheduledmaintenance;
        }

        public static FullyObservableCollection<Models.MaintenanceRecordModel> GetAllScheduledMaintenance()
        {
            FullyObservableCollection<Models.MaintenanceRecordModel> _scheduledmaintenance = new FullyObservableCollection<Models.MaintenanceRecordModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAllScheduledMaintenance";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _scheduledmaintenance.Add(new Models.MaintenanceRecordModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetID = ConvertObjToInt(or["AssetID"]),
                            Description = or["Description"].ToString() ?? string.Empty,
                            ScheduledMaintenanceDate = ConvertDefaultDateToNull(or["ScheduledMaintenanceDate"]),
                            Label = or["Label"].ToString() ?? string.Empty,
                            CustomerName = or["CustomerName"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _scheduledmaintenance;
        }

        public static FullyObservableCollection<Models.MaintenanceRecordModel> GetAllScheduledAssetAudits()
        {
            FullyObservableCollection<Models.MaintenanceRecordModel> _scheduledaudits = new FullyObservableCollection<Models.MaintenanceRecordModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAllScheduledAssetAudits";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _scheduledaudits.Add(new Models.MaintenanceRecordModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetID = ConvertObjToInt(or["AssetID"]),
                            ScheduledMaintenanceDate = ConvertDefaultDateToNull(or["AuditDate"]),
                            Label = or["Label"].ToString() ?? string.Empty,
                            CustomerName = or["CustomerName"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _scheduledaudits;
        }

        public static FullyObservableCollection<Models.MaintenanceTypeModel> GetMaintenanceTypes()
        {
            FullyObservableCollection<Models.MaintenanceTypeModel> _maintenancetypes = new FullyObservableCollection<Models.MaintenanceTypeModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMaintenanceTypes";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _maintenancetypes.Add(new Models.MaintenanceTypeModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            MaintenanceType = or["Description"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _maintenancetypes;
        }

        public static FullyObservableCollection<Models.OperatingCompanyModel> GetOperatingCompanies()
        {
            FullyObservableCollection<Models.OperatingCompanyModel> _operatingcompanies = new FullyObservableCollection<Models.OperatingCompanyModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetOperatingCompany";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _operatingcompanies.Add(new Models.OperatingCompanyModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            OperatingCompany = or["OperatingCompany"].ToString() ?? string.Empty,
                            Default = ConvertObjToBool(or["Default"])
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _operatingcompanies;
        }

        public static FullyObservableCollection<Models.CorporationModel> GetCorporations()
        {
            FullyObservableCollection<Models.CorporationModel> _corporations = new FullyObservableCollection<Models.CorporationModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCorporations";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _corporations.Add(new Models.CorporationModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            CorporationName = or["CorporationName"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _corporations;
        }      
              
        private static Models.AssetMovementReportModel ProcessMovementCode(int _assetid, ActivityType _activitycode, DateTime? _datemoved, string _label, string _destinationcustomer, string _sourcecustomer)
        {
            Models.AssetMovementReportModel _reportedmovement = new Models.AssetMovementReportModel(); 
            if(_datemoved !=null)
                _reportedmovement.DateMoved = _datemoved;
            _reportedmovement.ID = _assetid;
            _reportedmovement.AssetLabel = _label;
            switch (_activitycode) {

                case ActivityType.NewAsset:                    
                        _reportedmovement.Description = "New Asset " + _label + " added to " + _destinationcustomer;                   
                    break;
                    
                case ActivityType.Transfer:                                         
                        _reportedmovement.Description = "Asset " + _label + " moved from " + _sourcecustomer + " to " + _destinationcustomer;
                    break;

                case ActivityType.Deleted:
                        _reportedmovement.Description = "Deleted Asset " + _label + " from " + _sourcecustomer;
                    break;

                case ActivityType.Undeleted:
                       _reportedmovement.Description = "Undeleted Asset " + _label + " and added to " + _destinationcustomer;       
                    break;

            }            
            return _reportedmovement;            
        }

        public static FullyObservableCollection<Models.PhotoModel> GetAssetPhotos(int _assetid)
        {
            FullyObservableCollection<Models.PhotoModel> _photos = new FullyObservableCollection<Models.PhotoModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetPhotos";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _photos.Add(new Models.PhotoModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            PhotoFileName = or["PhotoFileName"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _photos;
        }

        public static FullyObservableCollection<Models.ConsumableModel> GetAssetConsumables(int _assetid, bool _selectused)
        {
            FullyObservableCollection<Models.ConsumableModel> _consumables = new FullyObservableCollection<Models.ConsumableModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetConsumables";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    oc.Parameters.Add("@_used", OleDbType.Boolean).Value = _selectused;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _consumables.Add(new Models.ConsumableModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetID = _assetid,
                            Description = or["Description"].ToString() ?? string.Empty,
                            Cost = ConvertObjToDecimal(or["Cost"]),
                            DatePurchased = ConvertDefaultDateToNull(or["DatePurchased"]),
                            OrderReference = or["OrderReference"].ToString() ?? string.Empty,
                            Supplier = or["Supplier"].ToString() ?? string.Empty,
                            Used = ConvertObjToBool(or["Used"]),
                            Quantity = ConvertObjToInt(or["Quantity"])
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _consumables;
        }

        public static FullyObservableCollection<Models.AssetSummaryModel> GetAvailableAssets(int _statusid, int _assetid)
        {
            FullyObservableCollection<Models.AssetSummaryModel> _availableassets = new FullyObservableCollection<Models.AssetSummaryModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAvailableAssets";
                    oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _statusid;
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _availableassets.Add(new Models.AssetSummaryModel
                        {
                            ID = ConvertObjToInt(or["AssetID"]),
                            Label = or["Label"].ToString() ?? string.Empty,
                            Description = or["Description"].ToString() ?? string.Empty,
                            Customer = or["CustomerName"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _availableassets;
        }
       
        public static FullyObservableCollection<Models.AssetGroupSpecificationModel> GetAssetGroupSpecifications(int _groupid)
        {
            FullyObservableCollection<Models.AssetGroupSpecificationModel> _specifications = new FullyObservableCollection<Models.AssetGroupSpecificationModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetGroupSpecifications";
                    oc.Parameters.Add("@_groupid", OleDbType.Integer).Value = _groupid;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _specifications.Add(new Models.AssetGroupSpecificationModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetGroupID = _groupid,
                            SpecificationNameID  = ConvertObjToInt(or["SpecificationNameID"]),
                            PropertyUnitID = ConvertObjToInt(or["PropertyUnitID"]),
                            MeasurementUnitID = ConvertObjToInt(or["MeasurementUnitID"]),                         
                            SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty

                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _specifications;
        }

        public static FullyObservableCollection<Models.AssetGroupSpecificationModel> GetAllAssetGroupSpecifications()
        {
            FullyObservableCollection<Models.AssetGroupSpecificationModel> _specifications = new FullyObservableCollection<Models.AssetGroupSpecificationModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAllAssetGroupSpecifications";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _specifications.Add(new Models.AssetGroupSpecificationModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetGroupID = ConvertObjToInt(or["AssetGroupID"]),
                            SpecificationNameID = ConvertObjToInt(or["SpecificationNameID"]),
                            PropertyUnitID = ConvertObjToInt(or["PropertyUnitID"]),
                            MeasurementUnitID = ConvertObjToInt(or["MeasurementUnitID"]),
                            SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty

                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _specifications;
        }

        public static FullyObservableCollection<Models.AssetGroupSpecDisplayDataModel> GetAssetGroupSpecificationsForDisplay(int _groupid)
        {
            FullyObservableCollection<Models.AssetGroupSpecDisplayDataModel> _specifications = new FullyObservableCollection<Models.AssetGroupSpecDisplayDataModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetGroupSpecificationsForDisplay";
                    oc.Parameters.Add("@_groupid", OleDbType.Integer).Value = _groupid;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _specifications.Add(new Models.AssetGroupSpecDisplayDataModel
                        {
                            AssetGroupSpecificationID = ConvertObjToInt(or["ID"]),
                            AssetGroupID = _groupid,
                            SpecificationName = or["SpecificationName"].ToString() ?? string.Empty,
                            SpecificationPropertyName = or["PropertyUnit"].ToString() ?? string.Empty,
                            SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty,
                            DataTypeID = ConvertObjToInt(or["MeasurementUnitID"])                                         
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _specifications;
        }

        public static FullyObservableCollection<Models.SearchFieldModel> GetSearchFields()
        {
            FullyObservableCollection<Models.SearchFieldModel> _searchfields = new FullyObservableCollection<Models.SearchFieldModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSearchFields";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _searchfields.Add(new Models.SearchFieldModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Label = or["Label"].ToString() ?? string.Empty,
                            QueryName = or["QueryName"].ToString() ?? string.Empty,
                            SearchField = or["SearchField"].ToString() ?? string.Empty,
                            SearchFieldType = or["SearchFieldType"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _searchfields;
        }
        
        public static Collection<Models.ReportModel> GetReports()
         {
            Collection<Models.ReportModel> _reports = new Collection<Models.ReportModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetReports";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _reports.Add(new Models.ReportModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Header = or["Header"].ToString() ?? string.Empty,
                            Parameter = or["Parameter"].ToString() ?? string.Empty,
                            Tooltip = or["Tooltip"].ToString() ?? string.Empty,
                            IconfileName = or["IconfileName"].ToString() ?? string.Empty,
                            HasDateFilter = ConvertObjToBool(or["HasDateFilter"])
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _reports;
        }        

        public static FullyObservableCollection<Models.SpecificationPropertyModel> GetSpecificationProperties()
        {
            FullyObservableCollection<Models.SpecificationPropertyModel> _properties = new FullyObservableCollection<Models.SpecificationPropertyModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSpecificationProperties";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _properties.Add(new Models.SpecificationPropertyModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            PropertyUnit = or["PropertyUnit"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _properties;
        }

        public static DataTable GetExcelReportData(string _excelreport)
        {
            DataTable _reportdata = new DataTable(_excelreport);
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _excelreport;
                    OleDbDataAdapter da = new OleDbDataAdapter(oc);
                    da.Fill(_reportdata);
                    da.Dispose();
                   
                }
                         
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _reportdata;
        }
       
        public static Dictionary<int, int> GetAssetSearch(string _queryname, string _criteria)
        {
            Dictionary<int, int> _assetids = new Dictionary<int, int>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _queryname;
                    oc.Parameters.Add("@_string", OleDbType.VarChar, _descriptionlength).Value = _criteria;
                    OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {
                        _assetids.Add(ConvertObjToInt(or["AssetID"]), ConvertObjToInt(or["AssetID"]));
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetids;
        }

        public static Dictionary<int, int> GetAssetSearch(string _queryname, int _id)
        {
            Dictionary<int, int> _assetids = new Dictionary<int, int>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _queryname;
                    oc.Parameters.Add("@_id", OleDbType.Integer).Value = _id;
                    OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {
                        _assetids.Add(ConvertObjToInt(or["AssetID"]), ConvertObjToInt(or["AssetID"]));
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetids;
        }

        public static Dictionary<int, int> GetAssetSearch(string _queryname, string _criteria, int _specificationid)
        {
            Dictionary<int, int> _assetids = new Dictionary<int, int>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _queryname;
                    oc.Parameters.Add("@_string", OleDbType.VarChar, _descriptionlength).Value = _criteria;
                    oc.Parameters.Add("@_specificationid", OleDbType.Integer).Value = _specificationid;
                    OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {
                        _assetids.Add(ConvertObjToInt(or["AssetID"]), ConvertObjToInt(or["AssetID"]));
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetids;
        }

        public static Dictionary<int, int> GetAssetSearch(string _queryname, DateTime? _startdate, DateTime? _enddate)
        {
            Dictionary<int, int> _assetids = new Dictionary<int, int>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _queryname;
                    oc.Parameters.Add("@_startdate", OleDbType.Date).Value = _startdate;
                    oc.Parameters.Add("@_enddate", OleDbType.Date).Value = _enddate;
                    OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {
                        _assetids.Add(ConvertObjToInt(or["AssetID"]), ConvertObjToInt(or["AssetID"]));
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assetids;
        }

        public static int CountUsedID(CountSPName _spname, int _id)
        {
            //run Access query
            int count = 0;
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _spname.ToString();
                    oc.Parameters.Add("@_id", OleDbType.Integer).Value = _id;
                    count = (int)oc.ExecuteScalar();
                   
                }
                
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return count;
        }

        public static FullyObservableCollection<Models.AssetModel> GetDuplicateAssetLabels()
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetDuplicateAssetLabels";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assets.Add(new Models.AssetModel
                        {
                             AssetID = ConvertObjToInt(or["AssetID"]),
                             Description = or["Description"].ToString() ?? string.Empty,
                             Label = or["Label"].ToString() ?? string.Empty,
                             ParentAssetID = ConvertObjToInt(or["ParentAssetID"])                 
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }

        public static FullyObservableCollection<Models.AssetModel> GetAssetsWithNoLocationInformation()
        {
            FullyObservableCollection<Models.AssetModel> _assets = new FullyObservableCollection<Models.AssetModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetsWithNoLocationInformation";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assets.Add(new Models.AssetModel
                        {
                            AssetID = ConvertObjToInt(or["AssetID"]),
                            Description = or["Description"].ToString() ?? string.Empty,
                            Label = or["Label"].ToString() ?? string.Empty,
                            ParentAssetID = ConvertObjToInt(or["ParentAssetID"])
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }
       
        public static FullyObservableCollection<Models.AssetSummaryModel> GetDeletedAssets()
        {
            FullyObservableCollection<Models.AssetSummaryModel> _assets = new FullyObservableCollection<Models.AssetSummaryModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetDeletedAssets";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _assets.Add(new Models.AssetSummaryModel
                        {
                            ID = ConvertObjToInt(or["AssetID"]),
                            Description = or["Description"].ToString() ?? string.Empty,
                            Label = or["Label"].ToString() ?? string.Empty,                            
                            ParentAssetID = ConvertObjToInt(or["ParentAssetID"])
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _assets;
        }

        public static FullyObservableCollection<Models.AdministratorUserModel> GetAdministrators()
        {
            FullyObservableCollection<Models.AdministratorUserModel> _administrators = new FullyObservableCollection<Models.AdministratorUserModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAdministrators";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _administrators.Add(new Models.AdministratorUserModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AdministratorName = or["AdministratorName"].ToString() ?? string.Empty,
                            LoginName = or["UserLogin"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _administrators;
        }

        public static int GetIDFromGUID(string _query, string _guidstring)
        {
            //run Access query
            int newassetid = 0;
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {                    
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _query;
                    oc.Parameters.Add("@_guidstring", OleDbType.VarChar, 255).Value = _guidstring;
                    newassetid = (int)oc.ExecuteScalar();
                                        
                }                
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return newassetid;
        }

        public static int[] GetLabelMask()
        {
            int[] _mask = new int[3];

            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetLabelMask";
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {                
                        _mask[0] = ConvertObjToInt(or["AreaChars"]);
                        _mask[1] = ConvertObjToInt(or["GroupChars"]);
                        _mask[2] = (ConvertObjToInt(or["MaximumIDValue"]) - 1).ToString().Length;
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _mask;
        }

        public static FullyObservableCollection<ViewModels.AuditDate> GetAssetAuditDates(int _assetid)
        {
            ViewModels.AuditDate _nextaudit;
            FullyObservableCollection<ViewModels.AuditDate> _audits = new FullyObservableCollection<ViewModels.AuditDate>();
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetAuditDates";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    OleDbDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        _nextaudit = new ViewModels.AuditDate();
                        _nextaudit.ID = ConvertObjToInt( or["ID"]);
                        _nextaudit.DateAudit = (DateTime)or["AuditDate"];
                        _audits.Add(_nextaudit);
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _audits;
        }

        #endregion
        
        #region Reports

        public static Collection<Models.AssetMovementReportModel> GetAssetMovementsReport(int _assetid)
        {
            Collection<Models.AssetMovementReportModel> _reportedmovements = new Collection<Models.AssetMovementReportModel>();
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetMovementsReport";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {
                        _reportedmovements.Add(ProcessMovementCode(_assetid, (ActivityType)Convert.ToInt32(or["ActivityCodeID"]), ConvertDefaultDateToNull(or["DateMoved"]),
                            or["Label"].ToString(), or["DestinationCustomer"].ToString(), or["SourceCustomer"].ToString()));
                                                                       
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _reportedmovements;
        }

        public static DataTable GetDateFilteredMovementReportData(string _month)
        {
            DataTable _reportdata = new DataTable("MovementsReport");
            DataColumn _dc;
            DataRow _dr;

            _dc = new DataColumn();
            _dc.Caption = "Asset ID";
            _dc.ColumnName = "ID";
            _dc.DataType = System.Type.GetType("System.Int32");
            _reportdata.Columns.Add(_dc);

            _dc = new DataColumn();
            _dc.Caption = "BAAN ID";
            _dc.ColumnName = "BAANID";
            _dc.DataType = System.Type.GetType("System.Int32");
            _reportdata.Columns.Add(_dc);

            _dc = new DataColumn();
            _dc.Caption = "Date Moved";
            _dc.ColumnName = "DateMoved";
            _dc.DataType = System.Type.GetType("System.DateTime");
            _reportdata.Columns.Add(_dc);

            _dc = new DataColumn();
            _dc.Caption = "Label";
            _dc.ColumnName = "Label";
            _dc.DataType = System.Type.GetType("System.String");
            _reportdata.Columns.Add(_dc);

            _dc = new DataColumn();
            _dc.Caption = "Description";
            _dc.ColumnName = "Description";
            _dc.DataType = System.Type.GetType("System.String");
            _reportdata.Columns.Add(_dc);

            Models.AssetMovementReportModel _reportedmovement = new Models.AssetMovementReportModel();
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMovementsReport";
                    oc.Parameters.Add("@_monthmoved", OleDbType.VarChar, _namelength).Value = _month;

                    OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {                       
                        _reportedmovement = (ProcessMovementCode(ConvertObjToInt(or["AssetID"]), (ActivityType)Convert.ToInt32(or["ActivityCodeID"]), ConvertDefaultDateToNull(or["DateMoved"]),
                           or["Label"].ToString(), or["DestinationCustomer"].ToString(), or["SourceCustomer"].ToString()));
                        _dr = _reportdata.NewRow();
                        _dr["ID"] = _reportedmovement.ID;
                        if(!string.IsNullOrEmpty(or["BAANID"].ToString()))
                            _dr["BAANID"] = Convert.ToInt32(or["BAANID"]);                         
                        _dr["DateMoved"] = _reportedmovement.DateMoved;
                        _dr["Label"] = _reportedmovement.AssetLabel;
                        _dr["Description"] = _reportedmovement.Description;
                        _reportdata.Rows.Add(_dr);
                    }
                    or.Close();
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
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _reportname;
                    oc.Parameters.Add("@_month", OleDbType.VarChar, _namelength).Value = _month;
                    
                    OleDbDataAdapter da = new OleDbDataAdapter();
                    da.SelectCommand = oc;
                    da.Fill(_reportdata);
                    da.Dispose();
                    
                }                
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return _reportdata;
        }

        #endregion
        
        #region Insert Queries
        //====================================================================================================================================
        //Insert queries

        public static void AddAsset(Models.AssetModel _asset)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddAsset";
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _asset.Description ?? string.Empty;
                    oc.Parameters.Add("@_applicationtype", OleDbType.VarChar, _descriptionlength).Value = _asset.ApplicationType ?? string.Empty;
                    oc.Parameters.Add("@_sitelocation", OleDbType.VarChar, _descriptionlength).Value = _asset.SiteLocation ?? string.Empty;
                    oc.Parameters.Add("@_manufacturername", OleDbType.VarChar, _descriptionlength).Value = _asset.ManufacturerName ?? string.Empty;
                    oc.Parameters.Add("@_modelno", OleDbType.VarChar, _descriptionlength).Value = _asset.ModelNo;
                    oc.Parameters.Add("@_serialno", OleDbType.VarChar, _descriptionlength).Value = _asset.SerialNo;
                    oc.Parameters.Add("@_dateinstalled", OleDbType.Date).Value = (_asset.DateInstalled == null) ? DefaultDate() : _asset.DateInstalled;
                    oc.Parameters.Add("@_purchaseprice", OleDbType.Decimal).Value = _asset.PurchasePrice;
                    oc.Parameters.Add("@_comments", OleDbType.VarChar, _descriptionlength).Value = _asset.Comments ?? string.Empty;
                    oc.Parameters.Add("@_chemicalsused", OleDbType.VarChar, _descriptionlength).Value = _asset.ChemicalsUsed ?? string.Empty;
                    oc.Parameters.Add("@_label", OleDbType.VarChar, _descriptionlength).Value = _asset.Label ?? string.Empty;
                    oc.Parameters.Add("@_datepurchased", OleDbType.Date).Value = (_asset.DatePurchased == null) ? DefaultDate() : _asset.DatePurchased;
                    oc.Parameters.Add("@_ponumber", OleDbType.VarChar, _descriptionlength).Value = _asset.PONumber ?? string.Empty;
                    oc.Parameters.Add("@_assettypeid", OleDbType.Integer).Value = _asset.AssetTypeID;
                    oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _asset.CustomerID;
                    oc.Parameters.Add("@_salesdivisionid", OleDbType.Integer).Value = _asset.SalesDivisionID;
                    oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _asset.StatusID;
                    oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _asset.ParentAssetID;
                    oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _asset.AssetGroupID;
                    oc.Parameters.Add("@_prefixid", OleDbType.Integer).Value = _asset.AssetAreaID;
                    oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _asset.OperatingCompanyID;              
                    oc.Parameters.Add("@_dimensions", OleDbType.VarChar, _descriptionlength).Value = _asset.Dimensions ?? string.Empty;
                    oc.Parameters.Add("@_suppliername", OleDbType.VarChar, _descriptionlength).Value = _asset.SupplierName ?? string.Empty;
                    oc.Parameters.Add("@_baanid", OleDbType.VarChar, _baanidlength).Value = _asset.BAANID ?? string.Empty;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddCustomer(Models.CustomerModel _customer)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddCustomer";
                    oc.Parameters.Add("@_customernumber", OleDbType.VarChar, _namelength).Value = _customer.CustomerNumber;
                    oc.Parameters.Add("@_customername", OleDbType.VarChar, _namelength).Value = _customer.CustomerName;
                    oc.Parameters.Add("@_location", OleDbType.VarChar, _descriptionlength).Value = _customer.Location;
                    oc.Parameters.Add("@_countryid", OleDbType.Integer).Value = _customer.CountryID;
                    oc.Parameters.Add("@_corporationid", OleDbType.Integer).Value = _customer.CorporationID;
                    oc.Parameters.Add("@_iconfilename", OleDbType.VarChar, _filenamelength).Value = _customer.IconFileName;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddAdministrator(Models.AdministratorUserModel _administrator)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddAdministrator";
                    oc.Parameters.Add("@_administratorname", OleDbType.VarChar, _namelength).Value = _administrator.AdministratorName;
                    oc.Parameters.Add("@_loginname", OleDbType.VarChar, _namelength).Value = _administrator.LoginName;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddCountry(Models.CountryModel _country)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddCountry";
                    oc.Parameters.Add("@_countryname", OleDbType.VarChar, _namelength).Value = _country.CountryName;
                    oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _country.OperatingCompanyID;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }
        
        public static void AddSalesDivision(Models.SalesDivisionModel _salesdivision)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddSalesDivision";
                    oc.Parameters.Add("@_salesdivisionname", OleDbType.VarChar, _namelength).Value = _salesdivision.SalesDivisionName;
                    oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _salesdivision.OperatingCompanyID;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddMaintenanceRecord(Models.MaintenanceRecordModel _maintenancerecord)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddMaintenanceRecord";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _maintenancerecord.AssetID;
                    oc.Parameters.Add("@_maintenancedate", OleDbType.Date).Value = (_maintenancerecord.MaintenanceDate == null) ? DefaultDate() : _maintenancerecord.MaintenanceDate;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _maintenancerecord.Description;
                    oc.Parameters.Add("@_cost", OleDbType.Decimal).Value = _maintenancerecord.Cost;
                    oc.Parameters.Add("@_maintainedby", OleDbType.VarChar, _namelength).Value = _maintenancerecord.MaintainedBy;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddScheduledMaintenance(Models.MaintenanceRecordModel _scheduledmaintenance)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddScheduledMaintenance";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _scheduledmaintenance.AssetID;
                    oc.Parameters.Add("@_maintenancedate", OleDbType.Date).Value = (_scheduledmaintenance.ScheduledMaintenanceDate == null) ? DefaultDate() : _scheduledmaintenance.ScheduledMaintenanceDate;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _scheduledmaintenance.Description;
                    oc.Parameters.Add("@_completed", OleDbType.Boolean).Value = _scheduledmaintenance.Completed;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }        

        public static void AddAssetArea(Models.AssetAreaModel _assetarea)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddAssetArea";
                    oc.Parameters.Add("@_prefix", OleDbType.VarChar, _prefixlength).Value = _assetarea.Prefix;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assetarea.Description;
                    oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _assetarea.Default;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddAssetGroup(Models.AssetGroupModel _assetgroup)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddAssetGroup";
                    oc.Parameters.Add("@_assetprefixid", OleDbType.Integer).Value = _assetgroup.AssetAreaID;
                    oc.Parameters.Add("@_group", OleDbType.VarChar, _namelength).Value = _assetgroup.Description;
                    oc.Parameters.Add("@_assetgroupidtext", OleDbType.VarChar, 255).Value = _assetgroup.AssetGroupIDText;
                    oc.Parameters.Add("@_canbeparent", OleDbType.Boolean).Value = _assetgroup.CanBeParent;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddAssetType(Models.AssetTypeModel _assettype)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddAssetType";
                    oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _assettype.AssetGroupID;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assettype.Description;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddAssetMovement(Models.AssetMovementModel _assetmovement)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddAssetMovement";
                    oc.Parameters.Add("@_datemoved", OleDbType.Date).Value = (_assetmovement.DateMoved == null) ? DefaultDate() : _assetmovement.DateMoved;
                    oc.Parameters.Add("@_activitycode", OleDbType.Integer).Value = _assetmovement.ActivityCodeID;
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetmovement.AssetID;
                    oc.Parameters.Add("@_sourcecustomer", OleDbType.Integer).Value = _assetmovement.SourceCustomerID;
                    oc.Parameters.Add("@_destinationcustomer", OleDbType.Integer).Value = _assetmovement.DestinationCustomerID;                                       
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddOperatingCompany(Models.OperatingCompanyModel _opco)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddOperatingCompany";
                    oc.Parameters.Add("@_operatingcompany", OleDbType.VarChar, _namelength).Value = _opco.OperatingCompany;
                    oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _opco.Default;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddCorporation(Models.CorporationModel _opco)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddCorporation";
                    oc.Parameters.Add("@_corporation", OleDbType.VarChar, _namelength).Value = _opco.CorporationName;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }
      
        public static void AddPhoto(Models.PhotoModel _photo)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddPhoto";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _photo.AssetID;
                    oc.Parameters.Add("@_photofilename", OleDbType.VarChar, _filenamelength).Value = _photo.PhotoFileName ?? string.Empty;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddSpecification(Models.AssetSpecificationModel _spec)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddSpecification";
                    oc.Parameters.Add("@_specification", OleDbType.VarChar, _namelength).Value = _spec.SpecificationName;
                    oc.Parameters.Add("@_measurementunitid", OleDbType.Integer).Value = _spec.MeasurementUnitID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }
       
        public static void AddSpecificationProperty(Models.SpecificationPropertyModel _specprop)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddSpecificationProperty";
                    oc.Parameters.Add("@_specpropertyunit", OleDbType.VarChar, _namelength).Value = _specprop.PropertyUnit;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddConsumable(Models.ConsumableModel _consumable)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddConsumable";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _consumable.AssetID;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _consumable.Description ?? string.Empty;
                    oc.Parameters.Add("@_cost", OleDbType.Decimal).Value = _consumable.Cost;
                    oc.Parameters.Add("@_datepurchased", OleDbType.Date).Value = (_consumable.DatePurchased == null) ? DefaultDate() : _consumable.DatePurchased;
                    oc.Parameters.Add("@_orderreference", OleDbType.VarChar, _namelength).Value = _consumable.OrderReference ?? string.Empty;
                    oc.Parameters.Add("@_supplier", OleDbType.VarChar, _namelength).Value = _consumable.Supplier ?? string.Empty;
                    oc.Parameters.Add("@_used", OleDbType.Boolean).Value = _consumable.Used;
                    oc.Parameters.Add("@_quantity", OleDbType.Integer).Value = _consumable.Quantity;                    
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }
       
        public static void AddAssetGroupSpecification(Models.AssetGroupSpecificationModel _specification)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddAssetGroupSpecification";
                    oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _specification.AssetGroupID;
                    oc.Parameters.Add("@_specificationnameid", OleDbType.Integer).Value = _specification.SpecificationNameID;
                    oc.Parameters.Add("@_propertyunitid", OleDbType.Integer).Value = _specification.PropertyUnitID;
                    oc.Parameters.Add("@_specificationoptions", OleDbType.VarChar, 255).Value = _specification.SpecificationOptions;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddGuid(string _query,  string _guidstring)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _query;
                    oc.Parameters.Add("@_guidstring", OleDbType.VarChar, 255).Value = _guidstring;
                    // execute
                    oc.ExecuteNonQuery();                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void AddAuditDate(int _assetid, DateTime _dtaudit)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "AddAuditDate";
                    oc.Parameters.Add("@_nextauditdate", OleDbType.Date).Value = _dtaudit;
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }


        #endregion

        #region Update Queries
        //====================================================================================================================================
        //Update queries

        public static void UpdateAsset(Models.AssetModel _asset)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateAsset";
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _asset.Description ?? string.Empty;
                    oc.Parameters.Add("@_applicationtype", OleDbType.VarChar, _descriptionlength).Value = _asset.ApplicationType ?? string.Empty;
                    oc.Parameters.Add("@_sitelocation", OleDbType.VarChar, _descriptionlength).Value = _asset.SiteLocation ?? string.Empty;
                    oc.Parameters.Add("@_manufacturername", OleDbType.VarChar, _descriptionlength).Value = _asset.ManufacturerName ?? string.Empty;
                    oc.Parameters.Add("@_modelno", OleDbType.VarChar, _descriptionlength).Value = _asset.ModelNo ?? string.Empty;
                    oc.Parameters.Add("@_serialno", OleDbType.VarChar, _descriptionlength).Value = _asset.SerialNo ?? string.Empty;
                    oc.Parameters.Add("@_dateinstalled", OleDbType.Date).Value = (_asset.DateInstalled == null) ? DefaultDate() : _asset.DateInstalled;
                    oc.Parameters.Add("@_purchaseprice", OleDbType.Decimal).Value = _asset.PurchasePrice;
                    oc.Parameters.Add("@_comments", OleDbType.VarChar, _descriptionlength).Value = _asset.Comments ?? string.Empty;
                    oc.Parameters.Add("@_chemicalsused", OleDbType.VarChar, _descriptionlength).Value = _asset.ChemicalsUsed ?? string.Empty;
                    oc.Parameters.Add("@_label", OleDbType.VarChar, _descriptionlength).Value = _asset.Label ?? string.Empty;
                    oc.Parameters.Add("@_datepurchased", OleDbType.Date).Value = (_asset.DatePurchased == null) ? DefaultDate() : _asset.DatePurchased;
                    oc.Parameters.Add("@_ponumber", OleDbType.VarChar, _descriptionlength).Value = _asset.PONumber ?? string.Empty;
                    oc.Parameters.Add("@_assettypeid", OleDbType.Integer).Value = _asset.AssetTypeID;
                    oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _asset.CustomerID;
                    oc.Parameters.Add("@_salesdivisionid", OleDbType.Integer).Value = _asset.SalesDivisionID;
                    oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _asset.StatusID;
                    oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _asset.ParentAssetID;
                    oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _asset.AssetGroupID;
                    oc.Parameters.Add("@_prefixid", OleDbType.Integer).Value = _asset.AssetAreaID;
                    oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _asset.OperatingCompanyID;
                    oc.Parameters.Add("@_dimensions", OleDbType.VarChar, _descriptionlength).Value = _asset.Dimensions ?? string.Empty;
                    oc.Parameters.Add("@_suppliername", OleDbType.VarChar, _descriptionlength).Value = _asset.SupplierName ?? string.Empty;
                    oc.Parameters.Add("@_baanid", OleDbType.VarChar, _baanidlength).Value = _asset.BAANID ?? string.Empty;
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _asset.AssetID;
                    // execute
                    oc.ExecuteNonQuery();
                  
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateChildAsset(int _assetid, int _customerid, int _statusid)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query                
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateChildAsset";
                    oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customerid;
                    oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _statusid;
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateCustomer(Models.CustomerModel _customer)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateCustomer";
                    oc.Parameters.Add("@_customernumber", OleDbType.VarChar, _namelength).Value = _customer.CustomerNumber ?? string.Empty;
                    oc.Parameters.Add("@_customername", OleDbType.VarChar, _namelength).Value = _customer.CustomerName ?? string.Empty;
                    oc.Parameters.Add("@_location", OleDbType.VarChar, _namelength).Value = _customer.Location;
                    oc.Parameters.Add("@_countryid", OleDbType.Integer).Value = _customer.CountryID;
                    oc.Parameters.Add("@_corporationid", OleDbType.Integer).Value = _customer.CorporationID;
                    oc.Parameters.Add("@_iconfilename", OleDbType.VarChar, _filenamelength).Value = _customer.IconFileName ?? string.Empty;
                    oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customer.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }
        
        public static void UpdateAdministrator(Models.AdministratorUserModel _administrator)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateAdministrator";
                    oc.Parameters.Add("@_administratorname", OleDbType.VarChar, _namelength).Value = _administrator.AdministratorName ?? string.Empty;
                    oc.Parameters.Add("@_loginname", OleDbType.VarChar, _namelength).Value = _administrator.LoginName ?? string.Empty;
                    oc.Parameters.Add("@_administratorid", OleDbType.Integer).Value = _administrator.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateCountry(Models.CountryModel _country)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateCountry";
                    oc.Parameters.Add("@_countryname", OleDbType.VarChar, _namelength).Value = _country.CountryName ?? string.Empty;
                    oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _country.OperatingCompanyID;
                    oc.Parameters.Add("@_countryid", OleDbType.Integer).Value = _country.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }
        
        public static void UpdateSalesDivision(Models.SalesDivisionModel _salesdivision)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateSalesDivision";
                    oc.Parameters.Add("@_salesdivisionname", OleDbType.VarChar, _namelength).Value = _salesdivision.SalesDivisionName ?? string.Empty;
                    oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _salesdivision.OperatingCompanyID;
                    oc.Parameters.Add("@_salesdivisionid", OleDbType.Integer).Value = _salesdivision.ID;
                    // execute
                    oc.ExecuteNonQuery();
                  
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateOperatingCompany(Models.OperatingCompanyModel _operatingcompany)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateOperatingCompany";
                    oc.Parameters.Add("@_operatingcompanyname", OleDbType.VarChar, _namelength).Value = _operatingcompany.OperatingCompany ?? string.Empty;
                    oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _operatingcompany.Default;
                    oc.Parameters.Add("@_operatingcompanyid", OleDbType.Integer).Value = _operatingcompany.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }       

        public static void UpdateMaintenanceRecord(Models.MaintenanceRecordModel _maintenancerecord)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateMaintenanceRecord";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _maintenancerecord.AssetID;
                    oc.Parameters.Add("@_maintenancedate", OleDbType.Date).Value = (_maintenancerecord.MaintenanceDate == null) ? DefaultDate() : _maintenancerecord.MaintenanceDate;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _maintenancerecord.Description;
                    oc.Parameters.Add("@_cost", OleDbType.Decimal).Value = _maintenancerecord.Cost;
                    oc.Parameters.Add("@_maintainedby", OleDbType.VarChar, _namelength).Value = _maintenancerecord.MaintainedBy ?? string.Empty;
                    oc.Parameters.Add("@_completed", OleDbType.Boolean).Value = _maintenancerecord.Completed;
                    oc.Parameters.Add("@_datescheduled", OleDbType.Date).Value = (_maintenancerecord.ScheduledMaintenanceDate == null) ? DefaultDate() : _maintenancerecord.ScheduledMaintenanceDate;
                    oc.Parameters.Add("@_maintenancerecordid", OleDbType.Integer).Value = _maintenancerecord.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }
       
        public static void UpdateMaintenanceType(Models.MaintenanceTypeModel _maintenancetype)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateMaintenanceType";
                    oc.Parameters.Add("@_maintenancetype", OleDbType.VarChar, _namelength).Value = _maintenancetype.MaintenanceType;
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _maintenancetype.ID;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetArea(Models.AssetAreaModel _assetarea)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateAssetArea";
                    oc.Parameters.Add("@_prefix", OleDbType.VarChar, _prefixlength).Value = _assetarea.Prefix;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assetarea.Description;
                    oc.Parameters.Add("@_default", OleDbType.Boolean).Value = _assetarea.Default;
                    oc.Parameters.Add("@_assetareaid", OleDbType.Integer).Value = _assetarea.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }       

        public static void UpdateAssetGroup(Models.AssetGroupModel _assetgroup)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
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
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetType(Models.AssetTypeModel _assettype)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateAssetType";
                    oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _assettype.AssetGroupID;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _assettype.Description;
                    oc.Parameters.Add("@_assettypeid", OleDbType.Integer).Value = _assettype.ID;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateCorporation(Models.CorporationModel _corporation)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateCorporation";
                    oc.Parameters.Add("@_corporationname", OleDbType.VarChar, _namelength).Value = _corporation.CorporationName ?? string.Empty;
                    oc.Parameters.Add("@_corporationid", OleDbType.Integer).Value = _corporation.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }
       
        public static void UpdateAssetPhoto(Models.PhotoModel _photo)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateAssetPhoto";
                    oc.Parameters.Add("@_photofilename", OleDbType.VarChar, _filenamelength).Value = _photo.PhotoFileName ?? string.Empty;
                    oc.Parameters.Add("@_photoid", OleDbType.Integer).Value = _photo.ID;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateSpecification(Models.AssetSpecificationModel _spec)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateSpecification";
                    oc.Parameters.Add("@_specificationname", OleDbType.VarChar, _namelength).Value = _spec.SpecificationName;
                    oc.Parameters.Add("@_measurementunitid", OleDbType.Integer).Value = _spec.MeasurementUnitID;
                    oc.Parameters.Add("@_specificationtypeid", OleDbType.Integer).Value = _spec.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateAssetGroupSpecification(Models.AssetGroupSpecificationModel _spec)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateAssetGroupSpecification";
                    oc.Parameters.Add("@_assetgroupid", OleDbType.Integer).Value = _spec.AssetGroupID;
                    oc.Parameters.Add("@_specificationnameid", OleDbType.Integer).Value = _spec.SpecificationNameID;
                    oc.Parameters.Add("@_propertyunitid", OleDbType.Integer).Value = _spec.PropertyUnitID;
                    oc.Parameters.Add("@_specificationoptions", OleDbType.VarChar, 255).Value = _spec.SpecificationOptions;
                    oc.Parameters.Add("@_id", OleDbType.Integer).Value = _spec.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateSpecificationRecord(Models.AssetGroupSpecDisplayDataModel _spec)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;

                    oc.Parameters.Add("@_specificationvalue", OleDbType.VarChar, _descriptionlength).Value = _spec.SpecificationValue ?? string.Empty;
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
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateSpecificationProperty(Models.SpecificationPropertyModel _specprop)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateSpecificationProperty";
                    oc.Parameters.Add("@_specproperty", OleDbType.VarChar, _namelength).Value = _specprop.PropertyUnit;
                    oc.Parameters.Add("@_specpropertyid", OleDbType.Integer).Value = _specprop.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateConsumable(Models.ConsumableModel _consumable)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateConsumable";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _consumable.AssetID;
                    oc.Parameters.Add("@_description", OleDbType.VarChar, _descriptionlength).Value = _consumable.Description ?? string.Empty;
                    oc.Parameters.Add("@_cost", OleDbType.Decimal).Value = _consumable.Cost;
                    oc.Parameters.Add("@_datepurchased", OleDbType.Date).Value = (_consumable.DatePurchased == null) ? DefaultDate() : _consumable.DatePurchased;
                    oc.Parameters.Add("@_orderreference", OleDbType.VarChar, _namelength).Value = _consumable.OrderReference ?? string.Empty;
                    oc.Parameters.Add("@_supplier", OleDbType.VarChar, _namelength).Value = _consumable.Supplier ?? string.Empty;
                    oc.Parameters.Add("@_used", OleDbType.Boolean).Value = _consumable.Used;
                    oc.Parameters.Add("@_quantity", OleDbType.Integer).Value = _consumable.Quantity;
                    oc.Parameters.Add("@_consumableid", OleDbType.Integer).Value = _consumable.ID;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UpdateParentAssetID(int _assetid, int _parentassetid, int _customerid)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateParentAssetID";
                    oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _parentassetid;
                    oc.Parameters.Add("@_customerid", OleDbType.Integer).Value = _customerid;
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    // execute
                    oc.ExecuteNonQuery();
                  
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void UnDeleteAsset(int _assetid, int _defaultcustomerid, int _statusid)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UnDeleteAsset";
                    oc.Parameters.Add("@_defaultcustomerid", OleDbType.Integer).Value = _defaultcustomerid;
                    oc.Parameters.Add("@_statusid", OleDbType.Integer).Value = _statusid;
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _assetid;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }            
        }

        public static void UpdateAuditDate(int _id, DateTime _dtaudit)
        {
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    //run Access query
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "UpdateAuditDate";
                    oc.Parameters.Add("@_nextauditdate", OleDbType.Date).Value = _dtaudit;
                    oc.Parameters.Add("@_id", OleDbType.Integer).Value = _id;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void SetDefaultCustomer(int _defaultcustomerid)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "SetDefaultCustomer";
                    oc.Parameters.Add("@_defaultcustomerid", OleDbType.Integer).Value = _defaultcustomerid;
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void ResetDefaultCustomer()
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "ResetDefaultCustomer";
                    // execute
                    oc.ExecuteNonQuery();
                   
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        #endregion


        #region Delete Queries

        public static void RemoveChildAsset(int _childassetid)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "RemoveChildAsset";
                    oc.Parameters.Add("@_assetid", OleDbType.Integer).Value = _childassetid;
                    // execute
                    oc.ExecuteNonQuery();
                  
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void SetConsumableToUsed(int _consumableid)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "SetConsumableToUsed";
                    oc.Parameters.Add("@_consumableid", OleDbType.Integer).Value = _consumableid;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public static void DeleteItem(int _id, DeleteSPName _sp)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = _sp.ToString();
                    oc.Parameters.Add("@_id", OleDbType.Integer).Value = _id;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }            
        }

        public static void SetParentAssetID(int _assetid, int _parentassetid)
        {
            //run Access query
            try
            {
                using (OleDbCommand oc = new OleDbCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "SetParentAssetID";
                    oc.Parameters.Add("@_parentassetid", OleDbType.Integer).Value = _parentassetid;
                    oc.Parameters.Add("@_id", OleDbType.Integer).Value = _assetid;
                    // execute
                    oc.ExecuteNonQuery();
                    
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        #endregion


        #region Helpers

        private static int ConvertObjToInt(object _obj)
        {
            int _id = 0;
            bool _isnumber = int.TryParse(_obj.ToString(), out _id);
            return _id;
        }

        private static decimal ConvertObjToDecimal(object _obj)
        {
            decimal _id = 0;
            bool _isnumber = decimal.TryParse(_obj.ToString(), out _id);
            return _id;
        }

        private static bool ConvertObjToBool(object _obj)
        {
            bool _bool = false;
            bool _isbool = bool.TryParse(_obj.ToString(), out _bool);
            return _bool;
        }

        private static DateTime? ConvertObjToDate(object _obj)
        {
            DateTime _dt;
            bool _isdate = DateTime.TryParse(_obj.ToString(), out _dt);
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

            if(_isdate)
                if (_newdt == DefaultDate() || _dt == null)
                    return (DateTime?)null;
                else
                    return _newdt;
            else
                return (DateTime?)null;

        }

        private static void ShowError(Exception e,[CallerMemberName] string _operationtype=null)
        {

            IMessageBoxService _msg = new MessageBoxService();
            _msg.ShowMessage("Error during " + _operationtype + " operation\n" + e.Message.ToString(), _operationtype + " Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            _msg = null;
        }


    }
    #endregion

}





