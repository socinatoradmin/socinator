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
    public class BoardScraperBase : ModuleSettingsUserControl<BoardScraperViewModel, BoardScraperModel>
    {
    }

    /// <summary>
    ///     Interaction logic for BoardScraper.xaml
    /// </summary>
    public sealed partial class BoardScraper
    {
        private BoardScraperViewModel _objBoardScraperViewModel;

        private BoardScraper()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: BoardScraperHeaderControl,
                footer: BoardScraperFooterControl,
                queryControl: BoardScraperSearchQueryControl,
                MainGrid: MainGrid,
                activityType: ActivityType.BoardScraper,
                moduleName: PdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BoardScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BoardScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static BoardScraper CurrentBoardScraper { get; set; }

        public BoardScraperViewModel ObjBoardScraperViewModel
        {
            get => _objBoardScraperViewModel;
            set
            {
                _objBoardScraperViewModel = value;
                OnPropertyChanged(nameof(ObjBoardScraperViewModel));
            }
        }

        public static BoardScraper GetSingeltonObjectBoardScraper()
        {
            return CurrentBoardScraper ?? (CurrentBoardScraper = new BoardScraper());
        }
    }
}