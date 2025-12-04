using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.GrowFollower;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.Unfollow
{
    public class UnFollowConfigurationBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.IsUnfollowFollowings)
            {
                Model.IsChkCustomFollowUsersListChecked = false;
                Model.RemoveAllFollowUsers = false;
            }

            if (Model.IsUnfollowFollowers)
            {
                Model.IsChkPeopleFollowedBySoftwareChecked = false;
                Model.IsChkPeopleFollowedOutsideSoftwareChecked = false;
                Model.IsChkCustomUsersListChecked = false;
            }

            if (Model.IsUnfollowFollowings && !Model.IsChkPeopleFollowedBySoftwareChecked &&
                !Model.IsChkPeopleFollowedOutsideSoftwareChecked &&
                !Model.IsChkCustomUsersListChecked)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one Unfollow source option...");
                return false;
            }

            if (Model.IsUnfollowFollowers && !Model.RemoveAllFollowUsers && !Model.IsChkCustomFollowUsersListChecked)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one Unfollow Remove source option...");
                return false;
            }

            if (Model.IsChkCustomUsersListChecked && string.IsNullOrEmpty(Model.CustomUsersList) ||
                Model.IsChkCustomFollowUsersListChecked && string.IsNullOrEmpty(Model.CustomFollowUsersList))
            {
                Dialog.ShowDialog(this, "Error",
                    "Please enter atleast one custom username");
                return false;
            }

            // Check AutoFollow.Unfollow
            if (Model.IsChkEnableAutoFollowUnfollowChecked)
                if (!Model.IsChkStopUnfollowToolWhenReachedSpecifiedFollowings &&
                    !Model.IsChkWhenFollowerFollowingsGreater)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please select atleast one checkbox option inside AutoFollow/Unfollow feature to  Stat/Stop Unfollow/Follow process.");
                    return false;
                }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.UnfollowerModel =
                        templateModel.ActivitySettings.GetActivityModel<UnfollowerModel>(ObjViewModel.Model, true);
                else
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
    public partial class UnFollowConfiguration : UnFollowConfigurationBase
    {
        public UnFollowConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unfollow,
                Enums.GdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
        }

        private void InputBoxControl_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            ObjViewModel.UnfollowerModel.CustomUsersList = CustomUsersListInputBox.InputText;
        }

        #region Object creation and INotifyPropertyChanged Implementation

        private static UnFollowConfiguration CurrentUnFollowConfiguration { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUnFollowConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UnFollowConfiguration GetSingeltonObjectUnFollowConfiguration()
        {
            return CurrentUnFollowConfiguration ?? (CurrentUnFollowConfiguration = new UnFollowConfiguration());
        }

        #endregion
    }
}