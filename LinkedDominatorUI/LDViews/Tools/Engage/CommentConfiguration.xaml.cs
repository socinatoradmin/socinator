using System;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Engage;
using LinkedDominatorCore.LDViewModel.Engage;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Engage
{
    public class CommentConfigurationBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CommentModel =
                        templateModel.ActivitySettings.GetActivityModel<CommentModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new CommentViewModel();
                ObjViewModel.CommentModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        protected override bool ValidateExtraProperty()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.CommentModel.ManageCommentModel.LstQueries.Count > 0)
                foreach (var item in ObjViewModel.CommentModel.ManageCommentModel.LstQueries)
                    if (item.Content.QueryValue != "All")
                    {
                        string Comment = null;
                        try
                        {
                            Comment = ObjViewModel.CommentModel.LstDisplayManageCommentModel.FirstOrDefault(x =>
                                    x.LstQueries.FirstOrDefault(y =>
                                        y.Content.QueryType == item.Content.QueryType &&
                                        y.Content.QueryValue == item.Content.QueryValue) != null)
                                ?.CommentText;
                        }

                        catch (Exception ex)
                        {
                        }

                        if (Comment == null)
                        {
                            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                                string.Format("LangKeyPleaseInputAtleastOneCommentForQuery".FromResourceDictionary(),
                                    item.Content.QueryType));
                            return false;
                        }
                    }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for CommentConfiguration.xaml
    /// </summary>
    public partial class CommentConfiguration : CommentConfigurationBase
    {
        public CommentConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Comment,
                LdMainModules.Engage.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentSearchControl
            );
            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static CommentConfiguration CurrentCommentConfiguration { get; set; }

        public static CommentConfiguration GetSingletonObjectCommentConfiguration()
        {
            return CurrentCommentConfiguration ?? (CurrentCommentConfiguration = new CommentConfiguration());
        }
    }
}