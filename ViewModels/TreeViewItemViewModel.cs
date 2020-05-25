using System.Collections.ObjectModel;
using System.Windows;

namespace AssetManager.ViewModels
{
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class ZTreeViewItemViewModel : ViewModelBase
    {      
        //static readonly TreeViewItemViewModel DummyChild = new TreeViewItemViewModel();
        readonly ObservableCollection<ZTreeViewItemViewModel> _children;
        ZTreeViewItemViewModel _parent;
        bool _isExpanded;
        bool _isSelected;
        bool _isFiltered;
   
        protected ZTreeViewItemViewModel(ZTreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;
            _children = new ObservableCollection<ZTreeViewItemViewModel>();
        }

        // This is used to create the DummyChild instance.
        //private TreeViewItemViewModel()
        //{
        //}

        Visibility isvisible;
        public Visibility Visibility
        {
            get { return isvisible; }
            set { SetField(ref isvisible, value); }
        }

        Visibility chkboxvisibility = Visibility.Visible;
        public Visibility CheckBoxVisibility
        {
            get { return chkboxvisibility; }
            set { SetField(ref chkboxvisibility, value); }
        }

        bool? chkboxchecked = false;
        public bool? IsChecked
        {
            get { return chkboxchecked; }
            set { SetField(ref chkboxchecked, value); }
        }
                    
        public ObservableCollection<ZTreeViewItemViewModel> Children
        {
            get { return _children; }
        }
               
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                //if (value != _isExpanded)
                //{
                    SetField(ref _isExpanded, value);
                //}

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
               
            }
        }             
      
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                //if (value != _isSelected)
                //{
                    SetField(ref _isSelected, value);
                //}
            }
        }
              
        public bool IsFiltered
        {
            get { return _isFiltered; }
            set{
                if (value != _isFiltered)
                {
                    SetField(ref _isFiltered, value);
                }
            }
        }           

        public ZTreeViewItemViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

    }
}
