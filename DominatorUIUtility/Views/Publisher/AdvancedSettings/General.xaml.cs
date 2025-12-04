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
    ///     Interaction logic for General.xaml
    /// </summary>
    public partial class General : UserControl, INotifyPropertyChanged
    {
        private static General ObjGeneral;
        private readonly IGenericFileManager _genericFileManager;

        private GeneralViewModel _generalViewModel = new GeneralViewModel();

        public General()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InitializeComponent();
            MainGrid.DataContext = GeneralViewModel;
        }

        public GeneralViewModel GeneralViewModel
        {
            get => _generalViewModel;
            set
            {
                _generalViewModel = value;
                OnPropertyChanged(nameof(GeneralViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static General GetSingeltonGeneralObject()
        {
            if (ObjGeneral == null)
                ObjGeneral = new General();
            return ObjGeneral;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void General_OnLoaded(object sender, RoutedEventArgs e)
        {
            var campaignId = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.CampaignId;

            var generaldata = _genericFileManager.GetModuleDetails<GeneralModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                .FirstOrDefault(x => x.CampaignId == campaignId);

            if (generaldata == null)
            {
                var newGeneralModel = new GeneralModel();
                newGeneralModel.InitializeGeneralModel();
                GeneralViewModel.GeneralModel = newGeneralModel;
            }
            else
            {
                GeneralViewModel.GeneralModel = generaldata;
            }
        }
    }
}