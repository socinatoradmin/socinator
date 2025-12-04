using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinMessenger;
using System;

namespace PinDominator.PDViews.Tools.BroadCastMessage
{
    public class BroadCastMessageConfigurationBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.BroadcastMessagesModel =
                        templateModel.ActivitySettings.GetActivityModel<BroadcastMessagesModel>(ObjViewModel.Model);
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

    /// <summary>
    ///     Interaction logic for BroadCastMessageConfiguration.xaml
    /// </summary>
    public partial class BroadCastMessageConfiguration
    {
        public BroadCastMessageConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.BroadcastMessages,
                Enums.PdMainModule.PinMessenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: BroadCastMessageConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BroadCastMessageVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BroadCastMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static BroadCastMessageConfiguration CurrentBroadCastMessageConfiguration { get; set; }

        /// <summary>
        ///     GetSingletonObjectBroadCastMessageConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static BroadCastMessageConfiguration GetSingletonObjectBroadCastMessageConfiguration()
        {
            return CurrentBroadCastMessageConfiguration ??
                   (CurrentBroadCastMessageConfiguration = new BroadCastMessageConfiguration());
        }
    }
}