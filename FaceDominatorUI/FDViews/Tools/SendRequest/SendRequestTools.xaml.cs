using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FDViewModel.FriendsViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.SendRequest
{
    public class SendRequestToolsBase : ModuleSettingsUserControl<SendFrinedRequestViewModel, SendFriendRequestModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.ChkCommentOnUserLatestPostsChecked)
                if (string.IsNullOrEmpty(Model.UploadComment))
                {
                    Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                        "LangKeyAddCommentSaveIt".FromResourceDictionary());
                    return false;
                }

            if (Model.ChkSendDirectMessageAfterFollowChecked)
                if (string.IsNullOrEmpty(Model.Message))
                {
                    Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                        "LangKeyTypeMessageSaveIt".FromResourceDictionary());
                    return false;
                }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendFriendRequestModel
                        = templateModel.ActivitySettings.GetActivityModel<SendFriendRequestModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new SendFrinedRequestViewModel();
                ObjViewModel.SendFriendRequestModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for SendRequestTools.xaml
    /// </summary>
    public partial class SendRequestTools
    {
        public SendRequestTools()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendFriendRequest,
                FdMainModule.Friends.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FindFriendsSearchControl
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.SendRequestVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.SendRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }
    }
}