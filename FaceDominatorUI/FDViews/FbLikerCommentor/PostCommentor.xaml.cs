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
    public class PostCommentorBase : ModuleSettingsUserControl<PostCommentorViewModel, PostCommentorModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!Model.PostLikeCommentorModel.IsOwnWallChecked && !Model.PostLikeCommentorModel.IsNewsfeedChecked &&
                !Model.PostLikeCommentorModel.IsFriendTimeLineChecked &&
                !Model.PostLikeCommentorModel.IsCustomPostListChecked
                && !Model.PostLikeCommentorModel.IsCampaignChecked && !Model.PostLikeCommentorModel.IsGroupChecked
                && !Model.PostLikeCommentorModel.IsPageChecked && !Model.PostLikeCommentorModel.IsKeywordChecked
                && !Model.PostLikeCommentorModel.IsCampaignChked && !Model.PostLikeCommentorModel.IsPostSharerChecked
                && !Model.PostLikeCommentorModel.IsPostScraperCampaignChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneOption".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsFriendTimeLineChecked &&
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
                    "LangKeyErrorAddAtleastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsPostScraperCampaignChecked && Model.PostLikeCommentorModel.ListPostScraperCampaign.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.LikerCommentorConfigModel.LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneComment".FromResourceDictionary());
                return false;
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

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for PostCommentor.xaml
    /// </summary>
    public partial class PostCommentor
    {
        private PostCommentor()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: LikeCommentFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.PostCommentor,
                moduleName: FdMainModule.LikerCommentor.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.PostCommentorVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.PostLikerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static PostCommentor CurrentPostCommentor { get; set; }

        public static PostCommentor GetSingeltonObjectPostCommentor()
        {
            return CurrentPostCommentor ?? (CurrentPostCommentor = new PostCommentor());
        }

        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}


        //private void LikeCommentFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        //=> base.FooterControl_OnSelectAccountChanged(sender, e);

        //private void LikeCommentFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //=> base.CreateCampaign();

        //private void LikeCommentFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //=> UpdateCampaign();

        //private void Postoptions_Checked(object sender, RoutedEventArgs e)
        //{
        //    var currentCheckBoxItem = sender as CheckBox;
        //    if (currentCheckBoxItem.IsChecked == true)
        //    {
        //        string content = currentCheckBoxItem.Name.ToString();
        //        ObjViewModel.PostCommentorModel.PostLikeCommentOptions.Add(content);
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