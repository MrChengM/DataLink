using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
namespace SignalMonitor
{
    public class MyEventCommand : TriggerAction<DependencyObject>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(MyEventCommand), new PropertyMetadata(null));
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(MyEventCommand), new PropertyMetadata(null));
        protected override void Invoke(object parameter)
        {

            ExCommandParameter exParameter = new ExCommandParameter
            {

                Sender = AssociatedObject,

                //Parameter = GetValue(CommandParameterProperty),

                Parameter = CommandParameter,

                EventArgs = parameter as EventArgs

            };
            if (Command != null && Command.CanExecute(exParameter))
            {
                Command.Execute(exParameter);
            }
        }
    }
    public class ExCommandParameter

    {

        /// <summary>

        /// 事件触发源

        /// </summary>

        public DependencyObject Sender { get; set; }

        /// <summary>

        /// 事件参数

        /// </summary>

        public EventArgs EventArgs { get; set; }

        /// <summary>

        /// 额外参数

        /// </summary>

        public object Parameter { get; set; }



    }

}
