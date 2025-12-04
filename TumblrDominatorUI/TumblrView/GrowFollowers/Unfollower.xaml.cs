using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.GrowFollower;

namespace TumblrDominatorUI.TumblrView.GrowFollowers
{
    public class UnFollowerBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (!Model.IsChkPeopleFollowedBySoftwareChecked && !Model.IsChkPeopleFollowedOutsideSoftwareChecked &&
                !Model.IsChkCustomUsersListChecked)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one Unfollow source");
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for UnFollower.xaml
    /// </summary>
    public sealed partial class UnFollower
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
                moduleName: Enums.TmbMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UnFollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UnFollower CurrentUnFollower { get; set; }


        /// <summary>
        ///     GetSingeltonObjectUnfollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UnFollower GetSingeltonObjectUnfollower()
        {
            return CurrentUnFollower ?? (CurrentUnFollower = new UnFollower());
        }
    }
}