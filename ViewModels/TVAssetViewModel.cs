using System;
using System.Windows;
using AssetManager.DragDrop;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class TVAssetViewModel : UserControls.TreeViewItemViewModel, IDropable, IDragable
    {
        AssetModel asset;

        public TVAssetViewModel(TVAssetViewModel parentasset) : base(parentasset, true)
        {
            Asset = parentasset.Asset;            
        }

        public TVAssetViewModel(AssetModel asset, TVAssetViewModel assetvm) : base(assetvm, true)
        {
            Asset = asset;
            IsExpanded = false;
            IsSelected = false;
            CheckBoxVisibility = Visibility.Visible;                        
        }
              
        public AssetModel Asset
        {
            get { return asset; }
            set { SetField(ref asset, value); }
        }
       
        #region IDropable Members
        /// <summary>
        /// Only TVAssetViewModel can be dropped
        /// </summary>
        Type IDropable.DataType
        {
            get { return typeof(TVAssetViewModel); }
        }

        /// <summary>
        /// Drop data into this ViewModel
        /// </summary>
        void IDropable.Drop(object data, int index)
        {
            //if moving within customer, reassign the children to the 
            //level above first

            if (data is TVAssetViewModel source)
            {
                if (source.Asset.ID == Asset.ID || source.Asset.ParentAssetID == Asset.ID) //if dragged and dropped yourself, don't need to do anything
                    return;
                SQLiteQueries.UpdateParentAssetID(source.Asset.ID, Asset.ID, Asset.CustomerID);
                AssetTreeExViewModel.MoveAsset(source.Asset.ID, Asset.ID, Asset.CustomerID);

            }
        }

        #endregion

        #region IDragable Members

        /// <summary>
        /// Only TVAssetViewModel can be dragged
        /// </summary>
        Type IDragable.DataType
        {
            get
            {
                return typeof(TVAssetViewModel);
            }
        }

        /// <summary>
        /// Remove this TVAssetViewModel from the 
        /// TVAssetViewModel
        /// </summary>
        /// <param name="i"></param>
        void IDragable.Remove(object data)
        {

        }

        #endregion

    }
}
