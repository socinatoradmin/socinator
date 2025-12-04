using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel.Startup;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for MakeAdmin.xaml
    /// </summary>
    public partial class MakeAdmin : UserControl
    {
        public IMakeAdminViewModel ViewModel;

        public MakeAdmin(IMakeAdminViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
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
                Dialog.ShowDialog("Warning", "Please select accounts, You have selected only destinations !");
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
                        Dialog.ShowDialog("Warning", "Please select both account group and friends for accounts!");
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
                Dialog.ShowDialog("Warning", "Please select destination!");
                return new SelectAccountDetailsModel();
            }

            return selectAccountDetailsModel;
        }

        private void SelectAccountDetails_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = InstanceProvider.GetInstance<ISelectActivityViewModel>();

            SelectAccountDetailsControl selectAccountDetailsControl;

            var hiddenColumnList = new List<FbEntityTypes> {FbEntityTypes.Page};

            if (ViewModel.SelectAccountDetailsModel.AccountsWithNetwork.Count != 0)
            {
                var newModel = ViewModel.SelectAccountDetailsModel.DeepCloneObject();

                newModel.IsDisplaySingleAccount = true;

                newModel.DisplayAccount = viewModel.SelectAccount.AccountBaseModel.UserName;

                selectAccountDetailsControl = new SelectAccountDetailsControl(newModel);
            }
            else
            {
                var displayAccount = viewModel.SelectAccount.AccountBaseModel.UserName;
                selectAccountDetailsControl =
                    new SelectAccountDetailsControl(hiddenColumnList, displayAccount, true, string.Empty);
            }

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl, "Select Account Details");

            window.Closed += (senders, events) => { ViewModel.IsSelctDetails = false; };

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    var model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model = SaveDestinationExecute(model);

                    //model = selectAccountDetailsControl.GetPageInviterDetails(model);

                    ViewModel.SelectAccountDetailsModel = model;

                    ViewModel.IsSelctDetails = false;

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