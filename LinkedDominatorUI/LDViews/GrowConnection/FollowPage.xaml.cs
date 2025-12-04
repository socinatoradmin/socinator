using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.GrowConnection
{
    public class FollowPageBase : ModuleSettingsUserControl<FollowPagesViewModel, FollowPagesModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!ValidateQuery())
                return false;

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for FollowPage.xaml
    /// </summary>
    public partial class FollowPage
    {
        private FollowPage()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: FollowHeader,
                footer: FollowFooter,
                queryControl: FollowSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.FollowPages,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.ConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }


        private static FollowPage CurrentFollowPage { get; set; }

        public static FollowPage GetSingeltonObjectFollowPages()
        {
            return CurrentFollowPage ?? (CurrentFollowPage = new FollowPage());
        }
    }
}