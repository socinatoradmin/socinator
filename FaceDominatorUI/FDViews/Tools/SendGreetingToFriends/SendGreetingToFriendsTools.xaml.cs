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

namespace FaceDominatorUI.FDViews.Tools.SendGreetingToFriends
{
    public class SendGreetingsToFriendsToolsBase : ModuleSettingsUserControl<SendGreetingsToFriendsViewModel,
        SendGreetingsToFriendsModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneMessage".FromResourceDictionary());
                return false;
            }

            if (Model.IsFilterByDays && Model.DaysBefore.StartValue == 0 && Model.DaysBefore.EndValue == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidDaysFilter".FromResourceDictionary());
                return false;
            }

            if (Model.IsPostToOwnWallChecked && Model.ListPostDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOnePostDescription".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendGreetingsToFriendsModel =
                        templateModel.ActivitySettings.GetActivityModelNonQueryList<SendGreetingsToFriendsModel>(
                            ObjViewModel.Model);
                else
                    ObjViewModel = new SendGreetingsToFriendsViewModel();
                ObjViewModel.SendGreetingsToFriendsModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class SendGreetingToFriendsTools
    {
        public SendGreetingToFriendsTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendGreetingsToFriends,
                FdMainModule.Messanger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.SendGreetingsToFriendVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.SendGreetingsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static SendGreetingToFriendsTools CurrentSendGreetingsToFriendsTools { get; set; }

        public static SendGreetingToFriendsTools GetSingeltonObjectSendGreetingsToFriendsTools()
        {
            return CurrentSendGreetingsToFriendsTools ??
                   (CurrentSendGreetingsToFriendsTools = new SendGreetingToFriendsTools());
        }
    }
}