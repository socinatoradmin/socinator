using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDViewModel.LikerCommentorViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;

namespace FaceDominatorUI.FDViews.FbLikerCommentor
{
    public class
        PostLikerCommentorBase : ModuleSettingsUserControl<PostLikerCommentorViewModel, PostLikerCommentorModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.PostLikeCommentorModel.IsFriendTimeLineChecked &&
                Model.PostLikeCommentorModel.ListFriendProfileUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostLikeCommentorModel.IsCustomPostListChecked &&
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

            if (!Model.PostLikeCommentorModel.IsOwnWallChecked && !Model.PostLikeCommentorModel.IsNewsfeedChecked &&
                !Model.PostLikeCommentorModel.IsFriendTimeLineChecked &&
                !Model.PostLikeCommentorModel.IsCustomPostListChecked
                && !Model.PostLikeCommentorModel.IsCampaignChecked && !Model.PostLikeCommentorModel.IsGroupChecked
                && !Model.PostLikeCommentorModel.IsPageChecked)
            {
                Dialog.ShowDialog(this, "Error",
                    "LangKeySelectAtleastOneOption".FromResourceDictionary());
                return false;
            }

            if (Model.LikerCommentorConfigModel.IsCommentFilterChecked &&
                Model.LikerCommentorConfigModel.LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneComment".FromResourceDictionary());
                return false;
            }
            if (!Model.IsActionasPageChecked && !Model.IsActionasOwnAccountChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectReactByOwnAccountOrPage".FromResourceDictionary());
                return false;
            }
            if (Model.IsActionasPageChecked && Model.ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneOwnPage".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for PostLikerCommentor.xaml
    /// </summary>
    public partial class PostLikerCommentor
    {
        private PostLikerCommentor()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: LikeCommentFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.PostLikerCommentor,
                moduleName: FdMainModule.LikerCommentor.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.PostLikerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.PostLikerKnowledgeBaseLink;
            ContactSupportLink = FdConstants.PostLikerContactLink;
            try
            {
                SetDataContext();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            DialogParticipation.SetRegister(this, this);
        }

        private static PostLikerCommentor CurrentPostLikerCommentor { get; set; }

        public static PostLikerCommentor GetSingeltonObjectPostLikerCommentor()
        {
            return CurrentPostLikerCommentor ?? (CurrentPostLikerCommentor = new PostLikerCommentor());
        }

        private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        {
            HelpFlyout.IsOpen = true;
        }

        private void LikeCommentFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        {
            FooterControl_OnSelectAccountChanged(sender, e);
        }

        private void LikeCommentFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        {
            CreateCampaign();
        }

        private void LikeCommentFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        {
            UpdateCampaign();
        }

        //        private void Postoptions_Checked(object sender, RoutedEventArgs e)
        //        {
        //            var currentCheckBoxItem = sender as CheckBox;
        //            if (currentCheckBoxItem != null && currentCheckBoxItem.IsChecked == true)
        //            {
        //                string content = currentCheckBoxItem.Name;
        //                ObjViewModel.PostLikerCommentorModel.PostLikeCommentOptions.Add(content);
        //            }
        //        }

        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //}
    }
}