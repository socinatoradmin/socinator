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
using TwtDominatorCore.TDViewModel.GrowFollower;

namespace TwtDominatorUI.TDViews.Tools.Follow
{
    public class FollowerConfigurationBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
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
                    ObjViewModel.FollowerModel =
                        templateModel.ActivitySettings.GetActivityModel<FollowerModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new FollowerViewModel();

                ObjViewModel.FollowerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for FollowConfiguration.xaml
    /// </summary>
    public partial class FollowConfiguration : FollowerConfigurationBase
    {
        //public ObservableCollectionBase<string> lstAccounts = null;

        public FollowConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Follow,
                Enums.TdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FollowConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.FollowVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.FollowVideoTutorialsLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}