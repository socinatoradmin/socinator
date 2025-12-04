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
    ///     Interaction logic for Pinterest.xaml
    /// </summary>
    public partial class Pinterest : UserControl
    {
        private static Pinterest ObjPinterest;
        private readonly IGenericFileManager _genericFileManager;
        private PinterestViewModel _pinterestViewModel = new PinterestViewModel();

        public Pinterest()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = PinterestViewModel;
        }

        public PinterestViewModel PinterestViewModel
        {
            get => _pinterestViewModel;
            set
            {
                _pinterestViewModel = value;
                OnPropertyChanged(nameof(PinterestViewModel));
            }
        }

        public static Pinterest GetSingeltonPinterestObject()
        {
            if (ObjPinterest == null)
                ObjPinterest = new Pinterest();
            return ObjPinterest;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Pinterest_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;
            var pinterestModel = _genericFileManager.GetModuleDetails<PinterestModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest))
                .FirstOrDefault(x => x.CampaignId == campaignId);
            PinterestViewModel.PinterestModel = pinterestModel ?? new PinterestModel();
        }
    }
}