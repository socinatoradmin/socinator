using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinMessenger;
using System;
using System.Windows;

namespace PinDominator.PDViews.Tools.SendMessageToNewFollowers
{
    public class SendMessageToNewFollowersConfigurationBase : ModuleSettingsUserControl<
        SendMessageToNewFollowersViewModel, SendMessageToNewFollowersModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.LstMessages.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one message.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SendMessageToNewFollowersModel =
                        templateModel.ActivitySettings.GetActivityModel<SendMessageToNewFollowersModel>(
                            ObjViewModel.Model, true);
                else if (ObjViewModel == null)
                    ObjViewModel = new SendMessageToNewFollowersViewModel();

                ObjViewModel.SendMessageToNewFollowersModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for SendMessageToNewFollowersConfiguration.xaml
    /// </summary>
    public partial class SendMessageToNewFollowersConfiguration
    {
        public SendMessageToNewFollowersConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.SendMessageToFollower,
                Enums.PdMainModule.PinMessenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SendMessageToNewFollowersVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SendMessageToNewFollowersKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static SendMessageToNewFollowersConfiguration CurrentSendMessageToNewFollowersConfiguration
        {
            get;
            set;
        }

        /// <summary>
        ///     GetSingletonObjectSendMessageToNewFollowersConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static SendMessageToNewFollowersConfiguration GetSingletonObjectSendMessageToNewFollowersConfiguration()
        {
            return CurrentSendMessageToNewFollowersConfiguration ?? (CurrentSendMessageToNewFollowersConfiguration =
                       new SendMessageToNewFollowersConfiguration());
        }
    }
}