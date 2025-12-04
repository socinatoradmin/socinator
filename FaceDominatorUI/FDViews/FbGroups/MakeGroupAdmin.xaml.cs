using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDModel.GroupsModel;
using FaceDominatorCore.FDViewModel.Groups;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.FDViews.FbGroups
{
    public class MakeGroupAdminBase : ModuleSettingsUserControl<MakeAdminViewModel, MakeAdminModel>
    {
        protected override bool ValidateCampaign()
        {
            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if (selectAccountDetailsControl.GetGroupInviterDetails(Model.SelectAccountDetailsModel).GroupInviterDetails
                    .Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtLeastOneMakeAdminDetails".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }

        public override void SelectAccount()
        {
            var hiddenColumnList = new List<FbEntityTypes> { FbEntityTypes.Page };

            var listAccountIds = ObjViewModel.MakeAdminModel.SelectAccountDetailsModel.AccountsWithNetwork
                .Where(x => x.Key == SocialNetworks.Facebook).Select(y => y.Value).ToList();

            if (_footerControl.list_SelectedAccounts.Count != listAccountIds.Count)
            {
                var accountDetailsModel = ObjViewModel.MakeAdminModel.SelectAccountDetailsModel;
                accountDetailsModel =
                    RemoveUnnecessaryAccountsAsync(accountDetailsModel, _footerControl.list_SelectedAccounts);
                ObjViewModel.MakeAdminModel.SelectAccountDetailsModel = accountDetailsModel;
            }

            var newModel = ObjViewModel.MakeAdminModel.SelectAccountDetailsModel.DeepCloneObject();

            var selectAccountDetailsControl = newModel.AccountsWithNetwork.Count != 0
                ? new SelectAccountDetailsControl(newModel)
                : new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, string.Empty);

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl, "Select Account Details");

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

                    var model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model = SaveDestinationExecute(model);

                    // model = selectAccountDetailsControl.GetGroupInviterDetails(model);

                    ObjViewModel.MakeAdminModel.SelectAccountDetailsModel = model;

                    var listSelectedAccountId = model.AccountsWithNetwork
                        .Where(x => x.Key == SocialNetworks.Facebook).Select(y => y.Value).ToList();

                    var lstSelectedAccount = new List<string>();

                    listSelectedAccountId.ForEach(x =>
                    {
                        var accountName = accountsFileManager.GetAccountById(x).AccountBaseModel.UserName;
                        lstSelectedAccount.Add(accountName);
                    });

                    FooterControl_OnSelectAccountChanged(lstSelectedAccount);

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
                          selectAccountDetailsModel.AccountFriendsPair.Count == 0 &&
                          selectAccountDetailsModel.CustomDestinations.Count == 0))
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

        private SelectAccountDetailsModel RemoveUnnecessaryAccountsAsync(
            SelectAccountDetailsModel selctAccountDetailsModel, List<string> accounts)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

            var accountFriendsPair = (from detail in selctAccountDetailsModel.AccountFriendsPair
                                      let accountName = accountsFileManager.GetAccountById(detail.Key).AccountBaseModel.UserName
                                      where accounts.Contains(accountName)
                                      select new KeyValuePair<string, string>
                                          (detail.Key, detail.Value)).ToList();


            selctAccountDetailsModel.AccountFriendsPair = accountFriendsPair;

            var accountGroupPair = new List<KeyValuePair<string, string>>();
            selctAccountDetailsModel.AccountGroupPair.ForEach(x =>
            {
                var accountName = accountsFileManager.GetAccountById(x.Key).AccountBaseModel.UserName;
                if (accounts.Contains(accountName)) accountGroupPair.Add(x);
            });
            selctAccountDetailsModel.AccountGroupPair = accountGroupPair;

            selctAccountDetailsModel.ListSelectDestination.ForEach(x =>
            {
                var accountName = accountsFileManager.GetAccountById(x.AccountId).AccountBaseModel.UserName;
                if (!accounts.Contains(accountName))
                {
                    x.GroupSelectorText = $"0/{x.TotalGroups}";
                    x.FriendsSelectorText = $"0/{x.TotalFriends}";
                    x.IsAccountSelected = false;
                }
            });

            return selctAccountDetailsModel;
        }
    }

    public partial class MakeGroupAdmin
    {
        public MakeGroupAdmin()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: MakeGroupAdminHeader,
                footer: MakeGroupAdminFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.MakeAdmin,
                moduleName: FdMainModule.Groups.ToString()
            );

            // Help control links. 
            VideoTutorialLink = "";
            KnowledgeBaseLink = "";
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static MakeGroupAdmin _currentMakeGroupAdmin { get; set; }

        public static MakeGroupAdmin GetSingletonMakeGroupAdmin()
        {
            return _currentMakeGroupAdmin ?? (_currentMakeGroupAdmin = new MakeGroupAdmin());
        }
    }
}