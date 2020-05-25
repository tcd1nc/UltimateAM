using System;
using System.Windows.Input;
using System.Windows;
using System.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using static AssetManager.SQLiteQueries;
using static AssetManager.GlobalClass;
using AssetManager.Models;
using AssetManager.UserControls;

namespace AssetManager.ViewModels
{
    public class AssetTreeExViewModel : ViewModelBase
    {
        #region Declarations
        //Command declarations ============================================================
        ICommand clearfilter;      //Clear filter
        ICommand closeall;         //Close all child windows and application

        Window owner = Application.Current.Windows[0];

        public FullyObservableCollection<TVCustomerViewModel> allcustomers = new FullyObservableCollection<TVCustomerViewModel>();

        #endregion Declarations

        #region Constructors

        public AssetTreeExViewModel()
        {
            IsEnabled = IsAdministrator;
            BuildSearchFields();

            GetAllItems();
            
            Customers = allcustomers;
            Delimiter = Defaults.Delimiter;
            LabelMask = string.Empty;
        }

        #endregion Constructors

        bool isenabled;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        public FullyObservableCollection<TVCustomerViewModel> Customers
        {
            get { return allcustomers; }
            set { SetField(ref allcustomers, value); }
        }

        Collection<TreeViewItemViewModel> checkeditems;
        public Collection<TreeViewItemViewModel> CheckedItems
        {
            get { return checkeditems; }
            set { SetField(ref checkeditems, value); }
        }


        #region Search Properties

        SearchFieldModel searchselectedstem;
        public SearchFieldModel SearchSelectedItem
        {
            get { return searchselectedstem; }
            set
            {
                if (value != null)
                {                   
                    LabelMask = value.Mask;                   
                }
                SetField(ref searchselectedstem, value);
            }
        }

        public FullyObservableCollection<SearchFieldModel> SearchFields { get; set; } = new FullyObservableCollection<SearchFieldModel>();

        FullyObservableCollection<SearchComboModel> searchcombo = new FullyObservableCollection<SearchComboModel>();
        public FullyObservableCollection<SearchComboModel> SearchCombo
        {
            get { return searchcombo; }
            set { SetField(ref searchcombo, value); }
        }

        string searchtext;
        public string SearchText
        {
            get { return searchtext; }
            set { SetField(ref searchtext, value); }
        }

      

        int searchhits;
        public int SearchHits
        {
            get { return searchhits; }
            set { SetField(ref searchhits, value); }
        }

        string labelmask;
        public string LabelMask
        {
            get { return labelmask; }
            set { SetField(ref labelmask, value); }
        }

        string delimiter;
        public string Delimiter
        {
            get { return delimiter; }
            set { SetField(ref delimiter, value); }
        }


        bool clearsearch = false;
        public bool ClearSearch
        {
            get { return clearsearch; }
            set { SetField(ref clearsearch, value); }
        }

        private void BuildSearchFields()
        {
            SearchFields = GetSearchFields();                       
        }

        #endregion

        #region Commands
        
        ICommand customerdlg;
        public ICommand OpenCustomerDialog
        {
            get
            {
                if (customerdlg == null)
                    customerdlg = new DelegateCommand(CanExecute, ExecOpenCustomersDialog);
                return customerdlg;
            }
        }

        private void ExecOpenCustomersDialog(object parameter)
        {

            IMessageBoxService msg = new MessageBoxService();
            if (msg.OpenCustomersDlg() == true)
                GetAllItems();

        }

        ICommand searchdata;
        public ICommand SearchData
        {
            get
            {
                if (searchdata == null)
                    searchdata = new DelegateCommand(CanExecute, SearchDatabase);
                return searchdata;
            }
        }

        Dictionary<int, int> assets = new Dictionary<int, int>();

