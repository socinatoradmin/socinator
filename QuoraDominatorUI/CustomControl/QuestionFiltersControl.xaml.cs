using System.Windows;
using QuoraDominatorCore.Models;

namespace QuoraDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for QuestionFiltersControl.xaml
    /// </summary>
    public partial class QuestionFiltersControl
    {
        // Using a DependencyProperty as the backing store for AnswerFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuestionFilterProperty =
            DependencyProperty.Register("QuestionFilter", typeof(QuestionFilterModel), typeof(QuestionFiltersControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        private static readonly RoutedEvent SaveQuestionFilterEvent =
            EventManager.RegisterRoutedEvent("SaveQuestionFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(QuestionFiltersControl));

        public QuestionFiltersControl()
        {
            InitializeComponent();
            QuestionFilter = new QuestionFilterModel {SaveCloseButtonVisible = true};
            MainGrid.DataContext = this;
        }

        public QuestionFilterModel QuestionFilter
        {
            get => (QuestionFilterModel) GetValue(QuestionFilterProperty);
            set => SetValue(QuestionFilterProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public event RoutedEventHandler SaveQuestionFilterEventHandler
        {
            add => AddHandler(SaveQuestionFilterEvent, value);
            remove => RemoveHandler(SaveQuestionFilterEvent, value);
        }

        private void SaveAnswerFilterEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveQuestionFilterEvent);
            RaiseEvent(rountedargs);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveAnswerFilterEventArgsHandler();
        }
    }
}