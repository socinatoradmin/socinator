using System.Threading;
using System.Windows;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorUI.CustomControl
{
    public partial class ChannelFiltersControl
    {
        // Using a DependencyProperty as the backing store for ChannelFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChannelFilterProperty =
            DependencyProperty.Register("ChannelFilter", typeof(ChannelFilterModel), typeof(ChannelFiltersControl),
                new FrameworkPropertyMetadata
                {
                    BindsTwoWayByDefault = true
                });


        private static readonly RoutedEvent SaveChannelFilterEvent =
            EventManager.RegisterRoutedEvent("SaveChannelFilterEventHandler", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ChannelFiltersControl));

        public ChannelFiltersControl()
        {
            InitializeComponent();
            ChannelFilter = new ChannelFilterModel();
            MainGrid.DataContext = this;
        }

        public ChannelFilterModel ChannelFilter
        {
            get => (ChannelFilterModel)GetValue(ChannelFilterProperty);
            set => SetValue(ChannelFilterProperty, value);
        }

        public void LoadUserControl()
        {
            Thread.Sleep(2000);
            Dispatcher.Invoke(() =>
            {
                ChannelFilter = new ChannelFilterModel();
                MainGrid.DataContext = this;
            });
        }


        private void CaptionTitleShouldContainsWordPhrase_OnGetInputClick(object sender, RoutedEventArgs e)
        {
        }

        private void CaptionDescriptionShouldContainsWordPhrase_OnGetInputClick(object sender, RoutedEventArgs e)
        {
        }

        private void CaptionTitleShouldNotContainsWordPhrase_OnGetInputClick(object sender, RoutedEventArgs e)
        {
        }

        private void CaptionDescriptionShouldNotContainsWordsOrPhrasesLike_OnGetInputClick(object sender,
            RoutedEventArgs e)
        {
        }
    }
}