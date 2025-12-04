using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    ///     Interaction logic for GooglePlus.xaml
    /// </summary>
    public partial class GooglePlus : UserControl, INotifyPropertyChanged
    {
        private static GooglePlus ObJGooglePlus;
        private readonly IGenericFileManager _genericFileManager;
        private GooglePlusViewModel _googlePlusViewModel = new GooglePlusViewModel();

        public GooglePlus()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = GooglePlusViewModel;
        }

        public GooglePlusViewModel GooglePlusViewModel
        {
            get => _googlePlusViewModel;
            set
            {
                _googlePlusViewModel = value;
                OnPropertyChanged(nameof(GooglePlusViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static GooglePlus GetSingeltonGooglePlusObject()
        {
            if (ObJGooglePlus == null)
                ObJGooglePlus = new GooglePlus();
            return ObJGooglePlus;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GooglePlus_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            //var googlePlusModel = _genericFileManager.GetModuleDetails<GooglePlusModel>
            //        (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Gplus))
            //    .FirstOrDefault(x => x.CampaignId == campaignId);
            //GooglePlusViewModel.GooglePlusModel = googlePlusModel ?? (new GooglePlusModel());
        }
    }
}