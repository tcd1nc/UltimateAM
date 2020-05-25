using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace AssetManager.Behaviours
{
    public static class ItemsControlHelper
    {
        public static readonly DependencyProperty ScrollToLastItemProperty =  DependencyProperty.RegisterAttached("ScrollToLastItem",
                typeof(bool), typeof(ItemsControlHelper), new FrameworkPropertyMetadata(false, OnScrollToLastItemChanged));

        public static void SetScrollToLastItem(UIElement sender, bool value)
        {
            sender.SetValue(ScrollToLastItemProperty, value);
        }

        public static bool GetScrollToLastItem(UIElement sender)
        {
            return (bool)sender.GetValue(ScrollToLastItemProperty);
        }

        private static void OnScrollToLastItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                itemsControl.ItemContainerGenerator.StatusChanged += (s, a) => OnItemsChanged(itemsControl, s, a);
            }
        }

        static void OnItemsChanged(ItemsControl itemsControl, object sender, EventArgs e)
        {
            var generator = sender as ItemContainerGenerator;
            if (generator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (itemsControl.Items.Count > 0)
                {
                    ScrollIntoView(itemsControl, itemsControl.Items[itemsControl.Items.Count - 1]);
                }
            }
        }

        private static void ScrollIntoView(ItemsControl itemsControl, object item)
        {
            if (itemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                OnBringItemIntoView(itemsControl, item);
            }
            else
            {
                Func<object, object> onBringIntoView = (o) => OnBringItemIntoView(itemsControl, item);
                itemsControl.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                      new DispatcherOperationCallback(onBringIntoView));
            }
        }

        private static object OnBringItemIntoView(ItemsControl itemsControl, object item)
        {
            if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement element)
            {
                element.BringIntoView();
            }
            return null;
        }
    }


    public static class LBScrollHelper
    {
        public static readonly DependencyProperty ScrollToLastItemProperty = DependencyProperty.RegisterAttached("ScrollToLastItem",
                typeof(bool), typeof(LBScrollHelper), new FrameworkPropertyMetadata(false, OnScrollToLastItemChanged));

        public static void SetScrollToLastItem(UIElement sender, bool value)
        {
            sender.SetValue(ScrollToLastItemProperty, value);
        }

        public static bool GetScrollToLastItem(UIElement sender)
        {
            return (bool)sender.GetValue(ScrollToLastItemProperty);
        }

        private static void OnScrollToLastItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                MoveToLastItem(itemsControl);
            }
        }

        static void MoveToLastItem(ItemsControl itemsControl)
        {
            (itemsControl as ListBox).SelectedItem = (itemsControl as ListBox).Items[itemsControl.Items.Count - 1];
            (itemsControl as ListBox).SelectedIndex = itemsControl.Items.Count - 1;
            (itemsControl as ListBox).ScrollIntoView(itemsControl.Items[itemsControl.Items.Count - 1]);
        }


        public static readonly DependencyProperty ScrollToSelectedItemProperty = DependencyProperty.RegisterAttached("ScrollToSelectedItem",
                typeof(int), typeof(LBScrollHelper), new FrameworkPropertyMetadata(-1, OnScrollToSelectedItemChanged));

        public static void SetScrollToSelectedItem(UIElement sender, int value)
        {
            sender.SetValue(ScrollToSelectedItemProperty, value);
        }

        public static int GetScrollToSelectedItem(UIElement sender)
        {
            return (int)sender.GetValue(ScrollToSelectedItemProperty);
        }

        private static void OnScrollToSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
            {
                if (itemsControl.Items.Count > 0)
                    MoveToSelectedItem(itemsControl, (int)e.NewValue);
            }
        }

        static void MoveToSelectedItem(ItemsControl itemsControl, int _index)
        {
            (itemsControl as ListBox).SelectedItem = (itemsControl as ListBox).Items[_index];
            (itemsControl as ListBox).SelectedIndex = _index;
            (itemsControl as ListBox).ScrollIntoView(itemsControl.Items[_index]);
          
        }

    }

    public static class GridScroll
    {
        public static readonly DependencyProperty SelectingItemProperty = DependencyProperty.RegisterAttached(
            "SelectingItem",
            typeof(int),
            typeof(GridScroll),
            new PropertyMetadata(default(int), OnSelectingItemChanged));

        public static int GetSelectingItem(DependencyObject target)
        {
            return (int)target.GetValue(SelectingItemProperty);
        }

        public static void SetSelectingItem(DependencyObject target, int value)
        {
            target.SetValue(SelectingItemProperty, value);
        }

        static void OnSelectingItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid == null)
                return;

            // Works with .Net 4.5
            int i = (int)e.NewValue;
            grid.Dispatcher.InvokeAsync(() =>
            {
                grid.UpdateLayout();
                if (grid.Items.Count > 0 && i > 0)
                    grid.ScrollIntoView(grid.Items[i], null);
            });

        }
    }
}
