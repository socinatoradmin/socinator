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
    ///     Interaction logic for Reddit.xaml
    /// </summary>
    public partial class Reddit : UserControl
    {
        private static Reddit _objReddit;
        private readonly IGenericFileManager _genericFileManager;

        private RedditViewModel _redditViewModel = new RedditViewModel();

        public Reddit()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = RedditViewModel;
        }

        public RedditViewModel RedditViewModel
        {
            get => _redditViewModel;
            set
            {
                _redditViewModel = value;
                OnPropertyChanged(nameof(RedditViewModel));
            }
        }

        public static Reddit GetSingeltonRedditObject()
        {
            return _objReddit ?? (_objReddit = new Reddit());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Reddit_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var redditModel = _genericFileManager.GetModuleDetails<RedditModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Reddit))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            RedditViewModel.RedditModel = redditModel ?? new RedditModel();
        }
    }
}