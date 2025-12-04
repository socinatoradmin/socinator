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
    public class RemoveVoteBase : ModuleSettingsUserControl<RemoveVoteViewModel, RemoveVoteModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Input Error", "Please add at least one query.");
            return false;
        }
    }


    public partial class Removevote : RemoveVoteBase
    {
        public Removevote()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: RemoveVoteHeader,
                footer: RemoveVoteFooter,
                queryControl: RemoveVoteSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.RemoveVote,
                moduleName: RdMainModule.Voting.ToString()
            );

            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.RemoveVoteKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.RemoveVoteVideoTutorialsLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Removevote CurrentRemoveVote { get; set; }

        /// <summary>
        ///     This method set DataContext of Comment model
        /// </summary>
        protected sealed override void SetDataContext()
        {
            base.SetDataContext();

            CampaignName = CampaignName;
        }

        public static Removevote GetSingletonObjectRemovevote()
        {
            return CurrentRemoveVote ?? (CurrentRemoveVote = new Removevote());
        }
    }
}