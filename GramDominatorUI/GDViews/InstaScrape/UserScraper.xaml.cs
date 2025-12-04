using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaScraper;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.InstaScrape
{
    public class UserScraperBase : ModuleSettingsUserControl<UserScraperViewModel, UserScraperModel>
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

    /// <summary>
    ///     Interaction logic for UserScraper.xaml
    /// </summary>
    public partial class UserScraper : UserScraperBase
    {
        private UserScraper()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: UserScraperHeader,
                footer: UserScraperFooter,
                queryControl: UserScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UserScraper,
                moduleName: GdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UserScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UserScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static UserScraper CurrentUserScraper { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUserScraper is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UserScraper GetSingeltonObjectUserScraper()
        {
            return CurrentUserScraper ?? (CurrentUserScraper = new UserScraper());
        }

        private void Chk_RequiredData(object sender, RoutedEventArgs e)
        {
            ObjViewModel.CheckAllReqData(sender);
        }
    }
}