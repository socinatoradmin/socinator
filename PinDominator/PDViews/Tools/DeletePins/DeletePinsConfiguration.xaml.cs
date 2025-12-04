using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.DeletePins;
using System;
using System.Linq;
using System.Windows;

namespace PinDominator.PDViews.Tools.DeletePins
{
    public class DeletePinsConfigurationBase : ModuleSettingsUserControl<DeletePinsViewModel, DeletePinModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.IsDeteleAllPins && !Model.IsDeteleAllPinsFromBoard)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one delete source");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.DeletePinModel =
                        templateModel.ActivitySettings.GetActivityModel<DeletePinModel>(ObjViewModel.Model, true);
                else if (ObjViewModel == null)
                    ObjViewModel = new DeletePinsViewModel();

                ObjViewModel.DeletePinModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for DeletePinsConfiguration.xaml
    /// </summary>
    public partial class DeletePinsConfiguration 
    {
        public DeletePinsConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.DeletePin,
                Enums.PdMainModule.Poster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.DeletePinsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.DeletePinsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static DeletePinsConfiguration CurrentDeletePinsConfiguration { get; set; }

        public static DeletePinsConfiguration GetSingeltonObjectDeletePinsConfiguration()
        {
            return CurrentDeletePinsConfiguration ?? (CurrentDeletePinsConfiguration = new DeletePinsConfiguration());
        }

        private void DeteleAllPins_Click(object sender, RoutedEventArgs e)
        {
            ObjViewModel.DeletePinModel.IsDeteleAllPinsFromBoard = false;
        }

        private void DetelePinsOfBoards_Click(object sender, RoutedEventArgs e)
        {
            ObjViewModel.DeletePinModel.IsDeteleAllPins = false;
            var objBoards = new PinDominatorCore.PDModel.Boards();
            try
            {
                var accountsFileManager =
                    InstanceProvider.GetInstance<IAccountsFileManager>();
                var accountModel = accountsFileManager.GetAccount(AccountGrowthHeader.SelectedItem, SocialNetwork);
                IDbAccountService dbAccountService = new DbAccountService(accountModel);
                var boardDetails = dbAccountService.GetOwnBoards().ToList();

                objBoards.Account = AccountGrowthHeader.SelectedItem;
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

            //ObjViewModel.DeletePinModel.LstBoardsDetails.Clear();
            if (!ObjViewModel.DeletePinModel.LstBoardsDetails.Any(x => x.Account == AccountGrowthHeader.SelectedItem))
                ObjViewModel.DeletePinModel.LstBoardsDetails.Add(objBoards);
        }
    }
}