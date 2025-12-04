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
    public class BroadcastMessagesBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (!ObjViewModel.BroadcastMessagesModel.IsCheckedBySoftware
                && !ObjViewModel.BroadcastMessagesModel.IsCheckedOutSideSoftware
                && !ObjViewModel.BroadcastMessagesModel.IsCheckedLangKeyCustomUserList &&
                !ObjViewModel.BroadcastMessagesModel.IsGroup
                && !ObjViewModel.BroadcastMessagesModel.IsFollower)

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }
            //if (ObjViewModel.BroadcastMessagesModel.IsCheckedLangKeyCheckedLangKeyCustomUserListList && (ObjViewModel.BroadcastMessagesModel.UrlList == null || ObjViewModel.BroadcastMessagesModel.UrlList.Count == 0))
            //{
            //    if (string.IsNullOrEmpty(ObjViewModel.BroadcastMessagesModel.UrlInput))
            //    {
            //        Dialog.ShowDialog("Error", "please input at least one custom user profile url");
            //            }
            //    else
            //    {
            //        Dialog.ShowDialog("Error", "please save your custom user profile url(S)");
            //            }

            //    return false;
            //}

            if (ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseInputAtleastOneMessage".FromResourceDictionary());
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
        private static BroadcastMessages _objBroadcastMessages;

        public BroadcastMessages()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: BrodCastMessageFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.BroadcastMessages,
                moduleName: LdMainModules.Messenger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BroadcastMessagesVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BroadcastMessagesKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);

            base.SetDataContext();
        }

        public static BroadcastMessages GetSingletonBroadcastMessages()
        {
            return _objBroadcastMessages ?? (_objBroadcastMessages = new BroadcastMessages());
        }
    }
}