using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.Message;

namespace TumblrDominatorUI.TumblrView.Activity.Message
{
    public class
        BroadcastMessagesConfigBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.BroadcastMessagesModel =
                        JsonConvert.DeserializeObject<BroadcastMessagesModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new BroadcastMessagesViewModel();
                ObjViewModel.BroadcastMessagesModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for BroadcastMessagesConfig.xaml
    /// </summary>
    public partial class BroadcastMessagesConfig
    {
        public BroadcastMessagesConfig()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BroadcastMessages,
                Enums.TmbMainModule.Message.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: BroadcastMessagesSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.BroadcastMessagesVideoTutorialsLink;
        }

        private static BroadcastMessagesConfig CurrentBroadcastMessagesConfig { get; set; }

        public static BroadcastMessagesConfig GetSingeltonObjectSendMessageToFollowerConfig()
        {
            return CurrentBroadcastMessagesConfig ?? (CurrentBroadcastMessagesConfig = new BroadcastMessagesConfig());
        }
    }
}