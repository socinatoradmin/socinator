using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;

namespace RedditDominatorUI.RDViews.UrlScraper
{
    public class UserScraperBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog("Error", "Please add atleast one query");
            return false;
        }
    }


    /// <summary>
    ///     Interaction logic for UserScraper.xaml
    /// </summary>
    public sealed partial class UserScraper : UserScraperBase
    {
        public UserScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UserScraperHeader,
                footer: UserScraperFooter,
                queryControl: UserScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UserScraper,
                moduleName: Enums.RdMainModule.UserScraper.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.UserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UserScraper CurrentUserScraper { get; set; }

        public static UserScraper GetSingletonObjectUserScraper()
        {
            return CurrentUserScraper ?? (CurrentUserScraper = new UserScraper());
        }
    }
}