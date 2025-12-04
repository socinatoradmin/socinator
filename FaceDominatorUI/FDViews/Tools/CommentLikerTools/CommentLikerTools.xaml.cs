using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDViewModel.LikerCommentorViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.CommentLikerTools
{
    public class CommentLikerToolsBase : ModuleSettingsUserControl<CommentLikerViewModel, CommentLikerModule>
    {
        protected override bool ValidateExtraProperty()
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
                    "Please select atleast one reaction type.");
                return false;
            }

            if (Model.IsActionasOwnAccountChecked == false && Model.IsActionasPageChecked == false)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectReactByOwnAccountOrPage".FromResourceDictionary());
                return false;
            }

            if (Model.IsActionasPageChecked == true && Model.ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAleastOnePageUrlAndSave".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CommentLikerModule
                        = templateModel.ActivitySettings.GetActivityModel<CommentLikerModule>(ObjViewModel.Model);
                else
                    ObjViewModel = new CommentLikerViewModel();
                ObjViewModel.CommentLikerModule.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for CommentLikerTools.xaml
    /// </summary>
    public partial class CommentLikerTools
    {
        public CommentLikerTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.LikeComment,
                FdMainModule.LikerCommentor.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentSearchControl
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.CommentLikerVideoTutoorialLink;
            KnowledgeBaseLink = FdConstants.CommentLikerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }

        private static CommentLikerTools CurrentCommentLikerTools { get; set; }

        public static CommentLikerTools GetSingeltonObjectCommentLikerTools()
        {
            return CurrentCommentLikerTools ?? (CurrentCommentLikerTools = new CommentLikerTools());
        }
    }
}