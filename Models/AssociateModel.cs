
using System.ComponentModel;

namespace AssetManager.Models
{
    public class AssociateModel :ViewModelBase, IDataErrorInfo
    {
        int _id;
        public int ID
        {
            get { return _id; }
            set { SetField(ref _id, value); }
        }

        string _associatename;
        public string AssociateName {
            get { return _associatename; }
            set { SetField(ref _associatename, value); }
        }

        string _loginname;
        public string LoginName {
            get { return _loginname; }
            set { SetField(ref _loginname, value); }
        }

        bool _ismanager;
        public bool Manager
        {
            get { return _ismanager; }
            set { SetField(ref _ismanager, value); }
        }

        public string Error
        {
            get
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("Error in Associate Model", "Error in Associate Model", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                _msg = null;
                return "Error in Associate";

                //throw new NotImplementedException();
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                switch (columnName)
                {
                    case "AssociateName":
                        if (string.IsNullOrEmpty(AssociateName))
                            result = "Name is required!";

            //            foreach (Models.AssociateModel am in StaticCollections.Associates)
            //            {
             //               if (ID != am.ID && am.AssociateName == AssociateName)
            //                    result = "Name is not unique";
             //           }
                        break;

                };
                return result;
            }

        }
    
    }
}
