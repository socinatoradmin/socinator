using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherManagePostPublished.xaml
    /// </summary>
    public partial class PublisherManagePostPublished : UserControl, INotifyPropertyChanged
    {
        private static PublisherManagePostPublished _publisherManagePostPublished;

        private PublisherManagePostPublishedViewModel _publisherManagePostPublishedViewModel =
            new PublisherManagePostPublishedViewModel();

        private PublisherManagePostPublished()
        {
            InitializeComponent();
            PublishedPostList.DataContext = PublisherManagePostPublishedViewModel;
        }

        public PublisherManagePostPublishedViewModel PublisherManagePostPublishedViewModel
        {
            get => _publisherManagePostPublishedViewModel;
            set
            {
                if (_publisherManagePostPublishedViewModel == value)
                    return;
                _publisherManagePostPublishedViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostPublishedViewModel));
            }
        }

        //private static PublisherManagePostPublished _instance;


        //public static PublisherManagePostPublished Instance { get; set; }
        //    = _instance ?? (_instance = new PublisherManagePostPublished());

        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherManagePostPublished GetPublisherManagePostPublished()
        {
            return _publisherManagePostPublished ??
                   (_publisherManagePostPublished = new PublisherManagePostPublished());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}