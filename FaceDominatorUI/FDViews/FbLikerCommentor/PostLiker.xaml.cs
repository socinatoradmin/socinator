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
    public class PostLikerBase : ModuleSettingsUserControl<PostLikerViewModel, PostLikerModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!Model.PostLikeCommentorModel.IsOwnWallChecked && !Model.PostLikeCommentorModel.IsNewsfeedChecked &&
                !Model.PostLikeCommentorModel.IsFriendTimeLineChecked &&
                !Model.PostLikeCommentorModel.IsCustomPostListChecked
                && !Model.PostLikeCommentorModel.IsCampaignChecked && !Model.PostLikeCommentorModel.IsGroupChecked
                && !Model.PostLikeCommentorModel.IsPageChecked && !Model.PostLikeCommentorModel.IsKeywordChecked
                && !Model.PostLikeCommentorModel.IsCampaignChked && !Model.PostLikeCommentorModel.IsPostSharerChecked
                && !Model.PostLikeCommentorModel.IsHashtagChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneOption".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsFriendTimeLineChecked &&
                Model.PostLikeCommentorModel.ListFriendProfileUrl != null &&
                Model.PostLikeCommentorModel.ListFriendProfileUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsGroupChecked && Model.PostLikeCommentorModel.ListGroupUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsPageChecked && Model.PostLikeCommentorModel.ListPageUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsCampaignChecked &&
                Model.PostLikeCommentorModel.ListFaceDominatorCampaign.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsCustomPostListChecked &&
                Model.PostLikeCommentorModel.ListCustomPostList.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsKeywordChecked && Model.PostLikeCommentorModel.ListKeywords.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsCampaignChked && Model.PostLikeCommentorModel.ListCampaign.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsPostSharerChecked && Model.PostLikeCommentorModel.ListPostSharer.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                   "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }


            if (Model.PostLikeCommentorModel.IsHashtagChecked && Model.PostLikeCommentorModel.ListHashtags.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.LikerCommentorConfigModel.ListReactionType.Count == 0)
            {
                if (Model.LikerCommentorConfigModel.IsLikeFilterChkd)
                    Model.LikerCommentorConfigModel.ListReactionType.Add(DominatorHouseCore.Enums.FdQuery.ReactionType.Like);
                else
                {
                    Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneReactionType".FromResourceDictionary());
                    return false;
                }
            }

            if (Model.IsActionasPageChecked && Model.ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneOwnPage".FromResourceDictionary());
                return false;
            }

            if (!Model.IsActionasPageChecked && !Model.IsActionasOwnAccountChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectReactByOwnAccountOrPage".FromResourceDictionary());
                return false;
            }

            if (Model.CommentOnPostChecked && string.IsNullOrEmpty(Model.UploadComment))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddCommentSaveIt".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for PostLiker.xaml
    /// </summary>
    public partial class PostLiker
    {
        private PostLiker()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: LikeCommentFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.PostLiker,
                moduleName: FdMainModule.LikerCommentor.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.PostLikerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.PostLikerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static PostLiker CurrentPostLiker { get; set; }

        public static PostLiker GetSingeltonObjectPostLiker()
        {
            return CurrentPostLiker ?? (CurrentPostLiker = new PostLiker());
        }

        #region OldEvents

        //private void LikeCommentFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //=> base.FooterControl_OnSelectAccountChanged(sender, e);

        //private void LikeCommentFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //=> base.CreateCampaign();

        //private void LikeCommentFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //=> UpdateCampaign();

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}

        //private void Postoptions_Checked(object sender, RoutedEventArgs e)
        //{
        //    var currentCheckBoxItem = sender as CheckBox;
        //    if (currentCheckBoxItem.IsChecked == true)
        //    {
        //        string content = currentCheckBoxItem.Name.ToString();
        //        ObjViewModel.PostLikerModel.PostLikeCommentOptions.Add(content);
        //    }
        //}
        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //} 

        #endregion
    }
}