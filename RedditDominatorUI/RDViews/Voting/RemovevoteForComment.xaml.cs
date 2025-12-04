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
    public class RemovevoteForCommentBase : ModuleSettingsUserControl<RemoveVoteViewModel, RemoveVoteModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error", "Please add at least one query.");
            return false;
        }
    }


    public sealed partial class RemovevoteForComment : RemovevoteForCommentBase
    {
        public RemovevoteForComment()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: RemoveVoteHeader,
                footer: RemoveVoteFooter,
                queryControl: RemoveVoteSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.RemoveVoteComment,
                moduleName: RdMainModule.Voting.ToString()
            );

            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.RemoveVoteForCommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.RemoveVoteForCommentVideoTutorialsLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static RemovevoteForComment CurrentRemoveVote { get; set; }

        public static RemovevoteForComment GetSingletonObjectRemovevoteForComment()
        {
            return CurrentRemoveVote ?? (CurrentRemoveVote = new RemovevoteForComment());
        }
    }
}