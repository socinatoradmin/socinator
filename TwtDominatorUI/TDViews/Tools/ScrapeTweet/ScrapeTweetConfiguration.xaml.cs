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

namespace TwtDominatorUI.TDViews.Tools.ScrapeTweet
{
    public class ScrapeTweetConfigBase : ModuleSettingsUserControl<ScrapeTweetViewModel, ScrapeTweetModel>
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
                    ObjViewModel.ScrapeTweetModel =
                        templateModel.ActivitySettings.GetActivityModel<ScrapeTweetModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new ScrapeTweetViewModel();

                ObjViewModel.ScrapeTweetModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for ScrapeTweetConfiguration.xaml
    /// </summary>
    public partial class ScrapeTweetConfiguration : ScrapeTweetConfigBase
    {
        public ScrapeTweetConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.TweetScraper,
                Enums.TdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ScrapeTweetConfigurationSearchControl
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.ScrapeTweetVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.ScrapeTweetKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}