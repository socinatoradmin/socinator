using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaScraper;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.InstaScrape
{
    public class HashtagsScraperBase : ModuleSettingsUserControl<HashtagsScraperViewModel, HashtagsScraperModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (string.IsNullOrEmpty(Model.Keyword))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please enter atleast one keyword");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for HashtagsScraper.xaml
    /// </summary>
    public partial class HashtagsScraper : HashtagsScraperBase
    {
        private HashtagsScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HashtagsScraperHeader,
                footer: HashtagsScraperFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.HashtagsScraper,
                moduleName: Enums.GdMainModule.Scraper.ToString()
            );
            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.HashtagVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.HashtagKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static HashtagsScraper CurrentHashtagsScraper { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUserScraper is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static HashtagsScraper GetSingeltonObjectHashtagsScraper()
        {
            return CurrentHashtagsScraper ?? (CurrentHashtagsScraper = new HashtagsScraper());
        }
    }
}