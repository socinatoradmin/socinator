using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.GrowFollower;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.GrowFollowers
{
    public class FollowBackBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
    }

    /// <summary>
    ///     Interaction logic for FollowBack.xaml
    /// </summary>
    public partial class FollowBack : FollowBackBase
    {
        public FollowBack()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: FollowBackHeader,
                footer: FollowBackFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.FollowBack,
                moduleName: TdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.FollowBackVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.FollowBackKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region SingletonObject Creation

        private static FollowBack objFollowBack { get; set; }

        public static FollowBack GetSingletonObjectFollowBack()
        {
            return objFollowBack ?? (objFollowBack = new FollowBack());
        }

        #endregion
    }
}