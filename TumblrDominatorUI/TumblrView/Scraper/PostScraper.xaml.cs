using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Scraper;

namespace TumblrDominatorUI.TumblrView.Scraper
{
    public class PostScraperBase : ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    public sealed partial class PostScraper
    {
        private PostScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderControl,
                footer: PostScraperFooter,
                queryControl: ScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.PostScraper,
                moduleName: Enums.TmbMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.PostScraperVideoTutorialLink;
            KnowledgeBaseLink = ConstantHelpDetails.PostScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static PostScraper _currentPostScraper;

        public static PostScraper GetSingletonObjectPostScraperConfig()
        {
            return _currentPostScraper ?? (_currentPostScraper = new PostScraper());
        }

        #region Object creation 

        /// <summary>
        /// GetSingeltonObjectLike is used to get the object of the current user control,
        /// if object is already created then its wont create a new object object, simply it returns already created object,
        /// otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>

        #endregion
    }
}