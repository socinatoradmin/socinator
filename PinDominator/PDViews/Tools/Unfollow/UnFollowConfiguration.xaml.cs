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

namespace PinDominator.PDViews.Tools.Unfollow
{
    public class UnfollowerBase : ModuleSettingsUserControl<UnfollowerViewModel, UnfollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.IsChkPeopleFollowedBySoftwareChecked && !Model.IsChkPeopleFollowedOutsideSoftwareChecked &&
                !Model.IsChkCustomUsersListChecked)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one unfollow source");
                return false;
            }

            if (Model.IsChkCustomUsersListChecked && string.IsNullOrEmpty(Model.CustomUsersList))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyCustomUserListEmpty".FromResourceDictionary());
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
    public partial class UnFollowConfiguration
    {
        public ObservableCollectionBase<string> LstAccounts = null;

        public UnFollowConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unfollow,
                Enums.PdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UnFollowVideoTutorialsLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static UnFollowConfiguration CurrentUnFollowConfiguration { get; set; }

        /// <summary>
        ///     GetSingletonObjectUnFollowConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static UnFollowConfiguration GetSingletonObjectUnFollowConfiguration()
        {
            return CurrentUnFollowConfiguration ?? (CurrentUnFollowConfiguration = new UnFollowConfiguration());
        }
    }
}