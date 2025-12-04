using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.PdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using PinDominator.CustomControl;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinScraper;
using System;
using System.Windows;

namespace PinDominator.PDViews.Tools.UserScraper
{
    public class UserScraperConfigurationBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
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

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UserScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<UserScraperModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new UserScraperViewModel();

                ObjViewModel.UserScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for UserScraperConfiguration.xaml
    /// </summary>
    public partial class UserScraperConfiguration
    {
        private UserScraperConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UserScraper,
                Enums.PdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UserScraperConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static UserScraperConfiguration CurrentUserScraper { get; set; }

        /// <summary>
        ///     GetSingletonObjectUserScraperConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UserScraperConfiguration GetSingletonObjectUserScraperConfiguration()
        {
            return CurrentUserScraper ?? (CurrentUserScraper = new UserScraperConfiguration());
        }
    }
}