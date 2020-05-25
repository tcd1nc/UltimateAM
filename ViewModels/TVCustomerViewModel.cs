using System;
using System.Windows;
using AssetManager.DragDrop;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class TVCustomerViewModel : UserControls.TreeViewItemViewModel, IDropable
    {
        TVCustomerModel customer;

        public TVCustomerViewModel(TVCustomerModel parentcustomer) : base(null, true)
        {
            Customer = parentcustomer;
            IsExpanded = false;
            IsSelected = false;
            CheckBoxVisibility = Visibility.Collapsed;
        }
               
        public TVCustomerModel Customer
        {
            get { return customer; }
            set { SetField(ref customer, value); }
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
            if (data is TVAssetViewModel source)
            {
                if (source.Asset.ParentAssetID == 0 && Customer.ID == source.Asset.CustomerID) //if dragged and dropped yourself, don't need to do anything
                    return;

                SQLiteQueries.UpdateParentAssetID(source.Asset.ID, 0, Customer.ID);
                AssetTreeExViewModel.MoveAsset(source.Asset.ID, 0, Customer.ID);

            }
        }

        #endregion

    }
}
