using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbMessanger
{
    public class
        MessageRecentFriendsBase : ModuleSettingsUserControl<MessageRecentFriendsViewModel, MessageRecentFriendsModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneMessage".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsLocationFilterChecked
                && Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
                && Model.GenderAndLocationFilter.ListLocationUrlPair.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectLocationAndSave".FromResourceDictionary());
                return false;
            }

            if (Model.DaysBefore.StartValue == 0 && Model.DaysBefore.EndValue == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidSourceFilter".FromResourceDictionary());
                return false;
            }


            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for MessageRecentFriends.xaml
    /// </summary>
    public partial class MessageRecentFriends
    {
        public MessageRecentFriends()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: SendRequestHeader,
                footer: SendRequestFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.SendMessageToNewFriends,
                moduleName: FdMainModule.Messanger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.SendMessageToNewFriendVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.MessageToNewFriendsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static MessageRecentFriends CurrentMessageRecentFriends { get; set; }

        public static MessageRecentFriends GetSingeltonObjectMessageRecentFriends()
        {
            return CurrentMessageRecentFriends ?? (CurrentMessageRecentFriends = new MessageRecentFriends());
        }
    }
}