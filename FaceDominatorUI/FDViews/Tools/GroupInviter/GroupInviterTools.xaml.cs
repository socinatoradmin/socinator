using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.FDViewModel.Inviter;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.FDViews.Tools.GroupInviter
{
    public class GroupInviterToolsBase : ModuleSettingsUserControl<GroupInviterViewModel, GroupInviterModel>
    {
        protected override bool ValidateExtraProperty()
        {
            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if (selectAccountDetailsControl.GetGroupInviterDetails(Model.SelectAccountDetailsModel).GroupInviterDetails
                    .Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one inviter details.");
                return false;
            }

            if (Model.InviterOptionsModel.IsSendInvitationWithNote &&
                string.IsNullOrEmpty(Model.InviterOptionsModel.Note))
            {
                Dialog.ShowDialog(this, "Error", "Please enter a note.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.GroupInviterModel
                        = templateModel.ActivitySettings.GetActivityModelNonQueryList<GroupInviterModel>(ObjViewModel
                            .Model);
                else
                    ObjViewModel = new GroupInviterViewModel();

                ObjViewModel.GroupInviterModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for GroupInviterTools.xaml
    /// </summary>
    public partial class GroupInviterTools
    {
        public GroupInviterTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.GroupInviter,
                FdMainModule.Inviter.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.GroupInviterVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.GroupInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static GroupInviterTools CurrentGroupInviterTools { get; set; }

        public static GroupInviterTools GetSingeltonObjectGroupInviterTools()
        {
            return CurrentGroupInviterTools ?? (CurrentGroupInviterTools = new GroupInviterTools());
        }

        private SelectAccountDetailsModel SaveDestinationExecute(SelectAccountDetailsModel selectAccountDetailsModel)
        {
            selectAccountDetailsModel.SelectedAccountIds.Clear();
            selectAccountDetailsModel.PublishOwnWallAccount.Clear();
            selectAccountDetailsModel.AccountsWithNetwork.Clear();

            var selectedAccountsCount =
                selectAccountDetailsModel.ListSelectDestination.Count(x => x.IsAccountSelected);

            if (selectedAccountsCount == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseSelectAccountsSelectedOnlyDestinations".FromResourceDictionary());
                return new SelectAccountDetailsModel();
            }

            selectAccountDetailsModel.ListSelectDestination.ForEach(x =>
            {
                // Check the account has been selected or not
                if (x.IsAccountSelected)
                {
                    if (!(selectAccountDetailsModel.AccountGroupPair.Count == 0 ||
                          selectAccountDetailsModel.AccountFriendsPair.Count == 0))
                    {
                        selectAccountDetailsModel.SelectedAccountIds.Add(x.AccountId);
                        selectAccountDetailsModel.AccountsWithNetwork.Add(
                            new KeyValuePair<SocialNetworks, string>(x.SocialNetworks, x.AccountId));
                    }
                    else
                    {
                        Dialog.ShowDialog(Application.Current.MainWindow,
                            "LangKeyWarning".FromResourceDictionary(),
                            "LangKeySelectBothAccountGroupAndFriendsForAccounts".FromResourceDictionary());
                    }
                }
                else
                {
                    // If account has selected, remove from selected lists
                    var unwantedGroups = selectAccountDetailsModel.AccountGroupPair.Where(y => y.Key == x.AccountId)
                        .Select(y => y.Key);
                    selectAccountDetailsModel.AccountGroupPair.RemoveAll(z => unwantedGroups.Contains(z.Key));

                    var unwantedPages = selectAccountDetailsModel.AccountPagesBoardsPair
                        .Where(y => y.Key == x.AccountId).Select(y => y.Key);
                    selectAccountDetailsModel.AccountPagesBoardsPair.RemoveAll(z => unwantedPages.Contains(z.Key));

                    selectAccountDetailsModel.DestinationDetailsModels.RemoveAll(z =>
                        z.AccountId == x.AccountId);
                }
            });

            selectAccountDetailsModel.AccountGroupPair =
                selectAccountDetailsModel.AccountGroupPair.Distinct().ToList();


            selectAccountDetailsModel.SelectedAccountIds =
                selectAccountDetailsModel.SelectedAccountIds.Distinct().ToList();

            selectAccountDetailsModel.AccountPagesBoardsPair =
                selectAccountDetailsModel.AccountPagesBoardsPair.Distinct().ToList();

            selectAccountDetailsModel.AccountFriendsPair =
                selectAccountDetailsModel.AccountFriendsPair.Distinct().ToList();

            if (selectAccountDetailsModel.AccountGroupPair.Count == 0 &&
                selectAccountDetailsModel.AccountPagesBoardsPair.Count == 0 &&
                selectAccountDetailsModel.AccountFriendsPair.Count == 0 &&
                selectAccountDetailsModel.PublishOwnWallAccount.Count == 0
            )
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeySelectDestination".FromResourceDictionary());
                return new SelectAccountDetailsModel();
            }

            return selectAccountDetailsModel;
        }

        private void SelectAccountDetails_Click(object sender, RoutedEventArgs e)
        {
            SelectAccountDetailsControl selectAccountDetailsControl;

            var hiddenColumnList = new List<FbEntityTypes> { FbEntityTypes.Page };


            if (ObjViewModel.GroupInviterModel.SelectAccountDetailsModel.AccountsWithNetwork.Count != 0)
            {
                var newModel = ObjViewModel.GroupInviterModel.SelectAccountDetailsModel.DeepCloneObject();

                newModel.IsDisplaySingleAccount = true;

                newModel.DisplayAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;

                selectAccountDetailsControl = new SelectAccountDetailsControl(newModel);
            }
            else
            {
                var displayAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
                selectAccountDetailsControl =
                    new SelectAccountDetailsControl(hiddenColumnList, displayAccount, true, string.Empty);
            }

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl,
                "LangKeySelectAccountDetails".FromResourceDictionary());

            window.Closed += (senders, events) => { ObjViewModel.GroupInviterModel.IsSelctDetails = false; };

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    var model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model = SaveDestinationExecute(model);

                    ObjViewModel.GroupInviterModel.SelectAccountDetailsModel = model;

                    ObjViewModel.GroupInviterModel.IsSelctDetails = false;

                    window.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            window.ShowDialog();
        }
    }
}