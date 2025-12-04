using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinScraper;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinScrape
{
    public class PinScraperBase : ModuleSettingsUserControl<PinScraperViewModel, PinScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for PinScraper.xaml
    /// </summary>
    public sealed partial class PinScraper
    {
        private PinScraperViewModel _objPinScraperViewModel;

        private PinScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: PinScraperHeaderControl,
                footer: PinScraperFooterControl,
                queryControl: PinScraperSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.PinScraper,
                moduleName: PdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.PinScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.PinScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static PinScraper CurrentPinScraper { get; set; }

        public PinScraperViewModel ObjPinScraperViewModel
        {
            get => _objPinScraperViewModel;
            set
            {
                _objPinScraperViewModel = value;
                OnPropertyChanged(nameof(ObjPinScraperViewModel));
            }
        }

        public static PinScraper GetSingletonObjectPinScraper()
        {
            return CurrentPinScraper ?? (CurrentPinScraper = new PinScraper());
        }
    }
}