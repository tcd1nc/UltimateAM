using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using System.Collections.ObjectModel;
using System.Xml;
using AssetManager.UserControls;
using AssetManager.Models;

namespace AssetManager
{
    /// <summary>
    /// Interaction logic for AssetTree.xaml
    /// </summary>
    public partial class AssetTree : Window
    {

        ObservableCollection<TreeCustomer> tcs;
        TreeCustomer tc;

        ObservableCollection<TreeCustomerEx> tcsex;
        TreeCustomerEx tcex;
        
        
        
        OleDbConnection _conn;

        public AssetTree()
        {                       
            InitializeComponent();

//System.Diagnostics.Stopwatch s = new System.Diagnostics.Stopwatch();
 //      s.Start();
            
//s.Stop();          
 //System.Diagnostics.Debug.WriteLine(s.Elapsed.TotalMilliseconds.ToString());            

             //_as = this.FindResource("AssetsCollectionViewSource") as CollectionViewSource;
            // _as.Source = new Assets();
            // treeview1.ItemsSource = _as.View;


            tcs =  GetTreeData();
            treeview1.ItemsSource = tcs;// GetTreeData();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
 //          <userSettings>
  //  <AssetManager.Properties.Settings>
  //    <setting name="AssetImageFolderPath" serializeAs="String" value="thing"/>
  //    <setting name="CompanyLogoFolderPath" serializeAs="String" value="thing"/>
 //     <setting name="CompanyLogoFileName" serializeAs="String" value="thing"/>
 //     <setting name="CustomerIconFolderPath" serializeAs="String" value=""  />
 //     <setting name="SalesDivisionIconFolderPath" serializeAs="String" value=""  />
 //     <setting name="AssetGroupIconFolderPath" serializeAs="String" value=""  />
  //    <setting name="AssetTypeIconFolderPath" serializeAs="String" value=""  />
  //    <setting name="AssetStatusIconFolderPath" serializeAs="String" value=""  />
   // </AssetManager.Properties.Settings>
 // </userSettings>




       //    checkStructure();
          //  string thing = AssetManager.Properties.Settings.Default.AssetImageFolderPath;
        }


#region Get Tree Data

        private ObservableCollection<TreeCustomer> GetTreeData()
        {
            _conn =  new OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AssetManagerConnectionString"].ConnectionString);                        
            using (_conn)
            {       
                ObservableCollection<TreeCustomer>  ztcs = new ObservableCollection<TreeCustomer>();
                _conn.Open();
                OleDbCommand oc = new OleDbCommand("SELECT ID, Name, Location, IconFile FROM Customers ORDER BY Name", _conn);              
                OleDbDataReader customerReader = oc.ExecuteReader(CommandBehavior.CloseConnection);               
                while (customerReader.Read())
                {
                    tc= new TreeCustomer();
                    tc.ID = (customerReader["ID"] is DBNull) ? 0 : System.Convert.ToInt32(customerReader["ID"]);
                    tc.Name = (customerReader["Name"] is DBNull) ? "" : customerReader["Name"].ToString();
                    tc.Location =(customerReader["Location"] is DBNull) ? "" : customerReader["Location"].ToString();
                    tc.IconFile = (customerReader["IconFile"] is DBNull) ? "" : customerReader["IconFile"].ToString();
                    //add children recursively
                    GetAsset(null, tc.ID);                    
                    //add customer with children to tree
                    ztcs.Add(tc);                    
                }
                customerReader.Close();
                //return  tcs as TreeCustomers;   
                return ztcs;
            }            
        }
        
