using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for SelectDestinations.xaml
    /// </summary>
    public partial class SelectDestinations : UserControl, INotifyPropertyChanged
    {
        private PublisherManageDestinationViewModel _publisherManageDestinationViewModel =
            new PublisherManageDestinationViewModel();

        public SelectDestinations()
        {
            InitializeComponent();
            ManageDestination.DataContext = PublisherManageDestinationViewModel;
            PublisherManageDestinationViewModel.InitializeDefaultDestinations();
        }

        public SelectDestinations(ObservableCollection<string> lstDestinationId) : this()
        {
            _publisherManageDestinationViewModel.ListPublisherManageDestinationModels?.ToList().ForEach(x =>
            {
                x.IsSelected = lstDestinationId.Contains(x.DestinationId);
            });
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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}