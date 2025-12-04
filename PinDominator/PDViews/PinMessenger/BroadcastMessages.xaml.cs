using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinMessenger;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinMessenger
{
    public class BroadcastMessagesBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
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

            if (ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one Message.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for PinMessenger.xaml
    /// </summary>
    public sealed partial class BroadcastMessages
    {
        private static BroadcastMessages _objBroadcastMessages;

        public BroadcastMessages()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: BroadcastMessagesHeader,
                footer: BroadcastMessagesFooter,
                queryControl: BroadcastMessagesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.BroadcastMessages,
                moduleName: PdMainModule.PinMessenger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BroadCastMessageVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BroadCastMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static BroadcastMessages GetSingletonObjectBroadcastMessages()
        {
            return _objBroadcastMessages ?? (_objBroadcastMessages = new BroadcastMessages());
        }
    }
}