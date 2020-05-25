
namespace AssetManager.ViewModels
{
    public class TVDeletedAssetViewModel : UserControls.TreeViewItemViewModel
    {
        Models.AssetModel _asset;

        public TVDeletedAssetViewModel(TVDeletedAssetViewModel _parentasset) : base(_parentasset, true)
        {
            Asset = _parentasset.Asset;
        }

        public TVDeletedAssetViewModel(Models.AssetModel _asset, TVDeletedAssetViewModel _assetvm) : base(_assetvm, true)
        {
            Asset = _asset;
            IsExpanded = false;
            IsSelected = false;
        }

        //protected override void LoadChildren()
        //{
        //    foreach (Models.AssetModel am in SQLiteQueries.GetDeletedChildAssets(Asset.ID))
        //        base.Children.Add(new TVDeletedAssetViewModel(am, this));
        //}

        public Models.AssetModel Asset
        {
            get { return _asset; }
            set { SetField(ref _asset, value); }
        }

        

    }
}
