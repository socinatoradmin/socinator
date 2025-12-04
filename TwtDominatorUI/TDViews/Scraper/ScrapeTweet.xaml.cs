using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.Scraper;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.Scraper
{
    public class ScrapeTweetBase : ModuleSettingsUserControl<ScrapeTweetViewModel, ScrapeTweetModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }


    /// <summary>
    ///     Interaction logic for ScrapeTweet.xaml
    /// </summary>
    public partial class ScrapeTweet : ScrapeTweetBase
    {
        private ScrapeTweet()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ScrapeTweetHeader,
                footer: ScrapeTweetFooter,
                queryControl: ScrapeTweetSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.TweetScraper,
                moduleName: TdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.ScrapeTweetVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.ScrapeTweetKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region Singleton object creation

        private static ScrapeTweet objScrapeTweet;

        public static ScrapeTweet GetSingletonObjectScrapeTweet()
        {
            return objScrapeTweet ?? (objScrapeTweet = new ScrapeTweet());
        }

        #endregion
    }
}