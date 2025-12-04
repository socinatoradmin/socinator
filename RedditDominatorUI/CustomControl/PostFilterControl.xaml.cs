using RedditDominatorCore.RDModel;
using System.Windows;

namespace RedditDominatorUI.CustomControl
{
    public partial class PostFilterControl
    {
        // Using a DependencyProperty as the backing store for PostFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostFilterProperty =
            DependencyProperty.Register("PostFilter", typeof(PostFilterModel), typeof(PostFilterControl),
                new PropertyMetadata(OnAvailableItemsChanged));

        public PostFilterControl()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
        }

        public PostFilterModel PostFilter
        {
            get => (PostFilterModel)GetValue(PostFilterProperty);
            set => SetValue(PostFilterProperty, value);
        }


        public static void OnAvailableItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // ReSharper disable once UnusedVariable
            var newValue = e.NewValue;
        }
    }
}