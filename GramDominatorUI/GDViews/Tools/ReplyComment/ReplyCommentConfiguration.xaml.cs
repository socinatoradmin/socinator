using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDViewModel.InstaLikerCommenter;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.ReplyComment
{
    public class ReplyCommentConfigurationBase : ModuleSettingsUserControl<ReplyCommentViewModel, ReplyCommentModel>
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
    ///     Interaction logic for ReplyCommentConfiguration.xaml
    /// </summary>
    public partial class ReplyCommentConfiguration : ReplyCommentConfigurationBase
    {
        public static ReplyCommentConfiguration objreplyCommentConfiguration;

        public ReplyCommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ReplyToComment,
                Enums.GdMainModule.LikeComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ReplyCommentConfigSearchControl
            );
        }

        public static ReplyCommentConfiguration SingletonReplyCommentConfiguration()
        {
            return objreplyCommentConfiguration ?? (objreplyCommentConfiguration = new ReplyCommentConfiguration());
        }
    }
}