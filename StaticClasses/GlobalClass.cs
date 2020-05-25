using System;
using System.IO;
using AssetManager.Models;

namespace AssetManager
{
    public static class GlobalClass
    {
        public static DefaultSettingsModel Defaults { get; private set; }
        public static FullyObservableCollection<BaseModel> Statuses { get; private set; }
        public static FullyObservableCollection<AssetAreaModel> AssetAreas { get; private set; }
        public static FullyObservableCollection<AssetGroupModel> AssetGroups { get; private set; }
        public static string connstr { get; private set; } 

        public static bool LoadAll()
        {                 
            try
            {
                GetConnStr();
                SetCurrentUser();                                              
                Defaults = SQLiteQueries.GetDefaultSettings();
                Statuses = SQLiteQueries.GetStatuses();
                AssetAreas = SQLiteQueries.GetAssetAreas();
                AssetGroups = SQLiteQueries.GetAssetGroups();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void GetConnStr()
        {
            System.Reflection.Assembly asmly = System.Reflection.Assembly.GetExecutingAssembly();
            connstr = "Data Source = " + Path.Combine(Path.GetDirectoryName(asmly.Location), "Assets.db3");

            //MessageBox.Show(connstr);
        }

        public static void SetCurrentUser()
        {
            try
            {
                AdministratorUserModel _currentuser;
                IsAdministrator = false;
                _currentuser = new AdministratorUserModel();
                _currentuser = SQLiteQueries.GetAdministratorNameFromUserLogin(Environment.UserName);

                if (!string.IsNullOrEmpty(_currentuser.Name))
                    IsAdministrator = true;
            }
            catch
            {
                IsAdministrator = false;
            }

#if DEBUG
                IsAdministrator = true;
#endif

        }
        public static bool IsAdministrator { get; private set; }


        
        public static string MakeLabel(int areaid, int groupid, int labelid)
        {
            string areaprefix = string.Empty;
            foreach(AssetAreaModel am in AssetAreas)            
                if(am.ID == areaid)
                {
                    areaprefix = am.Prefix;
                    break;
                }

            string groupcode = string.Empty;
            foreach (AssetGroupModel am in AssetGroups)
                if (am.ID == groupid)
                {
                    groupcode = am.AssetGroupIDText;
                    break;
                }

            return  areaprefix + "-" + groupcode + "-" + labelid.ToString("000");
        }
    }
}
