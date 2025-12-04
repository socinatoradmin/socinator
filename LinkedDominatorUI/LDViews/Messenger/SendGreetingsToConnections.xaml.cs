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
    public class SendGreetingsToConnectionsBase : ModuleSettingsUserControl<SendGreetingsToConnectionsViewModel,
        SendGreetingsToConnectionsModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (!ObjViewModel.SendGreetingsToConnectionsModel.IsCheckedBirthdayGreeting
                && !ObjViewModel.SendGreetingsToConnectionsModel.IsCheckedNewJobGreeting
                && !ObjViewModel.SendGreetingsToConnectionsModel.IsCheckedWorkAnniversaryGreeting)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfGreetingOptions".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries.Count > 0)
                //foreach (var item in ObjViewModel.SendGreetingsToConnectionsModel.ManageMessagesModel.LstQueries)
                //{
                //    if (item.Content.QueryValue != "All")
                //    {
                //        string message = null;
                //        try
                //        {
                //            message = ObjViewModel.SendGreetingsToConnectionsModel.LstDisplayManageMessagesModel
                //                .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == item.Content.QueryValue))
                //                .Select(x => x.MessagesText).ToList().GetRandomItem();
                //        }
                //        catch (Exception ex)
                //        { ex.DebugLog(); }
                //        if (message == null)
                //        {
                //            Dialog.ShowDialog("Error", "please input at least one greeting for Query [ " + item.Content.QueryValue + " ]");
                //            return false;

                //        }
                //    }
                //}

                if (ObjViewModel.SendGreetingsToConnectionsModel.LstDisplayManageMessagesModel.Count == 0)
                {
                    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                        "LangKeyPleaseInputAtleastOneMessage".FromResourceDictionary());
                    return false;
                }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for SendGreetingsToConnections.xaml
    /// </summary>
    public sealed partial class SendGreetingsToConnections
    {
        private static SendGreetingsToConnections _objSendGreetingsToConnections;

        public SendGreetingsToConnections()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: SendGreetingsToConnectionsFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.SendGreetingsToConnections,
                moduleName: LdMainModules.Messenger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SendGreetingsToConnectionsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendGreetingsToConnectionsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();

            DialogParticipation.SetRegister(this, this);
        }

        public static SendGreetingsToConnections GetSingeltonSendGreetingsToConnections()
        {
            return _objSendGreetingsToConnections ??
                   (_objSendGreetingsToConnections = new SendGreetingsToConnections());
        }
    }
}