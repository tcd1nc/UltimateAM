using System.Data;
using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using static AssetManager.GlobalClass;

namespace AssetManager.ViewModels
{
   public  class ReportViewModel :ViewModelBase
    {

        public ReportViewModel(Models.ReportModel _rm)
        {        
            //ReportData = GetExcelReportData(_rm.Parameter);
            //ReportTitle = _rm.Header;
          
        }

        public DataTable ReportData
        {
            get;
            set;
                
        }
        
        public string ReportTitle
        {
            get; set;
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
            //CreateExcelFile c = new CreateExcelFile();
            //c.CreateExcelReport(ReportData);
        }




    }
}
