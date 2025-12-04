using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaLikerCommenter;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.InstaLikeComment
{
    public class LikeCommentsBase : ModuleSettingsUserControl<LikeCommentViewModel, LikeCommentModel>
    {
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

        protected override bool ValidateExtraProperty()
        {
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
    }

    /// <summary>
    ///     Interaction logic for LikeComment.xaml
    /// </summary>
    public partial class LikeComment : LikeCommentsBase
    {
        private LikeComment()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: LikeCommentHeader,
                footer: LikeCommentFooter,
                queryControl: LikeCommentSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.LikeComment,
                moduleName: Enums.GdMainModule.LikeComment.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.LikeCommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.LikeCommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();

            DialogParticipation.SetRegister(this, this);
        }


        private static LikeComment CurrentLikeComment { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUserScraper is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static LikeComment GetSingeltonObjectLikeComment()
        {
            return CurrentLikeComment ?? (CurrentLikeComment = new LikeComment());
        }
    }
}