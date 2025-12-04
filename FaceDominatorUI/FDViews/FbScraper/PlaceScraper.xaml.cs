using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbScraper
{
    public class PlaceScraperBase : ModuleSettingsUserControl<PlaceScraperViewModel, PlaceScraperModel>
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
    ///     Interaction logic for FanapgeLiker.xaml
    /// </summary>
    public partial class PlaceScraper
    {
        public PlaceScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: MessageToFanpageFooter,
                queryControl: MessageToFanpagesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.PlaceScraper,
                moduleName: FdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.PlaceScrapperVideoTutorialsLink;
            KnowledgeBaseLink = "";
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static PlaceScraper CurrentMessageToPlaces { get; set; }

        public static PlaceScraper GetSingeltonObjectMessageToPlaces()
        {
            return CurrentMessageToPlaces ?? (CurrentMessageToPlaces = new PlaceScraper());
        }
    }
}