using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherScrapePost.xaml
    /// </summary>
    public partial class PublisherScrapePost : UserControl, INotifyPropertyChanged
    {
        private static PublisherScrapePost _instance;
        private PublisherScrapePostViewModel _publisherScrapePostViewModel;

        public PublisherScrapePost()
        {
            InitializeComponent();
        }

        public PublisherScrapePost(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            PublisherScrapePostViewModel = new PublisherScrapePostViewModel(tabItemsControl);
            tabItemsControl.PublisherScrapePostViewModel = PublisherScrapePostViewModel;
            MainGrid.DataContext = PublisherScrapePostViewModel;
        }

        public PublisherScrapePostViewModel PublisherScrapePostViewModel
        {
            get => _publisherScrapePostViewModel;
            set
            {
                if (_publisherScrapePostViewModel == value)
                    return;
                _publisherScrapePostViewModel = value;
                OnPropertyChanged(nameof(PublisherScrapePostViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherScrapePost GetPublisherScrapePost(
            PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return _instance ?? (_instance = new PublisherScrapePost(tabItemsControl));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}