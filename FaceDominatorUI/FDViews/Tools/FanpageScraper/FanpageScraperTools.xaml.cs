using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.ScraperModel;
using FaceDominatorCore.FDViewModel.ScraperViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.FanpageScraper
{
    public class FanpageScraperToolsBase : ModuleSettingsUserControl<FanpageScraperViewModel, FanpageScraperModel>
    {
        protected override bool ValidateExtraProperty()
        {
            //Check queries
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
                    ObjViewModel.FanpageScraperModel =
                        templateModel.ActivitySettings.GetActivityModel<FanpageScraperModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new FanpageScraperViewModel();
                ObjViewModel.FanpageScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for FanpageScraperTools.xaml
    /// </summary>
    public partial class FanpageScraperTools
    {
        public FanpageScraperTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.FanpageScraper,
                FdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FanpageSearchControl
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.FanpageScraperVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.FanpageScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }

        private static FanpageScraperTools CurrentFanpageScraperTools { get; set; }

        public static FanpageScraperTools GetSingeltonObjectFanpageScraperTools()
        {
            return CurrentFanpageScraperTools ?? (CurrentFanpageScraperTools = new FanpageScraperTools());
        }
    }
}