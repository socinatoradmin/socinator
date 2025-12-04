using System;
using System.Collections.Generic;
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

namespace TwtDominatorUI.TDViews.Tools.FollowBack
{
    public class FollowBackConfigurationBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
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
    ///     Interaction logic for FollowBackConfiguration.xaml
    /// </summary>
    public partial class FollowBackConfiguration : FollowBackConfigurationBase
    {
        private List<AccountModel> AccountDetails;
        public ObservableCollectionBase<string> lstAccounts = null;

        public FollowBackConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.FollowBack,
                Enums.TdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.FollowBackVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.FollowKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}