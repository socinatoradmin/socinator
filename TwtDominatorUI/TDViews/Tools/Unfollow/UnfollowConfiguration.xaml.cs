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

namespace TwtDominatorUI.TDViews.Tools.Unfollow
{
    public class UnFollowConfigurationBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!(Model.Unfollower.IsChkPeopleFollowedBySoftwareCheecked ||
                  Model.Unfollower.IsChkPeopleFollowedOutsideSoftwareChecked ||
                  Model.Unfollower.IsChkCustomUsersListChecked && !string.IsNullOrEmpty(Model.Unfollower.CustomUsers)))
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one unfollow source");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UnfollowerModel =
                        templateModel.ActivitySettings.GetActivityModel<UnfollowerModel>(ObjViewModel.Model, true);
                else if (ObjViewModel == null)
                    ObjViewModel = new UnfollowerViewModel();

                ObjViewModel.UnfollowerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for UnfollowConfiguration.xaml
    /// </summary>
    public partial class UnfollowConfiguration : UnFollowConfigurationBase
    {
        private List<AccountModel> AccountDetails;
        public ObservableCollectionBase<string> lstAccounts = null;

        public UnfollowConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unfollow,
                Enums.TdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.UnFollowVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.UnFollowVideoTutorialsLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}