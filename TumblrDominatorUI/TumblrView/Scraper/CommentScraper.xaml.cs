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
    public class CommentScraperBase : ModuleSettingsUserControl<CommentScraperViewModel, CommentScraperModel>
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

    public sealed partial class CommentScraper
    {
        private CommentScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderControl,
                footer: CommentScraperFooter,
                queryControl: CommentScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.CommentScraper,
                moduleName: Enums.TmbMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CommentScraperTutorialLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static CommentScraper _currentCommentScraper;

        public static CommentScraper GetSingletonObjectCommentScraperConfig()
        {
            return _currentCommentScraper ?? (_currentCommentScraper = new CommentScraper());
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