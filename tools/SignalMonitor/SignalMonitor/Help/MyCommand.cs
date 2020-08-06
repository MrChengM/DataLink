using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalMonitor
{
    public class MyCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;


        private object _sender = new object(); 
        private Func<T, bool> _canExecute;
        private Action<object,T> _execute;

        public object Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        public MyCommand(Func<T,bool> canExecute,Action<object,T> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null || _canExecute((T)parameter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Execute(object parameter)
        {
            if(_execute!=null)
            _execute(_sender,(T)parameter);
        }
    }
}
