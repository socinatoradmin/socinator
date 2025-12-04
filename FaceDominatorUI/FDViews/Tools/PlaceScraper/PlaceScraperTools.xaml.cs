using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.PlaceScraper
{
    public class PlaceScraperToolsBase : ModuleSettingsUserControl<PlaceScraperViewModel, PlaceScraperModel>
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
                    ObjViewModel.PlaceScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<PlaceScraperModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new PlaceScraperViewModel();
                ObjViewModel.PlaceScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for FanapgeLiker.xaml
    /// </summary>
    public partial class PlaceScraperTools
    {
        public PlaceScraperTools()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.PlaceScraper,
                FdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: MessageToFanpagesSearchControl
            );

            // Help control links. 
            VideoTutorialLink = "";
            KnowledgeBaseLink = "";
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            DialogParticipation.SetRegister(this, this);
        }

        private static PlaceScraperTools CurrentMessageToPlaces { get; set; }

        public static PlaceScraperTools GetSingeltonObjectMessageToPlaces()
        {
            return CurrentMessageToPlaces ?? (CurrentMessageToPlaces = new PlaceScraperTools());
        }
    }
}