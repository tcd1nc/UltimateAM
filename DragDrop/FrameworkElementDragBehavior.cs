using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Input;

namespace AssetManager.DragDrop
{
    public class FrameworkElementDragBehavior : Behavior<FrameworkElement>
    {
        private bool isMouseClicked = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseLeftButtonDown += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
            this.AssociatedObject.MouseLeftButtonUp += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp);
            this.AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
        }

        /// <summary>
        /// This was added to allow disabling of drag mechanism thru binding 
        /// and avoid need to code static variable into this class
        /// </summary>
        public static DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(FrameworkElementDragBehavior),
            new PropertyMetadata(DialogResultChanged));

        public static void SetIsEnabled(DependencyObject target, bool value)
        {
            target.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(DependencyObject target)
        {
            return (bool)target.GetValue(IsEnabledProperty);
        }

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FrameworkElementDragBehavior).IsEnabled = (bool)e.NewValue;
        }
        
        bool _isenabled;
        private bool IsEnabled
        {
            get { return _isenabled; }
            set { _isenabled = value; }
        }


        void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseClicked = IsEnabled;  
            // Dont need this if IsEnabled dependency property is used   isMouseClicked = GlobalClass.IsAdministrator;
            //  isMouseClicked = true;
        }

        void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMouseClicked = false;

        }

        void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isMouseClicked)
            {
                //set the item's DataContext as the data to be transferred
                if (this.AssociatedObject.DataContext is IDragable dragObject)
                {
                    DataObject data = new DataObject();
                    data.SetData(dragObject.DataType, this.AssociatedObject.DataContext);
                    ViewModels.AssetTreeExViewModel.GetChildIDs(dragObject as ViewModels.TVAssetViewModel);
                    System.Windows.DragDrop.DoDragDrop(this.AssociatedObject, data, DragDropEffects.Move);
                }
            }
           
          isMouseClicked = false;
        }


    }

}
