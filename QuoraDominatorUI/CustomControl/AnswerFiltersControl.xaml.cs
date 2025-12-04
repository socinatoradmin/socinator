using System.Windows;
using QuoraDominatorCore.Models;

namespace QuoraDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for AnswerFiltersControl.xaml
    /// </summary>
    public partial class AnswerFiltersControl
    {
        // Using a DependencyProperty as the backing store for AnswerFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnswerFilterProperty =
            DependencyProperty.Register("AnswerFilter", typeof(AnswerFilterModel), typeof(AnswerFiltersControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        private static readonly RoutedEvent SaveAnswerFilterEvent =
            EventManager.RegisterRoutedEvent("SaveAnswerFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(AnswerFiltersControl));

        public AnswerFiltersControl()
        {
            InitializeComponent();
            AnswerFilter = new AnswerFilterModel();
            MainGrid.DataContext = this;
        }

        public AnswerFilterModel AnswerFilter
        {
            get => (AnswerFilterModel) GetValue(AnswerFilterProperty);
            set => SetValue(AnswerFilterProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public event RoutedEventHandler SaveAnswerFilterEventHandler
        {
            add => AddHandler(SaveAnswerFilterEvent, value);
            remove => RemoveHandler(SaveAnswerFilterEvent, value);
        }

        private void SaveAnswerFilterEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveAnswerFilterEvent);
            RaiseEvent(rountedargs);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveAnswerFilterEventArgsHandler();
        }
    }
}