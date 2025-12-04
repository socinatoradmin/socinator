using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinScraper;
using System;

namespace PinDominator.PDViews.Tools.BoardScraper
{
    public class BoardScraperConfigurationBase : ModuleSettingsUserControl<BoardScraperViewModel, BoardScraperModel>
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
                    ObjViewModel.BoardScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<BoardScraperModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new BoardScraperViewModel();

                ObjViewModel.BoardScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for BoardScraperConfiguration.xaml
    /// </summary>
    public partial class BoardScraperConfiguration
    {
        private BoardScraperConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BoardScraper,
                Enums.PdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: BoardScraperConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BoardScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BoardScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static BoardScraperConfiguration CurrentBoardScraper { get; set; }

        /// <summary>
        ///     GetSingletonObjectBoardScraperConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static BoardScraperConfiguration GetSingletonObjectBoardScraperConfiguration()
        {
            return CurrentBoardScraper ?? (CurrentBoardScraper = new BoardScraperConfiguration());
        }
    }
}