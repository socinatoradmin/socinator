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
using DominatorHouseCore.ViewModel.AdvancedSettings;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.Views.Publisher.AdvancedSettings
{
    /// <summary>
    ///     Interaction logic for Instagram.xaml
    /// </summary>
    public partial class Instagram : UserControl, INotifyPropertyChanged
    {
        private static Instagram ObjInstagram;
        private readonly IGenericFileManager _genericFileManager;
        private InstagramViewModel _instagramViewModel = new InstagramViewModel();

        public Instagram()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = InstagramViewModel;
        }

        public InstagramViewModel InstagramViewModel
        {
            get => _instagramViewModel;
            set
            {
                _instagramViewModel = value;
                OnPropertyChanged(nameof(InstagramViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static Instagram GetSingeltonInstagramObject()
        {
            if (ObjInstagram == null)
                ObjInstagram = new Instagram();
            return ObjInstagram;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void Instagram_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var instagramModel = _genericFileManager.GetModuleDetails<InstagramModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            InstagramViewModel.InstagramModel = instagramModel ?? new InstagramModel();
        }
    }
}