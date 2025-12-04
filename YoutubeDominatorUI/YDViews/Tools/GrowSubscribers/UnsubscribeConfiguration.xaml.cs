using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;
using YoutubeDominatorCore.YoutubeViewModel.GrowSubscribers_ViewModel;

namespace YoutubeDominatorUI.YDViews.Tools.GrowSubscribers
{
    public class UnsubscribeConfigurationBase : ModuleSettingsUserControl<UnsubscribeViewModel, UnsubscribeModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.IsChkChannelSubscribedBySoftwareChecked && !Model.IsChkChannelSubscribedOutsideSoftwareChecked
                                                               && !Model.IsChkCustomChannelsListChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneUnsubscribeSource".FromResourceDictionary());
                return false;
            }

            if (Model.IsChkCustomChannelsListChecked &&
                (string.IsNullOrWhiteSpace(Model.CustomChannelsList) || Model.ListCustomChannels?.Count == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyCustomUserListEmpty".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UnsubscribeModel =
                        JsonConvert.DeserializeObject<UnsubscribeModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new UnsubscribeViewModel();

                ObjViewModel.UnsubscribeModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for EngageConfiguration.xaml
    /// </summary>
    public partial class UnsubscribeConfiguration
    {
        private static UnsubscribeConfiguration _currentUnsubscribeConfiguration;

        public UnsubscribeConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UnSubscribe,
                Enums.YdMainModule.UnSubscribe.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            //queryControl: UnsubscribeSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UnsubscribeVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UnsubscribeKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            AccountGrowthHeader.AccountItemSource =
                new ObservableCollectionBase<string>(accountsFileManager.GetUsers());
            MainGrid.DataContext = ObjViewModel.UnsubscribeModel;
        }

        public static UnsubscribeConfiguration GetSingeltonObjectUnsubscribeConfiguration()
        {
            return _currentUnsubscribeConfiguration ??
                   (_currentUnsubscribeConfiguration = new UnsubscribeConfiguration());
        }
    }
}