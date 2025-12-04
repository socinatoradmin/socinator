using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDViewModel.LikerCommentorViewModel;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.PostLiker
{
    public class PostLikerToolsBase : ModuleSettingsUserControl<PostLikerViewModel, PostLikerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!Model.PostLikeCommentorModel.IsOwnWallChecked && !Model.PostLikeCommentorModel.IsNewsfeedChecked &&
                !Model.PostLikeCommentorModel.IsFriendTimeLineChecked &&
                !Model.PostLikeCommentorModel.IsCustomPostListChecked
                && !Model.PostLikeCommentorModel.IsCampaignChecked && !Model.PostLikeCommentorModel.IsGroupChecked
                && !Model.PostLikeCommentorModel.IsPageChecked && !Model.PostLikeCommentorModel.IsKeywordChecked
                && !Model.PostLikeCommentorModel.IsCampaignChked)
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

            if (Model.LikerCommentorConfigModel.ListReactionType.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneReactionType".FromResourceDictionary());
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
                    "LangKeySelectAtleastOneActionAsOption".FromResourceDictionary());
                return false;
            }

            if (Model.CommentOnPostChecked && string.IsNullOrEmpty(Model.UploadComment))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddCommentSaveIt".FromResourceDictionary());
                return false;
            }

            return true;
        }


        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.PostLikerModel =
                        templateModel.ActivitySettings.GetActivityModelNonQueryList<PostLikerModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new PostLikerViewModel();
                ObjViewModel.PostLikerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    public partial class PostLikerTools
    {
        public PostLikerTools()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.PostLiker,
                FdMainModule.LikerCommentor.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.PostLikerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.PostLikerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }


        private static PostLikerTools CurrentPostLikerCommentorTools { get; set; }

        public static PostLikerTools GetSingeltonObjectPostLikerCommentorTools()
        {
            return CurrentPostLikerCommentorTools ?? (CurrentPostLikerCommentorTools = new PostLikerTools());
        }
    }
}