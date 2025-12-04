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
    public class PageInviterBase : ModuleSettingsUserControl<PageInviterViewModel, FanpageInviterModel>
    {
        protected override bool ValidateCampaign()
        {
            var selectAccountDetailsControl = new SelectAccountDetailsModel();

            if (Model.InviterDetailsModel.IsProfileUrl
                && selectAccountDetailsControl.GetPageInviterDetails(Model.SelectAccountDetailsModel).PageInviterDetails
                    .Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtLeastOneInviterDetails".FromResourceDictionary());
                return false;
            }

            if (!Model.InviterDetailsModel.IsPostUrl && !Model.InviterDetailsModel.IsRandomPosts
                                                     && !Model.InviterDetailsModel.IsSpecificPosts)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one post option.");
                return false;
            }

            if (!Model.InviterDetailsModel.IsProfileUrl && Model.InviterDetailsModel.IsSpecificPosts &&
                Model.InviterDetailsModel.ListPostUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one Specific post.");
                return false;
            }

            return base.ValidateCampaign();
        }

        public override void SelectAccount()
        {
            if (!Model.InviterDetailsModel.IsProfileUrl && !Model.InviterDetailsModel.IsPostUrl)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one inviter options.");
                return;
            }

            SelectAccountDetailsControl selectAccountDetailsControl;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Group
            };


            if (!ObjViewModel.FanpageInviterModel.InviterDetailsModel.IsProfileUrl)
            {
                hiddenColumnList.Add(FbEntityTypes.Friend);
                hiddenColumnList.Add(FbEntityTypes.CustomDestination);
            }

            var listAccountIds = ObjViewModel.FanpageInviterModel.SelectAccountDetailsModel.AccountsWithNetwork
                .Where(x => x.Key == SocialNetworks.Facebook).Select(y => y.Value).ToList();


            if (_footerControl.list_SelectedAccounts.Count != listAccountIds.Count)
            {
                var accountDetailsModel = ObjViewModel.FanpageInviterModel.SelectAccountDetailsModel;
                accountDetailsModel =
                    RemoveUnnecessaryAccountsAsync(accountDetailsModel, _footerControl.list_SelectedAccounts);
                ObjViewModel.FanpageInviterModel.SelectAccountDetailsModel = accountDetailsModel;
            }

            var newModel = ObjViewModel.FanpageInviterModel.SelectAccountDetailsModel.DeepCloneObject();


            if (newModel.AccountsWithNetwork.Count != 0)
            {
                newModel.CustomDestinationColWidth =
                    !ObjViewModel.FanpageInviterModel.InviterDetailsModel.IsProfileUrl ? "0" : "130";
                newModel.FriendColWidth =
                    !ObjViewModel.FanpageInviterModel.InviterDetailsModel.IsProfileUrl ? "0" : "130";

                selectAccountDetailsControl = new SelectAccountDetailsControl(newModel, true);
            }
            else
            {
                selectAccountDetailsControl =
                    new SelectAccountDetailsControl(hiddenColumnList, string.Empty, false, string.Empty, true);
            }

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl, "Select Account Details");

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();

                    var model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model = SaveDestinationExecute(model);

                    //model = selectAccountDetailsControl.GetPageInviterDetails(model);

                    ObjViewModel.FanpageInviterModel.SelectAccountDetailsModel = model;

                    var listSelectedAccountId = model.AccountsWithNetwork.Where(x => x.Key == SocialNetworks.Facebook)
                        .Select(y => y.Value).ToList();

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
                    if (!(selectAccountDetailsModel.AccountGroupPair.Count == 0 &&
                          selectAccountDetailsModel.AccountPagesBoardsPair.Count == 0 &&
                          selectAccountDetailsModel.AccountFriendsPair.Count == 0))
                    {
                        selectAccountDetailsModel.SelectedAccountIds.Add(x.AccountId);
                        selectAccountDetailsModel.AccountsWithNetwork.Add(
                            new KeyValuePair<SocialNetworks, string>(x.SocialNetworks, x.AccountId));
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
    ///     Interaction logic for PageInviter.xaml
    /// </summary>
    public partial class PageInviter
    {
        private PageInviter()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: PageInviterFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.PageInviter,
                moduleName: FdMainModule.Inviter.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.PageInviterVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.PageInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static PageInviter CurrentPageInviter { get; set; }

        public static PageInviter GetSingeltonObjectPageInviter()
        {
            return CurrentPageInviter ?? (CurrentPageInviter = new PageInviter());
        }
    }
}