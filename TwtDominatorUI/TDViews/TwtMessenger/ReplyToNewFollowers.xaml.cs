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
    public class ReplyToNewFollowersBase : ModuleSettingsUserControl<MessageViewModel, MessageModel>
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
    ///     Interaction logic for ReplyToNewFollowers.xaml
    /// </summary>
    public partial class ReplyToNewFollowers
    {
        private ReplyToNewFollowers()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: MessageHeader,
                footer: MessageFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.SendMessageToFollower,
                moduleName: TdMainModule.TwtMessenger.ToString()
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtMessageToNewFollowerVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtMessageToNewFollowerKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        #region SingletonObject Creation

        private static ReplyToNewFollowers objReplyToNewFollowers { get; set; }

        public static ReplyToNewFollowers GetSingletonObjectReplyToNewFollowers()
        {
            return objReplyToNewFollowers ?? (objReplyToNewFollowers = new ReplyToNewFollowers());
        }

        #endregion
    }
}