        private AssetNode GetAsset(AssetNode  ParentNode, Int32 CustomerID)
        {                
            OleDbCommand oassets = new OleDbCommand();
            OleDbDataReader assetReader;
            if (ParentNode == null )
                    oassets.CommandText = "SELECT Assets.AssetID, ParentAssetID, Assets.Label, Assets.ImageFile, Assets.CustomerID, AssetGroups.[Group] as Category, AssetTypes.Description"
                    + " FROM ((Assets LEFT OUTER JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID) LEFT OUTER JOIN AssetGroups ON Assets.GroupID = AssetGroups.ID)"
                    + " WHERE CustomerID=" + CustomerID + " AND ParentAssetID IS NULL"
                    + " ORDER BY Assets.Label";

            else
                oassets.CommandText = "SELECT Assets.AssetID, ParentAssetID, Assets.Label, Assets.ImageFile, Assets.CustomerID, AssetGroups.[Group] as Category, AssetTypes.Description"
                            + " FROM ((Assets LEFT OUTER JOIN AssetTypes ON Assets.TypeID = AssetTypes.ID) LEFT OUTER JOIN AssetGroups ON Assets.GroupID = AssetGroups.ID)"
                            + " WHERE CustomerID=" + CustomerID + " AND ParentAssetID =" + ParentNode.ID
                            + " ORDER BY Assets.Label";
            
            oassets.Connection = _conn;
            assetReader = oassets.ExecuteReader();
            AssetNode ta=null;                
            while (assetReader.Read())
            {
                ta = new AssetNode();
                ta.ID = (assetReader["AssetID"] is DBNull) ? 0 : System.Convert.ToInt32(assetReader["AssetID"]);                                                          
                if(assetReader["ParentAssetID"] is DBNull) 
                    ta.ParentAssetID = null;
                else
                    ta.ParentAssetID = System.Convert.ToInt32(assetReader["ParentAssetID"]);
                ta.CustomerID = (assetReader["CustomerID"] is DBNull) ? 0 : System.Convert.ToInt32(assetReader["CustomerID"]);
                ta.Label = (assetReader["Label"] is DBNull) ? "" : assetReader["Label"].ToString();
                ta.ImageFile = (assetReader["ImageFile"] is DBNull) ? "" : assetReader["ImageFile"].ToString();
                ta.Description = (assetReader["Description"] is DBNull) ? "" : assetReader["Description"].ToString();
                ta.Category = (assetReader["Category"] is DBNull) ? "" : assetReader["Category"].ToString();                       
                AssetNode children =GetAsset(ta, CustomerID);
                                               
                if(ParentNode==null)
                    tc.AssetCollection.Add(ta);
                else
                    ParentNode.Children.Add(ta);
                        
                //  System.Diagnostics.Debug.WriteLine("Parent Node:" + ta.ParentAssetID.ToString() + " Node:" + ta.ID.ToString());
            }
            assetReader.Close();           
    //        oassets.Dispose();
    //        oassets = null;
        //    assetReader.Dispose();
        //      assetReader = null;

            return ta;                              
        }

#endregion 



        




private void checkStructure()
        {
            IList<TreeCustomer> tcs = GetTreeData();            
            foreach(TreeCustomer tcst in tcs)
            {
                System.Diagnostics.Debug.WriteLine(tcst.Name.ToString());
                foreach (AssetNode an in tcst.AssetCollection)
                {
                    System.Diagnostics.Debug.WriteLine("    ID" + an.ID.ToString() + " ParentID:" + an.ParentAssetID.ToString());
                    foreach (AssetNode an1 in an.Children)
                    {                        
                        System.Diagnostics.Debug.WriteLine("        ID" + an1.ID.ToString() + " ParentID:" + an1.ParentAssetID.ToString());
                        foreach (AssetNode an2 in an1.Children)
                        {
                            System.Diagnostics.Debug.WriteLine("               ID" + an2.ID.ToString() + " ParentID:" + an2.ParentAssetID.ToString());
                            foreach (AssetNode an3 in an2.Children)
                            {
                                System.Diagnostics.Debug.WriteLine("                            ID" + an3.ID.ToString() + " ParentID:" + an3.ParentAssetID.ToString());
                            }
                        }
                    }
                }               
            }
        }



        private void test(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
            }
            DependencyObject obj = e.OriginalSource as DependencyObject;
            TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;          
            
