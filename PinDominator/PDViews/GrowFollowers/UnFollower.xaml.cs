using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.GrowFollower;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.GrowFollowers
{
    public class UnFollowerBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.IsChkPeopleFollowedBySoftwareChecked && !Model.IsChkPeopleFollowedOutsideSoftwareChecked &&
                !Model.IsChkCustomUsersListChecked)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one unfollow source");
                return false;
            }

            if (Model.IsChkCustomUsersListChecked && string.IsNullOrEmpty(Model.CustomUsersList))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyCustomUserListEmpty".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for UnFollower.xaml
    /// </summary>
    public sealed partial class UnFollower
    {
        /// Constructor
        private UnFollower()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: UnFollowHeader,
                footer: UnFollowFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.Unfollow,
                moduleName: PdMainModule.GrowFollower.ToString()
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
        ///     GetSingeltonObjectUnFollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UnFollower GetSingeltonObjectUnFollower()
        {
            return CurrentUnFollower ?? (CurrentUnFollower = new UnFollower());
        }
    }
}