using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.InstaLikerCommenter;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.InstaLikeComment
{
    public class ReplyCommentBase : ModuleSettingsUserControl<ReplyCommentViewModel, ReplyCommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstDisplayManageCommentModel.Count == 0)
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
    ///     Interaction logic for ReplyComment.xaml
    /// </summary>
    public partial class ReplyComment : ReplyCommentBase
    {
        public static ReplyComment objReplyComment;

        private ReplyComment()
        {
            InitializeComponent();
            InitializeBaseClass(
                header: ReplyCommentHeader,
                footer: ReplyCommentFooter,
                queryControl: ReplyCommentSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.ReplyToComment,
                moduleName: GdMainModule.LikeComment.ToString()
            );

            // Help control links.  
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static ReplyComment SingletonReplyComment()
        {
            return objReplyComment ?? (objReplyComment = new ReplyComment());
        }
    }
}