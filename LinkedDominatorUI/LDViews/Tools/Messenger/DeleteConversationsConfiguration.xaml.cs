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
    /// <summary>
    ///     Interaction logic for DeleteConversationsConfiguration.xaml
    /// </summary>
    //public partial class DeleteConversationsConfiguration : UserControl
    //{
    //    public DeleteConversationsConfiguration()
    //    {
    //        InitializeComponent();
    //    }
    //}
    public class
        DeleteConversationsConfigurationBase : ModuleSettingsUserControl<DeleteConversationsViewModel,
            DeleteConversationsModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.DeleteConversationsModel =
                        templateModel.ActivitySettings.GetActivityModel<DeleteConversationsModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new DeleteConversationsViewModel();
                ObjViewModel.DeleteConversationsModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        //protected override bool ValidateExtraProperty()
        //{
        //    // Check queries
        //    //if (!ObjViewModel.DeleteConversationsModel.IsCheckedBySoftware
        //    //    && !ObjViewModel.DeleteConversationsModel.IsCheckedOutSideSoftware
        //    //    && !ObjViewModel.BroadcastMessagesModel.IsCheckedLangKeyCustomUserList)
        //    //{
        //    //    //Dialog.ShowDialog("Error", "select at least once of the connection sources");
        //    //    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(), "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
        //    //    return false;
        //    //}

        //    //if (ObjViewModel.BroadcastMessagesModel.IsCheckedLangKeyCustomUserList && (ObjViewModel.BroadcastMessagesModel.UrlList == null || ObjViewModel.BroadcastMessagesModel.UrlList.Count == 0))
        //    //{
        //    //    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
        //    //        string.IsNullOrEmpty(ObjViewModel.BroadcastMessagesModel.UrlInput)
        //    //            ? "LangKeyPleaseAddProfileUrls".FromResourceDictionary()
        //    //            : "LangKeyPleaseSaveYourCustomUsersList".FromResourceDictionary());

        //    //    return false;
        //    //}

        //    //if (ObjViewModel.DeleteConversationsModel.ManageMessagesModel.LstQueries.Count <= 0)
        //    //    return true;

        //    //foreach (var item in ObjViewModel.BroadcastMessagesModel.ManageMessagesModel.LstQueries)
        //    //{
        //    //    if (item.Content.QueryValue == "All")
        //    //        continue;
        //    //    try
        //    //    {
        //    //        var message = ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessagesModel
        //    //            .Where(x => x.SelectedQuery.Any(y =>
        //    //                y.Content.QueryValue.ToString() == item.Content.QueryValue))
        //    //            .Select(x => x.MessagesText).ToList().GetRandomItem();
        //    //        if (message != null) continue;

        //    //        Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
        //    //            string.Format("LangKeyPleaseInputAtleastOneMessageForQuery".FromResourceDictionary(), item.Content.QueryValue));
        //    //        return false;
        //    //    }
        //    //    catch (Exception)
        //    //    {
        //    //        //ignored
        //    //    }
        //    //}

        //    //// if have selected at least one query than enough for run a campaign hence return true 
        //    ////Note : keep it here not place it above otherwise it will by default return true without saving selected queries
        //    //if (ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessagesModel.Count != 0)
        //    //    return true;

        //    //Dialog.ShowDialog("LangKeyError".FromResourceDictionary(), "LangKeyPleaseInputAtleastOneMessage".FromResourceDictionary());
        //    //return false;

        //}
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessagesConfiguration.xaml
    /// </summary>
    public partial class DeleteConversationsConfiguration : DeleteConversationsConfigurationBase
    {
        public DeleteConversationsConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.DeleteConversations,
                LdMainModules.Messenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.DeleteConversationVideoTutorialLink;
            KnowledgeBaseLink = ConstantHelpDetails.DeleteConversationKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static DeleteConversationsConfiguration CurrentDeleteConversationsConfiguration { get; set; }

        public static DeleteConversationsConfiguration GetSingeltonObjectDeleteConversationsConfiguration()
        {
            return CurrentDeleteConversationsConfiguration ??
                   (CurrentDeleteConversationsConfiguration = new DeleteConversationsConfiguration());
        }
    }
}