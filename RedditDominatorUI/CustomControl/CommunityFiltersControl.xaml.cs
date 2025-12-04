using RedditDominatorCore.RDModel;
using System.Windows;

namespace RedditDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for CommunityFiltersControl.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class CommunityFiltersControl
    {
        //  Using a DependencyProperty as the backing store for Community Filter.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommunityFiltersProperty =
            DependencyProperty.Register("CommunityFilter", typeof(CommunityFiltersModel),
                typeof(CommunityFiltersControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });


        private static readonly RoutedEvent SaveCommunityFilterEvent =
            EventManager.RegisterRoutedEvent("SaveCommunityFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(CommunityFiltersControl));

        public CommunityFiltersControl()
        {
            InitializeComponent();
            if (CommunityFilter == null)
                CommunityFilter = new CommunityFiltersModel();
            MainGrid.DataContext = this;
        }


        public CommunityFiltersModel CommunityFilter
        {
            get => (CommunityFiltersModel)GetValue(CommunityFiltersProperty);
            set => SetValue(CommunityFiltersProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // ReSharper disable once UnusedVariable
            var newValue = e.NewValue;
        }

        // ReSharper disable once UnusedMember.Global
        public event RoutedEventHandler SaveUserFilterEventHandler
        {
            add => AddHandler(SaveCommunityFilterEvent, value);
            remove => RemoveHandler(SaveCommunityFilterEvent, value);
        }


        private void SaveUserFilterEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveCommunityFilterEvent);
            RaiseEvent(rountedargs);
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void SaveButton_OnClick(object sender)
        {
            SaveUserFilterEventArgsHandler();
        }
    }
}