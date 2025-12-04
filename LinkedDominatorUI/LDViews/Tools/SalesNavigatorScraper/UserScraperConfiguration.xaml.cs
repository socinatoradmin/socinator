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
        SalesNavigatorUserScraperConfigurationBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UserScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<UserScraperModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new UserScraperViewModel();
                ObjViewModel.UserScraperModel.IsAccountGrowthActive = isToggleActive;
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
    ///     Interaction logic for UserScraperConfiguration.xaml
    /// </summary>
    public partial class UserScraperConfiguration : SalesNavigatorUserScraperConfigurationBase
    {
        public UserScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SalesNavigatorUserScraper,
                LdMainModules.SalesNavigatorScraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UserScraperSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SalesUserScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SalesUserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static UserScraperConfiguration CurrentUserScraperConfiguration { get; set; }

        public static UserScraperConfiguration GetSingeltonObjectUserScraperConfiguration()
        {
            return CurrentUserScraperConfiguration ??
                   (CurrentUserScraperConfiguration = new UserScraperConfiguration());
        }
    }
}