using System;
using System.Linq;
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
    public class
        BroadcastMessageConfigurationBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel
        >
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.BroadcastMessagesModel =
                        templateModel.ActivitySettings.GetActivityModel<BroadcastMessagesModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new BroadcastMessagesViewModel();
                ObjViewModel.BroadcastMessagesModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!ObjViewModel.BroadcastMessagesModel.IsCheckedBySoftware
                && !ObjViewModel.BroadcastMessagesModel.IsCheckedOutSideSoftware
                && !ObjViewModel.BroadcastMessagesModel.IsCheckedLangKeyCustomUserList)
            {
                //Dialog.ShowDialog("Error", "select at least once of the connection sources");
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.BroadcastMessagesModel.IsCheckedLangKeyCustomUserList &&
                (ObjViewModel.BroadcastMessagesModel.UrlList == null ||
                 ObjViewModel.BroadcastMessagesModel.UrlList.Count == 0))
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    string.IsNullOrEmpty(ObjViewModel.BroadcastMessagesModel.UrlInput)
                        ? "LangKeyPleaseAddProfileUrls".FromResourceDictionary()
                        : "LangKeyPleaseSaveYourCustomUsersList".FromResourceDictionary());

                return false;
            }

            if (ObjViewModel.BroadcastMessagesModel.ManageMessagesModel.LstQueries.Count <= 0)
                return true;

            foreach (var item in ObjViewModel.BroadcastMessagesModel.ManageMessagesModel.LstQueries)
            {
                if (item.Content.QueryValue == "All")
                    continue;
                try
                {
                    var message = ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessagesModel
                        .Where(x => x.SelectedQuery.Any(y =>
                            y.Content.QueryValue.ToString() == item.Content.QueryValue))
                        .Select(x => x.MessagesText).ToList().GetRandomItem();
                    if (message != null) continue;

                    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                        string.Format("LangKeyPleaseInputAtleastOneMessageForQuery".FromResourceDictionary(),
                            item.Content.QueryValue));
                    return false;
                }
                catch (Exception)
                {
                    //ignored
                }
            }

            // if have selected at least one query than enough for run a campaign hence return true 
            //Note : keep it here not place it above otherwise it will by default return true without saving selected queries
            if (ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessagesModel.Count != 0)
                return true;

            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                "LangKeyPleaseInputAtleastOneMessage".FromResourceDictionary());
            return false;
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessagesConfiguration.xaml
    /// </summary>
    public partial class BroadcastMessagesConfiguration : BroadcastMessageConfigurationBase
    {
        public BroadcastMessagesConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BroadcastMessages,
                LdMainModules.Messenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BroadcastMessagesVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BroadcastMessagesKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static BroadcastMessagesConfiguration CurrentBroadcastMessagesConfiguration { get; set; }

        public static BroadcastMessagesConfiguration GetSingeltonObjectBroadcastMessagesConfiguration()
        {
            return CurrentBroadcastMessagesConfiguration ??
                   (CurrentBroadcastMessagesConfiguration = new BroadcastMessagesConfiguration());
        }
    }
}