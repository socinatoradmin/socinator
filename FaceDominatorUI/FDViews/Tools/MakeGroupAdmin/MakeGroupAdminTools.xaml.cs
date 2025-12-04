using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.FDViewModel.Groups;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.FDViews.Tools.MakeGroupAdmin
{
    public class MakeGroupAdminToolsBase : ModuleSettingsUserControl<MakeAdminViewModel, MakeAdminModel>
    {
        protected override bool ValidateCampaign()
        {
            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if (selectAccountDetailsControl.GetGroupInviterDetails(Model.SelectAccountDetailsModel).GroupInviterDetails
                    .Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one Make Admin details.");
                return false;
            }

            return base.ValidateCampaign();
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.MakeAdminModel
                        = templateModel.ActivitySettings.GetActivityModelNonQueryList<MakeAdminModel>(
                            ObjViewModel.Model);

                ObjViewModel.MakeAdminModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for MakeGroupAdminTools.xaml
    /// </summary>
    public partial class MakeGroupAdminTools
    {
        public MakeGroupAdminTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.MakeAdmin,
                FdMainModule.Groups.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = "";
            KnowledgeBaseLink = "";
            ContactSupportLink = "";
        }

        private static MakeGroupAdminTools CurrentMakeGroupAdminTools { get; set; }

        public static MakeGroupAdminTools GetSingeltonObjectMakeGroupAdminTools()
        {
            return CurrentMakeGroupAdminTools ?? (CurrentMakeGroupAdminTools = new MakeGroupAdminTools());
        }

        private void SelectAccountDetails_Click(object sender, RoutedEventArgs e)
        {
            SelectAccountDetailsControl selectAccountDetailsControl;

            var hiddenColumnList = new List<FbEntityTypes> { FbEntityTypes.Page };


            if (ObjViewModel.MakeAdminModel.SelectAccountDetailsModel.AccountsWithNetwork.Count != 0)
            {
                var newModel = ObjViewModel.MakeAdminModel.SelectAccountDetailsModel.DeepCloneObject();

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

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl, "Select Account Details");

            window.Closed += (senders, events) =>
            {
                // ObjViewModel.MakeAdminModel.IsSelctDetails = false;
            };

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    var model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model = SaveDestinationExecute(model);

                    //model = selectAccountDetailsControl.GetPageInviterDetails(model);

                    ObjViewModel.MakeAdminModel.SelectAccountDetailsModel = model;

                    //ObjViewModel.MakeAdminModel.IsSelctDetails = false;

                    window.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            window.ShowDialog();
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
                    "Warning", "Please select accounts, You have selected only destinations !");
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
                            "Warning", "Please select both account group and friends for accounts!");
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
                    "Warning", "Please select destination!");
                return new SelectAccountDetailsModel();
            }

            return selectAccountDetailsModel;
        }
    }
}