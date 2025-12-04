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
    public class FollowBase : ModuleSettingsUserControl<FollowViewModel, FollowModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please add at least one query.");
                return false;
            }

            // Check AutoFollow.Unfollow
            if (!Model.IsChkEnableAutoFollowUnfollowChecked) return base.ValidateCampaign();
            if (Model.IsChkStopFollowToolWhenReachedSpecifiedFollowings ||
                Model.IsChkWhenFollowerFollowingsIsSmallerThanChecked) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error",
                "Please select atleast one checkbox option inside AutoFollow/Unfollow feature to  Stat/Stop Unfollow/Follow process.");
            return false;
        }
    }


    /// <summary>
    ///     Interaction logic for Follow.xaml
    /// </summary>
    public sealed partial class Follow : FollowBase
    {
        public Follow()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: FollowHeader,
                footer: FollowFooter,
                queryControl: FollowSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Follow,
                moduleName: Enums.RdMainModule.GrowFollower.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.FollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Follow CurrentFollow { get; set; }

        public static Follow GetSingletonObjectFollow()
        {
            return CurrentFollow ?? (CurrentFollow = new Follow());
        }
    }
}