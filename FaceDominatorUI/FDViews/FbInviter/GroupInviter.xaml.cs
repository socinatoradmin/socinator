using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.InviterModel;
using FaceDominatorCore.FDViewModel.Inviter;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.FDViews.FbInviter
{
    public class GroupInviterBase : ModuleSettingsUserControl<GroupInviterViewModel, GroupInviterModel>
    {
        protected override bool ValidateCampaign()
        {
            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if (selectAccountDetailsControl.GetGroupInviterDetails(Model.SelectAccountDetailsModel).GroupInviterDetails
                    .Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtLeastOneInviterDetails".FromResourceDictionary());
                return false;
            }

            if (Model.InviterOptionsModel.IsSendInvitationWithNote &&
                string.IsNullOrEmpty(Model.InviterOptionsModel.Note))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterANote".FromResourceDictionary());
                return false;
            }


            return base.ValidateCampaign();
        }

        public override void SelectAccount()
        {
            var hiddenColumnList = new List<FbEntityTypes> { FbEntityTypes.Page };


            var listAccountIds = ObjViewModel.GroupInviterModel.SelectAccountDetailsModel.AccountsWithNetwork
                .Where(x => x.Key == SocialNetworks.Facebook).Select(y => y.Value).ToList();


            if (_footerControl.list_SelectedAccounts.Count != listAccountIds.Count)
            {
                var accountDetailsModel = ObjViewModel.GroupInviterModel.SelectAccountDetailsModel;
                accountDetailsModel =
                    RemoveUnnecessaryAccountsAsync(accountDetailsModel, _footerControl.list_SelectedAccounts);
                ObjViewModel.GroupInviterModel.SelectAccountDetailsModel = accountDetailsModel;
            }

            var newModel = ObjViewModel.GroupInviterModel.SelectAccountDetailsModel.DeepCloneObject();

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

                    //model = selectAccountDetailsControl.GetGroupInviterDetails(model);

                    ObjViewModel.GroupInviterModel.SelectAccountDetailsModel = model;

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


    /// <summary>
    ///     Interaction logic for GroupInviter.xaml
    /// </summary>
    public partial class GroupInviter
    {
        private GroupInviter()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: GroupInviterFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.GroupInviter,
                moduleName: FdMainModule.Inviter.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.GroupInviterVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.GroupInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static GroupInviter CurrentGroupInviter { get; set; }

        public static GroupInviter GetSingeltonObjectGroupInviter()
        {
            return CurrentGroupInviter ?? (CurrentGroupInviter = new GroupInviter());
        }

        #region OldEvents

        // private void GroupInviterFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        //=> base.CreateCampaign();

        // private void GroupInviterFooter_UpdateCampaignChanged(object sender, RoutedEventArgs e)
        // => base.UpdateCampaign();


        // private void Header_OnCancelEditClick(object sender, RoutedEventArgs e)
        // {
        //     base.HeaderControl_OnCancelEditClick(sender, e);
        //     TabSwitcher.GoToCampaign();
        // }


        // private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        // {
        //     HelpFlyout.IsOpen = true;
        // } 

        #endregion
    }
}