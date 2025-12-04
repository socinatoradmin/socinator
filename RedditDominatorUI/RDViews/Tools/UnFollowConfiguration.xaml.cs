using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using System;

namespace RedditDominatorUI.RDViews.Tools
{
    /// <summary>
    ///     Interaction logic for UnFollowConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public class UnFollowConfigurationBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.IsChkPeopleFollowedBySoftwareChecked && !Model.IsChkPeopleFollowedOutsideSoftwareChecked &&
                !Model.IsChkCustomUsersListChecked)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one Unfollow source option...");
                return false;
            }

            if (Model.IsChkCustomUsersListChecked && string.IsNullOrEmpty(Model.CustomUsersList?.Trim()))
            {
                Dialog.ShowDialog(this, "Error",
                    "Please enter and save atleast one custom username");
                return false;
            }

            // Check AutoFollow.Unfollow
            if (!Model.IsChkEnableAutoFollowUnfollowChecked) return true;
            if (Model.IsChkStopUnfollowToolWhenReachedSpecifiedFollowings ||
                Model.IsChkWhenFollowerFollowingsGreater) return true;
            Dialog.ShowDialog(this, "Input Error",
                "Please select atleast one checkbox option inside AutoFollow/Unfollow feature to  Stat/Stop Unfollow/Follow process.");
            return false;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.UnfollowerModel =
                        JsonConvert.DeserializeObject<UnfollowerModel>(templateModel.ActivitySettings);
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
    ///     Interaction logic for UnFollowConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class UnFollowConfiguration
    {
        public UnFollowConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unfollow,
                Enums.RdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            VideoTutorialLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
        }

        private static UnFollowConfiguration CurrentUnFollowConfiguration { get; set; }

        public static UnFollowConfiguration GetSingeltonObjectUnFollowConfiguration()
        {
            return CurrentUnFollowConfiguration ?? (CurrentUnFollowConfiguration = new UnFollowConfiguration());
        }
    }
}