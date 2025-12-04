using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.Messages;

namespace QuoraDominatorUI.QDViews.Messages
{
    public class BroadcastMessagesBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog("Input Error", "Please provide message(s)");
                return false;
            }

            return true;
        }

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
    ///     Interaction logic for BroadcastMessages.xaml
    /// </summary>
    public partial class BroadcastMessages
    {
        private static BroadcastMessages _objBroadcastMessages;

        public BroadcastMessages()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: BrodCastMessageFooter,
                queryControl: BroadcastMessagesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.BroadcastMessages,
                moduleName: QdMainModule.Messages.ToString()
            );
            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BroadcastMessagesVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BroadcastMessagesKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        public static BroadcastMessages GetSingeltonBroadcastMessages()
        {
            return _objBroadcastMessages ?? (_objBroadcastMessages = new BroadcastMessages());
        }
    }
}