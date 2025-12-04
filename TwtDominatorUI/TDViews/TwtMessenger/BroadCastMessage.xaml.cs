using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtMessenger;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.TwtMessenger
{
    public class BroadCastMessageBase : ModuleSettingsUserControl<BroadCastMessageViewModel, MessageModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.MessageSetting.IsChkRandomFollowers && !Model.MessageSetting.IsChkCustomFollowers)
            {
                Dialog.ShowDialog(this, "Error", "Please select one message source.");
                return false;
            }

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please enter atleast one message");
                return false;
            }

            return true;
        }
    }


    /// <summary>
    ///     Interaction logic for BroadCastMessage.xaml
    /// </summary>
    public partial class BroadCastMessage : BroadCastMessageBase
    {
        private BroadCastMessage()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: MessageHeader,
                footer: BroadcastMessageFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.BroadcastMessages,
                moduleName: TdMainModule.TwtMessenger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtMessengerVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtMessengerKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            ObjViewModel.AddBroadCastQueries();
            DialogParticipation.SetRegister(this, this);
        }


        #region SingletonObject Creation

        private static BroadCastMessage objBroadCastMessage { get; set; }

        public static BroadCastMessage GetSingletonObjectBroadCastMessage()
        {
            return objBroadCastMessage ?? (objBroadCastMessage = new BroadCastMessage());
        }

        #endregion
    }
}