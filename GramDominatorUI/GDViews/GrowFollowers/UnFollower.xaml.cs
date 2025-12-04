using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.GrowFollower;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.GrowFollowers
{
    public class UnFollowerBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.IsUnfollowFollowings)
            {
                Model.IsChkCustomFollowUsersListChecked = false;
                Model.RemoveAllFollowUsers = false;
            }

            if (Model.IsUnfollowFollowers)
            {
                Model.IsChkPeopleFollowedBySoftwareChecked = false;
                Model.IsChkPeopleFollowedOutsideSoftwareChecked = false;
                Model.IsChkCustomUsersListChecked = false;
            }

            if (Model.IsUnfollowFollowings && !Model.IsChkPeopleFollowedBySoftwareChecked &&
                !Model.IsChkPeopleFollowedOutsideSoftwareChecked &&
                !Model.IsChkCustomUsersListChecked)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one Unfollow source option...");
                return false;
            }

            if (Model.IsUnfollowFollowers && !Model.RemoveAllFollowUsers && !Model.IsChkCustomFollowUsersListChecked)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one Unfollow Remove source option...");
                return false;
            }

            if (Model.IsChkCustomUsersListChecked && string.IsNullOrEmpty(Model.CustomUsersList) ||
                Model.IsChkCustomFollowUsersListChecked && string.IsNullOrEmpty(Model.CustomFollowUsersList))
            {
                Dialog.ShowDialog(this, "Error",
                    "Please enter atleast one custom username");
                return false;
            }

            // Check AutoFollow.Unfollow
            if (Model.IsChkEnableAutoFollowUnfollowChecked)
                if (!Model.IsChkStopUnfollowToolWhenReachedSpecifiedFollowings &&
                    !Model.IsChkWhenFollowerFollowingsGreater)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please select atleast one checkbox option inside AutoFollow/Unfollow feature to  Stat/Stop Unfollow/Follow process.");
                    return false;
                }

            if (Model.IsUserFollowedBeforeChecked && Model.FollowedBeforeDay == 0 && Model.FollowedBeforeHour == 0)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please provide input other than zero for \"Source Filter\" category");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    ///     Interaction logic for UnFollower.xaml
    /// </summary>
    public partial class UnFollower : UnFollowerBase
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
                moduleName: GdMainModule.GrowFollower.ToString()
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