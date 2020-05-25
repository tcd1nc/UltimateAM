using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;
using System.IO;
using System;

namespace AssetManager
{
    public enum ConfigFileError
    {
        [Description("Not connected")]
        NotConnected,
        [Description("Connection Ok")]
        ConnectionOk,
        [Description("Database File Not found")]
        DBNotFound,
        [Description("Photos folder Not found")]
        PhotosFolderNotFound,
        [Description("XML not well formed")]
        NotWellformed,
        [Description("XAML not well formed")]
        XAMLNotWellFormed
        
    }

    public class ConfigFileManager
    {
        string _defaultdbconnectionstring = string.Empty;
        string _defaultPhotosPath = string.Empty;
        string _sharepointdbpath = string.Empty;

        const int _defaultmaxphotosize = 100000;
        const long _defaulttargetphotoquality = 75;
        const int _defaultphotoheightandwidth = 300;

        const string _constconfigfilename = "\\AssetManager.exe.config";
        const string _constdbconnectionstring = "AssetManagerConnectionString";
        const string _constphotofilelocation = "PhotosFileLocation";
        const string _constsharepointdbpath = "SharePointDBPath";
        const string _constcopytosharepoint = "CopyToSharePoint";

        Configuration cfg;

        public ConfigFileManager()
        {
            try
            {
                ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
                filemap.ExeConfigFilename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + _constconfigfilename;
                cfg = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);
            }
            catch { }
        }

        public void SetUp()
        {
            bool _dbfilesetupok = true;
            bool _photosfoldersetupok = true;
            DataConnection = ConfigFileError.NotConnected;

            try
            {
                //ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
                //filemap.ExeConfigFilename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + _constconfigfilename;
                //cfg = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);

                _defaultdbconnectionstring = cfg.ConnectionStrings.ConnectionStrings[_constdbconnectionstring].ToString();
                _defaultPhotosPath = cfg.AppSettings.Settings[_constphotofilelocation].Value.ToString();

                MaxPhotoSize = GetMaxPhotoSizeFromConfig();
                TargetPhotoQuality = GetTargetPhotoQualityFromConfig();
                PhotoHeightAndWidth = GetPhotoHeightAndWidthFromConfig();
                ShowActivitiesOnStart = GetShowActivitiesOnStartFromConfig();
                SharePointDBPath = GetSharePointDBPathFromConfig();
                CopyToSharePoint = GetCopyToSharePointFromConfig();

                //config file data is ok to use
                if (dBFileExists(_defaultdbconnectionstring))
                    DatabaseConnectionString = _defaultdbconnectionstring;
                else
                {
                    _dbfilesetupok = GetDBLocation();
                    if (!_dbfilesetupok)
                    {
                        DataConnection = ConfigFileError.DBNotFound;
                        DatabaseError = "\nDatabase file not found";
                    }
                }

                if (photoFlderExists(_defaultPhotosPath))
                    PhotosPath = _defaultPhotosPath;
                else
                {
                    _photosfoldersetupok = GetPhotosFolderPath();
                    if (!_photosfoldersetupok)
                    {
                        DataConnection = ConfigFileError.PhotosFolderNotFound;
                        DatabaseError = "\nPhotos Folder not found";
                    }
                }

                if (_dbfilesetupok && _photosfoldersetupok)
                {
                    DataConnection = ConfigFileError.ConnectionOk;
                    GlobalClass.SetInitialConnection(DatabaseConnectionString);
                }
            }
            catch (Exception ex)
            {
                DatabaseError = ex.Message;
                DataConnection = ConfigFileError.NotConnected;
            }

            try {
                if (GlobalClass.Conn?.State == System.Data.ConnectionState.Open)
                    DataConnection = ConfigFileError.ConnectionOk;
                else
                    DataConnection = ConfigFileError.NotConnected;
            }
            catch
            {
                DataConnection = ConfigFileError.NotConnected;
            }
                

        }


        private string DefaultDatabasePath()
        {
            //return Environment.GetEnvironmentVariable("USERPROFILE") + "\\SharePoint\\AU_NZ Equipment Database - Document\\";
            return Environment.GetEnvironmentVariable("USERPROFILE") + "\\OneDrive - Buckman\\";
        }

