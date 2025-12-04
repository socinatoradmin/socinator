using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.AccountSetting.CustomControl
{
    /// <summary>
    ///     Interaction logic for NextPreviousButton.xaml
    /// </summary>
    public partial class NextPreviousButton : UserControl
    {
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register("NextCommand", typeof(ICommand), typeof(NextPreviousButton));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextCommandParameterProperty =
            DependencyProperty.Register("NextCommandParameter", typeof(string), typeof(NextPreviousButton),
                new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register("PreviousCommand", typeof(ICommand), typeof(NextPreviousButton));

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousCommandParameterProperty =
            DependencyProperty.Register("PreviousCommandParameter", typeof(string), typeof(NextPreviousButton),
                new PropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for PreviousVisiblity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviousVisiblityProperty =
            DependencyProperty.Register("PreviousVisiblity", typeof(Visibility), typeof(NextPreviousButton),
                new PropertyMetadata(Visibility.Visible));

        // Using a DependencyProperty as the backing store for NextButtonContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextButtonContentProperty =
            DependencyProperty.Register("NextButtonContent", typeof(string), typeof(NextPreviousButton),
                new PropertyMetadata("LangKeyNext".FromResourceDictionary()));

        public NextPreviousButton()
        {
            InitializeComponent();
            ButtonGrid.DataContext = this;
        }

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


        public Visibility PreviousVisiblity
        {
            get => (Visibility) GetValue(PreviousVisiblityProperty);
            set => SetValue(PreviousVisiblityProperty, value);
        }


        public string NextButtonContent
        {
            get => (string) GetValue(NextButtonContentProperty);
            set => SetValue(NextButtonContentProperty, value);
        }
    }
}