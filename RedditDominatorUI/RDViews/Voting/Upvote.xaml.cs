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
    public class UpvoteBase : ModuleSettingsUserControl<UpvoteViewModel, UpvoteModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error", "Please add at least one query.");
            return false;
        }
    }


    public sealed partial class Upvote : UpvoteBase
    {
        public Upvote()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UpvoteHeader,
                footer: UpvoteFooter,
                queryControl: UpvoteSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Upvote,
                moduleName: RdMainModule.Voting.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.UpVoteKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.UpVoteVideoTutorialsLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Upvote CurrentUpvote { get; set; }

        public static Upvote GetSingletonObjectUpvote()
        {
            return CurrentUpvote ?? (CurrentUpvote = new Upvote());
        }
    }
}