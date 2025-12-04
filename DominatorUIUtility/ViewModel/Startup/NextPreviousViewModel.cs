using System.Windows;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface INextPreviousViewModel
    {
    }

    public class NextPreviousViewModel : DependencyObject, INextPreviousViewModel
    {
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(NextPreviousViewModel));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandParameterProperty =
            DependencyProperty.Register("NextCommandParameter", typeof(string), typeof(NextPreviousViewModel),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged));

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(NextPreviousViewModel));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandParameterProperty =
            DependencyProperty.Register("PreviousCommandParameter", typeof(string), typeof(NextPreviousViewModel),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged));

        public ICommand NextCommand
        {
            get => (ICommand) GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        public string NextCommandParameter
        {
            get => (string) GetValue(NextCommandParameterProperty);
            set => SetValue(NextCommandParameterProperty, value);
        }


        public ICommand PreviousCommand
        {
            get => (ICommand) GetValue(PreviousCommandProperty);
            set => SetValue(PreviousCommandProperty, value);
        }

        public string PreviousCommandParameter
        {
            get => (string) GetValue(PreviousCommandParameterProperty);
            set => SetValue(PreviousCommandParameterProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var newValue = e.NewValue;
        }
    }
}