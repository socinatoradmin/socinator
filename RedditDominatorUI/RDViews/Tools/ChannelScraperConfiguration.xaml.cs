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
    ///     Interaction logic for ChannelScraperConfiguration.xaml
    /// </summary>
    public partial class ChannelScraperConfiguration : ChannelScraperConfigurationBase
    {
        public ChannelScraperConfiguration()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ChannelScraper,
                Enums.RdMainModule.ChannelScraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ChannelScraperSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.ChannelScraperVideoTutorialsLink;
        }

        private static ChannelScraperConfiguration CurrentChannelScraperConfiguration { get; set; }

        public static ChannelScraperConfiguration GetSingeltonObjectChannelScraperConfiguration()
        {
            return CurrentChannelScraperConfiguration ??
                   (CurrentChannelScraperConfiguration = new ChannelScraperConfiguration());
        }
    }


    public class
        ChannelScraperConfigurationBase : ModuleSettingsUserControl<ChannelScraperViewModel, ChannelScraperModel>
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
                    ObjViewModel.ChannelScraperModel =
                        JsonConvert.DeserializeObject<ChannelScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new ChannelScraperViewModel();

                ObjViewModel.ChannelScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}