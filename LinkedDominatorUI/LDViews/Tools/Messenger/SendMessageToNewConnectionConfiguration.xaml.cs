using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDViewModel.Messenger;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Messenger
{
    public class SendMessageToNewConnectionConfigurationBase : ModuleSettingsUserControl<
        SendMessageToNewConnectionViewModel, SendMessageToNewConnectionModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendMessageToNewConnectionModel =
                        templateModel.ActivitySettings.GetActivityModel<SendMessageToNewConnectionModel>(
                            ObjViewModel.Model, true);
                else
                    ObjViewModel = new SendMessageToNewConnectionViewModel();
                ObjViewModel.SendMessageToNewConnectionModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (string.IsNullOrWhiteSpace(ObjViewModel.SendMessageToNewConnectionModel.Message) && 
               ObjViewModel.SendMessageToNewConnectionModel.LstDisplayManageMessagesModel.Count <= 0)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseEnterAMesageAndPressTheSaveButton".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for SendMessageToNewConnectionConfiguration.xaml
    /// </summary>
    public partial class SendMessageToNewConnectionConfiguration : SendMessageToNewConnectionConfigurationBase
    {
        private SendMessageToNewConnectionConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendMessageToNewConnection,
                LdMainModules.Messenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SendMessageToNewConnectionVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendGreetingsToConnectionsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static SendMessageToNewConnectionConfiguration CurrentSendMessageToNewConnectionConfiguration
        {
            get;
            set;
        }

        public static SendMessageToNewConnectionConfiguration GetSingeltonObjecSendMessageToNewConnectionConfiguration()
        {
            return CurrentSendMessageToNewConnectionConfiguration ?? (CurrentSendMessageToNewConnectionConfiguration =
                       new SendMessageToNewConnectionConfiguration());
        }
    }
}