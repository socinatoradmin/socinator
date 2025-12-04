using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.SalesNavigatorScraper;
using LinkedDominatorCore.LDViewModel.SalesNavigatorScraper;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.SalesNavigatorScraper
{
    public class
        SalesNavigatorCompanyScraperConfigurationBase : ModuleSettingsUserControl<CompanyScraperViewModel,
            CompanyScraperModel>
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
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for CompanyScraperConfiguration.xaml
    /// </summary>
    public partial class CompanyScraperConfiguration : SalesNavigatorCompanyScraperConfigurationBase
    {
        public CompanyScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SalesNavigatorCompanyScraper,
                LdMainModules.SalesNavigatorScraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CompanyScraperSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SalesCompanyScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SalesUserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static CompanyScraperConfiguration CurrentCompanyScraperConfiguration { get; set; }

        public static CompanyScraperConfiguration GetSingletonObjectCompanyScraperConfiguration()
        {
            return CurrentCompanyScraperConfiguration ??
                   (CurrentCompanyScraperConfiguration = new CompanyScraperConfiguration());
        }
    }
}