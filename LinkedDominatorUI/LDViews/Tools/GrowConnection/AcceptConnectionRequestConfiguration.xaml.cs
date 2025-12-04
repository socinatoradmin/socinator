using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.GrowConnection
{
    public class AcceptConnectionRequestConfigurationBase : ModuleSettingsUserControl<AcceptConnectionRequestViewModel,
        AcceptConnectionRequestModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.AcceptConnectionRequestModel =
                        templateModel.ActivitySettings.GetActivityModel<AcceptConnectionRequestModel>(
                            ObjViewModel.Model, true);
                else
                    ObjViewModel = new AcceptConnectionRequestViewModel();
                ObjViewModel.AcceptConnectionRequestModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for AcceptConnectionRequestConfiguration.xaml
    /// </summary>
    public partial class AcceptConnectionRequestConfiguration : AcceptConnectionRequestConfigurationBase
    {
        private AcceptConnectionRequestConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AcceptConnectionRequest,
                LdMainModules.GrowConnection.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AcceptConnectionRequestVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AcceptConnectionRequestKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static AcceptConnectionRequestConfiguration CurrentAcceptConnectionRequestConfiguration { get; set; }

        public static AcceptConnectionRequestConfiguration GetSingeltonObjectAcceptConnectionRequestConfiguration()
        {
            return CurrentAcceptConnectionRequestConfiguration ?? (CurrentAcceptConnectionRequestConfiguration =
                       new AcceptConnectionRequestConfiguration());
        }
    }
}