        private string DefaultSharePointPath()
        {

#if DEBUG
            return Environment.GetEnvironmentVariable("USERPROFILE") ;
#else
return Environment.GetEnvironmentVariable("USERPROFILE") + "\\SharePoint\\AU_NZ Equipment Database - Document\\";   
#endif

        }


        //private string DefaultDatabaseFileName()
        //{           
        //        return DefaultDatabasePath() + "NewAssetManager.accdb";           
        //}

        //private string DefaultDatabaseConnectionString()
        //{            
        //    return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + DefaultDatabaseFileName() + "'";            
        //}

        //private string DefaultPhotosPath()
        //{
        //    //return DefaultDatabasePath() + "Asset Images\\";

        //    return Environment.GetEnvironmentVariable("USERPROFILE") + "\\SharePoint\\AU_NZ Equipment Database - Document\\Asset Images\\";

        //}

        public string SharePointDBPath { get;  set; }

        public string CopyToSharePoint { get; set; }

        public string DatabaseConnectionString { get; set; }

        public string PhotosPath { get; set; }

        public int MaxPhotoSize { get; set; }

        public long TargetPhotoQuality { get; set; }

        public int PhotoHeightAndWidth { get; set; }

        public bool ShowActivitiesOnStart { get; set; }

        public ConfigFileError DataConnection { get; set; }

        public string DatabaseError { get; set; }
       
        public string GetSharePointDBPathFromConfig()
        {
            string _path = string.Empty;
            try
            {
                return cfg.AppSettings.Settings[_constsharepointdbpath].Value;
            }
            catch { }
            return _path;
        }

        public string GetCopyToSharePointFromConfig()
        {
            string _path = string.Empty;
            try
            {
                return cfg.AppSettings.Settings[_constcopytosharepoint].Value;
            }
            catch { }
            return _path;
        }


        public int GetMaxPhotoSizeFromConfig()
        {
            int _maxsize = _defaultmaxphotosize;
            try
            {
                bool _isint = int.TryParse(cfg.AppSettings.Settings["MaxPhotoSize"].Value, out _maxsize);
            }
            catch { }
            return _maxsize;            
        }

        public long GetTargetPhotoQualityFromConfig()
        {
            long _targetquality = _defaulttargetphotoquality;
            try {
                bool _islng = long.TryParse(cfg.AppSettings.Settings["TargetPhotoQuality"].Value, out _targetquality);
            }
            catch { }
            return _targetquality;
        }

        public int GetPhotoHeightAndWidthFromConfig()
        {
            int _heightandwidth = _defaultphotoheightandwidth;
            try {
                bool _isint = int.TryParse(cfg.AppSettings.Settings["PhotoHeightAndWidth"].Value, out _heightandwidth);
            }
            catch { }
            return _heightandwidth;
        }

        public bool GetShowActivitiesOnStartFromConfig()
        {
            bool _showactivitiesonstart = false;
            try {
                bool _isbool = bool.TryParse(cfg.AppSettings.Settings["ShowActivitiesOnStart"].Value.ToString(), out _showactivitiesonstart);
            }
            catch { }
            return _showactivitiesonstart;
        }

        private bool ConfigFileExists(string _Configfilename)
        {
            if (System.IO.File.Exists(_Configfilename))
                return true;
            else
                return false;
        }

        private bool dBFileExists(string _databaseConnectionstring)
        {
            string _databaseFileName;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _databaseConnectionstring;
            _databaseFileName = conn.DataSource;         
            conn.Dispose();
            if (File.Exists(_databaseFileName))
                return true;
            else
                return false;
        }

        private bool photoFlderExists(string _photoFolderstring)
        {
            return Directory.Exists(_photoFolderstring);
        }

