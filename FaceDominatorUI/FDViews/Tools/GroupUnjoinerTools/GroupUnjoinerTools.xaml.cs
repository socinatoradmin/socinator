using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.FDViewModel.Groups;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.GroupUnjoinerTools
{
    public class GroupUnJoinerToolsBase : ModuleSettingsUserControl<GroupUnjoinerViewModelNew, GroupUnjoinerModelNew>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!Model.UnfriendOptionModel.IsAddedThroughSoftware && !Model.UnfriendOptionModel.IsAddedOutsideSoftware)
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
                    ObjViewModel.GroupUnJoinerModel
                        = templateModel.ActivitySettings.GetActivityModelNonQueryList<GroupUnjoinerModelNew>(
                            ObjViewModel.Model);
                else
                    ObjViewModel = new GroupUnjoinerViewModelNew();
                ObjViewModel.GroupUnJoinerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for GroupUnjoinerTools.xaml
    /// </summary>
    public partial class GroupUnjoinerTools
    {
        public GroupUnjoinerTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.GroupUnJoiner,
                FdMainModule.Groups.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.GroupUnJoinerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.GroupUnJoinerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }

        private static GroupUnjoinerTools CurrentGroupJoinerTools { get; set; }

        public static GroupUnjoinerTools GetSingeltonObjectGroupJoinerTools()
        {
            return CurrentGroupJoinerTools ?? (CurrentGroupJoinerTools = new GroupUnjoinerTools());
        }
    }
}