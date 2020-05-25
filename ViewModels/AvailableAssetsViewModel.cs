using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class AvailableAssetsViewModel :ViewModelBase
    {
        FullyObservableCollection<Models.AssetSummaryModel> _availableassets;
        FullyObservableCollection<Models.AssetTypeModel> _assettypes;
        int _parentassetid = 0;
        int _parentcustomerid = 0;

        public AvailableAssetsViewModel(object[] _assetdata)
        {
            _parentassetid =(int)_assetdata[0];
            _parentcustomerid = (int)_assetdata[1];

            AssetTypes = DatabaseQueries.GetAssetTypes();
            AvailableAssets = DatabaseQueries.GetAvailableAssets((int)StatusType.Available ,_parentassetid);

        }


        public FullyObservableCollection<Models.AssetSummaryModel> AvailableAssets
        {
            get { return _availableassets; }
            set { SetField(ref _availableassets, value); }
        }

        FullyObservableCollection<Models.AssetSummaryModel> _selectedassets = new FullyObservableCollection<Models.AssetSummaryModel>();
        public FullyObservableCollection<Models.AssetSummaryModel> SelectedItems
        {
            get { return _selectedassets; }
            set { SetField(ref _selectedassets, value); }
        }

        public FullyObservableCollection<Models.AssetTypeModel> AssetTypes
        {
            get { return _assettypes; }
            set { SetField(ref _assettypes, value); }
        }

        #region Commands
                               
        //save
        ICommand _cancel;
        public ICommand Cancel
        {
            get
            {
                if (_cancel == null)
                    _cancel = new DelegateCommand(CanExecute, ExecuteCancel);
                return _cancel;
            }
        }

        private void ExecuteCancel(object parameter)
        {
            DialogResult = false;
            CloseWindow();
        }

        bool _canexecutesave = false;
        private bool CanExecuteSave(object obj)
        {
            if (SelectedItems.Count > 0)
                return true;
            return _canexecutesave;
        }

        ICommand _saveandclose;
        public ICommand SaveAndClose
        {
            get
            {
                if (_saveandclose == null)
                    _saveandclose = new DelegateCommand(CanExecuteSave, ExecuteSaveAndClose);
                return _saveandclose;
            }
        }

        private void ExecuteSaveAndClose(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            Collection<object> _assets =parameter as Collection<object>;
            foreach (Models.AssetSummaryModel aam in SelectedItems)
            {
                DatabaseQueries.UpdateParentAssetID(aam.ID, _parentassetid, _parentcustomerid);
                ViewModels.AssetTreeExViewModel.MoveAsset(aam.ID, _parentassetid, _parentcustomerid);
            }

            DialogResult = true;
            CloseWindow();
        }


        

        #endregion




    }
}
