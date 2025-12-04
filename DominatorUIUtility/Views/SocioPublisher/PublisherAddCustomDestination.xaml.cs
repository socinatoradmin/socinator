using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherAddCustomDestination.xaml
    /// </summary>
    public partial class PublisherAddCustomDestination : UserControl, INotifyPropertyChanged
    {
        private static PublisherAddCustomDestination _publisherAddCustomDestination;


        private PublisherCustomDestinationViewModel _publisherCustomDestinationViewModel =
            new PublisherCustomDestinationViewModel();

        private PublisherAddCustomDestination()
        {
            InitializeComponent();
            CustomDestination.DataContext = PublisherCustomDestinationViewModel;
        }

        public PublisherCustomDestinationViewModel PublisherCustomDestinationViewModel
        {
            get => _publisherCustomDestinationViewModel;
            set
            {
                if (_publisherCustomDestinationViewModel == value)
                    return;
                _publisherCustomDestinationViewModel = value;
                OnPropertyChanged(nameof(PublisherCustomDestinationViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public static PublisherAddCustomDestination GetPublisherAddCustomDestination(
            ObservableCollection<PublisherCustomDestinationModel> alreadySavedDestination)
        {
            if (_publisherAddCustomDestination == null)
                _publisherAddCustomDestination = new PublisherAddCustomDestination();

            _publisherAddCustomDestination.PublisherCustomDestinationViewModel.LstCustomDestination =
                alreadySavedDestination;

            _publisherAddCustomDestination.CustomDestination.DataContext =
                _publisherAddCustomDestination.PublisherCustomDestinationViewModel;

            return _publisherAddCustomDestination;
        }

        public void ResetCurrectObject()
        {
            _publisherAddCustomDestination = new PublisherAddCustomDestination();
        }


        public ObservableCollection<PublisherCustomDestinationModel> GetSavedCustomDestination()
        {
            return PublisherCustomDestinationViewModel.LstCustomDestination;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}