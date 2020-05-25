using AssetManager.ViewModels;
using System;
using System.Windows;


namespace AssetManager
{
    public interface IMessageBoxService
    {
        GenericMessageBoxResult Show(string message, string caption, GenericMessageBoxButton buttons);
        bool ShowMessage(string text, string caption, GenericMessageBoxButton messageType);
        void Show(string message, string caption);
        string OpenFileDlg(string _title, bool _validatenames, bool _multiselect, string _filter, string _initialdirectory, Window _owner);
        string SaveFileDlg(string _title, string _filter, string suggestedfilename, Window _owner);
        GenericMessageBoxResult ShowMessage(string text, string caption, GenericMessageBoxButton messagebutton, GenericMessageBoxIcon messageicon);
        Models.AssetModel OpenAssetDlg(int customerid, int parentid);
        bool OpenAssetDlg(AssetTreeExViewModel tva, TVAssetViewModel obj);
        bool OpenDialog(string _name);
        bool OpenParameterizedDialog(string _name, object[] parameter);
        int OpenDefaultCustomersDlg();
        bool OpenAssetMovementsDlg(int _id);
        bool OpenCustomersDlg();
        DateTime? OpenAssetAuditsDlg(int _id);
        object OpenCustomerDlg(int id);
        string SelectFolderDlg();
    }

    public enum GenericMessageBoxButton
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum GenericMessageBoxResult
    {
        OK,
        Cancel,
        No,
        Yes
    }

    public enum GenericMessageBoxIcon
    {
        Asterisk,
        Error,
        Exclamation,
        Hand,
        Information,
        None,
        Question,
        Stop,
        Warning
    }

    
    public class MessageBoxService : IMessageBoxService
    {

        public Models.AssetModel OpenAssetDlg(int customerid, int parentid)
        {
            Window _owner;
            _owner = Application.Current.Windows[0];
            Models.AssetModel result;

            Views.AssetView dlg = new Views.AssetView(customerid, parentid)
            {
                Owner = _owner
            };
            dlg.ShowDialog();
            result =(Models.AssetModel)dlg.Tag;
            return result;
        }

        public bool OpenAssetDlg(AssetTreeExViewModel tva, TVAssetViewModel obj)
        {
            Window owner;
            owner = Application.Current.Windows[0];
            Views.AssetView dlg = new Views.AssetView(tva, obj)
            {
                Owner = owner
            };
            dlg.Show();
            return false;
        }
                
        public bool OpenAssetMovementsDlg(int _id)
        {
            Window _owner;
            _owner = Application.Current.Windows[0];
            bool result = false;

            if (_id > 0) 
            {
                Views.AssetMovementsView dlg = new Views.AssetMovementsView(_id)
                {
                    Owner = _owner
                };
                dlg.ShowDialog();

                result = (bool) dlg.DialogResult;
            }
            return result;
        }

        public object OpenCustomerDlg(int id)
        {
            Window _owner;
            _owner = Application.Current.Windows[0];
            Views.EditCustomerView dlg = new Views.EditCustomerView(id)
            {
                Owner = _owner
            };
            dlg.ShowDialog();
            return dlg.Tag;
        }


        public bool OpenCustomersDlg()
        {
            Window _owner;
            _owner = Application.Current.Windows[0];

            Views.EditCustomersView dlg = new Views.EditCustomersView()
            {
                Owner = _owner
            };
            dlg.ShowDialog();

            bool result = (bool)dlg.DialogResult;

            return result;
        }


        public int OpenDefaultCustomersDlg()
        {
            Window _owner;
            _owner = Application.Current.Windows[0];

            Views.DefaultCustomerView dlg = new Views.DefaultCustomerView
            {
                Owner = _owner
            };
            dlg.ShowDialog();

           int result =(int)dlg.Tag;                
           return result;
        }
        
        public DateTime? OpenAssetAuditsDlg(int _id)
        {
            Window _owner;
            _owner = Application.Current.Windows[0];
            DateTime? result = null;

            if (_id > 0)
            {
                Views.ScheduledAuditsView dlg = new Views.ScheduledAuditsView(_id)
                {
                    Owner = _owner
                };
                dlg.ShowDialog();

                result = (DateTime?)dlg.Tag;
            }
            return result;
        }


