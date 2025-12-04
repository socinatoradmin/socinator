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

namespace FaceDominatorUI.FDViews.Tools.PageInviter
{
    public class PageInviterToolsBase : ModuleSettingsUserControl<PageInviterViewModel, FanpageInviterModel>
    {
        protected override bool ValidateExtraProperty()
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

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.FanpageInviterModel
                        = templateModel.ActivitySettings.GetActivityModelNonQueryList<FanpageInviterModel>(ObjViewModel
                            .Model);
                else
                    ObjViewModel = new PageInviterViewModel();
                ObjViewModel.FanpageInviterModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for PageInviterTools.xaml
    /// </summary>
    public partial class PageInviterTools
    {
        private PageInviterTools()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.PageInviter,
                FdMainModule.Inviter.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.PageInviterVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.PageInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
        }

        private static PageInviterTools CurrentPageInviter { get; set; }

        public static PageInviterTools GetSingeltonObjectPageInviter()
        {
            return CurrentPageInviter ?? (CurrentPageInviter = new PageInviterTools());
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

        private void SelectAccountDetails_Click(object sender, RoutedEventArgs e)
        {
            SelectAccountDetailsControl selectAccountDetailsControl;

            var hiddenColumnList = new List<FbEntityTypes>
            {
                FbEntityTypes.Group /*FbEntityTypes.CustomDestination*/
            };


            if (!ObjViewModel.FanpageInviterModel.InviterDetailsModel.IsProfileUrl)
                hiddenColumnList.Add(FbEntityTypes.Friend);


            if (ObjViewModel.FanpageInviterModel.SelectAccountDetailsModel.AccountsWithNetwork.Count != 0)
            {
                var newModel = ObjViewModel.FanpageInviterModel.SelectAccountDetailsModel.DeepCloneObject();

                newModel.IsDisplaySingleAccount = true;

                newModel.DisplayAccount = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount;

                newModel.CustomDestinationColWidth =
                    !ObjViewModel.FanpageInviterModel.InviterDetailsModel.IsProfileUrl ? "0" : "130";

                newModel.FriendColWidth =
                    !ObjViewModel.FanpageInviterModel.InviterDetailsModel.IsProfileUrl ? "0" : "130";

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

            window.Closed += (senders, events) => { ObjViewModel.FanpageInviterModel.IsSelctDetails = false; };

            selectAccountDetailsControl.BtnSave.Click += (senders, events) =>
            {
                try
                {
                    var model = selectAccountDetailsControl.SelectAccountDetailsViewModel.SelectAccountDetailsModel;

                    model = SaveDestinationExecute(model);

                    //model = selectAccountDetailsControl.GetPageInviterDetails(model);

                    ObjViewModel.FanpageInviterModel.SelectAccountDetailsModel = model;


                    ObjViewModel.FanpageInviterModel.IsSelctDetails = false;

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