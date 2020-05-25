using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssetManager.Behaviours
{
    /// <summary>
    /// There are no direct ways to alter windows buttons in WPF so hooking into Windows OS is required
    /// </summary>
    public static class WindowBehavior
    {
        //private static readonly Type OwnerType = typeof(WindowBehavior);

        //#region Win32 imports

        //private const int GWL_STYLE = -16;
        //private const int WS_SYSMENU = 0x80000;
        //private const int WS_MINIMIZE = -131073;
        //private const int WS_MAXIMIZE = -65537;
        //private const uint MF_BYCOMMAND = 0x00000000;
        //private const uint MF_GRAYED = 0x00000001;
        //private const uint SC_CLOSE = 0xF060;
        //private const uint MF_ENABLED = 0x00000000;

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        //[DllImport("user32.dll")]
        //private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);        
        //[DllImport("user32.dll")]
        //private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        //[DllImport("user32.dll")]
        //private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        //#endregion

        //#region HideCloseButton (attached property)
        ///// <summary>
        ///// Hides entire window top border  - no icon or buttons
        ///// </summary>
        //public static readonly DependencyProperty HideCloseButtonProperty = DependencyProperty.RegisterAttached("HideCloseButton", typeof(bool), OwnerType,
        //        new FrameworkPropertyMetadata(false, new PropertyChangedCallback(HideCloseButtonChangedCallback)));

        //[AttachedPropertyBrowsableForType(typeof(Window))]
        //public static bool GetHideCloseButton(Window obj)
        //{
        //    return (bool)obj.GetValue(HideCloseButtonProperty);
        //}

        //[AttachedPropertyBrowsableForType(typeof(Window))]
        //public static void SetHideCloseButton(Window obj, bool value)
        //{
        //    obj.SetValue(HideCloseButtonProperty, value);
        //}

        //private static void HideCloseButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var window = d as Window;
        //    if (window == null) return;

        //    var hideCloseButton = (bool)e.NewValue;
        //    if (hideCloseButton && !GetIsHiddenCloseButton(window))
        //    {
        //        if (!window.IsLoaded)
        //        {
        //            window.Loaded += HideWhenLoadedDelegate;
        //        }
        //        else
        //        {
        //            HideCloseButton(window);
        //        }
        //        SetIsHiddenCloseButton(window, true);
        //    }
        //    else if (!hideCloseButton && GetIsHiddenCloseButton(window))
        //    {
        //        if (!window.IsLoaded)
        //        {
        //            window.Loaded -= ShowWhenLoadedDelegate;
        //        }
        //        else
        //        {
        //            ShowCloseButton(window);
        //        }
        //        SetIsHiddenCloseButton(window, false);
        //    }
        //}

        //private static readonly RoutedEventHandler HideWhenLoadedDelegate = (sender, args) => {
        //    if (sender is Window == false) return;
        //    var w = (Window)sender;
        //    HideCloseButton(w);
        //    w.Loaded -= HideWhenLoadedDelegate;
        //};

        //private static readonly RoutedEventHandler ShowWhenLoadedDelegate = (sender, args) => {
        //    if (sender is Window == false) return;
        //    var w = (Window)sender;
        //    ShowCloseButton(w);
        //    w.Loaded -= ShowWhenLoadedDelegate;
        //};

        //private static void HideCloseButton(Window w)
        //{
        //    var hwnd = new WindowInteropHelper(w).Handle;
        //    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        //}

        //private static void ShowCloseButton(Window w)
        //{
        //    var hwnd = new WindowInteropHelper(w).Handle;
        //    SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) | WS_SYSMENU);
        //}

        //#endregion

        //#region IsHiddenCloseButton (readonly attached property)

        //private static readonly DependencyPropertyKey IsHiddenCloseButtonKey = DependencyProperty.RegisterAttachedReadOnly("IsHiddenCloseButton", typeof(bool), OwnerType,
        //        new FrameworkPropertyMetadata(false));

        //public static readonly DependencyProperty IsHiddenCloseButtonProperty =
        //    IsHiddenCloseButtonKey.DependencyProperty;

        //[AttachedPropertyBrowsableForType(typeof(Window))]
        //public static bool GetIsHiddenCloseButton(Window obj)
        //{
        //    return (bool)obj.GetValue(IsHiddenCloseButtonProperty);
        //}

        //private static void SetIsHiddenCloseButton(Window obj, bool value)
        //{
        //    obj.SetValue(IsHiddenCloseButtonKey, value);
        //}

        //#endregion

        //#region Disable ControlBox (attached property)
        ///// <summary>
        ///// Toggle Enable/Disable state of ControlBox
        ///// </summary>
        //public static readonly DependencyProperty DisableControlBoxProperty =
        //  DependencyProperty.RegisterAttached("DisableControlBox", typeof(bool), OwnerType, new FrameworkPropertyMetadata(false, new PropertyChangedCallback(DisableControlBoxChangedCallback)));

        //[AttachedPropertyBrowsableForType(typeof(Window))]
        //public static bool GetDisableControlBox(Window obj)
        //{
        //    return (bool)obj.GetValue(DisableControlBoxProperty);
        //}
        //[AttachedPropertyBrowsableForType(typeof(Window))]
        //public static void SetDisableControlBox(Window obj, bool value)
        //{
        //    obj.SetValue(DisableControlBoxProperty, value);
        //}
        //private static void DisableControlBoxChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var window = d as Window;
        //    if (window == null) return;
        //    var disableControlBox = (bool)e.NewValue;
        //    if (disableControlBox)
        //    {
        //        if (!window.IsLoaded)
        //        {
        //            window.Loaded += LoadedDelegate;
        //        }
        //        else
        //        {
        //            DisableControlBox(window);
        //        }
        //    }
        //    else
        //    {
        //        if (!window.IsLoaded)
        //        {
        //            window.Loaded += LoadedDelegate;
        //        }
        //        else
        //        {
        //            EnableControlBox(window);
        //        }
        //    }

        //}

        //private static readonly RoutedEventHandler LoadedDelegate = (sender, args) =>
        //{
        //    if (sender is Window == false)
        //        return;
        //    var w = (Window)sender;
        //    DisableControlBox(w);
        //    w.Loaded -= LoadedDelegate;
        //};

        //private static void DisableControlBox(Window w)
        //{
        //    IntPtr menuHandle;
        //    var hwnd = new WindowInteropHelper(w).Handle;
        //    long value = GetWindowLong(hwnd, GWL_STYLE) & -131073 & -65537;
        //    menuHandle = GetSystemMenu(hwnd, false);
        //    if (menuHandle != IntPtr.Zero)
        //    {
        //        EnableMenuItem(menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        //    }
        //}

        //private static void EnableControlBox(Window w)
        //{
        //    IntPtr menuHandle;
        //    var hwnd = new WindowInteropHelper(w).Handle;
        //    long value = GetWindowLong(hwnd, GWL_STYLE) & -131073 & -65537;
        //    menuHandle = GetSystemMenu(hwnd, false);
        //    if (menuHandle != IntPtr.Zero)
        //    {
        //        EnableMenuItem(menuHandle, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED);
        //    }
        //}

        //#endregion


        public static DependencyProperty WindowClosingProperty = DependencyProperty.RegisterAttached("WindowClosing", typeof(ICommand), typeof(WindowBehavior), new PropertyMetadata(WindowClosing));

            public static void SetWindowClosing(DependencyObject target, ICommand value)
            {
                target.SetValue(WindowClosingProperty, value);
            }

            private static void WindowClosing(DependencyObject target, DependencyPropertyChangedEventArgs e)
            {
                if (target is Window control)
                {
                    if ((e.NewValue != null) && (e.OldValue == null))
                        control.Closing += Control_Closing;
                    else if ((e.NewValue == null) && (e.OldValue != null))
                        control.Closing -= Control_Closing;
                }
            }

            private static void Control_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                Control control = sender as Control;
                ICommand command = (ICommand)control.GetValue(WindowClosingProperty);
                if (command.CanExecute(null))
                {
                    command.Execute(null);
                }
                else
                {
                    ICommand cancelClosing = GetCancelClosing(sender as Window);
                    if (cancelClosing != null)
                    {
                        cancelClosing.Execute(null);
                    }
                    e.Cancel = true;
                }

            }

            public static ICommand GetCancelClosing(DependencyObject obj)
            {
                return (ICommand)obj.GetValue(CancelClosingProperty);
            }

            public static void SetCancelClosing(DependencyObject obj, ICommand value)
            {
                obj.SetValue(CancelClosingProperty, value);
            }

            public static readonly DependencyProperty CancelClosingProperty = DependencyProperty.RegisterAttached(
                "CancelClosing", typeof(ICommand), typeof(WindowBehavior));

        }
   
}
