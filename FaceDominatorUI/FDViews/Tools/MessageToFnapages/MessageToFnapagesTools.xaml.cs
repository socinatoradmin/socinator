using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.MessageToFnapages
{
    public class
        MessageToFnapagesToolsBase : ModuleSettingsUserControl<MessageToFanpagesViewModel, MessageToFanpagesModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneMessage".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.MessageToFanpagesModel =
                        templateModel.ActivitySettings.GetActivityModel<MessageToFanpagesModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new MessageToFanpagesViewModel();
                ObjViewModel.MessageToFanpagesModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for BroadcastMessageTools.xaml
    /// </summary>
    public partial class MessageToFnapagesTools
    {
        /*
                QueryContent _queryContent = new QueryContent { Content = new QueryInfo { QueryValue = "All" } };
        */

        public MessageToFnapagesTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.MessageToFanpages,
                FdMainModule.Messanger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: MessageToFanpagesSearchControl
            );

            // Help control links. 
            VideoTutorialLink = "";
            KnowledgeBaseLink = "https://help.socinator.com/support/solutions/articles/42000088686-facebook-auto-send-message-to-fan-pages";
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }

        private static MessageToFnapagesTools CurrentGroupJoinerTools { get; set; }

        public static MessageToFnapagesTools GetSingeltonObjectGroupJoinerTools()
        {
            return CurrentGroupJoinerTools ?? (CurrentGroupJoinerTools = new MessageToFnapagesTools());
        }
    }
}