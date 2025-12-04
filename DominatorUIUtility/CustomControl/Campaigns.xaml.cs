using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : INotifyPropertyChanged
    {
        private static Campaigns _instance;
        private CampaignViewModel _campaignViewModel = new CampaignViewModel();

        public Campaigns(SocialNetworks socialNetworks)
        {
            InitializeComponent();
            CampaignViewModel.SocialNetworks = socialNetworks;
            CampaignViewModel.SetActivityTypes();
            Campaign.DataContext = CampaignViewModel;
            CampaignViewModel.CampaignCollection =
                CollectionViewSource.GetDefaultView(CampaignViewModel.LstCampaignDetails);
            _instance = this;
        }

        public CampaignViewModel CampaignViewModel
        {
            get => _campaignViewModel;
            set
            {
                _campaignViewModel = value;
                OnPropertyChanged(nameof(CampaignViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static Campaigns GetCampaignsInstance(SocialNetworks socialNetworks)
        {
            return _instance ?? (_instance = new Campaigns(socialNetworks));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}