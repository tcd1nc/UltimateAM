using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using AssetManager.ViewModels;
using System.Windows.Threading;

namespace AssetManager.Behaviours
{
    /// <summary>
    /// Manages the dragging and dropping of treeViewItems in a treeView.
    /// The TreeViewItemViewModel parameter indicates the type of the objects in
    /// the treeView's items source.  The treeView's ItemsSource must be 
    /// set to an instance of ObservableCollection of TreeViewItemViewModel, or an 
    /// Exception will be thrown.
    /// </summary>
    /// <typeparam name="TreeViewItemViewModel">The type of the treeView's items.</typeparam>
    public  class TVDragDrop
    {

        public static DependencyProperty UseDnDProperty = DependencyProperty.RegisterAttached("UseDnD", typeof(bool),
           typeof(TVDragDrop), new UIPropertyMetadata(UseDnDChanged));
        
        public static DependencyProperty TVIClickProperty = DependencyProperty.RegisterAttached("TVIClick", typeof(ICommand),
           typeof(TVDragDrop), new UIPropertyMetadata(TVItemClicked));

        public static DependencyProperty TVIParameterProperty = DependencyProperty.RegisterAttached("TVIParameter", typeof(object), typeof(TVDragDrop), new UIPropertyMetadata(null));


        public static void SetTVIClick(DependencyObject target, ICommand value)
        {
            target.SetValue(TVIClickProperty, value);
        }

        public static void SetTVIParameter(DependencyObject target, object value)
        {
            target.SetValue(TVIParameterProperty, value);
        }
        public static object GetTVIParameter(DependencyObject target)
        {
            return target.GetValue(TVIParameterProperty);
        }
        
