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
    public class AutoReplyConfigBase : ModuleSettingsUserControl<MessageViewModel, MessageModel>
    {
        protected override bool ValidateExtraProperty()
        {
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
                    ObjViewModel = new MessageViewModel();

                ObjViewModel.MessageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for AutoReplyConfig.xaml
    /// </summary>
    public partial class AutoReplyConfig : AutoReplyConfigBase
    {
        public AutoReplyConfig()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AutoReplyToNewMessage,
                Enums.TdMainModule.TwtMessenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtReplyVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtReplyKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}