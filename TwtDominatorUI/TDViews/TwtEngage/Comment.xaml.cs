using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtEngage;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.TwtEngage
{
    public class CommentBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0 || Model.LstDisplayManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for Comment.xml
    /// </summary>
    public partial class Comment
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
                moduleName: TdMainModule.TwtEngage.ToString()
            );

            // Help control links. 

            VideoTutorialLink = TDHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region SingletonObject Creation

        private static Comment objComment;

        public static Comment GetSingletonObjectComment()
        {
            return objComment ?? (objComment = new Comment());
        }

        #endregion
    }
}