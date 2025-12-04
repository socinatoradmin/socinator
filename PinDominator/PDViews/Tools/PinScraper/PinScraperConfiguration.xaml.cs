using System;
using System.Windows;
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

namespace PinDominator.PDViews.Tools.PinScraper
{
    public class PinScraperConfigurationBase : ModuleSettingsUserControl<PinScraperViewModel, PinScraperModel>
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
                    ObjViewModel.PinScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<PinScraperModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new PinScraperViewModel();

                ObjViewModel.PinScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for PinScraperConfiguration.xaml
    /// </summary>
    public partial class PinScraperConfiguration
    {
        private PinScraperConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.PinScraper,
                Enums.PdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: PinScraperConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.PinScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.PinScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static PinScraperConfiguration CurrentPinScraper { get; set; }

        /// <summary>
        ///     GetSingletonObjectPinScraperConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static PinScraperConfiguration GetSingletonObjectPinScraperConfiguration()
        {
            return CurrentPinScraper ?? (CurrentPinScraper = new PinScraperConfiguration());
        }
    }
}