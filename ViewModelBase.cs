using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;

namespace AssetManager
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        
        ICommand _closewindowcommand;
        ICommand _shutdowncommand;                          

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(caller));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public ICommand CloseWindowCommand
        {
            get
            {
                if (_closewindowcommand == null)
                {
                    _closewindowcommand = new DelegateCommand(CanExecute, ExecuteCloseWindow);
                }
                return _closewindowcommand;
            }
        }

        public void ExecuteCloseWindow(object parameter)
        {
            var wnd = parameter as Window;
            if (wnd != null)
                wnd.Close();
        }
               
        public ICommand Shutdown
        {
            get
            {
                if (_shutdowncommand == null)
                {
                    _shutdowncommand = new DelegateCommand(CanExecute, ExecuteShutdown);
                }
                return _shutdowncommand;
            }
        }

        private void ExecuteShutdown(object parameter)
        {
            Application.Current.Shutdown();
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        bool? _blndialogresult;
        public bool? DialogResult
        {
            get { return _blndialogresult; }
            set { SetField(ref _blndialogresult, value); }
        }

        
        //DateTime? _selectauditDatedialogresult;
        //public DateTime? SelectAuditDateDialogResult
        //{
        //    get { return _selectauditDatedialogresult; }
        //    set { SetField(ref _selectauditDatedialogresult, value); }
        //}
       
        object _objdialogresult;
        public object ObjDialogResult
        {
            get { return _objdialogresult; }
            set { SetField(ref _objdialogresult, value); }
        }


        bool? _CloseWindowFlag;
        public bool? CloseWindowFlag
        {
            get { return _CloseWindowFlag; }
            set { SetField(ref _CloseWindowFlag, value); }
        }
        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null ? true : !CloseWindowFlag;
            }));


            //if(AssetDialogResult == AssetReturnValue.HasNewParent || AssetDialogResult == AssetReturnValue.IsDeleted || AssetDialogResult == AssetReturnValue.IsNew)
            //{
            //    //reload

            //}
        }

        ICommand _close;
        public ICommand Close
        {
            get
            {
                if (_close == null)
                    _close = new DelegateCommand(CanExecute, ExecuteClose);
                return _close;
            }
        }

        private void ExecuteClose(object parameter)
        {
            CloseWindow();
        }


    }


    public class DelegateCommand : ICommand
    {
        #region Commands


        Predicate<object> canExecute;
        Action<object> execute;

        public DelegateCommand(Predicate<object> _canexecute, Action<object> _execute) : this()
        {
            canExecute = _canexecute;
            execute = _execute;
        }

        public DelegateCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public void Execute(object parameter)
        {
            execute(parameter);
        }

        #endregion

    }

}
