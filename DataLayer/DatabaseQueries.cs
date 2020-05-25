using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using static AssetManager.GlobalClass;
using AssetManager.Models;

namespace AssetManager
{
    static class DatabaseQueries 
    {
        const int namelength = 50;
        const int descriptionlength = 255;
        const int prefixlength = 3;
        const int filenamelength = 255;
        const int SAPIDlength = 15;
          
        #region Get Queries

        public static Dictionary<int, string> GetAssetLabels()
        {
            Dictionary<int, string> assets = new Dictionary<int, string>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetLabels";
                    oc.Connection = Conn;
                    
                    SqlDataReader or = oc.ExecuteReader();
                    int id = 0;
                    string temp = string.Empty;
                    while (or.Read())
                    {
                        id = Convert.ToInt32(or["ID"]);
                        temp = or["Label"].ToString() ?? string.Empty;
                        assets.Add(id, temp);
                    }
                    or.Close();                    
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assets;
        }

        public static AssetModel GetAsset(int assetid)
        {
            AssetModel asset = new AssetModel();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAsset";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assetid;
                    oc.Connection = Conn;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        asset.ID = assetid;
                        asset.Name = or["Name"].ToString() ?? string.Empty;
                       
                       // asset.Category = or["Category"].ToString() ?? string.Empty;
                        asset.ApplicationType = or["ApplicationType"].ToString() ?? string.Empty;
                        asset.SiteLocation = or["SiteLocation"].ToString() ?? string.Empty;
                        asset.ManufacturerName = or["ManufacturerName"].ToString() ?? string.Empty;
                        asset.ModelNo = or["ModelNo"].ToString() ?? string.Empty;
                        asset.SerialNo = or["SerialNo"].ToString() ?? string.Empty;
                        asset.DateInstalled = ConvertObjToDate(or["DateInstalled"]);
                        asset.PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]);
                        asset.Comments = or["Comments"].ToString() ?? string.Empty;
                        asset.ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty;
                        asset.DatePurchased = ConvertObjToDate(or["DatePurchased"]);
                        asset.PONumber = or["PONumber"].ToString() ?? string.Empty;
                        asset.AssetTypeID = ConvertObjToInt(or["TypeID"]);
                        asset.AssetGroupID = ConvertObjToInt(or["GroupID"]);
                        asset.CustomerID = ConvertObjToInt(or["CustomerID"]);
                        asset.SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]);
                        asset.AssetAreaID = ConvertObjToInt(or["PrefixID"]);
                        asset.StatusID = ConvertObjToInt(or["StatusID"]);
                        asset.ParentAssetID = ConvertObjToInt(or["ParentAssetID"]);
                        asset.NextAuditDate = ConvertObjToDate(or["NextAuditDate"]);
                        asset.Dimensions = or["Dimensions"].ToString() ?? string.Empty;
                        asset.SupplierName = or["SupplierName"].ToString() ?? string.Empty;
                        asset.SAPID = or["SAPID"].ToString() ?? string.Empty;
                        asset.Customer = or["Customer"].ToString() ?? string.Empty;
                        asset.Location = or["Location"].ToString() ?? string.Empty;
                        asset.LabelID = ConvertObjToInt(or["LabelID"]);
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetChildAssets";
                    oc.Parameters.Add("@parentid", SqlDbType.Int).Value = parentassetid;
                    //leave connection open  - it will be closed once the entire collection of assets is retrieved
                    SqlDataReader or = oc.ExecuteReader();
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
                            DateInstalled = ConvertObjToDate(or["DateInstalled"]),
                            PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]),
                            Comments = or["Comments"].ToString() ?? string.Empty,
                            ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty,
                            LabelID = ConvertObjToInt(or["LabelID"]),
                            DatePurchased = ConvertObjToDate(or["DatePurchased"]),
                            PONumber = or["PONumber"].ToString() ?? string.Empty,
                            AssetTypeID = ConvertObjToInt(or["TypeID"]),
                            AssetGroupID = ConvertObjToInt(or["GroupID"]),
                            CustomerID = ConvertObjToInt(or["CustomerID"]),
                            SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]),
                            AssetAreaID = ConvertObjToInt(or["PrefixID"]),
                            StatusID = ConvertObjToInt(or["StatusID"]),
                            ParentAssetID = parentassetid,
                            NextAuditDate = ConvertObjToDate(or["NextAuditDate"]),
                            Dimensions = or["Dimensions"].ToString() ?? string.Empty,
                            SupplierName = or["SupplierName"].ToString() ?? string.Empty,
                            SAPID = or["SAPID"].ToString() ?? string.Empty
                        };
                        assets.Add(asset);
                    }
                    or.Close();                   
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetDeletedChildAssets";
                    oc.Parameters.Add("@parentid", SqlDbType.Int).Value = parentassetid;

                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        assets.Add(new AssetModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            LabelID = ConvertObjToInt(or["LabelID"]),
                            AssetGroupID = ConvertObjToInt(or["AssetGroupID"]),
                            AssetAreaID = ConvertObjToInt(or["AssetAreaID"]),
                            CustomerID = ConvertObjToInt(or["CustomerID"]),
                            ParentAssetID = parentassetid
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomerChildAssets";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = customerid;

                    SqlDataReader or = oc.ExecuteReader();
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
                            DateInstalled = ConvertObjToDate(or["DateInstalled"]),
                            PurchasePrice = ConvertObjToDecimal(or["PurchasePrice"]),
                            Comments = or["Comments"].ToString() ?? string.Empty,
                            ChemicalsUsed = or["ChemicalsUsed"].ToString() ?? string.Empty,
                            DatePurchased = ConvertObjToDate(or["DatePurchased"]),
                            PONumber = or["PONumber"].ToString() ?? string.Empty,
                            AssetTypeID = ConvertObjToInt(or["TypeID"]),
                            AssetGroupID = ConvertObjToInt(or["GroupID"]),
                            LabelID = ConvertObjToInt(or["LabelID"]),
                            CustomerID = customerid,
                            SalesDivisionID = ConvertObjToInt(or["SalesDivisionID"]),
                            AssetAreaID = ConvertObjToInt(or["PrefixID"]),
                            StatusID = ConvertObjToInt(or["StatusID"]),
                            ParentAssetID = ConvertObjToInt(or["ParentAssetID"]),
                            NextAuditDate = ConvertObjToDate(or["NextAuditDate"]),
                            Dimensions = or["Dimensions"].ToString() ?? string.Empty,
                            SupplierName = or["SupplierName"].ToString() ?? string.Empty,
                            SAPID = or["SAPID"].ToString() ?? string.Empty
                        };
                        assets.Add(asset);
                    }
                    or.Close();
                }               
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetParentAssetLabel";
                    oc.Parameters.Add("@parentassetid", SqlDbType.Int).Value = parentassetid;
                    label = (string)oc.ExecuteScalar();
                    
                }                     
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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

            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomer";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        customer.ID = id;
                        customer.CustomerNumber = or["CustomerNumber"].ToString() ?? string.Empty;
                        customer.Name = or["Name"].ToString() ?? string.Empty;
                        customer.Location = or["Location"].ToString() ?? string.Empty;
                        customer.CountryID = ConvertObjToInt(or["CountryID"]);
                        customer.CorporationID = ConvertObjToInt(or["CorporationID"]);
                        customer.IsDeletable = ConvertObjToBool(or["IsDeletable"]);                                                   
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return customer;
        }
        
        public static FullyObservableCollection<CustomerModel> GetCustomers()
        {
            FullyObservableCollection<CustomerModel> customers = new FullyObservableCollection<CustomerModel>();            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomersWithLogo";
           
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        customers.Add(new CustomerModel
                        {
                            ID = Convert.ToInt32(or["ID"]),
                            CustomerNumber = or["CustomerNumber"].ToString() ?? string.Empty,
                            Name = or["Name"].ToString() ?? string.Empty,
                            Location = or["Location"].ToString() ?? string.Empty,                           
                            CountryID = ConvertObjToInt(or["CountryID"]),
                            CorporationID = ConvertObjToInt(or["CorporationID"]),
                            CountryName = or["Country"].ToString() ?? string.Empty,
                            Logo = (or["Logo"] == DBNull.Value) ? null : (byte[])or["Logo"],
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false,
                            IsEnabled = true
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return customers;
        }

        public static FullyObservableCollection<TVCustomerModel> GetTVCustomers()
        {
            FullyObservableCollection<TVCustomerModel> customers = new FullyObservableCollection<TVCustomerModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomersWithLogo";

                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        customers.Add(new TVCustomerModel
                        {
                            ID = Convert.ToInt32(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,                            
                            Logo = (or["Logo"] == DBNull.Value) ? null : (byte[])or["Logo"],
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
            administrator.ID = 0;
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetLoggedInUser";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = loginname;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        administrator.ID = ConvertObjToInt(or["ID"]);
                        administrator.Name = or["Name"].ToString() ?? string.Empty;
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return administrator;
        }

        public static FullyObservableCollection<CountryModel> GetCountries()
        {
            FullyObservableCollection<CountryModel> countries = new FullyObservableCollection<CountryModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCountries";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        countries.Add(new CountryModel
                        {
                            ID =  Convert.ToInt32(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            OperatingCompanyID = ConvertObjToInt(or["OperatingCompanyID"]),
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return countries;
        }
                
        public static FullyObservableCollection<SalesDivisionModel> GetSalesDivisions()
        {
            FullyObservableCollection<SalesDivisionModel> salesdivisions = new FullyObservableCollection<SalesDivisionModel>();         
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSalesDivisions";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        salesdivisions.Add(new SalesDivisionModel
                        {
                            ID =  Convert.ToInt32(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return salesdivisions;
        }

        public static FullyObservableCollection<AssetAreaModel> GetAssetAreas()
        {
            FullyObservableCollection<AssetAreaModel> assetareas = new FullyObservableCollection<AssetAreaModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetAreas";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        assetareas.Add(new AssetAreaModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Prefix = or["Prefix"].ToString() ?? string.Empty,
                            Name = or["Name"].ToString() ?? string.Empty,
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assetareas;
        }

        public static FullyObservableCollection<AssetGroupModel> GetAssetGroups()
        {
            FullyObservableCollection<AssetGroupModel> assetgroups = new FullyObservableCollection<AssetGroupModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetGroups";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        assetgroups.Add(new AssetGroupModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetAreaID = ConvertObjToInt(or["AssetPrefixID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            AssetGroupIDText = (string.IsNullOrEmpty(or["AssetGroupIDText"].ToString()) ? "00" : or["AssetGroupIDText"].ToString()),
                            CanBeParent = ConvertObjToBool(or["CanBeParent"]),
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assetgroups;
        }

        public static FullyObservableCollection<AssetTypeModel> GetAssetTypes()
        {
            FullyObservableCollection<AssetTypeModel> assettypes = new FullyObservableCollection<AssetTypeModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetTypes";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        assettypes.Add(new AssetTypeModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetGroupID = ConvertObjToInt(or["AssetGroupID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assettypes;
        }
        
        public static FullyObservableCollection<AssetGroupSpecificationValuesModel> GetAssetGroupSpecificationValues(int assetid, int assetgroupid)
        {
            FullyObservableCollection<AssetGroupSpecificationValuesModel> assetspecifications = new FullyObservableCollection<AssetGroupSpecificationValuesModel>();            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetGroupSpecificationValues";
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = assetid;
                    oc.Parameters.Add("@groupid", SqlDbType.Int).Value = assetgroupid;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        assetspecifications.Add(new AssetGroupSpecificationValuesModel
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
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assetspecifications;
        }

        public static FullyObservableCollection<AssetSpecificationModel> GetSpecifications()
        {
            FullyObservableCollection<AssetSpecificationModel> assetspecifications = new FullyObservableCollection<AssetSpecificationModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSpecifications";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        assetspecifications.Add(new AssetSpecificationModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            MeasurementUnitID = ConvertObjToInt(or["MeasurementUnitID"]),
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assetspecifications;
        }

        public static FullyObservableCollection<MaintenanceRecordModel> GetMaintenanceRecords(int assetid)
        {
            FullyObservableCollection<MaintenanceRecordModel> maintenancerecords = new FullyObservableCollection<MaintenanceRecordModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMaintenanceRecords";
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = assetid;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        maintenancerecords.Add(new MaintenanceRecordModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            MaintenanceDate = ConvertObjToDate(or["MaintenanceDate"]),
                            Name = or["Description"].ToString() ?? string.Empty,
                            Cost = ConvertObjToDecimal(or["Cost"]),
                            Completed = ConvertObjToBool( or["Completed"]),
                            MaintainedBy = or["MaintainedBy"].ToString() ?? string.Empty,
                            AssetID = assetid
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return maintenancerecords;
        }

        public static FullyObservableCollection<MaintenanceRecordModel> GetScheduledMaintenance(int assetid)
        {
            FullyObservableCollection<MaintenanceRecordModel> scheduledmaintenance = new FullyObservableCollection<MaintenanceRecordModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetScheduledMaintenance";
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = assetid;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        scheduledmaintenance.Add(new MaintenanceRecordModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            ScheduledMaintenanceDate = ConvertObjToDate(or["ScheduledMaintenanceDate"]),
                            Name = or["Description"].ToString() ?? string.Empty,
                            AssetID = assetid
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return scheduledmaintenance;
        }

        public static FullyObservableCollection<MaintenanceRecordModel> GetAllScheduledMaintenance()
        {
            FullyObservableCollection<MaintenanceRecordModel> scheduledmaintenance = new FullyObservableCollection<MaintenanceRecordModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAllScheduledMaintenance";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        scheduledmaintenance.Add(new MaintenanceRecordModel
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
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return scheduledmaintenance;
        }

        public static FullyObservableCollection<MaintenanceRecordModel> GetAllScheduledAssetAudits()
        {
            FullyObservableCollection<MaintenanceRecordModel> scheduledaudits = new FullyObservableCollection<MaintenanceRecordModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAllScheduledAssetAudits";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        scheduledaudits.Add(new MaintenanceRecordModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetID = ConvertObjToInt(or["AssetID"]),
                            ScheduledMaintenanceDate = ConvertObjToDate(or["AuditDate"]),
                            CustomerName = or["Customer"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return scheduledaudits;
        }

        public static FullyObservableCollection<MaintenanceTypeModel> GetMaintenanceTypes()
        {
            FullyObservableCollection<MaintenanceTypeModel> maintenancetypes = new FullyObservableCollection<MaintenanceTypeModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMaintenanceTypes";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        maintenancetypes.Add(new MaintenanceTypeModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return maintenancetypes;
        }

        public static FullyObservableCollection<OperatingCompanyModel> GetOperatingCompanies()
        {
            FullyObservableCollection<OperatingCompanyModel> operatingcompanies = new FullyObservableCollection<OperatingCompanyModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetOperatingCompany";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        operatingcompanies.Add(new OperatingCompanyModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return operatingcompanies;
        }

        public static FullyObservableCollection<CorporationModel> GetCorporations()
        {
            FullyObservableCollection<CorporationModel> corporations = new FullyObservableCollection<CorporationModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCorporations";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        corporations.Add(new CorporationModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            LogoID = ConvertObjToInt(or["LogoID"]),
                            Logo = (or["Logo"] == DBNull.Value) ? null : (byte[])or["Logo"],
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                            
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return corporations;
        }      
              
        private static AssetMovementReportModel ProcessMovementCode(int assetid, ActivityType activitycode, DateTime? datemoved, string label, string destinationcustomer, string sourcecustomer)
        {
            AssetMovementReportModel reportedmovement = new AssetMovementReportModel(); 
            if(datemoved !=null)
                reportedmovement.DateMoved = datemoved;
            reportedmovement.ID = assetid;
            reportedmovement.AssetLabel = label;
            switch (activitycode) {

                case ActivityType.NewAsset:                    
                        reportedmovement.Description = "New Asset " + label + " added to " + destinationcustomer;                   
                    break;
                    
                case ActivityType.Transfer:                                         
                        reportedmovement.Description = "Asset " + label + " moved from " + sourcecustomer + " to " + destinationcustomer;
                    break;

                case ActivityType.Deleted:
                        reportedmovement.Description = "Deleted Asset " + label + " from " + sourcecustomer;
                    break;

                case ActivityType.Undeleted:
                       reportedmovement.Description = "Undeleted Asset " + label + " and added to " + destinationcustomer;       
                    break;

            }            
            return reportedmovement;            
        }

        public static FullyObservableCollection<PhotoModel> GetAssetPhotos(int assetid)
        {
            FullyObservableCollection<PhotoModel> photos = new FullyObservableCollection<PhotoModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetPhotos";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assetid;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        photos.Add(new PhotoModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            PhotoFileName = or["PhotoFileName"].ToString() ?? string.Empty,
                            Photo = (or["Photo"] == DBNull.Value) ? null : (byte[])or["Photo"]
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return photos;
        }

        public static FullyObservableCollection<AssetModel> GetAssetConsumables(int assetid, bool selectused)
        {
            FullyObservableCollection<AssetModel> consumables = new FullyObservableCollection<AssetModel>();            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetConsumables";
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = assetid;
                    
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        consumables.Add(new AssetModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            ParentAssetID = assetid,
                            Name = or["Name"].ToString() ?? string.Empty,
                            PurchasePrice = ConvertObjToDecimal(or["Cost"]),
                            DatePurchased = ConvertObjToDate(or["DatePurchased"]),
                            PONumber = or["OrderReference"].ToString() ?? string.Empty,
                            SupplierName = or["Supplier"].ToString() ?? string.Empty,
                            //Used = ConvertObjToBool(or["Used"]),
                            Quantity = ConvertObjToInt(or["Quantity"])
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return consumables;
        }

        public static FullyObservableCollection<AssetSummaryModel> GetAvailableAssets(int statusid, int assetid)
        {
            FullyObservableCollection<AssetSummaryModel> availableassets = new FullyObservableCollection<AssetSummaryModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAvailableAssets";
                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = statusid;
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = assetid;
                    SqlDataReader or = oc.ExecuteReader();
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
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetGroupSpecifications";
                    oc.Parameters.Add("@groupid", SqlDbType.Int).Value = groupid;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        specifications.Add(new AssetGroupSpecificationModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            AssetGroupID = groupid,
                            SpecificationNameID  = ConvertObjToInt(or["SpecificationNameID"]),
                            PropertyUnitID = ConvertObjToInt(or["PropertyUnitID"]),
                            MeasurementUnitID = ConvertObjToInt(or["MeasurementUnitID"]),                         
                            SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty

                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAllAssetGroupSpecifications";
                    SqlDataReader or = oc.ExecuteReader();
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
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetGroupSpecificationsForDisplay";
                    oc.Parameters.Add("@groupid", SqlDbType.Int).Value = groupid;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        specifications.Add(new AssetGroupSpecDisplayDataModel
                        {
                            AssetGroupSpecificationID = ConvertObjToInt(or["ID"]),
                            AssetGroupID = groupid,
                            SpecificationName = or["Name"].ToString() ?? string.Empty,
                            SpecificationPropertyName = or["PropertyUnit"].ToString() ?? string.Empty,
                            SpecificationOptions = or["SpecificationOptions"].ToString() ?? string.Empty,
                            DataTypeID = ConvertObjToInt(or["MeasurementUnitID"])                                         
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return specifications;
        }

        public static FullyObservableCollection<SearchFieldModel> GetSearchFields()
        {
            FullyObservableCollection<SearchFieldModel> searchfields = new FullyObservableCollection<SearchFieldModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSearchFields";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        searchfields.Add(new SearchFieldModel
                        {                            
                            Label = or["Label"].ToString() ?? string.Empty,
                            TableName = or["TableName"].ToString() ?? string.Empty,
                            FieldName = or["FieldName"].ToString() ?? string.Empty,
                            Mask = or["Mask"].ToString() ?? string.Empty
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return searchfields;
        }
        
        public static Collection<ReportModel> GetReports()
         {
            Collection<ReportModel> reports = new Collection<ReportModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetReports";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        reports.Add(new ReportModel
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
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return reports;
        }        

        public static FullyObservableCollection<SpecificationPropertyModel> GetSpecificationProperties()
        {
            FullyObservableCollection<SpecificationPropertyModel> properties = new FullyObservableCollection<SpecificationPropertyModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetSpecificationProperties";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        properties.Add(new SpecificationPropertyModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            PropertyUnit = or["PropertyUnit"].ToString() ?? string.Empty,
                            IsDeletable = ConvertObjToBool(or["IsDeletable"]),
                            Selected = false
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return properties;
        }
                  
        public static Dictionary<int, int> GetAssetSearch(string tablename, string fieldname, string searchword)
        {
            Dictionary<int, int> assetids = new Dictionary<int, int>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "SearchAssetField";
                    oc.Parameters.Add("@tblname", SqlDbType.NVarChar, descriptionlength).Value = tablename ?? string.Empty;
                    oc.Parameters.Add("@fldname", SqlDbType.NVarChar, descriptionlength).Value = fieldname ?? string.Empty;
                    oc.Parameters.Add("@searchword", SqlDbType.NVarChar, descriptionlength).Value = searchword ?? string.Empty;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        assetids.Add(ConvertObjToInt(or["ID"]), ConvertObjToInt(or["ID"]));
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return assetids;
        }

        //public static Dictionary<int, int> GetAssetSearch(string queryname, int id)
        //{
        //    Dictionary<int, int> assetids = new Dictionary<int, int>();
            
        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = queryname;
        //            oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
        //            SqlDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //            while (or.Read())
        //            {
        //                assetids.Add(ConvertObjToInt(or["ID"]), ConvertObjToInt(or["ID"]));
        //            }
        //            or.Close();
        //        }
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return assetids;
        //}

        //public static Dictionary<int, int> GetAssetSearch(string queryname, string criteria, int specificationid)
        //{
        //    Dictionary<int, int> assetids = new Dictionary<int, int>();
            
        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = queryname;
        //            oc.Parameters.Add("@string",SqlDbType.NVarChar, descriptionlength).Value = criteria;
        //            oc.Parameters.Add("@specificationid", SqlDbType.Int).Value = specificationid;
        //            SqlDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //            while (or.Read())
        //            {
        //                assetids.Add(ConvertObjToInt(or["ID"]), ConvertObjToInt(or["ID"]));
        //            }
        //            or.Close();
        //        }
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return assetids;
        //}

        //public static Dictionary<int, int> GetAssetSearch(string queryname, DateTime? startdate, DateTime? enddate)
        //{
        //    Dictionary<int, int> assetids = new Dictionary<int, int>();
            
        //    try
        //    {
        //        using (SqlCommand oc = new SqlCommand())
        //        {
        //            oc.Connection = Conn;
        //            oc.CommandType = CommandType.StoredProcedure;
        //            oc.CommandText = queryname;
        //            oc.Parameters.Add("@startdate", SqlDbType.Date).Value = startdate;
        //            oc.Parameters.Add("@enddate",SqlDbType.Date).Value = enddate;
        //            SqlDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
        //            while (or.Read())
        //            {
        //                assetids.Add(ConvertObjToInt(or["ID"]), ConvertObjToInt(or["ID"]));
        //            }
        //            or.Close();
        //        }
        //    }
        //    catch (SqlException sqle)
        //    {
        //        HandleSQLError(sqle);
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //    }
        //    return assetids;
        //}              
        
        public static FullyObservableCollection<AdministratorUserModel> GetAdministrators()
        {
            FullyObservableCollection<AdministratorUserModel> administrators = new FullyObservableCollection<AdministratorUserModel>();
            
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAdministrators";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        administrators.Add(new AdministratorUserModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            Name = or["Name"].ToString() ?? string.Empty,
                            LoginName = or["UserName"].ToString() ?? string.Empty,
                            IsDeletable=true
                        });
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return administrators;
        }
              
        public static int[] GetLabelMask()
        {
            int[] mask = new int[3];
                        
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetLabelMask";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {                
                        mask[0] = ConvertObjToInt(or["AreaChars"]);
                        mask[1] = ConvertObjToInt(or["GroupChars"]);
                        mask[2] = (ConvertObjToInt(or["MaximumIDValue"]) - 1).ToString().Length;
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return mask;
        }

        public static FullyObservableCollection<AuditDateModel> GetAssetAuditDates(int assetid)
        {
            AuditDateModel nextaudit;
            FullyObservableCollection<AuditDateModel> audits = new FullyObservableCollection<AuditDateModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetAuditDates";
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = assetid;
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        nextaudit = new AuditDateModel
                        {
                            ID = ConvertObjToInt(or["ID"]),
                            DateAudit = (DateTime)or["AuditDate"]
                        };
                        audits.Add(nextaudit);
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return audits;
        }

        public static DefaultSettingsModel GetDefaultSettings()
        {
            DefaultSettingsModel defsettings = new DefaultSettingsModel();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetDefaultSettings";
                    SqlDataReader or = oc.ExecuteReader();
                    while (or.Read())
                    {
                        defsettings.MaxPhotoSize = ConvertObjToInt(or["MaxPhotoSize"]); 
                        defsettings.PhotoHeightAndWidth = ConvertObjToInt(or["PhotoHeightAndWidth"]);
                        defsettings.TargetPhotoQuality = ConvertObjToInt(or["TargetPhotoQuality"]);
                    }
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return defsettings;
        }

        public static FullyObservableCollection<BaseModel> GetStatuses()
        {
            FullyObservableCollection<BaseModel> items = new FullyObservableCollection<BaseModel>();

            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetStatuses";
                    SqlDataReader or = oc.ExecuteReader();
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
                    or.Close();
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return items;
        }

        public static FullyObservableCollection<CustomReportModel> GetCustomReports()
        {
            FullyObservableCollection<CustomReportModel> customreports = new FullyObservableCollection<CustomReportModel>();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomReports";
                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            customreports.Add(new CustomReportModel
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Name = or["Name"].ToString() ?? string.Empty,
                                SPName = or["SPName"].ToString() ?? string.Empty,
                                CombineTables = ConvertObjToBool(or["CombineTables"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetCustomReportParameters";
                    oc.Parameters.Add("@customreportid", SqlDbType.Int).Value = customreportid;
                    CustomReportParametersModel cm;

                    using (SqlDataReader or = oc.ExecuteReader())
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
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = custreport.SPName;

                    if (parameters.Count > 0)
                    {
                        for (int i = 0; i < parameters.Count; i++)
                        {
                            switch (parameters[i].ParameterType)
                            {
                                case "String":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.NVarChar, descriptionlength).Value = parameters[i].Value ?? string.Empty;
                                    break;

                                case "DateTime":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.Date).Value = Convert.ToDateTime(parameters[i].Value);
                                    break;

                                case "Int32":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.Int).Value = Convert.ToInt32(parameters[i].Value);
                                    break;

                                case "Decimal":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.Decimal).Value = Convert.ToDecimal(parameters[i].Value);
                                    break;

                                case "Boolean":
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.Bit).Value = ConvertObjToBool(parameters[i].Value);
                                    break;

                                default:
                                    oc.Parameters.Add(parameters[i].Name, SqlDbType.NVarChar, descriptionlength).Value = parameters[i].Value ?? string.Empty;
                                    break;
                            }
                        }
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(oc))
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
                                if (f.FieldName == dc.ColumnName)
                                {
                                    dc.Caption = f.Caption;
                                    //dc.DataType = System.Type.GetType("System." + f.DataType); <======cannot change datatype after column is filled
                                    dc.ExtendedProperties.Add("Alignment", f.Alignment);
                                    dc.ExtendedProperties.Add("Format", f.Format);
                                    dc.ExtendedProperties.Add("FieldType", f.FieldType);
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
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetReportFields";
                    oc.Parameters.Add("@reportname", SqlDbType.NVarChar, namelength).Value = reportname;

                    using (SqlDataReader or = oc.ExecuteReader())
                    {
                        while (or.Read())
                        {
                            fields.Add(new CustomReportFieldsModel()
                            {
                                ID = ConvertObjToInt(or["ID"]),
                                Caption = or["Caption"].ToString() ?? string.Empty,
                                FieldName = or["Name"].ToString() ?? string.Empty,
                                DataType = or["DataType"].ToString() ?? string.Empty,
                                Alignment = or["Alignment"].ToString() ?? string.Empty,
                                Format = or["Format"].ToString() ?? string.Empty,
                                FieldType = ConvertObjToInt(or["FieldType"]),
                                System = ConvertObjToBool(or["System"]),
                                DataTypeID = ConvertObjToInt(or["DataTypeID"])
                            });
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                HandleSQLError(sqle);
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
                using (SqlCommand oc = new SqlCommand())
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetAssetMovementsReport";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assetid;
                    SqlDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {
                        reportedmovements.Add(ProcessMovementCode(assetid, (ActivityType)Convert.ToInt32(or["ActivityCodeID"]), ConvertObjToDate(or["DateMoved"]),
                            or["Label"].ToString(), or["Destination"].ToString(), or["Source"].ToString()));
                                                                       
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return reportedmovements;
        }

        public static DataTable GetDateFilteredMovementReportData(string month)
        {
            DataTable reportdata = new DataTable("MovementsReport");
            DataColumn dc;
            DataRow dr;

            dc = new DataColumn
            {
                Caption = "Asset ID",
                ColumnName = "ID",
                DataType = System.Type.GetType("System.Int32")
            };
            reportdata.Columns.Add(dc);

            dc = new DataColumn
            {
                Caption = "BAAN ID",
                ColumnName = "SAPID",
                DataType = System.Type.GetType("System.Int32")
            };
            reportdata.Columns.Add(dc);

            dc = new DataColumn
            {
                Caption = "Date Moved",
                ColumnName = "DateMoved",
                DataType = System.Type.GetType("System.DateTime")
            };
            reportdata.Columns.Add(dc);

            dc = new DataColumn
            {
                Caption = "Label",
                ColumnName = "Label",
                DataType = System.Type.GetType("System.String")
            };
            reportdata.Columns.Add(dc);

            dc = new DataColumn
            {
                Caption = "Description",
                ColumnName = "Description",
                DataType = System.Type.GetType("System.String")
            };
            reportdata.Columns.Add(dc);

            AssetMovementReportModel reportedmovement = new AssetMovementReportModel();
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                                    
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = "GetMovementsReport";
                    oc.Parameters.Add("@monthmoved",SqlDbType.NVarChar, namelength).Value = month;

                    SqlDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {                       
                        reportedmovement = (ProcessMovementCode(ConvertObjToInt(or["AssetID"]), (ActivityType)Convert.ToInt32(or["ActivityCodeID"]), ConvertObjToDate(or["DateMoved"]),
                           or["Label"].ToString(), or["DestinationCustomer"].ToString(), or["SourceCustomer"].ToString()));
                        dr = reportdata.NewRow();
                        dr["ID"] = reportedmovement.ID;
                        if(!string.IsNullOrEmpty(or["SAPID"].ToString()))
                            dr["SAPID"] = Convert.ToInt32(or["SAPID"]);                         
                        dr["DateMoved"] = reportedmovement.DateMoved;
                        dr["Label"] = reportedmovement.AssetLabel;
                        dr["Description"] = reportedmovement.Description;
                        reportdata.Rows.Add(dr);
                    }
                    or.Close();
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return reportdata;
        }

        public static DataTable GetDateFilteredReportData(string reportname, string month)
        {
            DataTable reportdata = new DataTable(reportname);                      
            try
            {
                using (SqlCommand oc = new SqlCommand())
                {
                                    
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    oc.CommandText = reportname;
                    oc.Parameters.Add("@month",SqlDbType.NVarChar, namelength).Value = month;
                    
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = oc;
                    da.Fill(reportdata);
                    da.Dispose();
                    
                }                
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            return reportdata;
        }

        #endregion
        
        #region Insert Queries
        
        public static int AddAsset(AssetModel asset)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddAsset";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namelength).Value = asset.Name ?? string.Empty;
                    oc.Parameters.Add("@applicationtype", SqlDbType.NVarChar, descriptionlength).Value = asset.ApplicationType ?? string.Empty;
                    oc.Parameters.Add("@sitelocation", SqlDbType.NVarChar, namelength).Value = asset.SiteLocation ?? string.Empty;
                    oc.Parameters.Add("@manufacturername", SqlDbType.NVarChar, namelength).Value = asset.ManufacturerName ?? string.Empty;
                    oc.Parameters.Add("@modelno", SqlDbType.NVarChar, namelength).Value = asset.ModelNo ?? string.Empty;
                    oc.Parameters.Add("@serialno", SqlDbType.NVarChar, namelength).Value = asset.SerialNo ?? string.Empty;
                    if (asset.DateInstalled == null)
                        oc.Parameters.Add("@dateinstalled", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@dateinstalled", SqlDbType.Date).Value = asset.DateInstalled;
                    oc.Parameters.Add("@purchaseprice", SqlDbType.Decimal).Value = asset.PurchasePrice;
                    oc.Parameters.Add("@comments", SqlDbType.NVarChar, descriptionlength).Value = asset.Comments ?? string.Empty;
                    oc.Parameters.Add("@chemicalsused", SqlDbType.NVarChar, descriptionlength).Value = asset.ChemicalsUsed ?? string.Empty;
                    oc.Parameters.Add("@datepurchased", SqlDbType.Date).Value = asset.DatePurchased;
                    oc.Parameters.Add("@ponumber", SqlDbType.NVarChar, namelength).Value = asset.PONumber ?? string.Empty;
                    oc.Parameters.Add("@assettypeid", SqlDbType.Int).Value = asset.AssetTypeID;
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = asset.CustomerID;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = asset.SalesDivisionID;
                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = asset.StatusID;
                    oc.Parameters.Add("@parentassetid", SqlDbType.Int).Value = asset.ParentAssetID;
                    oc.Parameters.Add("@assetgroupid", SqlDbType.Int).Value = asset.AssetGroupID;
                    oc.Parameters.Add("@prefixid", SqlDbType.Int).Value = asset.AssetAreaID;
                    oc.Parameters.Add("@labelid", SqlDbType.Int).Value = asset.LabelID;
                    oc.Parameters.Add("@dimensions", SqlDbType.NVarChar, namelength).Value = asset.Dimensions ?? string.Empty;
                    oc.Parameters.Add("@suppliername", SqlDbType.NVarChar, namelength).Value = asset.SupplierName ?? string.Empty;
                    oc.Parameters.Add("@sapid", SqlDbType.NVarChar, SAPIDlength).Value = asset.SAPID ?? string.Empty;
                    oc.Parameters.Add("@quantity", SqlDbType.Int).Value = 1;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }

                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;            
        }
                
        public static int AddCustomer(CustomerModel customer)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddCustomer";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@customernumber", SqlDbType.NVarChar, namelength).Value = customer.CustomerNumber ?? string.Empty; ;
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namelength).Value = customer.Name ?? string.Empty; ;
                    oc.Parameters.Add("@location", SqlDbType.NVarChar, namelength).Value = customer.Location ?? string.Empty; ;
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = customer.CountryID;
                    oc.Parameters.Add("@corporationid", SqlDbType.Int).Value = customer.CorporationID;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddCorporationLogo(int corporationid, byte[] logo)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddCorporationLogo";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@corporationid", SqlDbType.Int).Value = corporationid;
                    oc.Parameters.Add("@logo", SqlDbType.VarBinary).Value = logo;                    
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddAdministrator(AdministratorUserModel administrator)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddAdministrator";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = administrator.Name ?? string.Empty; ;
                    oc.Parameters.Add("@username",SqlDbType.NVarChar, namelength).Value = administrator.LoginName ?? string.Empty; ;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddCountry(CountryModel country)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddCountry";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = country.Name ?? string.Empty; ;
                    oc.Parameters.Add("@operatingcompanyid", SqlDbType.Int).Value = country.OperatingCompanyID;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddSalesDivision(SalesDivisionModel salesdivision)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddSalesDivision";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = salesdivision.Name ?? string.Empty; ;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddMaintenanceRecord(MaintenanceRecordModel maintenancerecord)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddMaintenanceRecord";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = maintenancerecord.AssetID;
                    oc.Parameters.Add("@maintenancedate",SqlDbType.Date).Value = maintenancerecord.MaintenanceDate;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, descriptionlength).Value = maintenancerecord.Name ?? string.Empty; ;
                    oc.Parameters.Add("@cost", SqlDbType.Decimal).Value = maintenancerecord.Cost;
                    oc.Parameters.Add("@maintainedby",SqlDbType.NVarChar, namelength).Value = maintenancerecord.MaintainedBy ?? string.Empty; ;
                    oc.Parameters.Add("@completed", SqlDbType.Bit).Value = maintenancerecord.Completed;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddScheduledMaintenance(MaintenanceRecordModel scheduledmaintenance)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddScheduledMaintenance";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = scheduledmaintenance.AssetID;
                    oc.Parameters.Add("@scheduledmaintenancedate", SqlDbType.Date).Value = scheduledmaintenance.ScheduledMaintenanceDate;
                    oc.Parameters.Add("@description",SqlDbType.NVarChar, descriptionlength).Value = scheduledmaintenance.Name ?? string.Empty; ;
                    oc.Parameters.Add("@cost", SqlDbType.Decimal).Value = scheduledmaintenance.Cost;
                    oc.Parameters.Add("@maintainedby", SqlDbType.NVarChar, descriptionlength).Value = scheduledmaintenance.MaintainedBy ?? string.Empty; ;
                    oc.Parameters.Add("@completed", SqlDbType.Bit).Value = scheduledmaintenance.Completed;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddAssetArea(AssetAreaModel assetarea)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddAssetArea";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@prefix",SqlDbType.NVarChar, prefixlength).Value = assetarea.Prefix ?? string.Empty; ;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, descriptionlength).Value = assetarea.Name ?? string.Empty; ;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddAssetGroup(AssetGroupModel assetgroup)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddAssetGroup";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@assetprefixid", SqlDbType.Int).Value = assetgroup.AssetAreaID;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = assetgroup.Name ?? string.Empty; ;
                    oc.Parameters.Add("@assetgrouptext",SqlDbType.NVarChar, 255).Value = assetgroup.AssetGroupIDText ?? string.Empty; ;
                    oc.Parameters.Add("@canbeparent", SqlDbType.Bit).Value = assetgroup.CanBeParent;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddAssetType(AssetTypeModel assettype)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddAssetType";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@assetgroupid", SqlDbType.Int).Value = assettype.AssetGroupID;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, descriptionlength).Value = assettype.Name ?? string.Empty; ;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddAssetMovement(AssetMovementModel assetmovement)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddAssetMovement";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@datemoved",SqlDbType.DateTime).Value = assetmovement.DateMoved;
                    oc.Parameters.Add("@activitycodeid", SqlDbType.Int).Value = assetmovement.ActivityCodeID;
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = assetmovement.AssetID;
                    oc.Parameters.Add("@sourceid", SqlDbType.Int).Value = assetmovement.SourceCustomerID;
                    oc.Parameters.Add("@destinationid", SqlDbType.Int).Value = assetmovement.DestinationCustomerID;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddOperatingCompany(OperatingCompanyModel opco)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddOperatingCompany";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = opco.Name ?? string.Empty; ;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddCorporation(CorporationModel opco)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddCorporation";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = opco.Name ?? string.Empty; ;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }
        
        public static int AddPhoto(PhotoModel photo)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddPhoto";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = photo.AssetID;
                    oc.Parameters.Add("@photofilename",SqlDbType.NVarChar, filenamelength).Value = photo.PhotoFileName ?? string.Empty;
                    oc.Parameters.Add("@photo", SqlDbType.VarBinary).Value = photo.Photo;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddSpecification(AssetSpecificationModel spec)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddSpecification";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = spec.Name ?? string.Empty; ;
                    oc.Parameters.Add("@measurementunitid", SqlDbType.Int).Value = spec.MeasurementUnitID;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }
        
        public static int AddSpecificationProperty(SpecificationPropertyModel specprop)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddSpecificationProperty";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@specpropertyunit",SqlDbType.NVarChar, namelength).Value = specprop.PropertyUnit ?? string.Empty; ;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddConsumable(AssetModel consumable)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddConsumable";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = consumable.ParentAssetID;
                    oc.Parameters.Add("@description",SqlDbType.NVarChar, descriptionlength).Value = consumable.Name ?? string.Empty;
                    oc.Parameters.Add("@cost", SqlDbType.Decimal).Value = consumable.PurchasePrice;
                    oc.Parameters.Add("@datepurchased",SqlDbType.Date).Value = consumable.DatePurchased;
                    oc.Parameters.Add("@orderreference",SqlDbType.NVarChar, namelength).Value = consumable.PONumber ?? string.Empty;
                    oc.Parameters.Add("@supplier",SqlDbType.NVarChar, namelength).Value = consumable.SupplierName ?? string.Empty;
                    //oc.Parameters.Add("@used", SqlDbType.Bit).Value = consumable.Used;
                    oc.Parameters.Add("@quantity", SqlDbType.Int).Value = consumable.Quantity;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddAssetGroupSpecification(AssetGroupSpecificationModel specification)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddAssetGroupSpecification";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@assetgroupid", SqlDbType.Int).Value = specification.AssetGroupID;
                    oc.Parameters.Add("@specificationnameid", SqlDbType.Int).Value = specification.SpecificationNameID;
                    oc.Parameters.Add("@propertyunitid", SqlDbType.Int).Value = specification.PropertyUnitID;
                    oc.Parameters.Add("@specificationoptions",SqlDbType.NVarChar, descriptionlength).Value = specification.SpecificationOptions ?? string.Empty; ;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddAuditDate(AuditDateModel audit)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "AddAuditDate";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@auditdate",SqlDbType.Date).Value = audit.DateAudit;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = audit.AssetID;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }

        public static int AddAssetGroupSpecificationValue(AssetGroupSpecificationValuesModel spec)
        {
            int insertedid = -1;
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;                   
                    oc.CommandText = "AddAssetGroupSpecificationValue";
                    oc.Parameters.Add("@CaseID", SqlDbType.Int);
                    oc.Parameters.Add("@specificationvalue", SqlDbType.NVarChar, descriptionlength).Value = spec.SpecificationValue ?? string.Empty;
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = spec.AssetID;
                    oc.Parameters.Add("@assetgroupid", SqlDbType.Int).Value = spec.AssetGroupID;
                    oc.Parameters.Add("@groupspecificationid", SqlDbType.Int).Value = spec.ID;
                    oc.Parameters["@CaseID"].Direction = ParameterDirection.Output;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                    insertedid = Convert.ToInt32(oc.Parameters["@CaseID"].Value);                   
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return insertedid;
        }


        #endregion

        #region Update Queries

        public static void UpdateAsset(AssetModel asset)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateAsset";
                    oc.Parameters.Add("@name", SqlDbType.NVarChar, namelength).Value = asset.Name ?? string.Empty;
                    oc.Parameters.Add("@applicationtype", SqlDbType.NVarChar, descriptionlength).Value = asset.ApplicationType ?? string.Empty;
                    oc.Parameters.Add("@sitelocation", SqlDbType.NVarChar, namelength).Value = asset.SiteLocation ?? string.Empty;
                    oc.Parameters.Add("@manufacturername", SqlDbType.NVarChar, namelength).Value = asset.ManufacturerName ?? string.Empty;
                    oc.Parameters.Add("@modelno", SqlDbType.NVarChar, namelength).Value = asset.ModelNo ?? string.Empty;
                    oc.Parameters.Add("@serialno", SqlDbType.NVarChar, namelength).Value = asset.SerialNo ?? string.Empty;
                    if (asset.DateInstalled == null)
                        oc.Parameters.Add("@dateinstalled", SqlDbType.Date).Value = DBNull.Value;
                    else
                        oc.Parameters.Add("@dateinstalled", SqlDbType.Date).Value = asset.DateInstalled;
                    oc.Parameters.Add("@purchaseprice", SqlDbType.Decimal).Value = asset.PurchasePrice;
                    oc.Parameters.Add("@comments", SqlDbType.NVarChar, descriptionlength).Value = asset.Comments ?? string.Empty;
                    oc.Parameters.Add("@chemicalsused", SqlDbType.NVarChar, descriptionlength).Value = asset.ChemicalsUsed ?? string.Empty;
                    oc.Parameters.Add("@datepurchased", SqlDbType.Date).Value = asset.DatePurchased;
                    oc.Parameters.Add("@ponumber", SqlDbType.NVarChar, namelength).Value = asset.PONumber ?? string.Empty;
                    oc.Parameters.Add("@assettypeid", SqlDbType.Int).Value = asset.AssetTypeID;
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = asset.CustomerID;
                    oc.Parameters.Add("@salesdivisionid", SqlDbType.Int).Value = asset.SalesDivisionID;
                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = asset.StatusID;
                    oc.Parameters.Add("@parentassetid", SqlDbType.Int).Value = asset.ParentAssetID;
                    oc.Parameters.Add("@assetgroupid", SqlDbType.Int).Value = asset.AssetGroupID;
                    oc.Parameters.Add("@prefixid", SqlDbType.Int).Value = asset.AssetAreaID;
                    oc.Parameters.Add("@labelid", SqlDbType.Int).Value = asset.LabelID;
                    oc.Parameters.Add("@dimensions", SqlDbType.NVarChar, namelength).Value = asset.Dimensions ?? string.Empty;
                    oc.Parameters.Add("@suppliername", SqlDbType.NVarChar, namelength).Value = asset.SupplierName ?? string.Empty;
                    oc.Parameters.Add("@sapid", SqlDbType.NVarChar, SAPIDlength).Value = asset.SAPID ?? string.Empty;
                    oc.Parameters.Add("@quantity", SqlDbType.Int).Value = 1;
                    oc.Parameters.Add("@deleted", SqlDbType.Bit).Value = 0;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = asset.ID;
      
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateChildAsset(int assetid, int customerid, int statusid)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateChildAsset";
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = statusid;
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = assetid;                    
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateCustomer(CustomerModel customer)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateCustomer";
                    oc.Parameters.Add("@customernumber",SqlDbType.NVarChar, namelength).Value = customer.CustomerNumber ?? string.Empty;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = customer.Name ?? string.Empty;
                    oc.Parameters.Add("@location",SqlDbType.NVarChar, namelength).Value = customer.Location ?? string.Empty; ;
                    oc.Parameters.Add("@countryid", SqlDbType.Int).Value = customer.CountryID;
                    oc.Parameters.Add("@corporationid", SqlDbType.Int).Value = customer.CorporationID;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = customer.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateCorporationLogo(int corporationid, byte[] logo)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateCorporationLogo";
                    oc.Parameters.Add("@logo", SqlDbType.VarBinary).Value = logo;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = corporationid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateAdministrator(AdministratorUserModel administrator)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateAdministrator";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = administrator.Name ?? string.Empty;
                    oc.Parameters.Add("@username",SqlDbType.NVarChar, namelength).Value = administrator.LoginName ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = administrator.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateCountry(CountryModel country)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateCountry";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = country.Name ?? string.Empty;
                    oc.Parameters.Add("@operatingcompanyid", SqlDbType.Int).Value = country.OperatingCompanyID;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = country.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateSalesDivision(SalesDivisionModel salesdivision)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateSalesDivision";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = salesdivision.Name ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = salesdivision.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateOperatingCompany(OperatingCompanyModel operatingcompany)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateOperatingCompany";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = operatingcompany.Name ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = operatingcompany.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateMaintenanceRecord(MaintenanceRecordModel maintenancerecord)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateMaintenanceRecord";
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = maintenancerecord.AssetID;
                    oc.Parameters.Add("@maintenancedate",SqlDbType.Date).Value = maintenancerecord.MaintenanceDate;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, descriptionlength).Value = maintenancerecord.Name ?? string.Empty; ;
                    oc.Parameters.Add("@cost", SqlDbType.Decimal).Value = maintenancerecord.Cost;
                    oc.Parameters.Add("@maintainedby",SqlDbType.NVarChar, namelength).Value = maintenancerecord.MaintainedBy ?? string.Empty;
                    oc.Parameters.Add("@completed", SqlDbType.Bit).Value = maintenancerecord.Completed;
                    oc.Parameters.Add("@datescheduled",SqlDbType.Date).Value = maintenancerecord.ScheduledMaintenanceDate;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = maintenancerecord.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateMaintenanceType(MaintenanceTypeModel maintenancetype)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateMaintenanceType";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = maintenancetype.Name ?? string.Empty; ;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = maintenancetype.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateAssetArea(AssetAreaModel assetarea)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateAssetArea";
                    oc.Parameters.Add("@prefix",SqlDbType.NVarChar, prefixlength).Value = assetarea.Prefix ?? string.Empty; ;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, descriptionlength).Value = assetarea.Name ?? string.Empty; ;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assetarea.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateAssetGroup(AssetGroupModel assetgroup)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateAssetGroup";
                    oc.Parameters.Add("@assetprefix", SqlDbType.Int).Value = assetgroup.AssetAreaID;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = assetgroup.Name ?? string.Empty; ;
                    oc.Parameters.Add("@assetgrouptext",SqlDbType.NVarChar, prefixlength).Value = assetgroup.AssetGroupIDText;
                    oc.Parameters.Add("@canbeparent", SqlDbType.Bit).Value = assetgroup.CanBeParent;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assetgroup.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateAssetType(AssetTypeModel assettype)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateAssetType";
                    oc.Parameters.Add("@assetgroupid", SqlDbType.Int).Value = assettype.AssetGroupID;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, descriptionlength).Value = assettype.Name ?? string.Empty; ;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assettype.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateCorporation(CorporationModel corporation)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateCorporation";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = corporation.Name ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = corporation.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateAssetPhoto(PhotoModel photo)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateAssetPhoto";
                    oc.Parameters.Add("@photofilename",SqlDbType.NVarChar, filenamelength).Value = photo.PhotoFileName ?? string.Empty;
                    oc.Parameters.Add("@photo", SqlDbType.VarBinary).Value = photo.Photo;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = photo.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateSpecification(AssetSpecificationModel spec)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateSpecification";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = spec.Name ?? string.Empty; ;
                    oc.Parameters.Add("@measurementunitid", SqlDbType.Int).Value = spec.MeasurementUnitID;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = spec.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateAssetGroupSpecification(AssetGroupSpecificationModel spec)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateAssetGroupSpecification";
                    oc.Parameters.Add("@assetgroupid", SqlDbType.Int).Value = spec.AssetGroupID;
                    oc.Parameters.Add("@specificationnameid", SqlDbType.Int).Value = spec.SpecificationNameID;
                    oc.Parameters.Add("@propertyunitid", SqlDbType.Int).Value = spec.PropertyUnitID;
                    oc.Parameters.Add("@specificationoptions",SqlDbType.NVarChar, descriptionlength).Value = spec.SpecificationOptions ?? string.Empty; ;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = spec.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateAssetGroupSpecificationValue(AssetGroupSpecificationValuesModel spec)
        {            
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;                                        
                    oc.CommandText = "UpdateAssetGroupSpecificationValue";
                    oc.Parameters.Add("@specificationvalue",SqlDbType.NVarChar, descriptionlength).Value = spec.SpecificationValue ?? string.Empty;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = spec.AssetGroupSpecificationValueID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();                    
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateSpecificationProperty(SpecificationPropertyModel specprop)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateSpecificationProperty";
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, namelength).Value = specprop.PropertyUnit ?? string.Empty; ;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = specprop.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateConsumable(AssetModel consumable)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateConsumable";
                    //oc.Parameters.Add("@assetid", SqlDbType.Int).Value = consumable.AssetID;
                    oc.Parameters.Add("@name",SqlDbType.NVarChar, descriptionlength).Value = consumable.Name ?? string.Empty;
                    oc.Parameters.Add("@cost", SqlDbType.Decimal).Value = consumable.PurchasePrice;
                    oc.Parameters.Add("@datepurchased",SqlDbType.Date).Value = consumable.DatePurchased;
                    oc.Parameters.Add("@orderreference",SqlDbType.NVarChar, namelength).Value = consumable.PONumber ?? string.Empty;
                    oc.Parameters.Add("@supplier",SqlDbType.NVarChar, namelength).Value = consumable.SupplierName ?? string.Empty;
                    //oc.Parameters.Add("@used", SqlDbType.Bit).Value = consumable.Used;
                    oc.Parameters.Add("@quantity", SqlDbType.Int).Value = consumable.Quantity;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = consumable.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateParentAssetID(int assetid, int parentassetid, int customerid)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateParentAssetID";
                    oc.Parameters.Add("@parentassetid", SqlDbType.Int).Value = parentassetid;
                    oc.Parameters.Add("@customerid", SqlDbType.Int).Value = customerid;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assetid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UnDeleteAsset(int assetid, int defaultcustomerid, int statusid)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UnDeleteAsset";
                    oc.Parameters.Add("@defaultcustomerid", SqlDbType.Int).Value = defaultcustomerid;
                    oc.Parameters.Add("@statusid", SqlDbType.Int).Value = statusid;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assetid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void UpdateAuditDate(AuditDateModel audit)
        {
            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "UpdateAuditDate";
                    oc.Parameters.Add("@nextauditdate",SqlDbType.Date).Value = audit.DateAudit;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = audit.ID;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void SetDefaultCustomer(int defaultcustomerid)
        {

            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "SetDefaultCustomer";
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = defaultcustomerid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void ResetDefaultCustomer()
        {

            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "ResetDefaultCustomer";
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Delete Queries

        public static void RemoveChildAsset(int childassetid)
        {

            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "RemoveChildAsset";
                    oc.Parameters.Add("@assetid", SqlDbType.Int).Value = childassetid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void SetConsumableToUsed(int consumableid)
        {

            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "SetConsumableToUsed";
                    oc.Parameters.Add("@consumableid", SqlDbType.Int).Value = consumableid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void DeleteItem(int id, DeleteSPName sp)
        {

            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = sp.ToString();
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        public static void SetParentAssetID(int assetid, int parentassetid)
        {

            using (SqlCommand oc = new SqlCommand())
            {
                try
                {
                    oc.Connection = Conn;
                    oc.CommandType = CommandType.StoredProcedure;
                    SqlTransaction transaction;
                    transaction = Conn.BeginTransaction("LocalTransaction");
                    oc.Transaction = transaction;
                    oc.CommandText = "SetParentAssetID";
                    oc.Parameters.Add("@parentassetid", SqlDbType.Int).Value = parentassetid;
                    oc.Parameters.Add("@id", SqlDbType.Int).Value = assetid;
                    oc.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (SqlException sqle)
                {
                    HandleSQLError(sqle);
                }
                catch (Exception e)
                {
                    ShowError(e);
                    try
                    {
                        oc.Transaction.Rollback();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        public static DateTime? ConvertDateToMonth(DateTime? dt)
        {
            if (dt == null) return null;
            return new DateTime(((DateTime)dt).Year, ((DateTime)dt).Month, 1);
        }

        private static int ConvertObjToInt(object obj)
        {
            bool isnumber = int.TryParse(obj.ToString(), out int id);
            return id;
        }

        private static decimal ConvertObjToDecimal(object obj)
        {
            bool isnumber = decimal.TryParse(obj.ToString(), out decimal id);
            return id;
        }

        private static bool ConvertObjToBool(object obj)
        {
            bool.TryParse(obj.ToString(), out bool isbool);
            return isbool;
        }

        private static DateTime? ConvertObjToDate(object obj)
        {
            bool isdate = DateTime.TryParse(obj.ToString(), out DateTime dt);
            if (isdate)
                return dt;
            else
                return null;
        }

       
        private static void ShowError(Exception e,[CallerMemberName] string operationtype=null)
        {
            IMessageBoxService msg = new MessageBoxService();
            msg.ShowMessage("Error during " + operationtype + " operation\n" + e.Message.ToString(), operationtype + " Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            msg = null;
        }

        public static void HandleSQLError(SqlException e, [CallerMemberName] string operationtype = null)
        {
            IMessageBoxService msg = new MessageBoxService();
            string msgtext = string.Empty;
            switch (e.Number)
            {
                case 4060:
                    msgtext = "Unable to connect to database";
                    break;

                case 2812:
                    msgtext = "Database is missing a stored procedure";
                    break;

                case -1:
                    msgtext = "Cannot locate database - Please check the VPN connection";
                    break;

                default:
                    msgtext = "SQL error " + e.Number.ToString();
                    break;
            }

            msg.ShowMessage(msgtext + "\nError occurred in " + operationtype + " function\n" + e.Message.ToString() + "\nProgram now shutting down", operationtype + " Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            msg = null;
            //App.CloseProgram();

        }

    }
    #endregion

}





