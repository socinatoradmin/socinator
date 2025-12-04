using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherDefaultPage.xaml
    /// </summary>
    public partial class PublisherDefaultPage : UserControl, INotifyPropertyChanged
    {
        private static PublisherDefaultPage _indexPage;

        private PublisherDefaultPage()
        {
            InitializeComponent();
            PublisherDefault.DataContext = PublisherDefaultViewModel;
        }

        public PublisherDefaultViewModel PublisherDefaultViewModel { get; } =
            InstanceProvider.GetInstance<PublisherDefaultViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherDefaultPage Instance()
        {
            if (_indexPage == null) _indexPage = new PublisherDefaultPage();
            _indexPage.PublisherDefaultViewModel.InitializeDefaultCampaignStatus();
            return _indexPage;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}