           if(((item.Header).GetType().Equals(typeof(AssetNode))))        
           {
                Int32 ID = ((AssetNode)(item.Header)).ID;

                EquipmentWindow EW = new EquipmentWindow(false, ID);
                EW.Owner = this;
                EW.ShowDialog();
               
//                ((AssetNode)(item.Header)).Label = "CSE-11-111";

           }
           else
                if (((item.Header).GetType().Equals(typeof(TreeCustomer))))
                {
                   MessageBox.Show(((TreeCustomer)(item.Header)).Name.ToString());
                }
        }


        private static DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
        {
            var parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }
 
        private void testnode()
        {
           // updateCustomerNode(2, "new", "dunno","");
            updateAssetNode(1, 1313, 1320, "CSE-11-111", "Whatever");
        }

        public void updateCustomerNode(Int32 ID, string Name, string Location, string IconFile)
        {
                       
            foreach (TreeCustomer t in treeview1.ItemsSource)
            {
                if (t.ID == ID)
                {
                    t.Name = Name;
                    t.Location = Location;
                    t.IsExpanded = true;
                }                
            }
        }

        public void updateAssetNode(Int32 CustomerID, Int32 AssetID, Int32 ParentID, string Label, string Description)
        {
   

            AssetNode target=null;          

            foreach (TreeCustomer t in tcs)// treeview1.ItemsSource)
            {
                if (t.ID == CustomerID)
                {
                    t.IsExpanded = true;
                    t.Name = t.Name + " changed";
                                        
                    //top level search only
                    foreach(AssetNode a in t.AssetCollection)
                    {
                        if (a.ID == AssetID)
                        {
                            target = a;
                            t.AssetCollection.Remove(a);                                                                 
                            break;            
                        }                                                
                    }
                }
            }

            foreach (TreeCustomer t in tcs)// treeview1.ItemsSource)
            {
                if (t.ID == CustomerID)
                {                                
                    //top level search only
                    foreach (AssetNode b in t.AssetCollection)
                    {
                        if (b.ID == ParentID)
                        {
                            b.Children.Add(target);
                            break;
                        }
                    }
                }
            }

        }



        private AssetNode RemoveTargetNode(Int32 CustomerID, Int32 AssetID)
        {
            AssetNode target = null;
            foreach (TreeCustomer t in tcs)// treeview1.ItemsSource)
            {
                if (t.ID == CustomerID)
                {
                    //top level search only
                    foreach (AssetNode a in t.AssetCollection)
                    {
                        if (a.ID == AssetID)
                        {
                            target = a;
                            t.AssetCollection.Remove(a);
                            return target;                            
                        }
                        //need recursive search....



                    }
                }
            }
            return target; //not found
        }

        private void ReAttachTargetNode(AssetNode target, Int32 NewCustomerID, Int32 NewParentID)
        {
            if (target != null)
            {
                foreach (TreeCustomer t in tcs)// treeview1.ItemsSource)
                {
                    if (t.ID == NewCustomerID)
                    {
                        //top level search only
                        foreach (AssetNode b in t.AssetCollection)
                        {
                            if (b.ID == NewParentID)
                            {
                                b.Children.Add(target);
                                break;
                            }

                           // else recursively search through children

                               

                        }
                    }
                }
            }
        }
    





        public void updateassets()
        {
           // MessageBox.Show("updating");
           // AssetNode an = (AssetNode)treeview1.SelectedItem;
            //an.Label = "CSE-11-111";

          //  MessageBox.Show(an.ID.ToString());

      
            treeview1.ItemsSource = GetTreeData();
           

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btntest_Click(object sender, RoutedEventArgs e)
        {
            testnode();

        }




    }

    class TreeViewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TreeViewItem item = (TreeViewItem)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }

   public static class ExtensionMethods 
{
  private static Action EmptyDelegate = delegate() { };

 
  public static void Refresh(this UIElement uiElement)

  {
   uiElement.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, EmptyDelegate);
  }
}



}
