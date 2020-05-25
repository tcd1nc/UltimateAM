using System;
using System.Data;
using System.Windows.Input;
using static AssetManager.SQLiteQueries;
using static AssetManager.GlobalClass;
using AssetManager.Models;

namespace AssetManager.ViewModels
{
   public  class MovementsReportDateFilterViewModel :ViewModelBase
    {
        private string _reportname;
        private DataTable _reportdata;
        const string _movementsreport = "GetMovementsReport";

        public MovementsReportDateFilterViewModel(ReportModel _rm)
        {
            _reportname = _rm.Parameter;
            ReportTitle = _rm.Header;
            DataMonth = DateTime.Now;            
        }

        public DataTable ReportData
        {
            get { return _reportdata; }
            set { SetField(ref _reportdata, value); }                
        }
        
        public string ReportTitle
        {
            get; set;
        }

        DateTime _datamonth;

        public DateTime DataMonth
        {
            get { return _datamonth; }
            set {
                if(value!=null)
                    GetData(value);
                SetField(ref _datamonth, value); }
        }

        private void GetData(DateTime _month)
        {
            string _strmonth = string.Empty;
            _strmonth = _month.Month + "-" + _month.Year;

            if (!string.IsNullOrEmpty(_strmonth))
            {
               ReportData = GetDateFilteredMovementReportData(_strmonth);              
            }
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
