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
    ///     Interaction logic for AutoReplyToNewMessageConfiguration.xaml
    /// </summary>
    public class AutoReplyToNewMessageConfigurationBase : ModuleSettingsUserControl<AutoReplyToNewMessageViewModel,
        AutoReplyToNewMessageModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.AutoReplyToNewMessageModel =
                        JsonConvert.DeserializeObject<AutoReplyToNewMessageModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new AutoReplyToNewMessageViewModel();


                ObjViewModel.AutoReplyToNewMessageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public partial class AutoReplyToNewMessageConfiguration
    {
        public AutoReplyToNewMessageConfiguration()
        {
            InitializeComponent();
            VideoTutorialLink = ConstantHelpDetails.AutoReplyToNewMessageVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AutoReplyToNewMessage,
                QdMainModule.Messages.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
        }
    }
}