        /// <summary>
        /// Search database according to SearchFieldType
        /// </summary>
        /// <param name="parameter"></param>
        private void SearchDatabase(object parameter)
        {
            if (SearchSelectedItem != null)
            {                
                assets = GetAssetSearch(SearchSelectedItem.TableName, SearchSelectedItem.FieldName, SearchText);                          
                SearchHits = assets.Count;

                if (SearchHits > 0)
                {
                    foreach (TVCustomerViewModel cm in AllItems)
                    {
                        foreach (TVAssetViewModel am in cm.Children)
                        {
                            if (assets.ContainsKey(am.Asset.ID))
                            {
                                am.IsFiltered = true;
                                am.IsExpanded = true;
                                cm.IsExpanded = true;
                                //to scroll into view
                                cm.IsSelected = true;
                                assets.Remove(am.Asset.ID);
                            }
                            RecursivelyGetFilteredAsset2(am.Children, cm);
                            //   GetFilteredAsset(am.Children, cm);
                        }
                    }
                }
            }
        }

        private void RecursivelyGetFilteredAsset2(ObservableCollection<TreeViewItemViewModel> theseNodes, TVCustomerViewModel cm)
        {
            foreach (TVAssetViewModel am in theseNodes)
            {
                if (assets.ContainsKey(am.Asset.ID))
                {
                    if (am.Parent != null)
                    {
                        am.Parent.IsExpanded = true;
                    }
                    am.IsFiltered = true;
                    am.IsExpanded = true;
                    cm.IsExpanded = true;
                    //optimise search by removing element when found
                    assets.Remove(am.Asset.ID);
                }
                RecursivelyGetFilteredAsset2(am.Children, cm);
            }
        }

        /// <summary>
        /// Non-recursive version
        /// </summary>
        /// <param name="theseNodes"></param>
        /// <param name="cm"></param>
        public void GetFilteredAsset(ObservableCollection<TreeViewItemViewModel> theseNodes, TVCustomerViewModel cm)
        {
            var stack = new Stack<TVAssetViewModel>();
            foreach (TVAssetViewModel tv in theseNodes)
                stack.Push(tv);
            while (stack.Count != 0)
            {
                TVAssetViewModel current = stack.Pop();
                //  if (assets.Contains(current.Asset.AssetID))
                if (assets.ContainsKey(current.Asset.ID))
                {
                    if (current.Parent != null)
                    {
                        current.Parent.IsExpanded = true;
                    }
                    current.IsFiltered = true;
                    current.IsExpanded = true;
                    cm.IsExpanded = true;
                    //optimise search by removing element when found
                    //          var id = assets.Where(x => x == current.Asset.AssetID).FirstOrDefault();
                    //        assets.Remove(id);

                    assets.Remove(current.Asset.ID);
                }
                foreach (TVAssetViewModel child in (current.Children).Reverse())
                    stack.Push(child);
            }
        }


        /// <summary>
        /// Consider non-recursive approach...
        /// </summary>
        private void ClearSearchFilter()
        {
            ClearSearch = true;
            ClearSearch = false;
            SearchHits = 0;

            GetAllItems();
        }

        /// <summary>
        /// Consider non-recursive approach...
        /// </summary>
        /// <param name="theseNodes"></param>
        private void RecursivelyResetFilteredAsset(ObservableCollection<TreeViewItemViewModel> theseNodes)
        {
            foreach (TVAssetViewModel am in theseNodes)
            {
                am.IsFiltered = false;
                am.IsExpanded = false;
                if (am.Parent != null)
                    am.Parent.IsExpanded = false;

                RecursivelyResetFilteredAsset(am.Children);
            }
        }

        /// <summary>
        /// Non-recursive option
        /// </summary>
        /// <param name="theseNodes"></param>
        public void ResetFilteredAssets(ObservableCollection<TreeViewItemViewModel> theseNodes)
        {
            var stack = new Stack<TVAssetViewModel>();
            foreach (TVAssetViewModel tv in theseNodes)
                stack.Push(tv);
            while (stack.Count != 0)
            {
                TVAssetViewModel current = stack.Pop();
                current.IsFiltered = false;
                current.IsExpanded = false;
                if (current.Parent != null)
                    current.Parent.IsExpanded = false;
                foreach (TVAssetViewModel child in (current.Children).Reverse())
                    stack.Push(child);
            }
        }

      
        ICommand newdialog;
        public ICommand OpenDialog
        {
            get
            {
                if (newdialog == null)
                    newdialog = new DelegateCommand(CanExecute, OpenSelectedDialog);
                return newdialog;
            }
        }

        private void OpenSelectedDialog(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            msg.OpenDialog((string)parameter);
            msg = null;
        }

