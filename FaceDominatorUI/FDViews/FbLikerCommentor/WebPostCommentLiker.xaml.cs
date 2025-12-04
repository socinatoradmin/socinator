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
    public class
        WebPostCommentLikerBase : ModuleSettingsUserControl<WebPostLikeCommentViewModel, WebPostCommentLikerModel>
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

            if (Model.LikerCommentorConfigModel.ListReactionType.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneReactionType".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for WebPostCommentLiker.xaml
    /// </summary>
    public partial class WebPostCommentLiker
    {
        public WebPostCommentLiker()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: CommentScraperFooter,
                queryControl: CommentSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.WebPostLikeComment,
                moduleName: FdMainModule.LikerCommentor.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.CommentScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.CommentScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static WebPostCommentLiker CurrentCommentLiker { get; set; }

        public static WebPostCommentLiker GetSingeltonObjectWebPostCommentLiker()
        {
            return CurrentCommentLiker ?? (CurrentCommentLiker = new WebPostCommentLiker());
        }
    }
}