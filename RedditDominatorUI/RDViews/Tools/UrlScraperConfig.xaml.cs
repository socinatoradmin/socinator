using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Newtonsoft.Json;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using System;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UrlScraperConfig.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public class UrlScraperConfigBase : ModuleSettingsUserControl<UrlScraperViewModel, UrlScraperModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return true;
            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
            return false;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.UrlScraperModel =
                        JsonConvert.DeserializeObject<UrlScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new UrlScraperViewModel();

                ObjViewModel.UrlScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class UrlScraperConfig : UrlScraperConfigBase
    {
        public UrlScraperConfig()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UrlScraper,
                Enums.RdMainModule.UrlScraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: UrlScraperSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.UrlScraperVideoTutorialsLink;
        }

        private static UrlScraperConfig CurrentUrlScraperConfig { get; set; }

        public static UrlScraperConfig GetSingeltonObjectUrlScraperConfig()
        {
            return CurrentUrlScraperConfig ?? (CurrentUrlScraperConfig = new UrlScraperConfig());
        }
    }
}