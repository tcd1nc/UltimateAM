using System.Windows.Input;

namespace AssetManager.ViewModels
{
    public class ObjectCRUDViewModel : ViewModelBase
    {
       public bool _canexecutesave = true;
       public bool _canexecutedelete = true;
       public bool _canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }
        public ICommand Delete { get; set; }

        int _scrolltoselecteditem;
        public int ScrollToSelectedItem
        {
            get { return _scrolltoselecteditem; }
            set { SetField(ref _scrolltoselecteditem, value); }
        }

        private bool _isduplicatename;
        public bool DuplicateName
        {
            get { return _isduplicatename; }
            set { SetField(ref _isduplicatename, value); }
        }

        string _selectedlabel;
        public string SelectedLabel
        {
            get { return _selectedlabel; }
            set { SetField(ref _selectedlabel, value); }
        }

    }
}
