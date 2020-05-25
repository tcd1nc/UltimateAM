
using System.Windows.Input;
using static AssetManager.SQLiteQueries;

namespace AssetManager.ViewModels
{
    public  class UserSettingsViewModel: ViewModelBase
    {

        public UserSettingsViewModel()
        {
            //bool _result;
            //bool _isbool = bool.TryParse(Application.Current.Resources["ShowActivitiesOnStart"].ToString(), out _result);
            //ShowScheduledActivities = _result;

            PhotosFileLocation = GlobalClass.Defaults.PhotosFileLocation;
        }

        
        bool _showscheduledactivities;
        public bool ShowScheduledActivities
        {
            get { return _showscheduledactivities; }
            set { SetField(ref _showscheduledactivities, value); }
        }


        string photofolder;
        public string PhotosFileLocation
        {
            get {return photofolder; }
            set { SetField(ref photofolder, value); }
        }


        bool canexecutesselectfolder = true;
        private bool CanExecuteSelectFolder(object obj)
        {
            return canexecutesselectfolder;
        }

        ICommand selectfolder;
        public ICommand SelectPhotosFolder
        {
            get
            {
                if (selectfolder == null)
                    selectfolder = new DelegateCommand(CanExecuteSelectFolder, ExecuteSelectFolder);
                return selectfolder;
            }
        }

        private void ExecuteSelectFolder(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string folder = msg.SelectFolderDlg();
            if (!string.IsNullOrEmpty(folder))
                PhotosFileLocation = folder;
            msg = null;
        }
        
        private void Save()
        {
            GlobalClass.Defaults.PhotosFileLocation = PhotosFileLocation;
            UpdateDefaultSettings(GlobalClass.Defaults);
        }


        bool _canexecutesave = true;
        private bool CanExecuteSave(object obj)
        {
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

            Save();

            DialogResult = true;
            
        }

        

    }
}
