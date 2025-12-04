using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.Subscribe
{
    public class SubscribeBase : ModuleSettingsUserControl<SubscribeViewModel, SubscribeModel>
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

            // Check AutoSubscribe.UnSubscribe
            if (!Model.IsChkEnableAutoSubscribeUnSubscribeChecked) return base.ValidateCampaign();
            if (Model.IsChkStopSubscribeToolWhenReachedSpecifiedSubscribeings ||
                Model.IsChkWhenSubscribeerSubscribeingsIsSmallerThanChecked) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error",
                "Please select atleast one checkbox option inside AutoSubscribe/UnSubscribe feature to  Stat/Stop UnSubscribe/Subscribe process.");
            return false;
        }
    }


    public partial class Subscribe : SubscribeBase
    {
        public Subscribe()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: SubscribeHeader,
                footer: SubscribeFooter,
                queryControl: SubscribeSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Subscribe,
                moduleName: RdMainModule.Voting.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.SubscribeKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.SubscribeVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Subscribe CurrentSubscribe { get; set; }

        protected sealed override void SetDataContext()
        {
            base.SetDataContext();

            CampaignName = CampaignName;
        }

        public static Subscribe GetSingletonObjectSubscribe()
        {
            return CurrentSubscribe ?? (CurrentSubscribe = new Subscribe());
        }
    }
}