using System.Data;
using System.Windows.Input;
using static AssetManager.GlobalClass;

namespace AssetManager.ViewModels
{
    public class DataErrorsViewModel : ViewModelBase
    {
        FullyObservableCollection<Models.AssetModel> _assets;
        FullyObservableCollection<Models.AssetModel> _assetswithnolocationinformation;
        public DataErrorsViewModel()
        {
            //Assets = DatabaseQueries.GetDuplicateAssetLabels();
            //AssetsWithNoLocationInformation = DatabaseQueries.GetAssetsWithNoLocationInformation();
            
        }

        public FullyObservableCollection<Models.AssetModel> Assets
        {
            get { return _assets; }
            set { SetField(ref _assets, value); }
        }
        
        public FullyObservableCollection<Models.AssetModel> AssetsWithNoLocationInformation
        {
            get { return _assetswithnolocationinformation; }
            set { SetField(ref _assetswithnolocationinformation, value); }
        }

        ICommand _exporttoexcel;
        public ICommand ExportToExcel
        {
            get
            {
                if (_exporttoexcel == null)
                    _exporttoexcel = new DelegateCommand(CanExecute, ExecuteExportToExcel);
                return _exporttoexcel;
            }
        }

        private void ExecuteExportToExcel(object parameter)
        {
            CreateExcelFile c = new CreateExcelFile();

            if (Assets.Count > 0)
                c.CreateExcelReport(MakeDTReport(Assets,"Duplicate Asset Labels" ));

            if(AssetsWithNoLocationInformation.Count > 0)
                c.CreateExcelReport(MakeDTReport(AssetsWithNoLocationInformation, "Missing Location Data"));

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

        private DataTable MakeDTReport(FullyObservableCollection<Models.AssetModel> _assets, string _name)
        {
            DataTable dt = new DataTable();
            dt.TableName = _name;
            dt.Columns.Add("AssetID");
            dt.Columns.Add("Label");
            dt.Columns.Add("Name");

            DataRow row;
            foreach (Models.AssetModel am in _assets)
            {
                row = dt.NewRow();
                row["AssetID"] = am.ID;
                row["Label"] = am.Label;
                row["Description"] = am.Name;
                dt.Rows.Add(row);
            }
            return dt;
        }

    }

}
