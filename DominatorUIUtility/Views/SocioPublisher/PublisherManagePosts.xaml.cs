using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherManagePosts.xaml
    /// </summary>
    public partial class PublisherManagePosts : UserControl, INotifyPropertyChanged
    {
        private static PublisherManagePosts _instance;

        private PublisherManagePostsViewModel _publisherManagePostsViewModel = new PublisherManagePostsViewModel();

        public PublisherManagePosts()
        {
            InitializeComponent();
            ManagePost.DataContext = PublisherManagePostsViewModel;
            PublisherManagePostsViewModel.TabChangeExecute(ConstantVariable.DraftPostList);
        }

        public PublisherManagePostsViewModel PublisherManagePostsViewModel
        {
            get => _publisherManagePostsViewModel;
            set
            {
                if (_publisherManagePostsViewModel == value)
                    return;
                _publisherManagePostsViewModel = value;
                OnPropertyChanged(nameof(PublisherManagePostsViewModel));
            }
        }

        public static PublisherManagePosts Instance { get; set; }
            = _instance ?? (_instance = new PublisherManagePosts());


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}