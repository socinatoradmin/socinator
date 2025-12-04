using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.ViewModel.Messages;

namespace QuoraDominatorUI.QDViews.Activity.Message
{
    /// <summary>
    ///     Interaction logic for MessageConfiguration.xaml
    /// </summary>
    public class
        MessageConfigurationBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.BroadcastMessagesModel =
                        JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new BroadcastMessagesViewModel();


                ObjViewModel.BroadcastMessagesModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class MessageConfiguration
    {
        private static MessageConfiguration _objBroadcastMessages;

        public MessageConfiguration()
        {
            InitializeComponent();
            VideoTutorialLink = ConstantHelpDetails.BroadcastMessagesVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BroadcastMessages,
                QdMainModule.Messages.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: BroadcastMessagesSearchControl
            );
        }

        public static MessageConfiguration GetSingeltonBroadcastMessages()
        {
            return _objBroadcastMessages ?? (_objBroadcastMessages = new MessageConfiguration());
        }
    }
}