        ICommand doubleclick;
        public ICommand DoubleClickCommand
        {
            get
            {
                if (doubleclick == null)
                    doubleclick = new DelegateCommand(CanExecute, ExecuteDoubleClick);

                return doubleclick;
            }
        }

        private void ExecuteDoubleClick(object obj)
        {
            if (obj.GetType().Equals(typeof(TVCustomerViewModel)))
            {               
                IMessageBoxService msgbox = new MessageBoxService();
                CustomerViewModel result = (CustomerViewModel) msgbox.OpenCustomerDlg((obj as TVCustomerViewModel).Customer.ID);
                if (result.Customer.Name !=null && result.Customer.ID !=-1)
                    (obj as TVCustomerViewModel).Customer.Name = result.Customer.Name;
             
            }
            else
            {
                if (obj.GetType().Equals(typeof(TVAssetViewModel)))
                {
                    if (!(obj as TVAssetViewModel).IsSelected)
                        return;

                    int ID = (obj as TVAssetViewModel).Asset.ID;
                    if (!OpenAssetWindow(ID))
                    {
                        IMessageBoxService msgbox = new MessageBoxService();
                        _ = (bool)msgbox.OpenAssetDlg(this, (TVAssetViewModel)obj);
                    }
                }
                else
                    return;
            }                       
        }

        public void ProcessAssetChanges(TVAssetViewModel original, AssetModel asset )
        {
            if (asset.Category != null)
                original.Asset.Category = asset.Category;

            original.Asset.Label = MakeLabel(asset.AssetAreaID, asset.AssetGroupID, asset.LabelID);
        }


        //Command to call delegate to clear filter
        public ICommand ClearFilter
        {
            get
            {
                if (clearfilter == null)
                    clearfilter = new DelegateCommand(CanExecute, ExecuteClearFilter);
                return clearfilter;
            }
        }
        private void ExecuteClearFilter(object parameter)
        {
            ClearSearchFilter();
        }

        public ICommand CloseAll
        {
            get
            {
                if (closeall == null)
                    closeall = new DelegateCommand(CanExecute, ExecuteCloseAll);
                return closeall;
            }
        }

        /// <summary>
        /// Close all child windows before closing program
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteCloseAll(object parameter)
        {
            foreach (Window w in owner.OwnedWindows)
            {
                if (w.GetType().Equals(typeof(Views.AssetView)))
                {
                    if (((Views.AssetView)w).Tag != null)
                    {
                        if ((int)((Views.AssetView)w).Tag > 0)
                        {
                            if (IsEnabled)
                                ((AssetViewModel)((Views.AssetView)w).DataContext).SaveAll();
                            // w.Close();
                        }
                        //else
                        //  w.Close();
                    }
                }
                w.Close();
            }
           
            Application.Current.Shutdown();
        }

        ICommand addnewasset;
        public ICommand AddNewAsset
        {
            get
            {
                if (addnewasset == null)
                    addnewasset = new DelegateCommand(CanExecute, ExecuteAddNewAsset);
                return addnewasset;
            }
        }

        private void ExecuteAddNewAsset(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            AssetModel result = msg.OpenAssetDlg(0, 0);
            if (result != null)            
                AddAssetNode(result);
            
            msg = null;
        }

            #endregion Commands

        #region Context Menu Commands

            ICommand addnewassetcm;
        public ICommand AddNewAssetCM
        {
            get
            {
                if (addnewassetcm == null)
                    addnewassetcm = new DelegateCommand(CanExecute, ExecuteAddNewAssetCM);
                return addnewassetcm;
            }
        }

        private void ExecuteAddNewAssetCM(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            if (parameter.GetType().Equals(typeof(TVAssetViewModel)))
            {
                TVAssetViewModel asset = parameter as TVAssetViewModel;
                AssetModel result = msg.OpenAssetDlg(asset.Asset.CustomerID, asset.Asset.ID);
                if (result != null)                
                    AddAssetNode(result);                
            }
            else
            if (parameter.GetType().Equals(typeof(TVCustomerViewModel)))
            {
                TVCustomerViewModel customer = parameter as TVCustomerViewModel;
                AssetModel result = msg.OpenAssetDlg(customer.Customer.ID, 0);
                if (result != null)
                    AddAssetNode(result);
            }
        }

