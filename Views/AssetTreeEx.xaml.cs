using System;
using System.Windows;


namespace AssetManager
{
    /// <summary>
    /// Interaction logic for AssetTreeEx.xaml
    /// </summary>
    public partial class AssetTreeEx : Window
    {

        string _connectionString;

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;

                MessageBox.Show("Whoops! Please contact the developer with the following information:\n\n" + ex.Message + ex.StackTrace,
                      "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            finally
            {
                Application.Current.Shutdown();
            }
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
            e.Handled = true;
        }

        public AssetTreeEx()
        {
            InitializeComponent();

            //handle other errors
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
                             
        }

        private void LoadInitialData()
        {
   
            //get config.xml file location which should be in same folder as exe
            System.Reflection.Assembly asmly = System.Reflection.Assembly.GetExecutingAssembly();
            //To get the Directory path
            string theDirectory = System.IO.Path.GetDirectoryName(asmly.Location);
            if (System.IO.File.Exists(theDirectory + "\\config.xml"))
                ConfigFileClass.ConfigFilePath = theDirectory + "\\config.xml";
            else
                ConfigFileClass.ConfigFilePath = "c:\\temp\\config.xml";

            ConfigFileManager cf = new ConfigFileManager();

            if (cf.UseDefaultSettings)
            {
                ConfigFileClass.DatabaseConnectionString = ConfigFileClass.DefaultDatabaseConnectionString;
                ConfigFileClass.PhotosPath = ConfigFileClass.DefaultPhotosPath;

                _connectionString = ConfigFileClass.DatabaseConnectionString;// cf.GetConnectionString("AssetManagerConnectionString");                      

                //     LoadInitialData();

            }
            else
            {
                if (!cf.ConfigFileExists)
                {
                    MessageBox.Show("The Config.XML file is missing.", "Unable to start - Config.xml Missing", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Application.Current.Shutdown();
                }
                else
                {
                    if (!cf.ConfigIsWellFormed)
                    {
                        MessageBox.Show("The Config.XML file is corrupted.", "Unable to start - Config.xml file corrupted", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        if (!cf.DatabaseFileExists)
                        {
                            MessageBox.Show("The database file could not be found.", "Unable to start - database file missing", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            Application.Current.Shutdown();
                        }
                        else
                        {
                            if (!cf.PhotoFolderExists)
                            {
                                MessageBox.Show("The photos folder could not be found.", "Unable to start - photos folder missing", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                Application.Current.Shutdown();
                            }
                            else
                            {
                                ConfigFileClass.DatabaseConnectionString = cf.GetConnectionStringFromConfig("AssetManagerConnectionString");
                                ConfigFileClass.PhotosPath = cf.GetPhotosFolderFromConfig("PhotosFileLocation");

                                _connectionString = ConfigFileClass.DatabaseConnectionString;// cf.GetConnectionString("AssetManagerConnectionString");
                               
                                //     this.Loaded += (o, e) =>
                                //      {
                                //        LoadInitialData();
                                //      };                                                   
                            }
                        }
                    }
                }
            }

        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window w in this.OwnedWindows)
            {
                w.Close();
            }
            Application.Current.Shutdown();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            LoadInitialData();

            this.DataContext = new ViewModels.AssetTreeExViewModel();
        }



        //Treeview interaction - not MVVM
        /*       private void NodeDoubleClick(object sender, MouseButtonEventArgs e)
               {
                   if (sender is TreeViewItem)
                   {
                       if (!((TreeViewItem)sender).IsSelected)
                          return;

                       DependencyObject obj = e.OriginalSource as DependencyObject;
                       TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;
                       //item.Header contains the data for the node of a particular Type
                       if (((item.Header).GetType().Equals(typeof(AssetNodeEx))))
                       {
                           int ID = ((AssetNodeEx)(item.Header)).ID;
                           EquipmentWindow EW = new EquipmentWindow(ID);
                           EW.Owner = this;
                           EW.Show();
                           e.Handled = true;
                       }         
                   }
               }

               //get treeview node
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

               */
        //=============================================================================================================================================
        //                  Old code - hold onto if drag n drop required

        /*      private void checkStructure()
              {
                  IList<TreeCustomerEx> tcsex = GetTreeDataEx("");
                  foreach (TreeCustomerEx tcst in tcsex)
                  {
                      System.Diagnostics.Debug.WriteLine(tcst.Name.ToString());
                      if (tcst.AssetNodeExChildrenP != null) 
                      { 
                          foreach (AssetNodeEx an in tcst.AssetNodeExChildrenP)
                          {
                              System.Diagnostics.Debug.WriteLine("    ID" + an.ID.ToString() + " ParentID:" + an.ParentAssetID.ToString());

                              foreach (AssetNodeEx an1 in an.AssetNodeExChildren)
                              {
                                  System.Diagnostics.Debug.WriteLine("        ID" + an1.ID.ToString() + " ParentID:" + an1.ParentAssetID.ToString());
                                  foreach (AssetNodeEx an2 in an1.AssetNodeExChildren)
                                  {                                      
                                      System.Diagnostics.Debug.WriteLine("               ID" + an2.ID.ToString() + " ParentID:" + an2.ParentAssetID.ToString());
                                      foreach (AssetNodeEx an3 in an2.AssetNodeExChildren)
                                      {                                      
                                          System.Diagnostics.Debug.WriteLine("                      ID" + an3.ID.ToString() + " ParentID:" + an3.ParentAssetID.ToString());                                                                     
                                          foreach (AssetNodeEx an4 in an3.AssetNodeExChildren)
                                          {                                     
                                              System.Diagnostics.Debug.WriteLine("                           ID" + an4.ID.ToString() + " ParentID:" + an4.ParentAssetID.ToString());
                                              foreach (AssetNodeEx an5 in an4.AssetNodeExChildren)
                                              {                                     
                                                  System.Diagnostics.Debug.WriteLine("                                  ID" + an5.ID.ToString() + " ParentID:" + an5.ParentAssetID.ToString());
                                                  foreach (AssetNodeEx an6 in an5.AssetNodeExChildren)
                                                  {                                     
                                                      System.Diagnostics.Debug.WriteLine("                                        ID" + an6.ID.ToString() + " ParentID:" + an6.ParentAssetID.ToString());
                                                      foreach (AssetNodeEx an7 in an6.AssetNodeExChildren)
                                                      {                                    
                                                          System.Diagnostics.Debug.WriteLine("                                               ID" + an7.ID.ToString() + " ParentID:" + an7.ParentAssetID.ToString());                                                    
                                                          foreach (AssetNodeEx an8 in an7.AssetNodeExChildren)
                                                          {                                                        
                                                              System.Diagnostics.Debug.WriteLine("                                                     ID" + an8.ID.ToString() + " ParentID:" + an8.ParentAssetID.ToString());
                                                              foreach (AssetNodeEx an9 in an8.AssetNodeExChildren)
                                                              {
                                                                  System.Diagnostics.Debug.WriteLine("                                                              ID" + an9.ID.ToString() + " ParentID:" + an9.ParentAssetID.ToString());
                                                                  foreach (AssetNodeEx an10 in an9.AssetNodeExChildren)
                                                                  {
                                                                      System.Diagnostics.Debug.WriteLine("                                                                        ID" + an10.ID.ToString() + " ParentID:" + an10.ParentAssetID.ToString());
                                                                      foreach (AssetNodeEx an11 in an10.AssetNodeExChildren)
                                                                      {
                                                                          System.Diagnostics.Debug.WriteLine("                                                                                  ID" + an11.ID.ToString() + " ParentID:" + an11.ParentAssetID.ToString());
                                                                      }                                                             
                                                                  }                                                         
                                                              }                                                                                                                                                                                                                 
                                                          }                                                                                                                                                                                                                                                 
                                                      }                                            
                                                  }                                                                          
                                              }
                                          }
                                      }
                                  }
                              }
                          }
                      }
                  }
              }
           */
        /*     private void NodeSelected(object sender, MouseButtonEventArgs e)
            {
                if (sender is TreeViewItem)
                {
                    if (!((TreeViewItem)sender).IsSelected)
                       return;                
                }
                DependencyObject obj = e.OriginalSource as DependencyObject;
                TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;

                if (((item.Header).GetType().Equals(typeof(AssetNodeEx))))
                {
                    int ID = ((AssetNodeEx)(item.Header)).ID;            
                }
            }

            private void NodeDeSelected(object sender, MouseButtonEventArgs e)
            {
                if (sender is TreeViewItem)
                {
                    if (!((TreeViewItem)sender).IsSelected)
                       return;                
                }
                DependencyObject obj = e.OriginalSource as DependencyObject;
                TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;

                if (((item.Header).GetType().Equals(typeof(AssetNodeEx))))
                {
                    int ID = ((AssetNodeEx)(item.Header)).ID;
                }
            }

            public void updateCustomerNode(int ID, string Name, string Location, string IconFile)
            {
                foreach (TreeCustomerEx t in treeview2.ItemsSource)
                {
                    if (t.ID == ID)
                    {
                        t.Name = Name;
                        t.Location = Location;
                        t.IsExpanded = true;
                    }
                }
            }

           public void updateAssetNode(int CustomerID, int AssetID, int ParentID, string Label, string Description)
            {
                AssetNodeEx target = GetAssetNode(CustomerID, AssetID);
                target.Label = Label;
                target.Description = Description;
            }

            private TreeCustomerEx GetCustomerNode(int CustomerID)
            {
                foreach (TreeCustomerEx t in tcsex)// treeview1.ItemsSource)
                {
                    if (t.ID == CustomerID)
                    {
                        return t;                    
                    }
                }
                return null;
            }

            private AssetNodeEx GetAssetNode(int CustomerID, int AssetID)
            {           
                foreach (TreeCustomerEx t in tcsex)// treeview1.ItemsSource)
                {
                    if (t.ID == CustomerID)
                    {
                        //top level search only                    
                        foreach (AssetNodeEx a in t.AssetNodeExChildrenP)
                        {                                             
                            if (a.ID == AssetID)
                               return a;                                                                                   
                            else
                            {
                                //recursively get node from AssetNodeEx using a.AssetNodeExChildrenP as starting collection                            
                               AssetNodeEx n= FindNode(a, AssetID);// when found
                               if (n != null)
                                   return n;                                                    
                            }                 
                        }
                    }            
                }
                return null;       
            }

            private AssetNodeEx FindNode(AssetNodeEx node, int ID)
            {
                if (node == null)
                    return null;
                //maybe not required
                if (node.ID == ID)
                    return node;
                foreach (AssetNodeEx a in node.AssetNodeExChildren)
                {
                    AssetNodeEx foundNode = FindNode(a, ID);
                    if (foundNode != null)
                        return foundNode;
                }
                return null;
            }

            private AssetNodeEx GetAssetNodeParent(int CustomerID, int AssetID)
            {
                return null;
            }

            // Helper to search up the VisualTree
            private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
            {
                if (current != null)
                {
                    do
                    {
                        if (current is T)
                            return (T)current;
                        current = VisualTreeHelper.GetParent(current);
                    }
                    while (current != null);
                }
                return null;
            }

            private static T FindParent<T>(DependencyObject current) where T : DependencyObject
            {
                if (current != null)
                {
                    do
                    {
                        current = VisualTreeHelper.GetParent(current);
                        if (current is T)
                            return (T)current;
                    }
                    while (current != null);
                }
                    return null;

            }

            private void FindDropTargetParent(TreeView tv, out TreeViewItem pItemNode, DragEventArgs pDragEventArgs)
            {
                pItemNode = null;
                DependencyObject k = VisualTreeHelper.HitTest(tv, pDragEventArgs.GetPosition(tv)).VisualHit;
                while (k != null)
                {
                    if (k is TreeViewItem)
                    {
                        TreeViewItem treeNode = k as TreeViewItem;
                        if (treeNode.DataContext is AssetNodeEx)
                        {
                            pItemNode = treeNode;
                            break;
                        }
                        else
                            if (treeNode.DataContext is TreeCustomerEx)
                            {
                                pItemNode = treeNode;
                                break;
                            }
                    }
                    else if (k == tv)
                    {
                        return;
                    }
                    k = VisualTreeHelper.GetParent(k);
                }
            }

            private bool FindDropTargetInParentTree(int sourceid,  TreeView tv, DragEventArgs pDragEventArgs)
            {    
                DependencyObject k = VisualTreeHelper.HitTest(tv, pDragEventArgs.GetPosition(tv)).VisualHit;
                while (k != null)
                {
                    if (k is TreeViewItem)
                    {
                        TreeViewItem treeNode = k as TreeViewItem;
                        if (treeNode.DataContext is AssetNodeEx)
                        {
                            AssetNodeEx _dc = treeNode.DataContext as AssetNodeEx;
                            if (_dc.ID == sourceid)
                               return true;                        
                        }
                    }
                    k = VisualTreeHelper.GetParent(k);
                }
                return false;
            }

            private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                foreach (Window w in this.OwnedWindows)
                {
                    w.Close();
                }
            }

    */
        //======================================================================================================================================

    }


}