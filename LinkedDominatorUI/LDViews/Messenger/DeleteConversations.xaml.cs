using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDViewModel.Messenger;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Messenger
{
    /// <summary>
    ///     Interaction logic for DeleteConversations.xaml
    /// </summary>
    //public partial class DeleteConversations : UserControl
    //{
    //    public DeleteConversations()
    //    {
    //        InitializeComponent();
    //    }
    //}
    public class
        DeleteConversationsBase : ModuleSettingsUserControl<DeleteConversationsViewModel, DeleteConversationsModel>
    {
    }

    /// <summary>
    ///     Interaction logic for BroadcastMessages.xaml
    /// </summary>
    public partial class DeleteConversations : DeleteConversationsBase
    {
        private static DeleteConversations _deleteConversations;

        public DeleteConversations()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: DeleteConversationsFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.DeleteConversations,
                moduleName: LdMainModules.Messenger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BroadcastMessagesVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BroadcastMessagesKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);

            base.SetDataContext();
        }

        public static DeleteConversations GetSingletonDeleteConversations()
        {
            return _deleteConversations ?? (_deleteConversations = new DeleteConversations());
        }
    }
}