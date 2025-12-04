using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinMessenger;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinMessenger
{
    public class SendMessageToNewFollowersBase : ModuleSettingsUserControl<SendMessageToNewFollowersViewModel,
        SendMessageToNewFollowersModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.LstMessages.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one message.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for SendMessageToNewFollowers.xaml
    /// </summary>
    public sealed partial class SendMessageToNewFollowers
    {
        public SendMessageToNewFollowers()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: SendMessageToNewFollowersHeader,
                footer: SendMessageToNewFollowersFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.SendMessageToFollower,
                moduleName: PdMainModule.PinMessenger.ToString()
            );
            VideoTutorialLink = ConstantHelpDetails.SendMessageToNewFollowersVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendMessageToNewFollowersKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static SendMessageToNewFollowers CurrentSendMessageToNewFollowers { get; set; }

        public static SendMessageToNewFollowers GetSingletonObjectSendMessageToNewFollowers()
        {
            return CurrentSendMessageToNewFollowers ??
                   (CurrentSendMessageToNewFollowers = new SendMessageToNewFollowers());
        }
    }
}