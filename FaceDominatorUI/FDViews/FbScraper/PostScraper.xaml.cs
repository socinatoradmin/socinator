using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FDViewModel.ScraperViewModel;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace FaceDominatorUI.FDViews.FbScraper
{
    public class PostScraperBase : ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
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

            if (Model.PostLikeCommentorModel.IsHashtagChecked && Model.PostLikeCommentorModel.ListHashtags.Count == 0)
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


            if (!Model.PostLikeCommentorModel.IsOwnWallChecked && !Model.PostLikeCommentorModel.IsNewsfeedChecked &&
                !Model.PostLikeCommentorModel.IsFriendTimeLineChecked &&
                !Model.PostLikeCommentorModel.IsCustomPostListChecked
                && !Model.PostLikeCommentorModel.IsCampaignChecked && !Model.PostLikeCommentorModel.IsGroupChecked
                && !Model.PostLikeCommentorModel.IsPageChecked && !Model.PostLikeCommentorModel.IsKeywordChecked
                && !Model.PostLikeCommentorModel.IsCampaignChked && !Model.PostLikeCommentorModel.IsHashtagChecked
                && !Model.PostLikeCommentorModel.IsPostSharerChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneOption".FromResourceDictionary());
                return false;
            }

            if (Model.PostFilterModel.CaptionWhitelist != null)
                Model.PostFilterModel.RestrictedPostCaptionList =
                    new ObservableCollection<string>(Regex.Split(Model.PostFilterModel.CaptionWhitelist, "\r\n"));
            if (Model.PostFilterModel.CaptionBlacklists != null)
                Model.PostFilterModel.RestrictedPostCaptionList =
                    new ObservableCollection<string>(Regex.Split(Model.PostFilterModel.CaptionBlacklists, "\r\n"));
            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for PostScraper.xaml
    /// </summary>
    public partial class PostScraper
    {
        public PostScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: PostScraperHeader,
                footer: PostScraperFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.PostScraper,
                moduleName: FdMainModule.Friends.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.PostScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.PostScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static PostScraper CurrentPostScraper { get; set; }

        public static PostScraper GetSingeltonObjectPostScraper()
        {
            return CurrentPostScraper ?? (CurrentPostScraper = new PostScraper());
        }


        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}


        //private void PostSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        //    => base.SearchQueryControl_OnCustomFilterChanged(sender, e);


        //private void PostScraperFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        // => base.FooterControl_OnSelectAccountChanged(sender, e);

        //private void PostScraperFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //=> base.CreateCampaign();

        //private void PostScraperFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        //    => UpdateCampaign();


        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //} 

        #endregion
    }
}