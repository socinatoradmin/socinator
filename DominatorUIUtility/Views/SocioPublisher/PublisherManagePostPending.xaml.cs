using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherManagePostPending.xaml
    /// </summary>
    public partial class PublisherManagePostPending : UserControl, INotifyPropertyChanged
    {
        private static PublisherManagePostPending _publisherManagePostPending;

        private PublisherManagePostPendingViewModel _publisherManagePostPendingViewModel =
            new PublisherManagePostPendingViewModel();

        private PublisherManagePostPending()
        {
            InitializeComponent();
            PendingPostLists.DataContext = PublisherManagePostPendingViewModel;
        }

        public PublisherManagePostPendingViewModel PublisherManagePostPendingViewModel
        {
            get => _publisherManagePostPendingViewModel;
            set
            {
                if (_publisherManagePostPendingViewModel == value)
                    return;
                _publisherManagePostPendingViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostPendingViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherManagePostPending GetPublisherManagePostPending()
        {
            return _publisherManagePostPending ?? (_publisherManagePostPending = new PublisherManagePostPending());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}