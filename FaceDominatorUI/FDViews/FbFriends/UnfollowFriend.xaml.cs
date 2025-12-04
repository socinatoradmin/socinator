using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FDViewModel.FriendsViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbFriends
{
    /// <summary>
    /// Interaction logic for UnfollowFriend.xaml
    /// </summary>

    public class UnfollowFriendBase : ModuleSettingsUserControl<UnfollowFriendViewModel, UnfollowFriendModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!Model.UnfriendOptionModel.IsAddedThroughSoftware && !Model.UnfriendOptionModel.IsAddedOutsideSoftware
                                                                  && !Model.UnfriendOptionModel.IsCustomUserList &&
                                                                  !Model.UnfriendOptionModel.IsMutualFriends)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyselectAtLeastOneSource".FromResourceDictionary());
                return false;
            }

            if (Model.UnfriendOptionModel.IsFilterApplied &&
                (Model.UnfriendOptionModel.DaysBefore == 0 && Model.UnfriendOptionModel.HoursBefore == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidSourceFilter".FromResourceDictionary());
                return false;
            }

            if (Model.UnfriendOptionModel.IsCustomUserList &&
                (Model.UnfriendOptionModel.LstCustomUsers == null ||
                 Model.UnfriendOptionModel.LstCustomUsers.Count == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterTheUserUrls".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsLocationFilterChecked
                && (Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
                    && Model.GenderAndLocationFilter.ListLocationUrlPair.Count == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectLocationAndSave".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public partial class UnfollowFriend
    {
        public UnfollowFriend()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UnfollowFriendHeader,
                footer: UnfollowFriendFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.Unfollow,
                moduleName: FdMainModule.Friends.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.UnfriendVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.UnfriendKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UnfollowFriend CurrentUnFollowFriend { get; set; }

        public static UnfollowFriend GetSingeltonObjectUnfollow()
        {
            return CurrentUnFollowFriend ?? (CurrentUnFollowFriend = new UnfollowFriend());
        }

    }
}
