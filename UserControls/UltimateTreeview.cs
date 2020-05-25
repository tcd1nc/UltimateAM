using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AssetManager.UserControls
{

    public class TreeViewItemViewModel : ViewModelBase
    {
        readonly ObservableCollection<TreeViewItemViewModel> _children;
        TreeViewItemViewModel _parent;
        bool _isExpanded;
        bool _isSelected;
        bool _isFiltered;

        protected TreeViewItemViewModel(TreeViewItemViewModel parent, bool lazyLoadChildren)
        {
            _parent = parent;
            _children = new ObservableCollection<TreeViewItemViewModel>();
        }

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

        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return _children; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { SetField(ref _isExpanded, value);
                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetField(ref _isSelected, value); }
        }

        public bool IsFiltered
        {
            get { return _isFiltered; }
            set { SetField(ref _isFiltered, value); }
        }

        public TreeViewItemViewModel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

    }


    public class UltimateTreeView : TreeView
    {
        private RoutedEventHandler Checked_EventHandler;
        private RoutedEventHandler Unchecked_EventHandler;

        public UltimateTreeView() : base()
        {
            Checked_EventHandler = new RoutedEventHandler(CheckBoxTreeViewItem_Checked);
            Unchecked_EventHandler = new RoutedEventHandler(CheckBoxTreeViewItem_Unchecked);

            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(ItemsSourceProperty, typeof(UltimateTreeView));
            if (dpd != null)
            {
                dpd.AddValueChanged(this, ItemsSourceChanged);
            }
        }

        public static DependencyProperty CheckedItemsProperty = DependencyProperty.Register("CheckedItems", typeof(ObservableCollection<TreeViewItemViewModel>), typeof(UltimateTreeView));
            
        public static DependencyProperty ScrollItemIntoViewProperty = DependencyProperty.Register("ScrollItemIntoView", typeof(bool), 
            typeof(UltimateTreeView), new PropertyMetadata(false, ScrollItemIntoViewChanged));

        public bool ScrollItemIntoView
        {
            get { return (bool)base.GetValue(ScrollItemIntoViewProperty); }
            set { base.SetValue(ScrollItemIntoViewProperty, value); }
        }
        
        private static void ScrollItemIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UltimateTreeView)d).ScrollItemIntoView = (bool)e.NewValue;
        }

        private void ItemsSourceChanged(object sender, EventArgs e)
        {                           
            CheckedItems = new ObservableCollection<TreeViewItemViewModel>();
        }

        internal void OnNewContainer(TViewItem newContainer)
        {
            newContainer.Checked -= Checked_EventHandler;
            newContainer.Unchecked -= Unchecked_EventHandler;
            newContainer.Checked += Checked_EventHandler;
            newContainer.Unchecked += Unchecked_EventHandler;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            TViewItem checkBoxTreeViewItem = new TViewItem();
            OnNewContainer(checkBoxTreeViewItem);
            return checkBoxTreeViewItem;
        }

        private void CheckBoxTreeViewItem_Checked(object sender, RoutedEventArgs e)
        {
            TViewItem checkBoxTreeViewItem = sender as TViewItem;
            Action action = () =>
            {
                if(!CheckedItems.Contains((TreeViewItemViewModel)checkBoxTreeViewItem.Header))
                    CheckedItems.Add((TreeViewItemViewModel)checkBoxTreeViewItem.Header);
            };
            this.Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
        }

        private void CheckBoxTreeViewItem_Unchecked(object sender, RoutedEventArgs e)
        {
            TViewItem checkBoxTreeViewItem = sender as TViewItem;
            Action action = () =>
            {
                CheckedItems.Remove((TreeViewItemViewModel)checkBoxTreeViewItem.Header);
            };
            this.Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
        }

        public ObservableCollection<TreeViewItemViewModel> CheckedItems
        {
            get { return (ObservableCollection<TreeViewItemViewModel>)base.GetValue(CheckedItemsProperty); }
            set { base.SetValue(CheckedItemsProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        //{
        //    TreeViewItem item = e.Source as TreeViewItem;
        //    if (item != null)
        //    {
        //        item.BringIntoView();
        //        e.Handled = true;
        //    }
        //   // base.OnSelectedItemChanged(e);
        //}

    }


    public class TViewItem : TreeViewItem
    {
        public TViewItem() : base()
        {
        }
            
        public static readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent("Checked",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(TViewItem));

        public static readonly RoutedEvent UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(TViewItem));

        public static readonly RoutedEvent IndeterminateCheckedEvent = EventManager.RegisterRoutedEvent("IndeterminateChecked",
            RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(TViewItem));

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(TViewItem),
                            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CheckedPropertyChanged));

        public bool? IsChecked
        {
            get { return (bool?)base.GetValue(IsCheckedProperty); }
            set { base.SetValue(IsCheckedProperty, value); }
        }

        private static void CheckedPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            TViewItem checkBoxTreeViewItem = (TViewItem)source;
            if (checkBoxTreeViewItem.IsChecked == true)
                checkBoxTreeViewItem.OnChecked(new RoutedEventArgs(CheckedEvent, checkBoxTreeViewItem));
            else
            if (checkBoxTreeViewItem.IsChecked == false)
                checkBoxTreeViewItem.OnUnchecked(new RoutedEventArgs(UncheckedEvent, checkBoxTreeViewItem));
            else
                checkBoxTreeViewItem.OnIndeterminateChecked(new RoutedEventArgs(IndeterminateCheckedEvent, checkBoxTreeViewItem));

            SetParentCheckedState((TreeViewItemViewModel)checkBoxTreeViewItem.Header);
            SetChildrenCheckedState((TreeViewItemViewModel)checkBoxTreeViewItem.Header);
        }

        private static ItemsControl GetSelectedTreeViewItemParent(TViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TViewItem) && !(parent is UltimateTreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent is UltimateTreeView)
                return null;

            return parent as ItemsControl;
        }      

        private static Collection<TViewItem> GetAllItemContainers(ItemsControl itemsControl)
        {
            Collection<TViewItem> allItems = new Collection<TViewItem>();
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                // try to get the item Container   
                TViewItem childItemContainer = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TViewItem;
                // the item container maybe null if it is still not generated from the runtime   
                if (childItemContainer != null)
                {
                    allItems.Add(childItemContainer);
                }
            }
            return allItems;
        }

        private static void SetChildCheckedState(TreeViewItemViewModel itemsControl, bool checkedstate)
        {
            itemsControl.IsChecked = checkedstate;           
            for (int i = 0; i < itemsControl.Children.Count; i++)
            {                           
                TreeViewItemViewModel tvi = itemsControl.Children[i]; 
                 
                if (tvi != null)
                {                          
                    tvi.IsChecked = checkedstate;
                    SetChildCheckedState(tvi, checkedstate);
                }  
            }                     
        }
        
        //private static T FindVisualChild<T>(Visual visual) where T : Visual
        //{
        //    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
        //    {
        //        Visual child = (Visual)VisualTreeHelper.GetChild(visual, i);
        //        if (child != null)
        //        {
        //            T correctlyTyped = child as T;
        //            if (correctlyTyped != null)
        //            {
        //                return correctlyTyped;
        //            }

        //            T descendent = FindVisualChild<T>(child);
        //            if (descendent != null)
        //            {
        //                return descendent;
        //            }
        //        }
        //    }

        //    return null;
        //}
             
        //--------------------------------------------------------------------------
        private static void SetParentCheckedState(TreeViewItemViewModel tvitem)
        {
            if (tvitem != null)
            {
                TreeViewItemViewModel parent = tvitem.Parent;
               
                if (!(parent is null) && parent.CheckBoxVisibility == Visibility.Visible)
                {
                    Collection<TreeViewItemViewModel> items = parent.Children;
                    int cntindeterminate = 0;
                    int cntunchecked = 0;
                    int cntchecked = 0;

                    foreach (var i in items)
                    {
                        if (i.IsChecked == true)
                            cntchecked++;
                        else
                        if (i.IsChecked == false)
                            cntunchecked++;
                        else
                        if (i.IsChecked == null)
                            cntindeterminate++;
                    }

                    if (cntindeterminate > 0 || (cntunchecked > 0 && cntunchecked < items.Count))
                        parent.IsChecked = null;
                    else
                        if (cntchecked == items.Count)
                        parent.IsChecked = true;
                    else
                        if (cntunchecked == items.Count)
                        parent.IsChecked = false;

                }
            }
        }

        public static void SetChildrenCheckedState(TreeViewItemViewModel tvitem)
        {
            if (tvitem!= null && (tvitem.IsChecked == true || tvitem.IsChecked == false))
            {
                SetChildCheckedState(tvitem, (bool)tvitem.IsChecked);
            }
        }

        public static readonly DependencyProperty CheckBoxVisibilityProperty = DependencyProperty.Register("CheckBoxVisibility", typeof(Visibility), typeof(TViewItem),
               new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CheckBoxVisibilityPropertyChanged));

        public Visibility CheckBoxVisibility
        {
            get { return (Visibility)base.GetValue(CheckBoxVisibilityProperty); }
            set { base.SetValue(CheckBoxVisibilityProperty, value); }
        }

        private static void CheckBoxVisibilityPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            TViewItem checkBoxTreeViewItem = (TViewItem)source;
            checkBoxTreeViewItem.CheckBoxVisibility = (Visibility)(e.NewValue);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            PropertyInfo parentTreeViewPi = typeof(TViewItem).GetProperty("ParentTreeView", BindingFlags.Instance | BindingFlags.NonPublic);
            UltimateTreeView parentCheckBoxTreeView = parentTreeViewPi.GetValue(this, null) as UltimateTreeView;
            TViewItem checkBoxTreeViewItem = new TViewItem();
            parentCheckBoxTreeView.OnNewContainer(checkBoxTreeViewItem);
            return checkBoxTreeViewItem;
        }

        //protected override void OnSelected(RoutedEventArgs e)
        //{
        //    // Only react to the Selected event raised by the TreeViewItem
        //    // whose IsSelected property was modified. Ignore all ancestors
        //    // who are merely reporting that a descendant's Selected fired.
        //    if (!Object.ReferenceEquals(this, e.OriginalSource))
        //        return;
        //    if (e.OriginalSource is TViewItem item)
        //        item.BringIntoView();
        //    //e.Handled = true;

        //    base.OnSelected(e);
        //}

        //protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        //{
        //    base.OnMouseDoubleClick(e);
        //}

        //protected override void OnDragEnter(DragEventArgs e)
        //{
        //    base.OnDragEnter(e);
        //}

        //protected override void OnDragOver(DragEventArgs e)
        //{
        //    base.OnDragOver(e);
        //}

        //protected override void OnDragLeave(DragEventArgs e)
        //{
        //    base.OnDragLeave(e);
        //}

        //protected override void OnDrop(DragEventArgs e)
        //{
        //    base.OnDrop(e);
        //}

        public event RoutedEventHandler Checked
        {
            add
            {
                AddHandler(CheckedEvent, value);
            }
            remove
            {
                RemoveHandler(CheckedEvent, value);
            }
        }
        public event RoutedEventHandler Unchecked
        {
            add
            {
                AddHandler(UncheckedEvent, value);
            }
            remove
            {
                RemoveHandler(UncheckedEvent, value);
            }
        }
        public event RoutedEventHandler IndeterminateChecked
        {
            add
            {
                AddHandler(IndeterminateCheckedEvent, value);
            }
            remove
            {
                RemoveHandler(IndeterminateCheckedEvent, value);
            }
        }

        protected virtual void OnChecked(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }
        protected virtual void OnUnchecked(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }
        protected virtual void OnIndeterminateChecked(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

    }
}