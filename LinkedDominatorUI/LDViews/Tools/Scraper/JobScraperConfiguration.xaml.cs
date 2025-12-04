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
    public class JobScraperConfigurationBase : ModuleSettingsUserControl<JobScraperViewModel, JobScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.JobScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<JobScraperModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new JobScraperViewModel();
                ObjViewModel.JobScraperModel.IsAccountGrowthActive = isToggleActive;
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
    ///     Interaction logic for JobScraperConfiguration.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class JobScraperConfiguration : JobScraperConfigurationBase
    {
        public JobScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.JobScraper,
                LdMainModules.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: JobScraperSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.JobScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.JobScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static JobScraperConfiguration CurrentJobScraperConfiguration { get; set; }

        public static JobScraperConfiguration GetSingletonObjectJobScraperConfiguration()
        {
            return CurrentJobScraperConfiguration ?? (CurrentJobScraperConfiguration = new JobScraperConfiguration());
        }
    }
}