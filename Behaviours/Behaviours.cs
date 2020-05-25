using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace AssetManager.Behaviours
{
    
    public class MouseDoubleClick
    {
        public static DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseDoubleClick), new UIPropertyMetadata(CommandChanged));
        public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(MouseDoubleClick), new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }
        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is Control control)
            {
                if ((e.NewValue != null) && (e.OldValue == null))                
                    control.MouseDoubleClick += OnMouseDoubleClick;                
                else 
                if ((e.NewValue == null) && (e.OldValue != null))                
                    control.MouseDoubleClick -= OnMouseDoubleClick;                
            }
        }

        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            ICommand command = (ICommand)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);
            //e.Handled = true;
            //command.Execute(commandParameter);

            //ensure that event on called on selected element
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                    return;
            }

            if (command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
                e.Handled = true;
            }
        }
    }

    public static class DialogCloser
    {
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached("DialogResult", typeof(bool?),
                typeof(DialogCloser), new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.DialogResult = e.NewValue as bool?;
        }
        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }

        public static readonly DependencyProperty DialogCloseProperty = DependencyProperty.RegisterAttached("DialogClose", typeof(bool?),
                typeof(DialogCloser), new PropertyMetadata(false, DialogCloseChanged));

        private static void DialogCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && (e.NewValue as bool?) == true)
                window.Close();
        }

        public static void SetDialogClose(Window target, bool? value)
        {
            target.SetValue(DialogCloseProperty, value);
        }


        /// <summary>
        /// Generic attached property to allow passing on objects to/from windows - replaces the use of adding a public property to window
        /// </summary>
        public static readonly DependencyProperty ObjDialogResultProperty = DependencyProperty.RegisterAttached("ObjDialogResult", typeof(object),
              typeof(DialogCloser), new PropertyMetadata(ObjDialogResultChanged));

        private static void ObjDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                window.Tag = e.NewValue;
                //window.Close();
            }
        }

        public static void SetObjDialogResult(Window target, object value)
        {
            target.SetValue(ObjDialogResultProperty, value);
        }

    }

}
