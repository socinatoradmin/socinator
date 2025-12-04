using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherMonitorFolder.xaml
    /// </summary>
    public partial class PublisherMonitorFolder : UserControl, INotifyPropertyChanged
    {
        private static PublisherMonitorFolder _instance;

        private PublisherMonitorFolderViewModel _publisherMonitorFolderViewModel;

        private PublisherMonitorFolder()
        {
            InitializeComponent();
        }

        public PublisherMonitorFolder(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            PublisherMonitorFolderViewModel = new PublisherMonitorFolderViewModel(tabItemsControl);
            tabItemsControl.PublisherMonitorFolderViewModel = PublisherMonitorFolderViewModel;
            MainGrid.DataContext = PublisherMonitorFolderViewModel;
        }

        public PublisherMonitorFolderViewModel PublisherMonitorFolderViewModel
        {
            get => _publisherMonitorFolderViewModel;
            set
            {
                if (_publisherMonitorFolderViewModel == value)
                    return;
                _publisherMonitorFolderViewModel = value;
                OnPropertyChanged(nameof(PublisherMonitorFolderViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherMonitorFolder GetPublisherMonitorFolder(
            PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return _instance ?? (_instance = new PublisherMonitorFolder(tabItemsControl));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}