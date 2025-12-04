using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.Voting;
using System;

namespace QuoraDominatorUI.QDViews.Voting
{
    public class UpvotePostsBase : ModuleSettingsUserControl<UpvotePostsViewModel, UpvotePostsModel>
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
                    ObjViewModel.UpvotePostsModel =
                        templateModel.ActivitySettings.GetActivityModel<UpvotePostsModel>(ObjViewModel.UpvotePostsModel);
                else if (ObjViewModel == null)
                    ObjViewModel = new UpvotePostsViewModel();

                ObjViewModel.UpvotePostsModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
    /// <summary>
    /// Interaction logic for UpvotePosts.xaml
    /// </summary>
    public partial class UpvotePosts
    {
        private static UpvotePosts Instance;
        public UpvotePosts()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UpvotePostsHeader,
                footer: UpvotePostsFooter,
                queryControl: UpvotePostsSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UpvotePost,
                moduleName: QdMainModule.Voting.ToString()
            );


            //VideoTutorialLink = ConstantHelpDetails.UpvoteAnswersVideoTutorialsLink;
            //KnowledgeBaseLink = ConstantHelpDetails.UpvoteAnswersKnowledgeBaseLink;
            //ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }
        public static UpvotePosts GetSingeltonInstance()
        {
            return Instance ?? (Instance = new UpvotePosts());
        }
    }
}
