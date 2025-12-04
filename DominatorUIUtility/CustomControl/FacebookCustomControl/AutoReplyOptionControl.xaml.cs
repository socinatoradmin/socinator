using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.FacebookModels;

namespace DominatorUIUtility.CustomControl.FacebookCustomControl
{
    /// <summary>
    ///     Interaction logic for AutoReplyOptionControl.xaml
    /// </summary>
    public partial class AutoReplyOptionControl : INotifyPropertyChanged
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnfriendOptionProperty =
            DependencyProperty.Register("AutoReplyOptionModel", typeof(AutoReplyOptionModel),
                typeof(AutoReplyOptionControl), new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckedChangedCommandProperty =
            DependencyProperty.Register("CheckedChangedCommand", typeof(ICommand), typeof(AutoReplyOptionControl));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(AutoReplyOptionControl),
                new FrameworkPropertyMetadata());

        public AutoReplyOptionControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }


        public AutoReplyOptionModel AutoReplyOptionModel
        {
            get => (AutoReplyOptionModel) GetValue(UnfriendOptionProperty);
            set => SetValue(UnfriendOptionProperty, value);
        }


        public ICommand CheckedChangedCommand
        {
            get => (ICommand) GetValue(CheckedChangedCommandProperty);
            set => SetValue(CheckedChangedCommandProperty, value);
        }


        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}