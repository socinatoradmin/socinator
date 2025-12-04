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

namespace FaceDominatorUI.FDViews.Tools.MessageToPlaces
{
    public class MessageToPlcaesToolsBase : ModuleSettingsUserControl<MessageToPlacesViewModel, MessageToPlacesModel>
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
                    ObjViewModel.MessageToPlacesModel =
                        templateModel.ActivitySettings.GetActivityModel<MessageToPlacesModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new MessageToPlacesViewModel();
                ObjViewModel.MessageToPlacesModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class MessageToPlcaesTools
    {
        /*
                QueryContent _queryContent = new QueryContent { Content = new QueryInfo { QueryValue = "All" } };
        */

        public MessageToPlcaesTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.MessageToPlaces,
                FdMainModule.Messanger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: MessageToFanpagesSearchControl
            );

            // Help control links. 
            VideoTutorialLink = "";
            KnowledgeBaseLink = "";
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }

        private static MessageToPlcaesTools CurrentGroupJoinerTools { get; set; }

        public static MessageToPlcaesTools GetSingeltonObjectMessageToFanpageTools()
        {
            return CurrentGroupJoinerTools ?? (CurrentGroupJoinerTools = new MessageToPlcaesTools());
        }
    }
}