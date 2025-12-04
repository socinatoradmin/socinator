using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.Voting
{
    public class UpvoteForCommentBase : ModuleSettingsUserControl<UpvoteForCommentViewModel, UpvoteModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error", "Please add at least one query.");
            return false;
        }
    }


    public sealed partial class UpvoteForComment : UpvoteForCommentBase
    {
        public UpvoteForComment()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UpvoteHeader,
                footer: UpvoteFooter,
                queryControl: UpvoteSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UpvoteComment,
                moduleName: RdMainModule.GrowUpvote.ToString()
            );

            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.UpVoteForCommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.UpVoteForCommentVideoTutorialsLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UpvoteForComment CurrentUpvote { get; set; }

        public static UpvoteForComment GetSingletonObjectUpvoteForComment()
        {
            return CurrentUpvote ?? (CurrentUpvote = new UpvoteForComment());
        }
    }
}