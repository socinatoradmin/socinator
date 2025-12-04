using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherSharePost.xaml
    /// </summary>
    public partial class PublisherSharePost : UserControl, INotifyPropertyChanged
    {
        private static PublisherSharePost _instance;
        private PublisherSharePostViewModel _publisherSharePostViewModel;

        private PublisherSharePost()
        {
            InitializeComponent();
        }

        public PublisherSharePost(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            PublisherSharePostViewModel = new PublisherSharePostViewModel(tabItemsControl);
            tabItemsControl.PublisherSharePostViewModel = PublisherSharePostViewModel;
            MainGrid.DataContext = PublisherSharePostViewModel;
        }

        public PublisherSharePostViewModel PublisherSharePostViewModel
        {
            get => _publisherSharePostViewModel;
            set
            {
                if (_publisherSharePostViewModel == value)
                    return;
                _publisherSharePostViewModel = value;
                OnPropertyChanged(nameof(PublisherSharePostViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherSharePost GetPublisherSharePost(
            PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return _instance ?? (_instance = new PublisherSharePost(tabItemsControl));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}