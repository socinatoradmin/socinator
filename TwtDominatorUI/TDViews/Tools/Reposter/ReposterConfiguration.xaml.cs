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
using TwtDominatorCore.TDViewModel.TwtBlaster;

namespace TwtDominatorUI.TDViews.Tools.Reposter
{
    public class ReposterConfigBase : ModuleSettingsUserControl<ReposterViewModel, ReposterModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ReposterModel =
                        templateModel.ActivitySettings.GetActivityModel<ReposterModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new ReposterViewModel();

                ObjViewModel.ReposterModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for ReposterConfiguration.xaml
    /// </summary>
    public partial class ReposterConfiguration : ReposterConfigBase
    {
        public ReposterConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            //ListQueryType

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Reposter,
                Enums.TdMainModule.TwtBlaster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ReposterConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.ReposterVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.ReposterKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}