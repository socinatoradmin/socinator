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
        SendGreetingsToFriendsBase : ModuleSettingsUserControl<SendGreetingsToFriendsViewModel,
            SendGreetingsToFriendsModel>
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

            if (Model.IsFilterByDays && Model.DaysBefore.StartValue == 0 && Model.DaysBefore.EndValue == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidDaysFilter".FromResourceDictionary());
                return false;
            }

            if (Model.IsPostToOwnWallChecked && Model.ListPostDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOnePostDescription".FromResourceDictionary());
                return false;
            }


            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for MessageRecentFriends.xaml
    /// </summary>
    public partial class SendGreetingsToFriends
    {
        public SendGreetingsToFriends()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: SendRequestHeader,
                footer: SendRequestFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.SendGreetingsToFriends,
                moduleName: FdMainModule.Messanger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.SendGreetingsToFriendVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.SendGreetingsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static SendGreetingsToFriends CurrentSendGreetingsToFriends { get; set; }

        public static SendGreetingsToFriends GetSingeltonObjectSendGreetingsToFriends()
        {
            return CurrentSendGreetingsToFriends ?? (CurrentSendGreetingsToFriends = new SendGreetingsToFriends());
        }
    }
}