        private static void TVItemClicked(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is TreeViewItem control)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.Selected += Control_Selected;
                    control.PreviewMouseDown += Control_PreviewMouseDown;
                    control.PreviewMouseMove += Control_PreviewMouseMove;
                    control.DragOver += Control_DragOver;
                    control.DragLeave += Control_DragLeave;                    
                    control.DragEnter += Control_DragEnter;
                    control.Drop += Control_Drop;
                }
                else
                    if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.Selected -= Control_Selected;
                    control.PreviewMouseDown -= Control_PreviewMouseDown;
                    control.PreviewMouseMove -= Control_PreviewMouseMove;
                    control.DragOver -= Control_DragOver;
                    control.DragLeave -= Control_DragLeave;
                    control.DragEnter -= Control_DragEnter;
                    control.Drop -= Control_Drop;
                }                
            }
        }

        private static void Control_Drop(object sender, DragEventArgs e)
        {
            if (ItemUnderDragCursor != null)
                ItemUnderDragCursor = null;

            e.Effects = DragDropEffects.None;

            if (!e.Data.GetDataPresent(typeof(TreeViewItemViewModel)))
                return;

            // Get the data object which was dropped.
            if (!(e.Data.GetData(typeof(TreeViewItemViewModel)) is TreeViewItemViewModel data))
                return;

            // Get the ObservableCollection<ItemType> which contains the dropped data object.
            ObservableCollection<TreeViewItemViewModel> itemsSource = ThisTreeView.ItemsSource as ObservableCollection<TreeViewItemViewModel>;
            if (itemsSource == null)
                throw new Exception(
                    "A treeView managed by treeViewDragManager must have its ItemsSource set to an ObservableCollection<TreeViewItemViewModel>.");

            int oldIndex = itemsSource.IndexOf(data);
            TreeViewItem newIndex = IndexUnderDragCursor;

            //if (newIndex == null)
            //{
            //    // The drag started somewhere else, and our treeView is empty
            //    // so make the new item the first in the list.
            //    if (itemsSource.Count == 0)
            //        newIndex = null;

            //    // The drag started somewhere else, but our treeView has items
            //    // so make the new item the last in the list.
            //    else if (oldIndex < 0)
            //        newIndex = itemsSource[itemsSource.Count-1];

            //    // The user is trying to drop an item from our treeView into
            //    // our treeView, but the mouse is not over an item, so don't
            //    // let them drop it.
            //    else
            //        return;
            //}

            //// Dropping an item back onto itself is not considered an actual 'drop'.
            //if (oldIndex == newIndex)
            //    return;

            //if (ProcessDrop != null)
            //{
            //    // Let the client code process the drop.
            //    TVProcessDropEventArgs<TreeViewItemViewModel> args = new TVProcessDropEventArgs<TreeViewItemViewModel>(itemsSource, data, oldIndex, newIndex, e.AllowedEffects);
            //    ProcessDrop(ThisTreeView, args);
            //    e.Effects = args.Effects;
            //}
            //else
            //{
            //    // Move the dragged data object from it's original index to the
            //    // new index (according to where the mouse cursor is).  If it was
            //    // not previously in the ListBox, then insert the item.
            //    if (oldIndex > -1)
            //        itemsSource.Move(oldIndex, newIndex);
            //    else
            //        itemsSource.Insert(newIndex, data);

            //    // Set the Effects property so that the call to DoDragDrop will return 'Move'.
            //    e.Effects = DragDropEffects.Move;
            //}
            
        }

        private static void Control_DragEnter(object sender, DragEventArgs e)
        {
            if (dragAdorner != null && dragAdorner.Visibility != Visibility.Visible)
            {
                // Update the location of the adorner and then show it.				
                UpdateDragAdornerLocation();
                dragAdorner.Visibility = Visibility.Visible;
            }
            
        }

        private static void Control_DragLeave(object sender, DragEventArgs e)
        {
            if (!IsMouseOver(ThisTreeView))
            {
                if (ItemUnderDragCursor != null)
                    ItemUnderDragCursor = null;

                if (dragAdorner != null)
                    dragAdorner.Visibility = Visibility.Collapsed;
            }
            
        }

        private static void Control_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;

            if (ShowDragAdornerResolved)
                UpdateDragAdornerLocation();

            // Update the item which is known to be currently under the drag cursor.
            //int index = IndexUnderDragCursor;
            //ItemUnderDragCursor = index < 0 ? null : ThisTreeView.Items[index] as TreeViewItemViewModel;

            
        }

        static AdornerLayer adornerLayer;

        private static void Control_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!CanStartDragOperation)
                return;

            // Select the item the user clicked on.
            //if( this.treeView.SelectedItem != this.indexToSelect )
            //	this.treeView.SelectedItem = this.indexToSelect;

            // If the item at the selected index is null, there's nothing
            // we can do, so just return;
            //if (ThisTreeView.SelectedItem == null)
            //        return;


            if (SelectedTreeViewItem == null)
                return;

            
            InitializeDragOperation(SelectedTreeViewItem);
            PerformDragOperation();
            FinishDragOperation(SelectedTreeViewItem, adornerLayer);
            
        }

        private static void Control_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (IsMouseOverScrollbar)
            //{
            //    // 4/13/2007 - Set the flag to false when cursor is over scrollbar.
            //    canInitiateDrag = false;
            //    return;
            //}

            //int index = IndexUnderDragCursor;
            //canInitiateDrag = index > -1;

            //if (canInitiateDrag)
            //{
            // Remember the location - PARENT and index of the treeViewItem the user clicked on for later.
            if (SelectedTreeViewItem != null)
            {
                ptMouseDown = MouseUtilities.GetMousePosition(ThisTreeView);
                //indexToSelect = index;
                
                //MessageBox.Show(ptMouseDown.X.ToString() + " " + ptMouseDown.Y.ToString());
                //e.Handled = true;
            }

            else
            {
                ptMouseDown = new Point(-10000, -10000);
                indexToSelect = null;// -1;

               // e.Handled = true;
            }
        }

        private static void Control_Selected(object sender, RoutedEventArgs e)
        {
            SelectedTreeViewItem = (TreeViewItem)sender;
            e.Handled = true;
            adornerLayer = ShowDragAdornerResolved ? InitializeAdornerLayer(SelectedTreeViewItem) : null;



            // ptMouseDown = MouseUtilities.GetMousePosition(SelectedTreeViewItem);
        }

        public static void SetUseDnD(DependencyObject target, bool value)
        {
            target.SetValue(UseDnDProperty, value);
        }
                
        public static bool GetUseDnD(DependencyObject target)
        {
            return (bool)target.GetValue(UseDnDProperty);
        }

        private static TreeView thistreeview;
        public static TreeView ThisTreeView
        {
            get { return thistreeview; }
            set { thistreeview = value; }
        }

        private static TreeViewItem selectedtreeviewitem;
        public static TreeViewItem SelectedTreeViewItem
        {
            get { return selectedtreeviewitem; }
            set { selectedtreeviewitem = value; }
        }

        private static void UseDnDChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is TreeView control)
            {
                ThisTreeView =(TreeView) target;
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    //control.PreviewMouseLeftButtonDown += treeView_PreviewMouseLeftButtonDown;// Control_PreviewMouseLeftButtonDown;
                    //control.PreviewMouseMove += treeView_PreviewMouseMove;// Control_PreviewMouseMove;
                    //control.DragOver += treeView_DragOver;// Control_DragOver;
                    //control.DragLeave += treeView_DragLeave;// Control_DragLeave;
                    //control.DragEnter += treeView_DragEnter;// Control_DragEnter;
                    //control.Drop += treeView_Drop;// Control_Drop;
                    //control.SelectedItemChanged += Control_SelectedItemChanged;
                }
                //else
                //if ((e.NewValue == null) && (e.OldValue != null))
                //{
                ////    control.PreviewMouseLeftButtonDown -= treeView_PreviewMouseLeftButtonDown;// Control_PreviewMouseLeftButtonDown;
                //    //control.PreviewMouseMove -= treeView_PreviewMouseMove;// Control_PreviewMouseMove;
                //    //control.DragOver -= treeView_DragOver;// treeView_DragOver;// Control_DragOver;
                //    //control.DragLeave -= treeView_DragLeave;// Control_DragLeave;
                //    //control.DragEnter -= treeView_DragEnter;// Control_DragEnter;
                ////    control.Drop -= treeView_Drop;// Control_Drop;
                //}
            }
        }

        static bool canInitiateDrag = false;
       static DragAdorner dragAdorner;
       static double dragAdornerOpacity = 0.7;
       static TreeViewItem indexToSelect = null;
       static bool isDragInProgress;
       static TreeViewItem itemUnderDragCursor;
       static Point ptMouseDown;
       static bool showDragAdorner = true;

            #region Constructors

            /// <summary>
            /// Initializes a new instance of treeViewDragManager.
            /// </summary>
           //static public TVDragDrop()
           // {
           //     canInitiateDrag = false;
           //     dragAdornerOpacity = 0.7;
           //     indexToSelect = null;
           //     showDragAdorner = true;
           // }

           // /// <summary>
           // /// Initializes a new instance of treeViewDragManager.
           // /// </summary>
           // /// <param name="treeView"></param>
           //static public TVDragDrop(TreeView treeView) : this()
           // {
           //     this.TreeView = treeView;
           // }

            /// <summary>
            /// Initializes a new instance of treeViewDragManager.
            /// </summary>
            /// <param name="treeView"></param>
            /// <param name="dragAdornerOpacity"></param>
            //public TVDragDrop(TreeView treeView, double dragAdornerOpacity) : this(treeView)
            //{
            //    this.DragAdornerOpacity = dragAdornerOpacity;
            //}

            /// <summary>
            /// Initializes a new instance of treeViewDragManager.
            /// </summary>
            /// <param name="treeView"></param>
            /// <param name="showDragAdorner"></param>
            //public TVDragDrop(TreeView treeView, bool showDragAdorner) : this(treeView)
            //{
            //    this.ShowDragAdorner = showDragAdorner;
            //}

            #endregion Constructors

            #region DragAdornerOpacity

            /// <summary>
            /// Gets/sets the opacity of the drag adorner.  This property has no
            /// effect if ShowDragAdorner is false. The default value is 0.7
            /// </summary>
            //public double DragAdornerOpacity
            //{
            //    get { return this.dragAdornerOpacity; }
            //    set
            //    {
            //        if (this.IsDragInProgress)
            //            throw new InvalidOperationException("Cannot set the DragAdornerOpacity property during a drag operation.");

            //        if (value < 0.0 || value > 1.0)
            //            throw new ArgumentOutOfRangeException("DragAdornerOpacity", value, "Must be between 0 and 1.");

            //        this.dragAdornerOpacity = value;
            //    }
            //}

            #endregion DragAdornerOpacity

            /// <summary>
            /// Returns true if there is currently a drag operation being managed.
            /// </summary>
         static public bool IsDragInProgress
            {
                get { return isDragInProgress; }
                private set { isDragInProgress = value; }
            }

            #region TreeView

            /// <summary>
            /// Gets/sets the treeView whose dragging is managed.  This property
            /// can be set to null, to prevent drag management from occuring.  If
            /// the treeView's AllowDrop property is false, it will be set to true.
            /// </summary>
            //public TreeView TreeView
            //{
            //    get { return treeView; }
            //    set
            //    {
            //        if (this.IsDragInProgress)
            //            throw new InvalidOperationException("Cannot set the treeView property during a drag operation.");

            //        if (this.treeView != null)
            //        {
            //            #region Unhook Events

            //            this.treeView.PreviewMouseLeftButtonDown -= treeView_PreviewMouseLeftButtonDown;
            //            this.treeView.PreviewMouseMove -= treeView_PreviewMouseMove;
            //            this.treeView.DragOver -= treeView_DragOver;
            //            this.treeView.DragLeave -= treeView_DragLeave;
            //            this.treeView.DragEnter -= treeView_DragEnter;
            //            this.treeView.Drop -= treeView_Drop;

            //            #endregion Unhook Events
            //        }

            //        this.treeView = value;

            //        if (this.treeView != null)
            //        {
            //            if (!this.treeView.AllowDrop)
            //                this.treeView.AllowDrop = true;

            //            #region Hook Events

            //            this.treeView.PreviewMouseLeftButtonDown += treeView_PreviewMouseLeftButtonDown;
            //            this.treeView.PreviewMouseMove += treeView_PreviewMouseMove;
            //            this.treeView.DragOver += treeView_DragOver;
            //            this.treeView.DragLeave += treeView_DragLeave;
            //            this.treeView.DragEnter += treeView_DragEnter;
            //            this.treeView.Drop += treeView_Drop;

            //            #endregion Hook Events
            //        }
            //    }
            //}

            #endregion treeView

            /// <summary>
            /// Raised when a drop occurs.  By default the dropped item will be moved
            /// to the target index.  Handle this event if relocating the dropped item
            /// requires custom behavior.  Note, if this event is handled the default
            /// item dropping logic will not occur.
            /// </summary>
            //public static event EventHandler<TVProcessDropEventArgs<TreeViewItemViewModel>> ProcessDrop;

            /// <summary>
            /// Gets/sets whether a visual representation of the treeViewItem being dragged
            /// follows the mouse cursor during a drag operation.  The default value is true.
            /// </summary>
            public static bool ShowDragAdorner
            {
                get { return showDragAdorner; }
                set
                {
                    if (IsDragInProgress)
                        throw new InvalidOperationException("Cannot set the ShowDragAdorner property during a drag operation.");

                    showDragAdorner = value;
                }
            }
               
        static void treeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
           

        }
        
        static void treeView_PreviewMouseMove(object sender, MouseEventArgs e)
            {
           
        }

        static void treeView_DragOver(object sender, DragEventArgs e)
            {
                
            }

        static void treeView_DragLeave(object sender, DragEventArgs e)
            {
               
            }

        static void treeView_DragEnter(object sender, DragEventArgs e)
            {
                
            }

        static void treeView_Drop(object sender, DragEventArgs e)
            {
                
            }

        static bool CanStartDragOperation
            {
                get
                {
                    //if (Mouse.LeftButton != MouseButtonState.Pressed)
                    //    return false;

                    //if (!canInitiateDrag)
                    //    return false;

                    //if (indexToSelect == null)
                    //    return false;

                    //if (!HasCursorLeftDragThreshold)
                    //    return false;

                    return true;
                }
            }

        static void FinishDragOperation(TreeViewItem draggedItem, AdornerLayer adornerLayer)
            {
                // Let the treeViewItem know that it is not being dragged anymore.
                TreeViewItemDragState.SetIsBeingDragged(SelectedTreeViewItem, false);

                IsDragInProgress = false;

                if (ItemUnderDragCursor != null)
                    ItemUnderDragCursor = null;

                // Remove the drag adorner from the adorner layer.
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(dragAdorner);
                    dragAdorner = null;
                }
            }

        static TreeViewItem GettreeViewItem(TreeViewItemViewModel dataItem)
        {
            if (ThisTreeView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return ThisTreeView.ItemContainerGenerator.ContainerFromItem(dataItem) as TreeViewItem;
        }

        static TreeViewItem GettreeViewItem(TreeViewItem treeviewitem)
        {
            if (ThisTreeView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return ThisTreeView.ItemContainerGenerator.ContainerFromItem(treeviewitem) as TreeViewItem;
        }

        static bool HasCursorLeftDragThreshold
            {
                get
                {
                    if (indexToSelect == null)
                        return false;

                    TreeViewItem item = GettreeViewItem(indexToSelect);
                    Rect bounds = VisualTreeHelper.GetDescendantBounds(item);
                    Point ptInItem = ThisTreeView.TranslatePoint(ptMouseDown, item);

                    // In case the cursor is at the very top or bottom of the treeViewItem
                    // we want to make the vertical threshold very small so that dragging
                    // over an adjacent item does not select it.
                    double topOffset = Math.Abs(ptInItem.Y);
                    double btmOffset = Math.Abs(bounds.Height - ptInItem.Y);
                    double vertOffset = Math.Min(topOffset, btmOffset);

                    double width = SystemParameters.MinimumHorizontalDragDistance * 2;
                    double height = Math.Min(SystemParameters.MinimumVerticalDragDistance, vertOffset) * 2;
                    Size szThreshold = new Size(width, height);

                    Rect rect = new Rect(ptMouseDown, szThreshold);
                    rect.Offset(szThreshold.Width / -2, szThreshold.Height / -2);
                    Point ptIntreeView = MouseUtilities.GetMousePosition(ThisTreeView);
                    return !rect.Contains(ptIntreeView);
                }
            }

        /// <summary>
        /// Returns the index of the treeViewItem underneath the
        /// drag cursor, or -1 if the cursor is not over an item.
        /// </summary>
        static TreeViewItem IndexUnderDragCursor
            {
                get
                {
                    foreach(TreeViewItem tvi in ThisTreeView.Items)
                    {
                        TreeViewItem item = GettreeViewItem(tvi);
                        if (IsMouseOver(item))
                        {
                            return item;
                           
                        }
                    }
                    return null;
                }
            }

        static AdornerLayer InitializeAdornerLayer(TreeViewItem itemToDrag)
            {
                // Create a brush which will paint the treeViewItem onto
                // a visual in the adorner layer.
                VisualBrush brush = new VisualBrush(SelectedTreeViewItem);

            // Create an element which displays the source item while it is dragged.
            dragAdorner = new DragAdorner(SelectedTreeViewItem, SelectedTreeViewItem.RenderSize, brush)
            {
                // Set the drag adorner's opacity.		
                Opacity = 0.7// DragAdornerOpacity
                };

                AdornerLayer layer = AdornerLayer.GetAdornerLayer(ThisTreeView);
                layer.Add(dragAdorner);

                // Save the location of the cursor when the left mouse button was pressed.
                ptMouseDown = MouseUtilities.GetMousePosition(ThisTreeView);

                return layer;
            }

        static void InitializeDragOperation(TreeViewItem itemToDrag)
            {
                // Set some flags used during the drag operation.
                IsDragInProgress = true;
                canInitiateDrag = false;

                // Let the treeViewItem know that it is being dragged.
                TreeViewItemDragState.SetIsBeingDragged(SelectedTreeViewItem, true);
            }

        static bool IsMouseOver(Visual target)
            {
                // We need to use MouseUtilities to figure out the cursor
                // coordinates because, during a drag-drop operation, the WPF
                // mechanisms for getting the coordinates behave strangely.

                Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
                Point mousePos = MouseUtilities.GetMousePosition(target);
                return bounds.Contains(mousePos);
            }

            /// <summary>
            /// Returns true if the mouse cursor is over a scrollbar in the treeView.
            /// </summary>
        static bool IsMouseOverScrollbar
            {
                get
                {
                    Point ptMouse = MouseUtilities.GetMousePosition(ThisTreeView);
                    HitTestResult res = VisualTreeHelper.HitTest(ThisTreeView, ptMouse);
                    if (res == null)
                        return false;

                    DependencyObject depObj = res.VisualHit;
                    while (depObj != null)
                    {
                        if (depObj is ScrollBar)
                            return true;

                        // VisualTreeHelper works with objects of type Visual or Visual3D.
                        // If the current object is not derived from Visual or Visual3D,
                        // then use the LogicalTreeHelper to find the parent element.
                        if (depObj is Visual || depObj is System.Windows.Media.Media3D.Visual3D)
                            depObj = VisualTreeHelper.GetParent(depObj);
                        else
                            depObj = LogicalTreeHelper.GetParent(depObj);
                    }

                    return false;
                }
            }

        static TreeViewItem ItemUnderDragCursor
            {
                get { return itemUnderDragCursor; }
                set
                {
                    if (itemUnderDragCursor == value)
                        return;

                    // The first pass handles the previous item under the cursor.
                    // The second pass handles the new one.
                    for (int i = 0; i < 2; ++i)
                    {
                        if (i == 1)
                            itemUnderDragCursor = value;

                        if (itemUnderDragCursor != null)
                        {
                            TreeViewItem treeViewItem = GettreeViewItem(itemUnderDragCursor);
                            if (treeViewItem != null)
                                TreeViewItemDragState.SetIsUnderDragCursor(treeViewItem, i == 1);
                        }
                    }
                }
            }
                 
        static void PerformDragOperation()
        {
            DragDropEffects allowedEffects = DragDropEffects.Move | DragDropEffects.Move | DragDropEffects.Link;
            if (System.Windows.DragDrop.DoDragDrop(ThisTreeView, SelectedTreeViewItem, allowedEffects) != DragDropEffects.None)
            {
                // The item was dropped into a new location,
                // so make it the new selected item.
                //this.treeView.SelectedItem = selectedItem;
            }
        }     

        static bool ShowDragAdornerResolved
            {
            get { return ShowDragAdorner; }// && DragAdornerOpacity > 0.0; }
            }
            
         static void UpdateDragAdornerLocation()
            {
                if (dragAdorner != null)
                {
                    Point ptCursor = MouseUtilities.GetMousePosition(ThisTreeView);

                    double left = ptCursor.X - ptMouseDown.X;

                    // 4/13/2007 - Made the top offset relative to the item being dragged.
                    TreeViewItem itemBeingDragged = GettreeViewItem(indexToSelect);
                    Point itemLoc = itemBeingDragged.TranslatePoint(new Point(0, 0), ThisTreeView);
                    double top = itemLoc.Y + ptCursor.Y - ptMouseDown.Y;

                    dragAdorner.SetOffsets(left, top);
                }
            }

        }

        /// <summary>
        /// Exposes attached properties used in conjunction with the treeViewDragDropManager class.
        /// Those properties can be used to allow triggers to modify the appearance of treeViewItems
        /// in a treeView during a drag-drop operation.
        /// </summary>
        public static class TreeViewItemDragState
        {
            #region IsBeingDragged

            /// <summary>
            /// Identifies the treeViewItemDragState's IsBeingDragged attached property.  
            /// This field is read-only.
            /// </summary>
            public static readonly DependencyProperty IsBeingDraggedProperty =
                DependencyProperty.RegisterAttached(
                    "IsBeingDragged",
                    typeof(bool),
                    typeof(TreeViewItemDragState),
                    new UIPropertyMetadata(false));

            /// <summary>
            /// Returns true if the specified treeViewItem is being dragged, else false.
            /// </summary>
            /// <param name="item">The treeViewItem to check.</param>
            public static bool GetIsBeingDragged(TreeViewItem item)
            {
                return (bool)item.GetValue(IsBeingDraggedProperty);
            }

            /// <summary>
            /// Sets the IsBeingDragged attached property for the specified treeViewItem.
            /// </summary>
            /// <param name="item">The treeViewItem to set the property on.</param>
            /// <param name="value">Pass true if the element is being dragged, else false.</param>
            internal static void SetIsBeingDragged(TreeViewItem item, bool value)
            {
                item.SetValue(IsBeingDraggedProperty, value);
            }

            #endregion IsBeingDragged

            #region IsUnderDragCursor

            /// <summary>
            /// Identifies the TreeViewItemDragState's IsUnderDragCursor attached property.  
            /// This field is read-only.
            /// </summary>
            public static readonly DependencyProperty IsUnderDragCursorProperty =
                DependencyProperty.RegisterAttached(
                    "IsUnderDragCursor",
                    typeof(bool),
                    typeof(TreeViewItemDragState),
                    new UIPropertyMetadata(false));

            /// <summary>
            /// Returns true if the specified treeViewItem is currently underneath the cursor 
            /// during a drag-drop operation, else false.
            /// </summary>
            /// <param name="item">The treeViewItem to check.</param>
            public static bool GetIsUnderDragCursor(TreeViewItem item)
            {
                return (bool)item.GetValue(IsUnderDragCursorProperty);
            }

            /// <summary>
            /// Sets the IsUnderDragCursor attached property for the specified treeViewItem.
            /// </summary>
            /// <param name="item">The treeViewItem to set the property on.</param>
            /// <param name="value">Pass true if the element is underneath the drag cursor, else false.</param>
            internal static void SetIsUnderDragCursor(TreeViewItem item, bool value)
            {
                item.SetValue(IsUnderDragCursorProperty, value);
            }

            #endregion IsUnderDragCursor
        }


    /// <summary>
    /// Event arguments used by the treeViewDragDropManager.ProcessDrop event.
    /// </summary>
    /// <typeparam name="TreeViewItemViewModel">The type of data object being dropped.</typeparam>
    public class TVProcessDropEventArgs<TreeViewItemViewModel> : EventArgs where TreeViewItemViewModel : class
        {
            #region Data

            ObservableCollection<TreeViewItemViewModel> itemsSource;
        TreeViewItemViewModel dataItem;
            int oldIndex;
            int newIndex;
            DragDropEffects allowedEffects = DragDropEffects.None;
            DragDropEffects effects = DragDropEffects.None;

            #endregion Data

            #region Constructor

            internal TVProcessDropEventArgs(
                ObservableCollection<TreeViewItemViewModel> itemsSource,
                TreeViewItemViewModel dataItem,
                int oldIndex,
                int newIndex,
                DragDropEffects allowedEffects)
            {
                this.itemsSource = itemsSource;
                this.dataItem = dataItem;
                this.oldIndex = oldIndex;
                this.newIndex = newIndex;
                this.allowedEffects = allowedEffects;
            }

            #endregion Constructor

            #region Public Properties

            /// <summary>
            /// The items source of the treeView where the drop occurred.
            /// </summary>
            public ObservableCollection<TreeViewItemViewModel> ItemsSource
            {
                get { return this.itemsSource; }
            }

            /// <summary>
            /// The data object which was dropped.
            /// </summary>
            public TreeViewItemViewModel DataItem
            {
                get { return this.dataItem; }
            }

            /// <summary>
            /// The current index of the data item being dropped, in the ItemsSource collection.
            /// </summary>
            public int OldIndex
            {
                get { return this.oldIndex; }
            }

            /// <summary>
            /// The target index of the data item being dropped, in the ItemsSource collection.
            /// </summary>
            public int NewIndex
            {
                get { return this.newIndex; }
            }

            /// <summary>
            /// The drag drop effects allowed to be performed.
            /// </summary>
            public DragDropEffects AllowedEffects
            {
                get { return allowedEffects; }
            }

            /// <summary>
            /// The drag drop effect(s) performed on the dropped item.
            /// </summary>
            public DragDropEffects Effects
            {
                get { return effects; }
                set { effects = value; }
            }

            #endregion Public Properties
        }

    public class MouseUtilities
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hwnd, ref Win32Point pt);

        /// <summary>
        /// Returns the mouse cursor location.  This method is necessary during 
        /// a drag-drop operation because the WPF mechanisms for retrieving the
        /// cursor coordinates are unreliable.
        /// </summary>
        /// <param name="relativeTo">The Visual to which the mouse coordinates will be relative.</param>
        public static Point GetMousePosition(Visual relativeTo)
        {
            Win32Point mouse = new Win32Point();
            GetCursorPos(ref mouse);

            // Using PointFromScreen instead of Dan Crevier's code (commented out below)
            // is a bug fix created by William J. Roberts.  Read his comments about the fix
            // here: http://www.codeproject.com/useritems/ListViewDragDropManager.asp?msg=1911611#xx1911611xx
            return relativeTo.PointFromScreen(new Point((double)mouse.X, (double)mouse.Y));

            #region Commented Out
            //System.Windows.Interop.HwndSource presentationSource =
            //    (System.Windows.Interop.HwndSource)PresentationSource.FromVisual( relativeTo );
            //ScreenToClient( presentationSource.Handle, ref mouse );
            //GeneralTransform transform = relativeTo.TransformToAncestor( presentationSource.RootVisual );
            //Point offset = transform.Transform( new Point( 0, 0 ) );
            //return new Point( mouse.X - offset.X, mouse.Y - offset.Y );
            #endregion Commented Out
        }
    }

    /// <summary>
	/// Renders a visual which can follow the mouse cursor, 
	/// such as during a drag-and-drop operation.
	/// </summary>
	public  class DragAdorner : Adorner
    {
        #region Data

        private System.Windows.Shapes.Rectangle child = null;
        private double offsetLeft = 0;
        private double offsetTop = 0;

        #endregion Data

        #region Constructor

        /// <summary>
        /// Initializes a new instance of DragVisualAdorner.
        /// </summary>
        /// <param name="adornedElement">The element being adorned.</param>
        /// <param name="size">The size of the adorner.</param>
        /// <param name="brush">A brush to with which to paint the adorner.</param>
        public DragAdorner(UIElement adornedElement, Size size, Brush brush): base(adornedElement)
            
        {
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Fill = brush;
            rect.Width = size.Width;
            rect.Height = size.Height;
            rect.IsHitTestVisible = false;
            this.child = rect;
        }

        #endregion Constructor

        #region Public Interface

        #region GetDesiredTransform

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.offsetLeft, this.offsetTop));
            return result;
        }

        #endregion GetDesiredTransform

        #region OffsetLeft

        /// <summary>
        /// Gets/sets the horizontal offset of the adorner.
        /// </summary>
        public double OffsetLeft
        {
            get { return this.offsetLeft; }
            set
            {
                this.offsetLeft = value;
                UpdateLocation();
            }
        }

        #endregion OffsetLeft

        #region SetOffsets

        /// <summary>
        /// Updates the location of the adorner in one atomic operation.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void SetOffsets(double left, double top)
        {
            this.offsetLeft = left;
            this.offsetTop = top;
            this.UpdateLocation();
        }

        #endregion SetOffsets

        #region OffsetTop

        /// <summary>
        /// Gets/sets the vertical offset of the adorner.
        /// </summary>
        public double OffsetTop
        {
            get { return this.offsetTop; }
            set
            {
                this.offsetTop = value;
                UpdateLocation();
            }
        }

        #endregion OffsetTop

        #endregion Public Interface

        #region Protected Overrides

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Override.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        /// <summary>
        /// Override.  Always returns 1.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        #endregion Protected Overrides

        #region Private Helpers

        private void UpdateLocation()
        {
            if (this.Parent is AdornerLayer adornerLayer)
                adornerLayer.Update(this.AdornedElement);
        }

        #endregion Private Helpers
    }
    
   
}








