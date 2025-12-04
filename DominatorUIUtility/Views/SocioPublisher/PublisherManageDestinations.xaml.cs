using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherManageDestinations.xaml
    /// </summary>
    public partial class PublisherManageDestinations : UserControl, INotifyPropertyChanged
    {
        private static PublisherManageDestinations _indexPage;

        private PublisherManageDestinationViewModel _publisherManageDestinationViewModel =
            new PublisherManageDestinationViewModel();

        private PublisherManageDestinations()
        {
            InitializeComponent();
            ManageDestination.DataContext = PublisherManageDestinationViewModel;
        }

        public PublisherManageDestinationViewModel PublisherManageDestinationViewModel
        {
            get => _publisherManageDestinationViewModel;
            set
            {
                _publisherManageDestinationViewModel = value;
                OnPropertyChanged(nameof(PublisherManageDestinationViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherManageDestinations Instance()
        {
            if (_indexPage == null)
                _indexPage = new PublisherManageDestinations();

            _indexPage.PublisherManageDestinationViewModel.InitializeDefaultDestinations();

            return _indexPage;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var destination = ((FrameworkElement) sender).DataContext as PublisherManageDestinationModel;

            if (destination == null)
                return;

            PublisherCreateDestination.Instance.PublisherCreateDestinationsViewModel.EditDestinationId =
                destination.DestinationId;

            PublisherCreateDestination.Instance.PublisherCreateDestinationsViewModel.IsSavedDestination = true;

            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                = PublisherCreateDestination.Instance;
        }
    }
}