        ICommand deletecustomer;
        public ICommand DeleteCustomer
        {
            get
            {
                if (deletecustomer == null)
                    deletecustomer = new DelegateCommand(CanExecute, ExecuteDeleteCustomer);
                return deletecustomer;
            }
        }

        private void ExecuteDeleteCustomer(object parameter)
        {
            if (parameter.GetType().Equals(typeof(TVCustomerViewModel)))
            {
                TVCustomerViewModel customer = parameter as TVCustomerViewModel;
                IMessageBoxService msg = new MessageBoxService();
                if (customer.Children.Count == 0)
                {
                    if (msg.ShowMessage("Do you want to delete " + customer.Customer.Name, "Delete Customer: " + customer.Customer.Name, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question) == GenericMessageBoxResult.OK)
                    {
                        Customers.Remove(customer);
                        DeleteItem(customer.Customer.ID, "Customers");                        
                    }
                }
                else
                    msg.ShowMessage(customer.Customer.Name + " has Assets and cannot be deleted" + "\n\nDelete the Assets first and then delete the customer.",
                        "Unable to Delete Customer: " + customer.Customer.Name, GenericMessageBoxButton.OK, GenericMessageBoxIcon.Information);
            }
        }

        private bool CanExecuteDeleteAsset(object obj)
        {
            return true;
        }

        ICommand deleteasset;
        public ICommand DeleteAsset
        {
            get
            {
                if (deleteasset == null)
                    deleteasset = new DelegateCommand(CanExecuteDeleteAsset, ExecuteDeleteAsset);
                return deleteasset;
            }
        }

        private void ExecuteDeleteAsset(object parameter)
        {
            if (parameter.GetType().Equals(typeof(TVAssetViewModel)))
            {
                TVAssetViewModel asset = parameter as TVAssetViewModel;
                IMessageBoxService msg = new MessageBoxService();
                if (msg.ShowMessage("Are you sure you want to delete this Asset: " + "asset.Asset.Label" + " and any associated child Assets?", "Removing Asset: " + "asset.Asset.Label", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.Yes))                
                    //change to delete all children too and set parentid to 0 .
                    DeleteAssetDFS(asset);
            }
        }

        private bool CanExecuteMoveAsset(object obj)
        {
            return true;
        }

        ICommand moveasset;
        public ICommand MoveSelectedAsset
        {
            get
            {
                if (moveasset == null)
                    moveasset = new DelegateCommand(CanExecuteMoveAsset, ExecuteMoveAsset);
                return moveasset;
            }
        }

        TVAssetViewModel movingasset;
        string movingassetlabel;
        public string MovingAssetLabel
        {
            get { return movingassetlabel; }
            set { SetField(ref movingassetlabel, value); }
        }

        private void ExecuteMoveAsset(object parameter)
        {
            if (parameter.GetType().Equals(typeof(TVAssetViewModel)))
            {
                if (parameter is TVAssetViewModel asset)
                {
                    movingasset = asset;
                    asset.IsSelected = true;
                    movingassetlabel = "asset.Asset.Label";
                }
            }
        }

        private bool CanExecutePasteAsset(object obj)
        {
            if (movingasset == null)
                return false;
            return true;
        }

        ICommand pasteasset;
        public ICommand PasteSelectedAsset
        {
            get
            {
                if (pasteasset == null)
                    pasteasset = new DelegateCommand(CanExecutePasteAsset, ExecutePasteAsset);
                return pasteasset;
            }
        }

        private void ExecutePasteAsset(object parameter)
        {
            if (parameter.GetType().Equals(typeof(TVAssetViewModel)))
            {
                if (parameter is TVAssetViewModel targetasset
                    && !(targetasset.Asset.CustomerID == movingasset.Asset.CustomerID && targetasset.Asset.ID == movingasset.Asset.ParentAssetID))
                {
                    UpdateParentAssetID(movingasset.Asset.ID, targetasset.Asset.ID, targetasset.Asset.CustomerID);
                    MoveAsset(movingasset.Asset.ID, targetasset.Asset.ID, targetasset.Asset.CustomerID);
                    movingasset = null;
                    MovingAssetLabel = string.Empty;
                }
            }
            else
            if (parameter.GetType().Equals(typeof(TVCustomerViewModel)))
            {
                if (parameter is TVCustomerViewModel targetcustomer
                    && !(targetcustomer.Customer.ID == movingasset.Asset.CustomerID && movingasset.Asset.ParentAssetID == 0))
                {
                    UpdateParentAssetID(movingasset.Asset.ID, 0, targetcustomer.Customer.ID);
                    MoveAsset(movingasset.Asset.ID, 0, targetcustomer.Customer.ID);
                    movingasset = null;
                    MovingAssetLabel = string.Empty;
                }
            }
        }

