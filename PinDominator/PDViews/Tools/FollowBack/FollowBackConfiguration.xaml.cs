using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.GrowFollower;
using System;

namespace PinDominator.PDViews.Tools.FollowBack
{
    public class FollowBackConfigurationBase : ModuleSettingsUserControl<FollowBackViewModel, FollowBackModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.FollowBackModel =
                        templateModel.ActivitySettings.GetActivityModel<FollowBackModel>(ObjViewModel.Model, true);
                else if (ObjViewModel == null)
                    ObjViewModel = new FollowBackViewModel();

                ObjViewModel.FollowBackModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for FollowBackConfiguration.xaml
    /// </summary>
    public partial class FollowBackConfiguration
    {
        public FollowBackConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.FollowBack,
                Enums.PdMainModule.PinMessenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BoardScraperVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BoardScraperKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static FollowBackConfiguration CurrentFollowBackConfiguration { get; set; }

        /// <summary>
        ///     GetSingletonObjectFollowBackConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static FollowBackConfiguration GetSingletonObjectFollowBackConfiguration()
        {
            return CurrentFollowBackConfiguration ?? (CurrentFollowBackConfiguration = new FollowBackConfiguration());
        }
    }
}