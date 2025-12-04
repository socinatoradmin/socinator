using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherRssFeed.xaml
    /// </summary>
    public partial class PublisherRssFeed : UserControl, INotifyPropertyChanged
    {
        private static PublisherRssFeed _instance;

        private PublisherRssFeedViewModel _publisherRssFeedViewModel = new PublisherRssFeedViewModel();

        private PublisherRssFeed()
        {
            InitializeComponent();
        }

        private PublisherRssFeed(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            PublisherRssFeedViewModel = new PublisherRssFeedViewModel(tabItemsControl);
            tabItemsControl.PublisherRssFeedViewModel = PublisherRssFeedViewModel;
            MainGrid.DataContext = PublisherRssFeedViewModel;
        }

        public PublisherRssFeedViewModel PublisherRssFeedViewModel
        {
            get => _publisherRssFeedViewModel;
            set
            {
                if (_publisherRssFeedViewModel == value)
                    return;
                _publisherRssFeedViewModel = value;
                OnPropertyChanged(nameof(PublisherRssFeedViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherRssFeed GetPublisherRssFeed(
            PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return _instance ?? (_instance = new PublisherRssFeed(tabItemsControl));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}