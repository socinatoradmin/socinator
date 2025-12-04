using DominatorHouseCore.Enums;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.GrowFollower;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorUI.TDViews.GrowFollowers
{
    public class MuteUsersBase : ModuleSettingsUserControl<MuteViewModel, MuteModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }
    }

    /// <summary>
    ///     Interaction logic for MuteUsers.xaml
    /// </summary>
    public partial class MuteUsers : MuteUsersBase
    {
        private MuteUsers()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: MuteHeader,
                footer: MuteFooter,
                queryControl: MuteSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Mute,
                moduleName: TdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.MuteVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.MuteKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region SingletonObject Creation

        private static MuteUsers _objMuteUsers;

        public static MuteUsers GetSingletonObjectMuteUsers()
        {
            return _objMuteUsers ?? (_objMuteUsers = new MuteUsers());
        }

        #endregion
    }
}