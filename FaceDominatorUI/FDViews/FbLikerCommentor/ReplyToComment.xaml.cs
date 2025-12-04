using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDViewModel.LikerCommentorViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbLikerCommentor
{
    public class ReplyToCommentBase : ModuleSettingsUserControl<ReplyToCommentsViewModel, ReplyToCommentModel>
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

            if (Model.LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneComment".FromResourceDictionary());
                return false;
            }

            if (!Model.IsActionasOwnAccountChecked && !Model.IsActionasPageChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectReactByOwnAccountOrPage".FromResourceDictionary());
                return false;
            }

            if (Model.IsActionasPageChecked && Model.ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAleastOnePageUrlAndSave".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for ReplyToComment.xaml
    /// </summary>
    public partial class ReplyToComment
    {
        public ReplyToComment()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: ReplyToCommentsFooter,
                queryControl: ReplyToCommentsQuery,
                MainGrid: MainGrid,
                activityType: ActivityType.ReplyToComment,
                moduleName: FdMainModule.LikerCommentor.ToString()
            );

            VideoTutorialLink = FdConstants.ReplyToCommentVideoTutorialLink;
            KnowledgeBaseLink = FdConstants.ReplyToCommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static ReplyToComment CurrentReplyToComment { get; set; }

        public static ReplyToComment GetSingeltonObjectReplyToComment()
        {
            return CurrentReplyToComment ?? (CurrentReplyToComment = new ReplyToComment());
        }
    }
}