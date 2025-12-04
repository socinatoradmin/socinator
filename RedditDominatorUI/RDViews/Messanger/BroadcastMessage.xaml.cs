using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.Messanger
{
    public class BroadcastMessageBase : ModuleSettingsUserControl<BrodcastMessageViewModel, BrodcastMessageModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.LstManageMessagesModel.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Error", "Please add at least one message.");
            return false;
        }
    }


    /// <summary>
    ///     Interaction logic for BroadCastMessages.xaml
    /// </summary>
    public sealed partial class BroadcastMessage : BroadcastMessageBase
    {
        private static BroadcastMessage _objBroadcastMessages;

        private BroadcastMessage()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: SendMessageHeader,
                footer: BrodcastMessageFooter,
                queryControl: BrodcastMessagesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.BroadcastMessages,
                moduleName: RdMainModule.Messanger.ToString()
            );
            KnowledgeBaseLink = ConstantHelpDetails.MessengerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.MessengerVideoTutorialsLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static BroadcastMessage GetSingeltonBroadcastMessages()
        {
            return _objBroadcastMessages ?? (_objBroadcastMessages = new BroadcastMessage());
        }
    }
}