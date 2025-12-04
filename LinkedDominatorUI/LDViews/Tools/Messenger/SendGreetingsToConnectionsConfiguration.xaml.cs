using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDViewModel.Messenger;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Messenger
{
    public class SendGreetingsToConnectionsConfigurationBase : ModuleSettingsUserControl<
        SendGreetingsToConnectionsViewModel, SendGreetingsToConnectionsModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendGreetingsToConnectionsModel =
                        templateModel.ActivitySettings.GetActivityModel<SendGreetingsToConnectionsModel>(
                            ObjViewModel.Model, true);
                else
                    ObjViewModel = new SendGreetingsToConnectionsViewModel();
                ObjViewModel.SendGreetingsToConnectionsModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!ObjViewModel.SendGreetingsToConnectionsModel.IsCheckedBirthdayGreeting
                && !ObjViewModel.SendGreetingsToConnectionsModel.IsCheckedNewJobGreeting
                && !ObjViewModel.SendGreetingsToConnectionsModel.IsCheckedWorkAnniversaryGreeting
            )

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
                //        string Message = null;
                //        try
                //        {
                //            Message = ObjViewModel.SendGreetingsToConnectionsModel.LstDisplayManageMessagesModel
                //                .Where(x => x.SelectedQuery.Any(y => y.Content.QueryValue.ToString() == item.Content.QueryValue))
                //                .Select(x => x.MessagesText).ToList().GetRandomItem();
                //        }
                //        catch (Exception ex)
                //        { }
                //        if (Message == null)
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
    ///     Interaction logic for SendGreetingsToConnectionsConfiguration.xaml
    /// </summary>
    public partial class SendGreetingsToConnectionsConfiguration : SendGreetingsToConnectionsConfigurationBase
    {
        public SendGreetingsToConnectionsConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendGreetingsToConnections,
                LdMainModules.Messenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SendMessageToNewConnectionVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendMessageToNewConnectionKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static SendGreetingsToConnectionsConfiguration CurrentSendGreetingsToConnectionsConfiguration
        {
            get;
            set;
        }

        public static SendGreetingsToConnectionsConfiguration
            GetSingeltonObjectSendGreetingsToConnectionsConfiguration()
        {
            return CurrentSendGreetingsToConnectionsConfiguration ?? (CurrentSendGreetingsToConnectionsConfiguration =
                       new SendGreetingsToConnectionsConfiguration());
        }
    }
}