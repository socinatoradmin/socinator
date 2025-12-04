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
    public class FollowerBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for Follower.xaml
    /// </summary>
    public partial class Follower
    {
        /// Constructor
        private Follower()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderControl,
                footer: FollowFooter,
                queryControl: FollowerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Follow,
                moduleName: Enums.TmbMainModule.GrowFollower.ToString()
            );


            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.FollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            ;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Follower CurrentFollower { get; set; }

        /// <summary>
        ///     GetSingeltonObjectFollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Follower GetSingeltonObjectFollower()
        {
            return CurrentFollower ?? (CurrentFollower = new Follower());
        }

        protected sealed override void SetDataContext()
        {
            base.SetDataContext();

            // NOTE: Fill-in with testing data to SKIP entering each time on testing. 
            // Will be disabled in Debug and Release configs
        }
    }
}