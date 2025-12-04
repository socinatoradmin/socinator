using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDViewModel.Scraper;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Scraper
{
    public class
        CompanyScraperConfigurationBase : ModuleSettingsUserControl<CompanyScraperViewModel, CompanyScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CompanyScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<CompanyScraperModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new CompanyScraperViewModel();
                ObjViewModel.CompanyScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }

    /// <summary>
    ///     Interaction logic for CompanyScraperConfiguration.xaml
    /// </summary>
    public partial class CompanyScraperConfiguration : CompanyScraperConfigurationBase
    {
        public CompanyScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.CompanyScraper,
                LdMainModules.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CompanyScraperSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CompanyScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static CompanyScraperConfiguration CurrentCompanyScraperConfiguration { get; set; }

        public static CompanyScraperConfiguration GetSingeltonObjectCompanyScraperConfiguration()
        {
            return CurrentCompanyScraperConfiguration ??
                   (CurrentCompanyScraperConfiguration = new CompanyScraperConfiguration());
        }
    }
}