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

namespace FaceDominatorUI.FDViews.Tools.EventInviter
{
    public class EventInviterToolsBase : ModuleSettingsUserControl<EventInviterViewModel, EventInviterModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.InviterDetailsModel.ListEventUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterEventUrl".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.EventInviterModel
                        = templateModel.ActivitySettings.GetActivityModelNonQueryList<EventInviterModel>(ObjViewModel
                            .Model);
                else
                    ObjViewModel = new EventInviterViewModel();


                ObjViewModel.EventInviterModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for EventInviterTools.xaml
    /// </summary>
    public partial class EventInviterTools
    {
        public EventInviterTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.EventInviter,
                FdMainModule.Inviter.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.EventInviterVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.EventInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static EventInviterTools CurrentEventInviterTools { get; set; }

        public static EventInviterTools GetSingeltonObjectEventInviterTools()
        {
            return CurrentEventInviterTools ?? (CurrentEventInviterTools = new EventInviterTools());
        }

        private void EventInviterFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        {
            SelectAccountDetailsControl selectAccountDetailsControl;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Group, FbEntityTypes.Page, FbEntityTypes.CustomDestination
            };


            if (ObjViewModel.EventInviterModel.SelectAccountDetailsModel.AccountsWithNetwork.Count != 0)
            {
                var newModel = ObjViewModel.EventInviterModel.SelectAccountDetailsModel.DeepCloneObject();

                newModel.IsDisplaySingleAccount = true;

                newModel.DisplayAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;

                newModel.CustomDestinationColWidth = "0";

                selectAccountDetailsControl = new SelectAccountDetailsControl(newModel);
            }
            else
            {
                var displayAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;
                selectAccountDetailsControl =
                    new SelectAccountDetailsControl(hiddenColumnList, displayAccount, true, string.Empty, true);
            }

            var objDialog = new Dialog();

            var window = objDialog.GetMetroWindow(selectAccountDetailsControl,
                "LangKeySelectAccountDetails".FromResourceDictionary());

            window.Closed += (senders, events) => { ObjViewModel.EventInviterModel.IsSelctDetails = false; };

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                var model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                model = SaveDestinationExecute(model);

                ObjViewModel.EventInviterModel.SelectAccountDetailsModel = model;

                ObjViewModel.EventInviterModel.IsSelctDetails = false;


                window.Close();
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
                    "LangKeyWarning".FromResourceDictionary(),
                    "LangKeyPleaseSelectAccountsSelectedOnlyDestinations".FromResourceDictionary());
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
                    "LangKeyWarning".FromResourceDictionary(), "LangKeySelectDestination".FromResourceDictionary());
                return new SelectAccountDetailsModel();
            }

            return selectAccountDetailsModel;
        }
    }
}