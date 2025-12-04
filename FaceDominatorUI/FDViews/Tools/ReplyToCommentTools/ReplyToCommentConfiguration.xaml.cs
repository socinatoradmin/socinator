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

namespace FaceDominatorUI.FDViews.Tools.ReplyToCommentTools
{
    /// <summary>
    ///     Interaction logic for ReplyToCommentConfiguration.xaml
    /// </summary>
    public class
        ReplyToCommentConfigurationBase : ModuleSettingsUserControl<ReplyToCommentsViewModel, ReplyToCommentModel>
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

            if (Model.LstManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneComment".FromResourceDictionary());
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
                    ObjViewModel.ReplyToCommentsModel
                        = templateModel.ActivitySettings.GetActivityModel<ReplyToCommentModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new ReplyToCommentsViewModel();
                ObjViewModel.ReplyToCommentsModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class ReplyToCommentConfiguration
    {
        public ReplyToCommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ReplyToComment,
                FdMainModule.LikerCommentor.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ReplyToCommentControl
            );

            VideoTutorialLink = FdConstants.ReplyToCommentVideoTutorialLink;
            KnowledgeBaseLink = FdConstants.ReplyMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static ReplyToCommentConfiguration CurrentReplyToCommentConfiguration { get; set; }

        public static ReplyToCommentConfiguration GetSingeltonObjectReplyToCommentConfiguration()
        {
            return CurrentReplyToCommentConfiguration ??
                   (CurrentReplyToCommentConfiguration = new ReplyToCommentConfiguration());
        }
    }
}