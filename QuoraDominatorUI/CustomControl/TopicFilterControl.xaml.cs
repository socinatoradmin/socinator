using QuoraDominatorCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuoraDominatorUI.CustomControl
{
    /// <summary>
    /// Interaction logic for TopicFilterControl.xaml
    /// </summary>
    public partial class TopicFilterControl : UserControl
    {
        public TopicFilterControl()
        {
            InitializeComponent();
            TopicFilter = new TopicFilterModel { SaveCloseButtonVisible = true };
            MainGrid.DataContext = this;
        }
        public static readonly DependencyProperty EnableAnswerRangeFilter =
            DependencyProperty.Register("IsEnableAnswerFilter", typeof(Visibility), typeof(TopicFilterControl),new FrameworkPropertyMetadata());
        public static readonly DependencyProperty TopicFilterProperty =
            DependencyProperty.Register("TopicFilter", typeof(TopicFilterModel), typeof(TopicFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        private static readonly RoutedEvent SaveTopicFilterEvent =
            EventManager.RegisterRoutedEvent("SaveQuestionFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(TopicFilterControl));
        public TopicFilterModel TopicFilter
        {
            get => (TopicFilterModel)GetValue(TopicFilterProperty);
            set => SetValue(TopicFilterProperty, value);
        }
        public Visibility IsEnableAnswerFilter
        {
            get => (Visibility)GetValue(EnableAnswerRangeFilter);
            set => SetValue(EnableAnswerRangeFilter, value);
        }
        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public event RoutedEventHandler SaveTopicFilterEventHandler
        {
            add => AddHandler(SaveTopicFilterEvent, value);
            remove => RemoveHandler(SaveTopicFilterEvent, value);
        }

        private void SaveAnswerFilterEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveTopicFilterEvent);
            RaiseEvent(rountedargs);
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveAnswerFilterEventArgsHandler();
        }
    }
}
