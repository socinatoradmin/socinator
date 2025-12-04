using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;

namespace FaceDominatorUI.FDViews.Tools.BrodcastMessage
{
    public class BrodcastMessageToolsBase : ModuleSettingsUserControl<BrodcastMessageViewModel, BrodcastMessageModel>
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
                    ObjViewModel.BrodcastMessageModel =
                        JsonConvert.DeserializeObject<BrodcastMessageModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new BrodcastMessageViewModel();
                ObjViewModel.BrodcastMessageModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class BroadcastMessageTools
    {
        /*
                QueryContent _queryContent = new QueryContent { Content = new QueryInfo { QueryValue = "All" } };
        */

        public BroadcastMessageTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BroadcastMessages,
                FdMainModule.Messanger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FindFriendsSearchControl
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.BrodcastMessageVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.BrodcastMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }

        private static BroadcastMessageTools CurrentGroupJoinerTools { get; set; }

        public static BroadcastMessageTools GetSingeltonObjectGroupJoinerTools()
        {
            return CurrentGroupJoinerTools ?? (CurrentGroupJoinerTools = new BroadcastMessageTools());
        }
    }
}