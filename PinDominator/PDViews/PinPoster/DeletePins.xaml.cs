using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.DeletePins;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinPoster
{
    public class DeletePinsBase : ModuleSettingsUserControl<DeletePinsViewModel, DeletePinModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.IsDeteleAllPinsFromBoard &&
                !((DeletePinModel) Model).LstBoardsDetails.Any(x => x.LstBoards.Any(y => y.IsCheckBoard)))
            {
                Dialog.ShowDialog(this, "Error", "Please select atleast one Board");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for DeletePins.xaml
    /// </summary>
    public sealed partial class DeletePins
    {
        public DeletePins()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: DeletePinsHeader,
                footer: DeletePinsFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.DeletePin,
                moduleName: PdMainModule.Poster.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.DeletePinsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.DeletePinsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static DeletePins CurrentDeletePins { get; set; }

        public static DeletePins GetSingletonObjectDeletePins()
        {
            return CurrentDeletePins ?? (CurrentDeletePins = new DeletePins());
        }

        public override void SelectAccount()
        {
            try
            {
                var objSelectAccountControl = new SelectAccountControl(_footerControl.list_SelectedAccounts);

                var objDialog = new Dialog();

                var window = objDialog.GetMetroWindow(objSelectAccountControl, "Select Account");

                objSelectAccountControl.btnSave.Click += (senders, events) =>
                {
                    var selectedAccount = objSelectAccountControl.GetSelectedAccount();
                    if (selectedAccount.Count > 0)
                    {
                        _footerControl.list_SelectedAccounts = objSelectAccountControl.GetSelectedAccount().ToList();
                        SelectedAccountCount = _footerControl.list_SelectedAccounts.Count +
                                               $" {"LangKeyAccountSelected".FromResourceDictionary()}";
                        GlobusLogHelper.log.Info(Log.SelectedAccount, SocinatorInitialize.ActiveSocialNetwork,
                            CampaignName, _footerControl.list_SelectedAccounts.Count, CampaignName);
                    }
                    else
                    {
                        SelectedAccountCount = ConstantVariable.NoAccountSelected();
                        _footerControl.list_SelectedAccounts = selectedAccount.ToList();
                    }

                    window.Close();
                };

                objSelectAccountControl.btnCancel.Click += (senders, eventArgs) => window.Close();
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            ObjViewModel.DeletePinModel.LstBoardsDetails = new ObservableCollection<PinDominatorCore.PDModel.Boards>
            (ObjViewModel.DeletePinModel.LstBoardsDetails.Where(x =>
                DeletePinsFooter.list_SelectedAccounts.Contains(x.Account)));
            //ObjViewModel.DeletePinModel.LstBoardsDetails.Clear();
            foreach (var account in DeletePinsFooter.list_SelectedAccounts)
            {
                var objBoards = new PinDominatorCore.PDModel.Boards();
                try
                {
                    var accountsFileManager =
                        InstanceProvider.GetInstance<IAccountsFileManager>();
                    var accountModel = accountsFileManager.GetAccount(account, SocialNetwork);
                    IDbAccountService dbAccountService = new DbAccountService(accountModel);
                    var boardDetails = dbAccountService.GetOwnBoards().ToList();

                    objBoards.Account = account;
                    foreach (var board in boardDetails)
                    {
                        var objBoard = new Board {BoardName = board.BoardName, BoardUrl = board.BoardUrl};
                        objBoards.LstBoards.Add(objBoard);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (!ObjViewModel.DeletePinModel.LstBoardsDetails.Any(x => x.Account == account))
                    ObjViewModel.DeletePinModel.LstBoardsDetails.Add(objBoards);
            }
        }

        private void DetelePinsOfBoards_Click(object sender, RoutedEventArgs e)
        {
            ObjViewModel.DeletePinModel.IsDeteleAllPins = false;
            if (DeletePinsFooter.list_SelectedAccounts.Count == 0)
                GlobusLogHelper.log.Info("Please add atleast one account");
        }

        private void DeteleAllPins_Click(object sender, RoutedEventArgs e)
        {
            ObjViewModel.DeletePinModel.IsDeteleAllPinsFromBoard = false;
        }
    }
}