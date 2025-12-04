using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherCreateCampaigns.xaml
    /// </summary>
    public partial class PublisherCreateCampaigns : UserControl, INotifyPropertyChanged
    {
        private static PublisherCreateCampaigns _currentObject;
        private PublisherCreateCampaignViewModel _publisherCreateCampaignViewModel;

        private PublisherCreateCampaigns()
        {
            InitializeComponent();
            PublisherCreateCampaignViewModel = InstanceProvider.GetInstance<PublisherCreateCampaignViewModel>();
            CreateCampaign.DataContext = PublisherCreateCampaignViewModel;
        }

        public PublisherCreateCampaignViewModel PublisherCreateCampaignViewModel
        {
            get => _publisherCreateCampaignViewModel;
            set
            {
                if (_publisherCreateCampaignViewModel == value)
                    return;
                _publisherCreateCampaignViewModel = value;
                OnPropertyChanged(nameof(PublisherCreateCampaignViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherCreateCampaigns GetSingeltonPublisherCreateCampaigns()
        {
            return _currentObject ?? (_currentObject = new PublisherCreateCampaigns());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ToggleCampaignStatus_OnClick(object sender, RoutedEventArgs e)
        {
            var onLabel = ToggleCampaignStatus.OnLabel;
            switch (onLabel)
            {
                case "Completed":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus =
                        PublisherCampaignStatus.Active;
                    PublishScheduler.ScheduleTodaysPublisherByCampaign(PublisherCreateCampaignViewModel
                        .PublisherCreateCampaignModel.CampaignId);
                    PublisherInitialize.GetInstance.UpdateCampaignStatus(
                        PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignId,
                        PublisherCampaignStatus.Active);
                    break;
                case "Active":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus =
                        PublisherCampaignStatus.Paused;
                    PublishScheduler.StopPublishingPosts(PublisherCreateCampaignViewModel.PublisherCreateCampaignModel
                        .CampaignId);
                    PublisherInitialize.GetInstance.UpdateCampaignStatus(
                        PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignId,
                        PublisherCampaignStatus.Paused);
                    break;
                case "Paused":
                    PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignStatus =
                        PublisherCampaignStatus.Completed;
                    PublishScheduler.StopPublishingPosts(PublisherCreateCampaignViewModel.PublisherCreateCampaignModel
                        .CampaignId);
                    PublisherInitialize.GetInstance.UpdateCampaignStatus(
                        PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.CampaignId,
                        PublisherCampaignStatus.Completed);
                    break;
            }
        }
    }
}