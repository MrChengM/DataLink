using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace SignalMonitor
{
    public class MyCommand<T> : ICommand where T: RoutedEventArgs
    {
        public event EventHandler CanExecuteChanged;


        private object _sender = new object(); 
        private Func<T, bool> _canExecute;
        private Action<object, object,T> _execute;

        public object Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        public MyCommand(Func<T,bool> canExecute,Action<object, object,T> execute)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            var exParameter = parameter as ExCommandParameter;
            if (exParameter != null)
            {
                if (_canExecute == null || _canExecute((T)exParameter.EventArgs))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
               return true;
            }
         
        }

        public void Execute(object parameter)
        {
            var exParameter = parameter as ExCommandParameter;
            if (exParameter != null)
                _execute?.Invoke(exParameter.Sender, exParameter.Parameter,(T)exParameter.EventArgs);
            else
                _execute(null,null,null);
        }
    }
}
