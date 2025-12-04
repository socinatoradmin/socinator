using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DominatorHouseCore.Converters
{
    public class InvokeCommandWithArgsAction : TriggerAction<DependencyObject>
    {
        private string commandName;

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandWithArgsAction), null);

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandWithArgsAction), null);



        public string CommandName
        {
            get
            {
                ReadPreamble();
                return commandName;
            }
            set
            {
                if (CommandName != value)
                {
                    WritePreamble();
                    commandName = value;
                    WritePostscript();
                }
            }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (Command == null)
                return;

            // parameter = EventArgs
            var args = parameter;

            // merge both values into tuple
            var finalParam = new
            {
                Row = CommandParameter,   // current row
                EventArgs = args          // event args (KeyEventArgs or RoutedEventArgs)
            };

            if (Command.CanExecute(finalParam))
                Command.Execute(finalParam);
        }
    }
}
