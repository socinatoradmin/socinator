using FaceDominatorCore.FDModel.FilterModel;
using System.Windows;

namespace FaceDominatorUI.CustomControl
{
    /// <summary>
    ///     Interaction logic for CommentFilterControl.xaml
    /// </summary>
    public partial class CommentFilterControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FdCommentFilterProperty =
            DependencyProperty.Register("CommentFilter", typeof(FdCommentFilterModel), typeof(CommentFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        public static readonly DependencyProperty IsSaveCloseButtonVisisbleProperty =
            DependencyProperty.Register("IsSaveCloseButtonVisisble", typeof(bool), typeof(CommentFilterControl),
                new FrameworkPropertyMetadata(OnAvailableItemsChanged)
                {
                    BindsTwoWayByDefault = true
                });

        private static readonly RoutedEvent SaveFanpageFilterEvent =
            EventManager.RegisterRoutedEvent("SaveCommentFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(CommentFilterControl));

        public CommentFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public FdCommentFilterModel CommentFilter
        {
            get => (FdCommentFilterModel)GetValue(FdCommentFilterProperty);
            set => SetValue(FdCommentFilterProperty, value);
        }

        public bool IsSaveCloseButtonVisisble
        {
            get => (bool)GetValue(IsSaveCloseButtonVisisbleProperty);
            set => SetValue(IsSaveCloseButtonVisisbleProperty, value);
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveUserFilterEventArgsHandler();
        }

        private void SaveUserFilterEventArgsHandler()
        {
            var rountedargs = new RoutedEventArgs(SaveFanpageFilterEvent);
            RaiseEvent(rountedargs);
        }
    }
}