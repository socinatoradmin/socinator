using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using System.Linq;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.AutoFeedActivity
{
    public class RedditPostAutoActivityBase : ModuleSettingsUserControl<PostAutoActivityViewModel, PostAutoActivityModel>
    {
        private readonly IGenericFileManager _genericFileManager;
        private readonly ICampaignsFileManager campaignFileManager;
        public RedditPostAutoActivityBase()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
        }
        protected override bool ValidateCampaign()
        {
            try
            {
                var otherConfig = _genericFileManager.GetModel<RedditOtherConfigModel>(ConstantVariable.GetOtherRedditSettingsFile()) ??
                    new RedditOtherConfigModel();
                var IsSelectedAutoActivity = Model.IsChkUpvote || Model.IsChkDownvote || Model.IsChkUpvoteDownvoteComment || Model.IsChkFollowPostOwner || Model.IsChkJoinPostCommunity || Model.IsCheckVisitAndScroll;
                if (!IsSelectedAutoActivity)
                {
                    Dialog.ShowDialog(this, "Input Error",
                    "Please Select At Least One Auto Activity To Perform");
                    return false;
                }
                if (otherConfig != null && otherConfig.IsEnableFeedActivity)
                {
                    var LstCampaignDetails = Campaigns.GetCampaignsInstance(SocialNetwork).CampaignViewModel
                        .LstCampaignDetails.Where(x => x.Status == "Active").ToList();
                    if (LstCampaignDetails.Count >= otherConfig.MaxThreadCount)
                    {
                        return Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(), "LangKeyRunningOnMaxThreadLimit".FromResourceDictionary(), "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) == MessageDialogResult.Affirmative;
                    }
                    return true;
                }
                else
                {
                    Dialog.ShowDialog(this, "Enable Auto Activity",
                    "Please Enable Auto Activity From Other Configuration To Start Auto Activity.");
                    return false;
                }
            }
            catch { }
            return base.ValidateCampaign();
        }
    }
    /// <summary>
    /// Interaction logic for RedditPostAutoActivity.xaml
    /// </summary>
    public partial class RedditPostAutoActivity : RedditPostAutoActivityBase
    {
        private static RedditPostAutoActivity instance;
        public static RedditPostAutoActivity GetSingletonInstance() => instance ?? (instance = new RedditPostAutoActivity());
        public RedditPostAutoActivity()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AutoActivityHeader,
                footer: AutoActivityFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.AutoActivity,
                moduleName: RdMainModule.AutoFeedActivity.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.AutoActivityKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.AutoActivityVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }
    }
}
