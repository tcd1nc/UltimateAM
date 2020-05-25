using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace AssetManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        bool doShutdown = false;

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //App.Current.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            // Set application startup culture based on config settings

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            if (doShutdown)            
                ExecuteShutdown();            
            else                            
            if( GlobalClass.LoadAll() == true)                 
                this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            else
            {
                MessageBox.Show("Database Connection Failed!\nProgram closing.", "Database connection failed", MessageBoxButton.OK, MessageBoxImage.Error);
                ExecuteShutdown();
            }            
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled error has occurred " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;

            
            Shutdown(0);
        }
                      
        public void ExecuteShutdown()
        {
            App.Current.Dispatcher.UnhandledException -= Dispatcher_UnhandledException;
           
            Shutdown();       
        }

    }

}
