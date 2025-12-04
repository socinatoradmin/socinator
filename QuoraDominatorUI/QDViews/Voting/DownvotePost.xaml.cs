using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.ViewModel.Voting;
using System;

namespace QuoraDominatorUI.QDViews.Voting
{
    public class DownvotePostsBase : ModuleSettingsUserControl<DownvotePostViewModel, DownvotePostModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.DownvotePostModel =
                        templateModel.ActivitySettings.GetActivityModel<DownvotePostModel>(ObjViewModel.DownvotePostModel);
                else if (ObjViewModel == null)
                    ObjViewModel = new DownvotePostViewModel();

                ObjViewModel.DownvotePostModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
    /// <summary>
    /// Interaction logic for DownvotePost.xaml
    /// </summary>
    public partial class DownvotePost
    {
        private static DownvotePost Instance;
        public DownvotePost()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: DownvotePostsHeader,
                footer: DownvotePostsFooter,
                queryControl: DownvotePostsSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.DownVotePost,
                moduleName: QdMainModule.Voting.ToString()
            );


            //VideoTutorialLink = ConstantHelpDetails.UpvoteAnswersVideoTutorialsLink;
            //KnowledgeBaseLink = ConstantHelpDetails.UpvoteAnswersKnowledgeBaseLink;
            //ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }
        public static DownvotePost GetSingeltonInstance()
        {
            return Instance ?? (Instance = new DownvotePost());
        }
    }
}
