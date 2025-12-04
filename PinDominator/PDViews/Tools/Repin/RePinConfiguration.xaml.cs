using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominator.CustomControl;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinPoster;
using System;
using System.Linq;
using System.Windows;

namespace PinDominator.PDViews.Tools.Repost
{
    public class RePinConfigurationBase : ModuleSettingsUserControl<RePinViewModel, RePinModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (ObjViewModel.RePinModel.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.RePinModel.AccountPagesBoardsPair == null ||
                ObjViewModel.RePinModel.AccountPagesBoardsPair.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please select at least one board.");
                return false;
            }

            if (ObjViewModel.RePinModel.ChkCommentOnPinAfterRepinChecked &&
                ObjViewModel.RePinModel.LstComments.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one comment in after repin action.");
                return false;
            }

            if (ObjViewModel.RePinModel.ChkTryOnPinAfterRepinChecked && ObjViewModel.RePinModel.LstNotes.Count == 0)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please add at least one try text in after repin action.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.RePinModel =
                        templateModel.ActivitySettings.GetActivityModel<RePinModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new RePinViewModel();

                ObjViewModel.RePinModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for RePinConfiguration.xaml
    /// </summary>
    public partial class RePinConfiguration
    {
        private Window _window;
        public ObservableCollectionBase<string> LstAccounts = null;

        public RePinConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Repin,
                Enums.PdMainModule.Poster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: RePinConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.RePinVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.RePinKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        public BoardCreateDestination BoardCreateDestination { get; set; }

        private static RePinConfiguration CurrentRePinConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF RePinConfiguration
        /// </summary>
        /// <returns></returns>
        public static RePinConfiguration GetSingeltonObjectRePinConfiguration()
        {
            return CurrentRePinConfiguration ?? (CurrentRePinConfiguration = new RePinConfiguration());
        }

        private void accountGrowthHeader_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ObjViewModel.RePinModel.LstBoardsDetails.Clear();
            SetAccountModeDataContext(SocialNetworks.Pinterest);
            var objBoards = new PinDominatorCore.PDModel.Boards();
            try
            {
                var account = AccountGrowthHeader.SelectedItem;
                var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
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

            ObjViewModel.RePinModel.LstBoardsDetails.Add(objBoards);
        }

        private void SelectBoards_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BoardCreateDestination = new BoardCreateDestination(ObjViewModel, AccountGrowthHeader.SelectedItem);
                BoardCreateDestination.InitializeProperties();
                BoardCreateDestination.BtnSave.Click += BtnSaveEvent;

                BoardCreateDestination.BoardCreateDestinationsViewModel.PublisherCreateDestinationModel =
                    new BoardCreateDestinationModel();
                _window = new Dialog().GetMetroWindow(BoardCreateDestination, "Select Boards");
                _window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnSaveEvent(object sender, EventArgs e)
        {
            try
            {
                BoardCreateDestination.BtnSaveEvent(sender, e);
                _window.Close();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Model.MediaPath = string.Empty;
        }
    }
}