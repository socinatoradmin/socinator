using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtMessenger;

namespace TwtDominatorUI.TDViews.Tools.Message
{
    public class BroadCastMessageConfigBase : ModuleSettingsUserControl<BroadCastMessageViewModel, MessageModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.MessageSetting.IsChkRandomFollowers && !Model.MessageSetting.IsChkCustomFollowers)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one Message source");
                return false;
            }

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please enter atleast one message");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.MessageModel =
                        templateModel.ActivitySettings.GetActivityModel<MessageModel>(ObjViewModel.Model, true);
                else if (ObjViewModel == null)
                    ObjViewModel = new BroadCastMessageViewModel();

                ObjViewModel.MessageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for BroadCastMessageConfig.xaml
    /// </summary>
    public partial class BroadCastMessageConfig : BroadCastMessageConfigBase
    {
        public BroadCastMessageConfig()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BroadcastMessages,
                Enums.TdMainModule.TwtMessenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtMessengerVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtMessengerKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            //  MessageViewModel.IsBroadCastMessageModule = true;
        }
    }
}