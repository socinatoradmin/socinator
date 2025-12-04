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
    ///     Interaction logic for SendMessageToNewFollowerTab.xaml
    /// </summary>
    public class SendMessageToNewFollowerConfigurationBase : ModuleSettingsUserControl<SendMessageToFollowerViewModel,
        SendMessageToFollowerModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendMessageToFollowerModel =
                        JsonConvert.DeserializeObject<SendMessageToFollowerModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new SendMessageToFollowerViewModel();


                ObjViewModel.SendMessageToFollowerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class SendMessageToNewFollowerConfiguration
    {
        public SendMessageToNewFollowerConfiguration()
        {
            InitializeComponent();
            VideoTutorialLink = ConstantHelpDetails.SendMessageToFollowerVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                Main,
                ActivityType.SendMessageToFollower,
                QdMainModule.Messages.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
        }
    }
}