        #endregion
        
        FullyObservableCollection<TreeViewItemViewModel> allitems = new FullyObservableCollection<TreeViewItemViewModel>();
        public FullyObservableCollection<TreeViewItemViewModel> AllItems
        {
            get { return allitems; }
            set { SetField(ref allitems, value); }
        }

        private void GetAllItems()
        {
            AllItems?.Clear();
            FullyObservableCollection<TVCustomerModel> customers = GetTVCustomers();
            foreach (TVCustomerModel cm in customers)
            {
                //add customer to tree
                TreeViewItemViewModel customer = new TVCustomerViewModel(cm);

                FullyObservableCollection<AssetModel> assets = GetCustomerChildAssets(cm.ID);
                foreach (AssetModel am in assets)
                {
                    TVAssetViewModel asset = new TVAssetViewModel(am, null);
                    GetAllSubItems(asset);
                    customer.Children.Add(asset);
                }
                AllItems.Add(customer);
            }
        }

        private void GetAllSubItems(TVAssetViewModel tvi)
        {
            FullyObservableCollection<AssetModel> assets = GetChildAssets(tvi.Asset.ID);
            foreach (AssetModel tvasset in assets)
            {
                TVAssetViewModel asset = new TVAssetViewModel(tvasset, tvi);
                tvi.Children.Add(asset);
                GetAllSubItems(asset);
            }
        }
        
        public void RemoveCustomerVM(int customerid)
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> change this
            foreach (TVCustomerViewModel cust in allcustomers)
            {
                if (cust.Customer.ID == customerid)
                {
                    allcustomers.Remove(cust);
                    break;
                }
            }
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        }
               
        
        //===============================================================================================================================================

        #region Static classes to manipulate the treeview nodes
        //Remove customer
        //Add new Asset node
        //Add new Customer node
        //Move Asset nodes
        //Update child nodes when moving from customer to new customer
        //Get Asset node based on AssetID
        //Get Customer node based on CustomerID
        //Update open windows when changing parent CustomerID      
        //Delete Asset
        //Log Asset movements
        //===============================================================================================================================================

        public static void RemoveCustomer(int custid)
        {
            Window owner = Application.Current.Windows[0];
            (owner.DataContext as AssetTreeExViewModel).RemoveCustomerVM(custid);
        }
        
        public static void AddAssetNode(AssetModel asset)
        {
            TVCustomerViewModel customer;
            TVAssetViewModel parentnode = null;

            //determine if asset is in tree
            //if found then update via bindings
            //else add to tree

            if (asset.ParentAssetID == 0) //customer is parent
            {
                //get customer node
                customer = GetCustomerNode(asset.CustomerID);
                //look for asset under this customer              
                if (GetCustomerAsset(customer, asset.ID) == null) //not found
                {
                    //new Nov 2016 - added tvasset.Parent = customer;
                    TVAssetViewModel tvasset = new TVAssetViewModel(asset, null)
                    {
                        Parent = customer
                    };
                    customer.Children.Add(tvasset);

                    //add to customer node
                    // customer.Children.Add(new TVAssetViewModel(asset, null));
                }
            }
            else //asset is parent
            {
                //get customer node
                customer = GetCustomerNode(asset.CustomerID);
                //get parent node of asset                            
                parentnode = GetAssetDFS(customer, asset.ParentAssetID);

                //new Nov 2016 - added tvasset.Parent = parentnode;
                TVAssetViewModel tvasset = new TVAssetViewModel(asset, null)
                {
                    Parent = parentnode
                };
                parentnode.Children.Add(tvasset);

                //        parentnode.Children.Add(new TVAssetViewModel(asset, null));


            }
            LogMovement(ActivityType.NewAsset, asset.ID, 0, asset.CustomerID);
        }

