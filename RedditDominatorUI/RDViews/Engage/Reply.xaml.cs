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
    public class ReplyBase : ModuleSettingsUserControl<ReplyViewModel, ReplyModel>
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

    /// <summary>
    ///     Interaction logic for Reply.xaml
    /// </summary>
    public sealed partial class Reply : ReplyBase
    {
        public Reply()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ReplyHeader,
                footer: ReplyFooter,
                queryControl: ReplySearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Reply,
                moduleName: RdMainModule.GrowReply.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.ReplyKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.ReplyVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Reply CurrentReply { get; set; }


        public static Reply GetSingeltonObjectReply()
        {
            return CurrentReply ?? (CurrentReply = new Reply());
        }
    }
}