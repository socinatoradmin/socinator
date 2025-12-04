using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.Instachat;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Instachats
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
    public partial class BroadcastMessages : BroadcastMessagesBase
    {
        private static BroadcastMessages ObjBroadcastMessages;

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
                moduleName: Enums.GdMainModule.InstaChat.ToString()
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
            return ObjBroadcastMessages ?? (ObjBroadcastMessages = new BroadcastMessages());
        }
    }
}