using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Profilling;
using LinkedDominatorCore.LDViewModel.Profilling;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Profilling
{
    public class
        ProfileEndorsementBase : ModuleSettingsUserControl<ProfileEndorsementViewModel, ProfileEndorsementModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!ObjViewModel.ProfileEndorsementModel.IsCheckedBySoftware
                && !ObjViewModel.ProfileEndorsementModel.IsCheckedOutSideSoftware
                && !ObjViewModel.ProfileEndorsementModel.IsCheckedLangKeyCustomUserList
            )

            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheConnectionSources".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for ProfileEndorsement.xaml
    /// </summary>
    public partial class ProfileEndorsement : ProfileEndorsementBase
    {
        private ProfileEndorsement()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: ProfileEndorsementHeader,
                footer: ProfileEndorsementFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.ProfileEndorsement,
                moduleName: LdMainModules.Profilling.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ProfileEndorsementVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ProfileEndorsementKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static ProfileEndorsement CurrentProfileEndorsement { get; set; }

        /// <summary>
        ///     GetSingeltonObjectProfileEndorsement is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static ProfileEndorsement GetSingeltonObjectProfileEndorsement()
        {
            return CurrentProfileEndorsement ?? (CurrentProfileEndorsement = new ProfileEndorsement());
        }
    }
}