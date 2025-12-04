using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherCreateDestination.xaml
    /// </summary>
    public partial class PublisherCreateDestination : UserControl, INotifyPropertyChanged
    {
        private static PublisherCreateDestination _indexPage;

        private PublisherCreateDestinationsViewModel _publisherCreateDestinationsViewModel =
            new PublisherCreateDestinationsViewModel();

        private PublisherCreateDestination()
        {
            InitializeComponent();
            PublisherCreateDestinationsViewModel.IsNeedToNavigate = false;
            CreateDestination.DataContext = PublisherCreateDestinationsViewModel;
        }

        public PublisherCreateDestination(bool isNeedToNevigate) : this()
        {
            PublisherCreateDestinationsViewModel.IsNeedToNavigate = isNeedToNevigate;
            CreateDestination.DataContext = PublisherCreateDestinationsViewModel;
            if (isNeedToNevigate)
                BtnBackToCampaign.Visibility = Visibility.Collapsed;
        }

        public PublisherCreateDestinationsViewModel PublisherCreateDestinationsViewModel
        {
            get => _publisherCreateDestinationsViewModel;
            set
            {
                _publisherCreateDestinationsViewModel = value;
                OnPropertyChanged(nameof(PublisherCreateDestinationsViewModel));
            }
        }

        public static PublisherCreateDestination Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherCreateDestination());

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PublisherCreateDestination_OnLoaded(object sender, RoutedEventArgs e)
        {
            PublisherCreateDestinationsViewModel.NetworkSelectionChangedExecute(selectedNetwork.SelectedItem);
            if (!PublisherCreateDestinationsViewModel.IsSavedDestination)
                return;

            if (!string.IsNullOrEmpty(PublisherCreateDestinationsViewModel.EditDestinationId))
            {
                PublisherCreateDestinationsViewModel.EditDestination();
                CreateDestination.DataContext = PublisherCreateDestinationsViewModel;
            }
            else
            {
                PublisherCreateDestinationsViewModel.Title = "LangKeyCreateDestination".FromResourceDictionary();
                PublisherCreateDestinationsViewModel = new PublisherCreateDestinationsViewModel();
                CreateDestination.DataContext = PublisherCreateDestinationsViewModel;
            }
        }
    }
}