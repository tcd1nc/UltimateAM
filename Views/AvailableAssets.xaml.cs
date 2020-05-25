using System;
using System.Windows;
using System.Windows.Data;

namespace AssetManager.Views
{
    /// <summary>
    /// Interaction logic for AvailableAssets.xaml
    /// </summary>
    public partial class AvailableAssets : Window
    {
        CollectionViewSource _avassets;
        Int32 _parentassetid;
        Int32 _customerid;

       
        public AvailableAssets()
        {
            InitializeComponent();
            DataContext = new ViewModels.AvailableAssetsViewModel();

        }

        public AvailableAssets(Int32 ParentAssetID, Int32 CustomerID)
        {
            InitializeComponent();
            _parentassetid = ParentAssetID;
            _customerid = CustomerID;
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedAssets();
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _avassets = this.FindResource("availableassets") as CollectionViewSource;
            //_avassets.Source = new NoParentAssets(_parentassetid);
        }

        private void AddSelectedAssets()
        {
            for(var i=0; i < lstchkAvailableAssets.SelectedItems.Count; i++ )
            {
                //add asset by updating asset parent id to _assetid
                //NoParentAsset np = lstchkAvailableAssets.SelectedItems[i] as NoParentAsset;
                //       AddToParent(_parentassetid, np.ID,_customerid);
                //if (DataLayer.DatabaseQueries.AddToParent(_parentassetid, np.ID, _customerid) == 0)
                //    this.DialogResult = false;    
            }           
        }

        //public void AddToParent(Int32 ParentID, Int32 AssetID, Int32 CustomerID)
        //{
        //    using (OleDbConnection CONN = new OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings["AssetManagerConnectionString"].ConnectionString))
        //    {
        //       CONN.Open(); 
        //        OleDbCommand oc = new OleDbCommand("UPDATE Assets SET ParentAssetID = " + ParentID + ", CustomerID = " + CustomerID + " WHERE AssetID = " + AssetID,CONN);
        //        int rows = oc.ExecuteNonQuery();
        //        if(rows ==0)
        //            this.DialogResult = false;
        //        CONN.Close();
        //    }
        //}             
    }
}
