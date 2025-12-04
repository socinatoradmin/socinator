using RedditDominatorCore.RDModel;
using System.Windows;

namespace RedditDominatorUI.CustomControl
{
    public partial class UserFiltersControl
    {
        // Using a DependencyProperty as the backing store for UserFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserFilterProperty =
            DependencyProperty.Register("UserFilter", typeof(UserFilterModel), typeof(UserFiltersControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });


        private static readonly RoutedEvent SaveUserFilterEvent =
            EventManager.RegisterRoutedEvent("SaveUserFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(UserFiltersControl));

        public UserFiltersControl()
        {
            InitializeComponent();
            if (UserFilter == null)
                UserFilter = new UserFilterModel();
            MainGrid.DataContext = this;
        }


        public UserFilterModel UserFilter
        {
            get => (UserFilterModel)GetValue(UserFilterProperty);
            set => SetValue(UserFilterProperty, value);
        }

        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}