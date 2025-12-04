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
    public class AutoReplyBase : ModuleSettingsUserControl<MessageViewModel, MessageModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please enter atleast one message");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReply.xaml
    /// </summary>
    public partial class AutoReply : AutoReplyBase
    {
        private AutoReply()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: MessageHeader,
                footer: MessageFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.AutoReplyToNewMessage,
                moduleName: TdMainModule.TwtMessenger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtReplyVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtReplyKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        #region SingletonObject Creation

        private static AutoReply objAutoReply { get; set; }

        public static AutoReply GetSingletonObjectAutoReply()
        {
            return objAutoReply ?? (objAutoReply = new AutoReply());
        }

        #endregion
    }
}