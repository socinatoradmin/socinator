using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.SendMessageToNewFriends
{
    public class
        SendMessageToNewFriendsToolsBase : ModuleSettingsUserControl<MessageRecentFriendsViewModel,
            MessageRecentFriendsModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneMessage".FromResourceDictionary());
                return false;
            }

            if (Model.DaysBefore.StartValue == 0 && Model.DaysBefore.EndValue == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidSourceFilter".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.MessageRecentFriendsModel =
                        templateModel.ActivitySettings.GetActivityModelNonQueryList<MessageRecentFriendsModel>(
                            ObjViewModel.Model);
                else
                    ObjViewModel = new MessageRecentFriendsViewModel();
                ObjViewModel.MessageRecentFriendsModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for SendMessageToNewFriends.xaml
    /// </summary>
    public partial class SendMessageToNewFriendsTools
    {
        public SendMessageToNewFriendsTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendMessageToNewFriends,
                FdMainModule.Messanger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.SendMessageToNewFriendVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.MessageToNewFriendsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }


        private static SendMessageToNewFriendsTools CurrentSendMessageToNewFriendsTools { get; set; }

        public static SendMessageToNewFriendsTools GetSingeltonObjectSendMessageToNewFriendsTools()
        {
            return CurrentSendMessageToNewFriendsTools ??
                   (CurrentSendMessageToNewFriendsTools = new SendMessageToNewFriendsTools());
        }
    }
}