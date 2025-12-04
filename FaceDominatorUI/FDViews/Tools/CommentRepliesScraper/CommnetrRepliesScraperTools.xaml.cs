using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FDViewModel.ScraperViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.CommentRepliesScraper
{
    /// <summary>
    ///     Interaction logic for CommnetrRepliesScraperTools.xaml
    /// </summary>
    public class
        CommnetrRepliesScraperToolsBase : ModuleSettingsUserControl<CommentRepliesScraperViewModel,
            CommentRepliesScraperModel>
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

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CommentRepliesScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<CommentRepliesScraperModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new CommentRepliesScraperViewModel();
                ObjViewModel.CommentRepliesScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class CommnetrRepliesScraperTools
    {
        public CommnetrRepliesScraperTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.CommentRepliesScraper,
                FdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentSearchControl
            );

            // Help control links. 
            //VideoTutorialLink = FdConstants.CommentScraperVideoTutorialsLink;
            //KnowledgeBaseLink = FdConstants.CommentScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static CommnetrRepliesScraperTools CurrentCommnetrRepliesScraperTools { get; set; }

        public static CommnetrRepliesScraperTools GetSingletonObjectCommnetrRepliesScraperTools()
        {
            return CurrentCommnetrRepliesScraperTools ??
                   (CurrentCommnetrRepliesScraperTools = new CommnetrRepliesScraperTools());
        }
    }
}