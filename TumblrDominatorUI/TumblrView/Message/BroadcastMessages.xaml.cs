using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Message;

namespace TumblrDominatorUI.TumblrView.Message
{
    public class BroadcastMessagesBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error", "Please provide message(s)");
                return false;
            }

            return true;
        }

        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please add at least one query.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for BroadcastMessages.xaml
    /// </summary>
    public sealed partial class BroadcastMessages
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
                moduleName: Enums.TmbMainModule.Message.ToString()
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