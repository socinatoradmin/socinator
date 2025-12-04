using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    ///     Interaction logic for Facebook.xaml
    /// </summary>
    public partial class Facebook : UserControl, INotifyPropertyChanged
    {
        private static Facebook ObjFacebook;
        private readonly IGenericFileManager _genericFileManager;


        private FacebookViewModel _facebookViewModel = new FacebookViewModel();

        public Facebook()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = FacebookViewModel;
            FacebookViewModel.FacebookModel.CampaignId = "";
        }

        public FacebookViewModel FacebookViewModel
        {
            get => _facebookViewModel;
            set
            {
                _facebookViewModel = value;
                OnPropertyChanged(nameof(FacebookViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static Facebook GetSingeltonFacebookObject()
        {
            if (ObjFacebook == null)
                ObjFacebook = new Facebook();
            return ObjFacebook;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void Facebook_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var facebookModel = _genericFileManager.GetModuleDetails<FacebookModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook))
                .FirstOrDefault(x => x.CampaignId == campaignId);

            FacebookViewModel.FacebookModel = facebookModel ?? new FacebookModel();

            if (!FacebookViewModel.FacebookModel.IsPostAsPage && !FacebookViewModel.FacebookModel.IsPostAsSamePage)
                FacebookViewModel.FacebookModel.IsPostAsOwnAccount = true;
        }
    }
}