using System.Data.OleDb;
using System.Xml;

namespace Project_Tracker
{
    public class ConfigFileManager
    {
        XmlDocument _xmlDoc;
        string _xmlFileName;

        public ConfigFileManager(string configpath)
        {
            _xmlFileName = configpath;
        }

        #region Properties

       
        public bool ConfigIsWellFormed
        {
            get { return XMLIsWellFormed(); }           
        }

        public bool ConfigFileExists
        {
            get { return ConfigFlExists(); }
        }

        public bool DatabaseFileExists
        {
            get { return DBFileExists(GetConnectionStringFromConfig("ProjectTrackerConnectionString")); }
        }

        #endregion

        #region Public functions

        public string GetConnectionStringFromConfig(string _databaseName)
        {            
            return GetParameter("connectionStrings", "add", "name", _databaseName, "connectionString");
        }


        public string GetParameter(string _section, string _elementName, string _paramName, string _paramCriteria, string _attributeName)
        {
            _xmlDoc = GetRootNode();
            if (!string.IsNullOrEmpty(_section))
                _section = _section + "/";

            try
            {
                return _xmlDoc.DocumentElement.SelectSingleNode("//" + _section + _elementName + "[@" + _paramName + "='" + _paramCriteria + "']/@" + _attributeName).Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public XmlDocument GetRootNode()
        {
            _xmlDoc = new XmlDocument();         
            _xmlDoc.Load(_xmlFileName);
            return _xmlDoc;
        }       

        #endregion

        #region Private functions

        private bool ConfigFlExists()
        {
            return System.IO.File.Exists(_xmlFileName);                       
        }

        private bool DBFileExists(string _databaseConnectionstring)
        {
            string _databaseFileName;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = _databaseConnectionstring;
            _databaseFileName = conn.DataSource;
            conn.Dispose();
            
            if (System.IO.File.Exists(_databaseFileName))
                return true;
            else
                return false;
        }
        
        private bool XMLIsWellFormed()
        {
            bool _IsWellFormed = false;

            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(_xmlFileName);
                _IsWellFormed = true;
            }
            catch 
            {
                _IsWellFormed =  false;
            }

            return _IsWellFormed;
        }
        
        //private bool UseDefSettings()
        //{
        //    if (System.IO.File.Exists(ConfigFileClass.DefaultDatabasePath))                                      
        //        return true;                            
        //    else
        //        return false;
        //}

        #endregion
    }


    

}