        public bool OpenDialog(string _name)
        {
            Window _owner;
            _owner = Application.Current.Windows[0];
      
            switch (_name)
            {                
                case "ScheduledActivities":
                    Views.ScheduledActivityAlerterView schedalerter = new Views.ScheduledActivityAlerterView
                    {
                        Owner = _owner
                    };
                    schedalerter.Show();
                    return false;

                case "SalesDivisions":
                    Views.EditSalesDivisionView eav = new Views.EditSalesDivisionView
                    {
                        Owner = _owner
                    };
                    eav.ShowDialog();
                    return (bool)eav.DialogResult;
                                
                case "Countries":
                    Views.EditCountryView ecv = new Views.EditCountryView
                    {
                        Owner = _owner
                    };
                    ecv.ShowDialog();
                    return (bool)ecv.DialogResult;
                    
                case "Corporations":
                    Views.EditCorporation corps = new Views.EditCorporation
                    {
                        Owner = _owner
                    };
                    corps.ShowDialog();
                    return (bool)corps.DialogResult;
                    
                case "AssetAreas":
                    Views.EditAssetAreaView assetareas = new Views.EditAssetAreaView
                    {
                        Owner = _owner
                    };
                    assetareas.ShowDialog();
                    return (bool)assetareas.DialogResult;
                    
                case "AssetTypes":
                    Views.EditAssetType assettypes = new Views.EditAssetType
                    {
                        Owner = _owner
                    };
                    assettypes.ShowDialog();
                    return (bool)assettypes.DialogResult;
                    
                case "AssetGroups":
                    Views.EditAssetGroup assetgroups = new Views.EditAssetGroup
                    {
                        Owner = _owner
                    };
                    assetgroups.ShowDialog();
                    return (bool)assetgroups.DialogResult;
                    
                case "AssetSpecifications":
                    Views.EditAssetSpecificationView specs = new Views.EditAssetSpecificationView
                    {
                        Owner = _owner
                    };
                    specs.ShowDialog();
                    return (bool)specs.DialogResult;
                    
                case "Users":
                    Views.AdministratorView users = new Views.AdministratorView
                    {
                        Owner = _owner
                    };
                    users.ShowDialog();
                    return (bool)users.DialogResult;
                    
                case "OperatingCompanies":
                    Views.EditOperatingCompanyView opcos = new Views.EditOperatingCompanyView
                    {
                        Owner = _owner
                    };
                    opcos.ShowDialog();
                    return (bool)opcos.DialogResult;
                    
                case "DeletedAssets":
                    Views.DeletedAssetsView deletedassets = new Views.DeletedAssetsView
                    {
                        Owner = _owner
                    };
                    deletedassets.ShowDialog();
                    return (bool)deletedassets.DialogResult;
                    
                case "About":
                    Views.AboutView about = new Views.AboutView
                    {
                        Owner = _owner
                    };
                    about.ShowDialog();
                    return (bool)about.DialogResult;
                                    
                case "DefaultCustomer":
                    Views.DefaultCustomerView defcustomer = new Views.DefaultCustomerView
                    {
                        Owner = _owner
                    };
                    defcustomer.ShowDialog();
                    return (bool)defcustomer.DialogResult;
                                       
                case "GroupSpecifications":
                    Views.GroupSpecificationsView groupspecs = new Views.GroupSpecificationsView
                    {
                        Owner = _owner
                    };
                    groupspecs.ShowDialog();
                    return (bool)groupspecs.DialogResult;
                    
                case "SpecificationProperties":
                    Views.EditSpecificationPropertyView specprops = new Views.EditSpecificationPropertyView
                    {
                        Owner = _owner
                    };
                    specprops.ShowDialog();
                    return (bool)specprops.DialogResult;
                    
                case "UserSettings":
                    Views.UserSettingsView userset = new Views.UserSettingsView
                    {
                        Owner = _owner
                    };
                    userset.ShowDialog();
                    return (bool)userset.DialogResult;
                   
                case "Reports":
                    Views.CustomReportsView custrepts = new Views.CustomReportsView
                    {
                        Owner = _owner
                    };
                    custrepts.ShowDialog();
                    return false;                    

                default:
                    return false;
            }
            
        }

        public bool OpenParameterizedDialog(string _name, object[] parameter)
        {
            Window _owner;
            _owner = Application.Current.Windows[0];
            //bool result;

            switch (_name)
            {
                //case "AvailableAssets":
                //    Views.AvailableAssetsView availassets = new Views.AvailableAssetsView(parameter);
                //    availassets.Owner = _owner;
                //    availassets.ShowDialog();
                //    result = (bool)availassets.DialogResult;
                //    return result;

                default:
                    return false;
            }
           
        }

