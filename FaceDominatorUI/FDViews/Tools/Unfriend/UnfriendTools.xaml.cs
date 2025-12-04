using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FDViewModel.FriendsViewModel;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.Unfriend
{
    public class UnfriendToolsBase : ModuleSettingsUserControl<UnfriendViewModel, UnfriendModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.UnfriendOptionModel.IsAddedThroughSoftware && !Model.UnfriendOptionModel.IsAddedOutsideSoftware
                                                                  && !Model.UnfriendOptionModel.IsCustomUserList
                                                                  && !Model.UnfriendOptionModel.IsMutualFriends)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyselectAtLeastOneSource".FromResourceDictionary());
                return false;
            }

            if (Model.UnfriendOptionModel.IsFilterApplied &&
                (Model.UnfriendOptionModel.DaysBefore == 0 && Model.UnfriendOptionModel.HoursBefore == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidSourceFilter".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UnfriendModel
                        = templateModel.ActivitySettings
                            .GetActivityModelNonQueryList<UnfriendModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new UnfriendViewModel();
                ObjViewModel.UnfriendModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for UnfriendTools.xaml
    /// </summary>
    public partial class UnfriendTools
    {
        public UnfriendTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unfriend,
                FdMainModule.Friends.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.UnfriendVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.UnfriendKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }


        private static UnfriendTools CurrentUnfriendTools { get; set; }

        public static UnfriendTools GetSingeltonObjectUnfriendTools()
        {
            return CurrentUnfriendTools = CurrentUnfriendTools ?? (CurrentUnfriendTools = new UnfriendTools());
        }
    }
}