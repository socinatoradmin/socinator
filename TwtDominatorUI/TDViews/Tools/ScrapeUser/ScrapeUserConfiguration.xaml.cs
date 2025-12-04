using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.Scraper;

namespace TwtDominatorUI.TDViews.Tools.ScrapeUser
{
    public class ScrapeUserConfigBase : ModuleSettingsUserControl<ScrapeUserViewModel, ScrapeUserModel>
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
                    ObjViewModel.ScrapeUserModel =
                        templateModel.ActivitySettings.GetActivityModel<ScrapeUserModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new ScrapeUserViewModel();

                ObjViewModel.ScrapeUserModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for ScrapeUserConfiguration.xaml
    /// </summary>
    public partial class ScrapeUserConfiguration : ScrapeUserConfigBase
    {
        public ScrapeUserConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UserScraper,
                Enums.TdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ScrapeUserConfigSearchControl
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.ScrapeUserVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.ScrapeUserVideoTutorialsLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}