        public string OpenFileDlg(string _title, bool _validatenames, bool _multiselect, string _filter, string _initialdirectory, Window _owner)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = _filter, 
                Title = _title,
                ValidateNames = _validatenames,
                Multiselect = _multiselect,
                InitialDirectory = _initialdirectory ?? string.Empty
            };
            if (_owner == null)
                dlg.ShowDialog();
            else
                dlg.ShowDialog(_owner);
            return (dlg.FileName != null) ? dlg.FileName : string.Empty;
        }

        public string SaveFileDlg(string _title, string _filter, string suggestedfilename, Window _owner)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = _filter,
                Title = _title,
                ValidateNames = true,
                FileName = suggestedfilename
            };

            if (_owner == null)
                dlg.ShowDialog();
            else
                dlg.ShowDialog(_owner);
            string filename = dlg.FileName ?? string.Empty;
            return filename;
        }

        public string SelectFolderDlg()
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = false;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return fbd.SelectedPath;                    
                }
            }
            return string.Empty;
        }


        public GenericMessageBoxResult Show(string message, string caption, GenericMessageBoxButton buttons)
        {
            //   var slButtons = buttons == GenericMessageBoxButton.Ok
            //                    ? MessageBoxButton.OK
            //                   : MessageBoxButton.OKCancel;


            var slButtons = MessageBoxButton.OK;
            switch (buttons)
            {
                case GenericMessageBoxButton.OK:
                    slButtons = MessageBoxButton.OK;
                    break;
                case GenericMessageBoxButton.OKCancel:
                    slButtons = MessageBoxButton.OKCancel;
                    break;
                case GenericMessageBoxButton.YesNo:
                    slButtons = MessageBoxButton.YesNo;
                    break;
                case GenericMessageBoxButton.YesNoCancel:
                    slButtons = MessageBoxButton.YesNoCancel;
                    break;
                default:
                    break;
            }
            
            var result = System.Windows.MessageBox.Show(message, caption, slButtons);
            var returnedResult = GenericMessageBoxResult.OK;
            switch (result)
            {
                case MessageBoxResult.OK:
                    returnedResult = GenericMessageBoxResult.OK;
                    break;
                case MessageBoxResult.Cancel:
                    returnedResult = GenericMessageBoxResult.Cancel;
                    break;
                case MessageBoxResult.Yes:
                    returnedResult = GenericMessageBoxResult.Yes;
                    break;
                case MessageBoxResult.No:
                    returnedResult = GenericMessageBoxResult.No;
                    break;
                default:
                    break;

            }

            return returnedResult;
        }

        public void Show(string message, string caption)
        {
            System.Windows.MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        public bool ShowMessage(string text, string caption, GenericMessageBoxButton messageType)
        {
           return System.Windows.MessageBox.Show(text, caption, MessageBoxButton.OK) == MessageBoxResult.OK;
        }

        public GenericMessageBoxResult ShowMessage(string text, string caption, GenericMessageBoxButton messagebutton, GenericMessageBoxIcon messageicon)
        {
            var slIcon = MessageBoxImage.None;
            switch (messageicon)
            {
                case GenericMessageBoxIcon.Asterisk:
                    slIcon = MessageBoxImage.Asterisk;
                    break;
                case GenericMessageBoxIcon.Error:
                    slIcon = MessageBoxImage.Error;
                    break;
                case GenericMessageBoxIcon.Exclamation:
                    slIcon = MessageBoxImage.Exclamation;
                    break;
                case GenericMessageBoxIcon.Hand:
                    slIcon = MessageBoxImage.Hand;
                    break;
                case GenericMessageBoxIcon.Information:
                    slIcon = MessageBoxImage.Information;
                    break;
                case GenericMessageBoxIcon.None:
                    slIcon = MessageBoxImage.None;
                    break;
                case GenericMessageBoxIcon.Question:
                    slIcon = MessageBoxImage.Question;
                    break;
                case GenericMessageBoxIcon.Stop:
                    slIcon = MessageBoxImage.Stop;
                    break;
                case GenericMessageBoxIcon.Warning:
                    slIcon = MessageBoxImage.Warning;
                    break;

                default:
                    break;
            }

            var slButtons = MessageBoxButton.OK;
            switch (messagebutton)
            {
                case GenericMessageBoxButton.OK:
                    slButtons = MessageBoxButton.OK;
                    break;
                case GenericMessageBoxButton.OKCancel:
                    slButtons = MessageBoxButton.OKCancel;
                    break;
                case GenericMessageBoxButton.YesNo:
                    slButtons = MessageBoxButton.YesNo;
                    break;
                case GenericMessageBoxButton.YesNoCancel:
                    slButtons = MessageBoxButton.YesNoCancel;
                    break;
                default:
                    break;
            }

            var result = System.Windows.MessageBox.Show(text, caption, slButtons, slIcon);
            var returnedResult = GenericMessageBoxResult.OK;
            switch (result)
            {
                case MessageBoxResult.OK:
                    returnedResult = GenericMessageBoxResult.OK;
                    break;
                case MessageBoxResult.Cancel:
                    returnedResult = GenericMessageBoxResult.Cancel;
                    break;
                case MessageBoxResult.Yes:
                    returnedResult = GenericMessageBoxResult.Yes;
                    break;
                case MessageBoxResult.No:
                    returnedResult = GenericMessageBoxResult.No;
                    break;
                default:
                    break;
            }
            return returnedResult;
        }

    }

}
