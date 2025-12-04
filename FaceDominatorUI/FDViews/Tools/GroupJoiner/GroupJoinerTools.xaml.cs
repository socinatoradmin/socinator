using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.FDViewModel.Groups;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.GroupJoiner
{
    public class GroupJoinerToolsBase : ModuleSettingsUserControl<GroupJoinerViewModel, GroupJoinerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count != 0) return true;
            Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
            return false;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.GroupJoinerModel
                        = templateModel.ActivitySettings.GetActivityModel<GroupJoinerModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new GroupJoinerViewModel();
                ObjViewModel.GroupJoinerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for GroupJoinerTools.xaml
    /// </summary>
    public partial class GroupJoinerTools
    {
        public GroupJoinerTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.GroupJoiner,
                FdMainModule.Groups.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: GroupsSearchControl
            );
            // Help control links. 
            VideoTutorialLink = FdConstants.GroupJoinerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.GroupJoinerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }

        private static GroupJoinerTools CurrentGroupJoinerTools { get; set; }

        public static GroupJoinerTools GetSingeltonObjectGroupJoinerTools()
        {
            return CurrentGroupJoinerTools ?? (CurrentGroupJoinerTools = new GroupJoinerTools());
        }
    }
}