        public static TVAssetViewModel GetCustomerAsset(TVCustomerViewModel cv, int assetid)
        {
            foreach (TVAssetViewModel tvasset in cv.Children)
            {
                if (tvasset.Asset.ID == assetid)                
                    return tvasset;                
            }
            return null;
        }

        /// <summary>
        /// Add new Customer Node to treeview
        /// </summary>
        /// <param name="customer">CustomerModel</param>
        public static void AddCustomerNode(TVCustomerModel customer)
        {
            Window owner = Application.Current.Windows[0];
            FullyObservableCollection<TVCustomerViewModel> customers = (owner.DataContext as AssetTreeExViewModel).allcustomers;
            customers.Add(new TVCustomerViewModel(customer));
        }
        
        /// <summary>
        /// Move selected asset to new parent with new customer
        /// Update open Asset windows
        /// </summary>
        /// <param name="assetid"></param>
        /// <param name="newparentassetid"></param>
        /// <param name="newcustomerid"></param>
        public static void MoveAsset(int assetid, int newparentassetid, int newcustomerid)
        {
            Window owner = Application.Current.Windows[0];
            FullyObservableCollection<TVCustomerViewModel> customers = (owner.DataContext as AssetTreeExViewModel).allcustomers;

            TreeViewItemViewModel asset = null;
            //get the node representing the asset
            foreach (TVCustomerViewModel cm in customers)
            {
                asset = GetAssetDFS(cm, assetid);
                if (asset != null)
                    break;
            }

            TVCustomerViewModel customer;
            if (asset != null)
            {
                TreeViewItemViewModel parentasset = null;
                //new parent id = 0 occurs when the asset is added directly to customer
                if (newparentassetid != 0)
                    foreach (TVCustomerViewModel cm in customers)
                    {
                        parentasset = GetAssetDFS(cm, newparentassetid);
                        if (parentasset != null)
                            break;
                    }
                else
                    parentasset = GetCustomerNode(newcustomerid);

                //in error condition, check that parent asset is not null - rare but still should be checked
                if (parentasset != null)
                {
                    //check to see if asset is directly under a customer or if its under another asset          
                    if ((asset as TVAssetViewModel).Asset.ParentAssetID == 0)
                    {
                        //get customer node from customerid
                        //remove asset node from customer node if customerid !=0
                        if ((asset as TVAssetViewModel).Asset.CustomerID != 0)
                        {
                            customer = GetCustomerNode((asset as TVAssetViewModel).Asset.CustomerID);
                            customer.Children.Remove(asset);
                        }
                    }
                    else
                        asset.Parent.Children.Remove(asset);

                    //add movement to log
                    LogMovement(ActivityType.Transfer, assetid, (asset as TVAssetViewModel).Asset.CustomerID, newcustomerid);

                    //update child assets to correct customerid and status
                    UpdateChildAsset(assetid, newcustomerid, 1);
                    UpdateCustomerID(asset.Children, newcustomerid);
                    //update open windows to reflect the changes made to data
                    UpdateRelatedWindow(assetid, newcustomerid, 1, owner);
                    (asset as TVAssetViewModel).Asset.ParentAssetID = newparentassetid;
                    (asset as TVAssetViewModel).Asset.CustomerID = newcustomerid;
                    (asset as TVAssetViewModel).Asset.StatusID = 1;
                    (asset as TVAssetViewModel).IsExpanded = true;
                    parentasset.Children.Add(asset);
                    parentasset.IsExpanded = true;
                    asset.Parent = parentasset;
                }
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theseNodes"></param>
        /// <param name="custid"></param>
        /// <param name="amm"></param>
        public static void UpdateCustomerID(ObservableCollection<TreeViewItemViewModel> theseNodes, int custid)
        {
            foreach (TVAssetViewModel am in theseNodes)
            {
                LogMovement(ActivityType.Transfer, am.Asset.ID, am.Asset.CustomerID, custid);

                am.Asset.CustomerID = custid;
                //Call datalayer update
                UpdateChildAsset(am.Asset.ID, custid, 1);

                //if asset window is open then update window values
                UpdateRelatedWindow(am.Asset.ID, custid, 1, Application.Current.Windows[0]);

                if (am.Children != null)
                    UpdateCustomerID(am.Children, custid);
            }

        }

        /// <summary>
        /// Depth first search of asset tree for the given node
        /// If found the node is returned as a static variable
        /// else null
        /// </summary>
        /// This avoids recursion and finds the first occurrence of the target node 
        /// in the case where there could be multiple instances or circular references.
        /// Need to be careful about size of stack. Data tree shape might prefer BFS instead.
        /// Loop should run faster than recursion
        /// <returns></returns>
        public static TVAssetViewModel GetAssetDFS(TVCustomerViewModel cv, int targetid)
        {
            var stack = new Stack<TVAssetViewModel>();
            foreach (TVAssetViewModel tv in cv.Children)
                stack.Push(tv);
            while (stack.Count != 0)
            {
                TVAssetViewModel current = stack.Pop();
                //assign current node to found asset
                if (current.Asset.ID == targetid)                
                    return current;
                
                foreach (TVAssetViewModel child in (current.Children).Reverse())
                    stack.Push(child);
            }
            return null;
        }

        /// <summary>
        /// Get Customer matching given ID
        /// </summary>
        /// <param name="cv">Customer node collection</param>
        /// <param name="targetid">Customer ID</param>
        /// <returns></returns>
        public static TVCustomerViewModel GetCustomerNode(int targetid)
        {
            Window owner = Application.Current.Windows[0];
            FullyObservableCollection<TVCustomerViewModel> customers = (owner.DataContext as AssetTreeExViewModel).allcustomers;

            foreach (TVCustomerViewModel cm in customers)
            {
                if (cm.Customer.ID == targetid)
                    return cm;
            }
            return null;
        }

        public static TVAssetViewModel GetAssetNode(int assetid, int customerid)
        {
            TVCustomerViewModel customer = GetCustomerNode(customerid);
            return GetAssetDFS(customer, assetid);
        }

        /// <summary>
        /// Update open windows with new CustomerID and Status
        /// </summary>
        /// <param name="id">Current Asset ID</param>
        /// <param name="customerid">New Customer ID</param>
        /// <param name="statusid">New Status ID</param>
        /// <param name="owner">Owner of window</param>
        public static void UpdateRelatedWindow(int id, int customerid, int statusid, Window owner)
        {
            foreach (Window w in owner.OwnedWindows)
            {
                if (w.GetType().Equals(typeof(Views.AssetView)))
                {
                    AssetViewModel assetvm = ((AssetViewModel)((Views.AssetView)w).DataContext);
                    if (assetvm.Asset.ID == id)
                    {
                        assetvm.Asset.CustomerID = customerid;
                        assetvm.Asset.StatusID = statusid;
                        break;
                    }
                }
            }
        }

        public static void CloseRelatedWindow(int id, Window owner)
        {
            foreach (Window w in owner.OwnedWindows)
            {
                if (w.GetType().Equals(typeof(Views.AssetView)))
                {
                    AssetViewModel assetvm = ((AssetViewModel)((Views.AssetView)w).DataContext);
                    if (assetvm.Asset.ID == id)
                    {
                        assetvm.CloseWindow();
                        break;
                    }
                }
            }
        }

        public static bool OpenAssetWindow(int assetid)
        {
            bool isopen = false;
            Window owner = Application.Current.Windows[0];
            foreach (Window w in owner.OwnedWindows)
            {
                if (w.GetType().Equals(typeof(Views.AssetView)))
                {
                    if (((AssetViewModel)((Views.AssetView)w).DataContext).Asset.ID == assetid)
                    {
                        w.Activate();
                        isopen = true;
                        break;
                    }
                }
            }
            return isopen;
        }

        /// <summary>
        /// DFS search to undelete assets undet the given asset. 
        /// Undeleted assets are located at root of selected customer
        /// </summary>
        /// <param name="assetid">Current Asset ID</param>
        /// <param name="newcustomerid">New (default) Customer ID for undeleted assets</param>
        public static void UnDeleteChildAssets(int assetid, int newcustomerid)
        {
            FullyObservableCollection<AssetModel> assets = GetDeletedChildAssets(assetid);
            var stack = new Stack<AssetModel>();
            foreach (AssetModel tv in assets)
                stack.Push(tv);
            while (stack.Count != 0)
            {
                AssetModel current = stack.Pop();
                //assign current node to found asset
                //SQLiteQueries.UnDeleteAsset(current.ID, newcustomerid, (int)StatusType.Available);

                LogMovement(ActivityType.Undeleted, assetid, 0, newcustomerid);

                foreach (AssetModel child in GetChildAssets(current.ID))
                    stack.Push(child);

            }           
        }

        /// <summary>
        /// Build new subtree with deleted asset
        /// </summary>
        /// <param name="assetid">Current Asset ID</param>
        /// <param name="newcustomerid">New Customer ID</param>
        // public static void UnDeleteAsset(Models.AssetSummaryModel asset, int newcustomerid)
        public static void UnDeleteAsset(AssetModel asset, int newcustomerid)
        {
            TVCustomerViewModel customer;
            customer = GetCustomerNode(newcustomerid);
            //SQLiteQueries.UnDeleteAsset(asset.ID, newcustomerid, (int)StatusType.InUse);
            LogMovement(ActivityType.Undeleted, asset.ID, 0, newcustomerid);

            //update child assets to correct customer and status
            UnDeleteChildAssets(asset.ID, newcustomerid);

            //add first node
            AssetModel am = GetAsset(asset.ID);
            am.CustomerID = newcustomerid;
            customer.Children.Add(new TVAssetViewModel(am, null));

        }

        /// <summary>
        /// Delete asset matching assetid. Child assets are also deleted
        /// </summary>
        /// <param name="assetid">Current Asset ID</param>
        /// <param name="assetparentid">Asset parent ID</param>
        /// <param name="customerid">Asset customer ID</param>
        public static void DeleteAssetDFS(TVAssetViewModel tvasset)
        {
            Window owner = Application.Current.Windows[0];
            TVCustomerViewModel cm = GetCustomerNode(tvasset.Asset.CustomerID);
            TVAssetViewModel asset = null;
            asset = GetAssetDFS(cm, tvasset.Asset.ID);
            TVAssetViewModel assetparent = null;
            assetparent = GetAssetDFS(cm, tvasset.Asset.ParentAssetID);

            if (asset != null)
            {
                if (assetparent != null)
                    assetparent.Children.Remove(asset);
                else
                    cm.Children.Remove(asset);

                SetParentAssetID(tvasset.Asset.ID, 0);

                var stack = new Stack<TVAssetViewModel>();
                stack.Push(asset);
                while (stack.Count != 0)
                {
                    TVAssetViewModel current = stack.Pop();
                    DeleteAsset((current).Asset);
                    CloseRelatedWindow(current.Asset.ID, owner);
                    LogMovement(ActivityType.Deleted, current.Asset.ID, tvasset.Asset.CustomerID, 0);

                    foreach (TVAssetViewModel child in (current.Children).Reverse())
                        stack.Push(child);
                }
            }           
        }

        public static Collection<int> ChildNodeIDs = new Collection<int>();

        /// <summary>
        /// Create list of child nodes that cannot be dropped onto.
        /// </summary>
        /// <param name="tvasset"></param>
        /// <returns></returns>
        public static Collection<int> GetChildIDs(TVAssetViewModel tvasset)
        {
            ChildNodeIDs.Clear();
            var stack = new Stack<TVAssetViewModel>();
            stack.Push(tvasset);
            while (stack.Count != 0)
            {
                TVAssetViewModel current = stack.Pop();
                //add current asset id to collecton
                ChildNodeIDs.Add(current.Asset.ID);

                foreach (TVAssetViewModel child in (current.Children).Reverse())
                    stack.Push(child);
            }
            return null;
        }

        /// <summary>
        /// Log asset creation, deletion and movements within customers
        /// </summary>
        /// <param name="activitycodeid"></param>
        /// <param name="assetid">Current Asset</param>
        /// <param name="srccustomerid">Source customer ID</param>
        /// <param name="destcustomerid">Destination customer ID</param>
        public static void LogMovement(ActivityType activitycodeid, int assetid, int srccustomerid, int destcustomerid)
        {
            AssetMovementModel amm = new AssetMovementModel
            {
                AssetID = assetid,
                ActivityCodeID = (int)activitycodeid,
                DateMoved = DateTime.Now,
                SourceCustomerID = srccustomerid,
                DestinationCustomerID = destcustomerid
            };
            AddAssetMovement(amm);
        }

        #endregion
    }

}
