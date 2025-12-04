using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
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
    public class UserScraperConfigurationBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
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
    public partial class UserScraperConfiguration : UserScraperConfigurationBase
    {
        public UserScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UserScraper,
                LdMainModules.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UserScraperSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static UserScraperConfiguration CurrentUserScraperConfiguration { get; set; }

        public static UserScraperConfiguration GetSingeltonObjectUserScraperConfiguration()
        {
            return CurrentUserScraperConfiguration ??
                   (CurrentUserScraperConfiguration = new UserScraperConfiguration());
        }

        private void UserScraperConfigurationBase_Loaded(object sender, RoutedEventArgs e)
        {
            SetSelectedAccounts(SocialNetworks.LinkedIn);
            base.SetAccountModeDataContext(SocialNetworks.LinkedIn);
        }

        private void AccountGrowthHeader_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.LinkedIn).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount = AccountGrowthHeader.SelectedItem;
            base.SetAccountModeDataContext(SocialNetworks.LinkedIn);
        }

        private void AccountGrowthHeader_SaveClick(object sender, RoutedEventArgs e)
        {
            SaveConfigurations();
        }

        private void UserScraperSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            SearchQueryControl_OnAddQuery(sender, e, typeof(LDScraperUserQueryParameters));
        }

        private void UserScraperSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        {
            SearchQueryControl_OnCustomFilterChanged(sender, e);
        }

        private void TglStatus_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ChangeAccountsModuleStatus(ObjViewModel.UserScraperModel.IsAccountGrowthActive,
                AccountGrowthHeader.SelectedItem, SocialNetworks.LinkedIn))
                ObjViewModel.UserScraperModel.IsAccountGrowthActive = false;
        }
    }
}