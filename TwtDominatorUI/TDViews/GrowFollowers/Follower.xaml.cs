using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.GrowFollower;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.GrowFollowers
{
    public class FollowerBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }


    /// <summary>
    ///     Interaction logic for Follower.xaml
    /// </summary>
    public partial class Follower : FollowerBase
    {
        private Follower()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: FollowHeader,
                footer: FollowFooter,
                queryControl: FollowerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Follow,
                moduleName: TdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.FollowVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.FollowKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region SingletonObject Creation

        private static Follower objFollower { get; set; }

        public static Follower GetSingletonObjectFollower()
        {
            return objFollower ?? (objFollower = new Follower());
        }

        #endregion
    }
}