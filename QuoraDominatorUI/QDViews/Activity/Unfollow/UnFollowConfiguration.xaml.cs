using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.ViewModel.GrowFollower;
using DominatorHouseCore.Utility;

namespace QuoraDominatorUI.QDViews.Activity.Unfollow
{
    public class UnFollowConfigurationBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if ((ObjViewModel.UnfollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked ||
                 ObjViewModel.UnfollowerModel.IsChkPeopleFollowedBySoftwareChecked ||
                 ObjViewModel.UnfollowerModel.IsChkCustomUsersListChecked) &&
                (ObjViewModel.UnfollowerModel.IsWhoDoNotFollowBackChecked ||
                 ObjViewModel.UnfollowerModel.IsWhoFollowBackChecked))
                return true;
            Dialog.ShowDialog(this, "Error",
                "Please select one  Unfollow source and one source type");
            return false;
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
    ///     Interaction logic for UnFollowConfiguration.xaml
    /// </summary>
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
                QdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
        }

        #region Object creation and INotifyPropertyChanged Implementation

        private static UnFollowConfiguration CurrentUnFollowConfiguration { get; set; }

        /// <summary>
        ///     GetSingeltonObjectUnfollow is used to get the object of the current user control,
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