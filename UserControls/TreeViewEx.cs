using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Data;
using System.Windows;

namespace AssetManager.UserControls
{
    class TreeViewEx
    { }

    public class AssetNodeEx : ViewModelBase
    {       
        public AssetNodeEx() 
        {
            _assetnodeexchildren = new ObservableCollection<AssetNodeEx>();        
        }
        
        private bool _HasChildren;
        private int _ID;
        private int? _ParentAssetID;
        private int _CustomerID;
        private string _Label;
        private string _Description;
        private string _Category;
        private bool _CanBeParent=true;
        private ObservableCollection<AssetNodeEx> _assetnodeexchildren;
        private bool _IsFiltered;
        private bool _IsExpanded;
        private bool _IsSelected;
        private Visibility _Visibility;

        public bool IsExpanded { get { return _IsExpanded; } set { _IsExpanded = value; OnPropertyChanged("IsExpanded"); } }
        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged("IsSelected"); } }

        public int ID { get { return _ID; } set { _ID = value; OnPropertyChanged("ID"); } }
        public int? ParentAssetID { get { return _ParentAssetID; } set { _ParentAssetID = value; OnPropertyChanged("ParentAssetID"); } }
        public int CustomerID { get { return _CustomerID; } set { _CustomerID = value; OnPropertyChanged("CustomerID"); } }
        public string Label { get { return _Label; } set { _Label = value; OnPropertyChanged("Label"); } }
        public string Description { get { return _Description; } set { _Description = value; OnPropertyChanged("Description"); } }
        public string Category { get { return _Category; } set { _Category = value; OnPropertyChanged("Category"); } }                
        public bool HasChildren { get { return _HasChildren; } set { _HasChildren = value; OnPropertyChanged("HasChildren"); } }
        public bool CanBeParent { get { return _CanBeParent; } set { _CanBeParent = value; OnPropertyChanged("CanBeParent"); } }
        public bool IsFiltered { get { return _IsFiltered; } set { _IsFiltered = value; OnPropertyChanged("IsFiltered"); } }
        public Visibility Visibility { get { return _Visibility; } set { _Visibility = value; OnPropertyChanged("Visibility"); } }
        public ObservableCollection<AssetNodeEx> AssetNodeExChildren { get { return _assetnodeexchildren; } set { _assetnodeexchildren = value; OnPropertyChanged("AssetNodeExChildren"); } }

    }


    public class TreeCustomerEx : ViewModelBase
    {        
        public TreeCustomerEx() 
        {            
            _assetnodeexchildren = new ObservableCollection<AssetNodeEx>();     
        }

        private bool _HasChildren;
        private int _ID;
        private string _Name;
        private string _Location;
        private string _IconFile;
        private bool _CanBeParent;
        private Visibility _Visibility;
        private ObservableCollection<AssetNodeEx> _assetnodeexchildren;
        private bool _IsExpanded;
        private bool _IsSelected;
        
        public bool IsExpanded { get { return _IsExpanded; } set { _IsExpanded = value; OnPropertyChanged("IsExpanded"); } }
        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; OnPropertyChanged("IsSelected"); } }
        public int ID { get { return _ID; } set { _ID = value; OnPropertyChanged("ID"); } }
        public string Name { get { return _Name; } set { _Name = value; OnPropertyChanged("Name"); } }
        public string Location { get { return _Location; } set { _Location = value; OnPropertyChanged("Location"); } }
        public string IconFile { get { return _IconFile; } set { _IconFile = value; OnPropertyChanged("IconFile"); } }
        public bool HasChildren { get { return _HasChildren; } set { _HasChildren = value; OnPropertyChanged("HasChildren"); } }
        public bool CanBeParent { get { return true; } set { _CanBeParent = value; OnPropertyChanged("CanBeParent"); } }
        public Visibility Visibility { get { return _Visibility; } set { _Visibility = value; OnPropertyChanged("Visibility"); } }
        public ObservableCollection<AssetNodeEx> AssetNodeExChildrenP { get { return _assetnodeexchildren; } set { _assetnodeexchildren = value; OnPropertyChanged("AssetNodeExChildrenP"); } }
   
    }


    public class TreeCustomersEx : ObservableCollection<TreeCustomerEx>
    {
        private ObservableCollection<TreeCustomerEx> _treecustomer;
        public TreeCustomersEx()
        {
            _treecustomer = new ObservableCollection<TreeCustomerEx>();
        }
        public ObservableCollection<TreeCustomerEx> Tree { get { return _treecustomer; } set { _treecustomer = value; } }
    }



    public class FilteredAssets : List<int>
    {
        public FilteredAssets()
        {}

        public FilteredAssets(int CustomerID, string TypeSearchStr, string GroupSearchStr)
        {
            string _where = " WHERE ";
            string _customerStr = "(CustomerID= ";
            string _TypeStr = "AssetTypes.Description IN ";
            string _GroupStr = "AssetGroups.[Group] IN ";
            string _searchStr = "";
            string _SELECTstr = "SELECT Assets.AssetID FROM ((Assets LEFT OUTER JOIN  AssetTypes ON Assets.TypeID = AssetTypes.ID) LEFT OUTER JOIN AssetGroups ON Assets.GroupID = AssetGroups.ID)";

            if (CustomerID != 0)            
                _searchStr = _customerStr + CustomerID.ToString() + ")";
            
            if ((!string.IsNullOrEmpty(TypeSearchStr)) && (!string.IsNullOrEmpty(GroupSearchStr)))
            {
                if(_searchStr != "")                
                    _searchStr = _searchStr + " AND ((" + _TypeStr + "(" + TypeSearchStr + ")) OR (" + _GroupStr + "(" + GroupSearchStr + ")))";                                                            
                else
                    _searchStr = "((" + _TypeStr + "(" + TypeSearchStr + ")) OR (" + _GroupStr + "(" + GroupSearchStr + ")))"; 
            }
            else
            {
                if (!string.IsNullOrEmpty(TypeSearchStr)) 
                {                                 
                    if(_searchStr != "")                    
                        _searchStr = _searchStr + " AND (" + _TypeStr + "(" + TypeSearchStr + "))";                    
                    else
                        _searchStr = "(" + _TypeStr + "(" + TypeSearchStr + "))";
                }
                else
                {
                    if (!string.IsNullOrEmpty(GroupSearchStr))
                    {
                        if(_searchStr != "")                    
                            _searchStr = _searchStr + " AND (" + _GroupStr + "(" + GroupSearchStr + "))";                    
                        else
                            _searchStr = "(" + _GroupStr + "(" + GroupSearchStr + "))";
                    }
                }
            }

            if (!((string.IsNullOrEmpty(TypeSearchStr)) && (string.IsNullOrEmpty(GroupSearchStr))))
            {
                if (_searchStr != "")
                    _searchStr = _where + _searchStr;

                using (OleDbConnection CONN = new OleDbConnection(ConfigFileClass.DatabaseConnectionString))
                {
                    CONN.Open();
                    OleDbCommand oc = new OleDbCommand(_SELECTstr + _searchStr, CONN);
                    OleDbDataReader or = oc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (or.Read())
                    {
                        this.Add(Convert.ToInt32(or["AssetID"]));
                    }
                    or.Close();
                  
                }
            }
        }

    }


}
