using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.GrowFollower;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.GrowFollowers
{
    public class UnFollowerBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!(Model.Unfollower.IsChkPeopleFollowedBySoftwareCheecked ||
                  Model.Unfollower.IsChkPeopleFollowedOutsideSoftwareChecked ||
                  Model.Unfollower.IsChkCustomUsersListChecked && !string.IsNullOrEmpty(Model.Unfollower.CustomUsers)))
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one unfollow source");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    ///     Interaction logic for Unfollower.xaml
    /// </summary>
    public partial class Unfollower : UnFollowerBase
    {
        private Unfollower()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: UnfollowHeader,
                footer: UnFollowFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.Unfollow,
                moduleName: TdMainModule.GrowFollower.ToString()
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.UnFollowVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.UnFollowKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region SingletonObject Creation

        private static Unfollower ObjUnfollower { get; set; }

        public static Unfollower GetSingletonObjectUnfollower()
        {
            return ObjUnfollower ?? (ObjUnfollower = new Unfollower());
        }

        #endregion
    }
}