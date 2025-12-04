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

namespace FaceDominatorUI.FDViews.Tools.WatchPartyInviterTools
{
    public class
        WatchPartyInviterToolsBase : ModuleSettingsUserControl<WatchPartyInviterViewModel, WatchPartyInviterModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.WatchPartyInviterModel
                        = templateModel.ActivitySettings.GetActivityModelNonQueryList<WatchPartyInviterModel>(
                            ObjViewModel.Model);

                ObjViewModel.WatchPartyInviterModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class WatchPartyInviterTools
    {
        public WatchPartyInviterTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.WatchPartyInviter,
                FdMainModule.Inviter.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.WatchPartyInviterVideoTutorialLink;
            KnowledgeBaseLink = FdConstants.WatchPartyInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static WatchPartyInviterTools CurrentWatchPartyInviterTools { get; set; }

        public static WatchPartyInviterTools GetSingeltonObjectWatchPartyInviterTools()
        {
            return CurrentWatchPartyInviterTools ?? (CurrentWatchPartyInviterTools = new WatchPartyInviterTools());
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
                    if (selectAccountDetailsModel.AccountFriendsPair.Count > 0 &&
                        Model.InviterDetailsModel.IsProfileUrl)
                    {
                        selectAccountDetailsModel.SelectedAccountIds.Add(x.AccountId);
                        selectAccountDetailsModel.AccountsWithNetwork.Add(
                            new KeyValuePair<SocialNetworks, string>(x.SocialNetworks, x.AccountId));
                    }
                    else if (!Model.InviterDetailsModel.IsProfileUrl)
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
                selectAccountDetailsModel.PublishOwnWallAccount.Count == 0 && Model.InviterDetailsModel.IsProfileUrl
            )
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeySelectDestination".FromResourceDictionary());
                return new SelectAccountDetailsModel();
            }

            return selectAccountDetailsModel;
        }

        private void EventInviterFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        {
            SelectAccountDetailsControl selectAccountDetailsControl;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Group, FbEntityTypes.Page, FbEntityTypes.CustomDestination
            };

            if (!ObjViewModel.WatchPartyInviterModel.InviterDetailsModel.IsProfileUrl)
                hiddenColumnList.Add(FbEntityTypes.Friend);

            if (ObjViewModel.WatchPartyInviterModel.SelectAccountDetailsModel.AccountsWithNetwork.Count != 0)
            {
                var newModel = ObjViewModel.WatchPartyInviterModel.SelectAccountDetailsModel.DeepCloneObject();

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

            window.Closed += (senders, events) => { ObjViewModel.WatchPartyInviterModel.IsSelctDetails = false; };

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    var model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model = SaveDestinationExecute(model);

                    //model = selectAccountDetailsControl.GetPageInviterDetails(model);

                    ObjViewModel.WatchPartyInviterModel.SelectAccountDetailsModel = model;


                    ObjViewModel.WatchPartyInviterModel.IsSelctDetails = false;

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