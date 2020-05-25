using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using static AssetManager.GlobalClass;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
    public class DeletedAssetsViewModel :ViewModelBase
    {

        FullyObservableCollection<TVDeletedAssetViewModel> _deletedassets = new FullyObservableCollection<TVDeletedAssetViewModel>();
        public DeletedAssetsViewModel()
        {
            FullyObservableCollection<AssetSummaryModel> _assetsummary = GetDeletedAssets();

            AssetModel _deletedAsset;
            foreach (AssetSummaryModel asset in _assetsummary)
            {
                _deletedAsset = new AssetModel
                {
                    ID = asset.ID,
                    Label = MakeLabel(asset.AssetAreaID, asset.AssetGroupID, asset.LabelID),// asset.Label;
                    Name = asset.Description,
                    ParentAssetID = asset.ParentAssetID
                };
                _deletedassets.Add(new TVDeletedAssetViewModel(_deletedAsset, null));
            }
            DeletedAssets.ItemPropertyChanged += DeletedAssets_ItemPropertyChanged;
            
        }

        private void DeletedAssets_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            TVDeletedAssetViewModel _asset = DeletedAssets[e.CollectionIndex];
            if (CheckedItems.Contains(_asset.Asset))
            {
                if (_asset.IsChecked == false)
                    CheckedItems.Remove(_asset.Asset);
            }
            else
            {
                if (_asset.IsChecked == true)
                    CheckedItems.Add(_asset.Asset);
            }
        }

        public FullyObservableCollection<TVDeletedAssetViewModel> DeletedAssets
        {
            get { return _deletedassets; }
            set { SetField(ref _deletedassets, value); }
        }

        FullyObservableCollection<AssetModel> _selectedassets = new FullyObservableCollection<AssetModel>();
        public FullyObservableCollection<AssetModel> CheckedItems
        {
            get { return _selectedassets; }
            set { SetField(ref _selectedassets, value); }
        }

        #region Commands
        ICommand _undelete;
        public ICommand Undelete                                                       
        {
            get
            {
                if (_undelete == null)
                    _undelete = new DelegateCommand(CanUndelete, ExecuteUndelete);
                return _undelete;
            }
        }

        bool _canundelete = false;
        private bool  CanUndelete (object obj)
        {
            if (CheckedItems.Count > 0)
                return true;

            return _canundelete;
        }

        private void ExecuteUndelete(object parameter)                                        
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            if (_msgboxcommand.ShowMessage("The selected Assets will be undeleted and placed in the default customer location", "Undeleting", GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Information).Equals(GenericMessageBoxResult.OK))
            {
              
                int result = _msgboxcommand.OpenDefaultCustomersDlg();
                if (result > 0)
                {
                    foreach (Models.AssetModel am in CheckedItems)
                    {
                        ViewModels.AssetTreeExViewModel.UnDeleteAsset(am, result);
                    }
                    DialogResult = true;                    
                }
            }
            _msgboxcommand = null;
        }

        ICommand _close;
        public ICommand CloseForm
        {
            get
            {
                if (_close == null)
                    _close = new DelegateCommand(CanClose, ExecuteClose);
                return _close;
            }
        }

        bool _canclose = true;
        private bool CanClose(object obj)
        {

            return _canclose;
        }

        private void ExecuteClose(object parameter)
        {
            DialogResult = true;         
        }
            #endregion
        }
}
