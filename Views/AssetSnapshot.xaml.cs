using System;
using System.Windows;
using System.Windows.Data;



namespace AssetManager
{
    /// <summary>
    /// Interaction logic for AssetSnapshot.xaml
    /// </summary>
    public partial class AssetSnapshot : Window
    {

        CollectionViewSource _as;

        public AssetSnapshot()
        {
            InitializeComponent();
        }

        public AssetSnapshot(Int32 AssetID)
        {
            InitializeComponent();

            //get asset description and image file name
            //load image file name
            _as = this.FindResource("AssetView") as CollectionViewSource;
      //      _as.Source = new Assets(AssetID);


        }


        private void bntClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
