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
    public class DownvoteBase : ModuleSettingsUserControl<DownvoteViewModel, DownvoteModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error", "Please add at least one query.");
            return false;
        }
    }


    public sealed partial class Downvote : DownvoteBase
    {
        public Downvote()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: DownvoteHeader,
                footer: DownvoteFooter,
                queryControl: DownvoteSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Downvote,
                moduleName: RdMainModule.Voting.ToString()
            );

            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.DownvoteKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.DownvoteVideoTutorialsLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Downvote CurrentDownvote { get; set; }

        public static Downvote GetSingletonObjectDownvote()
        {
            return CurrentDownvote ?? (CurrentDownvote = new Downvote());
        }
    }
}