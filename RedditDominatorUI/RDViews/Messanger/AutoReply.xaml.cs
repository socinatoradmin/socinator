using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.Messanger
{
    public class AutoReplyBase : ModuleSettingsUserControl<AutoReplyViewModel, AutoReplyModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (string.IsNullOrEmpty(Model.Message))
            {
                Dialog.ShowDialog(this, "Error", "Please enter atleast one message");
                return false;
            }
            if (string.IsNullOrEmpty(Model.SpecificWord))
            {
                Dialog.ShowDialog(this, "Error", "Please enter atleast one specific word");
                return false;
            }

            return true;
        }
    }
    /// <summary>
    /// Interaction logic for AutoReply.xaml
    /// </summary>
    public sealed partial class AutoReply : AutoReplyBase
    {
        public AutoReply()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: MessageHeader,
                footer: MessageFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.AutoReplyToNewMessage,
                moduleName: RdMainModule.Messanger.ToString()
            );

            // Help control links. 
            //VideoTutorialLink = TDHelpDetails.TwtReplyVideoTutorialsLink;
            //KnowledgeBaseLink = TDHelpDetails.TwtReplyKnowledgeBaseLink;
            //ContactSupportLink = TDHelpDetails.ContactLink;

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
