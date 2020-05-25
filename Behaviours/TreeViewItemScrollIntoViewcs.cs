using System;
using System.Windows;
using System.Windows.Controls;

namespace AssetManager.Behaviours
{
    public class BringSelectedItemIntoViewBehavior
    {
        public static readonly DependencyProperty IsBringSelectedIntoViewProperty = DependencyProperty.RegisterAttached(
            "IsBringSelectedIntoView", typeof(bool), typeof(BringSelectedItemIntoViewBehavior), new PropertyMetadata(default(bool), PropertyChangedCallback));

        public static void SetIsBringSelectedIntoView(DependencyObject element, bool value)
        {
            element.SetValue(IsBringSelectedIntoViewProperty, value);
        }

        public static bool GetIsBringSelectedIntoView(DependencyObject element)
        {
            return (bool)element.GetValue(IsBringSelectedIntoViewProperty);
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is TreeViewItem treeViewItem))
            {
                return;
            }

            if (!((bool)dependencyPropertyChangedEventArgs.OldValue) &&
                ((bool)dependencyPropertyChangedEventArgs.NewValue))
            {
                treeViewItem.Unloaded += TreeViewItemOnUnloaded;
                treeViewItem.Selected += TreeViewItemOnSelected;
            }
        }

        private static void TreeViewItemOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!(sender is TreeViewItem treeViewItem))
            {
                return;
            }

            treeViewItem.Unloaded -= TreeViewItemOnUnloaded;
            treeViewItem.Selected -= TreeViewItemOnSelected;
        }

        private static void TreeViewItemOnSelected(object sender, RoutedEventArgs routedEventArgs)
        {
            var treeViewItem = sender as TreeViewItem;
            treeViewItem.BringIntoView();
        }
    }

    public static class TreeViewItemBehavior
    {
        #region IsBroughtIntoViewWhenSelected
        public static bool GetIsBroughtIntoViewWhenSelected(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(IsBroughtIntoViewWhenSelectedProperty);
        }
        public static void SetIsBroughtIntoViewWhenSelected(TreeViewItem treeViewItem, bool value)
        {
            treeViewItem.SetValue(IsBroughtIntoViewWhenSelectedProperty, value);
        }

        public static readonly DependencyProperty IsBroughtIntoViewWhenSelectedProperty =
            DependencyProperty.RegisterAttached("IsBroughtIntoViewWhenSelected", typeof(bool), typeof(TreeViewItemBehavior), new UIPropertyMetadata(false, OnIsBroughtIntoViewWhenSelectedChanged));

        static void OnIsBroughtIntoViewWhenSelectedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is TreeViewItem item))
                return;
            if (e.NewValue is bool == false)
                return;
            if ((bool)e.NewValue)
                item.Selected += OnTreeViewItemSelected;
            else
                item.Selected -= OnTreeViewItemSelected;
        }

        static void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            // Only react to the Selected event raised by the TreeViewItem
            // whose IsSelected property was modified. Ignore all ancestors
            // who are merely reporting that a descendant's Selected fired.
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
                return;
            if (e.OriginalSource is TreeViewItem item)
                item.BringIntoView();
            //item.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new DispatcherOperationCallback(ScrollItemIntoView), item);

        }
        
        static object ScrollItemIntoView(object sender)
        {
            if (sender is TreeViewItem _item)
                _item.BringIntoView();
            return null;
        }
        #endregion // IsBroughtIntoViewWhenSelected
    }

}
