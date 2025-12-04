using FaceDominatorCore.FDModel.FilterModel;
using System.Windows;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for GroupFilterControl.xaml
    /// </summary>
    public partial class GroupFilterControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupFilterProperty =
            DependencyProperty.Register("GroupFilter", typeof(FdGroupFilterModel), typeof(GroupFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsSaveCloseButtonVisisbleProperty =
            DependencyProperty.Register("IsSaveCloseButtonVisisble", typeof(bool), typeof(GroupFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsUnjoinModelProperty =
            DependencyProperty.Register("IsUnjoinModel", typeof(bool), typeof(GroupFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSkipJoinedGroupsRequiredProperty =
            DependencyProperty.Register("IsSkipJoinedGroupsRequired", typeof(bool), typeof(GroupFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSourceTypeRequiredProperty =
            DependencyProperty.Register("IsSourceTypeRequired", typeof(bool), typeof(GroupFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public GroupFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }


        public FdGroupFilterModel GroupFilter
        {
            get => (FdGroupFilterModel)GetValue(GroupFilterProperty);
            set => SetValue(GroupFilterProperty, value);
        }

        public bool IsSaveCloseButtonVisisble
        {
            get => (bool)GetValue(IsSaveCloseButtonVisisbleProperty);
            set => SetValue(IsSaveCloseButtonVisisbleProperty, value);
        }


        public bool IsUnjoinModel
        {
            get => (bool)GetValue(IsUnjoinModelProperty);
            set => SetValue(IsUnjoinModelProperty, value);
        }

        //        void SaveUserFilterEventArgsHandler()
        //        {
        //            var rountedargs = new RoutedEventArgs(SaveGroupFilterEvent);
        //            RaiseEvent(rountedargs);
        //
        //        }

        //        private static readonly RoutedEvent SaveGroupFilterEvent =
        //            EventManager.RegisterRoutedEvent("SaveGroupFilterEventHandler", RoutingStrategy.Bubble,
        //                typeof(RoutedEventHandler), typeof(GroupFilterControl));
        //
        //        public event RoutedEventHandler SaveUserFilterEventHandler
        //        {
        //            add { AddHandler(SaveGroupFilterEvent, value); }
        //            remove { RemoveHandler(SaveGroupFilterEvent, value); }
        //        }


        public bool IsSkipJoinedGroupsRequired
        {
            get => (bool)GetValue(IsSkipJoinedGroupsRequiredProperty);
            set => SetValue(IsSkipJoinedGroupsRequiredProperty, value);
        }

        public bool IsSourceTypeRequired
        {
            get => (bool)GetValue(IsSourceTypeRequiredProperty);
            set => SetValue(IsSourceTypeRequiredProperty, value);
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            //SaveUserFilterEventArgsHandler();
        }
    }
}