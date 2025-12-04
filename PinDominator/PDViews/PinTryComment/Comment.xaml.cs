using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinTryCommenter;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinTryComment
{
    public class CommentBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one query.");
                return false;
            }

            if (ObjViewModel.CommentModel.LstDisplayManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one Comment.");
                return false;
            }

            if (Model.ChkCommentOnUserLatestPostsChecked && Model.LstComments.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one comment in after comment action.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for Comment.xaml
    /// </summary>
    public sealed partial class Comment
    {
        private CommentViewModel _objCommentViewModel;

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
                moduleName: PdMainModule.TryComment.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public CommentViewModel ObjCommentViewModel
        {
            get => _objCommentViewModel;
            set
            {
                _objCommentViewModel = value;
                OnPropertyChanged(nameof(ObjCommentViewModel));
            }
        }

        private static Comment CurrentComment { get; set; }

        public static Comment GetSingeltonObjectComment()
        {
            return CurrentComment ?? (CurrentComment = new Comment());
        }
    }
}