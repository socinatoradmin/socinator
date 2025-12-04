using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.Boards;
using System;

namespace PinDominator.PDViews.Tools.CreateBoard
{
    public class CreateBoardConfigurationBase : ModuleSettingsUserControl<BoardViewModel, BoardModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.BoardDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one board.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.BoardModel =
                        templateModel.ActivitySettings.GetActivityModel<BoardModel>(ObjViewModel.Model, true);
                else if (ObjViewModel == null)
                    ObjViewModel = new BoardViewModel();

                ObjViewModel.BoardModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for CreateBoardConfiguration.xaml
    /// </summary>
    public partial class CreateBoardConfiguration
    {
        public CreateBoardConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.CreateBoard,
                Enums.PdMainModule.Boards.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.CreateBoardVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CreateBoardKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }
    }
}