using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.GrowFollower;

namespace QuoraDominatorUI.QDViews.GrowFollowers
{
    public class UnFollowerBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if ((ObjViewModel.UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked ||
                 ObjViewModel.UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked ||
                 ObjViewModel.UnfollowerModel.IsChkCustomUsersListChecked) &&
                (ObjViewModel.UnfollowerModel.IsWhoDoNotFollowBackChecked ||
                 ObjViewModel.UnfollowerModel.IsWhoFollowBackChecked))
                return true;
            Dialog.ShowDialog(this, "Error",
                "Please select one  Unfollow source and one source type");
            return false;
        }

        protected override bool ValidateQuery()
        {
            if ((ObjViewModel.UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked ||
                 ObjViewModel.UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked ||
                 ObjViewModel.UnfollowerModel.IsChkCustomUsersListChecked) &&
                (ObjViewModel.UnfollowerModel.IsWhoDoNotFollowBackChecked ||
                 ObjViewModel.UnfollowerModel.IsWhoFollowBackChecked))
                return true;
            Dialog.ShowDialog(this, "Error",
                "Please select one Unfollow source and one source type");
            return false;
        }
    }

    /// <summary>
    ///     Interaction logic for Unfollow.xaml
    /// </summary>
    public partial class Unfollow
    {
        public Unfollow()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: UnfollowHeader,
                footer: UnFollowFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.Unfollow,
                moduleName: QdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UnFollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static Unfollow CurrentUnFollower { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUnfollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Unfollow GetSingeltonObjectUnfollower()
        {
            return CurrentUnFollower ?? (CurrentUnFollower = new Unfollow());
        }
    }
}