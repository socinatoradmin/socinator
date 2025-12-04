using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Windows;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.GrowFollower;

namespace TumblrDominatorUI.TumblrView.Activity.Unfollow
{
    public class UnfollowConfigurationBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.IsChkPeopleFollowedBySoftwareChecked && !Model.IsChkPeopleFollowedOutsideSoftwareChecked &&
                !Model.IsChkCustomUsersListChecked)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one Unfollow source");
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
                        JsonConvert.DeserializeObject<UnfollowerModel>(templateModel.ActivitySettings);
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
    ///     Interaction logic for UnfollowConfiguration.xaml
    /// </summary>
    public partial class UnfollowConfiguration
    {
        public UnfollowConfiguration()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unfollow,
                Enums.TmbMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            VideoTutorialLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
            DialogParticipation.SetRegister(this, this);
        }

        private void InputBoxControl_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            ObjViewModel.UnfollowerModel.CustomUsersList = CustomUsersListInputBox.InputText;
        }
    }
}