using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.Engage
{
    public class CommentBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
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

            if (Model.LstManageCommentModel.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                "LangKeyAddAtLeastOneComment".FromResourceDictionary());
            return false;
        }
    }

    public sealed partial class Comment : CommentBase
    {
        private Comment()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: CommentHeader,
                footer: CommentFooter,
                queryControl: CommentSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Comment,
                moduleName: RdMainModule.GrowComment.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Comment _comment { get; set; }

        public static Comment GetSingeltonObjectComment()
        {
            return _comment ?? (_comment = new Comment());
        }
    }
}