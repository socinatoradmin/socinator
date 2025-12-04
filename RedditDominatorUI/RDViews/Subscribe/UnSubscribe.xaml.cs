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
    public class UnSubscribeBase : ModuleSettingsUserControl<UnSubscribeViewModel, UnSubscribeModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!Model.IsChkCommunitySubscribedBySoftwareChecked && !Model
                                                                     .IsChkCommunitySubscribedOutsideSoftwareChecked
                                                                 && !Model.IsChkCustomCommunityListChecked)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one UnSubscribe source option...");
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

    public partial class UnSubscribe : UnSubscribeBase
    {
        public UnSubscribe()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UnSubscribeHeader,
                footer: UnSubscribeFooter,
                //queryControl: UnSubscribeSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UnSubscribe,
                moduleName: RdMainModule.GrowUnSubscribe.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.UnSubscribeKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.UnSubscribeVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UnSubscribe CurrentUnSubscribe { get; set; }

        protected sealed override void SetDataContext()
        {
            base.SetDataContext();

            CampaignName = CampaignName;
        }

        public static UnSubscribe GetSingletonObjectUnSubscribe()
        {
            return CurrentUnSubscribe ?? (CurrentUnSubscribe = new UnSubscribe());
        }
    }
}