        public static void AddOrUpdateAppSetting(string key, string value)
        {
            try
            {
                ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
                filemap.ExeConfigFilename = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + _constconfigfilename;// "\\AssetManager.exe.config";

                Configuration cfg;
                cfg = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);

                var settings = cfg.AppSettings.Settings;
                    if (settings[key] == null)                    
                        settings.Add(key, value);                    
                    else                    
                        settings[key].Value = value;
                    
                    cfg.Save(ConfigurationSaveMode.Modified); //ConfigurationSaveMode.Full)
                    //System.Configuration.ConfigurationManager.RefreshSection(configfile.AppSettings.SectionInformation.Name);
                    Application.Current.Resources[key] = value;
            }
            catch (ConfigurationErrorsException)
            { }
        }

        private void UpdateAppSetting(string key, string value)
        {
            try
            {
                var settings = cfg.AppSettings.Settings;
                settings[key].Value = value;
                cfg.Save(ConfigurationSaveMode.Full); //ConfigurationSaveMode.Modified)
                ConfigurationManager.RefreshSection("appSettings");
                //Application.Current.Resources[key] = value;
            }
            catch (ConfigurationErrorsException)
            { }
        }

        private bool UpdateConnectionStringSetting(string key, string value)
        {
            bool _success = false;
            try
            {
                var settings = cfg.ConnectionStrings.ConnectionStrings;
                settings[key].ConnectionString = value;
                cfg.Save(ConfigurationSaveMode.Full); //ConfigurationSaveMode.Modified)
                ConfigurationManager.RefreshSection("connectionStrings");
                _success = true;
            }
            catch (ConfigurationErrorsException)
            {
                _success = false;
            }
            return _success;

        }

        private bool GetDBLocation()
        {
            bool _filefound = false;
            while (!_filefound)
            {
                //browse
                var dlg = new System.Windows.Forms.OpenFileDialog();
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = true;
                dlg.DefaultExt = "*.accdb";
                dlg.Multiselect = false;
                dlg.Title = "Database not found - Please Select NewAssetManager Database File";
                dlg.Filter = "Access DB Files (.accdb)|*.accdb";

                if(Directory.Exists(DefaultDatabasePath()))
                    dlg.InitialDirectory = DefaultDatabasePath();
                
                var result = dlg.ShowDialog();
                if(!string.IsNullOrEmpty(dlg.FileName))
                {
                    //build new connection string from dlg.filename
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cfg.ConnectionStrings.ConnectionStrings[_constdbconnectionstring].ToString());
                    // Supply the additional values.
                    builder.DataSource = dlg.FileName;
                    builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                    DatabaseConnectionString = builder.ConnectionString;                                     
                    UpdateConnectionStringSetting(_constdbconnectionstring, DatabaseConnectionString);                                                      
                    return true;
                }
                else                
                    break;                
            }
            return _filefound;
        }

        private bool GetPhotosFolderPath()
        {
            bool _folderfound = false;
            while (!_folderfound)
            {
                //browse
                var dlg = new System.Windows.Forms.FolderBrowserDialog();
                dlg.Description = "Select Asset Images Folder";
                dlg.ShowNewFolderButton = false;
                dlg.RootFolder = Environment.SpecialFolder.MyComputer;                
                var result = dlg.ShowDialog();
                if (!string.IsNullOrEmpty(dlg.SelectedPath))
                {
                    PhotosPath = dlg.SelectedPath +"\\";
                    UpdateAppSetting(_constphotofilelocation, PhotosPath);
                    return true;
                }
                else
                    break;
                                
            }
            return _folderfound;
        }

        public bool SelectSharePointDBLocation()
        {
            bool _filefound = false;
            while (!_filefound)
            {
                //browse
                var dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = false;
                dlg.DefaultExt = "*.accdb";
                dlg.AddExtension = true;
                dlg.CreatePrompt = false;
                dlg.OverwritePrompt = false;
                dlg.Title = "Please Enter or Select a Database File Name to Sync to";
                dlg.Filter = "Access DB Files (.accdb)|*.accdb";
                               
                if (Directory.Exists(DefaultSharePointPath()))
                    dlg.InitialDirectory =  DefaultSharePointPath();

                Window _owner;
                _owner = Application.Current.Windows[0];

                var result = dlg.ShowDialog(_owner.GetIWin32Window());
                if (!string.IsNullOrEmpty(dlg.FileName))
                {
                    string foldername = System.IO.Path.GetDirectoryName(dlg.FileName);
                    if (System.IO.Directory.Exists(foldername))
                    {
                        UpdateAppSetting(_constsharepointdbpath, dlg.FileName);
                        Application.Current.Resources["SharePointDBPath"] = dlg.FileName;
                        return true;
                    }
                    else
                        return false;                    
                }
                else
                    break;
            }
            return _filefound;
        }

    }

    public static class MyWpfExtensions
    {
        public static System.Windows.Forms.IWin32Window GetIWin32Window(this System.Windows.Media.Visual visual)
        {
            var source = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle)
            {
                _handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return _handle; }
            }
            #endregion
        }
    }
}
