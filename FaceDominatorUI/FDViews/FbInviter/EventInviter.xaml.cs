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
    public class EventInviterBase : ModuleSettingsUserControl<EventInviterViewModel, EventInviterModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.InviterDetailsModel.ListEventUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterEventUrl".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for EventInviter.xaml
    /// </summary>
    public partial class EventInviter
    {
        private EventInviter()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: EventInviterFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.EventInviter,
                moduleName: FdMainModule.Inviter.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.EventInviterVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.EventInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static EventInviter CurrentEventInviter { get; set; }

        public static EventInviter GetSingeltonObjectEventInviter()
        {
            return CurrentEventInviter ?? (CurrentEventInviter = new EventInviter());
        }


        public override void SelectAccount()
        {
            SelectAccountDetailsControl selectAccountDetailsControl;


            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Group, FbEntityTypes.Page, FbEntityTypes.CustomDestination
            };


            var listAccountIds = ObjViewModel.EventInviterModel.SelectAccountDetailsModel.AccountsWithNetwork
                .Where(x => x.Key == SocialNetworks.Facebook).Select(y => y.Value).ToList();


            if (_footerControl.list_SelectedAccounts.Count != listAccountIds.Count)
            {
                var accountDetailsModel = ObjViewModel.EventInviterModel.SelectAccountDetailsModel;
                //accountDetailsModel = RemoveUnnecessaryAccountsAsync(accountDetailsModel, _footerControl.list_SelectedAccounts);
                ObjViewModel.EventInviterModel.SelectAccountDetailsModel = accountDetailsModel;
            }


            if (ObjViewModel.EventInviterModel.SelectAccountDetailsModel.AccountsWithNetwork.Count != 0)
            {
                ObjViewModel.EventInviterModel.SelectAccountDetailsModel.CustomDestinationColWidth = "0";
                selectAccountDetailsControl =
                    new SelectAccountDetailsControl(ObjViewModel.EventInviterModel.SelectAccountDetailsModel, true);
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

                    ObjViewModel.EventInviterModel.SelectAccountDetailsModel = model;

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
    }
}