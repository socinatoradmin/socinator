using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaLikerCommenter;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.InstaLikeComment
{
    public class CommentBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstDisplayManageCommentModel.Count == 0 && string.IsNullOrEmpty(Model.MentionUsers))
            {
                Dialog.ShowDialog(this, "Input Error", "Please provide comment(s)");
                return false;
            }

            if (Model.PostFilterModel.PostCategory.FilterPostCategory)
                if (Model.PostFilterModel.PostCategory.IgnorePostImages &&
                    Model.PostFilterModel.PostCategory.IgnorePostVideos &&
                    Model.PostFilterModel.PostCategory.IgnorePostAlbums)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please check maximum two options for Post Type filteration inside Post Filter category.");
                    return false;
                }

            if (Model.IsChkAfterCommentAction &&
                (!Model.IsChkLikePostAfterComment && !Model.IsChkFollowUserAfterComment))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one option inside After Comment Action category");
                return false;
            }

            if (Model.IsChkMentionRandomUsers && string.IsNullOrEmpty(Model.MentionUsers))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please enter user(s) to mention in comment");
                return false;
            }

            return true;
        }

        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for Comment.xaml
    /// </summary>
    public partial class Comment : CommentBase
    {
        private Comment()
        {
            InitializeComponent();
            InitializeBaseClass(
                header: CommentHeader,
                footer: CommentFooter,
                queryControl: CommentSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Comment,
                moduleName: GdMainModule.LikeComment.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        #region singleton method Creation

        private static Comment CurrentComment { get; set; }

        public static Comment GetSingeltonObjectComment()
        {
            return CurrentComment ?? (CurrentComment = new Comment());
        }

        #endregion
    }
}