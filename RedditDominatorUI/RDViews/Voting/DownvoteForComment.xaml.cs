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
    public class DownvoteForCommentBase : ModuleSettingsUserControl<DownvoteForCommentViewModel, DownvoteModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error", "Please add at least one query.");
            return false;
        }
    }


    public sealed partial class DownvoteForComment : DownvoteForCommentBase
    {
        public DownvoteForComment()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: DownvoteHeader,
                footer: DownvoteFooter,
                queryControl: DownvoteSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.DownvoteComment,
                moduleName: RdMainModule.GrowDownvote.ToString()
            );

            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.DownVoteForCommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.DownVoteForCommentVideoTutorialsLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static DownvoteForComment CurrentFollower { get; set; }

        public static DownvoteForComment GetSingeltonObjectDownvoteForComment()
        {
            return CurrentFollower ?? (CurrentFollower = new DownvoteForComment());
        }
    }
}