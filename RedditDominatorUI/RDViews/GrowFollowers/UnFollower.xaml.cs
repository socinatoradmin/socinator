using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;

namespace RedditDominatorUI.RDViews.GrowFollowers
{
    public class UnFollowerBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!Model.IsChkPeopleFollowedBySoftwareChecked && !Model.IsChkPeopleFollowedOutsideSoftwareChecked &&
                !Model.IsChkCustomUsersListChecked)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one Unfollow source option...");
                return false;
            }

            if (Model.IsChkCustomUsersListChecked && string.IsNullOrEmpty(Model.CustomUsersList?.Trim()))
            {
                Dialog.ShowDialog(this, "Error",
                    "Please enter and save atleast one custom username");
                return false;
            }

            // Check AutoFollow.Unfollow
            if (!Model.IsChkEnableAutoFollowUnfollowChecked) return base.ValidateCampaign();
            if (Model.IsChkStopUnfollowToolWhenReachedSpecifiedFollowings || Model.IsChkWhenFollowerFollowingsGreater)
                return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error",
                "Please select atleast one checkbox option inside AutoFollow/Unfollow feature to  Stat/Stop Unfollow/Follow process.");
            return false;
        }
    }


    /// <summary>
    ///     Interaction logic for UnFollower.xaml
    /// </summary>
    public sealed partial class UnFollower : UnFollowerBase
    {
        private UnFollower()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: UnfollowHeader,
                footer: UnFollowFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.Unfollow,
                moduleName: Enums.RdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.UnFollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static UnFollower CurrentUnFollower { get; set; }

        public static UnFollower GetSingeltonObjectUnfollower()
        {
            return CurrentUnFollower ?? (CurrentUnFollower = new UnFollower());
        }
    }
}