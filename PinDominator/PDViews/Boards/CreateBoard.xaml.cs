using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.Boards;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.Boards
{
    public class CreateBoardBase : ModuleSettingsUserControl<BoardViewModel, BoardModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.BoardDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one board.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for CreateBoard.xaml
    /// </summary>
    public sealed partial class CreateBoard
    {
        private static CreateBoard _objCreateBoard;

        private BoardViewModel _objBoardViewModel;

        public CreateBoard()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: CreateBoardHeader,
                footer: CreateBoardFooter,
                MainGrid: MainGrid,
                moduleName: PdMainModule.Boards.ToString(),
                activityType: ActivityType.CreateBoard
            );
            VideoTutorialLink = ConstantHelpDetails.CreateBoardVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CreateBoardKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        public BoardViewModel ObjBoardViewModel
        {
            get => _objBoardViewModel;
            set
            {
                _objBoardViewModel = value;
                OnPropertyChanged(nameof(ObjBoardViewModel));
            }
        }

        public static CreateBoard GetSingletonObjectCreateBoard()
        {
            return _objCreateBoard ?? (_objCreateBoard = new CreateBoard());
        }
    }
}